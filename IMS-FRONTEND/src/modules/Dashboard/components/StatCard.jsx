import { Link } from 'react-router-dom'

export default function StatCard({
  title,
  value,
  description,
  trend,
  trendDirection,
  icon: Icon,
  to,
  tone = 'primary',
  tooltip,
}) {
  const Component = to ? Link : 'article'
  const label = tooltip || description || `Open ${title}`
  const interactiveProps = to
    ? {
        to,
        title: label,
        'aria-label': label,
      }
    : {
        title: label,
        'aria-label': label,
      }

  return (
    <Component className={`dashboard-stat dashboard-stat--${tone} ${to ? 'is-clickable' : ''}`} {...interactiveProps}>
      <span className="dashboard-stat__icon" aria-hidden="true">
        <Icon size={16} />
      </span>
      <div className="dashboard-stat__content">
        <strong className="dashboard-stat__value" title={String(value)}>{value}</strong>
        <span className="dashboard-stat__label" title={title}>{title}</span>
        {trend ? (
          <span
            className={`dashboard-stat__trend ${trendDirection ? `is-${trendDirection}` : ''}`}
            title={typeof trend === 'string' ? trend : undefined}
          >
            {trend}
          </span>
        ) : null}
        {description ? <p className="dashboard-stat__description">{description}</p> : null}
      </div>
    </Component>
  )
}
