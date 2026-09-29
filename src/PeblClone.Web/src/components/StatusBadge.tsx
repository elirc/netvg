import type { ComplianceRiskLevel, WorkerStatus } from '../services/workforceTypes';

type BadgeVariant = WorkerStatus | ComplianceRiskLevel | 'Approved' | 'Pending' | 'Api' | 'Mock';

interface StatusBadgeProps {
  label: BadgeVariant | string;
}

export function StatusBadge({ label }: StatusBadgeProps) {
  return <span className={`status status-${label.toString().toLowerCase()}`}>{humanize(label.toString())}</span>;
}

function humanize(value: string): string {
  return value.replace(/([a-z])([A-Z])/g, '$1 $2');
}
