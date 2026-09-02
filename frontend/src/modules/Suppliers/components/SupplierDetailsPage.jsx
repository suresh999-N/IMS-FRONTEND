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

function SupplierDetailsOverview({ supplier, purchases = [] }) {
  const primaryContact = supplier.contacts?.find((contact) => contact.isPrimary) || supplier.contacts?.[0]
  const primaryBank = supplier.bankAccounts?.[0]
  const paymentTerms = supplier.paymentTerm || supplier.paymentTermsProfile || {}

  const computedTotalPurchases = useMemo(() => {
    const rawTotal = supplier.totalPurchaseAmount ?? supplier.purchases ?? supplier.totalPurchases ?? supplier.totalAmount ?? supplier.purchaseAmount
    if (rawTotal != null && Number(rawTotal) > 0) {
      return Number(rawTotal)
    }
    if (purchases && purchases.length > 0) {
      return purchases.reduce((sum, p) => sum + (Number(p.totalAmount ?? p.TotalAmount ?? p.amount ?? p.grandTotal ?? 0)), 0)
    }
    return rawTotal ?? 0
  }, [purchases, supplier])

  const computedLastPurchaseDate = useMemo(() => {
    if (supplier.lastPurchaseDate) {
      return supplier.lastPurchaseDate
    }
    if (purchases && purchases.length > 0) {
      const dates = purchases
        .map((p) => p.orderDate || p.createdAt || p.createdDate || p.date)
        .filter(Boolean)
        .sort((a, b) => new Date(b) - new Date(a))
      if (dates.length > 0) return dates[0]
    }
    return null
  }, [purchases, supplier])

  const computedOutstandingPayable = useMemo(() => {
    const rawOutstanding = supplier.outstandingPayable ?? supplier.outstandingAmount ?? supplier.outstandingBalance ?? supplier.balanceAmount ?? supplier.outstanding ?? supplier.balance
    if (rawOutstanding != null && Number(rawOutstanding) > 0) {
      return Number(rawOutstanding)
    }
    if (computedTotalPurchases > 0) {
      return computedTotalPurchases
    }
    return Number(rawOutstanding || 0)
  }, [supplier, computedTotalPurchases])

  return (
    <div className="supplier-details__overview">
      <div className="card supplier-profile-card">
        <div>
          <span>{formatEmpty(supplier.supplierCode)}</span>
          <h2>{formatEmpty(supplier.name)}</h2>
          <p>{formatEmpty(supplier.companyName)}</p>
        </div>
        <StatusBadge type={getStatusBadgeType(supplier.status)}>
          {formatStatus(supplier.status)}
        </StatusBadge>
      </div>

      <div className="supplier-detail-grid">
        <DetailCard icon={CreditCard} label="Outstanding Payable" value={formatNullableCurrency(formatCurrency, computedOutstandingPayable)} helper="API-reported open payable" />
        <DetailCard icon={Truck} label="Total Purchases" value={formatNullableCurrency(formatCurrency, computedTotalPurchases)} helper={`${purchases ? purchases.length : 0} purchase order records`} />
        <DetailCard icon={Building2} label="Last Purchase" value={formatLastPurchase(computedLastPurchaseDate, formatDate)} helper={formatCategory(supplier.category)} />
        <DetailCard label="Primary Contact" value={formatEmpty(primaryContact?.name || supplier.contact)} helper={formatEmpty(primaryContact?.phone || supplier.phone)} />
        <DetailCard icon={Mail} label="Email" value={formatEmpty(supplier.email || primaryContact?.email)} helper="Supplier communication" />
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
          <strong>{formatEmpty([supplier.gstNumber, supplier.panNumber].filter(Boolean).join(' / '))}</strong>
        </div>
      </div>
    </div>
  )
}

export default function SupplierDetailsPage({
  supplier,
  purchases,
  payments,
  initialTab = 'overview',
  onBack,
  onDocumentsChange,
}) {
  const [activeTab, setActiveTab] = useState(initialTab)
  const performance = useMemo(() => supplier.performance || {}, [supplier])

  return (
    <div className="supplier-details">
      <div className="supplier-details__header">
        <div>
          <h2>{formatEmpty(supplier.name)}</h2>
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

      {activeTab === 'overview' ? <SupplierDetailsOverview supplier={supplier} purchases={purchases} payments={payments} /> : null}
      {activeTab === 'purchaseHistory' ? <SupplierPurchaseHistoryTab purchases={purchases} /> : null}
      {activeTab === 'paymentHistory' ? <SupplierPaymentsTab payments={payments} /> : null}
      {activeTab === 'performance' ? <SupplierPerformanceTab performance={performance} /> : null}
      {activeTab === 'documents' ? (
        <SupplierDocumentsTab
          supplierId={supplier.id || supplier.supplierId}
          documents={supplier.documents || []}
          onDocumentsChange={onDocumentsChange}
        />
      ) : null}
    </div>
  )
}
