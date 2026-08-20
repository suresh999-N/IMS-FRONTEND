import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Save, AlertCircle, Info, PackageSearch } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import {
  createSalesReturn,
  getSalesReturnById,
  getSalesReturnCustomers,
  getSalesReturnInvoices,
  getSalesReturnInvoiceItems,
  updateSalesReturn,
} from '../../../api/salesReturnApi'
import { getCustomers } from '../../../api/customersApi'
import { formatCurrency } from '../../../utils/helpers'
import './SalesReturns.css'

export default function CreateSalesReturn({ mode = 'create' }) {
  const navigate = useNavigate()
  const { id } = useParams()
  const isEditMode = mode === 'edit' || Boolean(id)

  // Master Data
  const [customers, setCustomers] = useState([])
  const [availableInvoices, setAvailableInvoices] = useState([])

  // Header State
  const [customerId, setCustomerId] = useState('')
  const [invoiceId, setInvoiceId] = useState('')
  const [returnDate, setReturnDate] = useState(() => new Date().toISOString().slice(0, 10))
  const [reason, setReason] = useState('')

  // Items State
  const [items, setItems] = useState([])

  // UI States
  const [loading, setLoading] = useState(true)
  const [loadingInvoices, setLoadingInvoices] = useState(false)
  const [loadingItems, setLoadingItems] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [validationErrors, setValidationErrors] = useState({})

  // Fetch initial data
  const initData = useCallback(async () => {
    setLoading(true)
    try {
      let custData = []
      const customersRes = await getSalesReturnCustomers()
      if (customersRes && customersRes.success && Array.isArray(customersRes.data) && customersRes.data.length > 0) {
        custData = customersRes.data
      } else {
        const fallbackCustRes = await getCustomers()
        if (fallbackCustRes && fallbackCustRes.success && Array.isArray(fallbackCustRes.data)) {
          custData = fallbackCustRes.data
        }
      }
      setCustomers(custData)

      if (isEditMode && id) {
        const returnRes = await getSalesReturnById(id)
        if (returnRes && returnRes.success && returnRes.data) {
          const rec = returnRes.data
          const editCustId = String(rec.customerId || rec.customer_id || '')
          const editInvId = String(rec.invoiceId || rec.invoice_id || '')

          setCustomerId(editCustId)
          setInvoiceId(editInvId)

          if (rec.returnDate || rec.return_date) {
            setReturnDate(String(rec.returnDate || rec.return_date).slice(0, 10))
          }
          setReason(rec.reason || '')

          if (editCustId) {
            const invRes = await getSalesReturnInvoices(editCustId)
            if (invRes && invRes.success && Array.isArray(invRes.data)) {
              setAvailableInvoices(invRes.data)
            }
          }

          if (Array.isArray(rec.items) && rec.items.length > 0) {
            setItems(
              rec.items.map((line, index) => ({
                id: line.id || line.salesReturnItemId || index + 1,
                productId: String(line.productId || line.product_id || ''),
                productName: line.productName || line.product?.name || `Product #${line.productId}`,
                variantId: line.variantId || line.variant_id ? String(line.variantId || line.variant_id) : '',
                variantName: line.variantName || '',
                invoicedQuantity: String(line.invoicedQuantity ?? line.receivedQuantity ?? line.quantity ?? '1'),
                remainingReturnableQuantity: Number(line.remainingReturnableQuantity ?? line.invoicedQuantity ?? line.quantity ?? '9999'),
                returnQuantity: String(line.returnQuantity ?? line.quantity ?? '1'),
                price: String(line.price ?? '0'),
              }))
            )
          }
        } else {
          showToast(returnRes?.error || 'Failed to load sales return for editing.', 'error')
        }
      }
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Error loading reference data.', 'error')
    } finally {
      setLoading(false)
    }
  }, [id, isEditMode])

  useEffect(() => {
    initData()
  }, [initData])

  // Handle Customer Selection
  const handleCustomerChange = async (e) => {
    const selectedCust = e.target.value
    setCustomerId(selectedCust)
    setInvoiceId('')
    setItems([])
    setAvailableInvoices([])

    if (!selectedCust) return

    setLoadingInvoices(true)
    try {
      const res = await getSalesReturnInvoices(selectedCust)
      if (res && res.success && Array.isArray(res.data)) {
        setAvailableInvoices(res.data)
        if (res.data.length === 0) {
          showToast('No invoices found for the selected customer.', 'info')
        }
      } else {
        setAvailableInvoices([])
        showToast(res?.error || 'Failed to fetch customer invoices.', 'error')
      }
    } catch (err) {
      setAvailableInvoices([])
      showToast(err instanceof Error ? err.message : 'Failed to fetch customer invoices.', 'error')
    } finally {
      setLoadingInvoices(false)
    }
  }

  // Handle Invoice Selection
  const handleInvoiceChange = async (e) => {
    const selectedInvId = e.target.value
    setInvoiceId(selectedInvId)
    setItems([])

    if (!selectedInvId) return

    setLoadingItems(true)
    try {
      const invItemsRes = await getSalesReturnInvoiceItems(selectedInvId)
      if (invItemsRes && invItemsRes.success && Array.isArray(invItemsRes.data)) {
        if (invItemsRes.data.length === 0) {
          showToast('No returnable line items found for this invoice.', 'error')
          return
        }

        const loadedItems = invItemsRes.data.map((line, idx) => {
          const invoicedQty = Number(line.invoicedQuantity ?? 1)
          const remainingQty = Number(line.remainingReturnableQuantity ?? invoicedQty)
          const defaultReturnQty = Math.max(0, remainingQty)

          return {
            id: line.invoiceItemId || Date.now() + idx,
            productId: String(line.productId),
            productName: line.productName || `Product #${line.productId}`,
            variantId: line.variantId ? String(line.variantId) : '',
            variantName: line.variantName || '',
            invoicedQuantity: String(invoicedQty),
            remainingReturnableQuantity: remainingQty,
            returnQuantity: String(defaultReturnQty),
            price: String(line.price ?? 0),
          }
        })

        setItems(loadedItems)

        const availableToReturn = loadedItems.some((item) => item.remainingReturnableQuantity > 0)
        if (!availableToReturn) {
          showToast('All items for this invoice have already been returned.', 'warning')
        }
      } else {
        showToast(invItemsRes?.error || 'Failed to load invoice items.', 'error')
      }
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Error loading invoice items.', 'error')
    } finally {
      setLoadingItems(false)
    }
  }

  // Item Return Quantity Handler
  const handleItemQuantityChange = (index, value) => {
    setItems((prev) => {
      const updated = [...prev]
      updated[index] = { ...updated[index], returnQuantity: value }
      return updated
    })
  }

  // Calculate Total Return Amount
  const totalReturnAmount = useMemo(() => {
    return items.reduce((sum, item) => {
      const qty = Number(item.returnQuantity) || 0
      const price = Number(item.price) || 0
      return sum + qty * price
    }, 0)
  }, [items])

  // Form Validation
  const validateForm = () => {
    const errors = {}

    if (!customerId) errors.customerId = 'Customer is required.'
    if (!invoiceId) errors.invoiceId = 'Invoice is required.'
    if (!returnDate) errors.returnDate = 'Return Date is required.'
    if (!reason.trim()) errors.reason = 'Reason for return is required.'

    if (items.length === 0) {
      errors.items = 'At least one return item is required.'
    } else {
      const itemErrors = []
      let hasPositiveReturn = false

      items.forEach((item, index) => {
        const errs = {}
        if (!item.productId) errs.productId = 'Product required.'
        const returnQtyNum = Number(item.returnQuantity)
        if (isNaN(returnQtyNum) || returnQtyNum < 0) {
          errs.returnQuantity = 'Return Qty cannot be negative.'
        } else if (returnQtyNum > 0) {
          hasPositiveReturn = true
        }

        if (
          item.remainingReturnableQuantity !== undefined &&
          returnQtyNum > Number(item.remainingReturnableQuantity)
        ) {
          errs.returnQuantity = `Return Qty exceeds remaining limit (${item.remainingReturnableQuantity}).`
        }

        if (Object.keys(errs).length > 0) itemErrors[index] = errs
      })

      if (!hasPositiveReturn) {
        errors.items = 'At least one item must have a return quantity greater than 0.'
      }

      if (itemErrors.length > 0) errors.itemErrors = itemErrors
    }

    setValidationErrors(errors)
    return Object.keys(errors).length === 0
  }

  // Submit Handler
  const handleSubmit = async (e) => {
    if (e && e.preventDefault) e.preventDefault()

    if (!validateForm()) {
      showToast('Please resolve validation errors before submitting.', 'error')
      return
    }

    setSubmitting(true)
    try {
      const validItems = items.filter((item) => Number(item.returnQuantity) > 0)

      const payload = {
        customerId: Number(customerId),
        invoiceId: Number(invoiceId),
        returnDate: new Date(returnDate).toISOString(),
        reason: reason.trim(),
        items: validItems.map((item) => ({
          productId: Number(item.productId),
          variantId: item.variantId ? Number(item.variantId) : null,
          returnQuantity: Number(item.returnQuantity),
        })),
      }

      let apiRes = null
      if (isEditMode && id) {
        apiRes = await updateSalesReturn(id, payload)
      } else {
        apiRes = await createSalesReturn(payload)
      }

      if (!apiRes || apiRes.success === false) {
        const errorMsg = apiRes?.error || apiRes?.message || 'Failed to save sales return on server.'
        showToast(errorMsg, 'error')
        return
      }

      showToast(
        isEditMode ? 'Sales return updated successfully.' : 'Sales return created successfully.',
        'success'
      )
      navigate('/pos/returns')
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Submission error occurred.', 'error')
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) {
    return <StateBlock state="loading" message="Loading form data..." />
  }

  return (
    <main className="create-sales-return-page">
      <PageHeader
        title={isEditMode ? `Edit Sales Return #${id}` : 'Create Sales Return'}
        subtitle={
          isEditMode
            ? 'Update customer product return details.'
            : 'Record customer item returns against an issued sales invoice.'
        }
        primaryAction={{
          icon: ArrowLeft,
          label: 'Back to Returns',
          onClick: () => navigate('/pos/returns'),
          variant: 'secondary',
        }}
      />

      <form onSubmit={handleSubmit} noValidate>
        {/* Step 1: Return Header Details */}
        <section className="card form-section-card">
          <div className="section-header-title-container">
            <span className="section-step-badge">1</span>
            <h3 className="section-title-text">Return Header Details</h3>
            <span className="section-subtitle-text">Select customer and invoice to load returnable items</span>
          </div>

          <div className="form-grid">
            {/* Customer Field */}
            <div className="form-field">
              <label htmlFor="customerId">
                Customer <span className="required-star">*</span>
              </label>
              <select
                id="customerId"
                value={customerId}
                onChange={handleCustomerChange}
                className={validationErrors.customerId ? 'input-error' : ''}
              >
                <option value="">-- Select Customer --</option>
                {customers.map((c) => {
                  const cId = String(c.id ?? c.customerId ?? c.customer_id)
                  const cName = c.name || c.customerName || `Customer #${cId}`
                  return (
                    <option key={cId} value={cId}>
                      {cName}
                    </option>
                  )
                })}
              </select>
              {validationErrors.customerId ? (
                <span className="field-error">{validationErrors.customerId}</span>
              ) : (
                <div className="field-hint-pill">
                  <Info size={13} /> Select customer first
                </div>
              )}
            </div>

            {/* Invoice Field */}
            <div className="form-field">
              <label htmlFor="invoiceId">
                Invoice <span className="required-star">*</span>
              </label>
              <select
                id="invoiceId"
                value={invoiceId}
                onChange={handleInvoiceChange}
                disabled={!customerId || loadingInvoices}
                className={validationErrors.invoiceId ? 'input-error' : ''}
              >
                <option value="">
                  {!customerId
                    ? '-- Select Customer First --'
                    : loadingInvoices
                    ? 'Loading customer invoices...'
                    : availableInvoices.length === 0
                    ? 'No invoices found for customer'
                    : '-- Select Invoice --'}
                </option>
                {availableInvoices.map((inv) => {
                  const iId = String(inv.id ?? inv.invoiceId ?? inv.invoice_id)
                  const iNum = inv.invoiceNumber || inv.invoiceNo || inv.number || `INV-${String(iId).padStart(6, '0')}`
                  const iDate = inv.invoiceDate || inv.date ? ` (${String(inv.invoiceDate || inv.date).slice(0, 10)})` : ''
                  const iTotal = inv.totalAmount !== undefined ? ` - ${formatCurrency(inv.totalAmount)}` : ''
                  return (
                    <option key={iId} value={iId}>
                      {iNum}{iDate}{iTotal}
                    </option>
                  )
                })}
              </select>
              {validationErrors.invoiceId && (
                <span className="field-error">{validationErrors.invoiceId}</span>
              )}
            </div>

            {/* Return Date Field */}
            <div className="form-field">
              <label htmlFor="returnDate">
                Return Date <span className="required-star">*</span>
              </label>
              <input
                id="returnDate"
                type="date"
                value={returnDate}
                onChange={(e) => setReturnDate(e.target.value)}
                className={validationErrors.returnDate ? 'input-error' : ''}
              />
              {validationErrors.returnDate && (
                <span className="field-error">{validationErrors.returnDate}</span>
              )}
            </div>

            {/* Reason Field */}
            <div className="form-field">
              <label htmlFor="reason">
                Reason for Return <span className="required-star">*</span>
              </label>
              <input
                id="reason"
                type="text"
                placeholder="Enter mandatory reason (e.g. Damaged item)..."
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                className={validationErrors.reason ? 'input-error' : ''}
              />
              {validationErrors.reason && (
                <span className="field-error">{validationErrors.reason}</span>
              )}
            </div>
          </div>
        </section>

        {/* Step 2: Returned Items */}
        <section className="card form-section-card">
          <div className="section-header-title-container">
            <span className="section-step-badge">2</span>
            <h3 className="section-title-text">Returned Line Items</h3>
            <span className="section-subtitle-text">Adjust quantity for items being returned to stock</span>
          </div>

          {validationErrors.items && (
            <div className="form-global-error">
              <AlertCircle size={16} /> {validationErrors.items}
            </div>
          )}

          {loadingItems ? (
            <StateBlock state="loading" message="Loading invoice line items..." />
          ) : items.length === 0 ? (
            <div className="empty-items-notice-card">
              <PackageSearch size={36} className="empty-icon" />
              <p className="empty-title">No Line Items Loaded</p>
              <p className="empty-desc">
                {invoiceId
                  ? 'No returnable line items found for the selected invoice.'
                  : 'Select a Customer and Invoice above to automatically populate returnable items.'}
              </p>
            </div>
          ) : (
            <div className="items-table-container">
              <table className="items-table">
                <thead>
                  <tr>
                    <th style={{ width: '28%' }}>Product *</th>
                    <th style={{ width: '18%' }}>Variant</th>
                    <th style={{ width: '14%' }}>Invoiced Qty</th>
                    <th style={{ width: '14%' }}>Returnable Qty</th>
                    <th style={{ width: '14%' }}>Return Qty *</th>
                    <th style={{ width: '12%' }}>Unit Price</th>
                    <th style={{ width: '14%' }} className="text-right">Line Total</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((item, index) => {
                    const itemErr = validationErrors.itemErrors?.[index] || {}
                    const lineTotal = (Number(item.returnQuantity) || 0) * (Number(item.price) || 0)

                    return (
                      <tr key={item.id}>
                        <td className="font-semibold">{item.productName || `Product #${item.productId}`}</td>
                        <td>{item.variantName || (item.variantId ? `Variant #${item.variantId}` : '-')}</td>
                        <td>
                          <input type="number" readOnly value={item.invoicedQuantity} className="input-readonly" />
                        </td>
                        <td>
                          <input type="number" readOnly value={item.remainingReturnableQuantity} className="input-readonly" />
                        </td>
                        <td>
                          <input
                            type="number"
                            step="0.01"
                            min="0"
                            max={item.remainingReturnableQuantity}
                            placeholder="0.00"
                            value={item.returnQuantity}
                            onChange={(e) => handleItemQuantityChange(index, e.target.value)}
                            className={itemErr.returnQuantity ? 'input-error' : ''}
                          />
                          {itemErr.returnQuantity && (
                            <span className="field-error">{itemErr.returnQuantity}</span>
                          )}
                        </td>
                        <td>
                          <input type="number" step="0.01" readOnly value={item.price} className="input-readonly" />
                        </td>
                        <td className="text-right font-semibold">{formatCurrency(lineTotal)}</td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          )}

          {items.length > 0 && (
            <div className="return-totals-summary">
              <div className="total-amount-box">
                <span className="total-label">Total Return Amount:</span>
                <span className="total-value">{formatCurrency(totalReturnAmount)}</span>
              </div>
            </div>
          )}
        </section>

        {/* Submit Actions */}
        <div className="form-submit-bar">
          <button
            type="button"
            className="erp-button erp-button--secondary"
            onClick={() => navigate('/pos/returns')}
            disabled={submitting}
            style={{ height: '40px', padding: '0 20px', borderRadius: '8px', fontWeight: 600 }}
          >
            Cancel
          </button>
          <button
            type="submit"
            className="erp-button erp-button--primary"
            disabled={submitting || loadingItems}
            style={{
              height: '40px',
              padding: '0 24px',
              borderRadius: '8px',
              fontWeight: 700,
              display: 'inline-flex',
              alignItems: 'center',
              gap: '8px',
            }}
          >
            <Save size={16} /> {submitting ? 'Saving Return...' : isEditMode ? 'Update Return' : 'Save Return'}
          </button>
        </div>
      </form>
    </main>
  )
}
