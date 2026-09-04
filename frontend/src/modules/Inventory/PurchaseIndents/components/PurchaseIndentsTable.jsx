import { Ban, Check, Download, Eye, Mail, Pencil, Printer, RefreshCw, ShoppingCart, Trash2 } from 'lucide-react'
import { useMemo } from 'react'
import { ActionMenu, DataTable, FilterBar, StatusBadge } from '../../../../components/erp'
import { formatDate } from '../../../../utils/helpers'

const EMPTY_VALUE = 'Not Available'
const NOT_ASSIGNED = 'Not Assigned'
const DEFAULT_STATUS = 'Pending'
const DEFAULT_PRIORITY = 'Medium'

function getIndentNumber(indent) {
  return indent?.indentNumber || indent?.indentNo || indent?.purchaseIndentId || indent?.indentId || indent?.id || EMPTY_VALUE
}

function getStatusKind(status) {
  const normalized = String(status || DEFAULT_STATUS).toLowerCase()

  if (normalized.includes('converted') || normalized.includes('ordered')) {
    return 'converted'
  }

  if (normalized.includes('approved')) {
    return 'approved'
  }

  if (normalized.includes('rejected')) {
    return 'rejected'
  }

  return 'pending'
}

function getBadgeStatus(statusKind) {
  if (statusKind === 'approved') {
    return 'success'
  }

  if (statusKind === 'rejected') {
    return 'failed'
  }

  if (statusKind === 'converted') {
    return 'info'
  }

  return 'warning'
}

function getItemQuantity(item) {
  return Number(item?.requiredQty ?? item?.quantity ?? 0)
}

function getIndentQuantity(indent) {
  if (Array.isArray(indent?.items) && indent.items.length > 0) {
    return indent.items.reduce((sum, item) => sum + getItemQuantity(item), 0)
  }

  return Number(indent?.totalQuantity ?? indent?.requiredQty ?? indent?.quantity ?? 0)
}

function getProductId(item) {
  return item?.productId || item?.id
}

function getProductNameFromList(products, productId) {
  if (!productId) {
    return ''
  }

  const product = products.find((item) =>
    String(item?.productId ?? item?.id) === String(productId)
  )

  return product?.name || product?.productName || ''
}

function getItemProductName(item, products = []) {
  return (
    item?.productName ||
    item?.name ||
    getProductNameFromList(products, getProductId(item)) ||
    EMPTY_VALUE
  )
}

function getIndentProductsText(indent, products = []) {
  if (Array.isArray(indent?.items) && indent.items.length > 0) {
    return indent.items.map((item) => getItemProductName(item, products)).join('; ')
  }

  return (
    indent?.productName ||
    getProductNameFromList(products, indent?.productId) ||
    EMPTY_VALUE
  )
}

function getFirstProductSummary(indent, products = []) {
  if (Array.isArray(indent?.items) && indent.items.length > 0) {
    const firstProductName = getItemProductName(indent.items[0], products)
    return indent.items.length === 1
      ? firstProductName
      : `${firstProductName} (+${indent.items.length - 1} more)`
  }

  return getIndentProductsText(indent, products)
}

export default function PurchaseIndentsTable({
  indents,
  products,
  canDelete,
  onDelete,
  onView,
  onApprove,
  onReject,
  onConvert,
  onRefresh,
  loading,
  selectedIndentIds = [],
  onSelectionChange,
  onBulkExport,
  onBulkPrint,
  onBulkMail,
  onBulkDelete,
  canBulkDelete = true,
  onClearSelection,
  onEdit,
  onMail,
  onPdf,
  onPrint,
  busyAction = null,
}) {
  const safeIndents = Array.isArray(indents) ? indents : []
  const safeProducts = useMemo(() => (Array.isArray(products) ? products : []), [products])

  const columns = useMemo(() => [
    {
      key: 'indentNumber',
      label: 'Indent Number',
      sortable: true,
      mobilePrimary: true,
      mobileLabel: 'Purchase Indent',
      className: 'purchases-page__col-po-number',
      tableWidth: 180,
      style: { width: 180, minWidth: 180 },
      headerStyle: { width: 180, minWidth: 180 },
      searchValue: (indent) =>
        `${getIndentNumber(indent)} ${getIndentProductsText(indent, safeProducts)} ${indent?.requestedByDisplay || ''} ${indent?.supplierDisplay || ''} ${indent?.departmentDisplay || ''} ${indent?.status || DEFAULT_STATUS}`,
      render: (indent) => getIndentNumber(indent),
    },
    {
      key: 'productName',
      label: 'Product',
      sortable: true,
      tableWidth: 320,
      style: { width: 320, minWidth: 320 },
      headerStyle: { width: 320, minWidth: 320 },
      render: (indent) => {
        const text = getFirstProductSummary(indent, safeProducts)
        return (
          <span style={{ whiteSpace: 'normal', wordBreak: 'break-word', lineHeight: '1.4', display: 'inline-block' }}>
            {text}
          </span>
        )
      },
    },

    {
      key: 'priority',
      label: 'Priority',
      sortable: true,
      tableWidth: 120,
      style: { width: 120, minWidth: 120 },
      headerStyle: { width: 120, minWidth: 120 },
      render: (indent) => (
        <span className={`badge badge--priority-${String(indent?.priority || DEFAULT_PRIORITY).toLowerCase()}`}>
          {indent?.priority || DEFAULT_PRIORITY}
        </span>
      ),
    },
    {
      key: 'indentDate',
      label: 'Request Date',
      sortable: true,
      tableWidth: 140,
      style: { width: 140, minWidth: 140 },
      headerStyle: { width: 140, minWidth: 140 },
      render: (indent) => indent?.indentDate ? formatDate(indent.indentDate) : EMPTY_VALUE,
    },
    {
      key: 'quantity',
      label: 'Quantity',
      sortable: true,
      tableWidth: 110,
      style: { width: 110, minWidth: 110 },
      headerStyle: { width: 110, minWidth: 110 },
      render: (indent) => getIndentQuantity(indent) || EMPTY_VALUE
    },
    {
      key: 'status',
      label: 'Status',
      sortable: true,
      mobileStatus: true,
      tableWidth: 120,
      style: { width: 120, minWidth: 120 },
      headerStyle: { width: 120, minWidth: 120 },
      render: (indent) => {
        const rawStatus = indent?.status || DEFAULT_STATUS
        const badgeStatus = getBadgeStatus(getStatusKind(rawStatus))
        return (
          <StatusBadge status={badgeStatus}>
            {rawStatus}
          </StatusBadge>
        )
      },
    },
    {
      key: 'requestedByDisplay',
      label: 'Requested By',
      sortable: true,
      tableWidth: 140,
      style: { width: 140, minWidth: 140 },
      headerStyle: { width: 140, minWidth: 140 },
      render: (indent) => indent?.requestedByDisplay || NOT_ASSIGNED,
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
    render: (indent) => {
      const indentId = indent?.purchaseIndentId || indent?.indentId || indent?.id
      const statusKind = getStatusKind(indent?.status)
      const isPending = statusKind === 'pending'
      const isApproved = statusKind === 'approved'
      const isBusy = (actionKey) =>
        String(busyAction?.id || '') === String(indentId || '') &&
        busyAction?.key === actionKey

      return (
        <ActionMenu
          iconOnly
          label={`Actions for ${getIndentNumber(indent)}`}
          menuKey={indentId}
          className="purchases-page__row-actions"
          actions={[
          {
            key: 'view',
            label: 'View Details',
            icon: Eye,
            loading: isBusy('view'),
            onClick: () => onView?.(indent),
          },

          isPending && {
            key: 'approve',
            label: 'Approve',
            icon: Check,
            loading: isBusy('approve'),
            onClick: () => onApprove?.(indent),
          },

          isPending && {
            key: 'reject',
            label: 'Reject',
            icon: Ban,
            tone: 'danger',
            loading: isBusy('reject'),
            onClick: () => onReject?.(indent),
          },

          isApproved && {
            key: 'convert-po',
            label: 'Convert to Purchase Order',
            icon: ShoppingCart,
            loading: isBusy('convert-po'),
            onClick: () => onConvert?.(indent),
          },

          isPending && {
            key: 'edit',
            label: 'Edit',
            icon: Pencil,
            onClick: () => onEdit?.(indent),
          },
          {
            key: 'mail',
            label: isBusy('mail') ? 'Preparing email...' : 'Mail Copy',
            icon: Mail,
            loading: isBusy('mail'),
            onClick: () => onMail?.(indent),
          },
          {
            key: 'pdf',
            label: isBusy('pdf') ? 'Downloading...' : 'Download PDF',
            icon: Download,
            loading: isBusy('pdf'),
            onClick: () => onPdf?.(indent),
          },
          {
            key: 'print',
            label: isBusy('print') ? 'Preparing print...' : 'Print',
            icon: Printer,
            loading: isBusy('print'),
            onClick: () => onPrint?.(indent),
          },
          canDelete && isPending && {
            key: 'delete',
            label: 'Delete',
            icon: Trash2,
            tone: 'danger',
            onClick: () => onDelete(indent),
          },
        ].filter(Boolean)}
        />
      )
    },
    },
  ], [busyAction, canDelete, onApprove, onConvert, onDelete, onEdit, onMail, onPdf, onPrint, onReject, onView, safeProducts])

  const hasSelection = selectedIndentIds.length > 0

  const selectionToolbar = hasSelection ? (
    <FilterBar className="resource-center__product-style-selection-actions" ariaLabel="Selected purchase indents actions">
      <div className="resource-center__product-style-selection-summary" aria-live="polite">
        <Check size={15} />
        <strong>{selectedIndentIds.length} selected</strong>
      </div>
      <button
        type="button"
        className="button button-secondary resource-center__product-style-selection-button"
        onClick={onBulkExport}
      >
        <Download size={15} />
        Export
      </button>
      <button
        type="button"
        className="button button-secondary resource-center__product-style-selection-button"
        onClick={onBulkPrint}
      >
        <Printer size={15} />
        Print
      </button>
      <button
        type="button"
        className="button button-secondary resource-center__product-style-selection-button"
        onClick={onBulkMail}
      >
        <Mail size={15} />
        Mail
      </button>
      {canDelete ? (
        <button
        type="button"
        className="button button-secondary resource-center__product-style-selection-button resource-center__product-style-selection-button--danger"
        onClick={onBulkDelete}
        disabled={!canBulkDelete}
        title={canBulkDelete ? 'Delete selected pending indents' : 'Only pending indents can be deleted'}
      >
          <Trash2 size={15} />
          Delete
        </button>
      ) : null}
    </FilterBar>
  ) : (
    <FilterBar className="purchases-page__table-actions">
      <button
        type="button"
        className="button button-secondary"
        onClick={onRefresh}
        disabled={loading}
      >
        <RefreshCw size={16} className={loading ? 'animate-spin' : ''} />
        Refresh
      </button>
    </FilterBar>
  )

  return (
    <div className="card purchases-page__table-card">
      <DataTable
        className="purchases-page__table"
        rows={safeIndents}
        columns={columns}
        loading={loading}
        defaultPageSize={20}
        defaultSortKey=""
        splitToolbar
        toolbarContent={selectionToolbar}
        enableRowSelection={true}
        hideSelectionSummary={true}
        selectedRowKeys={selectedIndentIds}
        onSelectionChange={onSelectionChange}
        keyField="purchaseIndentId"
        showSearch={!hasSelection}
        showColumnControls={!hasSelection}
        columnStorageKey="ims.purchase-indents.visibleColumns.compact.v6"
        defaultVisibleColumnKeys={['indentNumber', 'productName', 'priority', 'indentDate', 'quantity', 'status', 'requestedByDisplay', 'actions']}
        fitExplicitColumnsToContainer={false}
        searchPlaceholder="Search indents by number or status"
        emptyMessage="No purchase indents found."
      />
    </div>
  )
}
