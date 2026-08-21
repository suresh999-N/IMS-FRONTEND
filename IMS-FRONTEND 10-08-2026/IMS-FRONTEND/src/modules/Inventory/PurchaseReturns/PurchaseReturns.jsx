import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  ChevronRight,
  Plus,
  RefreshCw,
  SlidersHorizontal,
  RotateCcw,
  ReceiptText,
  Building2,
  Eye,
  Edit,
  Trash2,
  Download,
  CheckCircle,
  Send,
} from 'lucide-react'
import {
  getPurchaseReturns,
  deletePurchaseReturn,
  submitPurchaseReturn,
  approvePurchaseReturn,
  getPurchaseReturnErrorMessage,
} from '../../../api/purchaseReturnsApi'
import { ActionMenu, DataTable, FilterBar, StatusBadge } from '../../../components/erp'
import { showToast } from '../../../components/common/toast'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import '../../POS/Sales/Sales.css'
import '../../POS/SalesReturns/SalesReturns.css'
import './PurchaseReturns.css'

function getReturnRowKey(r) {
  return String(r?.returnId ?? r?.id ?? r?.returnNumber ?? '')
}

export default function PurchaseReturns() {
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
      const res = await getPurchaseReturns({
        status: statusFilter !== 'all' ? statusFilter : undefined,
      })

      const payload = res?.data || res
      const listData = Array.isArray(payload?.data)
        ? payload.data
        : Array.isArray(payload)
          ? payload
          : []

      setReturns(listData)
    } catch (err) {
      console.error('Failed to load purchase returns', err)
      setError(getPurchaseReturnErrorMessage(err, 'Unable to load purchase returns workspace.'))
    } finally {
      setIsLoading(false)
    }
  }, [statusFilter])

  useEffect(() => {
    fetchReturns()
  }, [fetchReturns])

  const summary = useMemo(() => {
    const totalCount = returns.length
    const totalValue = returns.reduce(
      (sum, r) => sum + Number(r.totalAmount ?? r.totalReturnAmount ?? r.grandTotal ?? 0),
      0,
    )
    const draftCount = returns.filter((r) => r.status === 'Draft').length
    const pendingCount = returns.filter((r) => r.status === 'Pending Approval').length
    const approvedCount = returns.filter((r) => r.status === 'Approved').length
    const completedCount = returns.filter(
      (r) => r.status === 'Completed' || r.status === 'Refund Processed',
    ).length

    return {
      totalCount,
      totalValue,
      draftCount,
      pendingCount,
      approvedCount,
      completedCount,
    }
  }, [returns])

  const selectedReturns = useMemo(() => {
    const selectedSet = new Set(selectedReturnIds.map(String))
    return returns.filter((r) => selectedSet.has(getReturnRowKey(r)))
  }, [returns, selectedReturnIds])

  const tableReturns = useMemo(
    () =>
      returns.map((r, index) => ({
        ...r,
        __rowKey: getReturnRowKey(r) || `pr-return-${index}`,
      })),
    [returns],
  )

  const filteredReturns = useMemo(() => {
    if (statusFilter === 'all') {
      return tableReturns
    }
    return tableReturns.filter((r) => {
      const st = String(r.status || 'Draft').trim().toLowerCase()
      const filterKey = statusFilter.toLowerCase()
      return st === filterKey
    })
  }, [statusFilter, tableReturns])

  const handleDelete = async (row) => {
    const id = row.returnId || row.id
    const label = row.returnNumber || `PR-${id}`
    if (!window.confirm(`Are you sure you want to delete ${label}?`)) return
    try {
      const res = await deletePurchaseReturn(id)
      if (res?.success || !res?.error) {
        showToast({
          type: 'success',
          title: 'Purchase Returns',
          message: `${label} deleted successfully.`,
        })
        fetchReturns()
      } else {
        showToast({
          type: 'error',
          title: 'Delete Failed',
          message: getPurchaseReturnErrorMessage(res, 'Failed to delete return.'),
        })
      }
    } catch (err) {
      showToast({
        type: 'error',
        title: 'Delete Error',
        message: getPurchaseReturnErrorMessage(err, 'Failed to delete return.'),
      })
    }
  }

  const handleSubmit = async (id) => {
    try {
      const res = await submitPurchaseReturn(id)
      if (res?.success || !res?.error) {
        showToast({ type: 'success', title: 'Purchase Returns', message: 'Purchase return submitted for approval.' })
        fetchReturns()
      } else {
        showToast({ type: 'error', title: 'Submit Failed', message: getPurchaseReturnErrorMessage(res, 'Failed to submit return.') })
      }
    } catch (err) {
      showToast({ type: 'error', title: 'Submit Error', message: getPurchaseReturnErrorMessage(err, 'Failed to submit return.') })
    }
  }

  const handleApprove = async (id) => {
    if (!window.confirm('Approve this purchase return? Inventory will be deducted and return movements logged.')) return
    try {
      const res = await approvePurchaseReturn(id)
      if (res?.success || !res?.error) {
        showToast({ type: 'success', title: 'Purchase Returns', message: 'Purchase return approved successfully.' })
        fetchReturns()
      } else {
        showToast({ type: 'error', title: 'Approve Failed', message: getPurchaseReturnErrorMessage(res, 'Failed to approve return.') })
      }
    } catch (err) {
      showToast({ type: 'error', title: 'Approve Error', message: getPurchaseReturnErrorMessage(err, 'Failed to approve return.') })
    }
  }

  const exportCSV = () => {
    if (returns.length === 0) return
    const headers = ['Return Number', 'GRN Number', 'Supplier', 'Return Date', 'Items', 'Total Amount', 'Status']
    const rowsCSV = returns.map((r) => [
      r.returnNumber || `PR-${r.returnId || r.id}`,
      r.grnNumber || (r.grnId ? `GRN-${r.grnId}` : ''),
      r.supplierName || '',
      r.returnDate ? formatDate(r.returnDate) : '',
      r.itemCount ?? r.items?.length ?? 0,
      r.totalAmount ?? r.totalReturnAmount ?? r.grandTotal ?? 0,
      r.status || 'Draft',
    ])
    const csvContent =
      'data:text/csv;charset=utf-8,' +
      [headers.join(','), ...rowsCSV.map((e) => e.join(','))].join('\n')
    const encodedUri = encodeURI(csvContent)
    const link = document.createElement('a')
    link.setAttribute('href', encodedUri)
    link.setAttribute('download', `purchase-returns-${new Date().toISOString().substring(0, 10)}.csv`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
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
        `${r.returnNumber || ''} ${r.grnNumber || ''} ${r.supplierName || ''} ${r.status || ''} ${r.reason || ''}`,
      render: (r) => {
        const id = r.returnId || r.id
        return (
          <div className="catalog-page__tree-cell">
            <button
              type="button"
              className="catalog-page__tree-toggle"
              onClick={(e) => {
                e.stopPropagation()
                navigate(`/purchase-returns/returns/${id}`)
              }}
              title="View Return Details"
            >
              <ChevronRight size={16} />
            </button>
            <RotateCcw size={16} className="catalog-page__tree-icon" style={{ color: '#059669' }} />
            <div className="catalog-page__entity">
              <strong style={{ color: '#059669' }}>
                {r.returnNumber || `PR-${String(id).padStart(5, '0')}`}
              </strong>
              {r.returnDate ? <span>{formatDate(r.returnDate)}</span> : null}
            </div>
          </div>
        )
      },
    },
    {
      key: 'grnNumber',
      label: 'GRN Number',
      sortable: true,
      tableWidth: 180,
      style: { width: 180, minWidth: 160 },
      headerStyle: { width: 180, minWidth: 160 },
      render: (r) => (
        <div className="sales-page__invoice-cell">
          <ReceiptText size={15} style={{ color: '#475569' }} />
          <strong>{r.grnNumber || (r.grnId ? `GRN-${String(r.grnId).padStart(6, '0')}` : '—')}</strong>
        </div>
      ),
    },
    {
      key: 'supplierName',
      label: 'Supplier',
      sortable: true,
      tableWidth: 200,
      style: { width: 200, minWidth: 180 },
      headerStyle: { width: 200, minWidth: 180 },
      searchValue: (r) => r.supplierName || '',
      render: (r) => (
        <div className="sales-page__customer-cell">
          <Building2 size={15} />
          <span style={{ whiteSpace: 'nowrap', fontWeight: 600, color: '#111827' }}>
            {r.supplierName || 'Unassigned Supplier'}
          </span>
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
          {r.itemCount ?? r.items?.length ?? 1}
        </span>
      ),
    },
    {
      key: 'totalAmount',
      label: 'Return Amount',
      sortable: true,
      tableWidth: 150,
      style: { width: 150, minWidth: 130 },
      headerStyle: { width: 150, minWidth: 130 },
      sortValue: (r) => Number(r.totalAmount ?? r.totalReturnAmount ?? r.grandTotal ?? 0),
      render: (r) => (
        <strong style={{ whiteSpace: 'nowrap', fontWeight: 600, color: '#059669' }}>
          {formatCurrency(r.totalAmount ?? r.totalReturnAmount ?? r.grandTotal ?? 0)}
        </strong>
      ),
    },
    {
      key: 'status',
      label: 'Status',
      sortable: true,
      mobileStatus: true,
      tableWidth: 140,
      style: { width: 140, minWidth: 120 },
      headerStyle: { width: 140, minWidth: 120 },
      render: (r) => <StatusBadge status={r.status || 'Draft'} />,
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
        const id = r.returnId || r.id
        const st = r.status || 'Draft'
        return (
          <ActionMenu
            iconOnly
            label={`Actions for ${r.returnNumber || id}`}
            actions={[
              {
                key: 'details',
                label: 'View Details',
                icon: Eye,
                onClick: () => navigate(`/purchase-returns/returns/${id}`),
              },
              st === 'Draft' ? {
                key: 'edit',
                label: 'Edit Draft',
                icon: Edit,
                onClick: () => navigate(`/purchase-returns/returns/${id}/edit`),
              } : null,
              st === 'Draft' ? {
                key: 'submit',
                label: 'Submit for Approval',
                icon: Send,
                onClick: () => handleSubmit(id),
              } : null,
              st === 'Pending Approval' ? {
                key: 'approve',
                label: 'Approve Return',
                icon: CheckCircle,
                onClick: () => handleApprove(id),
              } : null,
              st === 'Draft' ? {
                key: 'delete',
                label: 'Delete Draft',
                icon: Trash2,
                variant: 'danger',
                onClick: () => handleDelete(r),
              } : null,
            ].filter(Boolean)}
          />
        )
      },
    },
  ]

  return (
    <div className="page sales-page">
      {/* Compact Header matching SalesReturnsList.jsx exactly */}
      <header className="sales-page__compact-header" aria-label="Purchase returns summary">
        <div className="sales-page__compact-main">
          <h1>Purchase Returns</h1>
          <div className="sales-page__metrics" aria-label="Purchase return metrics">
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
              {summary.completedCount} Completed
            </span>
          </div>
        </div>

        <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
          <button
            type="button"
            className="button button-secondary"
            onClick={exportCSV}
            title="Export Purchase Returns CSV"
          >
            <Download size={15} />
            Export
          </button>
          <button
            type="button"
            className="button button-primary sales-page__add-button"
            onClick={() => navigate('/purchase-returns/create')}
          >
            <Plus size={16} />
            Create Purchase Return
          </button>
        </div>
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

      {/* Main Table Card reusing DataTable & Filter Toolbar */}
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
          searchPlaceholder="Search purchase returns by return #, GRN, supplier, or status..."
          toolbarContent={
            selectedReturns.length === 0 ? (
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
            ) : null
          }
          filterContent={
            selectedReturns.length === 0 ? (
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
            )
          }
          columnStorageKey="ims.purchaseReturns.visibleColumns.v2"
          defaultVisibleColumnKeys={[
            'returnNumber',
            'grnNumber',
            'supplierName',
            'returnDate',
            'itemCount',
            'totalAmount',
            'status',
            'actions',
          ]}
          fitExplicitColumnsToContainer
          enableRowSelection
          selectedRowKeys={selectedReturnIds}
          onSelectionChange={setSelectedReturnIds}
          keyField="__rowKey"
          emptyMessage="No purchase returns available."
        />
      </div>
    </div>
  )
}
