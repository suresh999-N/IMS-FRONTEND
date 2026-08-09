import { useCallback, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Pencil, Trash2, AlertCircle, RefreshCw } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import FormModal from '../../../layouts/FormModal'
import { getPurchaseReturnById, deletePurchaseReturn } from '../../../api/purchaseReturnApi'
import { getSuppliers } from '../../../api/suppliersApi'
import { getProductCatalog } from '../../../api/productApi'
import { apiRequest } from '../../../api/apiClient'
import { API_ENDPOINTS } from '../../../api/endpoints'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import './PurchaseReturns.css'

export default function PurchaseReturnDetails({ data = {}, actions = {} }) {
  const navigate = useNavigate()
  const { id } = useParams()

  const [returnDetails, setReturnDetails] = useState(null)
  const [supplierName, setSupplierName] = useState('')
  const [grnNumber, setGrnNumber] = useState('')
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
      const [detailsRes, suppliersRes, grnsRes, productsRes, variantsRes] = await Promise.allSettled([
        getPurchaseReturnById(id),
        getSuppliers(),
        apiRequest(API_ENDPOINTS.goodsReceipts.list),
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
      } else if (Array.isArray(data.purchaseReturns)) {
        rec = data.purchaseReturns.find((r) => String(r.id) === String(id) || String(r.returnId) === String(id))
      }

      if (rec) {
        setReturnDetails(rec)

        // Resolve supplier name
        const supList = suppliersRes.status === 'fulfilled' && suppliersRes.value?.success
          ? (suppliersRes.value.data ?? [])
          : (Array.isArray(data.suppliers) ? data.suppliers : [])
        const foundSup = supList.find((s) => String(s.id ?? s.supplierId ?? s.supplier_id) === String(rec.supplierId || rec.supplier_id))
        setSupplierName(foundSup?.name || rec.supplierName || `Supplier #${rec.supplierId || rec.supplier_id}`)

        // Resolve GRN number
        const grnList = grnsRes.status === 'fulfilled' && grnsRes.value?.success
          ? (Array.isArray(grnsRes.value.data) ? grnsRes.value.data : grnsRes.value.data?.items ?? [])
          : []
        const foundGrn = grnList.find((g) => String(g.id ?? g.grnId ?? g.grn_id) === String(rec.grnId || rec.grn_id))
        setGrnNumber(foundGrn?.grnNumber || foundGrn?.number || rec.grnNumber || (rec.grnId ? `GRN-${rec.grnId}` : '-'))
      } else {
        setError('Purchase return record not found.')
      }
    } catch (err) {
      if (Array.isArray(data.purchaseReturns)) {
        const rec = data.purchaseReturns.find((r) => String(r.id) === String(id) || String(r.returnId) === String(id))
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
  }, [id, data.purchaseReturns, data.suppliers, data.products])

  useEffect(() => {
    fetchDetails()
  }, [fetchDetails])

  const handleDelete = async () => {
    setDeleting(true)
    try {
      const res = await deletePurchaseReturn(id)
      if (res && res.success) {
        actions.deletePurchaseReturn?.(id)
        showToast('Purchase return deleted successfully.', 'success')
        navigate('/inventory/purchase-returns')
      } else if (typeof actions.deletePurchaseReturn === 'function') {
        actions.deletePurchaseReturn(id)
        showToast('Purchase return deleted successfully.', 'success')
        navigate('/inventory/purchase-returns')
      } else {
        showToast(res?.error || 'Failed to delete purchase return.', 'error')
      }
    } catch (err) {
      if (typeof actions.deletePurchaseReturn === 'function') {
        actions.deletePurchaseReturn(id)
        showToast('Purchase return deleted successfully.', 'success')
        navigate('/inventory/purchase-returns')
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
      <main className="purchase-return-details-page">
        <PageHeader
          title="Purchase Return Details"
          primaryAction={{
            icon: ArrowLeft,
            label: 'Back to Returns',
            onClick: () => navigate('/inventory/purchase-returns'),
            variant: 'secondary',
          }}
        />
        <div className="card purchase-returns-error-card">
          <AlertCircle size={24} className="error-icon" />
          <div>
            <h3>Record Not Found</h3>
            <p>{error || 'The requested purchase return could not be retrieved.'}</p>
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
    <main className="purchase-return-details-page">
      <PageHeader
        title={`Purchase Return #${retId}`}
        subtitle="Complete details of returned purchase transaction."
        primaryAction={{
          icon: ArrowLeft,
          label: 'Back to Returns',
          onClick: () => navigate('/inventory/purchase-returns'),
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
              onClick={() => navigate(`/inventory/purchase-returns/edit/${retId}`)}
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
            <span className="label">Supplier</span>
            <span className="value font-semibold">{supplierName || '-'}</span>
          </div>

          <div className="details-item">
            <span className="label">Goods Receipt / GRN</span>
            <span className="value grn-badge">{grnNumber || `-`}</span>
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
            <table className="purchase-returns-table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Variant</th>
                  <th className="text-right">Received Quantity</th>
                  <th className="text-right">Return Quantity</th>
                  <th className="text-right">Price</th>
                  <th className="text-right">Total</th>
                </tr>
              </thead>
              <tbody>
                {returnDetails.items.map((line, idx) => {
                  const prodName = productsMap[String(line.productId || line.product_id)] || line.productName || `Product #${line.productId || line.product_id}`
                  const varName = (line.variantId || line.variant_id) ? (variantsMap[String(line.variantId || line.variant_id)] || line.variantName || `Variant #${line.variantId || line.variant_id}`) : '-'
                  const receivedQty = Number(line.receivedQuantity ?? line.quantity ?? line.returnQuantity ?? 0)
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
          title="Delete Purchase Return?"
          onClose={() => setShowDeleteModal(false)}
        >
          <div className="delete-confirm-content">
            <p>This action will permanently remove this purchase return and its associated items.</p>
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
