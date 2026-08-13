import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { AlertCircle, ArrowLeft, Info, PackageSearch, Save } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import {
  createPurchaseReturn,
  getPurchaseReturnById,
  getPurchaseReturnGrnItems,
  getPurchaseReturnGrns,
  getPurchaseReturnSuppliers,
  updatePurchaseReturn,
} from '../../../api/purchaseReturnApi'
import { formatCurrency } from '../../../utils/helpers'
import './PurchaseReturns.css'

const getArrayFromResponse = (response) => {
  if (Array.isArray(response)) return response
  if (Array.isArray(response?.data)) return response.data
  if (Array.isArray(response?.data?.items)) return response.data.items
  if (Array.isArray(response?.items)) return response.items
  return null
}

const getResponseData = (response) => {
  if (response?.data !== undefined && !Array.isArray(response?.data)) {
    return response.data
  }
  return response
}

const getId = (item) => item?.id ?? item?.supplierId ?? item?.supplier_id
const getSupplierName = (supplier) =>
  supplier?.name ?? supplier?.supplierName ?? supplier?.supplier_name ?? (getId(supplier) ? `Supplier #${getId(supplier)}` : '-')

const getGrnId = (grn) => grn?.id ?? grn?.grnId ?? grn?.grn_id
const getGrnNumber = (grn) =>
  grn?.grnNumber ?? grn?.grn_number ?? grn?.number ?? (getGrnId(grn) ? `GRN-${getGrnId(grn)}` : '-')

const getProductId = (item) => item?.productId ?? item?.product_id
const getVariantId = (item) => item?.variantId ?? item?.variant_id
const getProductName = (item) =>
  item?.productName ?? item?.product_name ?? item?.name ?? (getProductId(item) ? `Product #${getProductId(item)}` : '-')
const getVariantName = (item) =>
  item?.variantName ?? item?.variant_name ?? item?.variant?.name ?? item?.sku ?? (getVariantId(item) ? `Variant #${getVariantId(item)}` : '')

const getNumber = (...values) => {
  for (const value of values) {
    if (value !== null && value !== undefined && value !== '') {
      const number = Number(value)
      if (Number.isFinite(number)) return number
    }
  }
  return 0
}

const normalizeGrnItem = (line, index = 0) => {
  const receivedQuantity = getNumber(
    line?.receivedQuantity,
    line?.quantityReceived,
    line?.received_quantity,
    line?.quantity,
  )

  const previouslyReturnedQuantity = getNumber(
    line?.previouslyReturnedQuantity,
    line?.returnedQuantity,
    line?.alreadyReturnedQuantity,
    line?.previously_returned_quantity,
    line?.already_returned_quantity,
  )

  const explicitReturnableQuantity =
    line?.returnableQuantity ??
    line?.availableReturnQuantity ??
    line?.remainingQuantity ??
    line?.returnable_quantity ??
    line?.available_return_quantity ??
    line?.remaining_quantity

  const calculatedReturnableQuantity = Math.max(0, receivedQuantity - previouslyReturnedQuantity)

  const returnableQuantity =
    explicitReturnableQuantity !== undefined &&
    explicitReturnableQuantity !== null &&
    explicitReturnableQuantity !== ''
      ? Math.max(0, getNumber(explicitReturnableQuantity))
      : calculatedReturnableQuantity

  return {
    id: `${Date.now()}-${index}-${Math.random()}`,
    productId: String(getProductId(line) ?? ''),
    variantId: String(getVariantId(line) ?? ''),
    productName: getProductName(line),
    variantName: getVariantName(line),
    receivedQuantity: String(receivedQuantity),
    previouslyReturnedQuantity: String(previouslyReturnedQuantity),
    returnableQuantity: String(returnableQuantity),
    returnQuantity: '0',
    price: String(
      getNumber(
        line?.unitPrice,
        line?.price,
        line?.unitCost,
        line?.purchasePrice,
        line?.unit_price,
        line?.unit_cost,
      )
    ),
  }
}

export default function CreatePurchaseReturn() {
  const navigate = useNavigate()
  const { id } = useParams()
  const isEditMode = Boolean(id)

  const [suppliers, setSuppliers] = useState([])
  const [allGrns, setAllGrns] = useState([])
  const [items, setItems] = useState([])

  const [supplierId, setSupplierId] = useState('')
  const [grnId, setGrnId] = useState('')
  const [returnDate, setReturnDate] = useState(() => new Date().toISOString().slice(0, 10))
  const [reason, setReason] = useState('')

  const [loading, setLoading] = useState(true)
  const [loadingGrnItems, setLoadingGrnItems] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState('')
  const [validationErrors, setValidationErrors] = useState({})

  // Load Reference Data
  const loadReferenceData = useCallback(async () => {
    setLoading(true)
    setError('')

    try {
      const suppliersResponse = await getPurchaseReturnSuppliers()
      const supplierData = getArrayFromResponse(suppliersResponse)

      if (supplierData === null) {
        throw new Error('Supplier API returned an unexpected response format.')
      }

      setSuppliers(supplierData)

      if (isEditMode) {
        const response = await getPurchaseReturnById(id)
        const record = getResponseData(response)

        if (!record) {
          throw new Error('Purchase return record not found.')
        }

        const recordSupplierId = record?.supplierId ?? record?.supplier_id ?? ''
        const recordGrnId = record?.grnId ?? record?.grn_id ?? ''

        setSupplierId(String(recordSupplierId))
        setGrnId(String(recordGrnId))

        if (recordSupplierId) {
          const grnsResponse = await getPurchaseReturnGrns(recordSupplierId)
          const grnData = getArrayFromResponse(grnsResponse)
          setAllGrns(grnData || [])
        }

        const dateValue = record?.returnDate ?? record?.return_date
        if (dateValue) {
          setReturnDate(String(dateValue).slice(0, 10))
        }

        setReason(record?.reason ?? '')

        const recordItems = Array.isArray(record?.items)
          ? record.items
          : Array.isArray(record?.returnItems)
          ? record.returnItems
          : []

        if (recordItems.length) {
          setItems(
            recordItems.map((line, index) => ({
              ...normalizeGrnItem(line, index),
              returnQuantity: String(getNumber(line?.returnQuantity, line?.quantity, line?.return_quantity)),
            }))
          )
        }
      }
    } catch (err) {
      setError(
        err?.response?.data?.message ||
        err?.response?.data?.title ||
        err?.message ||
        'Failed to load purchase return data.'
      )
    } finally {
      setLoading(false)
    }
  }, [id, isEditMode])

  useEffect(() => {
    loadReferenceData()
  }, [loadReferenceData])

  // Filter GRNs by Supplier
  const availableGrns = useMemo(() => {
    if (!supplierId) return []
    return allGrns.filter((grn) => {
      const grnSupplierId = grn?.supplierId ?? grn?.supplier_id ?? grn?.supplier?.id
      return String(grnSupplierId ?? '') === String(supplierId)
    })
  }, [allGrns, supplierId])

  // Total Return Amount Calculation
  const totalReturnAmount = useMemo(
    () =>
      items.reduce(
        (sum, item) => sum + getNumber(item.returnQuantity) * getNumber(item.price),
        0
      ),
    [items]
  )

  // Load GRN Items
  const loadGrnItems = useCallback(async (selectedGrnId) => {
    if (!selectedGrnId) {
      setItems([])
      return
    }

    setLoadingGrnItems(true)
    setValidationErrors((prev) => ({ ...prev, grnItems: undefined }))

    try {
      const response = await getPurchaseReturnGrnItems(selectedGrnId)
      const rawItems = getArrayFromResponse(response)

      if (rawItems === null) {
        throw new Error('GRN items API returned an unexpected response format.')
      }

      if (!rawItems.length) {
        throw new Error('No returnable items found for the selected GRN.')
      }

      const normalizedItems = rawItems
        .map(normalizeGrnItem)
        .filter((item) => getNumber(item.returnableQuantity) > 0)

      if (!normalizedItems.length) {
        throw new Error('All items in this GRN have already been returned.')
      }

      setItems(normalizedItems)
    } catch (err) {
      setItems([])
      setValidationErrors((prev) => ({
        ...prev,
        grnItems: err?.response?.data?.message || err?.message || 'Failed to load GRN returnable items.',
      }))
    } finally {
      setLoadingGrnItems(false)
    }
  }, [])

  // Handle Supplier Change
  const handleSupplierChange = async (event) => {
    const selectedSupplierId = event.target.value
    setSupplierId(selectedSupplierId)
    setGrnId('')
    setItems([])
    setAllGrns([])
    setValidationErrors({})

    if (selectedSupplierId) {
      try {
        const response = await getPurchaseReturnGrns(selectedSupplierId)
        const grnData = getArrayFromResponse(response)
        setAllGrns(grnData || [])
      } catch (err) {
        showToast('Failed to load Goods Receipts for selected supplier.', 'error')
      }
    }
  }

  // Handle GRN Change
  const handleGrnChange = async (event) => {
    const selectedGrnId = event.target.value
    setGrnId(selectedGrnId)
    setItems([])
    setValidationErrors({})

    if (selectedGrnId) {
      await loadGrnItems(selectedGrnId)
    }
  }

  // Handle Return Quantity Input
  const handleReturnQuantityChange = (index, value) => {
    setItems((previous) =>
      previous.map((item, itemIndex) =>
        itemIndex === index ? { ...item, returnQuantity: value } : item
      )
    )
    setValidationErrors((prev) => ({ ...prev, itemErrors: undefined }))
  }

  // Validation
  const validateForm = () => {
    const errors = {}
    if (!supplierId) errors.supplierId = 'Supplier is required.'
    if (!grnId) errors.grnId = 'Goods Receipt / GRN is required.'
    if (!returnDate) errors.returnDate = 'Return date is required.'
    if (!reason.trim()) errors.reason = 'Reason for return is required.'
    if (!items.length) errors.items = 'No returnable items available for this GRN.'

    const itemErrors = []
    items.forEach((item, index) => {
      const rowErrors = {}
      const returnQuantity = getNumber(item.returnQuantity)
      const returnableQuantity = getNumber(item.returnableQuantity)
      const price = getNumber(item.price)

      if (returnQuantity <= 0) {
        rowErrors.returnQuantity = 'Return quantity must be greater than 0.'
      }
      if (returnQuantity > returnableQuantity) {
        rowErrors.returnQuantity = `Cannot return more than ${returnableQuantity}.`
      }
      if (price < 0) {
        rowErrors.price = 'Unit price cannot be negative.'
      }

      if (Object.keys(rowErrors).length) {
        itemErrors[index] = rowErrors
      }
    })

    if (itemErrors.length) errors.itemErrors = itemErrors
    setValidationErrors(errors)
    return Object.keys(errors).length === 0
  }

  // Submit Handler
  const handleSubmit = async (event) => {
    event.preventDefault()
    if (submitting) return

    if (!validateForm()) {
      showToast('Please resolve validation errors before submitting.', 'error')
      return
    }

    setSubmitting(true)
    try {
      const payload = {
        supplierId: Number(supplierId),
        grnId: Number(grnId),
        returnDate,
        reason: reason.trim(),
        items: items.map((item) => ({
          productId: Number(item.productId),
          variantId: item.variantId ? Number(item.variantId) : null,
          returnQuantity: Number(item.returnQuantity),
        })),
      }

      if (isEditMode) {
        await updatePurchaseReturn(id, payload)
      } else {
        await createPurchaseReturn(payload)
      }

      showToast(
        isEditMode ? 'Purchase return updated successfully.' : 'Purchase return created successfully.',
        'success'
      )
      navigate('/inventory/purchase-returns')
    } catch (err) {
      showToast(err?.response?.data?.message || err?.message || 'Failed to save purchase return.', 'error')
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) {
    return <StateBlock state="loading" message="Loading purchase return data..." />
  }

  if (error) {
    return (
      <main className="create-purchase-return-page">
        <PageHeader title={isEditMode ? `Edit Purchase Return #${id}` : 'Create Purchase Return'} />
        <div className="card purchase-returns-error-card" style={{ padding: '24px', display: 'flex', alignItems: 'center', gap: '16px' }}>
          <AlertCircle size={24} className="error-icon" style={{ color: '#dc2626' }} />
          <div style={{ flex: 1 }}>
            <h3 style={{ margin: '0 0 4px 0' }}>Unable to load Purchase Return</h3>
            <p style={{ margin: 0, color: '#64748b' }}>{error}</p>
          </div>
          <button type="button" className="erp-button erp-button--primary" onClick={loadReferenceData}>
            Retry
          </button>
        </div>
      </main>
    )
  }

  return (
    <main className="create-purchase-return-page">
      <PageHeader
        title={isEditMode ? `Edit Purchase Return #${id}` : 'Create Purchase Return'}
        subtitle={
          isEditMode
            ? 'Update the supplier goods return transaction.'
            : 'Return defective or excess goods to the supplier against a GRN.'
        }
        primaryAction={{
          icon: ArrowLeft,
          label: 'Back to Returns',
          onClick: () => navigate('/inventory/purchase-returns'),
          variant: 'secondary',
        }}
      />

      <form onSubmit={handleSubmit} noValidate>
        {/* Step 1: Header Details */}
        <section className="card form-section-card">
          <div className="section-header-title-container">
            <span className="section-step-badge">1</span>
            <h3 className="section-title-text">Return Header Details</h3>
            <span className="section-subtitle-text">Select supplier and GRN to load returnable items</span>
          </div>

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
                disabled={isEditMode}
                className={validationErrors.supplierId ? 'input-error' : ''}
              >
                <option value="">-- Select Supplier --</option>
                {suppliers.map((supplier) => {
                  const supplierValue = String(getId(supplier) ?? '')
                  if (!supplierValue) return null
                  return (
                    <option key={supplierValue} value={supplierValue}>
                      {getSupplierName(supplier)}
                    </option>
                  )
                })}
              </select>
              {validationErrors.supplierId ? (
                <span className="field-error">{validationErrors.supplierId}</span>
              ) : (
                <div className="field-hint-pill">
                  <Info size={13} /> Select supplier first
                </div>
              )}
            </div>

            {/* GRN Field */}
            <div className="form-field">
              <label htmlFor="grnId">
                Goods Receipt / GRN <span className="required-star">*</span>
              </label>
              <select
                id="grnId"
                value={grnId}
                onChange={handleGrnChange}
                disabled={!supplierId || isEditMode}
                className={validationErrors.grnId ? 'input-error' : ''}
              >
                <option value="">
                  {!supplierId
                    ? '-- Select Supplier First --'
                    : availableGrns.length === 0
                    ? 'No eligible GRNs available'
                    : '-- Select GRN --'}
                </option>
                {availableGrns.map((grn) => {
                  const value = String(getGrnId(grn) ?? '')
                  if (!value) return null
                  const date = grn?.receivedDate ?? grn?.received_date ?? grn?.date
                  return (
                    <option key={value} value={value}>
                      {getGrnNumber(grn)}
                      {date ? ` (${String(date).slice(0, 10)})` : ''}
                    </option>
                  )
                })}
              </select>
              {validationErrors.grnId && <span className="field-error">{validationErrors.grnId}</span>}
              {validationErrors.grnItems && <span className="field-error">{validationErrors.grnItems}</span>}
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
                placeholder="Enter mandatory reason for returning goods..."
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

        {/* Step 2: Line Items */}
        <section className="card form-section-card">
          <div className="section-header-title-container">
            <span className="section-step-badge">2</span>
            <h3 className="section-title-text">Returned Line Items</h3>
            <span className="section-subtitle-text">Adjust quantity for items returned to supplier</span>
          </div>

          {validationErrors.items && (
            <div className="form-global-error">
              <AlertCircle size={16} /> {validationErrors.items}
            </div>
          )}

          {loadingGrnItems ? (
            <StateBlock state="loading" message="Loading returnable items from GRN..." />
          ) : items.length === 0 ? (
            <div className="empty-items-notice-card">
              <PackageSearch size={36} className="empty-icon" />
              <p className="empty-title">No Line Items Loaded</p>
              <p className="empty-desc">
                {grnId
                  ? 'No returnable items found for the selected GRN.'
                  : 'Select a Supplier and GRN above to automatically populate returnable items.'}
              </p>
            </div>
          ) : (
            <div className="items-table-container">
              <table className="items-table">
                <thead>
                  <tr>
                    <th style={{ width: '26%' }}>Product *</th>
                    <th style={{ width: '16%' }}>Variant</th>
                    <th style={{ width: '13%' }}>Received Qty</th>
                    <th style={{ width: '15%' }}>Previously Returned</th>
                    <th style={{ width: '13%' }}>Returnable Qty</th>
                    <th style={{ width: '13%' }}>Return Qty *</th>
                    <th style={{ width: '12%' }}>Unit Price</th>
                    <th style={{ width: '12%' }} className="text-right">Line Total</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((item, index) => {
                    const itemErr = validationErrors.itemErrors?.[index] || {}
                    const lineTotal = getNumber(item.returnQuantity) * getNumber(item.price)

                    return (
                      <tr key={item.id}>
                        <td className="font-semibold">{item.productName || `Product #${item.productId}`}</td>
                        <td>{item.variantName || (item.variantId ? `Variant #${item.variantId}` : '-')}</td>
                        <td>
                          <input type="number" readOnly value={item.receivedQuantity} className="input-readonly" />
                        </td>
                        <td>
                          <input type="number" readOnly value={item.previouslyReturnedQuantity} className="input-readonly" />
                        </td>
                        <td>
                          <input type="number" readOnly value={item.returnableQuantity} className="input-readonly" />
                        </td>
                        <td>
                          <input
                            type="number"
                            step="0.01"
                            min="0"
                            max={item.returnableQuantity}
                            placeholder="0.00"
                            value={item.returnQuantity}
                            onChange={(e) => handleReturnQuantityChange(index, e.target.value)}
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
            onClick={() => navigate('/inventory/purchase-returns')}
            disabled={submitting}
            style={{ height: '40px', padding: '0 20px', borderRadius: '8px', fontWeight: 600 }}
          >
            Cancel
          </button>
          <button
            type="submit"
            className="erp-button erp-button--primary"
            disabled={submitting || loadingGrnItems}
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