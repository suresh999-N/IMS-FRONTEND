import { AlertTriangle, CheckCircle2 } from 'lucide-react'
import { Link } from 'react-router-dom'
import SkeletonCard from './SkeletonCard'

export default function LowStockWidget({ items = [], isLoading }) {
  const safeItems = Array.isArray(items) ? items : []
  const isHealthy = !isLoading && safeItems.length === 0

  return (
    <section className={`dashboard-panel low-stock-widget ${isHealthy ? 'is-healthy' : ''}`}>
      <div className="dashboard-panel__header">
        <div>
          <h2>Low Stock</h2>
        </div>
        {safeItems.length > 4 && (
          <Link className="dashboard-panel__link" to="/inventory/products?filter=low-stock">View all</Link>
        )}
        <strong className={`low-stock-widget__count ${safeItems.length > 0 ? 'is-warning' : 'is-healthy'}`}>
          {safeItems.length}
        </strong>
      </div>

      {isLoading ? (
        <div className="low-stock-widget__list">
          <SkeletonCard variant="row" />
          <SkeletonCard variant="row" />
          <SkeletonCard variant="row" />
        </div>
      ) : safeItems.length > 0 ? (
        <div className="low-stock-widget__list">
          {safeItems.slice(0, 50).map((item) => (
            <Link
              className="low-stock-row"
              key={item.id || item.sku || item.name}
              to={`/inventory/products/${item.productId || item.ProductId || item.id}`}
            >
              <span className="low-stock-row__icon" aria-hidden="true">
                <AlertTriangle size={16} />
              </span>
              <div>
                <strong>{item.name}</strong>
                <span>Stock: {item.stock} - Reorder Level: {item.reorderLevel}</span>
              </div>
              <span className="low-stock-row__badge">
                {item.status || (Number(item.stock) <= 0 ? 'Critical' : 'Low Stock')}
              </span>
            </Link>
          ))}
          <Link className="low-stock-widget__action" to="/inventory/purchases">
            Create Purchase Order
          </Link>
        </div>
      ) : (
        <div className="dashboard-empty dashboard-empty--success">
          <CheckCircle2 size={18} strokeWidth={2.5} />
          <strong>No low-stock products found</strong>
        </div>
      )}
    </section>
  )
}
