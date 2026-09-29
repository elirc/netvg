import assert from 'node:assert/strict';
import test from 'node:test';
import { RequestGate } from '../src/services/requestGate.ts';

function deferred() {
  let resolve;
  let reject;
  const promise = new Promise((yes, no) => { resolve = yes; reject = no; });
  return { promise, resolve, reject };
}

// This finite harness exercises the real gate with delayed promises. React's
// input handlers and effect cleanup are source-reviewed, not mounted here.
async function complete(gate, pending, commits) {
  const token = gate.next();
  try {
    const value = await pending;
    if (gate.isCurrent(token)) commits.push({ value });
  } catch (error) {
    if (gate.isCurrent(token)) commits.push({ error: error.message });
  } finally {
    if (gate.isCurrent(token)) commits.push({ loading: false });
  }
}

test('a slower old response cannot replace the latest result or loading state', async () => {
  const gate = new RequestGate();
  const first = deferred();
  const second = deferred();
  const commits = [];
  const a = complete(gate, first.promise, commits);
  const b = complete(gate, second.promise, commits);
  second.resolve('current quote');
  await b;
  first.resolve('stale quote');
  await a;
  assert.deepEqual(commits, [{ value: 'current quote' }, { loading: false }]);
});

test('input-edit invalidation suppresses a delayed rejection and allows a fresh retry', async () => {
  const gate = new RequestGate();
  const old = deferred();
  const commits = [];
  const pending = complete(gate, old.promise, commits);
  gate.invalidate();
  old.reject(new Error('outdated failure'));
  await pending;
  assert.deepEqual(commits, []);
  await complete(gate, Promise.resolve('edited-input quote'), commits);
  assert.deepEqual(commits, [{ value: 'edited-input quote' }, { loading: false }]);
});

test('cleanup invalidation suppresses completion without affecting another form instance', async () => {
  const unmounted = new RequestGate();
  const active = new RequestGate();
  const response = deferred();
  const oldCommits = [];
  const activeCommits = [];
  const pending = complete(unmounted, response.promise, oldCommits);
  unmounted.invalidate();
  await complete(active, Promise.resolve('visible quote'), activeCommits);
  response.resolve('unmounted quote');
  await pending;
  assert.deepEqual(oldCommits, []);
  assert.deepEqual(activeCommits, [{ value: 'visible quote' }, { loading: false }]);
});
