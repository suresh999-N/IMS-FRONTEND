import { CalendarDays, Star, TimerReset, TrendingUp } from 'lucide-react'
import { formatDate } from '../../../utils/helpers'
import { formatEmpty } from '../supplierFormatters'
import { SupplierMetricCard, SupplierSection } from './SupplierFormSections'

export default function SupplierPerformanceTab({ performance }) {
  const totalOrders = Number(performance?.totalOrders ?? 0)
  const onTimeDeliveries = Number(performance?.onTimeDeliveries ?? 0)
  const delayedDeliveries = Number(performance?.delayedDeliveries ?? 0)
  const onTimeRate = totalOrders
    ? Math.round((onTimeDeliveries / totalOrders) * 100)
    : (totalOrders === 0 ? 100 : 0)

  const ratingValue = performance?.vendorRating
    ? (String(performance.vendorRating).includes('/5') ? performance.vendorRating : `${performance.vendorRating}/5`)
    : '5.0/5'

  return (
    <SupplierSection
      title="Performance"
      description="Read-only vendor scorecard derived from supplier_performance."
    >
      <div className="supplier-performance-grid">
        <SupplierMetricCard label="Total Orders" value={formatEmpty(performance?.totalOrders ?? 0)} helper="POs raised against this vendor." />
        <SupplierMetricCard label="On-Time Deliveries" value={formatEmpty(performance?.onTimeDeliveries ?? 0)} helper={totalOrders ? `${onTimeRate}% delivery reliability` : '100% delivery reliability'} tone="success" />
        <SupplierMetricCard label="Delayed Deliveries" value={formatEmpty(performance?.delayedDeliveries ?? 0)} helper="Requires procurement review." tone="warning" />
        <SupplierMetricCard label="Vendor Rating" value={ratingValue} helper="Weighted delivery and quality score." />
      </div>
      <div className="supplier-analytics">
        <div className="supplier-analytics__row">
          <span><TimerReset size={16} /> Delivery Reliability</span>
          <strong>{onTimeRate}%</strong>
        </div>
        <div className="supplier-progress"><span style={{ width: `${onTimeRate}%` }} /></div>
        <div className="supplier-analytics__split">
          <span><CalendarDays size={16} /> Last Supply Date: <strong>{formatDate(performance?.lastSupplyDate) || 'No supply history'}</strong></span>
          <span><TrendingUp size={16} /> Return Percentage: <strong>{performance?.returnPercentage != null ? `${performance.returnPercentage}%` : '0%'}</strong></span>
          <span><Star size={16} /> Rating: <strong>{ratingValue}</strong></span>
        </div>
      </div>
    </SupplierSection>
  )
}
