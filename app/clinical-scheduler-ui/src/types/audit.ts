export interface AuditLogEntry {
  id: number;
  staffId?: number;
  action: string;
  entityType: string;
  entityId?: number;
  detail?: string;
  icon?: string;
  performedBy: string;
  timestamp: string;
}
