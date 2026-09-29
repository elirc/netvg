import type { WorkforceDashboard } from './workforceTypes';

export const mockDashboard: WorkforceDashboard = {
  summary: {
    totalWorkers: 5,
    activeCountries: 5,
    workersNeedingAttention: 1,
    openComplianceItems: 5,
    monthlyPayrollUsd: 48150,
    averageComplianceScore: 91
  },
  workers: [
    {
      id: '57d3718c-7f34-48d5-a9ef-13cc9a50c177',
      fullName: 'Mateus Oliveira',
      role: 'Customer Success Lead',
      country: 'Brazil',
      countryIsoCode: 'BR',
      employmentType: 'EmployerOfRecord',
      status: 'NeedsAttention',
      monthlyCost: 8900,
      currency: 'USD',
      onboardingProgress: 67,
      requiresAttention: true
    },
    {
      id: '7398f342-1f9e-48ef-a551-09d225868ce6',
      fullName: 'Aiko Tanaka',
      role: 'Engineering Manager',
      country: 'Japan',
      countryIsoCode: 'JP',
      employmentType: 'EmployerOfRecord',
      status: 'Onboarding',
      monthlyCost: 12800,
      currency: 'USD',
      onboardingProgress: 33,
      requiresAttention: false
    },
    {
      id: 'df8b2ea2-03b7-4c38-9a7b-2595fe17d3f5',
      fullName: 'Nina Schneider',
      role: 'Senior Product Designer',
      country: 'Germany',
      countryIsoCode: 'DE',
      employmentType: 'EmployerOfRecord',
      status: 'Active',
      monthlyCost: 11250,
      currency: 'USD',
      onboardingProgress: 100,
      requiresAttention: false
    },
    {
      id: '92f3ac13-9235-4a8a-a886-13b6aa9ce051',
      fullName: 'Priya Menon',
      role: 'Data Analyst',
      country: 'India',
      countryIsoCode: 'IN',
      employmentType: 'Contractor',
      status: 'Active',
      monthlyCost: 5200,
      currency: 'USD',
      onboardingProgress: 100,
      requiresAttention: false
    },
    {
      id: 'aa6fcbf9-efc5-42d5-9ebe-e7f1b8b4fd52',
      fullName: 'Ethan Brooks',
      role: 'Revenue Operations Manager',
      country: 'Canada',
      countryIsoCode: 'CA',
      employmentType: 'PayrollOnly',
      status: 'Active',
      monthlyCost: 10300,
      currency: 'USD',
      onboardingProgress: 100,
      requiresAttention: false
    }
  ],
  countries: [
    {
      isoCode: 'CA',
      name: 'Canada',
      region: 'North America',
      complianceScore: 96,
      payrollLeadTimeDays: 9,
      employerOfRecordAvailable: true,
      contractorManagementAvailable: true,
      requiredDocuments: ['Government ID', 'Social insurance number', 'Province-specific tax forms'],
      statutoryBenefits: ['CPP/QPP', 'Employment insurance', 'Provincial leave rules']
    },
    {
      isoCode: 'DE',
      name: 'Germany',
      region: 'Europe',
      complianceScore: 94,
      payrollLeadTimeDays: 12,
      employerOfRecordAvailable: true,
      contractorManagementAvailable: true,
      requiredDocuments: ['Passport or national ID', 'Signed local employment agreement', 'Tax identification number'],
      statutoryBenefits: ['Public health insurance', 'Pension contribution', 'Paid annual leave']
    },
    {
      isoCode: 'JP',
      name: 'Japan',
      region: 'Asia Pacific',
      complianceScore: 91,
      payrollLeadTimeDays: 18,
      employerOfRecordAvailable: true,
      contractorManagementAvailable: false,
      requiredDocuments: ['Residence card', 'My Number registration', 'Employment agreement'],
      statutoryBenefits: ['Health insurance', 'Employee pension', 'Employment insurance']
    },
    {
      isoCode: 'BR',
      name: 'Brazil',
      region: 'Latin America',
      complianceScore: 88,
      payrollLeadTimeDays: 15,
      employerOfRecordAvailable: true,
      contractorManagementAvailable: true,
      requiredDocuments: ['CPF tax ID', 'Work eligibility attestation', 'Employment agreement'],
      statutoryBenefits: ['13th salary', 'FGTS contribution', 'Meal allowance policy']
    },
    {
      isoCode: 'IN',
      name: 'India',
      region: 'Asia Pacific',
      complianceScore: 86,
      payrollLeadTimeDays: 11,
      employerOfRecordAvailable: true,
      contractorManagementAvailable: true,
      requiredDocuments: ['PAN', 'Aadhaar or passport', 'Local contract'],
      statutoryBenefits: ['Provident fund', 'Gratuity eligibility', 'State-specific shops act compliance']
    }
  ],
  complianceAlerts: [
    {
      id: 'BR-PAY-014',
      title: '13th salary accrual review required',
      riskLevel: 'High',
      country: 'Brazil',
      owner: 'Payroll',
      dueDate: '2026-06-12'
    },
    {
      id: 'BR-TAX-090',
      title: 'CPF validation pending',
      riskLevel: 'Medium',
      country: 'Brazil',
      owner: 'Compliance',
      dueDate: '2026-06-17'
    },
    {
      id: 'JP-AUTH-032',
      title: 'Residence card image awaiting approval',
      riskLevel: 'Medium',
      country: 'Japan',
      owner: 'Mobility',
      dueDate: '2026-06-15'
    }
  ],
  payrollRuns: [
    {
      id: 'RUN-BR-2026-06',
      country: 'Brazil',
      periodEnd: '2026-06-15',
      approvalDeadline: '2026-06-10',
      grossPayroll: 8900,
      currency: 'USD',
      workerCount: 1,
      isApproved: false
    },
    {
      id: 'RUN-DE-2026-06',
      country: 'Germany',
      periodEnd: '2026-06-15',
      approvalDeadline: '2026-06-11',
      grossPayroll: 11250,
      currency: 'USD',
      workerCount: 1,
      isApproved: false
    },
    {
      id: 'RUN-IN-2026-06',
      country: 'India',
      periodEnd: '2026-06-15',
      approvalDeadline: '2026-06-12',
      grossPayroll: 5200,
      currency: 'USD',
      workerCount: 1,
      isApproved: true
    }
  ],
  activity: [
    {
      id: '1',
      title: 'Aiko Tanaka moved to Onboarding',
      description: 'Engineering Manager in JP via EmployerOfRecord',
      eventDate: '2026-06-27',
      category: 'workforce'
    },
    {
      id: '2',
      title: 'Mateus Oliveira moved to NeedsAttention',
      description: 'Customer Success Lead in BR via EmployerOfRecord',
      eventDate: '2026-04-28',
      category: 'attention'
    }
  ]
};
