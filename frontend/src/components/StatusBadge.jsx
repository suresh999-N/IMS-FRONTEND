const STATUS_TYPE_MAP = {
  active: 'success',
  approved: 'success',
  available: 'success',
  completed: 'success',
  good: 'success',
  'in-stock': 'success',
  paid: 'success',
  received: 'success',
  success: 'success',
  'low-stock': 'warning',
  partial: 'warning',
  'partially-paid': 'warning',
  warning: 'warning',
  'out-of-stock': 'failed',
  overdue: 'failed',
  blocked: 'failed',
  cancelled: 'cancelled',
  canceled: 'cancelled',
  critical: 'failed',
  disabled: 'failed',
  failed: 'failed',
  inactive: 'failed',
  pending: 'pending',
  prospect: 'pending',
  unpaid: 'pending',
  draft: 'draft',
  reversed: 'draft',
  archived: 'draft',
  discontinued: 'draft',
}

function normalizeStatusKey(value) {
  return String(value || '')
    .trim()
    .toLowerCase()
    .replace(/[_\s]+/g, '-')
}

function getStatusType(status, fallbackType) {
  const key = normalizeStatusKey(status)
  return STATUS_TYPE_MAP[key] || fallbackType || 'info'
}

export default function StatusBadge({
  children,
  label,
  status,
  type,
  icon: Icon,
  className = '',
  onClick,
  onDoubleClick,
  onKeyDown,
  disabled = false,
  title,
  ariaLabel,
}) {
  const content = children ?? label ?? status
  const resolvedType = getStatusType(status ?? content, type)
  const isInteractive = Boolean(onClick || onDoubleClick || onKeyDown)
  const resolvedClassName = `status-badge status-${resolvedType} ${isInteractive ? 'status-badge--button' : ''} ${className}`.trim()

  if (isInteractive) {
    return (
      <button
        type="button"
        className={resolvedClassName}
        onClick={onClick}
        onDoubleClick={onDoubleClick}
        onKeyDown={onKeyDown}
        disabled={disabled}
        title={title || `Update status: ${content}`}
        aria-label={ariaLabel || title || `Update status: ${content}`}
        data-row-click-ignore="true"
      >
        {Icon ? <Icon size={14} /> : null}
        {content}
      </button>
    )
  }

  return (
    <span className={resolvedClassName}>
      {Icon ? <Icon size={14} /> : null}
      {content}
    </span>
  )
}
