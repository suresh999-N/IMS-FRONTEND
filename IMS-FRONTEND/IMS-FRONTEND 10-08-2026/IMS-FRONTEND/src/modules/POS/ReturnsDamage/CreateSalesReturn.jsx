import React, { useState, useEffect, useMemo } from 'react'
import { useNavigate, useParams, Link } from 'react-router-dom'
import {
  ArrowLeft,
  AlertCircle,
  FileText,
  Save,
  Send,
} from 'lucide-react'
import apiClient from '../../../api/apiClient'
import { API_ENDPOINTS } from '../../../api/endpoints'
import {
  legacyGetReturnableInvoices,
  legacyGetInvoiceReturnableDetails,
  legacyCreateSalesReturn,
  legacyUpdateSalesReturn,
  legacyGetSalesReturnById,
} from '../../../api/returnsExchangeApi'
import { formatCurrency } from '../../../utils/helpers'
import { showToast } from '../../../components/common/toast'
import { FormControl, Input, Select } from '../../../components/erp'
import '../Sales/Sales.css'
import './SalesReturns.css'

export default function CreateSalesReturn() {
  const navigate = useNavigate()
  const { returnId } = useParams()
  const isEdit = Boolean(returnId)

  const [invoices, setInvoices] = useState([])
  const [warehouses, setWarehouses] = useState([])
  const [selectedInvoiceId, setSelectedInvoiceId] = useState('')
  const [invoiceDetails, setInvoiceDetails] = useState(null)

  const [warehouseId, setWarehouseId] = useState('')
  const [returnDate, setReturnDate] = useState(new Date().toISOString().substring(0, 10))
  const [reason, setReason] = useState('Defective / Damaged Item')
  const [notes, setNotes] = useState('')

  const [itemQuantities, setItemQuantities] = useState({})
  const [editingQtyStr, setEditingQtyStr] = useState({})
  const [focusedKey, setFocusedKey] = useState(null)
  const [qtyErrors, setQtyErrors] = useState({})

  const [loading, setLoading] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState(null)

  useEffect(() => {
    const loadInitialData = async () => {
      setLoading(true)
      setError(null)
      try {
        const [invRes, whRes] = await Promise.all([
          legacyGetReturnableInvoices().catch(() => ({ success: true, data: [] })),
          apiClient(API_ENDPOINTS.warehouses.list).catch(() => ({ success: true, data: [] })),
        ])

        const invoiceList = invRes?.data ? (Array.isArray(invRes.data) ? invRes.data : []) : (Array.isArray(invRes) ? invRes : [])
        setInvoices(invoiceList)

        let whList = whRes?.data ? (Array.isArray(whRes.data) ? whRes.data : []) : (Array.isArray(whRes) ? whRes : [])
        if (!Array.isArray(whList) || whList.length === 0) {
          whList = [{ warehouseId: 1, id: 1, name: 'Main Warehouse' }]
        }
        setWarehouses(whList)
        setWarehouseId(whList[0]?.warehouseId || whList[0]?.id || 1)

        if (isEdit && returnId) {
          const editRes = await legacyGetSalesReturnById(returnId)
          const editData = editRes?.data || editRes
          if (!editData) throw new Error('Sales return record not found.')

          setSelectedInvoiceId(editData.invoiceId)
          setWarehouseId(editData.warehouseId || (whList[0]?.warehouseId || whList[0]?.id))
          setReason(editData.reason || '')
          setNotes(editData.notes || '')
          setReturnDate(new Date(editData.returnDate).toISOString().substring(0, 10))

          const invDetailsRes = await legacyGetInvoiceReturnableDetails(editData.invoiceId)
          const invDetails = invDetailsRes?.data || invDetailsRes
          setInvoiceDetails(invDetails)

          const initialMap = {}
          const initialStrMap = {}
          ;(editData.items || []).forEach((item) => {
            const key = `${item.productId}_${item.variantId || 0}`
            initialMap[key] = item.returnQuantity
            initialStrMap[key] = String(item.returnQuantity)
          })
          setItemQuantities(initialMap)
          setEditingQtyStr(initialStrMap)
        }
      } catch (err) {
        console.error('Error loading initial data for sales return', err)
        setError('Failed to load invoice or warehouse options.')
      } finally {
        setLoading(false)
      }
    }
    loadInitialData()
  }, [returnId, isEdit])

  const handleInvoiceChange = async (e) => {
    const invId = e.target.value
    setSelectedInvoiceId(invId)
    setInvoiceDetails(null)
    setItemQuantities({})
    setEditingQtyStr({})
    setQtyErrors({})
    setError(null)

    if (!invId) return

    setLoading(true)
    try {
      const res = await legacyGetInvoiceReturnableDetails(invId)
      const details = res?.data || res
      if (details && Array.isArray(details.items)) {
        setInvoiceDetails(details)
      } else {
        setError(res?.error || 'Failed to fetch invoice return details.')
      }
    } catch (err) {
      console.error('Failed to fetch invoice return details', err)
      setError(err.response?.data?.message || err.message || 'Failed to fetch invoice return details.')
    } finally {
      setLoading(false)
    }
  }

  const handleQtyInputChange = (itemKey, rawValue, maxReturnable) => {
    setEditingQtyStr((prev) => ({ ...prev, [itemKey]: rawValue }))

    if (rawValue === '' || rawValue === undefined) {
      setItemQuantities((prev) => ({ ...prev, [itemKey]: 0 }))
      setQtyErrors((prev) => ({ ...prev, [itemKey]: null }))
      return
    }

    const num = parseFloat(rawValue)
    if (isNaN(num)) {
      setQtyErrors((prev) => ({ ...prev, [itemKey]: 'Invalid number' }))
      return
    }

    if (num < 0) {
      setQtyErrors((prev) => ({ ...prev, [itemKey]: 'Cannot be negative' }))
      setItemQuantities((prev) => ({ ...prev, [itemKey]: 0 }))
      return
    }

    if (num > maxReturnable) {
      setQtyErrors((prev) => ({ ...prev, [itemKey]: `Max allowed is ${maxReturnable}` }))
      setItemQuantities((prev) => ({ ...prev, [itemKey]: maxReturnable }))
      return
    }

    // Valid quantity
    setQtyErrors((prev) => ({ ...prev, [itemKey]: null }))
    setItemQuantities((prev) => ({
      ...prev,
      [itemKey]: num,
    }))
  }

  const hasAnyQtyError = useMemo(() => {
    return Object.values(qtyErrors).some((err) => Boolean(err))
  }, [qtyErrors])

  const financialSummary = useMemo(() => {
    if (!invoiceDetails || !invoiceDetails.items) {
      return { subtotal: 0, tax: 0, total: 0 }
    }
    let subtotal = 0
    let tax = 0

    invoiceDetails.items.forEach((item) => {
      const key = `${item.productId}_${item.variantId || 0}`
      const qty = itemQuantities[key] || 0
      if (qty > 0) {
        const lineSubtotal = qty * item.price
        const lineTax = lineSubtotal * ((item.taxPercent || 0) / 100)
        subtotal += lineSubtotal
        tax += lineTax
      }
    })

    return {
      subtotal,
      tax,
      total: subtotal + tax,
    }
  }, [invoiceDetails, itemQuantities])

  const handleSubmitForm = async (submitForApproval = false) => {
    if (!selectedInvoiceId) {
      setError('Please select an Invoice.')
      showToast({ type: 'error', title: 'Validation Error', message: 'Please select an invoice before submitting.' })
      return
    }

    if (hasAnyQtyError) {
      showToast({ type: 'error', title: 'Validation Error', message: 'Please fix return quantity errors before submitting.' })
      return
    }

    if (!invoiceDetails || !invoiceDetails.items) {
      setError('No items available to return.')
      return
    }

    const itemsToSubmit = []
    invoiceDetails.items.forEach((item) => {
      const key = `${item.productId}_${item.variantId || 0}`
      const qty = itemQuantities[key] || 0
      if (qty > 0) {
        itemsToSubmit.push({
          productId: item.productId,
          variantId: item.variantId || null,
          invoicedQuantity: item.soldQuantity,
          returnQuantity: qty,
          price: item.price,
          tax: item.taxPercent || 0,
          discount: item.discountPercent || 0,
        })
      }
    })

    if (itemsToSubmit.length === 0) {
      setError('Please enter a return quantity greater than 0 for at least one item.')
      showToast({ type: 'error', title: 'Validation Error', message: 'Please enter a return quantity greater than 0 for at least one item.' })
      return
    }

    setSubmitting(true)
    setError(null)

    const payload = {
      invoiceId: parseInt(selectedInvoiceId, 10),
      warehouseId: warehouseId ? parseInt(warehouseId, 10) : null,
      returnDate,
      reason,
      notes,
      submitForApproval,
      items: itemsToSubmit,
    }

    try {
      let res
      if (isEdit && returnId) {
        res = await legacyUpdateSalesReturn(returnId, payload)
      } else {
        res = await legacyCreateSalesReturn(payload)
      }

      if (res?.success || (res && !res.error)) {
        showToast({ type: 'success', title: 'Sales Return Saved', message: 'Sales return saved successfully.' })
        navigate('/pos/returns')
      } else {
        setError(res?.error || res?.message || 'Failed to save sales return.')
        showToast({ type: 'error', title: 'Submission Failed', message: res?.error || res?.message || 'Failed to save sales return.' })
      }
    } catch (err) {
      console.error('Failed to save sales return', err)
      setError(err.response?.data?.message || err.message || 'Failed to save sales return.')
      showToast({ type: 'error', title: 'Submission Error', message: err.response?.data?.message || err.message || 'Failed to save sales return.' })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="create-sales-return-page">
      {/* Clean Compact Header Matching ERP Standard */}
      <header className="sales-page__compact-header" style={{ marginBottom: '0.25rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <Link to="/pos/returns" className="button button-secondary" style={{ minHeight: '34px', padding: '0 0.65rem' }} title="Back to Sales Returns">
            <ArrowLeft size={16} />
          </Link>
          <div>
            <h1 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#111827', margin: 0, lineHeight: 1.2 }}>
              {isEdit ? 'Edit Sales Return' : 'Create Sales Return'}
            </h1>
            <p style={{ fontSize: '0.8125rem', color: '#6b7280', margin: '0.1rem 0 0 0' }}>
              Select a customer invoice to process return quantities and restore inventory.
            </p>
          </div>
        </div>

        <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
          <Link className="button button-secondary" to="/pos/returns">
            Cancel
          </Link>
          <button
            type="button"
            disabled={submitting || loading || hasAnyQtyError}
            onClick={() => handleSubmitForm(false)}
            className="button button-secondary"
          >
            <Save size={16} /> Save Draft
          </button>
          <button
            type="button"
            disabled={submitting || loading || hasAnyQtyError}
            onClick={() => handleSubmitForm(true)}
            className="button button-primary"
          >
            <Send size={16} /> Submit
          </button>
        </div>
      </header>

      {error && (
        <div style={{ padding: '0.875rem 1.15rem', background: '#fef2f2', border: '1px solid #fecaca', borderRadius: '8px', color: '#b91c1c', fontSize: '0.85rem', display: 'flex', alignItems: 'center', gap: '0.65rem' }}>
          <AlertCircle size={18} style={{ color: '#dc2626', flexShrink: 0 }} />
          <span>{error}</span>
        </div>
      )}

      {/* Row 1: Two Column Top Grid */}
      <div className="create-sales-return-top-grid">
        {/* Left Card: Invoice & Location Details */}
        <div className="sales-returns-card">
          <h2 style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827', margin: '0 0 1rem 0', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <FileText size={16} style={{ color: '#059669' }} />
            Invoice & Location Details
          </h2>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1.15rem' }}>
            <FormControl id="invoice-select" label="Select Customer Invoice" required>
              <Select
                id="invoice-select"
                disabled={isEdit}
                value={selectedInvoiceId}
                onChange={handleInvoiceChange}
                required
              >
                <option value="">Select Invoice...</option>
                {invoices.map((inv) => (
                  <option key={inv.invoiceId || inv.id} value={inv.invoiceId || inv.id}>
                    {inv.invoiceNumber} - {inv.customerName} ({formatCurrency(inv.totalAmount)})
                  </option>
                ))}
              </Select>
            </FormControl>

            <FormControl id="warehouse-select" label="Receiving Warehouse">
              <Select
                id="warehouse-select"
                value={warehouseId}
                onChange={(e) => setWarehouseId(e.target.value)}
              >
                {warehouses.map((wh) => (
                  <option key={wh.id || wh.warehouseId} value={wh.id || wh.warehouseId}>
                    {wh.name}
                  </option>
                ))}
              </Select>
            </FormControl>
          </div>

          {invoiceDetails && (
            <div style={{ marginTop: '1.15rem', padding: '0.75rem 1rem', background: '#f9fafb', border: '1px solid #e5e7eb', borderRadius: '8px', display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(130px, 1fr))', gap: '0.875rem', fontSize: '0.85rem' }}>
              <div>
                <span style={{ color: '#6b7280', display: 'block', fontSize: '0.75rem' }}>Customer:</span>
                <span style={{ fontWeight: 600, color: '#111827' }}>{invoiceDetails.customerName}</span>
              </div>
              <div>
                <span style={{ color: '#6b7280', display: 'block', fontSize: '0.75rem' }}>Invoice Date:</span>
                <span style={{ fontWeight: 500, color: '#374151' }}>
                  {invoiceDetails.invoiceDate ? new Date(invoiceDetails.invoiceDate).toLocaleDateString() : 'N/A'}
                </span>
              </div>
              <div>
                <span style={{ color: '#6b7280', display: 'block', fontSize: '0.75rem' }}>Invoice Total:</span>
                <span style={{ fontWeight: 700, color: '#059669' }}>{formatCurrency(invoiceDetails.totalAmount)}</span>
              </div>
            </div>
          )}
        </div>

        {/* Right Card: Return Reason & Notes */}
        <div className="sales-returns-card">
          <h2 style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827', margin: '0 0 1rem 0' }}>
            Return Reason & Notes
          </h2>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '1.15rem' }}>
            <FormControl id="reason-select" label="Reason for Return">
              <Select
                id="reason-select"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
              >
                <option value="Defective / Damaged Item">Defective / Damaged Item</option>
                <option value="Wrong Product Received">Wrong Product Received</option>
                <option value="Customer Changed Mind">Customer Changed Mind</option>
                <option value="Quality Not as Expected">Quality Not as Expected</option>
                <option value="Late Delivery">Late Delivery</option>
                <option value="Other">Other Reason</option>
              </Select>
            </FormControl>

            <FormControl id="return-date-input" label="Return Date" required>
              <Input
                id="return-date-input"
                type="date"
                value={returnDate}
                onChange={(e) => setReturnDate(e.target.value)}
                required
              />
            </FormControl>
          </div>

          <div style={{ marginTop: '1.15rem' }}>
            <FormControl id="notes-input" label="Additional Notes">
              <textarea
                id="notes-input"
                rows={2}
                placeholder="Enter customer notes or inspection remarks..."
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                style={{
                  width: '100%',
                  padding: '0.6rem 0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '6px',
                  fontSize: '0.875rem',
                  outline: 'none',
                  boxSizing: 'border-box',
                  fontFamily: 'inherit',
                }}
              />
            </FormControl>
          </div>
        </div>
      </div>

      {/* Row 2: Full-Width Returnable Products Table Card */}
      <div className="sales-returns-card-table">
        <div className="sales-returns-card-header">
          <h2 className="sales-returns-card-title">
            Returnable Products List {invoiceDetails?.items ? `(${invoiceDetails.items.length})` : ''}
          </h2>
        </div>

        {!invoiceDetails ? (
          <div style={{ padding: '3rem 2rem', textAlign: 'center', color: '#9ca3af', fontSize: '0.875rem' }}>
            Select a customer invoice above to load returnable products.
          </div>
        ) : !invoiceDetails.items || invoiceDetails.items.length === 0 ? (
          <div style={{ padding: '3rem 2rem', textAlign: 'center', color: '#6b7280', fontSize: '0.875rem' }}>
            No returnable products found on this invoice.
          </div>
        ) : (
          <div className="sales-returns-table-wrapper">
            <table className="sales-returns-table" style={{ width: '100%' }}>
              <thead>
                <tr>
                  <th style={{ width: '42%', textAlign: 'left' }}>Product Name</th>
                  <th style={{ width: '80px', textAlign: 'center' }}>Sold Qty</th>
                  <th style={{ width: '100px', textAlign: 'center' }}>Prev Returned</th>
                  <th style={{ width: '110px', textAlign: 'center' }}>Max Returnable</th>
                  <th style={{ width: '130px', textAlign: 'center' }}>Return Qty</th>
                  <th style={{ width: '120px', textAlign: 'right' }}>Price</th>
                  <th style={{ width: '140px', textAlign: 'right' }}>Line Total</th>
                </tr>
              </thead>
              <tbody>
                {invoiceDetails.items.map((item) => {
                  const itemKey = `${item.productId}_${item.variantId || 0}`
                  const currentQty = itemQuantities[itemKey] || 0
                  const isFocused = focusedKey === itemKey
                  const displayQtyStr = isFocused
                    ? (editingQtyStr[itemKey] !== undefined ? editingQtyStr[itemKey] : (currentQty === 0 ? '' : String(currentQty)))
                    : (currentQty === 0 ? '0' : String(currentQty))

                  const hasError = Boolean(qtyErrors[itemKey])
                  const lineTotal = currentQty * item.price * (1 + (item.taxPercent || 0) / 100)

                  return (
                    <tr key={itemKey}>
                      <td className="sales-returns-col-product" style={{ width: '42%', verticalAlign: 'middle' }}>
                        <div className="sales-returns-product-name" title={item.productName}>
                          {item.productName}
                        </div>
                        <div className="sales-returns-product-sku">
                          SKU: {item.productSKU || '—'}
                        </div>
                      </td>
                      <td style={{ width: '80px', textAlign: 'center', color: '#4b5563', fontWeight: 500, verticalAlign: 'middle' }}>
                        {item.soldQuantity}
                      </td>
                      <td style={{ width: '100px', textAlign: 'center', color: '#d97706', fontWeight: 500, verticalAlign: 'middle' }}>
                        {item.previouslyReturnedQuantity}
                      </td>
                      <td style={{ width: '110px', textAlign: 'center', color: '#059669', fontWeight: 600, verticalAlign: 'middle' }}>
                        {item.returnableQuantity}
                      </td>
                      <td style={{ width: '130px', textAlign: 'center', verticalAlign: 'middle' }}>
                        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                          <input
                            type="number"
                            min="0"
                            max={item.returnableQuantity}
                            step="1"
                            disabled={item.returnableQuantity <= 0}
                            value={displayQtyStr}
                            onFocus={(e) => {
                              setFocusedKey(itemKey)
                              setEditingQtyStr((prev) => ({
                                ...prev,
                                [itemKey]: currentQty === 0 ? '' : String(currentQty),
                              }))
                              e.target.select()
                            }}
                            onBlur={() => {
                              setFocusedKey(null)
                              if (editingQtyStr[itemKey] === '' || isNaN(parseFloat(editingQtyStr[itemKey]))) {
                                setItemQuantities((prev) => ({ ...prev, [itemKey]: 0 }))
                                setEditingQtyStr((prev) => ({ ...prev, [itemKey]: '0' }))
                                setQtyErrors((prev) => ({ ...prev, [itemKey]: null }))
                              }
                            }}
                            onChange={(e) => handleQtyInputChange(itemKey, e.target.value, item.returnableQuantity)}
                            className={`sales-returns-qty-input ${hasError ? 'sales-returns-qty-input--error' : ''}`}
                            style={hasError ? { borderColor: '#dc2626', boxShadow: '0 0 0 3px rgba(220, 38, 38, 0.15)' } : {}}
                            placeholder="0"
                          />
                          {hasError ? (
                            <span style={{ fontSize: '0.7rem', color: '#dc2626', fontWeight: 600, marginTop: '2px', whiteSpace: 'nowrap' }}>
                              {qtyErrors[itemKey]}
                            </span>
                          ) : null}
                        </div>
                      </td>
                      <td style={{ width: '120px', textAlign: 'right', color: '#374151', fontWeight: 500, verticalAlign: 'middle' }}>
                        {formatCurrency(item.price)}
                      </td>
                      <td style={{ width: '140px', textAlign: 'right', fontWeight: 700, color: '#059669', verticalAlign: 'middle' }}>
                        {formatCurrency(lineTotal)}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Row 3: Full-Width Financial Summary & Action Bar */}
      <div className="sales-returns-card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1.25rem' }}>
          <div style={{ flex: '1', minWidth: '240px' }}>
            <h3 style={{ fontSize: '0.85rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em', margin: 0 }}>
              Financial Breakdown
            </h3>
            <p style={{ fontSize: '0.8125rem', color: '#6b7280', margin: '0.25rem 0 0 0' }}>
              Total return refund calculated automatically based on item return quantities and applied taxes.
            </p>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: '2rem', flexWrap: 'wrap' }}>
            <div style={{ textAlign: 'right' }}>
              <span style={{ fontSize: '0.75rem', color: '#6b7280', display: 'block' }}>Subtotal</span>
              <span style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827' }}>{formatCurrency(financialSummary.subtotal)}</span>
            </div>

            <div style={{ textAlign: 'right' }}>
              <span style={{ fontSize: '0.75rem', color: '#6b7280', display: 'block' }}>Tax Amount</span>
              <span style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827' }}>{formatCurrency(financialSummary.tax)}</span>
            </div>

            <div style={{ textAlign: 'right', paddingLeft: '1.25rem', borderLeft: '1px solid #e5e7eb' }}>
              <span style={{ fontSize: '0.75rem', color: '#059669', fontWeight: 600, display: 'block' }}>Total Refund Amount</span>
              <span style={{ fontSize: '1.25rem', fontWeight: 700, color: '#059669' }}>{formatCurrency(financialSummary.total)}</span>
            </div>
          </div>
        </div>

        <div style={{ marginTop: '1.25rem', paddingTop: '1rem', borderTop: '1px solid #e5e7eb', display: 'flex', gap: '0.75rem', justifyContent: 'flex-end' }}>
          <Link className="button button-secondary" to="/pos/returns">
            Cancel
          </Link>
          <button
            type="button"
            disabled={submitting || loading || hasAnyQtyError}
            onClick={() => handleSubmitForm(false)}
            className="button button-secondary"
          >
            <Save size={16} /> Save Draft
          </button>
          <button
            type="button"
            disabled={submitting || loading || hasAnyQtyError}
            onClick={() => handleSubmitForm(true)}
            className="button button-primary"
          >
            <Send size={16} /> Submit
          </button>
        </div>
      </div>
    </div>
  )
}
