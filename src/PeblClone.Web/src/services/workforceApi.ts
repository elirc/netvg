import { mockDashboard } from './mockDashboard';
import type { HiringQuote, HiringQuoteRequest, WorkforceDashboard } from './workforceTypes';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? '';

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...init?.headers
    }
  });

  if (!response.ok) {
    const body = await response.text();
    let message = body;
    try {
      const problem = JSON.parse(body) as { detail?: string; title?: string };
      message = problem.detail ?? problem.title ?? body;
    } catch {
      // Some proxies return plain text; keep that useful diagnostic.
    }
    throw new Error(message || `Request failed with status ${response.status}`);
  }

  return response.json() as Promise<T>;
}

export async function getDashboard(): Promise<{ data: WorkforceDashboard; source: 'api' | 'mock' }> {
  try {
    return {
      data: await request<WorkforceDashboard>('/api/workforce/dashboard'),
      source: 'api'
    };
  } catch {
    return {
      data: mockDashboard,
      source: 'mock'
    };
  }
}

export async function createQuote(requestBody: HiringQuoteRequest): Promise<HiringQuote> {
  return request<HiringQuote>('/api/workforce/quotes', {
    method: 'POST',
    body: JSON.stringify(requestBody)
  });
}
