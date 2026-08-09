import { Boxes } from 'lucide-react'
import { Link } from 'react-router-dom'
import { formatCurrency } from '../../../utils/helpers'

export default function TopProductsList({ products = [] }) {
  const safeProducts = Array.isArray(products) ? products : []
  const topProducts = safeProducts.slice(0, 50)
  const totalRevenue = safeProducts.slice(0, 5).reduce((total, product) => total + Number(product.revenue || 0), 0)

  return (
    <section className="dashboard-panel compact-list top-products-panel">
      <div className="dashboard-panel__header">
        <div>
          <h2>Top Products</h2>
        </div>
        <Link className="dashboard-panel__link" to="/inventory/products">View all</Link>
      </div>

      {topProducts.length > 0 ? (
        <div className="top-products-list">
          {topProducts.map((product, index) => {
            const revenue = Number(product.revenue || 0)
            const percent = totalRevenue > 0 ? Math.round((revenue / totalRevenue) * 100) : 0

            return (
              <article className="top-product-row" key={product.id || product.name || index}>
                <span className="top-product-row__rank">#{index + 1}</span>
                <div className="top-product-row__main">
                  <div className="top-product-row__copy">
                    <strong>{product.name}</strong>
                    <small>{product.sku || 'No SKU'} - {product.totalSold || 0} sold</small>
                  </div>
                  <div className="top-product-row__bar" aria-label={`${percent}% of top product revenue`}>
                    <span style={{ width: `${percent}%` }} />
                  </div>
                </div>
                <em>{formatCurrency(revenue)}</em>
              </article>
            )
          })}
        </div>
      ) : (
        <div className="dashboard-empty dashboard-empty--compact">
          <Boxes size={18} />
          <strong>No product performance yet</strong>
          <p>Top products will appear after sales activity is available.</p>
        </div>
      )}
    </section>
  )
}
