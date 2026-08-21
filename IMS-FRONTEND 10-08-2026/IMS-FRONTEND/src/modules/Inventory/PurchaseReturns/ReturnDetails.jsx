import React, { useCallback, useEffect, useState, useMemo } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  RotateCcw,
  ArrowLeft,
  Printer,
  Edit,
  Trash2,
  FileText,
  Clock,
  PackageCheck,
  Coins,
  Building2,
  CheckCircle,
  XCircle,
  Send,
  CheckCheck,
} from 'lucide-react'
import { ConfirmationDialog, StatusBadge } from '../../../components/erp'
import { showToast } from '../../../components/common/toast'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import {
  getPurchaseReturnById,
  deletePurchaseReturn,
  submitPurchaseReturn,
  approvePurchaseReturn,
  rejectPurchaseReturn,
  completePurchaseReturn,
  getPurchaseReturnErrorMessage,
} from '../../../api/purchaseReturnsApi'
import '../../POS/Sales/Sales.css'
import '../../POS/ReturnsDamage/SalesReturns.css'
import './ReturnDetails.css'

export default function ReturnDetails() {
  const { returnId } = useParams()
  const navigate = useNavigate()

  const [record, setRecord] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [actionLoading, setActionLoading] = useState(false)
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false)
  const [showRejectModal, setShowRejectModal] = useState(false)
  const [rejectionReason, setRejectionReason] = useState('')

  const fetchDetail = useCallback(async () => {
    if (!returnId) return
    setLoading(true)
    setError(null)
    try {
      const response = await getPurchaseReturnById(returnId)
      if (response?.success && response.data) {
        setRecord(response.data)
      } else {
        setError(getPurchaseReturnErrorMessage(response, 'Failed to load purchase return details.'))
        setRecord(null)
      }
    } catch (err) {
      console.error('Failed to fetch purchase return details:', err)
      setError(getPurchaseReturnErrorMessage(err, 'Failed to load purchase return details.'))
      setRecord(null)
    } finally {
      setLoading(false)
    }
  }, [returnId])

  useEffect(() => {
    fetchDetail()
  }, [fetchDetail])

  const handleAction = async (actionFn, ...args) => {
    setActionLoading(true)
    try {
      const res = await actionFn(returnId, ...args)
      if (res?.success === false || res?.error) {
        showToast({
          type: 'error',
          title: 'Purchase Returns',
          message: getPurchaseReturnErrorMessage(res, 'Action failed.'),
        })
      } else {
        showToast({
          type: 'success',
          title: 'Purchase Returns',
          message: res?.message || 'Action completed successfully.',
        })
        await fetchDetail()
      }
    } catch (err) {
      showToast({
        type: 'error',
        title: 'Purchase Returns',
        message: getPurchaseReturnErrorMessage(err, 'Action failed.'),
      })
    } finally {
      setActionLoading(false)
    }
  }

  const handleRemove = async () => {
    if (!record) return
    const recId = record.returnId || record.id
    setActionLoading(true)
    try {
      const response = await deletePurchaseReturn(recId)
      if (response?.success || !response?.error) {
        showToast({
          type: 'success',
          title: 'Purchase Return Deleted',
          message: `${record.returnNumber || `PR-${recId}`} deleted successfully.`,
        })
        navigate('/inventory/purchase-returns')
      } else {
        showToast({
          type: 'error',
          title: 'Delete Failed',
          message: getPurchaseReturnErrorMessage(response, 'Failed to delete purchase return.'),
        })
      }
    } catch (err) {
      showToast({
        type: 'error',
        title: 'Delete Error',
        message: getPurchaseReturnErrorMessage(err, 'Failed to delete purchase return.'),
      })
    } finally {
      setActionLoading(false)
      setShowDeleteConfirm(false)
    }
  }

  const handleConfirmReject = async () => {
    if (!rejectionReason.trim()) {
      showToast({ type: 'error', title: 'Validation Error', message: 'Please enter a rejection reason.' })
      return
    }
    setShowRejectModal(false)
    await handleAction(rejectPurchaseReturn, { reason: rejectionReason })
    setRejectionReason('')
  }

  const calculatedTotals = useMemo(() => {
    if (!record || !Array.isArray(record.items)) {
      return { totalReturnedQty: 0, subtotal: 0, tax: 0, discount: 0, total: 0 }
    }
    let totalReturnedQty = 0
    let subtotal = 0
    let discount = 0
    let tax = 0

    record.items.forEach((item) => {
      const qty = Number(item.quantity ?? item.returnQuantity ?? 0)
      const price = Number(item.price ?? item.unitPrice ?? 0)
      const taxRate = Number(item.tax ?? item.taxRate ?? 0)
      const discPercent = Number(item.discount ?? item.discountRate ?? 0)

      totalReturnedQty += qty
      const grossAmount = qty * price
      const discountAmount = grossAmount * (discPercent / 100)
      const taxableAmount = grossAmount - discountAmount
      const taxAmount = taxableAmount * (taxRate / 100)

      subtotal += grossAmount
      discount += discountAmount
      tax += taxAmount
    })

    const total = subtotal - discount + tax
    return {
      totalReturnedQty,
      subtotal: Math.round(subtotal * 100) / 100,
      discount: Math.round(discount * 100) / 100,
      tax: Math.round(tax * 100) / 100,
      total: Math.round(total * 100) / 100,
    }
  }, [record])

  if (loading) {
    return (
      <div style={{ padding: '3.5rem', textAlign: 'center', color: '#6b7280' }}>
        <RotateCcw className="animate-spin" size={28} style={{ margin: '0 auto 0.5rem auto', color: '#059669' }} />
        Loading purchase return details...
      </div>
    )
  }

  if (error || !record) {
    return (
      <div className="sales-returns-container sales-returns-container--details">
        <div style={{ padding: '2.5rem', textAlign: 'center', color: '#dc2626', fontWeight: 600 }}>
          {error || 'Purchase return record not found.'}
        </div>
        <div style={{ textAlign: 'center' }}>
          <Link className="button button-secondary" to="/inventory/purchase-returns">
            <ArrowLeft size={16} /> Back to Purchase Returns
          </Link>
        </div>
      </div>
    )
  }

  const {
    returnId: recReturnId,
    id: recId,
    returnNumber,
    grnNumber,
    grnId,
    supplierName,
    supplierCode,
    warehouseName,
    returnDate,
    status = 'Draft',
    totalAmount,
    totalReturnAmount,
    grandTotal,
    reason,
    notes,
    rejectionReason: rejReason,
    createdBy,
    createdAt,
    submittedBy,
    submittedAt,
    approvedBy,
    approvedAt,
    rejectedBy,
    rejectedAt,
    completedBy,
    completedAt,
    updatedAt,
    items = [],
  } = record

  const actualTotalAmount = Number(totalAmount ?? totalReturnAmount ?? grandTotal ?? calculatedTotals.total)
  const returnNoFormatted = returnNumber || (recReturnId ? `PR-${String(recReturnId).padStart(5, '0')}` : `PR-${returnId}`)
  const grnNoFormatted = grnNumber || (grnId ? `GRN-${String(grnId).padStart(6, '0')}` : 'N/A')

  const workflowStages = ['Draft', 'Pending Approval', 'Approved', 'Completed']
  const currentStageIndex = workflowStages.indexOf(status) >= 0 ? workflowStages.indexOf(status) : (status === 'Draft' ? 0 : 2)

  return (
    <div className="sales-returns-container sales-returns-container--details">
      {/* 1. HEADER */}
      <header className="sales-returns-details-header">
        <div className="sales-returns-details-header-left">
          <Link to="/inventory/purchase-returns" className="button button-secondary sales-returns-back-btn" title="Back to Purchase Returns">
            <ArrowLeft size={16} />
          </Link>
          <div className="sales-returns-details-title-stack">
            <div className="sales-returns-details-title-row">
              <h1>
                Purchase Return: <span className="sales-returns-accent">{returnNoFormatted}</span>
              </h1>
              <StatusBadge status={status} />
            </div>
            <p className="sales-returns-details-subtitle">
              GRN: <strong>#{grnNoFormatted}</strong> &nbsp;|&nbsp; Supplier: <strong>{supplierName || 'Unassigned Supplier'}</strong>
            </p>
          </div>
        </div>

        <div className="sales-returns-details-header-right">
          <button onClick={() => window.print()} className="button button-secondary">
            <Printer size={15} /> Print
          </button>

          {status === 'Draft' && (
            <>
              <button onClick={() => navigate(`/inventory/purchase-returns/edit/${recReturnId || recId || returnId}`)} className="button button-secondary">
                <Edit size={15} /> Edit
              </button>
              <button disabled={actionLoading} onClick={() => handleAction(submitPurchaseReturn)} className="button button-primary">
                <Send size={15} /> Submit for Approval
              </button>
              <button disabled={actionLoading} onClick={() => setShowDeleteConfirm(true)} className="button button-secondary" style={{ color: '#dc2626' }}>
                <Trash2 size={15} /> Delete
              </button>
            </>
          )}

          {status === 'Pending Approval' && (
            <>
              <button disabled={actionLoading} onClick={() => setShowRejectModal(true)} className="button button-secondary" style={{ color: '#dc2626' }}>
                <XCircle size={15} /> Reject
              </button>
              <button disabled={actionLoading} onClick={() => handleAction(approvePurchaseReturn)} className="button button-primary">
                <CheckCircle size={15} /> Approve
              </button>
            </>
          )}

          {status === 'Approved' && (
            <button disabled={actionLoading} onClick={() => handleAction(completePurchaseReturn)} className="button button-primary">
              <CheckCheck size={15} /> Complete
            </button>
          )}

          {status === 'Rejected' && (
            <button onClick={() => navigate(`/inventory/purchase-returns/edit/${recReturnId || recId || returnId}`)} className="button button-secondary">
              <Edit size={15} /> Edit & Resubmit
            </button>
          )}
        </div>
      </header>

      {/* 2. WORKFLOW TIMELINE */}
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
            <h4 style={{ margin: 0, fontSize: '0.875rem', fontWeight: 600 }}>Purchase Return Request Rejected</h4>
            <p style={{ margin: '0.15rem 0 0 0', fontSize: '0.8125rem', color: '#b91c1c' }}>
              Reason: {rejReason || record.rejectionReason || 'No specific rejection reason provided.'}
            </p>
          </div>
        </div>
      )}

      {/* 3. KPI SUMMARY CARDS */}
      <div className="sales-returns-kpi-grid">
        <div className="sales-returns-kpi-card">
          <div>
            <p className="sales-returns-kpi-card__label">Total Return Value</p>
            <h3 className="sales-returns-kpi-card__value" style={{ color: '#059669' }}>
              {formatCurrency(actualTotalAmount)}
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
              {items.length || record.itemCount || 0}
            </h3>
          </div>
          <div className="sales-returns-kpi-card__icon sales-returns-kpi-card__icon--blue">
            <PackageCheck size={18} />
          </div>
        </div>

        <div className="sales-returns-kpi-card">
          <div>
            <p className="sales-returns-kpi-card__label">Returned Quantity</p>
            <h3 className="sales-returns-kpi-card__value">
              {calculatedTotals.totalReturnedQty || record.itemCount || 0} pcs
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
        {/* Left Card: Purchase Information */}
        <div className="sales-returns-card">
          <h2 style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827', margin: '0 0 1rem 0', display: 'flex', alignItems: 'center', gap: '0.5rem', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>
            <Building2 size={16} style={{ color: '#059669' }} />
            Purchase Information
          </h2>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '1rem' }}>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Supplier Name</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 600, color: '#111827' }}>{supplierName || '—'}</p>
              {supplierCode ? <span style={{ fontSize: '0.75rem', color: '#6b7280' }}>Code: {supplierCode}</span> : null}
            </div>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Goods Receipt (GRN)</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 600, color: '#111827' }}>#{grnNoFormatted}</p>
            </div>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Dispatch Warehouse</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 500, color: '#374151' }}>{warehouseName || 'Main Warehouse'}</p>
            </div>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Return Date</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 500, color: '#374151' }}>{formatDate(returnDate)}</p>
            </div>
          </div>
        </div>

        {/* Right Card: Return Details & Notes */}
        <div className="sales-returns-card">
          <h2 style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827', margin: '0 0 1rem 0', display: 'flex', alignItems: 'center', gap: '0.5rem', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>
            <FileText size={16} style={{ color: '#059669' }} />
            Return Details & Notes
          </h2>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '1rem' }}>
            <div>
              <span style={{ fontSize: '0.725rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.03em' }}>Reason for Return</span>
              <p style={{ margin: '0.2rem 0 0 0', fontSize: '0.875rem', fontWeight: 600, color: '#111827' }}>{reason || 'No specific reason provided.'}</p>
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
              {notes || 'No supplier return notes or debit memo remarks entered.'}
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
          <table className="sales-returns-table" style={{ width: '100%', minWidth: '850px' }}>
            <thead>
              <tr>
                <th className="sales-returns-col-product" style={{ width: '34%', minWidth: '280px', textAlign: 'left' }}>Product</th>
                <th style={{ textAlign: 'center', width: '80px' }}>GRN Qty</th>
                <th style={{ textAlign: 'center', width: '90px' }}>Prev Returned</th>
                <th style={{ textAlign: 'center', width: '90px' }}>Returned Qty</th>
                <th style={{ textAlign: 'right', width: '120px' }}>Purchase Price</th>
                <th style={{ textAlign: 'right', width: '80px' }}>Discount</th>
                <th style={{ textAlign: 'right', width: '75px' }}>Tax (%)</th>
                <th style={{ textAlign: 'right', width: '130px' }}>Return Amount</th>
                <th style={{ textAlign: 'center', width: '100px' }}>Status</th>
              </tr>
            </thead>
            <tbody>
              {items.length === 0 ? (
                <tr>
                  <td colSpan={9} style={{ textAlign: 'center', padding: '2rem', color: '#9ca3af' }}>
                    No returned line items found.
                  </td>
                </tr>
              ) : (
                items.map((item, idx) => {
                  const qty = Number(item.quantity ?? item.returnQuantity ?? 0)
                  const price = Number(item.price ?? item.unitPrice ?? 0)
                  const taxRate = Number(item.tax ?? item.taxRate ?? 0)
                  const discPercent = Number(item.discount ?? item.discountRate ?? 0)

                  const grossAmount = qty * price
                  const discountAmount = grossAmount * (discPercent / 100)
                  const taxableAmount = grossAmount - discountAmount
                  const taxAmount = taxableAmount * (taxRate / 100)
                  const lineTotal = taxableAmount + taxAmount

                  return (
                    <tr key={item.id || idx} style={{ height: '60px' }}>
                      <td className="sales-returns-col-product" style={{ width: '34%', minWidth: '280px', verticalAlign: 'middle' }}>
                        <div className="sales-returns-product-name" title={item.productName || `Product #${item.productId}`}>
                          {item.productName || `Product #${item.productId}`}
                        </div>
                        <div className="sales-returns-product-sku">
                          SKU: {item.sku || item.productSKU || '—'}
                          {item.variantName ? ` • ${item.variantName}` : ''}
                        </div>
                      </td>
                      <td style={{ textAlign: 'center', color: '#4b5563', fontWeight: 500, verticalAlign: 'middle' }}>
                        {item.grnQuantity ?? item.receivedQuantity ?? '—'}
                      </td>
                      <td style={{ textAlign: 'center', color: '#d97706', fontWeight: 500, verticalAlign: 'middle' }}>
                        {item.previouslyReturnedQuantity ?? 0}
                      </td>
                      <td style={{ textAlign: 'center', color: '#059669', fontWeight: 600, verticalAlign: 'middle' }}>
                        {qty}
                      </td>
                      <td style={{ textAlign: 'right', color: '#374151', fontWeight: 500, verticalAlign: 'middle' }}>
                        {formatCurrency(price)}
                      </td>
                      <td style={{ textAlign: 'right', color: '#6b7280', verticalAlign: 'middle' }}>
                        {discPercent ? `${discPercent}%` : '0%'}
                      </td>
                      <td style={{ textAlign: 'right', color: '#6b7280', verticalAlign: 'middle' }}>
                        {taxRate || 0}%
                      </td>
                      <td style={{ textAlign: 'right', fontWeight: 700, color: '#059669', verticalAlign: 'middle' }}>
                        {formatCurrency(lineTotal)}
                      </td>
                      <td style={{ textAlign: 'center', verticalAlign: 'middle' }}>
                        <StatusBadge status={status} />
                      </td>
                    </tr>
                  )
                })
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* 7 & 8. AUDIT & FINANCIAL SUMMARY SECTION */}
      <div className="create-sales-return-top-grid">
        {/* Audit Information Card */}
        <div className="sales-returns-card">
          <h3 style={{ fontSize: '0.85rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em', margin: '0 0 0.85rem 0', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.4rem' }}>
            Audit & Activity Trail
          </h3>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.6rem', fontSize: '0.8125rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
              <span>Created By:</span>
              <span style={{ fontWeight: 600, color: '#111827' }}>
                {createdBy || 'System'} {createdAt ? `(${formatDate(createdAt)})` : ''}
              </span>
            </div>

            {submittedBy && (
              <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
                <span>Submitted By:</span>
                <span style={{ fontWeight: 600, color: '#2563eb' }}>
                  {submittedBy} {submittedAt ? `(${formatDate(submittedAt)})` : ''}
                </span>
              </div>
            )}

            {approvedBy && (
              <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
                <span>Approved By:</span>
                <span style={{ fontWeight: 600, color: '#059669' }}>
                  {approvedBy} {approvedAt ? `(${formatDate(approvedAt)})` : ''}
                </span>
              </div>
            )}

            {rejectedBy && (
              <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
                <span>Rejected By:</span>
                <span style={{ fontWeight: 600, color: '#dc2626' }}>
                  {rejectedBy} {rejectedAt ? `(${formatDate(rejectedAt)})` : ''}
                </span>
              </div>
            )}

            {completedBy && (
              <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
                <span>Completed By:</span>
                <span style={{ fontWeight: 600, color: '#059669' }}>
                  {completedBy} {completedAt ? `(${formatDate(completedAt)})` : ''}
                </span>
              </div>
            )}

            <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
              <span>Last Updated:</span>
              <span style={{ fontWeight: 600, color: '#111827' }}>
                {updatedAt ? formatDate(updatedAt) : formatDate(createdAt || new Date())}
              </span>
            </div>
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

            {calculatedTotals.discount > 0 && (
              <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
                <span>Discount:</span>
                <span style={{ fontWeight: 600, color: '#dc2626' }}>-{formatCurrency(calculatedTotals.discount)}</span>
              </div>
            )}

            <div style={{ display: 'flex', justifyContent: 'space-between', color: '#4b5563' }}>
              <span>Tax Amount:</span>
              <span style={{ fontWeight: 600, color: '#111827' }}>{formatCurrency(calculatedTotals.tax)}</span>
            </div>

            <div style={{ paddingTop: '0.65rem', borderTop: '1px solid #e5e7eb', display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontSize: '1.05rem', fontWeight: 700, color: '#059669' }}>
              <span>Total Return Amount:</span>
              <span>{formatCurrency(actualTotalAmount)}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Reject Modal */}
      {showRejectModal && (
        <div className="sales-returns-modal-overlay">
          <div className="sales-returns-modal-content">
            <h3 className="sales-returns-modal-title">Reject Purchase Return Request</h3>
            <p style={{ margin: 0, fontSize: '0.85rem', color: '#6b7280' }}>
              Please enter a reason for rejecting this purchase return request.
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

      {/* Delete Confirmation Modal */}
      {showDeleteConfirm ? (
        <ConfirmationDialog
          title="Delete Purchase Return"
          message={`Are you sure you want to delete ${returnNoFormatted}? This action cannot be undone.`}
          confirmLabel={actionLoading ? 'Deleting...' : 'Delete'}
          disabled={actionLoading}
          onConfirm={handleRemove}
          onCancel={() => setShowDeleteConfirm(false)}
        />
      ) : null}
    </div>
  )
}
