import { FormEvent, useEffect, useRef, useState } from 'react';
import { createQuote } from '../services/workforceApi';
import type { CountryReadiness, EmploymentType, HiringQuote } from '../services/workforceTypes';
import { formatCurrency } from './formatters';
import { RequestGate } from '../services/requestGate';

interface QuoteBuilderProps {
  countries: CountryReadiness[];
}

export function QuoteBuilder({ countries }: QuoteBuilderProps) {
  const [countryIsoCode, setCountryIsoCode] = useState(countries[0]?.isoCode ?? 'DE');
  const [employmentType, setEmploymentType] = useState<EmploymentType>('EmployerOfRecord');
  const [monthlyCompensation, setMonthlyCompensation] = useState(9500);
  const [quote, setQuote] = useState<HiringQuote | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const requestNumber = useRef(new RequestGate());

  useEffect(() => () => { requestNumber.current.invalidate(); }, []);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsLoading(true);
    setError(null);
    setQuote(null);
    const currentRequest = requestNumber.current.next();

    try {
      const nextQuote = await createQuote({
        countryIsoCode,
        employmentType,
        monthlyCompensation,
        currency: 'USD'
      });
      if (requestNumber.current.isCurrent(currentRequest)) setQuote(nextQuote);
    } catch (submitError) {
      if (requestNumber.current.isCurrent(currentRequest)) {
        setError(submitError instanceof Error ? submitError.message : 'The quote could not be calculated.');
      }
    } finally {
      if (requestNumber.current.isCurrent(currentRequest)) setIsLoading(false);
    }
  }

  return (
    <section className="panel quote-panel">
      <div className="panel-heading">
        <div>
          <p className="eyebrow">Instant hiring quote</p>
          <h2>Plan a compliant hire</h2>
        </div>
      </div>

      <form id="quote-form" className="quote-form" onSubmit={handleSubmit}>
        <label>
          Country
          <select value={countryIsoCode} onChange={(event) => { requestNumber.current.invalidate(); setCountryIsoCode(event.target.value); setQuote(null); setError(null); setIsLoading(false); }}>
            {countries.map((country) => (
              <option key={country.isoCode} value={country.isoCode}>
                {country.name}
              </option>
            ))}
          </select>
        </label>

        <label>
          Engagement
          <select value={employmentType} onChange={(event) => { requestNumber.current.invalidate(); setEmploymentType(event.target.value as EmploymentType); setQuote(null); setError(null); setIsLoading(false); }}>
            <option value="EmployerOfRecord">Employer of Record</option>
            <option value="Contractor">Contractor</option>
            <option value="PayrollOnly">Payroll only</option>
          </select>
        </label>

        <label>
          Monthly compensation
          <input
            min="1000"
            step="250"
            type="number"
            value={monthlyCompensation}
            onChange={(event) => { requestNumber.current.invalidate(); setMonthlyCompensation(Number(event.target.value)); setQuote(null); setError(null); setIsLoading(false); }}
          />
        </label>

        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Calculating...' : 'Calculate'}
        </button>
      </form>

      {error ? (
        <div role="alert" className="empty-state">
          <p>{error}</p>
          <button type="submit" form="quote-form" disabled={isLoading}>Retry</button>
        </div>
      ) : quote ? (
        <div className="quote-result">
          <div>
            <span>Total monthly cost</span>
            <strong>{formatCurrency(quote.totalMonthlyCost, quote.currency)}</strong>
          </div>
          <dl>
            <div>
              <dt>Employer costs</dt>
              <dd>{formatCurrency(quote.estimatedEmployerCosts, quote.currency)}</dd>
            </div>
            <div>
              <dt>Platform fee</dt>
              <dd>{formatCurrency(quote.platformFee, quote.currency)}</dd>
            </div>
            <div>
              <dt>Onboarding</dt>
              <dd>{quote.estimatedOnboardingDays} days</dd>
            </div>
          </dl>
          <ul>
            {quote.complianceRequirements.slice(0, 3).map((requirement) => (
              <li key={requirement}>{requirement}</li>
            ))}
          </ul>
        </div>
      ) : (
        <p className="empty-state">Choose a country and engagement type to preview cost, timing, and compliance work.</p>
      )}
    </section>
  );
}
