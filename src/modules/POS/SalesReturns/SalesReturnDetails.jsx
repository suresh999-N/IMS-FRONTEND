import { useCallback, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Pencil, Trash2, AlertCircle, RefreshCw } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import FormModal from '../../../layouts/FormModal'
import { getSalesReturnById, deleteSalesReturn } from '../../../api/salesReturnApi'
import { getCustomers } from '../../../api/customersApi'
import { getProductCatalog } from '../../../api/productApi'
import { getInvoices } from '../../../api/businessApi'
import { apiRequest } from '../../../api/apiClient'
import { API_ENDPOINTS } from '../../../api/endpoints'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import './SalesReturns.css'

export default function SalesReturnDetails({ data = {}, actions = {} }) {
  const navigate = useNavigate()
  const { id } = useParams()

  const [returnDetails, setReturnDetails] = useState(null)
  const [customerName, setCustomerName] = useState('')
  const [invoiceNumber, setInvoiceNumber] = useState('')
  const [productsMap, setProductsMap] = useState({})
  const [variantsMap, setVariantsMap] = useState({})

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [deleting, setDeleting] = useState(false)

  const fetchDetails = useCallback(async () => {
    if (!id) return
    setLoading(true)
    setError(null)
    try {
      const [detailsRes, customersRes, invoicesRes, productsRes, variantsRes] = await Promise.allSettled([
        getSalesReturnById(id),
        getCustomers(),
        getInvoices({ pageSize: 500 }),
        getProductCatalog(),
        apiRequest(API_ENDPOINTS.productVariants.list),
      ])

      // Load products map
      const prodMap = {}
      const prodList = productsRes.status === 'fulfilled' && productsRes.value?.success
        ? (productsRes.value.data ?? [])
        : (Array.isArray(data.products) ? data.products : [])
      prodList.forEach((p) => {
        const pId = String(p.id ?? p.productId ?? p.product_id)
        prodMap[pId] = p.name || p.productName || `Product #${pId}`
      })
      setProductsMap(prodMap)

      // Load variants map
      const varMap = {}
      if (variantsRes.status === 'fulfilled' && variantsRes.value?.success) {
        const rawVars = Array.isArray(variantsRes.value.data) ? variantsRes.value.data : variantsRes.value.data?.items ?? []
        rawVars.forEach((v) => {
          const vId = String(v.id ?? v.variantId ?? v.variant_id)
          varMap[vId] = v.name || v.variantName || v.sku || `Variant #${vId}`
        })
      }
      setVariantsMap(varMap)

      // Resolve Record from API or local data
      let rec = null
      if (detailsRes.status === 'fulfilled' && detailsRes.value?.success && detailsRes.value.data) {
        rec = detailsRes.value.data
      } else if (Array.isArray(data.salesReturns)) {
        rec = data.salesReturns.find((r) => String(r.id) === String(id) || String(r.returnId) === String(id))
      }

      if (rec) {
        setReturnDetails(rec)

        // Resolve Customer name
        const cusList = customersRes.status === 'fulfilled' && customersRes.value?.success
          ? (customersRes.value.data ?? [])
          : (Array.isArray(data.customers) ? data.customers : [])
        const foundCust = cusList.find((c) => String(c.id ?? c.customerId ?? c.customer_id) === String(rec.customerId || rec.customer_id))
        setCustomerName(foundCust?.name || rec.customerName || `Customer #${rec.customerId || rec.customer_id}`)

        // Resolve Invoice number
        const invList = invoicesRes.status === 'fulfilled' && invoicesRes.value?.success
          ? (Array.isArray(invoicesRes.value.data) ? invoicesRes.value.data : [])
          : (Array.isArray(data.invoices) ? data.invoices : Array.isArray(data.sales) ? data.sales : Array.isArray(data.accountingInvoices) ? data.accountingInvoices : [])
        const foundInv = invList.find((inv) => String(inv.id ?? inv.invoiceId ?? inv.invoice_id) === String(rec.invoiceId || rec.invoice_id))
        setInvoiceNumber(foundInv?.invoiceNumber || foundInv?.invoiceNo || foundInv?.number || rec.invoiceNumber || (rec.invoiceId ? `SINV-${rec.invoiceId}` : '-'))
      } else {
        setError('Sales return record not found.')
      }
    } catch (err) {
      if (Array.isArray(data.salesReturns)) {
        const rec = data.salesReturns.find((r) => String(r.id) === String(id) || String(r.returnId) === String(id))
        if (rec) {
          setReturnDetails(rec)
        } else {
          setError(err instanceof Error ? err.message : 'Error fetching details.')
        }
      } else {
        setError(err instanceof Error ? err.message : 'Error fetching details.')
      }
    } finally {
      setLoading(false)
    }
  }, [id, data.salesReturns, data.customers, data.invoices, data.sales, data.accountingInvoices, data.products])

  useEffect(() => {
    fetchDetails()
  }, [fetchDetails])

  const handleDelete = async () => {
    setDeleting(true)
    try {
      const res = await deleteSalesReturn(id)
      if (res && res.success) {
        actions.deleteSalesReturn?.(id)
        showToast('Sales return deleted successfully.', 'success')
        navigate('/pos/returns')
      } else if (typeof actions.deleteSalesReturn === 'function') {
        actions.deleteSalesReturn(id)
        showToast('Sales return deleted successfully.', 'success')
        navigate('/pos/returns')
      } else {
        showToast(res?.error || 'Failed to delete sales return.', 'error')
      }
    } catch (err) {
      if (typeof actions.deleteSalesReturn === 'function') {
        actions.deleteSalesReturn(id)
        showToast('Sales return deleted successfully.', 'success')
        navigate('/pos/returns')
      } else {
        showToast(err instanceof Error ? err.message : 'Delete error occurred.', 'error')
      }
    } finally {
      setDeleting(false)
      setShowDeleteModal(false)
    }
  }

  if (loading) {
    return <StateBlock state="loading" message="Loading return details..." />
  }

  if (error || !returnDetails) {
    return (
      <main className="sales-return-details-page">
        <PageHeader
          title="Sales Return Details"
          primaryAction={{
            icon: ArrowLeft,
            label: 'Back to Returns',
            onClick: () => navigate('/pos/returns'),
            variant: 'secondary',
          }}
        />
        <div className="card sales-returns-error-card">
          <AlertCircle size={24} className="error-icon" />
          <div>
            <h3>Record Not Found</h3>
            <p>{error || 'The requested sales return could not be retrieved.'}</p>
          </div>
          <button className="erp-button erp-button--primary" onClick={fetchDetails} type="button">
            <RefreshCw size={14} /> Retry
          </button>
        </div>
      </main>
    )
  }

  const retId = returnDetails.returnId || returnDetails.id

  return (
    <main className="sales-return-details-page">
      <PageHeader
        title={`Sales Return #${retId}`}
        subtitle="Complete details of returned customer transaction."
        primaryAction={{
          icon: ArrowLeft,
          label: 'Back to Returns',
          onClick: () => navigate('/pos/returns'),
          variant: 'secondary',
        }}
      />

      {/* Header Info Summary Card */}
      <section className="card details-header-card">
        <div className="details-header-top">
          <div>
            <span className="details-badge">Return ID</span>
            <h2 className="details-id-title">#{retId}</h2>
          </div>
          <div className="details-action-buttons">
            <button
              className="erp-button erp-button--secondary"
              type="button"
              onClick={() => navigate(`/pos/returns/edit/${retId}`)}
            >
              <Pencil size={15} /> Edit
            </button>
            <button
              className="erp-button erp-button--danger"
              type="button"
              onClick={() => setShowDeleteModal(true)}
            >
              <Trash2 size={15} /> Delete
            </button>
          </div>
        </div>

        <div className="details-grid">
          <div className="details-item">
            <span className="label">Customer</span>
            <span className="value font-semibold">{customerName || '-'}</span>
          </div>

          <div className="details-item">
            <span className="label">Invoice</span>
            <span className="value invoice-badge">{invoiceNumber || `-`}</span>
          </div>

          <div className="details-item">
            <span className="label">Return Date</span>
            <span className="value">
              {returnDetails.returnDate ? formatDate(returnDetails.returnDate) : '-'}
            </span>
          </div>

          <div className="details-item">
            <span className="label">Total Amount</span>
            <span className="value font-semibold text-primary">
              {formatCurrency(returnDetails.totalAmount || 0)}
            </span>
          </div>

          <div className="details-item full-width">
            <span className="label">Reason for Return</span>
            <span className="value reason-text">
              {returnDetails.reason || 'No reason provided.'}
            </span>
          </div>
        </div>
      </section>

      {/* Return Items Table Card */}
      <section className="card details-items-card">
        <h3 className="section-title">Return Line Items</h3>

        {(!returnDetails.items || returnDetails.items.length === 0) ? (
          <p className="no-items-text">No item lines found for this return.</p>
        ) : (
          <div className="items-table-container">
            <table className="sales-returns-table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Variant</th>
                  <th className="text-right">Invoiced Quantity</th>
                  <th className="text-right">Return Quantity</th>
                  <th className="text-right">Price</th>
                  <th className="text-right">Total</th>
                </tr>
              </thead>
              <tbody>
                {returnDetails.items.map((line, idx) => {
                  const prodName = productsMap[String(line.productId || line.product_id)] || line.productName || `Product #${line.productId || line.product_id}`
                  const varName = (line.variantId || line.variant_id) ? (variantsMap[String(line.variantId || line.variant_id)] || line.variantName || `Variant #${line.variantId || line.variant_id}`) : '-'
                  const receivedQty = Number(line.receivedQuantity ?? line.invoicedQuantity ?? line.quantity ?? line.returnQuantity ?? 0)
                  const returnQty = Number(line.returnQuantity ?? line.quantity ?? 0)
                  const price = Number(line.price || 0)
                  const lineTotal = returnQty * price

                  return (
                    <tr key={line.id || idx}>
                      <td className="font-semibold">{prodName}</td>
                      <td>{varName}</td>
                      <td className="text-right">{receivedQty.toFixed(2)}</td>
                      <td className="text-right font-semibold text-primary">{returnQty.toFixed(2)}</td>
                      <td className="text-right">{formatCurrency(price)}</td>
                      <td className="text-right font-semibold">{formatCurrency(lineTotal)}</td>
                    </tr>
                  )
                })}
              </tbody>
              <tfoot>
                <tr>
                  <td colSpan={5} className="text-right font-semibold">Total Return Amount:</td>
                  <td className="text-right font-semibold text-primary" style={{ fontSize: '1.05rem' }}>
                    {formatCurrency(returnDetails.totalAmount || 0)}
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>
        )}
      </section>

      {/* Delete Confirmation Modal */}
      {showDeleteModal && (
        <FormModal
          isOpen={showDeleteModal}
          title="Delete Sales Return?"
          onClose={() => setShowDeleteModal(false)}
        >
          <div className="delete-confirm-content">
            <p>This action will permanently delete this sales return and its associated items.</p>
            <p className="delete-warning">Return ID: #{retId}</p>

            <div className="form-modal-actions">
              <button
                className="erp-button erp-button--secondary"
                type="button"
                disabled={deleting}
                onClick={() => setShowDeleteModal(false)}
              >
                Cancel
              </button>
              <button
                className="erp-button erp-button--danger"
                type="button"
                disabled={deleting}
                onClick={handleDelete}
              >
                {deleting ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </FormModal>
      )}
    </main>
  )
}
