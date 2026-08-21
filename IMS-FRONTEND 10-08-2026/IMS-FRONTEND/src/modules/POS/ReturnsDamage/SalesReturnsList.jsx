import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  ChevronRight,
  Plus,
  RefreshCw,
  SlidersHorizontal,
  RotateCcw,
  ReceiptText,
  UserRound,
  Eye,
  Edit,
  CheckCircle,
  Trash2,
} from 'lucide-react'
import {
  legacyGetSalesReturns,
  legacyDeleteSalesReturn,
  legacySubmitSalesReturn,
  legacyApproveSalesReturn,
} from '../../../api/returnsExchangeApi'
import { ActionMenu, DataTable, FilterBar, StatusBadge } from '../../../components/erp'
import { showToast } from '../../../components/common/toast'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import '../Sales/Sales.css'
import './SalesReturns.css'

function getReturnRowKey(r) {
  return String(r?.salesReturnId ?? r?.id ?? r?.returnNumber ?? '')
}

export default function SalesReturnsList() {
  const navigate = useNavigate()
  const [returns, setReturns] = useState([])
  const [selectedReturnIds, setSelectedReturnIds] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')
  const [statusFilter, setStatusFilter] = useState('all')

  const fetchReturns = useCallback(async () => {
    setIsLoading(true)
    setError('')
    try {
      const res = await legacyGetSalesReturns({
        status: statusFilter,
        page: 1,
        pageSize: 100,
      })

      const payload = res?.data || res
      const listData = Array.isArray(payload?.data)
        ? payload.data
        : Array.isArray(payload)
          ? payload
          : []

      setReturns(listData)
    } catch (err) {
      console.error('Failed to load sales returns', err)
      setError(err.response?.data?.message || err.message || 'Unable to load sales returns workspace.')
    } finally {
      setIsLoading(false)
    }
  }, [statusFilter])

  useEffect(() => {
    fetchReturns()
  }, [fetchReturns])

  const summary = useMemo(() => {
    const totalCount = returns.length
    const totalValue = returns.reduce((sum, r) => sum + Number(r.grandTotal || 0), 0)
    const draftCount = returns.filter((r) => r.status === 'Draft').length
    const pendingCount = returns.filter((r) => r.status === 'Pending Approval').length
    const approvedCount = returns.filter((r) => r.status === 'Approved').length
    const refundedCount = returns.filter((r) => r.status === 'Refund Processed' || r.status === 'Completed').length

    return {
      totalCount,
      totalValue,
      draftCount,
      pendingCount,
      approvedCount,
      refundedCount,
    }
  }, [returns])

  const selectedReturns = useMemo(() => {
    const selectedSet = new Set(selectedReturnIds.map(String))
    return returns.filter((r) => selectedSet.has(getReturnRowKey(r)))
  }, [returns, selectedReturnIds])

  const tableReturns = useMemo(
    () => returns.map((r, index) => ({
      ...r,
      __rowKey: getReturnRowKey(r) || `return-${index}`,
    })),
    [returns],
  )

  const filteredReturns = useMemo(() => {
    if (statusFilter === 'all') {
      return tableReturns
    }
    return tableReturns.filter((r) => {
      const st = String(r.status || '').trim().toLowerCase()
      const filterKey = statusFilter.toLowerCase()
      return st === filterKey
    })
  }, [statusFilter, tableReturns])

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this draft return?')) return
    try {
      await legacyDeleteSalesReturn(id)
      showToast({ type: 'success', title: 'Sales Returns', message: 'Draft return deleted successfully.' })
      fetchReturns()
    } catch (err) {
      showToast({ type: 'error', title: 'Sales Returns', message: err.response?.data?.message || 'Failed to delete return.' })
    }
  }

  const handleSubmit = async (id) => {
    try {
      await legacySubmitSalesReturn(id)
      showToast({ type: 'success', title: 'Sales Returns', message: 'Sales return submitted for approval.' })
      fetchReturns()
    } catch (err) {
      showToast({ type: 'error', title: 'Sales Returns', message: err.response?.data?.message || 'Failed to submit return.' })
    }
  }

  const handleApprove = async (id) => {
    if (!window.confirm('Approve this return? Stock will be restored and return_in movements logged.')) return
    try {
      await legacyApproveSalesReturn(id)
      showToast({ type: 'success', title: 'Sales Returns', message: 'Sales return approved successfully.' })
      fetchReturns()
    } catch (err) {
      showToast({ type: 'error', title: 'Sales Returns', message: err.response?.data?.message || 'Failed to approve return.' })
    }
  }

  const returnColumns = [
    {
      key: 'returnNumber',
      label: 'Return No',
      sortable: true,
      mobilePrimary: true,
      tableWidth: 240,
      style: { width: 240, minWidth: 220 },
      headerStyle: { width: 240, minWidth: 220 },
      searchValue: (r) =>
        `${r.returnNumber || ''} ${r.invoiceNumber || ''} ${r.customerName || ''} ${r.status || ''} ${r.reason || ''}`,
      render: (r) => {
        const id = r.salesReturnId || r.id
        return (
          <div className="catalog-page__tree-cell">
            <button
              type="button"
              className="catalog-page__tree-toggle"
              onClick={(e) => {
                e.stopPropagation()
                navigate(`/pos/returns/${id}`)
              }}
              title="View Return Details"
            >
              <ChevronRight size={16} />
            </button>
            <RotateCcw size={16} className="catalog-page__tree-icon" style={{ color: '#059669' }} />
            <div className="catalog-page__entity">
              <strong style={{ color: '#059669' }}>{r.returnNumber || `RET-${id}`}</strong>
              {r.returnDate ? <span>{formatDate(r.returnDate)}</span> : null}
            </div>
          </div>
        )
      },
    },
    {
      key: 'invoiceNumber',
      label: 'Invoice',
      sortable: true,
      tableWidth: 180,
      style: { width: 180, minWidth: 160 },
      headerStyle: { width: 180, minWidth: 160 },
      render: (r) => (
        <div className="sales-page__invoice-cell">
          <ReceiptText size={15} style={{ color: '#475569' }} />
          <strong>{r.invoiceNumber || `INV-${r.invoiceId}`}</strong>
        </div>
      ),
    },
    {
      key: 'customerName',
      label: 'Customer',
      sortable: true,
      tableWidth: 170,
      style: { width: 170, minWidth: 150 },
      headerStyle: { width: 170, minWidth: 150 },
      searchValue: (r) => r.customerName || '',
      render: (r) => (
        <div className="sales-page__customer-cell">
          <UserRound size={15} />
          <span style={{ whiteSpace: 'nowrap' }}>{r.customerName || 'Walk-in Customer'}</span>
        </div>
      ),
    },
    {
      key: 'returnDate',
      label: 'Date',
      sortable: true,
      tableWidth: 140,
      style: { width: 140, minWidth: 120 },
      headerStyle: { width: 140, minWidth: 120 },
      sortValue: (r) => new Date(r.returnDate || 0).getTime() || 0,
      render: (r) => (
        <span style={{ whiteSpace: 'nowrap', fontWeight: 600 }}>
          {r.returnDate ? formatDate(r.returnDate) : 'Not set'}
        </span>
      ),
    },
    {
      key: 'itemCount',
      label: 'Items',
      sortable: true,
      tableWidth: 90,
      style: { width: 90, minWidth: 80, textAlign: 'center' },
      headerStyle: { width: 90, minWidth: 80, textAlign: 'center' },
      render: (r) => (
        <span style={{ display: 'inline-block', width: '100%', textAlign: 'center', fontWeight: 600 }}>
          {r.items?.length ? r.items.length : (r.itemCount || 1)}
        </span>
      ),
    },
    {
      key: 'grandTotal',
      label: 'Return Amount',
      sortable: true,
      tableWidth: 140,
      style: { width: 140, minWidth: 130 },
      headerStyle: { width: 140, minWidth: 130 },
      sortValue: (r) => Number(r.grandTotal || 0),
      render: (r) => (
        <strong style={{ whiteSpace: 'nowrap', fontWeight: 600, color: '#0f172a' }}>
          {formatCurrency(r.grandTotal)}
        </strong>
      ),
    },
    {
      key: 'refundAmount',
      label: 'Refund Amount',
      sortable: true,
      tableWidth: 160,
      style: { width: 160, minWidth: 140 },
      headerStyle: { width: 160, minWidth: 140 },
      sortValue: (r) => Number(r.refundAmount || (r.status === 'Completed' || r.status === 'Refund Processed' ? r.grandTotal : 0)),
      render: (r) => {
        const refAmt = r.refundAmount || (r.status === 'Completed' || r.status === 'Refund Processed' ? r.grandTotal : 0)
        const pendingRef = (r.grandTotal || 0) - refAmt
        return (
          <div className="sales-page__money-stack">
            <strong style={{ whiteSpace: 'nowrap', color: '#059669' }}>{formatCurrency(refAmt)}</strong>
            {pendingRef > 0 ? (
              <span style={{ whiteSpace: 'nowrap', color: '#c2410c' }}>Pending {formatCurrency(pendingRef)}</span>
            ) : (
              <span style={{ whiteSpace: 'nowrap', color: '#64748b' }}>Fully Settled</span>
            )}
          </div>
        )
      },
    },
    {
      key: 'status',
      label: 'Status',
      sortable: true,
      mobileStatus: true,
      tableWidth: 130,
      style: { width: 130, minWidth: 110 },
      headerStyle: { width: 130, minWidth: 110 },
      render: (r) => (
        <StatusBadge status={r.status || 'Draft'} />
      ),
    },
    {
      key: 'actions',
      label: 'Actions',
      searchable: false,
      hideable: false,
      className: 'sales-page__col-actions',
      tableWidth: 80,
      style: { width: 80, minWidth: 70, maxWidth: 80 },
      headerStyle: { width: 80, minWidth: 70, maxWidth: 80 },
      render: (r) => {
        const id = r.salesReturnId || r.id
        return (
          <ActionMenu
            iconOnly
            label={`Actions for ${r.returnNumber || id}`}
            actions={[
              {
                key: 'details',
                label: 'View Details',
                icon: Eye,
                onClick: () => navigate(`/pos/returns/${id}`),
              },
              r.status === 'Draft' ? {
                key: 'edit',
                label: 'Edit Draft',
                icon: Edit,
                onClick: () => navigate(`/pos/returns/edit/${id}`),
              } : null,
              r.status === 'Draft' ? {
                key: 'submit',
                label: 'Submit for Approval',
                icon: CheckCircle,
                onClick: () => handleSubmit(id),
              } : null,
              r.status === 'Pending Approval' ? {
                key: 'approve',
                label: 'Approve Return',
                icon: CheckCircle,
                onClick: () => handleApprove(id),
              } : null,
              r.status === 'Draft' ? {
                key: 'delete',
                label: 'Delete Draft',
                icon: Trash2,
                variant: 'danger',
                onClick: () => handleDelete(id),
              } : null,
            ].filter(Boolean)}
          />
        )
      },
    },
  ]

  return (
    <div className="page sales-page">
      {/* Compact Header matching Sales.jsx exactly */}
      <header className="sales-page__compact-header" aria-label="Sales returns summary">
        <div className="sales-page__compact-main">
          <h1>Sales Returns</h1>
          <div className="sales-page__metrics" aria-label="Sales return metrics">
            <span className="sales-page__metric sales-page__metric--success">
              {summary.totalCount} Returns
            </span>
            <span className="sales-page__metric sales-page__metric--info">
              {formatCurrency(summary.totalValue)} Total
            </span>
            <span className="sales-page__metric sales-page__metric--warning">
              {summary.pendingCount} Pending
            </span>
            <span className="sales-page__metric sales-page__metric--value">
              {summary.approvedCount} Approved
            </span>
            <span className="sales-page__metric sales-page__metric--success">
              {summary.refundedCount} Refunded
            </span>
          </div>
        </div>

        <button
          type="button"
          className="button button-primary sales-page__add-button"
          onClick={() => navigate('/pos/returns/create')}
        >
          <Plus size={16} />
          Create Sales Return
        </button>
      </header>

      {error ? (
        <div className="message-box message-box--error page-error-banner" role="alert">
          <span>{error}</span>
          <button
            type="button"
            className="button button-secondary"
            onClick={() => fetchReturns()}
            disabled={isLoading}
          >
            <RefreshCw size={16} className={isLoading ? 'animate-spin' : ''} />
            Retry
          </button>
        </div>
      ) : null}

      {/* Main Table Card reusing Sales.jsx DataTable & Filter Toolbar */}
      <div className="card sales-page__table-card">
        <DataTable
          className="sales-page__table"
          rows={filteredReturns}
          columns={returnColumns}
          loading={isLoading}
          defaultPageSize={20}
          defaultSortKey="returnDate"
          defaultSortDirection="desc"
          splitToolbar
          showSearch={selectedReturns.length === 0}
          searchPlaceholder="Search sales returns by return #, invoice, customer, or status..."
          toolbarContent={selectedReturns.length === 0 ? (
            <FilterBar className="sales-page__toolbar-actions">
              <button
                type="button"
                className="button button-secondary"
                onClick={() => fetchReturns()}
                disabled={isLoading}
              >
                <RefreshCw size={15} className={isLoading ? 'animate-spin' : ''} />
                Refresh
              </button>
            </FilterBar>
          ) : null}
          filterContent={selectedReturns.length === 0 ? (
            <div className="sales-page__filters">
              <div className="sales-page__status-filter">
                <SlidersHorizontal size={15} />
                <select
                  value={statusFilter}
                  onChange={(e) => setStatusFilter(e.target.value)}
                  aria-label="Filter by status"
                >
                  <option value="all">All statuses</option>
                  <option value="Draft">Draft</option>
                  <option value="Pending Approval">Pending Approval</option>
                  <option value="Approved">Approved</option>
                  <option value="Refund Processed">Refund Processed</option>
                  <option value="Completed">Completed</option>
                  <option value="Rejected">Rejected</option>
                </select>
              </div>
            </div>
          ) : (
            <FilterBar className="sales-page__selection-actions" ariaLabel="Selected return actions">
              <div className="sales-selection-summary" aria-live="polite">
                <strong>{selectedReturns.length} selected</strong>
              </div>
              <button
                type="button"
                className="button button-secondary sales-page__selection-button"
                onClick={() => setSelectedReturnIds([])}
              >
                Clear
              </button>
            </FilterBar>
          )}
          columnStorageKey="ims.salesReturns.visibleColumns.v2"
          defaultVisibleColumnKeys={['returnNumber', 'invoiceNumber', 'customerName', 'returnDate', 'itemCount', 'grandTotal', 'refundAmount', 'status', 'actions']}
          fitExplicitColumnsToContainer
          enableRowSelection
          selectedRowKeys={selectedReturnIds}
          onSelectionChange={setSelectedReturnIds}
          keyField="__rowKey"
          emptyMessage="No sales returns available."
        />
      </div>
    </div>
  )
}
