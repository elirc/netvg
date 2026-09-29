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
    const message = await response.text();
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
  try {
    return await request<HiringQuote>('/api/workforce/quotes', {
      method: 'POST',
      body: JSON.stringify(requestBody)
    });
  } catch (error) {
    const country = mockDashboard.countries.find((item) => item.isoCode === requestBody.countryIsoCode);

    if (!country) {
      throw error;
    }

    const employerCostRate =
      requestBody.employmentType === 'EmployerOfRecord'
        ? 0.21
        : requestBody.employmentType === 'Contractor'
          ? 0.06
          : 0.09;
    const platformFee =
      requestBody.employmentType === 'EmployerOfRecord'
        ? 699
        : requestBody.employmentType === 'Contractor'
          ? 149
          : 299;
    const estimatedEmployerCosts = Number((requestBody.monthlyCompensation * employerCostRate).toFixed(2));

    return {
      countryIsoCode: country.isoCode,
      countryName: country.name,
      employmentType: requestBody.employmentType,
      baseCompensation: requestBody.monthlyCompensation,
      estimatedEmployerCosts,
      platformFee,
      totalMonthlyCost: requestBody.monthlyCompensation + estimatedEmployerCosts + platformFee,
      currency: requestBody.currency,
      estimatedOnboardingDays: country.payrollLeadTimeDays + 7,
      complianceRequirements: country.requiredDocuments
    };
  }
}
