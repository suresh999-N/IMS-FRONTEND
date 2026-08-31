import { CalendarDays, Star, TimerReset, TrendingUp } from 'lucide-react'
import { formatDate } from '../../../utils/helpers'
import { formatEmpty } from '../supplierFormatters'
import { SupplierMetricCard, SupplierSection } from './SupplierFormSections'

export default function SupplierPerformanceTab({ performance }) {
  const totalOrders = Number(performance?.totalOrders || 0)
  const onTimeDeliveries = Number(performance?.onTimeDeliveries || 0)
  const onTimeRate = totalOrders
    ? Math.round((onTimeDeliveries / totalOrders) * 100)
    : 0

  return (
    <SupplierSection
      title="Performance"
      description="Read-only vendor scorecard derived from supplier_performance."
    >
      <div className="supplier-performance-grid">
        <SupplierMetricCard label="Total Orders" value={formatEmpty(performance?.totalOrders)} helper="POs raised against this vendor." />
        <SupplierMetricCard label="On-Time Deliveries" value={formatEmpty(performance?.onTimeDeliveries)} helper={totalOrders ? `${onTimeRate}% delivery reliability` : 'No delivery history'} tone="success" />
        <SupplierMetricCard label="Delayed Deliveries" value={formatEmpty(performance?.delayedDeliveries)} helper="Requires procurement review." tone="warning" />
        <SupplierMetricCard label="Vendor Rating" value={performance?.vendorRating ? `${performance.vendorRating}/5` : 'Not rated'} helper="Weighted delivery and quality score." />
      </div>
      <div className="supplier-analytics">
        <div className="supplier-analytics__row">
          <span><TimerReset size={16} /> Delivery Reliability</span>
          <strong>{onTimeRate}%</strong>
        </div>
        <div className="supplier-progress"><span style={{ width: `${onTimeRate}%` }} /></div>
        <div className="supplier-analytics__split">
          <span><CalendarDays size={16} /> Last Supply Date: <strong>{formatDate(performance?.lastSupplyDate) || 'No supply history'}</strong></span>
          <span><TrendingUp size={16} /> Return Percentage: <strong>{performance?.returnPercentage ? `${performance.returnPercentage}%` : '0%'}</strong></span>
          <span><Star size={16} /> Rating: <strong>{performance?.vendorRating ? `${performance.vendorRating}/5` : 'Not rated'}</strong></span>
        </div>
      </div>
    </SupplierSection>
  )
}
