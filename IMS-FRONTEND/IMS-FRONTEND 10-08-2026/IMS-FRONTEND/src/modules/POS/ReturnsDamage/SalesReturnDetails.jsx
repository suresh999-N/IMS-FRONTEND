import React, { useState, useEffect, useMemo } from 'react'
import { useNavigate, useParams, Link } from 'react-router-dom'
import {
  RotateCcw,
  ArrowLeft,
  CheckCircle,
  XCircle,
  CreditCard,
  CheckCheck,
  Printer,
  Send,
  Edit,
  Trash2,
  FileText,
  Clock,
  PackageCheck,
  Coins,
} from 'lucide-react'
import {
  legacyGetSalesReturnById,
  legacySubmitSalesReturn,
  legacyApproveSalesReturn,
  legacyRejectSalesReturn,
  legacyProcessSalesReturnRefund,
  legacyCompleteSalesReturn,
  legacyDeleteSalesReturn,
} from '../../../api/returnsExchangeApi'
import { showToast } from '../../../components/common/toast'
import { formatCurrency } from '../../../utils/helpers'
import { StatusBadge } from '../../../components/erp'
import '../Sales/Sales.css'
import './SalesReturns.css'

export default function SalesReturnDetails() {
  const navigate = useNavigate()
  const { returnId } = useParams()

  const [returnDetails, setReturnDetails] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  // Modal States
  const [showRejectModal, setShowRejectModal] = useState(false)
  const [rejectionReason, setRejectionReason] = useState('')

  const [showRefundModal, setShowRefundModal] = useState(false)
  const [refundMethod, setRefundMethod] = useState('Cash')
  const [refundReference, setRefundReference] = useState('')

  const [actionLoading, setActionLoading] = useState(false)

  const fetchDetails = async () => {
    setLoading(true)
    setError(null)
    try {
      const res = await legacyGetSalesReturnById(returnId)
      const data = res?.data || res
      if (!data || !data.salesReturnId) {
        throw new Error('Sales return record not found.')
      }
      setReturnDetails(data)
    } catch (err) {
      console.error('Failed to load sales return details', err)
      setError(err.response?.data?.message || err.message || 'Failed to fetch sales return details.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (returnId) fetchDetails()
  }, [returnId])

  const handleAction = async (actionFn, ...args) => {
    setActionLoading(true)
    try {
      const res = await actionFn(returnId, ...args)
      if (res?.success === false) {
        showToast({ type: 'error', title: 'Sales Returns', message: res?.error || res?.message || 'Action failed.' })
      } else {
        showToast({ type: 'success', title: 'Sales Returns', message: 'Action completed successfully.' })
        await fetchDetails()
      }
    } catch (err) {
      showToast({ type: 'error', title: 'Sales Returns', message: err.response?.data?.message || err.message || 'Action failed.' })
    } finally {
      setActionLoading(false)
    }
  }

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this draft return?')) return
    setActionLoading(true)
    try {
      await legacyDeleteSalesReturn(returnId)
      showToast({ type: 'success', title: 'Sales Returns', message: 'Draft return deleted successfully.' })
      navigate('/pos/returns')
    } catch (err) {
      showToast({ type: 'error', title: 'Sales Returns', message: err.response?.data?.message || 'Failed to delete return.' })
    } finally {
      setActionLoading(false)
    }
  }

  const handleConfirmReject = async () => {
    if (!rejectionReason.trim()) {
      showToast({ type: 'error', title: 'Validation Error', message: 'Please enter a rejection reason.' })
      return
    }
    setShowRejectModal(false)
    await handleAction(legacyRejectSalesReturn, { reason: rejectionReason })
    setRejectionReason('')
  }

  const handleConfirmRefund = async () => {
    setShowRefundModal(false)
    await handleAction(legacyProcessSalesReturnRefund, {
      refundMethod,
      refundReference,
    })
    setRefundReference('')
  }

  const calculatedTotals = useMemo(() => {
    if (!returnDetails || !returnDetails.items) {
      return { totalReturnedQty: 0, subtotal: 0, tax: 0, total: 0 }
    }
    let totalReturnedQty = 0
    let subtotal = 0
    let tax = 0

    returnDetails.items.forEach((item) => {
      const qty = item.returnQuantity || 0
      const price = item.price || 0
      const taxRate = item.tax || 0
      totalReturnedQty += qty
      const lineSub = qty * price
      const lineTax = lineSub * (taxRate / 100)
      subtotal += lineSub
      tax += lineTax
    })

    return {
      totalReturnedQty,
      subtotal,
      tax,
      total: subtotal + tax,
    }
  }, [returnDetails])

  if (loading) {
    return (
      <div style={{ padding: '3.5rem', textAlign: 'center', color: '#6b7280' }}>
        <RotateCcw className="animate-spin" size={28} style={{ margin: '0 auto 0.5rem auto', color: '#059669' }} />
        Loading sales return details...
      </div>
    )
  }

  if (error || !returnDetails) {
    return (
      <div style={{ padding: '2.5rem', textAlign: 'center', color: '#dc2626', fontWeight: 600 }}>
        {error || 'Sales return record not found.'}
      </div>
    )
  }

  const {
    salesReturnId,
    returnNumber,
    invoiceNumber,
    customerName,
    warehouseName,
    returnDate,
    status,
    grandTotal = 0,
    reason,
    rejectionReason: rejReason,
    createdBy,
    createdAt,
    approvedBy,
    approvedAt,
    refundMethod: refMethod,
    refundReference: refRef,
    notes,
    items = [],
  } = returnDetails

  const workflowStages = ['Draft', 'Pending Approval', 'Approved', 'Refund Processed', 'Completed']
  const currentStageIndex = workflowStages.indexOf(status)

  return (
    <div className="sales-returns-container sales-returns-container--details">
      {/* 1. HEADER */}
      <header className="sales-returns-details-header">
        <div className="sales-returns-details-header-left">
          <Link to="/pos/returns" className="button button-secondary sales-returns-back-btn" title="Back to Sales Returns">
            <ArrowLeft size={16} />
          </Link>
          <div className="sales-returns-details-title-stack">
            <div className="sales-returns-details-title-row">
              <h1>
                Sales Return: <span className="sales-returns-accent">{returnNumber || `SRET-${salesReturnId}`}</span>
              </h1>
              <StatusBadge status={status} />
            </div>
            <p className="sales-returns-details-subtitle">
              Invoice: <strong>#{invoiceNumber || 'N/A'}</strong> &nbsp;|&nbsp; Customer: <strong>{customerName || 'N/A'}</strong>
            </p>
          </div>
        </div>

        <div className="sales-returns-details-header-right">
          <button onClick={() => window.print()} className="button button-secondary">
            <Printer size={15} /> Print
          </button>

          {status === 'Draft' && (
            <>
              <button onClick={() => navigate(`/pos/returns/edit/${salesReturnId}`)} className="button button-secondary">
                <Edit size={15} /> Edit
              </button>
              <button disabled={actionLoading} onClick={() => handleAction(legacySubmitSalesReturn)} className="button button-primary">
                <Send size={15} /> Submit for Approval
              </button>
              <button disabled={actionLoading} onClick={handleDelete} className="button button-secondary" style={{ color: '#dc2626' }}>
                <Trash2 size={15} /> Delete
              </button>
            </>
          )}

          {status === 'Pending Approval' && (
            <>
              <button disabled={actionLoading} onClick={() => setShowRejectModal(true)} className="button button-secondary" style={{ color: '#dc2626' }}>
                <XCircle size={15} /> Reject
              </button>
              <button disabled={actionLoading} onClick={() => handleAction(legacyApproveSalesReturn)} className="button button-primary">
                <CheckCircle size={15} /> Approve
              </button>
            </>
          )}

          {status === 'Approved' && (
            <button disabled={actionLoading} onClick={() => setShowRefundModal(true)} className="button button-primary">
              <CreditCard size={15} /> Process Refund
            </button>
          )}

          {(status === 'Refund Processed' || status === 'Approved') && (
            <button disabled={actionLoading} onClick={() => handleAction(legacyCompleteSalesReturn)} className="button button-primary">
              <CheckCheck size={15} /> Complete
            </button>
          )}
        </div>
      </header>

      {/* 2. WORKFLOW TIMELINE (Immediately below Header and above KPI Cards) */}
      {status !== 'Rejected' ? (
        <div className="sales-returns-stepper-compact">
          {workflowStages.map((stageLabel, idx) => {
            const isCompleted = idx < currentStageIndex
            const isActive = idx === currentStageIndex

            let stepClass = 'sales-returns-step-compact'
            if (isCompleted) stepClass += ' sales-returns-step-compact--completed'
            else if (isActive) stepClass += ' sales-returns-step-compact--active'

            return (
              <div key={stageLabel} className={stepClass}>
                <div className="sales-returns-step-compact__circle">
                  {isCompleted ? '✓' : idx + 1}
                </div>
                <span className="sales-returns-step-compact__label">
                  {stageLabel}
                </span>
              </div>
            )
          })}
        </div>
      ) : (
        <div style={{ padding: '0.875rem 1.15rem', background: '#fef2f2', border: '1px solid #fecaca', borderRadius: '10px', color: '#991b1b', display: 'flex', gap: '0.75rem', alignItems: 'center' }}>
          <XCircle size={20} style={{ color: '#dc2626', flexShrink: 0 }} />
          <div>
            <h4 style={{ margin: 0, fontSize: '0.875rem', fontWeight: 600 }}>Return Request Rejected</h4>
            <p style={{ margin: '0.15rem 0 0 0', fontSize: '0.8125rem', color: '#b91c1c' }}>
              Reason: {rejReason || 'No specific reason provided.'}
            </p>
          </div>
        </div>
      )}

      {/* 3. KPI SUMMARY CARDS */}
      <div className="sales-returns-kpi-grid">
        <div className="sales-returns-kpi-card">
          <div>
            <p className="sales-returns-kpi-card__label">Return Value</p>
            <h3 className="sales-returns-kpi-card__value" style={{ color: '#059669' }}>
              {formatCurrency(grandTotal || calculatedTotals.total)}
            </h3>
          </div>
          <div className="sales-returns-kpi-card__icon sales-returns-kpi-card__icon--emerald">
            <Coins size={18} />
          </div>
        </div>

        <div className="sales-returns-kpi-card">
          <div>
            <p className="sales-returns-kpi-card__label">Returned Items</p>
            <h3 className="sales-returns-kpi-card__value">
              {items.length}
            </h3>
          </div>
          <div className="sales-returns-kpi-card__icon sales-returns-kpi-card__icon--blue">
            <PackageCheck size={18} />
          </div>
        </div>

        <div className="sales-returns-kpi-card">
          <div>
            <p className="sales-returns-kpi-card__label">Return Quantity</p>
            <h3 className="sales-returns-kpi-card__value">
              {calculatedTotals.totalReturnedQty}
            </h3>
          </div>
          <div className="sales-returns-kpi-card__icon sales-returns-kpi-card__icon--amber">
            <RotateCcw size={18} />
          </div>
        </div>

        <div className="sales-returns-kpi-card">
          <div>
            <p className="sales-returns-kpi-card__label">Current Status</p>
            <div style={{ marginTop: '0.2rem' }}>
              <StatusBadge status={status} />
            </div>
          </div>
          <div className="sales-returns-kpi-card__icon sales-returns-kpi-card__icon--gray">
            <Clock size={18} />
          </div>
        </div>
      </div>

      {/* 4 & 5. INFORMATION SECTION (2-Column Grid) */}
      <div className="create-sales-return-top-grid">
        {/* Left Column Card: Invoice & Customer Info */}
        <div className="sales-returns-card">
          <h2 style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827', margin: '0 0 1rem 0', display: 'flex', alignItems: 'center', gap: '0.5rem', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>
            <FileText size={16} style={{ color: '#059669' }} />
            Invoice & Customer Details
          </h2>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '1rem' }}>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Invoice Number</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 600, color: '#111827' }}>#{invoiceNumber || 'N/A'}</p>
            </div>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Customer Name</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 600, color: '#111827' }}>{customerName || 'N/A'}</p>
            </div>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Receiving Warehouse</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 500, color: '#374151' }}>{warehouseName || 'Main Warehouse'}</p>
            </div>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Return Date</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 500, color: '#374151' }}>{new Date(returnDate).toLocaleDateString()}</p>
            </div>
          </div>
        </div>

        {/* Right Column Card: Return Details & Notes */}
        <div className="sales-returns-card">
          <h2 style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827', margin: '0 0 1rem 0', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>
            Return Details & Notes
          </h2>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '1rem' }}>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Reason for Return</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 600, color: '#111827' }}>{reason || 'N/A'}</p>
            </div>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Workflow Status</span>
              <div style={{ marginTop: '0.2rem' }}>
                <StatusBadge status={status} />
              </div>
            </div>
          </div>

          <div style={{ marginTop: '0.75rem', paddingTop: '0.75rem', borderTop: '1px solid #f3f4f6' }}>
            <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Additional Notes</span>
            <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.85rem', color: '#4b5563', lineHeight: 1.4 }}>
              {notes || 'No customer notes or inspection remarks entered.'}
            </p>
          </div>
        </div>
      </div>

      {/* 6. RETURNED PRODUCTS TABLE */}
      <div className="sales-returns-card-table">
        <div className="sales-returns-card-header">
          <h3 className="sales-returns-card-title">Returned Products ({items.length})</h3>
        </div>

        <div className="sales-returns-table-wrapper">
          <table className="sales-returns-table" style={{ width: '100%' }}>
            <thead>
              <tr>
                <th className="sales-returns-col-product" style={{ width: '38%' }}>Product</th>
                <th style={{ textAlign: 'center', width: '100px' }}>Sold Qty</th>
                <th style={{ textAlign: 'center', width: '110px' }}>Prev Returned</th>
                <th style={{ textAlign: 'center', width: '110px' }}>Returned Qty</th>
                <th style={{ textAlign: 'right', width: '120px' }}>Unit Price</th>
                <th style={{ textAlign: 'right', width: '120px' }}>Tax (%)</th>
                <th style={{ textAlign: 'right', width: '140px' }}>Refund Amount</th>
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id || item.productId}>
                  <td className="sales-returns-col-product">
                    <div className="sales-returns-product-name" title={item.productName}>
                      {item.productName}
                    </div>
                    <div className="sales-returns-product-sku">
                      SKU: {item.productSKU || item.sku || '—'}
                    </div>
                  </td>
                  <td style={{ textAlign: 'center', color: '#4b5563', fontWeight: 500 }}>
                    {item.invoicedQuantity ?? item.soldQuantity ?? '—'}
                  </td>
                  <td style={{ textAlign: 'center', color: '#d97706', fontWeight: 500 }}>
                    {item.previouslyReturnedQuantity ?? 0}
                  </td>
                  <td style={{ textAlign: 'center', color: '#059669', fontWeight: 600 }}>
                    {item.returnQuantity}
                  </td>
                  <td style={{ textAlign: 'right', color: '#374151', fontWeight: 500 }}>
                    {formatCurrency(item.price)}
                  </td>
                  <td style={{ textAlign: 'right', color: '#6b7280' }}>
                    {item.tax || 0}%
                  </td>
                  <td style={{ textAlign: 'right', fontWeight: 700, color: '#059669' }}>
                    {formatCurrency(item.total || (item.returnQuantity * item.price * (1 + (item.tax || 0) / 100)))}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* 7 & 8. AUDIT & FINANCIAL SUMMARY SECTION */}
      <div className="create-sales-return-top-grid">
        {/* Audit Information Card */}
        <div className="sales-returns-card">
          <h3 style={{ fontSize: '0.85rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em', margin: '0 0 0.85rem 0', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.4rem' }}>
            Audit & Execution Trail
          </h3>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.6rem', fontSize: '0.8125rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
              <span>Created:</span>
              <span style={{ fontWeight: 600, color: '#111827' }}>
                {createdBy || 'System'} {createdAt ? `(${new Date(createdAt).toLocaleDateString()})` : ''}
              </span>
            </div>

            {approvedBy && (
              <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
                <span>Approved By:</span>
                <span style={{ fontWeight: 600, color: '#111827' }}>
                  {approvedBy} {approvedAt ? `(${new Date(approvedAt).toLocaleDateString()})` : ''}
                </span>
              </div>
            )}

            {refMethod && (
              <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
                <span>Refund Method:</span>
                <span style={{ fontWeight: 600, color: '#059669' }}>
                  {refMethod} {refRef ? `(${refRef})` : ''}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Financial Summary Card */}
        <div className="sales-returns-card">
          <h3 style={{ fontSize: '0.85rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em', margin: '0 0 0.85rem 0', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.4rem' }}>
            Financial Summary
          </h3>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.6rem', fontSize: '0.875rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
              <span>Subtotal:</span>
              <span style={{ fontWeight: 600, color: '#111827' }}>{formatCurrency(calculatedTotals.subtotal)}</span>
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
              <span>Tax Amount:</span>
              <span style={{ fontWeight: 600, color: '#111827' }}>{formatCurrency(calculatedTotals.tax)}</span>
            </div>

            <div style={{ paddingTop: '0.65rem', borderTop: '1px solid #e5e7eb', display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontSize: '1.05rem', fontWeight: 700, color: '#059669' }}>
              <span>Total Refund Amount:</span>
              <span>{formatCurrency(grandTotal || calculatedTotals.total)}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Reject Modal */}
      {showRejectModal && (
        <div className="sales-returns-modal-overlay">
          <div className="sales-returns-modal-content">
            <h3 className="sales-returns-modal-title">Reject Sales Return Request</h3>
            <p style={{ margin: 0, fontSize: '0.85rem', color: '#6b7280' }}>
              Please enter a reason for rejecting this return request.
            </p>
            <textarea
              rows={3}
              placeholder="Enter rejection reason..."
              value={rejectionReason}
              onChange={(e) => setRejectionReason(e.target.value)}
              style={{ width: '100%', padding: '0.6rem', fontSize: '0.875rem', border: '1px solid #d1d5db', borderRadius: '6px', outline: 'none' }}
            />
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.65rem', paddingTop: '0.25rem' }}>
              <button onClick={() => setShowRejectModal(false)} className="sales-returns-btn sales-returns-btn--secondary">
                Cancel
              </button>
              <button onClick={handleConfirmReject} className="sales-returns-btn sales-returns-btn--danger">
                Confirm Rejection
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Refund Modal */}
      {showRefundModal && (
        <div className="sales-returns-modal-overlay">
          <div className="sales-returns-modal-content">
            <h3 className="sales-returns-modal-title">Process Customer Refund</h3>
            <p style={{ margin: 0, fontSize: '0.85rem', color: '#6b7280' }}>
              Record customer payment method and transaction reference details.
            </p>

            <div>
              <label style={{ display: 'block', fontSize: '0.775rem', fontWeight: 600, color: '#374151', marginBottom: '0.3rem' }}>Payment Method</label>
              <select
                value={refundMethod}
                onChange={(e) => setRefundMethod(e.target.value)}
                style={{ width: '100%', padding: '0.55rem', fontSize: '0.875rem', border: '1px solid #d1d5db', borderRadius: '6px', background: '#ffffff' }}
              >
                <option value="Cash">Cash</option>
                <option value="Bank Transfer">Bank Transfer</option>
                <option value="UPI">UPI</option>
                <option value="Wallet">Wallet</option>
                <option value="Credit Note">Credit Note</option>
              </select>
            </div>

            <div>
              <label style={{ display: 'block', fontSize: '0.775rem', fontWeight: 600, color: '#374151', marginBottom: '0.3rem' }}>Transaction Reference #</label>
              <input
                type="text"
                placeholder="e.g. TXN123456 or Credit Note #"
                value={refundReference}
                onChange={(e) => setRefundReference(e.target.value)}
                style={{ width: '100%', padding: '0.55rem', fontSize: '0.875rem', border: '1px solid #d1d5db', borderRadius: '6px' }}
              />
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.65rem', paddingTop: '0.25rem' }}>
              <button onClick={() => setShowRefundModal(false)} className="sales-returns-btn sales-returns-btn--secondary">
                Cancel
              </button>
              <button onClick={handleConfirmRefund} className="sales-returns-btn sales-returns-btn--primary">
                Process Refund
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
