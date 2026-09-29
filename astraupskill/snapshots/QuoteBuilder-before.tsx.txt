import { FormEvent, useState } from 'react';
import { createQuote } from '../services/workforceApi';
import type { CountryReadiness, EmploymentType, HiringQuote } from '../services/workforceTypes';
import { formatCurrency } from './formatters';

interface QuoteBuilderProps {
  countries: CountryReadiness[];
}

export function QuoteBuilder({ countries }: QuoteBuilderProps) {
  const [countryIsoCode, setCountryIsoCode] = useState(countries[0]?.isoCode ?? 'DE');
  const [employmentType, setEmploymentType] = useState<EmploymentType>('EmployerOfRecord');
  const [monthlyCompensation, setMonthlyCompensation] = useState(9500);
  const [quote, setQuote] = useState<HiringQuote | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsLoading(true);

    try {
      const nextQuote = await createQuote({
        countryIsoCode,
        employmentType,
        monthlyCompensation,
        currency: 'USD'
      });
      setQuote(nextQuote);
    } finally {
      setIsLoading(false);
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

      <form className="quote-form" onSubmit={handleSubmit}>
        <label>
          Country
          <select value={countryIsoCode} onChange={(event) => setCountryIsoCode(event.target.value)}>
            {countries.map((country) => (
              <option key={country.isoCode} value={country.isoCode}>
                {country.name}
              </option>
            ))}
          </select>
        </label>

        <label>
          Engagement
          <select value={employmentType} onChange={(event) => setEmploymentType(event.target.value as EmploymentType)}>
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
            onChange={(event) => setMonthlyCompensation(Number(event.target.value))}
          />
        </label>

        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Calculating...' : 'Calculate'}
        </button>
      </form>

      {quote ? (
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
