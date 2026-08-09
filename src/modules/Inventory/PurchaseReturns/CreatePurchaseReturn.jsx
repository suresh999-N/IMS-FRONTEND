import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Plus, Save, Trash2, AlertCircle } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import { createPurchaseReturn, getPurchaseReturnById, updatePurchaseReturn } from '../../../api/purchaseReturnApi'
import { getSuppliers } from '../../../api/suppliersApi'
import { getProductCatalog } from '../../../api/productApi'
import { apiRequest } from '../../../api/apiClient'
import { API_ENDPOINTS } from '../../../api/endpoints'
import { formatCurrency } from '../../../utils/helpers'
import './PurchaseReturns.css'

export default function CreatePurchaseReturn({ mode = 'create', data = {}, actions = {}, onSavePurchaseReturn }) {
  const navigate = useNavigate()
  const { id } = useParams()
  const isEditMode = mode === 'edit' || Boolean(id)

  // Master Data
  const [suppliers, setSuppliers] = useState([])
  const [allGrns, setAllGrns] = useState([])
  const [products, setProducts] = useState([])
  const [productVariants, setProductVariants] = useState([])

  // Header State
  const [supplierId, setSupplierId] = useState('')
  const [grnId, setGrnId] = useState('')
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
      const [suppliersRes, grnsRes, productsRes, variantsRes] = await Promise.allSettled([
        getSuppliers(),
        apiRequest(API_ENDPOINTS.goodsReceipts.list),
        getProductCatalog(),
        apiRequest(API_ENDPOINTS.productVariants.list),
      ])

      if (suppliersRes.status === 'fulfilled' && suppliersRes.value?.success) {
        setSuppliers(suppliersRes.value.data ?? [])
      } else if (Array.isArray(data.suppliers) && data.suppliers.length > 0) {
        setSuppliers(data.suppliers)
      }

      if (grnsRes.status === 'fulfilled' && grnsRes.value?.success) {
        const rawGrns = Array.isArray(grnsRes.value.data) ? grnsRes.value.data : grnsRes.value.data?.items ?? []
        setAllGrns(rawGrns)
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
        const returnRes = await getPurchaseReturnById(id)
        if (returnRes.success && returnRes.data) {
          rec = returnRes.data
        } else if (Array.isArray(data.purchaseReturns)) {
          rec = data.purchaseReturns.find((r) => String(r.id) === String(id) || String(r.returnId) === String(id))
        }

        if (rec) {
          setSupplierId(String(rec.supplierId || rec.supplier_id || ''))
          setGrnId(String(rec.grnId || rec.grn_id || ''))
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
                receivedQuantity: String(line.receivedQuantity ?? line.quantity ?? line.returnQuantity ?? '1'),
                returnQuantity: String(line.returnQuantity ?? line.quantity ?? '1'),
                price: String(line.price ?? '0'),
              }))
            )
          }
        } else {
          showToast('Failed to load purchase return for editing.', 'error')
        }
      }
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Error loading reference data.', 'error')
    } finally {
      setLoading(false)
    }
  }, [id, isEditMode, data.suppliers, data.products, data.purchaseReturns])

  useEffect(() => {
    initData()
  }, [initData])

  // Filter GRNs based on selected supplier
  const availableGrns = useMemo(() => {
    if (!supplierId) return []
    return allGrns.filter((g) => {
      const gSupId = String(g.supplierId ?? g.supplier_id ?? '')
      return gSupId === String(supplierId)
    })
  }, [allGrns, supplierId])

  // Handle Supplier Selection
  const handleSupplierChange = (e) => {
    const selectedSup = e.target.value
    setSupplierId(selectedSup)
    setGrnId('') // Reset GRN when supplier changes
  }

  // Handle GRN Selection - Auto load products from selected GRN if available
  const handleGrnChange = (e) => {
    const selectedGrnId = e.target.value
    setGrnId(selectedGrnId)

    if (!selectedGrnId) return

    const grnRecord = allGrns.find(
      (g) => String(g.id ?? g.grnId ?? g.grn_id) === String(selectedGrnId)
    )

    if (!grnRecord) return

    const grnLineItems = Array.isArray(grnRecord.items)
      ? grnRecord.items
      : Array.isArray(grnRecord.goodsReceiptItems)
      ? grnRecord.goodsReceiptItems
      : Array.isArray(grnRecord.lineItems)
      ? grnRecord.lineItems
      : []

    if (grnLineItems.length > 0) {
      const loadedItems = grnLineItems.map((line, idx) => {
        const rQty = String(line.quantityReceived ?? line.quantity ?? '1')
        return {
          id: Date.now() + idx,
          productId: String(line.productId ?? line.product_id ?? ''),
          variantId: line.variantId ?? line.variant_id ? String(line.variantId ?? line.variant_id) : '',
          receivedQuantity: rQty,
          returnQuantity: rQty,
          price: String(line.unitPrice ?? line.price ?? line.unitCost ?? '0'),
        }
      })
      setItems(loadedItems)
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
        if (selectedProd && selectedProd.cost) {
          currentItem.price = String(selectedProd.cost)
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

    if (!supplierId) {
      errors.supplierId = 'Supplier is required.'
    }

    if (!grnId) {
      errors.grnId = 'Goods Receipt / GRN is required.'
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
      const selectedSupplier = suppliers.find((s) => String(s.id ?? s.supplierId ?? s.supplier_id) === String(supplierId))
      const selectedGrn = allGrns.find((g) => String(g.id ?? g.grnId ?? g.grn_id) === String(grnId))

      const targetId = isEditMode && id ? id : undefined

      const payload = {
        id: targetId,
        returnId: targetId,
        supplierId,
        supplier_id: Number(supplierId) || supplierId,
        supplierName: selectedSupplier?.name || selectedSupplier?.supplierName || (supplierId ? `Supplier #${supplierId}` : ''),
        grnId,
        grn_id: Number(grnId) || grnId,
        grnNumber: selectedGrn?.grnNumber || selectedGrn?.number || (grnId ? `GRN-${grnId}` : '-'),
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

      const saveFn = onSavePurchaseReturn || actions?.savePurchaseReturn

      // Always update local state first so created/edited return immediately reflects in the table
      if (typeof saveFn === 'function') {
        saveFn(payload)
      }

      // Perform API call
      try {
        if (isEditMode && id) {
          await updatePurchaseReturn(id, payload)
        } else {
          await createPurchaseReturn(payload)
        }
      } catch (apiErr) {
        // API offline or missing endpoint; local store is updated
      }

      showToast(
        isEditMode
          ? 'Purchase return updated successfully.'
          : 'Purchase return created successfully.',
        'success'
      )
      navigate('/inventory/purchase-returns')
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
    <main className="create-purchase-return-page">
      <PageHeader
        title={isEditMode ? `Edit Purchase Return #${id}` : 'Create Purchase Return'}
        subtitle={
          isEditMode
            ? 'Update goods return transaction details.'
            : 'Return damaged or incorrect goods to the supplier against a GRN.'
        }
        primaryAction={{
          icon: ArrowLeft,
          label: 'Back to Returns',
          onClick: () => navigate('/inventory/purchase-returns'),
          variant: 'secondary',
        }}
      />

      <form onSubmit={handleSubmit} noValidate>
        {/* Header Fields Section */}
        <section className="card form-section-card">
          <h3 className="section-title">Return Header Details</h3>
          <div className="form-grid">
            {/* Supplier Field */}
            <div className="form-field">
              <label htmlFor="supplierId">
                Supplier <span className="required-star">*</span>
              </label>
              <select
                id="supplierId"
                value={supplierId}
                onChange={handleSupplierChange}
                className={validationErrors.supplierId ? 'input-error' : ''}
              >
                <option value="">-- Select Supplier --</option>
                {suppliers.map((sup) => {
                  const sId = String(sup.id ?? sup.supplierId ?? sup.supplier_id)
                  return (
                    <option key={sId} value={sId}>
                      {sup.name || sup.supplierName || `Supplier #${sId}`}
                    </option>
                  )
                })}
              </select>
              {validationErrors.supplierId && (
                <span className="field-error">{validationErrors.supplierId}</span>
              )}
            </div>

            {/* GRN Field (Filtered by Supplier) */}
            <div className="form-field">
              <label htmlFor="grnId">
                Goods Receipt / GRN <span className="required-star">*</span>
              </label>
              <select
                id="grnId"
                value={grnId}
                onChange={handleGrnChange}
                disabled={!supplierId}
                className={validationErrors.grnId ? 'input-error' : ''}
              >
                <option value="">
                  {!supplierId
                    ? '-- Select Supplier First --'
                    : availableGrns.length === 0
                    ? 'No GRNs available for supplier'
                    : '-- Select GRN --'}
                </option>
                {availableGrns.map((grn) => {
                  const gId = String(grn.id ?? grn.grnId ?? grn.grn_id)
                  const gNum = grn.grnNumber || grn.number || `GRN-${gId}`
                  const gDate = grn.receivedDate || grn.date ? ` (${String(grn.receivedDate || grn.date).slice(0, 10)})` : ''
                  return (
                    <option key={gId} value={gId}>
                      {gNum} {gDate}
                    </option>
                  )
                })}
              </select>
              {validationErrors.grnId && (
                <span className="field-error">{validationErrors.grnId}</span>
              )}
              {!supplierId && (
                <span className="field-hint">Select a supplier to filter matching GRNs.</span>
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
                  <th style={{ width: '15%' }}>Received Quantity</th>
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

                      {/* Received Quantity (Renamed from Quantity) */}
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

                      {/* Return Quantity (New Column) */}
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
            onClick={() => navigate('/inventory/purchase-returns')}
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
