import {
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react'

import { useNavigate } from 'react-router-dom'

import {
  AlertCircle,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  Eye,
  Pencil,
  Plus,
  RefreshCw,
  Search,
  Trash2,
} from 'lucide-react'

import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { showToast } from '../../../components/common/toast'
import FormModal from '../../../layouts/FormModal'
import { ActionMenu } from '../../../components/erp'
import Pagination from '../../../components/erp/Pagination'

import {
  deletePurchaseReturn,
  getPurchaseReturns,
  getPurchaseReturnGrns,
  getPurchaseReturnSuppliers,
} from '../../../api/purchaseReturnApi'

import {
  formatCurrency,
  formatDate,
} from '../../../utils/helpers'

import '../../../components/tables/TableComponent.css'
import './PurchaseReturns.css'

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

  const [searchQuery, setSearchQuery] = useState('')
  const [supplierFilter, setSupplierFilter] = useState('')

  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

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


      const returnsData =
        getArrayFromResponse(returnsResponse)

      const suppliersData =
        getArrayFromResponse(suppliersResponse)


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
    const query = searchQuery
      .trim()
      .toLowerCase()

    return returns.filter((item) => {
      const returnId = String(
        getReturnId(item) ?? ''
      ).toLowerCase()

      const supplierId =
        getReturnSupplierId(item)

      const grnId =
        getReturnGrnId(item)

      const supplierName = String(
        suppliersMap[String(supplierId ?? '')] ??
        item?.supplierName ??
        item?.supplier_name ??
        ''
      ).toLowerCase()

      const grnNumber = String(
        grnsMap[String(grnId ?? '')] ??
        item?.grnNumber ??
        item?.grn_number ??
        ''
      ).toLowerCase()

      const reason = String(
        item?.reason ?? ''
      ).toLowerCase()


      const matchesSearch =
        !query ||
        returnId.includes(query) ||
        supplierName.includes(query) ||
        grnNumber.includes(query) ||
        reason.includes(query)


      const matchesSupplier =
        !supplierFilter ||
        String(supplierId ?? '') ===
        String(supplierFilter)


      return (
        matchesSearch &&
        matchesSupplier
      )
    })
  }, [
    returns,
    suppliersMap,
    grnsMap,
    searchQuery,
    supplierFilter,
  ])

  // Reset page when search or supplier filter changes
  useEffect(() => {
    setCurrentPage(1)
  }, [searchQuery, supplierFilter])

  const totalPages = Math.max(1, Math.ceil(filteredReturns.length / pageSize))
  const safeCurrentPage = Math.min(Math.max(1, currentPage), totalPages)

  useEffect(() => {
    if (currentPage > totalPages) {
      setCurrentPage(totalPages)
    }
  }, [currentPage, totalPages])

  const visiblePages = useMemo(
    () => getVisiblePages(safeCurrentPage, totalPages),
    [safeCurrentPage, totalPages]
  )

  const paginatedReturns = useMemo(() => {
    const start = (safeCurrentPage - 1) * pageSize
    return filteredReturns.slice(start, start + pageSize)
  }, [filteredReturns, safeCurrentPage, pageSize])


  // =========================================================
  // DELETE
  // =========================================================

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
      await deletePurchaseReturn(
        deleteTargetId
      )

      setReturns((previous) =>
        previous.filter((item) => {
          const itemId =
            getReturnId(item)

          return (
            String(itemId) !==
            String(deleteTargetId)
          )
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
  // CREATE
  // =========================================================

  const handleCreate = () => {
    navigate(
      '/inventory/purchase-returns/create'
    )
  }


  // =========================================================
  // VIEW
  // =========================================================

  const handleView = (id) => {
    if (
      id === null ||
      id === undefined
    ) {
      return
    }

    navigate(
      `/inventory/purchase-returns/${id}`
    )
  }


  // =========================================================
  // EDIT
  // =========================================================

  const handleEdit = (id) => {
    if (
      id === null ||
      id === undefined
    ) {
      return
    }

    navigate(
      `/inventory/purchase-returns/edit/${id}`
    )
  }


  // =========================================================
  // DELETE
  // =========================================================

  const handleDelete = (id) => {
    if (
      id === null ||
      id === undefined
    ) {
      return
    }

    setDeleteTargetId(id)
  }


  // =========================================================
  // RESET
  // =========================================================

  const handleResetFilters = () => {
    setSearchQuery('')
    setSupplierFilter('')
  }


  // =========================================================
  // RENDER
  // =========================================================

  return (
    <main>
      <PageHeader
        title="Purchase Returns"
        subtitle="Manage and track goods returned to suppliers."
        primaryAction={{
          icon: Plus,
          label: 'Create Purchase Return',
          onClick: handleCreate,
        }}
      />


      {/* TOOLBAR */}

      <section className="purchase-returns-toolbar card">
        <div className="toolbar-search">
          <Search
            size={16}
            className="search-icon"
          />

          <input
            type="text"
            placeholder="Search by Return ID, Supplier, GRN or Reason"
            value={searchQuery}
            onChange={(event) =>
              setSearchQuery(
                event.target.value
              )
            }
          />

          {searchQuery && (
            <button
              type="button"
              className="clear-search-btn"
              onClick={() =>
                setSearchQuery('')
              }
            >
              Clear
            </button>
          )}
        </div>


        <div className="toolbar-filters">
          <div className="filter-group">
            <select
              id="supplier-select"
              value={supplierFilter}
              onChange={(event) =>
                setSupplierFilter(
                  event.target.value
                )
              }
              aria-label="Filter by supplier"
            >
              <option value="">
                Supplier
              </option>

              {suppliers.map((supplier) => {
                const supplierId =
                  getSupplierId(supplier)

                if (
                  supplierId === null ||
                  supplierId === undefined ||
                  supplierId === ''
                ) {
                  return null
                }

                return (
                  <option
                    key={String(supplierId)}
                    value={String(supplierId)}
                  >
                    {getSupplierName(supplier)}
                  </option>
                )
              })}
            </select>
          </div>


          {(searchQuery ||
            supplierFilter) && (
              <button
                className="erp-button erp-button--secondary"
                type="button"
                onClick={
                  handleResetFilters
                }
              >
                Reset Filters
              </button>
            )}
        </div>
      </section>


      {/* LOADING */}

      {loading && (
        <StateBlock
          state="loading"
          message="Loading purchase returns..."
        />
      )}


      {/* ERROR */}

      {!loading && error && (
        <div className="purchase-returns-error-card card">
          <AlertCircle
            size={24}
            className="error-icon"
          />

          <div>
            <h3>
              We could not load this workspace
            </h3>

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
      )}


      {/* EMPTY */}

      {!loading &&
        !error &&
        filteredReturns.length === 0 && (
          <div className="purchase-returns-empty card">
            <p className="empty-title">
              No purchase returns found
            </p>

            <p className="empty-subtitle">
              {searchQuery ||
                supplierFilter
                ? 'No returns match your filter criteria.'
                : 'Click "Create Purchase Return" to record your first return.'}
            </p>

            {!searchQuery &&
              !supplierFilter && (
                <button
                  className="erp-button erp-button--primary"
                  onClick={
                    handleCreate
                  }
                  type="button"
                >
                  <Plus size={14} />
                  Create Purchase Return
                </button>
              )}
          </div>
        )}


      {/* TABLE */}

      {!loading &&
        !error &&
        filteredReturns.length > 0 && (
          <section className="card purchase-returns-table-container">
            <div className="purchase-returns-table-viewport">
              <table className="purchase-returns-table">
                <thead>
                  <tr>
                    <th>Return ID</th>
                    <th>Supplier</th>
                    <th>GRN</th>
                    <th>Return Date</th>
                    <th className="text-right">
                      Total Amount
                    </th>
                    <th>Reason</th>
                    <th className="text-center actions-header">
                      Actions
                    </th>
                  </tr>
                </thead>

                <tbody>
                  {paginatedReturns.map((row) => {
                    const returnId =
                      getReturnId(row)

                    const supplierId =
                      getReturnSupplierId(row)

                    const grnId =
                      getReturnGrnId(row)

                    const supplierName =
                      suppliersMap[
                      String(supplierId ?? '')
                      ] ??
                      row?.supplierName ??
                      row?.supplier_name ??
                      (
                        supplierId
                          ? `Supplier #${supplierId}`
                          : '-'
                      )

                    const grnNumber =
                      grnsMap[
                      String(grnId ?? '')
                      ] ??
                      row?.grnNumber ??
                      row?.grn_number ??
                      (
                        grnId
                          ? `GRN-${grnId}`
                          : '-'
                      )

                    const returnDate =
                      getReturnDate(row)

                    const totalAmount =
                      getTotalAmount(row)

                    const reason =
                      row?.reason ?? ''


                    return (
                      <tr key={returnId}>
                        <td className="font-semibold text-primary">
                          {getReturnNumberDisplay(row)}
                        </td>

                        <td>
                          {supplierName}
                        </td>

                        <td>
                          {grnNumber}
                        </td>

                        <td>
                          {returnDate
                            ? formatDate(
                              returnDate
                            )
                            : '-'}
                        </td>

                        <td className="text-right font-semibold">
                          {formatCurrency(
                            totalAmount
                          )}
                        </td>

                        <td
                          className="reason-cell"
                          title={reason}
                        >
                          {reason
                            ? reason.length > 50
                              ? `${reason.slice(
                                0,
                                50
                              )}...`
                              : reason
                            : '-'}
                        </td>

                        <td className="text-center actions-cell">
                          <ActionMenu
                            iconOnly
                            label={`Actions for ${getReturnNumberDisplay(row) || returnId}`}
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
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>

            <Pagination className="table-component__pagination purchase-returns-pagination">
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
                    {[8, 10, 15, 20, 25].map((size) => (
                      <option key={size} value={size}>
                        {size}
                      </option>
                    ))}
                  </select>
                </label>
                <span className="table-component__status">
                  Showing {filteredReturns.length === 0 ? 0 : (safeCurrentPage - 1) * pageSize + 1}-
                  {Math.min(safeCurrentPage * pageSize, filteredReturns.length)} of {filteredReturns.length}
                </span>
              </div>

              <div className="table-component__page-controls">
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() => setCurrentPage(1)}
                  disabled={safeCurrentPage === 1}
                  aria-label="Go to first page"
                >
                  <ChevronsLeft size={16} />
                  First
                </button>
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() =>
                    setCurrentPage((currentValue) =>
                      Math.max(currentValue - 1, 1)
                    )
                  }
                  disabled={safeCurrentPage === 1}
                  aria-label="Go to previous page"
                >
                  <ChevronLeft size={16} />
                  Previous
                </button>
                {visiblePages.map((pageNumber) => (
                  <button
                    key={pageNumber}
                    type="button"
                    className={`table-component__page-number ${pageNumber === safeCurrentPage ? 'is-active' : ''}`.trim()}
                    onClick={() => setCurrentPage(pageNumber)}
                    aria-current={
                      pageNumber === safeCurrentPage ? 'page' : undefined
                    }
                    aria-label={`Go to page ${pageNumber}`}
                  >
                    {pageNumber}
                  </button>
                ))}
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() =>
                    setCurrentPage((currentValue) =>
                      Math.min(currentValue + 1, totalPages)
                    )
                  }
                  disabled={safeCurrentPage === totalPages}
                  aria-label="Go to next page"
                >
                  Next
                  <ChevronRight size={16} />
                </button>
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() => setCurrentPage(totalPages)}
                  disabled={safeCurrentPage === totalPages}
                  aria-label="Go to last page"
                >
                  Last
                  <ChevronsRight size={16} />
                </button>
              </div>
            </Pagination>
          </section>
        )}


      {/* DELETE MODAL */}

      {deleteTargetId !== null &&
        deleteTargetId !== undefined && (
          <FormModal
            isOpen={true}
            title="Delete Purchase Return?"
            onClose={() =>
              !deleting &&
              setDeleteTargetId(null)
            }
          >
            <div className="delete-confirm-content">
              <p>
                This action will permanently
                remove this purchase return
                and its associated items.
              </p>

              <p className="delete-warning">
                Return ID: #{deleteTargetId}
              </p>

              <div className="form-modal-actions">
                <button
                  className="erp-button erp-button--secondary"
                  type="button"
                  disabled={deleting}
                  onClick={() =>
                    setDeleteTargetId(null)
                  }
                >
                  Cancel
                </button>

                <button
                  className="erp-button erp-button--danger"
                  type="button"
                  disabled={deleting}
                  onClick={
                    handleDeleteConfirm
                  }
                >
                  {deleting
                    ? 'Deleting...'
                    : 'Delete'}
                </button>
              </div>
            </div>
          </FormModal>
        )}
    </main>
  )
}