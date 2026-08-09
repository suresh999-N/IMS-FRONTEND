import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Eye, Pencil, Plus, RefreshCw, Search, Trash2, AlertCircle, MoreVertical } from 'lucide-react'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import FormModal from '../../../layouts/FormModal'
import { getSalesReturns, deleteSalesReturn } from '../../../api/salesReturnApi'
import { getCustomers } from '../../../api/customersApi'
import { getInvoices } from '../../../api/businessApi'
import { API_ENDPOINTS } from '../../../api/endpoints'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import './SalesReturns.css'

export default function SalesReturns({ data = {}, actions = {} }) {
  const navigate = useNavigate()

  const [returns, setReturns] = useState([])
  const [customersMap, setCustomersMap] = useState({})
  const [invoicesMap, setInvoicesMap] = useState({})
  const [customersList, setCustomersList] = useState([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [searchQuery, setSearchQuery] = useState('')
  const [customerFilter, setCustomerFilter] = useState('')

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
      const [returnsRes, customersRes, invoicesRes] = await Promise.allSettled([
        getSalesReturns(),
        getCustomers(),
        getInvoices({ pageSize: 500 }),
      ])

      // Handle Customers lookup
      const cusList = customersRes.status === 'fulfilled' && customersRes.value?.success
        ? customersRes.value.data ?? []
        : (Array.isArray(data.customers) ? data.customers : [])
      setCustomersList(cusList)
      const cusMap = {}
      cusList.forEach((c) => {
        const id = String(c.id ?? c.customerId ?? c.customer_id)
        cusMap[id] = c.name || c.customerName || `Customer #${id}`
      })
      setCustomersMap(cusMap)

      // Handle Invoices lookup
      const invList = invoicesRes.status === 'fulfilled' && invoicesRes.value?.success
        ? (Array.isArray(invoicesRes.value.data) ? invoicesRes.value.data : [])
        : (Array.isArray(data.invoices) ? data.invoices : Array.isArray(data.sales) ? data.sales : Array.isArray(data.accountingInvoices) ? data.accountingInvoices : [])
      const invMap = {}
      invList.forEach((inv) => {
        const id = String(inv.id ?? inv.invoiceId ?? inv.invoice_id)
        invMap[id] = inv.invoiceNumber || inv.invoiceNo || inv.number || (id ? `SINV-${id}` : '-')
      })
      setInvoicesMap(invMap)

      // Handle Sales Returns (Merge API results and local state returns)
      const apiList = (returnsRes.status === 'fulfilled' && returnsRes.value?.success)
        ? (returnsRes.value.data ?? [])
        : []
      const localList = Array.isArray(data.salesReturns) ? data.salesReturns : []

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
      if (Array.isArray(data.salesReturns) && data.salesReturns.length > 0) {
        setReturns(data.salesReturns)
      } else {
        setError(err instanceof Error ? err.message : 'An error occurred while fetching sales returns.')
      }
    } finally {
      setLoading(false)
    }
  }, [data.customers, data.invoices, data.sales, data.accountingInvoices, data.salesReturns])

  useEffect(() => {
    fetchData()
  }, [fetchData])

  const filteredReturns = useMemo(() => {
    return returns.filter((item) => {
      const returnIdStr = String(item.returnId || item.id || '').toLowerCase()
      const customerNameStr = String(customersMap[String(item.customerId || item.customer_id)] || item.customerName || '').toLowerCase()
      const invoiceNumStr = String(invoicesMap[String(item.invoiceId || item.invoice_id)] || item.invoiceNumber || '').toLowerCase()
      const reasonStr = String(item.reason || '').toLowerCase()

      const query = searchQuery.toLowerCase().trim()

      const matchesSearch =
        !query ||
        returnIdStr.includes(query) ||
        customerNameStr.includes(query) ||
        invoiceNumStr.includes(query) ||
        reasonStr.includes(query)

      const matchesCustomer = !customerFilter || String(item.customerId || item.customer_id) === String(customerFilter)

      return matchesSearch && matchesCustomer
    })
  }, [returns, searchQuery, customerFilter, customersMap, invoicesMap])

  const handleDeleteConfirm = async () => {
    if (!deleteTargetId) return
    setDeleting(true)
    try {
      const res = await deleteSalesReturn(deleteTargetId)
      if (res && res.success) {
        actions.deleteSalesReturn?.(deleteTargetId)
        showToast('Sales return deleted successfully.', 'success')
        setReturns((prev) => prev.filter((r) => String(r.id) !== String(deleteTargetId) && String(r.returnId) !== String(deleteTargetId)))
      } else if (typeof actions.deleteSalesReturn === 'function') {
        actions.deleteSalesReturn(deleteTargetId)
        showToast('Sales return deleted successfully.', 'success')
        setReturns((prev) => prev.filter((r) => String(r.id) !== String(deleteTargetId) && String(r.returnId) !== String(deleteTargetId)))
      } else {
        showToast(res?.error || 'Failed to delete sales return.', 'error')
      }
    } catch (err) {
      if (typeof actions.deleteSalesReturn === 'function') {
        actions.deleteSalesReturn(deleteTargetId)
        showToast('Sales return deleted successfully.', 'success')
        setReturns((prev) => prev.filter((r) => String(r.id) !== String(deleteTargetId) && String(r.returnId) !== String(deleteTargetId)))
      } else {
        showToast(err instanceof Error ? err.message : 'Failed to delete sales return.', 'error')
      }
    } finally {
      setDeleting(false)
      setDeleteTargetId(null)
    }
  }

  return (
    <main className="sales-returns-page">
      <PageHeader
        title="Sales Returns"
        subtitle="Manage customer product returns and view transaction logs."
        primaryAction={{
          icon: Plus,
          label: 'Create Sales Return',
          onClick: () => navigate('/pos/returns/create'),
        }}
      />

      {/* Search & Filter Toolbar */}
      <section className="card sales-returns-toolbar">
        <div className="toolbar-search">
          <Search size={16} className="search-icon" />
          <input
            type="text"
            placeholder="Search by Return ID, Invoice #, Customer, or Reason..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>

        <div className="toolbar-filters">
          <div className="filter-group">
            <label htmlFor="customer-select">Filter Customer:</label>
            <select
              id="customer-select"
              value={customerFilter}
              onChange={(e) => setCustomerFilter(e.target.value)}
            >
              <option value="">All Customers</option>
              {customersList.map((c) => {
                const cId = String(c.id ?? c.customerId ?? c.customer_id)
                return (
                  <option key={cId} value={cId}>
                    {c.name || c.customerName || `Customer #${cId}`}
                  </option>
                )
              })}
            </select>
          </div>

          {(searchQuery || customerFilter) && (
            <button
              className="erp-button erp-button--secondary"
              type="button"
              onClick={() => {
                setSearchQuery('')
                setCustomerFilter('')
              }}
            >
              Reset Filters
            </button>
          )}
        </div>
      </section>

      {/* Content Area */}
      {loading ? (
        <StateBlock state="loading" message="Loading sales returns..." />
      ) : error ? (
        <div className="sales-returns-error-card card">
          <AlertCircle size={24} className="error-icon" />
          <div>
            <h3>Unable to load Sales Returns</h3>
            <p>{error}</p>
          </div>
          <button className="erp-button erp-button--primary" onClick={fetchData} type="button">
            <RefreshCw size={14} /> Retry
          </button>
        </div>
      ) : filteredReturns.length === 0 ? (
        <div className="sales-returns-empty card">
          <p className="empty-title">No sales returns found</p>
          <p className="empty-subtitle">
            {searchQuery || customerFilter
              ? 'No returns match your filter criteria.'
              : 'Click "Create Sales Return" to record your first customer return.'}
          </p>
          {!searchQuery && !customerFilter && (
            <button
              className="erp-button erp-button--primary"
              onClick={() => navigate('/pos/returns/create')}
              type="button"
            >
              <Plus size={14} /> Create Sales Return
            </button>
          )}
        </div>
      ) : (
        <section className="card sales-returns-table-container">
          <table className="sales-returns-table">
            <thead>
              <tr>
                <th>Return ID</th>
                <th>Invoice</th>
                <th>Customer</th>
                <th>Return Date</th>
                <th className="text-right">Total Amount</th>
                <th>Reason</th>
                <th className="text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredReturns.map((row) => {
                const retId = row.returnId || row.id
                const custId = row.customerId || row.customer_id
                const invId = row.invoiceId || row.invoice_id
                const customerName = customersMap[String(custId)] || row.customerName || (custId ? `Customer #${custId}` : '-')
                const invoiceName = invoicesMap[String(invId)] || row.invoiceNumber || (invId ? `SINV-${invId}` : '-')

                return (
                  <tr key={retId}>
                    <td className="font-semibold text-primary">#{retId}</td>
                    <td>
                      <span className="invoice-badge">{invoiceName}</span>
                    </td>
                    <td>{customerName}</td>
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
                                navigate(`/pos/returns/${retId}`)
                              }}
                            >
                              <Eye size={15} /> View Details
                            </button>
                            <button
                              type="button"
                              className="action-dropdown-item"
                              onClick={() => {
                                setActiveMenuId(null)
                                navigate(`/pos/returns/edit/${retId}`)
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
          title="Delete Sales Return?"
          onClose={() => setDeleteTargetId(null)}
        >
          <div className="delete-confirm-content">
            <p>This action will permanently delete this sales return and its associated items.</p>
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
