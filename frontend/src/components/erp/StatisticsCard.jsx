import './ERPComponents.css'

export default function StatisticsCard({
  icon: Icon,
  label,
  value,
  helper,
  trend,
  tone = 'neutral',
  className = '',
  title,
  valueTitle,
}) {
  const trendValue = typeof trend === 'string' ? trend : trend?.value
  const trendLabel = typeof trend === 'string' ? '' : trend?.label
  const trendTone = typeof trend === 'string' ? 'neutral' : trend?.tone || 'neutral'
  const resolvedValueTitle = valueTitle || (typeof value === 'string' && value !== '...' ? value : undefined)
  const resolvedCardTitle = title || (typeof label === 'string' && resolvedValueTitle ? `${label}: ${resolvedValueTitle}` : undefined)

  return (
    <div
      className={`card erp-statistics-card erp-statistics-card--${tone} ${className}`.trim()}
      title={resolvedCardTitle}
    >
      {Icon ? (
        <span className="erp-statistics-card__icon" aria-hidden="true">
          <Icon size={18} />
        </span>
      ) : null}
      <div className="erp-statistics-card__body">
        <p className="erp-statistics-card__label">{label}</p>
        <strong
          className="erp-statistics-card__value"
          title={resolvedValueTitle || (typeof value === 'string' || typeof value === 'number' ? String(value) : undefined)}
          data-tooltip={resolvedValueTitle}
        >
          {value}
        </strong>
        {trendValue ? (
          <p className={`erp-statistics-card__trend erp-statistics-card__trend--${trendTone}`.trim()}>
            <span>{trendValue}</span>
            {trendLabel ? <small>{trendLabel}</small> : null}
          </p>
        ) : helper ? (
          <p className="erp-statistics-card__helper">{helper}</p>
        ) : null}
      </div>
    </div>
  )
}
