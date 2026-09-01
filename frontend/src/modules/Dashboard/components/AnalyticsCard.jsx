export default function AnalyticsCard({
  title,
  value,
  caption,
  icon: Icon,
  tone = 'primary',
  isPlaceholder = false,
}) {
  return (
    <article className={`analytics-card analytics-card--${tone} ${isPlaceholder ? 'is-placeholder' : ''}`}>
      <span className="analytics-card__icon" aria-hidden="true">
        <Icon size={18} />
      </span>
      <div>
        <span>{title}</span>
        <strong>{value}</strong>
        <p>{caption}</p>
      </div>
    </article>
  )
}
