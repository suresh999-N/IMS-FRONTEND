export default function SkeletonCard({ variant = 'card' }) {
  return (
    <div className={`dashboard-skeleton dashboard-skeleton--${variant}`} aria-hidden="true">
      <span />
      <strong />
      <p />
    </div>
  )
}
