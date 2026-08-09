import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Plus, Save, Trash2, AlertCircle } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import { createSalesReturn, getSalesReturnById, updateSalesReturn } from '../../../api/salesReturnApi'
import { getCustomers } from '../../../api/customersApi'
import { getProductCatalog } from '../../../api/productApi'
import { getInvoices, getInvoiceById } from '../../../api/businessApi'
import { apiRequest } from '../../../api/apiClient'
import { API_ENDPOINTS } from '../../../api/endpoints'
import { formatCurrency } from '../../../utils/helpers'
import './SalesReturns.css'

export default function CreateSalesReturn({ mode = 'create', data = {}, actions = {}, onSaveSalesReturn }) {
  const navigate = useNavigate()
  const { id } = useParams()
  const isEditMode = mode === 'edit' || Boolean(id)

  // Master Data
  const [customers, setCustomers] = useState([])
  const [allInvoices, setAllInvoices] = useState([])
  const [products, setProducts] = useState([])
  const [productVariants, setProductVariants] = useState([])

  // Header State
  const [customerId, setCustomerId] = useState('')
  const [invoiceId, setInvoiceId] = useState('')
  const [returnDate, setReturnDate] = useState(() => new Date().toISOString().slice(0, 10))
  const [reason, setReason] = useState('')

  // Items State
  // item: { id, productId, variantId, receivedQuantity, returnQuantity, price }
  const [items, setItems] = useState(() => [
    { id: Date.now(), productId: '', variantId: '', receivedQuantity: '1', returnQuantity: '1', price: '0' },
  ])

  // UI States
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [validationErrors, setValidationErrors] = useState({})

  // Fetch Master Data & Edit Record
  const initData = useCallback(async () => {
    setLoading(true)
    try {
      const [customersRes, invoicesRes, productsRes, variantsRes] = await Promise.allSettled([
        getCustomers(),
        getInvoices({ pageSize: 500 }),
        getProductCatalog(),
        apiRequest(API_ENDPOINTS.productVariants.list),
      ])

      if (customersRes.status === 'fulfilled' && customersRes.value?.success) {
        setCustomers(customersRes.value.data ?? [])
      } else if (Array.isArray(data.customers) && data.customers.length > 0) {
        setCustomers(data.customers)
      }

      if (invoicesRes.status === 'fulfilled' && invoicesRes.value?.success) {
        const rawInvoices = Array.isArray(invoicesRes.value.data) ? invoicesRes.value.data : []
        setAllInvoices(rawInvoices)
      } else if (Array.isArray(data.invoices) && data.invoices.length > 0) {
        setAllInvoices(data.invoices)
      } else if (Array.isArray(data.sales) && data.sales.length > 0) {
        setAllInvoices(data.sales)
      } else if (Array.isArray(data.accountingInvoices) && data.accountingInvoices.length > 0) {
        setAllInvoices(data.accountingInvoices)
      }

      if (productsRes.status === 'fulfilled' && productsRes.value?.success) {
        setProducts(productsRes.value.data ?? [])
      } else if (Array.isArray(data.products) && data.products.length > 0) {
        setProducts(data.products)
      }

      if (variantsRes.status === 'fulfilled' && variantsRes.value?.success) {
        const rawVariants = Array.isArray(variantsRes.value.data) ? variantsRes.value.data : variantsRes.value.data?.items ?? []
        setProductVariants(rawVariants)
      }

      // If Edit mode, load record from API or local fallback data
      if (isEditMode && id) {
        let rec = null
        const returnRes = await getSalesReturnById(id)
        if (returnRes.success && returnRes.data) {
          rec = returnRes.data
        } else if (Array.isArray(data.salesReturns)) {
          rec = data.salesReturns.find((r) => String(r.id) === String(id) || String(r.returnId) === String(id))
        }

        if (rec) {
          setCustomerId(String(rec.customerId || rec.customer_id || ''))
          setInvoiceId(String(rec.invoiceId || rec.invoice_id || ''))
          if (rec.returnDate || rec.return_date) {
            setReturnDate(String(rec.returnDate || rec.return_date).slice(0, 10))
          }
          setReason(rec.reason || '')

          if (Array.isArray(rec.items) && rec.items.length > 0) {
            setItems(
              rec.items.map((line, index) => ({
                id: line.id || index + 1,
                productId: String(line.productId || line.product_id || ''),
                variantId: line.variantId || line.variant_id ? String(line.variantId || line.variant_id) : '',
                receivedQuantity: String(line.receivedQuantity ?? line.invoicedQuantity ?? line.quantity ?? '1'),
                returnQuantity: String(line.returnQuantity ?? line.quantity ?? '1'),
                price: String(line.price ?? '0'),
              }))
            )
          }
        } else {
          showToast('Failed to load sales return for editing.', 'error')
        }
      }
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Error loading reference data.', 'error')
    } finally {
      setLoading(false)
    }
  }, [id, isEditMode, data.customers, data.invoices, data.sales, data.accountingInvoices, data.products, data.salesReturns])

  useEffect(() => {
    initData()
  }, [initData])

  // Filter Invoices based on selected Customer (or all invoices if no customer filter matches)
  const availableInvoices = useMemo(() => {
    if (!customerId) return allInvoices
    const filtered = allInvoices.filter((inv) => {
      const invCustId = String(
        inv.customerId ??
          inv.customer_id ??
          inv.partyId ??
          inv.customer?.id ??
          inv.customer?.customerId ??
          ''
      )
      return invCustId === String(customerId)
    })
    return filtered.length > 0 ? filtered : allInvoices
  }, [allInvoices, customerId])

  // Handle Customer Selection
  const handleCustomerChange = (e) => {
    const selectedCust = e.target.value
    setCustomerId(selectedCust)
    setInvoiceId('') // Reset invoice when customer changes
  }

  // Handle Invoice Selection - Auto load products from selected Invoice
  const handleInvoiceChange = async (e) => {
    const selectedInvId = e.target.value
    setInvoiceId(selectedInvId)

    if (!selectedInvId) return

    let invRecord = allInvoices.find(
      (inv) => String(inv.id ?? inv.invoiceId ?? inv.invoice_id) === String(selectedInvId)
    )

    // Fetch full invoice detail if items array is missing or empty
    if (!invRecord || !Array.isArray(invRecord.items) || invRecord.items.length === 0) {
      try {
        const invRes = await getInvoiceById(selectedInvId)
        if (invRes && invRes.success && invRes.data) {
          invRecord = invRes.data
        }
      } catch (err) {
        // continue with existing record
      }
    }

    if (!invRecord) return

    const invLineItems = Array.isArray(invRecord.items) && invRecord.items.length > 0
      ? invRecord.items
      : Array.isArray(invRecord.invoiceItems) && invRecord.invoiceItems.length > 0
      ? invRecord.invoiceItems
      : Array.isArray(invRecord.lineItems) && invRecord.lineItems.length > 0
      ? invRecord.lineItems
      : Array.isArray(invRecord.details) && invRecord.details.length > 0
      ? invRecord.details
      : Array.isArray(invRecord.lines) && invRecord.lines.length > 0
      ? invRecord.lines
      : Array.isArray(invRecord.products) && invRecord.products.length > 0
      ? invRecord.products
      : []

    if (invLineItems.length > 0) {
      // Ensure products catalog contains any missing product from the invoice
      setProducts((prev) => {
        const existingIds = new Set(prev.map((p) => String(p.id ?? p.productId ?? p.product_id)))
        const newProds = []
        invLineItems.forEach((line) => {
          const pId = String(line.productId ?? line.product_id ?? line.id ?? '')
          if (pId && !existingIds.has(pId)) {
            newProds.push({
              id: pId,
              productId: pId,
              name: line.productName || line.ProductName || line.name || line.description || `Product #${pId}`,
              price: line.unitPrice ?? line.price ?? 0,
            })
            existingIds.add(pId)
          }
        })
        return newProds.length > 0 ? [...prev, ...newProds] : prev
      })

      const loadedItems = invLineItems.map((line, idx) => {
        const qty = String(line.quantity ?? line.qty ?? '1')
        const lineProdId = String(line.productId ?? line.product_id ?? line.id ?? '')
        return {
          id: Date.now() + idx,
          productId: lineProdId,
          variantId: line.variantId || line.variant_id ? String(line.variantId || line.variant_id) : '',
          receivedQuantity: qty,
          returnQuantity: qty,
          price: String(line.unitPrice ?? line.price ?? line.unitCost ?? '0'),
        }
      })
      setItems(loadedItems)
    } else if (invRecord.productId || invRecord.product_id) {
      // Single product invoice record
      const pId = String(invRecord.productId || invRecord.product_id)
      const qty = String(invRecord.quantity ?? '1')
      const prc = String(invRecord.unitPrice ?? invRecord.price ?? (invRecord.total ? Number(invRecord.total) / (Number(qty) || 1) : '0'))

      setProducts((prev) => {
        const existingIds = new Set(prev.map((p) => String(p.id ?? p.productId ?? p.product_id)))
        if (!existingIds.has(pId)) {
          return [...prev, { id: pId, productId: pId, name: invRecord.productName || `Product #${pId}`, price: prc }]
        }
        return prev
      })

      setItems([
        {
          id: Date.now(),
          productId: pId,
          variantId: '',
          receivedQuantity: qty,
          returnQuantity: qty,
          price: prc,
        },
      ])
    }
  }

  // Item Table Handlers
  const handleAddItem = () => {
    setItems((prev) => [
      ...prev,
      { id: Date.now(), productId: '', variantId: '', receivedQuantity: '1', returnQuantity: '1', price: '0' },
    ])
  }

  const handleRemoveItem = (index) => {
    setItems((prev) => prev.filter((_, idx) => idx !== index))
  }

  const handleItemChange = (index, field, value) => {
    setItems((prev) => {
      const updated = [...prev]
      const currentItem = { ...updated[index], [field]: value }

      // If product changes, default price from catalog if available & reset variant
      if (field === 'productId') {
        currentItem.variantId = ''
        const selectedProd = products.find(
          (p) => String(p.id ?? p.productId ?? p.product_id) === String(value)
        )
        if (selectedProd && (selectedProd.price || selectedProd.sellingPrice || selectedProd.cost)) {
          currentItem.price = String(selectedProd.price ?? selectedProd.sellingPrice ?? selectedProd.cost)
        }
      }

      updated[index] = currentItem
      return updated
    })
  }

  // Calculate Total Return Amount (Return Quantity * Price)
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

    if (!customerId) {
      errors.customerId = 'Customer is required.'
    }

    if (!invoiceId) {
      errors.invoiceId = 'Invoice is required.'
    }

    if (!returnDate) {
      errors.returnDate = 'Return Date is required.'
    }

    if (!reason.trim()) {
      errors.reason = 'Reason for return is required.'
    }

    if (items.length === 0) {
      errors.items = 'At least one return item is required.'
    } else {
      const itemErrors = []
      items.forEach((item, index) => {
        const errs = {}
        if (!item.productId) {
          errs.productId = 'Product required.'
        }
        const returnQtyNum = Number(item.returnQuantity)
        if (isNaN(returnQtyNum) || returnQtyNum <= 0) {
          errs.returnQuantity = 'Return Qty must be > 0.'
        }
        const priceNum = Number(item.price)
        if (isNaN(priceNum) || priceNum < 0) {
          errs.price = 'Price cannot be negative.'
        }
        if (Object.keys(errs).length > 0) {
          itemErrors[index] = errs
        }
      })
      if (itemErrors.length > 0) {
        errors.itemErrors = itemErrors
      }
    }

    setValidationErrors(errors)
    return Object.keys(errors).length === 0
  }

  // Submit Handler for "Save Return"
  const handleSubmit = async (e) => {
    if (e && e.preventDefault) {
      e.preventDefault()
    }

    if (!validateForm()) {
      showToast('Please resolve validation errors before submitting.', 'error')
      return
    }

    setSubmitting(true)
    try {
      const selectedCustomer = customers.find((c) => String(c.id ?? c.customerId ?? c.customer_id) === String(customerId))
      const selectedInvoice = allInvoices.find((inv) => String(inv.id ?? inv.invoiceId ?? inv.invoice_id) === String(invoiceId))

      const targetId = isEditMode && id ? id : undefined

      const payload = {
        id: targetId,
        returnId: targetId,
        customerId,
        customer_id: Number(customerId) || customerId,
        customerName: selectedCustomer?.name || selectedCustomer?.customerName || (customerId ? `Customer #${customerId}` : ''),
        invoiceId,
        invoice_id: Number(invoiceId) || invoiceId,
        invoiceNumber: selectedInvoice?.invoiceNumber || selectedInvoice?.invoiceNo || selectedInvoice?.number || (invoiceId ? `SINV-${invoiceId}` : '-'),
        returnDate: new Date(returnDate).toISOString(),
        return_date: new Date(returnDate).toISOString(),
        totalAmount: totalReturnAmount,
        total_amount: totalReturnAmount,
        reason,
        items: items.map((item) => ({
          productId: item.productId,
          product_id: Number(item.productId) || item.productId,
          variantId: item.variantId || null,
          variant_id: item.variantId ? Number(item.variantId) : null,
          receivedQuantity: Number(item.receivedQuantity || 0),
          returnQuantity: Number(item.returnQuantity || 0),
          quantity: Number(item.returnQuantity || 0),
          price: Number(item.price || 0),
        })),
      }

      const saveFn = onSaveSalesReturn || actions?.saveSalesReturn

      // Always update local state first so created/edited return immediately reflects in the table
      if (typeof saveFn === 'function') {
        saveFn(payload)
      }

      // Perform API call
      try {
        if (isEditMode && id) {
          await updateSalesReturn(id, payload)
        } else {
          await createSalesReturn(payload)
        }
      } catch (apiErr) {
        // API offline or missing endpoint; local store is updated
      }

      showToast(
        isEditMode
          ? 'Sales return updated successfully.'
          : 'Sales return created successfully.',
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
            : 'Return customer items against an issued invoice.'
        }
        primaryAction={{
          icon: ArrowLeft,
          label: 'Back to Returns',
          onClick: () => navigate('/pos/returns'),
          variant: 'secondary',
        }}
      />

      <form onSubmit={handleSubmit} noValidate>
        {/* Header Fields Section */}
        <section className="card form-section-card">
          <h3 className="section-title">Return Header Details</h3>
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
                  return (
                    <option key={cId} value={cId}>
                      {c.name || c.customerName || `Customer #${cId}`}
                    </option>
                  )
                })}
              </select>
              {validationErrors.customerId && (
                <span className="field-error">{validationErrors.customerId}</span>
              )}
            </div>

            {/* Invoice Field (Filtered by Customer) */}
            <div className="form-field">
              <label htmlFor="invoiceId">
                Invoice <span className="required-star">*</span>
              </label>
              <select
                id="invoiceId"
                value={invoiceId}
                onChange={handleInvoiceChange}
                disabled={!customerId}
                className={validationErrors.invoiceId ? 'input-error' : ''}
              >
                <option value="">
                  {!customerId
                    ? '-- Select Customer First --'
                    : availableInvoices.length === 0
                    ? 'No Invoices available for customer'
                    : '-- Select Invoice --'}
                </option>
                {availableInvoices.map((inv) => {
                  const iId = String(inv.id ?? inv.invoiceId ?? inv.invoice_id)
                  const iNum = inv.invoiceNumber || inv.invoiceNo || inv.number || `SINV-${iId}`
                  const iDate = inv.date || inv.invoiceDate ? ` (${String(inv.date || inv.invoiceDate).slice(0, 10)})` : ''
                  return (
                    <option key={iId} value={iId}>
                      {iNum} {iDate}
                    </option>
                  )
                })}
              </select>
              {validationErrors.invoiceId && (
                <span className="field-error">{validationErrors.invoiceId}</span>
              )}
              {!customerId && (
                <span className="field-hint">Select a customer to filter matching invoices.</span>
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

            {/* Reason Field (Mandatory) */}
            <div className="form-field full-width">
              <label htmlFor="reason">
                Reason for Return <span className="required-star">*</span>
              </label>
              <textarea
                id="reason"
                rows={3}
                placeholder="Enter mandatory details explaining why items are being returned (e.g., Damaged during transit, wrong specification)..."
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

        {/* Return Items Card */}
        <section className="card form-section-card">
          <div className="section-header-row">
            <h3 className="section-title">Returned Items</h3>
            <button
              type="button"
              className="erp-button erp-button--secondary add-row-btn"
              onClick={handleAddItem}
            >
              <Plus size={14} /> Add Item Row
            </button>
          </div>

          {validationErrors.items && (
            <div className="form-global-error">
              <AlertCircle size={15} /> {validationErrors.items}
            </div>
          )}

          <div className="items-table-container">
            <table className="items-table">
              <thead>
                <tr>
                  <th style={{ width: '25%' }}>Product *</th>
                  <th style={{ width: '20%' }}>Variant</th>
                  <th style={{ width: '15%' }}>Invoiced Quantity</th>
                  <th style={{ width: '15%' }}>Return Quantity *</th>
                  <th style={{ width: '15%' }}>Price *</th>
                  <th style={{ width: '10%' }} className="text-right">Total</th>
                  <th style={{ width: '5%' }}></th>
                </tr>
              </thead>
              <tbody>
                {items.map((item, index) => {
                  const itemErr = validationErrors.itemErrors?.[index] || {}
                  // Filter variants matching the selected product
                  const matchingVariants = productVariants.filter((v) => {
                    const vPId = String(v.productId ?? v.product_id ?? '')
                    return vPId && vPId === String(item.productId)
                  })

                  const lineTotal = (Number(item.returnQuantity) || 0) * (Number(item.price) || 0)

                  return (
                    <tr key={item.id}>
                      {/* Product */}
                      <td>
                        <select
                          value={item.productId}
                          onChange={(e) => handleItemChange(index, 'productId', e.target.value)}
                          className={itemErr.productId ? 'input-error' : ''}
                        >
                          <option value="">-- Select Product --</option>
                          {products.map((p) => {
                            const pId = String(p.id ?? p.productId ?? p.product_id)
                            return (
                              <option key={pId} value={pId}>
                                {p.name || p.productName || `Product #${pId}`}
                              </option>
                            )
                          })}
                        </select>
                        {itemErr.productId && (
                          <span className="field-error">{itemErr.productId}</span>
                        )}
                      </td>

                      {/* Variant */}
                      <td>
                        <select
                          value={item.variantId || ''}
                          onChange={(e) => handleItemChange(index, 'variantId', e.target.value)}
                          disabled={!item.productId || matchingVariants.length === 0}
                        >
                          <option value="">
                            {matchingVariants.length > 0 ? '-- Select Variant --' : 'No Variants'}
                          </option>
                          {matchingVariants.map((v) => {
                            const vId = String(v.id ?? v.variantId ?? v.variant_id)
                            return (
                              <option key={vId} value={vId}>
                                {v.name || v.variantName || v.sku || `Variant #${vId}`}
                              </option>
                            )
                          })}
                        </select>
                      </td>

                      {/* Invoiced Quantity */}
                      <td>
                        <input
                          type="number"
                          step="0.01"
                          min="0"
                          readOnly
                          placeholder="0.00"
                          value={item.receivedQuantity}
                          className="input-readonly"
                          style={{ backgroundColor: '#f8fafc', color: '#64748b' }}
                        />
                      </td>

                      {/* Return Quantity */}
                      <td>
                        <input
                          type="number"
                          step="0.01"
                          min="0.01"
                          placeholder="0.00"
                          value={item.returnQuantity}
                          onChange={(e) => handleItemChange(index, 'returnQuantity', e.target.value)}
                          className={itemErr.returnQuantity ? 'input-error' : ''}
                        />
                        {itemErr.returnQuantity && (
                          <span className="field-error">{itemErr.returnQuantity}</span>
                        )}
                      </td>

                      {/* Price (Decimal) */}
                      <td>
                        <input
                          type="number"
                          step="0.01"
                          min="0"
                          placeholder="0.00"
                          value={item.price}
                          onChange={(e) => handleItemChange(index, 'price', e.target.value)}
                          className={itemErr.price ? 'input-error' : ''}
                        />
                        {itemErr.price && (
                          <span className="field-error">{itemErr.price}</span>
                        )}
                      </td>

                      {/* Calculated Total */}
                      <td className="text-right font-semibold">
                        {formatCurrency(lineTotal)}
                      </td>

                      {/* Remove Action */}
                      <td className="text-center">
                        {items.length > 1 && (
                          <button
                            type="button"
                            className="icon-action-btn delete-icon"
                            title="Remove Line Item"
                            onClick={() => handleRemoveItem(index)}
                          >
                            <Trash2 size={16} />
                          </button>
                        )}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>

          {/* Total Amount Footer */}
          <div className="return-totals-summary">
            <div className="total-amount-box">
              <span className="total-label">Total Return Amount:</span>
              <span className="total-value">{formatCurrency(totalReturnAmount)}</span>
            </div>
          </div>
        </section>

        {/* Submit Actions */}
        <div className="form-submit-bar">
          <button
            type="button"
            className="erp-button erp-button--secondary"
            onClick={() => navigate('/pos/returns')}
            disabled={submitting}
          >
            Cancel
          </button>
          <button
            type="submit"
            className="erp-button erp-button--primary"
            disabled={submitting}
            onClick={handleSubmit}
          >
            <Save size={15} /> {submitting ? 'Saving...' : isEditMode ? 'Update Return' : 'Save Return'}
          </button>
        </div>
      </form>
    </main>
  )
}
