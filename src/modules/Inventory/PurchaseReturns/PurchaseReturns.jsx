import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Check, Eye, Pencil, Plus, RefreshCw, Trash2 } from 'lucide-react'
import { ActionMenu, DataTable, FilterBar } from '../../../components/erp'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import FormModal from '../../../layouts/FormModal'
import {
  deletePurchaseReturn,
  getPurchaseReturns,
  getPurchaseReturnSuppliers,
} from '../../../api/purchaseReturnApi'
import { formatCurrency, formatDate } from '../../../utils/helpers'
import './PurchaseReturns.css'

const getArrayFromResponse = (response) => {
  if (Array.isArray(response)) return response
  if (Array.isArray(response?.data)) return response.data
  if (Array.isArray(response?.data?.items)) return response.data.items
  if (Array.isArray(response?.items)) return response.items
  return null
}

const getSupplierId = (supplier) =>
  supplier?.id ?? supplier?.supplierId ?? supplier?.supplier_id

const getSupplierName = (supplier) =>
  supplier?.name ?? supplier?.supplierName ?? supplier?.supplier_name ?? (getSupplierId(supplier) ? `Supplier #${getSupplierId(supplier)}` : '-')

const getReturnId = (item) =>
  item?.purchaseReturnId ?? item?.returnId ?? item?.return_id ?? item?.id

const getReturnNumberDisplay = (item) => {
  const num = item?.returnNumber ?? item?.return_number
  if (num) return String(num).startsWith('#') ? num : `#${num}`
  const retId = getReturnId(item)
  return retId ? `#PRR-${String(retId).padStart(6, '0')}` : '-'
}

export default function PurchaseReturns() {
  const navigate = useNavigate()

  const [returns, setReturns] = useState([])
  const [suppliers, setSuppliers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [supplierFilter, setSupplierFilter] = useState('')

  const [selectedRowKeys, setSelectedRowKeys] = useState([])
  const [deleteTargetId, setDeleteTargetId] = useState(null)
  const [deleting, setDeleting] = useState(false)

  // Fetch Purchase Returns & Suppliers from Backend API
  const fetchData = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      const [returnsResponse, suppliersResponse] = await Promise.all([
        getPurchaseReturns(),
        getPurchaseReturnSuppliers(),
      ])

      const returnsData = getArrayFromResponse(returnsResponse)
      const suppliersData = getArrayFromResponse(suppliersResponse)

      if (returnsData === null) {
        throw new Error('Purchase Returns API returned an unexpected response format.')
      }

      setReturns(returnsData.filter(Boolean))
      setSuppliers(suppliersData || [])
    } catch (err) {
      setReturns([])
      setSuppliers([])
      setError(
        err?.response?.data?.message ||
        err?.response?.data?.title ||
        err?.message ||
        'Failed to load Purchase Returns.'
      )
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    fetchData()
  }, [fetchData])

  // Supplier Lookup Map
  const suppliersMap = useMemo(() => {
    const map = {}
    suppliers.forEach((supplier) => {
      const id = getSupplierId(supplier)
      if (id !== null && id !== undefined && id !== '') {
        map[String(id)] = getSupplierName(supplier)
      }
    })
    return map
  }, [suppliers])

  // Filter Returns based on Supplier Filter
  const filteredReturns = useMemo(() => {
    const list = Array.isArray(returns) ? returns.filter(Boolean) : []
    if (!supplierFilter) return list
    return list.filter((item) => String(item.supplierId ?? item.supplier_id ?? '') === String(supplierFilter))
  }, [returns, supplierFilter])

  // Header Metrics Summary
  const stats = useMemo(() => {
    const list = Array.isArray(returns) ? returns.filter(Boolean) : []
    const total = list.length
    const uniqueSuppliers = new Set(
      list.map((item) => item.supplierId || item.supplier_id).filter(Boolean)
    ).size
    const totalAmount = list.reduce(
      (sum, item) => sum + Number(item.totalAmount || item.total_amount || item.totalReturnAmount || 0),
      0
    )

    return {
      total,
      suppliers: uniqueSuppliers,
      totalAmount,
    }
  }, [returns])

  // Delete Handler
  const handleDeleteConfirm = async () => {
    if (!deleteTargetId || deleting) return
    setDeleting(true)
    try {
      await deletePurchaseReturn(deleteTargetId)
      setReturns((prev) => prev.filter((item) => String(getReturnId(item)) !== String(deleteTargetId)))
      showToast('Purchase return deleted successfully.', 'success')
    } catch (err) {
      showToast(
        err?.response?.data?.message || err?.message || 'Failed to delete purchase return.',
        'error'
      )
    } finally {
      setDeleting(false)
      setDeleteTargetId(null)
    }
  }

  // DataTable Column Definitions matching Purchase Indents UI
  const columns = useMemo(() => [
    {
      key: 'returnNumber',
      label: 'Return ID',
      sortable: true,
      mobilePrimary: true,
      tableWidth: 170,
      style: { width: 170, minWidth: 170 },
      headerStyle: { width: 170, minWidth: 170 },
      searchValue: (row) => {
        const supId = row.supplierId ?? row.supplier_id
        const supName = suppliersMap[String(supId ?? '')] || row.supplierName || ''
        const grnNum = row.grnNumber || (row.grnId ? `GRN-${row.grnId}` : '')
        return `${getReturnNumberDisplay(row)} ${supName} ${grnNum} ${row.reason || ''}`
      },
      render: (row) => {
        const retId = getReturnId(row)
        return (
          <span className="font-semibold text-primary" style={{ cursor: 'pointer' }} onClick={() => navigate(`/inventory/purchase-returns/${retId}`)}>
            {getReturnNumberDisplay(row)}
          </span>
        )
      },
    },
    {
      key: 'supplierName',
      label: 'Supplier',
      sortable: true,
      tableWidth: 220,
      style: { width: 220, minWidth: 220 },
      headerStyle: { width: 220, minWidth: 220 },
      render: (row) => {
        const supId = row.supplierId ?? row.supplier_id
        return suppliersMap[String(supId ?? '')] ?? row.supplierName ?? row.supplier_name ?? (supId ? `Supplier #${supId}` : '-')
      },
    },
    {
      key: 'grnNumber',
      label: 'GRN',
      sortable: true,
      tableWidth: 150,
      style: { width: 150, minWidth: 150 },
      headerStyle: { width: 150, minWidth: 150 },
      render: (row) => {
        const grnId = row.grnId ?? row.grn_id
        const grnNum = row.grnNumber || row.grn_number || (grnId ? `GRN-${grnId}` : '-')
        return <span className="grn-badge">{grnNum}</span>
      },
    },
    {
      key: 'returnDate',
      label: 'Return Date',
      sortable: true,
      tableWidth: 140,
      style: { width: 140, minWidth: 140 },
      headerStyle: { width: 140, minWidth: 140 },
      render: (row) => {
        const rDate = row.returnDate ?? row.return_date
        return rDate ? formatDate(rDate) : '-'
      },
    },
    {
      key: 'totalAmount',
      label: 'Total Amount',
      sortable: true,
      tableWidth: 160,
      style: { width: 160, minWidth: 160 },
      headerStyle: { width: 160, minWidth: 160 },
      render: (row) => {
        const amt = Number(row.totalAmount ?? row.total_amount ?? row.totalReturnAmount ?? 0)
        return (
          <span className="font-semibold" style={{ color: '#1e293b' }}>
            {formatCurrency(amt)}
          </span>
        )
      },
    },
    {
      key: 'reason',
      label: 'Reason',
      sortable: true,
      tableWidth: 240,
      style: { width: 240, minWidth: 240 },
      headerStyle: { width: 240, minWidth: 240 },
      render: (row) => (
        <span className="reason-cell" title={row.reason || ''}>
          {row.reason ? (row.reason.length > 45 ? `${row.reason.slice(0, 45)}...` : row.reason) : '-'}
        </span>
      ),
    },
    {
      key: 'actions',
      label: 'Actions',
      searchable: false,
      hideable: false,
      className: 'purchases-page__col-actions',
      tableWidth: 80,
      style: { width: 80, minWidth: 80, maxWidth: 80 },
      headerStyle: { width: 80, minWidth: 80, maxWidth: 80 },
      render: (row) => {
        const retId = getReturnId(row)
        return (
          <ActionMenu
            iconOnly
            label={`Actions for ${getReturnNumberDisplay(row)}`}
            menuKey={retId}
            className="purchases-page__row-actions"
            actions={[
              {
                key: 'view',
                label: 'View Details',
                icon: Eye,
                onClick: () => navigate(`/inventory/purchase-returns/${retId}`),
              },
              {
                key: 'edit',
                label: 'Edit',
                icon: Pencil,
                onClick: () => navigate(`/inventory/purchase-returns/edit/${retId}`),
              },
              {
                key: 'delete',
                label: 'Delete',
                icon: Trash2,
                tone: 'danger',
                onClick: () => setDeleteTargetId(retId),
              },
            ]}
          />
        )
      },
    },
  ], [navigate, suppliersMap])

  const hasSelection = selectedRowKeys.length > 0

  // Selection toolbar matching Purchase Indents
  const selectionToolbar = hasSelection ? (
    <FilterBar className="resource-center__product-style-selection-actions" ariaLabel="Selected purchase returns actions">
      <div className="resource-center__product-style-selection-summary" aria-live="polite">
        <Check size={15} />
        <strong>{selectedRowKeys.length} selected</strong>
      </div>
      <button
        type="button"
        className="button button-secondary resource-center__product-style-selection-button"
        onClick={() => setSelectedRowKeys([])}
      >
        Clear Selection
      </button>
    </FilterBar>
  ) : (
    <FilterBar className="purchases-page__table-actions" style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
      <div className="filter-group" style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        <select
          id="supplier-filter-select"
          value={supplierFilter}
          onChange={(e) => setSupplierFilter(e.target.value)}
          style={{
            height: '38px',
            padding: '0 32px 0 12px',
            border: '1px solid #cbd5e1',
            borderRadius: '8px',
            fontSize: '13px',
            fontWeight: '500',
            color: '#334155',
            backgroundColor: '#ffffff',
            cursor: 'pointer',
          }}
        >
          <option value="">All Suppliers</option>
          {suppliers.map((supplier) => {
            const supplierId = getSupplierId(supplier)
            if (!supplierId) return null
            return (
              <option key={String(supplierId)} value={String(supplierId)}>
                {getSupplierName(supplier)}
              </option>
            )
          })}
        </select>
      </div>

      <button
        type="button"
        className="button button-secondary"
        onClick={fetchData}
        disabled={loading}
      >
        <RefreshCw size={16} className={loading ? 'animate-spin' : ''} />
        Refresh
      </button>
    </FilterBar>
  )

  if (error) {
    return (
      <main className="purchase-returns-page">
        <PageHeader title="Purchase Returns" />
        <div className="card purchase-returns-error-card" style={{ padding: '24px', display: 'flex', alignItems: 'center', gap: '16px' }}>
          <RefreshCw size={24} style={{ color: '#ef4444' }} />
          <div style={{ flex: 1 }}>
            <h3 style={{ margin: '0 0 4px 0' }}>Unable to load Purchase Returns</h3>
            <p style={{ margin: 0, color: '#64748b' }}>{String(error)}</p>
          </div>
          <button className="erp-button erp-button--primary" onClick={fetchData} type="button">
            Retry
          </button>
        </div>
      </main>
    )
  }

  return (
    <main className="purchase-returns-page">
      {/* Compact Header matching Purchase Indents */}
      <header className="purchases-page__compact-header">
        <div className="purchases-page__compact-main">
          <h1>Purchase Returns</h1>
          <div className="purchases-page__metrics">
            <span className="purchases-page__metric purchases-page__metric--info">
              {stats.total} Returns
            </span>
            <span className="purchases-page__metric purchases-page__metric--warning">
              {stats.suppliers} Suppliers
            </span>
            <span className="purchases-page__metric purchases-page__metric--success">
              {formatCurrency(stats.totalAmount)} Total Returned
            </span>
          </div>
        </div>
        <button
          type="button"
          className="button button-primary"
          onClick={() => navigate('/inventory/purchase-returns/create')}
          style={{ display: 'inline-flex', alignItems: 'center', gap: '6px' }}
        >
          <Plus size={16} />
          Create Purchase Return
        </button>
      </header>

      {/* Main DataTable Card matching Purchase Indents UI */}
      <div className="card purchases-page__table-card">
        <DataTable
          className="purchases-page__table"
          rows={filteredReturns}
          columns={columns}
          loading={loading}
          defaultPageSize={20}
          defaultSortKey="returnDate"
          defaultSortDirection="desc"
          splitToolbar
          toolbarContent={selectionToolbar}
          enableRowSelection={true}
          hideSelectionSummary={true}
          selectedRowKeys={selectedRowKeys}
          onSelectionChange={setSelectedRowKeys}
          keyField="purchaseReturnId"
          showSearch={!hasSelection}
          showColumnControls={!hasSelection}
          columnStorageKey="ims.purchase-returns.visibleColumns.v1"
          defaultVisibleColumnKeys={['returnNumber', 'supplierName', 'grnNumber', 'returnDate', 'totalAmount', 'reason', 'actions']}
          fitExplicitColumnsToContainer={false}
          searchPlaceholder="Search by Return ID, Supplier, GRN, or Reason..."
          emptyMessage="No purchase returns found."
        />
      </div>

      {/* Delete Confirmation Modal */}
      {deleteTargetId && (
        <FormModal
          isOpen={Boolean(deleteTargetId)}
          title="Delete Purchase Return?"
          onClose={() => setDeleteTargetId(null)}
        >
          <div className="delete-confirm-content">
            <p>This action will permanently delete this purchase return and its associated items.</p>
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