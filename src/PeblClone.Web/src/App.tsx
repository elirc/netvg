import { useEffect, useMemo, useState } from 'react';
import { MetricCard } from './components/MetricCard';
import { StatusBadge } from './components/StatusBadge';
import { formatCurrency, formatDate, humanize } from './features/formatters';
import { QuoteBuilder } from './features/QuoteBuilder';
import { getDashboard } from './services/workforceApi';
import type { WorkforceDashboard } from './services/workforceTypes';

export function App() {
  const [dashboard, setDashboard] = useState<WorkforceDashboard | null>(null);
  const [source, setSource] = useState<'api' | 'mock'>('mock');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let isMounted = true;

    getDashboard()
      .then((result) => {
        if (!isMounted) {
          return;
        }

        setDashboard(result.data);
        setSource(result.source);
      })
      .finally(() => {
        if (isMounted) {
          setIsLoading(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, []);

  const urgentAlerts = useMemo(
    () => dashboard?.complianceAlerts.filter((alert) => alert.riskLevel !== 'Low') ?? [],
    [dashboard]
  );

  if (isLoading || !dashboard) {
    return (
      <main className="app-shell">
        <div className="loading-screen">Loading workforce operations...</div>
      </main>
    );
  }

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">A</span>
          <div>
            <strong>AtlasWork</strong>
            <small>Global Work Platform</small>
          </div>
        </div>
        <nav aria-label="Primary">
          <a className="active" href="#overview">
            Overview
          </a>
          <a href="#workers">Workers</a>
          <a href="#payroll">Payroll</a>
          <a href="#compliance">Compliance</a>
          <a href="#quote">Hire</a>
        </nav>
        <div className="sidebar-footer">
          <StatusBadge label={source === 'api' ? 'Api' : 'Mock'} />
          <span>{source === 'api' ? 'Connected to API' : 'Demo fallback active'}</span>
        </div>
      </aside>

      <section className="workspace">
        <header className="topbar" id="overview">
          <div>
            <p className="eyebrow">Global operations command center</p>
            <h1>Hire, pay, and manage distributed teams</h1>
          </div>
          <button type="button">Start hiring</button>
        </header>

        <section className="metrics-grid" aria-label="Workforce metrics">
          <MetricCard
            label="Total workers"
            value={dashboard.summary.totalWorkers.toString()}
            detail={`${dashboard.summary.activeCountries} active countries`}
          />
          <MetricCard
            label="Monthly payroll"
            value={formatCurrency(dashboard.summary.monthlyPayrollUsd)}
            detail="Consolidated USD view"
          />
          <MetricCard
            label="Compliance score"
            value={`${dashboard.summary.averageComplianceScore}%`}
            detail={`${dashboard.summary.openComplianceItems} open items`}
          />
          <MetricCard
            label="Needs attention"
            value={dashboard.summary.workersNeedingAttention.toString()}
            detail="Worker or country risk"
          />
        </section>

        <div className="content-grid">
          <section className="panel span-2" id="workers">
            <div className="panel-heading">
              <div>
                <p className="eyebrow">Workforce</p>
                <h2>Active worker portfolio</h2>
              </div>
              <button type="button" className="secondary-button">
                Export
              </button>
            </div>

            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Worker</th>
                    <th>Country</th>
                    <th>Type</th>
                    <th>Status</th>
                    <th>Onboarding</th>
                    <th>Monthly cost</th>
                  </tr>
                </thead>
                <tbody>
                  {dashboard.workers.map((worker) => (
                    <tr key={worker.id}>
                      <td>
                        <strong>{worker.fullName}</strong>
                        <span>{worker.role}</span>
                      </td>
                      <td>
                        {worker.country} <small>{worker.countryIsoCode}</small>
                      </td>
                      <td>{humanize(worker.employmentType)}</td>
                      <td>
                        <StatusBadge label={worker.status} />
                      </td>
                      <td>
                        <div className="progress" aria-label={`${worker.onboardingProgress}% complete`}>
                          <span style={{ width: `${worker.onboardingProgress}%` }} />
                        </div>
                      </td>
                      <td>{formatCurrency(worker.monthlyCost, worker.currency)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </section>

          <section className="panel" id="compliance">
            <div className="panel-heading">
              <div>
                <p className="eyebrow">Risk copilot</p>
                <h2>Recommended actions</h2>
              </div>
            </div>
            <div className="alert-list">
              {urgentAlerts.map((alert) => (
                <article key={alert.id} className="alert-item">
                  <StatusBadge label={alert.riskLevel} />
                  <strong>{alert.title}</strong>
                  <span>
                    {alert.country} - {alert.owner} - due {formatDate(alert.dueDate)}
                  </span>
                </article>
              ))}
            </div>
          </section>

          <section className="panel span-2 country-map">
            <div className="panel-heading">
              <div>
                <p className="eyebrow">Country readiness</p>
                <h2>Launch coverage</h2>
              </div>
            </div>
            <div className="country-grid">
              {dashboard.countries.map((country) => (
                <article key={country.isoCode} className="country-card">
                  <div>
                    <strong>{country.name}</strong>
                    <span>{country.region}</span>
                  </div>
                  <b>{country.complianceScore}%</b>
                  <small>{country.payrollLeadTimeDays} day payroll lead time</small>
                  <div className="pill-row">
                    {country.employerOfRecordAvailable ? <span>EOR</span> : null}
                    {country.contractorManagementAvailable ? <span>Contractors</span> : null}
                  </div>
                </article>
              ))}
            </div>
          </section>

          <section className="panel" id="payroll">
            <div className="panel-heading">
              <div>
                <p className="eyebrow">Payroll</p>
                <h2>Approval queue</h2>
              </div>
            </div>
            <div className="payroll-list">
              {dashboard.payrollRuns.map((run) => (
                <article key={run.id}>
                  <div>
                    <strong>{run.country}</strong>
                    <span>Deadline {formatDate(run.approvalDeadline)}</span>
                  </div>
                  <div>
                    <b>{formatCurrency(run.grossPayroll, run.currency)}</b>
                    <StatusBadge label={run.isApproved ? 'Approved' : 'Pending'} />
                  </div>
                </article>
              ))}
            </div>
          </section>

          <div id="quote">
            <QuoteBuilder countries={dashboard.countries} />
          </div>
        </div>
      </section>
    </main>
  );
}
