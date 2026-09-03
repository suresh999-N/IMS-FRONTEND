import { ArrowLeft, Building2, CreditCard, Landmark, Mail, Phone, Truck } from 'lucide-react'
import { useMemo, useState } from 'react'
import { StatusBadge } from '../../../components/erp'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import {
  formatEmpty,
  formatCategory,
  formatLastPurchase,
  formatNullableCurrency,
  formatPaymentMethod,
  formatStatus,
  getStatusBadgeType,
} from '../supplierFormatters'
import SupplierDocumentsTab from './SupplierDocumentsTab'
import SupplierPaymentsTab from './SupplierPaymentsTab'
import SupplierPerformanceTab from './SupplierPerformanceTab'
import SupplierPurchaseHistoryTab from './SupplierPurchaseHistoryTab'

const tabs = [
  { id: 'overview', label: 'Overview' },
  { id: 'purchaseHistory', label: 'Purchase History' },
  { id: 'paymentHistory', label: 'Payment History' },
  { id: 'performance', label: 'Performance' },
  { id: 'documents', label: 'Documents' },
]

function DetailCard({ icon: Icon, label, value, helper }) {
  return (
    <div className="supplier-detail-card">
      <div className="supplier-detail-card__icon">{Icon ? <Icon size={17} /> : null}</div>
      <div>
        <span>{label}</span>
        <strong>{value}</strong>
        {helper ? <p>{helper}</p> : null}
      </div>
    </div>
  )
}

function SupplierDetailsOverview({ supplier = {}, purchases = [], payments = [] }) {
  const currentSupplier = supplier || {}
  const primaryContact = Array.isArray(currentSupplier.contacts)
    ? currentSupplier.contacts.find((contact) => contact?.isPrimary) || currentSupplier.contacts[0]
    : null
  const primaryBank = Array.isArray(currentSupplier.bankAccounts) ? currentSupplier.bankAccounts[0] : null
  const paymentTerms = currentSupplier.paymentTerm || currentSupplier.paymentTermsProfile || {}

  const computedTotalPurchases = useMemo(() => {
    if (!currentSupplier) return 0
    const rawTotal = currentSupplier.totalPurchaseAmount ?? currentSupplier.purchases ?? currentSupplier.totalPurchases ?? currentSupplier.totalAmount ?? currentSupplier.purchaseAmount
    if (rawTotal != null && !isNaN(Number(rawTotal)) && Number(rawTotal) > 0) {
      return Number(rawTotal)
    }
    if (Array.isArray(purchases) && purchases.length > 0) {
      return purchases.reduce((sum, p) => sum + (Number(p?.totalAmount ?? p?.TotalAmount ?? p?.amount ?? p?.grandTotal ?? 0) || 0), 0)
    }
    return Number(rawTotal) || 0
  }, [purchases, currentSupplier])

  const computedLastPurchaseDate = useMemo(() => {
    if (currentSupplier.lastPurchaseDate) {
      return currentSupplier.lastPurchaseDate
    }
    if (Array.isArray(purchases) && purchases.length > 0) {
      const dates = purchases
        .map((p) => p?.orderDate || p?.createdAt || p?.createdDate || p?.date)
        .filter(Boolean)
        .sort((a, b) => new Date(b) - new Date(a))
      if (dates.length > 0) return dates[0]
    }
    return null
  }, [purchases, currentSupplier])

  const computedOutstandingPayable = useMemo(() => {
    const rawOutstanding = currentSupplier.outstandingPayable ?? currentSupplier.outstandingAmount ?? currentSupplier.outstandingBalance ?? currentSupplier.balanceAmount ?? currentSupplier.outstanding ?? currentSupplier.balance
    if (rawOutstanding != null && !isNaN(Number(rawOutstanding)) && Number(rawOutstanding) > 0) {
      return Number(rawOutstanding)
    }
    if (computedTotalPurchases > 0) {
      return computedTotalPurchases
    }
    return Number(rawOutstanding || 0)
  }, [currentSupplier, computedTotalPurchases])

  return (
    <div className="supplier-details__overview">
      <div className="card supplier-profile-card">
        <div>
          <span>{formatEmpty(currentSupplier.supplierCode)}</span>
          <h2>{formatEmpty(currentSupplier.name)}</h2>
          <p>{formatEmpty(currentSupplier.companyName)}</p>
        </div>
        <StatusBadge type={getStatusBadgeType(currentSupplier.status)}>
          {formatStatus(currentSupplier.status)}
        </StatusBadge>
      </div>

      <div className="supplier-detail-grid">
        <DetailCard icon={CreditCard} label="Outstanding Payable" value={formatNullableCurrency(formatCurrency, computedOutstandingPayable)} helper="API-reported open payable" />
        <DetailCard icon={Truck} label="Total Purchases" value={formatNullableCurrency(formatCurrency, computedTotalPurchases)} helper={`${Array.isArray(purchases) ? purchases.length : 0} purchase order records`} />
        <DetailCard icon={Building2} label="Last Purchase" value={formatLastPurchase(computedLastPurchaseDate, formatDate)} helper={formatCategory(currentSupplier.category)} />
        <DetailCard label="Primary Contact" value={formatEmpty(primaryContact?.name || currentSupplier.contact)} helper={formatEmpty(primaryContact?.phone || currentSupplier.phone)} />
        <DetailCard icon={Mail} label="Email" value={formatEmpty(currentSupplier.email || primaryContact?.email)} helper="Supplier communication" />
        <DetailCard icon={Landmark} label="Bank Summary" value={formatEmpty(primaryBank?.bankName)} helper={formatEmpty(primaryBank?.accountName || formatPaymentMethod(paymentTerms.preferredPaymentMethod))} />
      </div>

      <div className="card supplier-details__terms">
        <h3>Payment Controls</h3>
        <div>
          <span>Credit Days</span>
          <strong>{formatEmpty(paymentTerms.creditDays)}</strong>
        </div>
        <div>
          <span>Preferred Method</span>
          <strong>{formatEmpty(formatPaymentMethod(paymentTerms.preferredPaymentMethod))}</strong>
        </div>
        <div>
          <span>GST / PAN</span>
          <strong>{formatEmpty([currentSupplier.gstNumber, currentSupplier.panNumber].filter(Boolean).join(' / '))}</strong>
        </div>
      </div>
    </div>
  )
}

export default function SupplierDetailsPage({
  supplier = {},
  purchases = [],
  payments = [],
  initialTab = 'overview',
  onBack,
  onDocumentsChange,
}) {
  const [activeTab, setActiveTab] = useState(initialTab)
  const currentSupplier = supplier || {}

  const performance = useMemo(() => {
    const apiPerf = currentSupplier.performance || {}
    const totalOrdersFromApi = apiPerf.totalOrders ?? apiPerf.TotalOrders
    if (totalOrdersFromApi != null && totalOrdersFromApi !== '' && Number(totalOrdersFromApi) > 0) {
      return apiPerf
    }

    const poList = Array.isArray(purchases) ? purchases : []
    const totalOrders = poList.length

    if (totalOrders === 0) {
      return apiPerf
    }

    const completedOrReceived = poList.filter((p) => {
      const st = String(p?.status || p?.orderStatus || p?.Status || '').toLowerCase()
      return ['completed', 'received', 'delivered', 'fulfilled', 'closed', 'ordered'].includes(st)
    }).length

    const delayedCount = poList.filter((p) => {
      const st = String(p?.status || p?.orderStatus || p?.Status || '').toLowerCase()
      return ['delayed', 'overdue', 'cancelled', 'late'].includes(st)
    }).length

    const onTimeDeliveries = Math.max(0, completedOrReceived - delayedCount)
    const onTimeRate = totalOrders > 0 ? Math.round((onTimeDeliveries / totalOrders) * 100) : 100

    const sortedDates = poList
      .map((p) => p?.orderDate || p?.receivedDate || p?.createdAt || p?.createdDate || p?.date)
      .filter(Boolean)
      .sort((a, b) => new Date(b) - new Date(a))

    const lastSupplyDate = sortedDates[0] || null

    let calculatedRating = '5.0'
    if (onTimeRate < 50) calculatedRating = '2.5'
    else if (onTimeRate < 75) calculatedRating = '3.5'
    else if (onTimeRate < 90) calculatedRating = '4.2'
    else if (onTimeRate < 100) calculatedRating = '4.8'

    return {
      ...apiPerf,
      totalOrders,
      onTimeDeliveries,
      delayedDeliveries: delayedCount,
      vendorRating: apiPerf.vendorRating || apiPerf.rating || calculatedRating,
      lastSupplyDate: apiPerf.lastSupplyDate || lastSupplyDate,
      returnPercentage: apiPerf.returnPercentage || 0,
    }
  }, [currentSupplier, purchases])

  if (!supplier || (!supplier.id && !supplier.supplierId && !supplier.name)) {
    return null
  }

  return (
    <div className="supplier-details">
      <div className="supplier-details__header">
        <div>
          <h2>{formatEmpty(currentSupplier.name)}</h2>
          <p>Supplier operational workspace with procurement, payment, and performance context.</p>
        </div>
        <button type="button" className="button button-primary" onClick={onBack}>
          <ArrowLeft size={16} />
          Back to Suppliers
        </button>
      </div>

      <div className="card supplier-details__tabs">
        {tabs.map((tab) => (
          <button
            key={tab.id}
            type="button"
            className={activeTab === tab.id ? 'is-active' : ''}
            onClick={() => setActiveTab(tab.id)}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === 'overview' ? <SupplierDetailsOverview supplier={currentSupplier} purchases={purchases} payments={payments} /> : null}
      {activeTab === 'purchaseHistory' ? <SupplierPurchaseHistoryTab purchases={purchases} /> : null}
      {activeTab === 'paymentHistory' ? <SupplierPaymentsTab payments={payments} /> : null}
      {activeTab === 'performance' ? <SupplierPerformanceTab performance={performance} /> : null}
      {activeTab === 'documents' ? (
        <SupplierDocumentsTab
          supplierId={currentSupplier.id || currentSupplier.supplierId}
          documents={currentSupplier.documents || []}
          onDocumentsChange={onDocumentsChange}
        />
      ) : null}
    </div>
  )
}
