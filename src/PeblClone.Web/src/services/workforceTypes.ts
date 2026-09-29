export type EmploymentType = 'EmployerOfRecord' | 'Contractor' | 'PayrollOnly';

export type WorkerStatus = 'Draft' | 'Onboarding' | 'Active' | 'NeedsAttention' | 'Offboarding';

export type ComplianceRiskLevel = 'Low' | 'Medium' | 'High';

export interface WorkforceDashboard {
  summary: WorkforceSummary;
  workers: WorkerSummary[];
  countries: CountryReadiness[];
  complianceAlerts: ComplianceAlert[];
  payrollRuns: PayrollRun[];
  activity: ActivityItem[];
}

export interface WorkforceSummary {
  totalWorkers: number;
  activeCountries: number;
  workersNeedingAttention: number;
  openComplianceItems: number;
  monthlyPayrollUsd: number;
  averageComplianceScore: number;
}

export interface WorkerSummary {
  id: string;
  fullName: string;
  role: string;
  country: string;
  countryIsoCode: string;
  employmentType: EmploymentType;
  status: WorkerStatus;
  monthlyCost: number;
  currency: string;
  onboardingProgress: number;
  requiresAttention: boolean;
}

export interface CountryReadiness {
  isoCode: string;
  name: string;
  region: string;
  complianceScore: number;
  payrollLeadTimeDays: number;
  employerOfRecordAvailable: boolean;
  contractorManagementAvailable: boolean;
  requiredDocuments: string[];
  statutoryBenefits: string[];
}

export interface ComplianceAlert {
  id: string;
  title: string;
  riskLevel: ComplianceRiskLevel;
  country: string;
  owner: string;
  dueDate: string;
}

export interface PayrollRun {
  id: string;
  country: string;
  periodEnd: string;
  approvalDeadline: string;
  grossPayroll: number;
  currency: string;
  workerCount: number;
  isApproved: boolean;
}

export interface ActivityItem {
  id: string;
  title: string;
  description: string;
  eventDate: string;
  category: string;
}

export interface HiringQuoteRequest {
  countryIsoCode: string;
  employmentType: EmploymentType;
  monthlyCompensation: number;
  currency: string;
}

export interface HiringQuote {
  countryIsoCode: string;
  countryName: string;
  employmentType: EmploymentType;
  baseCompensation: number;
  estimatedEmployerCosts: number;
  platformFee: number;
  totalMonthlyCost: number;
  currency: string;
  estimatedOnboardingDays: number;
  complianceRequirements: string[];
}
