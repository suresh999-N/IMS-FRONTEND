import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Eye, Pencil, Plus, RefreshCw, Search, Trash2, AlertCircle, MoreVertical } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import FormModal from '../../../layouts/FormModal'
import { getPurchaseReturns, deletePurchaseReturn } from '../../../api/purchaseReturnApi'
import { getSuppliers } from '../../../api/suppliersApi'
import { apiRequest } from '../../../api/apiClient'
import { API_ENDPOINTS } from '../../../api/endpoints'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import './PurchaseReturns.css'

export default function PurchaseReturns({ data = {}, actions = {} }) {
  const navigate = useNavigate()

  const [returns, setReturns] = useState([])
  const [suppliersMap, setSuppliersMap] = useState({})
  const [grnsMap, setGrnsMap] = useState({})
  const [suppliersList, setSuppliersList] = useState([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [searchQuery, setSearchQuery] = useState('')
  const [supplierFilter, setSupplierFilter] = useState('')

  // State for delete modal & 3-dots action menu
  const [deleteTargetId, setDeleteTargetId] = useState(null)
  const [deleting, setDeleting] = useState(false)
  const [activeMenuId, setActiveMenuId] = useState(null)

  // Close 3-dots menu on outside click
  useEffect(() => {
    const handleOutsideClick = () => setActiveMenuId(null)
    document.addEventListener('click', handleOutsideClick)
    return () => document.removeEventListener('click', handleOutsideClick)
  }, [])

  const fetchData = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const [returnsRes, suppliersRes, grnsRes] = await Promise.allSettled([
        getPurchaseReturns(),
        getSuppliers(),
        apiRequest(API_ENDPOINTS.goodsReceipts.list),
      ])

      // Handle Suppliers lookup
      const supList = suppliersRes.status === 'fulfilled' && suppliersRes.value?.success
        ? suppliersRes.value.data ?? []
        : (Array.isArray(data.suppliers) ? data.suppliers : [])
      setSuppliersList(supList)
      const supMap = {}
      supList.forEach((s) => {
        const id = String(s.id ?? s.supplierId ?? s.supplier_id)
        supMap[id] = s.name || s.supplierName || `Supplier #${id}`
      })
      setSuppliersMap(supMap)

      // Handle GRNs lookup
      const grnList = grnsRes.status === 'fulfilled' && grnsRes.value?.success
        ? (Array.isArray(grnsRes.value.data) ? grnsRes.value.data : grnsRes.value.data?.items ?? [])
        : []
      const grnMap = {}
      grnList.forEach((g) => {
        const id = String(g.id ?? g.grnId ?? g.grn_id)
        grnMap[id] = g.grnNumber || g.number || `GRN-${id}`
      })
      setGrnsMap(grnMap)

      // Handle Purchase Returns (Combine API results and local returns)
      const apiList = (returnsRes.status === 'fulfilled' && returnsRes.value?.success)
        ? (returnsRes.value.data ?? [])
        : []
      const localList = Array.isArray(data.purchaseReturns) ? data.purchaseReturns : []

      const mergedMap = new Map()
      localList.forEach((item) => {
        const key = String(item.returnId || item.id || '')
        if (key) mergedMap.set(key, item)
      })
      apiList.forEach((item) => {
        const key = String(item.returnId || item.id || '')
        if (key) mergedMap.set(key, item)
      })

      const combinedReturns = Array.from(mergedMap.values())
      setReturns(combinedReturns)
    } catch (err) {
      if (Array.isArray(data.purchaseReturns) && data.purchaseReturns.length > 0) {
        setReturns(data.purchaseReturns)
      } else {
        setError(err instanceof Error ? err.message : 'An error occurred while fetching purchase returns.')
      }
    } finally {
      setLoading(false)
    }
  }, [data.suppliers, data.purchaseReturns])

  useEffect(() => {
    fetchData()
  }, [fetchData])

  const filteredReturns = useMemo(() => {
    return returns.filter((item) => {
      const returnIdStr = String(item.returnId || item.id || '').toLowerCase()
      const supplierName = (suppliersMap[String(item.supplierId)] || item.supplierName || '').toLowerCase()
      const grnStr = (grnsMap[String(item.grnId)] || item.grnNumber || item.grnId || '').toLowerCase()
      const reasonStr = String(item.reason || '').toLowerCase()

      const matchesSearch = !searchQuery ||
        returnIdStr.includes(searchQuery.toLowerCase()) ||
        supplierName.includes(searchQuery.toLowerCase()) ||
        grnStr.includes(searchQuery.toLowerCase()) ||
        reasonStr.includes(searchQuery.toLowerCase())

      const matchesSupplier = !supplierFilter || String(item.supplierId) === String(supplierFilter)

      return matchesSearch && matchesSupplier
    })
  }, [returns, searchQuery, supplierFilter, suppliersMap, grnsMap])

  const handleDeleteConfirm = async () => {
    if (!deleteTargetId) return
    setDeleting(true)
    try {
      const res = await deletePurchaseReturn(deleteTargetId)
      if (res && res.success) {
        actions.deletePurchaseReturn?.(deleteTargetId)
        showToast('Purchase return deleted successfully.', 'success')
        setReturns((prev) => prev.filter((r) => String(r.id) !== String(deleteTargetId) && String(r.returnId) !== String(deleteTargetId)))
      } else if (typeof actions.deletePurchaseReturn === 'function') {
        actions.deletePurchaseReturn(deleteTargetId)
        showToast('Purchase return deleted successfully.', 'success')
        setReturns((prev) => prev.filter((r) => String(r.id) !== String(deleteTargetId) && String(r.returnId) !== String(deleteTargetId)))
      } else {
        showToast(res?.error || 'Failed to delete purchase return.', 'error')
      }
    } catch (err) {
      if (typeof actions.deletePurchaseReturn === 'function') {
        actions.deletePurchaseReturn(deleteTargetId)
        showToast('Purchase return deleted successfully.', 'success')
        setReturns((prev) => prev.filter((r) => String(r.id) !== String(deleteTargetId) && String(r.returnId) !== String(deleteTargetId)))
      } else {
        showToast(err instanceof Error ? err.message : 'Failed to delete purchase return.', 'error')
      }
    } finally {
      setDeleting(false)
      setDeleteTargetId(null)
    }
  }

  return (
    <main className="purchase-returns-page">
      <PageHeader
        title="Purchase Returns"
        subtitle="Manage and track goods returned to suppliers."
        primaryAction={{
          icon: Plus,
          label: 'Create Purchase Return',
          onClick: () => navigate('/inventory/purchase-returns/create'),
        }}
      />

      {/* Toolbar / Filters */}
      <section className="purchase-returns-toolbar card">
        <div className="toolbar-search">
          <Search size={16} className="search-icon" />
          <input
            type="text"
            placeholder="Search by Return ID, Supplier, GRN or Reason..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          {searchQuery && (
            <button className="clear-search-btn" onClick={() => setSearchQuery('')} type="button">
              Clear
            </button>
          )}
        </div>

        <div className="toolbar-filters">
          <div className="filter-group">
            <label htmlFor="supplier-select">Supplier:</label>
            <select
              id="supplier-select"
              value={supplierFilter}
              onChange={(e) => setSupplierFilter(e.target.value)}
            >
              <option value="">All Suppliers</option>
              {suppliersList.map((s) => {
                const sId = String(s.id ?? s.supplierId ?? s.supplier_id)
                return (
                  <option key={sId} value={sId}>
                    {s.name || s.supplierName || `Supplier #${sId}`}
                  </option>
                )
              })}
            </select>
          </div>

          {(searchQuery || supplierFilter) && (
            <button
              className="erp-button erp-button--secondary"
              type="button"
              onClick={() => {
                setSearchQuery('')
                setSupplierFilter('')
              }}
            >
              Reset Filters
            </button>
          )}
        </div>
      </section>

      {/* Content Area */}
      {loading ? (
        <StateBlock state="loading" message="Loading purchase returns..." />
      ) : error ? (
        <div className="purchase-returns-error-card card">
          <AlertCircle size={24} className="error-icon" />
          <div>
            <h3>Unable to load Purchase Returns</h3>
            <p>{error}</p>
          </div>
          <button className="erp-button erp-button--primary" onClick={fetchData} type="button">
            <RefreshCw size={14} /> Retry
          </button>
        </div>
      ) : filteredReturns.length === 0 ? (
        <div className="purchase-returns-empty card">
          <p className="empty-title">No purchase returns found</p>
          <p className="empty-subtitle">
            {searchQuery || supplierFilter
              ? 'No returns match your filter criteria.'
              : 'Click "Create Purchase Return" to record your first return.'}
          </p>
          {!searchQuery && !supplierFilter && (
            <button
              className="erp-button erp-button--primary"
              onClick={() => navigate('/inventory/purchase-returns/create')}
              type="button"
            >
              <Plus size={14} /> Create Purchase Return
            </button>
          )}
        </div>
      ) : (
        <section className="card purchase-returns-table-container">
          <table className="purchase-returns-table">
            <thead>
              <tr>
                <th>Return ID</th>
                <th>Supplier</th>
                <th>GRN</th>
                <th>Return Date</th>
                <th className="text-right">Total Amount</th>
                <th>Reason</th>
                <th className="text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredReturns.map((row) => {
                const retId = row.returnId || row.id
                const supplierName = suppliersMap[String(row.supplierId)] || row.supplierName || `Supplier #${row.supplierId}`
                const grnName = grnsMap[String(row.grnId)] || row.grnNumber || (row.grnId ? `GRN-${row.grnId}` : '-')

                return (
                  <tr key={retId}>
                    <td className="font-semibold text-primary">#{retId}</td>
                    <td>{supplierName}</td>
                    <td>
                      <span className="grn-badge">{grnName}</span>
                    </td>
                    <td>{row.returnDate ? formatDate(row.returnDate) : '-'}</td>
                    <td className="text-right font-semibold">{formatCurrency(row.totalAmount || 0)}</td>
                    <td className="reason-cell" title={row.reason}>
                      {row.reason ? (row.reason.length > 50 ? `${row.reason.slice(0, 50)}...` : row.reason) : '-'}
                    </td>
                    <td className="text-right actions-cell">
                      <div className="actions-dropdown-container">
                        <button
                          className={`action-menu-btn ${activeMenuId === retId ? 'active' : ''}`}
                          type="button"
                          title="Actions"
                          onClick={(e) => {
                            e.stopPropagation()
                            setActiveMenuId((prev) => (prev === retId ? null : retId))
                          }}
                        >
                          <MoreVertical size={18} />
                        </button>
                        {activeMenuId === retId && (
                          <div className="action-dropdown-menu" onClick={(e) => e.stopPropagation()}>
                            <button
                              type="button"
                              className="action-dropdown-item"
                              onClick={() => {
                                setActiveMenuId(null)
                                navigate(`/inventory/purchase-returns/${retId}`)
                              }}
                            >
                              <Eye size={15} /> View Details
                            </button>
                            <button
                              type="button"
                              className="action-dropdown-item"
                              onClick={() => {
                                setActiveMenuId(null)
                                navigate(`/inventory/purchase-returns/edit/${retId}`)
                              }}
                            >
                              <Pencil size={15} /> Edit
                            </button>
                            <button
                              type="button"
                              className="action-dropdown-item danger-item"
                              onClick={() => {
                                setActiveMenuId(null)
                                setDeleteTargetId(retId)
                              }}
                            >
                              <Trash2 size={15} /> Delete
                            </button>
                          </div>
                        )}
                      </div>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </section>
      )}

      {/* Confirmation Modal for Delete */}
      {deleteTargetId && (
        <FormModal
          isOpen={Boolean(deleteTargetId)}
          title="Delete Purchase Return?"
          onClose={() => setDeleteTargetId(null)}
        >
          <div className="delete-confirm-content">
            <p>This action will permanently remove this purchase return and its associated items.</p>
            <p className="delete-warning">Return ID: #{deleteTargetId}</p>

            <div className="form-modal-actions">
              <button
                className="erp-button erp-button--secondary"
                type="button"
                disabled={deleting}
                onClick={() => setDeleteTargetId(null)}
              >
                Cancel
              </button>
              <button
                className="erp-button erp-button--danger"
                type="button"
                disabled={deleting}
                onClick={handleDeleteConfirm}
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
