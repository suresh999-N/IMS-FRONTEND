import { ExternalLink } from 'lucide-react'
import { DataTable, StatusBadge } from '../../../components/erp'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import { formatEmpty, formatStatus, getStatusBadgeType } from '../supplierFormatters'
import { SupplierSection } from './SupplierFormSections'

export default function SupplierPurchaseHistoryTab({ purchases }) {
  const columns = [
    {
      key: 'poNumber',
      label: 'PO Number',
      sortable: true,
      render: (po) => (
        <button type="button" className="supplier-link-button">
          {formatEmpty(po.poNumber)}
          <ExternalLink size={13} />
        </button>
      ),
    },
    { key: 'orderDate', label: 'Order Date', sortable: true, render: (po) => formatDate(po.orderDate) || 'Date pending' },
    { key: 'totalAmount', label: 'Total Amount', sortable: true, render: (po) => formatCurrency(po.totalAmount) },
    { key: 'status', label: 'Status', sortable: true, render: (po) => <StatusBadge type={getStatusBadgeType(po.status)}>{formatStatus(po.status)}</StatusBadge> },
    { key: 'receivedQuantity', label: 'Received Qty', sortable: true, render: (po) => formatEmpty(po.receivedQuantity) },
    { key: 'pendingQuantity', label: 'Pending Qty', sortable: true, render: (po) => formatEmpty(po.pendingQuantity) },
    { key: 'paymentStatus', label: 'Payment Status', sortable: true, render: (po) => <StatusBadge type={getStatusBadgeType(po.paymentStatus)}>{formatStatus(po.paymentStatus)}</StatusBadge> },
  ]

  return (
    <SupplierSection
      title="Purchase History"
      description="Read-only purchase order history from purchase_orders."
    >
      <DataTable
        rows={purchases}
        columns={columns}
        defaultPageSize={5}
        emptyMessage="No purchase orders found for this supplier."
        searchPlaceholder="Search purchase orders..."
        splitToolbar
      />
    </SupplierSection>
  )
}
