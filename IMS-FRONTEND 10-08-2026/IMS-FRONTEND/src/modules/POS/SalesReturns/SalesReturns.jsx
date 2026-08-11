import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Eye, Pencil, Plus, RefreshCw, Search, Trash2, AlertCircle, MoreVertical, ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight, IndianRupee, Users } from 'lucide-react'
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

function getVisiblePages(currentPage, totalPages) {
  const pages = []
  const maxVisiblePages = 5
  let startPage = Math.max(1, currentPage - 2)
  const endPage = Math.min(totalPages, startPage + maxVisiblePages - 1)

  if (endPage - startPage + 1 < maxVisiblePages) {
    startPage = Math.max(1, endPage - maxVisiblePages + 1)
  }

  for (let pageNumber = startPage; pageNumber <= endPage; pageNumber += 1) {
    pages.push(pageNumber)
  }

  return pages
}

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

  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

  // Close 3-dots menu on outside click
  useEffect(() => {
    const handleOutsideClick = () => setActiveMenuId(null)
    document.addEventListener('click', handleOutsideClick)
    return () => document.removeEventListener('click', handleOutsideClick)
  }, [])

  useEffect(() => {
    setCurrentPage(1)
  }, [searchQuery, customerFilter])

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

  const totalRows = filteredReturns.length
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize))
  const safePage = Math.min(currentPage, totalPages)
  const startIndex = (safePage - 1) * pageSize
  const endIndex = Math.min(startIndex + pageSize, totalRows)

  const paginatedReturns = useMemo(() => {
    return filteredReturns.slice(startIndex, endIndex)
  }, [filteredReturns, startIndex, endIndex])

  const visiblePages = useMemo(() => {
    return getVisiblePages(safePage, totalPages)
  }, [safePage, totalPages])

  useEffect(() => {
    if (currentPage > totalPages && totalPages > 0) {
      setCurrentPage(totalPages)
    }
  }, [totalPages, currentPage])

  const stats = useMemo(() => {
    const total = returns.length
    const uniqueCustomers = new Set(
      returns.map((item) => item.customerId || item.customer_id).filter(Boolean)
    ).size
    const totalAmount = returns.reduce(
      (sum, item) => sum + (item.totalAmount || 0),
      0
    )

    return {
      total,
      customers: uniqueCustomers,
      totalAmount,
    }
  }, [returns])

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
      <header className="purchases-page__compact-header">
        <div className="purchases-page__compact-main">
          <h1>Sales Returns</h1>
          <div className="purchases-page__metrics">
            <span className="purchases-page__metric purchases-page__metric--info">
              {stats.total} Returns
            </span>
            <span className="purchases-page__metric purchases-page__metric--warning">
              {stats.customers} Customers
            </span>
            <span className="purchases-page__metric purchases-page__metric--success">
              {formatCurrency(stats.totalAmount)} Total Returned
            </span>
          </div>
        </div>
        <button
          type="button"
          className="button button-primary"
          onClick={() => navigate('/pos/returns/create')}
          style={{ display: 'inline-flex', alignItems: 'center', gap: '6px' }}
        >
          <Plus size={16} />
          Create Sales Return
        </button>
      </header>

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
              {paginatedReturns.map((row) => {
                const retId = row.returnId || row.id
                const custId = row.customerId || row.customer_id
                const invId = row.invoiceId || row.invoice_id
                const customerName = customersMap[String(custId)] || row.customerName || (custId ? `Customer #${custId}` : '-')
                const invoiceName = invoicesMap[String(invId)] || row.invoiceNumber || (invId ? `SINV-${invId}` : '-')

                const returnCode = (val) => {
                  if (!val) return '-'
                  const str = String(val).trim().replace(/^#/, '')
                  if (/^SRR-\d{6}$/i.test(str)) return str.toUpperCase()
                  const digits = str.replace(/\D/g, '')
                  if (digits) return `SRR-${digits.padStart(6, '0')}`
                  return str.startsWith('SRR-') ? str : `SRR-${str}`
                }

                return (
                  <tr key={retId}>
                    <td className="font-semibold text-primary">{returnCode(row.returnNumber || row.return_number || retId)}</td>
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

          {totalRows > 0 && (
            <div className="table-component__pagination">
              <div className="table-component__pagination-metrics">
                <label className="table-component__rows-control">
                  <span>Rows</span>
                  <select
                    value={pageSize}
                    onChange={(event) => {
                      setPageSize(Number(event.target.value))
                      setCurrentPage(1)
                    }}
                  >
                    {[5, 10, 15, 20, 25].map((size) => (
                      <option key={size} value={size}>
                        {size}
                      </option>
                    ))}
                  </select>
                </label>
                <span className="table-component__status">
                  Showing {startIndex + 1}-{endIndex} of {totalRows}
                </span>
              </div>

              <div className="table-component__page-controls">
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() => setCurrentPage(1)}
                  disabled={safePage === 1}
                  aria-label="Go to first page"
                >
                  <ChevronsLeft size={16} />
                  First
                </button>
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() => setCurrentPage((val) => Math.max(val - 1, 1))}
                  disabled={safePage === 1}
                  aria-label="Go to previous page"
                >
                  <ChevronLeft size={16} />
                  Previous
                </button>
                {visiblePages.map((pageNumber) => (
                  <button
                    key={pageNumber}
                    type="button"
                    className={`table-component__page-number ${pageNumber === safePage ? 'is-active' : ''}`.trim()}
                    onClick={() => setCurrentPage(pageNumber)}
                    aria-current={pageNumber === safePage ? 'page' : undefined}
                    aria-label={`Go to page ${pageNumber}`}
                  >
                    {pageNumber}
                  </button>
                ))}
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() => setCurrentPage((val) => Math.min(val + 1, totalPages))}
                  disabled={safePage === totalPages}
                  aria-label="Go to next page"
                >
                  <ChevronRight size={16} />
                  Next
                </button>
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() => setCurrentPage(totalPages)}
                  disabled={safePage === totalPages}
                  aria-label="Go to last page"
                >
                  <ChevronsRight size={16} />
                  Last
                </button>
              </div>
            </div>
          )}
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
