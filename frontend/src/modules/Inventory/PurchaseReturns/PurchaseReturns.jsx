import {
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react'

import { useNavigate } from 'react-router-dom'

import {
  AlertCircle,
  Eye,
  Pencil,
  Plus,
  RefreshCw,
  Trash2,
} from 'lucide-react'

import PageHeader from '../../../components/common/PageHeader'
import { showToast } from '../../../components/common/toast'
import FormModal from '../../../layouts/FormModal'
import { ActionMenu, DataTable, FilterBar } from '../../../components/erp'

import {
  deletePurchaseReturn,
  getPurchaseReturns,
  getPurchaseReturnSuppliers,
} from '../../../api/purchaseReturnApi'

import {
  formatCurrency,
  formatDate,
} from '../../../utils/helpers'

import '../../../components/tables/TableComponent.css'
import './PurchaseReturns.css'

/**
 * Normalize API responses without hiding API errors.
 *
 * Supports:
 * 1. Direct array
 * 2. { data: [] }
 * 3. { data: { items: [] } }
 * 4. { items: [] }
 */
const getArrayFromResponse = (response) => {
  if (Array.isArray(response)) {
    return response
  }

  if (Array.isArray(response?.data)) {
    return response.data
  }

  if (Array.isArray(response?.data?.items)) {
    return response.data.items
  }

  if (Array.isArray(response?.items)) {
    return response.items
  }

  return null
}

const getSupplierId = (supplier) =>
  supplier?.id ??
  supplier?.supplierId ??
  supplier?.supplier_id

const getSupplierName = (supplier) =>
  supplier?.name ??
  supplier?.supplierName ??
  supplier?.supplier_name ??
  (
    getSupplierId(supplier)
      ? `Supplier #${getSupplierId(supplier)}`
      : '-'
  )

const getGrnId = (grn) =>
  grn?.id ??
  grn?.grnId ??
  grn?.grn_id

const getGrnNumber = (grn) =>
  grn?.grnNumber ??
  grn?.grn_number ??
  grn?.number ??
  (
    getGrnId(grn)
      ? `GRN-${getGrnId(grn)}`
      : '-'
  )

const getReturnId = (item) =>
  item?.purchaseReturnId ??
  item?.returnId ??
  item?.return_id ??
  item?.id

const getReturnNumberDisplay = (item) =>
  item?.returnNumber ??
  item?.return_number ??
  (getReturnId(item) ? `#${getReturnId(item)}` : '-')

const getReturnSupplierId = (item) =>
  item?.supplierId ??
  item?.supplier_id

const getReturnGrnId = (item) =>
  item?.grnId ??
  item?.grn_id

const getReturnDate = (item) =>
  item?.returnDate ??
  item?.return_date

const getTotalAmount = (item) =>
  item?.totalAmount ??
  item?.total_amount ??
  item?.totalReturnAmount ??
  0

export default function PurchaseReturns() {
  const navigate = useNavigate()

  // =========================================================
  // STATE
  // =========================================================

  const [returns, setReturns] = useState([])
  const [suppliers, setSuppliers] = useState([])
  const [grns, setGrns] = useState([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const [supplierFilter, setSupplierFilter] = useState('')

  const [deleteTargetId, setDeleteTargetId] = useState(null)
  const [deleting, setDeleting] = useState(false)

  // =========================================================
  // LOAD DATA
  // =========================================================

  const fetchData = useCallback(async () => {
    setLoading(true)
    setError('')

    try {
      const [
        returnsResponse,
        suppliersResponse,
      ] = await Promise.all([
        getPurchaseReturns(),
        getPurchaseReturnSuppliers(),
      ])

      const returnsData = getArrayFromResponse(returnsResponse)
      const suppliersData = getArrayFromResponse(suppliersResponse)

      if (returnsData === null) {
        throw new Error(
          'Purchase Returns API returned an unexpected response format.'
        )
      }

      if (suppliersData === null) {
        throw new Error(
          'Purchase Return Suppliers API returned an unexpected response format.'
        )
      }

      setReturns(returnsData)
      setSuppliers(suppliersData)
      setGrns([])
    } catch (err) {
      console.error(
        'Purchase Returns API error:',
        err
      )

      setReturns([])
      setSuppliers([])
      setGrns([])

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

  // =========================================================
  // SUPPLIER LOOKUP
  // =========================================================

  const suppliersMap = useMemo(() => {
    const map = {}

    suppliers.forEach((supplier) => {
      const id = getSupplierId(supplier)

      if (
        id === null ||
        id === undefined ||
        id === ''
      ) {
        return
      }

      map[String(id)] = getSupplierName(supplier)
    })

    return map
  }, [suppliers])

  // =========================================================
  // GRN LOOKUP
  // =========================================================

  const grnsMap = useMemo(() => {
    const map = {}

    grns.forEach((grn) => {
      const id = getGrnId(grn)

      if (
        id === null ||
        id === undefined ||
        id === ''
      ) {
        return
      }

      map[String(id)] = getGrnNumber(grn)
    })

    return map
  }, [grns])

  // =========================================================
  // FILTER RETURNS
  // =========================================================

  const filteredReturns = useMemo(() => {
    if (!supplierFilter) {
      return returns
    }

    return returns.filter((item) => {
      const supplierId = getReturnSupplierId(item)
      return String(supplierId ?? '') === String(supplierFilter)
    })
  }, [returns, supplierFilter])

  const tableRows = useMemo(() => {
    return filteredReturns.map((row, index) => {
      const returnId = getReturnId(row) ?? `return-${index}`
      const supplierId = getReturnSupplierId(row)
      const supplierName =
        suppliersMap[String(supplierId ?? '')] ??
        row?.supplierName ??
        row?.supplier_name ??
        (supplierId ? `Supplier #${supplierId}` : '-')
      const grnId = getReturnGrnId(row)
      const grnNumber =
        grnsMap[String(grnId ?? '')] ??
        row?.grnNumber ??
        row?.grn_number ??
        (grnId ? `GRN-${grnId}` : '-')
      const returnNumberDisplay = getReturnNumberDisplay(row)
      const returnDate = getReturnDate(row)
      const totalAmount = getTotalAmount(row)

      return {
        ...row,
        id: returnId,
        returnId,
        returnNumberDisplay,
        supplierName,
        grnNumber,
        returnDate,
        totalAmount,
      }
    })
  }, [filteredReturns, suppliersMap, grnsMap])

  // =========================================================
  // ACTIONS
  // =========================================================

  const handleCreate = useCallback(() => {
    navigate('/inventory/purchase-returns/create')
  }, [navigate])

  const handleView = useCallback((id) => {
    if (id !== null && id !== undefined) {
      navigate(`/inventory/purchase-returns/${id}`)
    }
  }, [navigate])

  const handleEdit = useCallback((id) => {
    if (id !== null && id !== undefined) {
      navigate(`/inventory/purchase-returns/edit/${id}`)
    }
  }, [navigate])

  const handleDelete = useCallback((id) => {
    if (id !== null && id !== undefined) {
      setDeleteTargetId(id)
    }
  }, [])

  const handleDeleteConfirm = async () => {
    if (
      deleteTargetId === null ||
      deleteTargetId === undefined ||
      deleting
    ) {
      return
    }

    setDeleting(true)

    try {
      await deletePurchaseReturn(deleteTargetId)

      setReturns((previous) =>
        previous.filter((item) => {
          const itemId = getReturnId(item)
          return String(itemId) !== String(deleteTargetId)
        })
      )

      showToast(
        'Purchase return deleted successfully.',
        'success'
      )
    } catch (err) {
      console.error(
        'Delete Purchase Return error:',
        err
      )

      showToast(
        err?.response?.data?.message ||
        err?.response?.data?.title ||
        err?.message ||
        'Failed to delete purchase return.',
        'error'
      )
    } finally {
      setDeleting(false)
      setDeleteTargetId(null)
    }
  }

  // =========================================================
  // TABLE COLUMNS
  // =========================================================

  const columns = useMemo(() => [
    {
      key: 'returnNumber',
      label: 'Return ID',
      sortable: true,
      mobilePrimary: true,
      tableWidth: 150,
      style: { width: 150, minWidth: 150 },
      headerStyle: { width: 150, minWidth: 150 },
      searchValue: (row) =>
        `${row.returnNumberDisplay || getReturnNumberDisplay(row)} ${getReturnId(row)}`,
      render: (row) => {
        const returnId = getReturnId(row)
        return (
          <button
            type="button"
            className="link-button font-semibold text-primary"
            onClick={() => handleView(returnId)}
            style={{
              background: 'none',
              border: 'none',
              padding: 0,
              cursor: 'pointer',
              textAlign: 'left',
              font: 'inherit',
            }}
          >
            {row.returnNumberDisplay || getReturnNumberDisplay(row)}
          </button>
        )
      },
    },
    {
      key: 'supplierName',
      label: 'Supplier',
      sortable: true,
      tableWidth: 200,
      style: { width: 200, minWidth: 180 },
      headerStyle: { width: 200, minWidth: 180 },
      searchValue: (row) => row.supplierName || '',
      render: (row) => row.supplierName || '-',
    },
    {
      key: 'grnNumber',
      label: 'GRN',
      align: 'center',
      sortable: true,
      className: 'text-center grn-cell',
      headerClassName: 'text-center grn-header',
      tableWidth: 140,
      style: { width: 140, minWidth: 120, textAlign: 'center' },
      headerStyle: { width: 140, minWidth: 120, textAlign: 'center' },
      searchValue: (row) => row.grnNumber || '',
      render: (row) => row.grnNumber || '-',
    },
    {
      key: 'returnDate',
      label: 'Return Date',
      sortable: true,
      className: 'date-cell',
      tableWidth: 150,
      style: { width: 150, minWidth: 130 },
      headerStyle: { width: 150, minWidth: 130 },
      render: (row) => (row.returnDate ? formatDate(row.returnDate) : '-'),
    },
    {
      key: 'totalAmount',
      label: 'Total Amount',
      align: 'right',
      sortable: true,
      className: 'text-right font-semibold amount-cell',
      headerClassName: 'text-right',
      tableWidth: 160,
      style: { width: 160, minWidth: 140, textAlign: 'right' },
      headerStyle: { width: 160, minWidth: 140, textAlign: 'right' },
      render: (row) => formatCurrency(row.totalAmount ?? 0),
    },
    {
      key: 'reason',
      label: 'Reason',
      sortable: true,
      className: 'reason-cell',
      tableWidth: 220,
      style: { width: 220, minWidth: 180 },
      headerStyle: { width: 220, minWidth: 180 },
      searchValue: (row) => row.reason || '',
      render: (row) => {
        const reason = row?.reason ?? ''
        return (
          <span className="reason-cell" title={reason}>
            {reason ? (reason.length > 50 ? `${reason.slice(0, 50)}...` : reason) : '-'}
          </span>
        )
      },
    },
    {
      key: 'actions',
      label: 'Actions',
      align: 'center',
      searchable: false,
      hideable: false,
      className: 'text-center actions-cell',
      headerClassName: 'text-center actions-header',
      tableWidth: 80,
      style: { width: 80, minWidth: 80, maxWidth: 80, textAlign: 'center' },
      headerStyle: { width: 80, minWidth: 80, maxWidth: 80, textAlign: 'center' },
      render: (row) => {
        const returnId = getReturnId(row)
        return (
          <ActionMenu
            iconOnly
            label={`Actions for ${row.returnNumberDisplay || returnId}`}
            menuKey={returnId}
            actions={[
              {
                key: 'view',
                label: 'View Details',
                icon: Eye,
                onClick: () => handleView(returnId),
              },
              {
                key: 'edit',
                label: 'Edit',
                icon: Pencil,
                onClick: () => handleEdit(returnId),
              },
              {
                key: 'delete',
                label: 'Delete',
                icon: Trash2,
                tone: 'danger',
                onClick: () => handleDelete(returnId),
              },
            ]}
          />
        )
      },
    },
  ], [handleView, handleEdit, handleDelete])

  // =========================================================
  // TOOLBAR CONTENT
  // =========================================================

  const filterContent = (
    <FilterBar className="purchase-returns__filters" ariaLabel="Purchase return filters">
      <div className="filter-group">
        <label htmlFor="supplier-select">Supplier:</label>
        <select
          id="supplier-select"
          value={supplierFilter}
          onChange={(event) => setSupplierFilter(event.target.value)}
          aria-label="Filter by supplier"
        >
          <option value="">All Suppliers</option>
          {suppliers.map((supplier) => {
            const supplierId = getSupplierId(supplier)
            if (supplierId === null || supplierId === undefined || supplierId === '') {
              return null
            }
            return (
              <option key={String(supplierId)} value={String(supplierId)}>
                {getSupplierName(supplier)}
              </option>
            )
          })}
        </select>
      </div>

      {supplierFilter ? (
        <button
          className="erp-button erp-button--secondary"
          type="button"
          onClick={() => setSupplierFilter('')}
        >
          Reset Filters
        </button>
      ) : null}
    </FilterBar>
  )

  const toolbarContent = (
    <FilterBar className="purchase-returns__toolbar-actions" ariaLabel="Purchase return table refresh actions">
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

  // =========================================================
  // RENDER
  // =========================================================

  return (
    <main className="purchase-returns-page">
      <PageHeader
        title="Purchase Returns"
        subtitle="Manage and track goods returned to suppliers."
        primaryAction={{
          icon: Plus,
          label: 'Create Purchase Return',
          onClick: handleCreate,
        }}
      />

      {error ? (
        <div className="purchase-returns-error-card card">
          <AlertCircle
            size={24}
            className="error-icon"
          />

          <div>
            <h3>We could not load this workspace</h3>
            <p>{error}</p>
          </div>

          <button
            className="erp-button erp-button--primary"
            onClick={fetchData}
            type="button"
          >
            <RefreshCw size={14} />
            Retry
          </button>
        </div>
      ) : null}

      <div className="card purchase-returns-table-card purchases-page__table-card">
        <DataTable
          className="purchases-page__table purchase-returns-data-table"
          rows={tableRows}
          columns={columns}
          loading={loading}
          defaultPageSize={10}
          defaultSortKey="returnDate"
          defaultSortDirection="desc"
          splitToolbar
          filterContent={filterContent}
          toolbarContent={toolbarContent}
          showSearch={true}
          showColumnControls={true}
          columnStorageKey="ims.purchase-returns.visibleColumns.v1"
          defaultVisibleColumnKeys={[
            'returnNumber',
            'supplierName',
            'grnNumber',
            'returnDate',
            'totalAmount',
            'reason',
            'actions',
          ]}
          searchPlaceholder="Search by Return ID, Supplier, GRN or Reason"
          emptyMessage="No purchase returns found."
          keyField="returnId"
        />
      </div>

      {deleteTargetId !== null && deleteTargetId !== undefined ? (
        <FormModal
          isOpen={true}
          title="Delete Purchase Return?"
          onClose={() => !deleting && setDeleteTargetId(null)}
        >
          <div className="delete-confirm-content">
            <p>
              This action will permanently remove this purchase return and its associated items.
            </p>

            <p className="delete-warning">
              Return ID: #{deleteTargetId}
            </p>

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
      ) : null}
    </main>
  )
}