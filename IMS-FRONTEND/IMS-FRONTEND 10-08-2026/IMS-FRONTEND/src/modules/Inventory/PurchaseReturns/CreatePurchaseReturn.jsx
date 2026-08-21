import React, { useEffect, useState, useMemo } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Save, Send, Trash2 } from 'lucide-react'
import { FormControl, Input, Select } from '../../../components/erp'
import { showToast } from '../../../components/common/toast'
import { getLocalTodayDate, toDateInputValue } from '../../../utils/dateUtils'
import { formatCurrency } from '../../../utils/helpers'
import {
  getPurchaseReturnSuppliers,
  getPurchaseReturnGoodsReceipts,
  getGoodsReceiptReturnItems,
  createPurchaseReturn,
  getPurchaseReturnById,
  getPurchaseReturnErrorMessage,
} from '../../../api/purchaseReturnsApi'
import '../../POS/Sales/Sales.css'
import '../../POS/ReturnsDamage/SalesReturns.css'
import './CreatePurchaseReturn.css'

export default function CreatePurchaseReturn() {
  const navigate = useNavigate()
  const { returnId: editId } = useParams()
  const isEditing = Boolean(editId)

  const [suppliers, setSuppliers] = useState([])
  const [grns, setGrns] = useState([])
  const [loadingRefs, setLoadingRefs] = useState(true)
  const [loadingGrnItems, setLoadingGrnItems] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  const [form, setForm] = useState({
    supplierId: '',
    grnId: '',
    returnDate: getLocalTodayDate(),
    reason: 'Defective / Damaged Item',
    notes: '',
  })

  const [items, setItems] = useState([])

  // Load Suppliers and GRNs
  useEffect(() => {
    async function loadInitial() {
      setLoadingRefs(true)
      const supRes = await getPurchaseReturnSuppliers()
      const grnRes = await getPurchaseReturnGoodsReceipts()
      setLoadingRefs(false)

      if (supRes.success && Array.isArray(supRes.data)) {
        setSuppliers(supRes.data)
      }
      if (grnRes.success && Array.isArray(grnRes.data)) {
        setGrns(grnRes.data)
      }

      if (isEditing) {
        const detailRes = await getPurchaseReturnById(editId)
        if (detailRes.success && detailRes.data) {
          const rec = detailRes.data
          setForm({
            supplierId: String(rec.supplierId || ''),
            grnId: String(rec.grnId || ''),
            returnDate: toDateInputValue(rec.returnDate) || getLocalTodayDate(),
            reason: rec.reason || 'Defective / Damaged Item',
            notes: rec.notes || '',
          })
          setItems(
            (rec.items || []).map((it) => ({
              productId: it.productId,
              productName: it.productName || `Product #${it.productId}`,
              sku: it.sku || '—',
              variantId: it.variantId,
              variantName: it.variantName,
              quantity: Number(it.quantity ?? it.returnQuantity ?? 1),
              price: Number(it.price ?? it.unitCost ?? it.unitPrice ?? 0),
              unitCost: Number(it.price ?? it.unitCost ?? it.unitPrice ?? 0),
              discount: Number(it.discount || 0),
              tax: Number(it.tax || 0),
              receivedQuantity: Number(it.receivedQuantity ?? it.grnQuantity ?? 9999),
              error: null,
            }))
          )
        }
      }
    }
    loadInitial()
  }, [editId, isEditing])

  // When Supplier changes, filter GRNs
  async function handleSupplierChange(supplierId) {
    setForm((prev) => ({ ...prev, supplierId, grnId: '' }))
    setItems([])
    if (supplierId) {
      const res = await getPurchaseReturnGoodsReceipts(Number(supplierId))
      if (res.success && Array.isArray(res.data)) {
        setGrns(res.data)
      }
    }
  }

  // When GRN changes, load products from GRN
  async function handleGrnChange(grnId) {
    setForm((prev) => ({ ...prev, grnId }))
    setItems([])

    if (grnId) {
      setLoadingGrnItems(true)
      const res = await getGoodsReceiptReturnItems(Number(grnId))
      setLoadingGrnItems(false)

      if (res.success && Array.isArray(res.data)) {
        setItems(
          res.data.map((it) => {
            const rawPrice = Number(it.rawUnitPrice ?? it.unitPrice ?? it.unitCost ?? it.price ?? 0)
            const discount = Number(it.discount || 0)
            const tax = Number(it.tax || it.taxPercentage || 0)
            const recQty = Number(it.receivedQuantity || 0)

            return {
              productId: it.productId,
              productName: it.productName || `Product #${it.productId}`,
              sku: it.sku || '—',
              variantId: it.variantId || null,
              variantName: it.variantName || null,
              quantity: recQty > 0 ? recQty : 1,
              price: rawPrice,
              unitCost: rawPrice,
              discount: discount,
              tax: tax,
              receivedQuantity: recQty,
              error: null,
            }
          })
        )
      }
    }
  }

  // Handle Return Qty input change with auto-correction & inline validation
  function handleQuantityChange(idx, rawValue) {
    setItems((prev) => {
      const next = [...prev]
      const currentItem = { ...next[idx] }
      const maxAllowed = currentItem.receivedQuantity > 0 ? currentItem.receivedQuantity : 999999

      // Enforce positive whole numbers only
      const digitsOnly = String(rawValue || '').replace(/[^0-9]/g, '')

      if (!digitsOnly || digitsOnly === '0') {
        currentItem.quantity = ''
        currentItem.error = 'Qty required (min 1).'
      } else {
        let num = parseInt(digitsOnly, 10)
        if (num > maxAllowed) {
          currentItem.quantity = maxAllowed
          currentItem.error = `Maximum return quantity is ${maxAllowed}.`
        } else {
          currentItem.quantity = num
          currentItem.error = null
        }
      }

      next[idx] = currentItem
      return next
    })
  }

  function handleRemoveItem(idx) {
    setItems((prev) => prev.filter((_, i) => i !== idx))
  }

  // Live ERP Financial Breakdown Calculations
  const calculatedTotals = useMemo(() => {
    let subtotal = 0
    let discount = 0
    let tax = 0

    items.forEach((item) => {
      const qty = Number(item.quantity || 0)
      const unitPrice = Number(item.price || item.unitCost || 0)
      const discPercent = Number(item.discount || 0)
      const taxRate = Number(item.tax || 0)

      const grossAmount = qty * unitPrice
      const discountAmount = grossAmount * (discPercent / 100)
      const taxableAmount = grossAmount - discountAmount
      const taxAmount = taxableAmount * (taxRate / 100)

      subtotal += grossAmount
      discount += discountAmount
      tax += taxAmount
    })

    const total = subtotal - discount + tax
    return {
      subtotal: Math.round(subtotal * 100) / 100,
      discount: Math.round(discount * 100) / 100,
      tax: Math.round(tax * 100) / 100,
      total: Math.round(total * 100) / 100,
    }
  }, [items])

  const hasValidationErrors = items.some(
    (it) => it.error || !it.quantity || Number(it.quantity) <= 0,
  )

  async function handleSubmit(e, submitForApproval = false) {
    if (e) e.preventDefault()

    if (!form.supplierId) {
      showToast({ type: 'error', title: 'Validation Error', message: 'Please select a supplier.' })
      return
    }

    if (!form.grnId) {
      showToast({ type: 'error', title: 'Validation Error', message: 'Please select a Goods Receipt (GRN).' })
      return
    }

    if (items.length === 0) {
      showToast({ type: 'error', title: 'Validation Error', message: 'At least one return line item is required.' })
      return
    }

    if (hasValidationErrors) {
      showToast({ type: 'error', title: 'Validation Error', message: 'Please correct invalid return quantities before submitting.' })
      return
    }

    const payload = {
      supplierId: Number(form.supplierId),
      grnId: Number(form.grnId),
      returnDate: form.returnDate,
      reason: form.reason,
      notes: form.notes,
      totalAmount: calculatedTotals.total,
      submitForApproval,
      items: items.map((it) => ({
        productId: Number(it.productId),
        variantId: it.variantId ? Number(it.variantId) : null,
        quantity: Number(it.quantity),
        price: Number(it.price || it.unitCost || 0),
      })),
    }

    setSubmitting(true)
    const response = await createPurchaseReturn(payload)
    setSubmitting(false)

    if (response.success && response.data) {
      showToast({
        type: 'success',
        title: 'Purchase Return Saved',
        message: `${response.data.returnNumber || 'PR'} saved successfully!`,
      })
      navigate('/purchase-returns/returns')
    } else {
      showToast({
        type: 'error',
        title: 'Submission Failed',
        message: getPurchaseReturnErrorMessage(response, 'Failed to save purchase return.'),
      })
    }
  }

  return (
    <div className="create-sales-return-page">
      {/* Clean Compact Header */}
      <header className="sales-page__compact-header" style={{ marginBottom: '0.25rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <Link to="/purchase-returns/returns" className="button button-secondary" style={{ minHeight: '34px', padding: '0 0.65rem' }} title="Back to Purchase Returns">
            <ArrowLeft size={16} />
          </Link>
          <div>
            <h1 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#111827', margin: 0, lineHeight: 1.2 }}>
              {isEditing ? 'Edit Purchase Return' : 'Create Purchase Return'}
            </h1>
            <p style={{ fontSize: '0.8125rem', color: '#6b7280', margin: '0.1rem 0 0 0' }}>
              Select a Goods Receipt (GRN) to return products back to the supplier.
            </p>
          </div>
        </div>

        <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
          <Link className="button button-secondary" to="/purchase-returns/returns">
            Cancel
          </Link>
          <button
            type="button"
            disabled={submitting || loadingRefs || hasValidationErrors}
            onClick={(e) => handleSubmit(e, false)}
            className="button button-secondary"
          >
            <Save size={16} /> Save Draft
          </button>
          <button
            type="button"
            disabled={submitting || loadingRefs || hasValidationErrors}
            onClick={(e) => handleSubmit(e, true)}
            className="button button-primary"
          >
            <Send size={16} /> Submit for Approval
          </button>
        </div>
      </header>

      {/* Row 1: Two Clean Cards */}
      <div className="create-sales-return-top-grid">
        {/* Left Card: Supplier & Goods Receipt */}
        <div className="sales-returns-card" style={{ padding: '28px' }}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
            <FormControl id="supplier-select" label="Supplier" required>
              <Select
                id="supplier-select"
                disabled={loadingRefs || isEditing}
                value={form.supplierId}
                onChange={(e) => handleSupplierChange(e.target.value)}
                required
              >
                <option value="">Select Supplier...</option>
                {suppliers.map((s) => (
                  <option key={s.supplierId || s.id} value={s.supplierId || s.id}>
                    {s.name} {s.supplierCode ? `(${s.supplierCode})` : ''}
                  </option>
                ))}
              </Select>
            </FormControl>

            <FormControl id="grn-select" label="Goods Receipt (GRN)" required>
              <Select
                id="grn-select"
                disabled={loadingRefs || !form.supplierId || isEditing}
                value={form.grnId}
                onChange={(e) => handleGrnChange(e.target.value)}
                required
              >
                <option value="">Select Goods Receipt...</option>
                {grns.map((g) => (
                  <option key={g.grnId || g.id} value={g.grnId || g.id}>
                    {g.grnNumber || `GRN-${g.grnId}`} {g.receiptDate ? `(${new Date(g.receiptDate).toLocaleDateString()})` : ''}
                  </option>
                ))}
              </Select>
            </FormControl>
          </div>
        </div>

        {/* Right Card: Reason for Return & Return Date */}
        <div className="sales-returns-card" style={{ padding: '28px' }}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
            <FormControl id="reason-select" label="Reason for Return">
              <Select
                id="reason-select"
                value={form.reason}
                onChange={(e) => setForm((prev) => ({ ...prev, reason: e.target.value }))}
              >
                <option value="Defective / Damaged Item">Defective / Damaged Item</option>
                <option value="Wrong Product Delivered">Wrong Product Delivered</option>
                <option value="Quality Not as Per Specification">Quality Not as Per Specification</option>
                <option value="Excess Inventory Returned">Excess Inventory Returned</option>
                <option value="Expired Stock">Expired Stock</option>
                <option value="Other">Other Reason</option>
              </Select>
            </FormControl>

            <FormControl id="return-date-input" label="Return Date" required>
              <Input
                id="return-date-input"
                type="date"
                value={form.returnDate}
                onChange={(e) => setForm((prev) => ({ ...prev, returnDate: e.target.value }))}
                required
              />
            </FormControl>
          </div>
        </div>
      </div>

      {/* Row 2: Full Width Additional Notes Card */}
      <div className="sales-returns-card" style={{ padding: '20px 28px', marginTop: '1.15rem', marginBottom: '1.25rem' }}>
        <FormControl id="notes-input" label="Additional Notes">
          <textarea
            id="notes-input"
            rows={2}
            placeholder="Enter supplier return notes or debit memo remarks..."
            value={form.notes}
            onChange={(e) => setForm((prev) => ({ ...prev, notes: e.target.value }))}
            style={{
              width: '100%',
              padding: '0.65rem 0.75rem',
              border: '1px solid #d1d5db',
              borderRadius: '8px',
              fontSize: '0.875rem',
              outline: 'none',
              boxSizing: 'border-box',
              fontFamily: 'inherit',
            }}
          />
        </FormControl>
      </div>

      {/* Row 3: Returnable Products Table Card */}
      <div className="sales-returns-card-table">
        <div className="sales-returns-card-header">
          <h2 className="sales-returns-card-title">
            Returnable Products {items.length > 0 ? `(${items.length})` : ''}
          </h2>
        </div>

        {!form.grnId ? (
          <div style={{ padding: '3rem 2rem', textAlign: 'center', color: '#9ca3af', fontSize: '0.875rem' }}>
            Select a Goods Receipt (GRN) above to load returnable products.
          </div>
        ) : loadingGrnItems ? (
          <div style={{ padding: '3rem 2rem', textAlign: 'center', color: '#6b7280', fontSize: '0.875rem' }}>
            Loading products from Goods Receipt...
          </div>
        ) : items.length === 0 ? (
          <div style={{ padding: '3rem 2rem', textAlign: 'center', color: '#6b7280', fontSize: '0.875rem' }}>
            No returnable products found on this Goods Receipt.
          </div>
        ) : (
          <div className="sales-returns-table-wrapper">
            <table className="sales-returns-table" style={{ width: '100%' }}>
              <thead>
                <tr>
                  <th style={{ width: '36%', textAlign: 'left' }}>Product</th>
                  <th style={{ width: '12%', textAlign: 'left' }}>SKU</th>
                  <th style={{ width: '10%', textAlign: 'center' }}>Rec. Qty</th>
                  <th style={{ width: '13%', textAlign: 'center' }}>Return Qty</th>
                  <th style={{ width: '13%', textAlign: 'right' }}>Unit Cost</th>
                  <th style={{ width: '8%', textAlign: 'center' }}>Disc %</th>
                  <th style={{ width: '8%', textAlign: 'center' }}>Tax %</th>
                  <th style={{ width: '14%', textAlign: 'right' }}>Line Total</th>
                  <th style={{ width: '6%', textAlign: 'center' }}>Action</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item, idx) => {
                  const unitPrice = Number(item.price || item.unitCost || 0)
                  const qty = Number(item.quantity || 0)
                  const discPercent = Number(item.discount || 0)
                  const taxRate = Number(item.tax || 0)

                  const grossAmount = qty * unitPrice
                  const discountAmount = grossAmount * (discPercent / 100)
                  const taxableAmount = grossAmount - discountAmount
                  const taxAmount = taxableAmount * (taxRate / 100)
                  const lineTotal = taxableAmount + taxAmount

                  return (
                    <tr key={idx}>
                      <td className="sales-returns-col-product" style={{ width: '36%', verticalAlign: 'middle' }}>
                        <div className="sales-returns-product-name" title={item.productName}>
                          {item.productName}
                        </div>
                        {item.variantName ? (
                          <div className="sales-returns-product-sku">Variant: {item.variantName}</div>
                        ) : null}
                      </td>
                      <td style={{ width: '12%', verticalAlign: 'middle' }}>
                        <span className="sales-returns-product-sku" style={{ fontFamily: 'monospace' }}>
                          {item.sku || '—'}
                        </span>
                      </td>
                      <td style={{ width: '10%', textAlign: 'center', color: '#4b5563', fontWeight: 500, verticalAlign: 'middle' }}>
                        {item.receivedQuantity ?? '—'}
                      </td>
                      <td style={{ width: '13%', textAlign: 'center', verticalAlign: 'middle' }}>
                        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                          <input
                            type="text"
                            inputMode="numeric"
                            pattern="[0-9]*"
                            className={`sales-returns-qty-input ${item.error ? 'sales-returns-qty-input--error' : ''}`}
                            style={{ width: '85px', textAlign: 'center' }}
                            value={item.quantity}
                            onChange={(e) => handleQuantityChange(idx, e.target.value)}
                            placeholder="1"
                            required
                          />
                          {item.error ? (
                            <span style={{ fontSize: '0.7rem', color: '#dc2626', fontWeight: 600, marginTop: '2px', whiteSpace: 'nowrap' }}>
                              {item.error}
                            </span>
                          ) : null}
                        </div>
                      </td>
                      <td style={{ width: '13%', textAlign: 'right', color: '#374151', fontWeight: 500, verticalAlign: 'middle' }}>
                        {formatCurrency(unitPrice)}
                      </td>
                      <td style={{ width: '8%', textAlign: 'center', color: '#6b7280', fontSize: '0.875rem', verticalAlign: 'middle' }}>
                        {discPercent ? `${discPercent}%` : '0%'}
                      </td>
                      <td style={{ width: '8%', textAlign: 'center', color: '#6b7280', fontSize: '0.875rem', verticalAlign: 'middle' }}>
                        {taxRate ? `${taxRate}%` : '0%'}
                      </td>
                      <td style={{ width: '14%', textAlign: 'right', fontWeight: 700, color: '#059669', verticalAlign: 'middle' }}>
                        {formatCurrency(lineTotal)}
                      </td>
                      <td style={{ width: '6%', textAlign: 'center', verticalAlign: 'middle' }}>
                        <button
                          type="button"
                          onClick={() => handleRemoveItem(idx)}
                          style={{ background: 'transparent', border: 'none', color: '#ef4444', cursor: 'pointer', padding: '4px' }}
                          title="Remove Item"
                        >
                          <Trash2 size={16} />
                        </button>
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Row 4: Full-Width Financial Summary & Action Bar */}
      <div className="sales-returns-card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1.25rem' }}>
          <div style={{ flex: '1', minWidth: '240px' }}>
            <h3 style={{ fontSize: '0.85rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em', margin: 0 }}>
              Financial Breakdown
            </h3>
            <p style={{ fontSize: '0.8125rem', color: '#6b7280', margin: '0.25rem 0 0 0' }}>
              Total return amount calculated automatically based on item quantities, discounts, and applied taxes.
            </p>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: '2rem', flexWrap: 'wrap' }}>
            <div style={{ textAlign: 'right' }}>
              <span style={{ fontSize: '0.75rem', color: '#6b7280', display: 'block' }}>Subtotal</span>
              <span style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827' }}>{formatCurrency(calculatedTotals.subtotal)}</span>
            </div>

            <div style={{ textAlign: 'right' }}>
              <span style={{ fontSize: '0.75rem', color: '#6b7280', display: 'block' }}>Discount</span>
              <span style={{ fontSize: '0.95rem', fontWeight: 600, color: calculatedTotals.discount > 0 ? '#dc2626' : '#111827' }}>
                {calculatedTotals.discount > 0 ? `-${formatCurrency(calculatedTotals.discount)}` : formatCurrency(0)}
              </span>
            </div>

            <div style={{ textAlign: 'right' }}>
              <span style={{ fontSize: '0.75rem', color: '#6b7280', display: 'block' }}>Tax Amount</span>
              <span style={{ fontSize: '0.95rem', fontWeight: 600, color: '#111827' }}>{formatCurrency(calculatedTotals.tax)}</span>
            </div>

            <div style={{ textAlign: 'right', paddingLeft: '1.25rem', borderLeft: '1px solid #e5e7eb' }}>
              <span style={{ fontSize: '0.75rem', color: '#059669', fontWeight: 600, display: 'block' }}>Total Return Amount</span>
              <span style={{ fontSize: '1.25rem', fontWeight: 700, color: '#059669' }}>{formatCurrency(calculatedTotals.total)}</span>
            </div>
          </div>
        </div>

        <div style={{ marginTop: '1.25rem', paddingTop: '1rem', borderTop: '1px solid #e5e7eb', display: 'flex', gap: '0.75rem', justifyContent: 'flex-end' }}>
          <Link className="button button-secondary" to="/purchase-returns/returns">
            Cancel
          </Link>
          <button
            type="button"
            disabled={submitting || loadingRefs || hasValidationErrors}
            onClick={(e) => handleSubmit(e, false)}
            className="button button-secondary"
          >
            <Save size={16} /> Save Draft
          </button>
          <button
            type="button"
            disabled={submitting || loadingRefs || hasValidationErrors}
            onClick={(e) => handleSubmit(e, true)}
            className="button button-primary"
          >
            <Send size={16} /> Submit for Approval
          </button>
        </div>
      </div>
    </div>
  )
}
