import { ArrowRightLeft, Check, Download, Eye, MoveRight, Pencil, Printer, Trash2 } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { ActionMenu, DataTable, FilterBar, StatusBadge, Modal } from '../../../components/erp'

const STATUS_OPTIONS = ['Active', 'Inactive']
const WAREHOUSE_COLUMNS_STORAGE_KEY = 'ims.warehouses.visibleColumns.v4'
const WAREHOUSE_DEFAULT_COLUMNS = [
  'name',
  'products',
  'location',
  'stockUnits',
  'status',
  'rackCount',
  'updatedAt',
  'actions',
]
const WAREHOUSE_COLUMN_WIDTHS = {
  name: 250,
  products: 200,
  location: 170,
  managerName: 150,
  contact: 160,
  stockUnits: 124,
  rackCount: 124,
  status: 96,
  updatedAt: 170,
  actions: 72,
}

function formatDate(value) {
  if (!value) {
    return '-'
  }

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return '-'
  }

  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  }).format(date)
}

function normalizeStatus(value) {
  return STATUS_OPTIONS.find((status) => status.toLowerCase() === String(value ?? '').toLowerCase()) ?? 'Active'
}

function escapeCsvValue(value) {
  const text = String(value ?? '')

  if (/[",\n\r]/.test(text)) {
    return `"${text.replace(/"/g, '""')}"`
  }

  return text
}

function escapeHtml(value) {
  return String(value ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;')
}

function exportWarehousesCsv(warehouses, productsCatalog = []) {
  const headers = [
    'Warehouse Name',
    'Warehouse Code',
    'Products',
    'Location',
    'Contact',
    'Capacity',
    'Stock Units',
    'Racks',
    'Bins',
    'Manager Name',
    'Status',
    'Last Updated',
  ]
  const rows = warehouses.map((warehouse) => {
    const grouped = getGroupedWarehouseProducts(warehouse.products, productsCatalog)
    const prodsSummary = grouped.length > 0
      ? grouped.map((p) => `${p.name}${p.sku ? ` [${p.sku}]` : ''} (${p.quantity}${p.unit ? ` ${p.unit}` : ''})`).join('; ')
      : 'No Products'

    const contactInfo = Array.from(new Set([
      warehouse.phone,
      warehouse.contactPhone,
      warehouse.email,
      warehouse.contactEmail,
      warehouse.contact,
    ].filter(Boolean))).join(' | ') || '-'

    return [
      warehouse.name || '',
      warehouse.warehouseCode || '',
      prodsSummary,
      warehouse.location || '',
      contactInfo,
      warehouse.capacity ? `${warehouse.capacity} sq ft` : '',
      warehouse.stockUnits ?? 0,
      warehouse.rackCount ?? 0,
      warehouse.binCount ?? 0,
      warehouse.managerName || '',
      normalizeStatus(warehouse.status),
      formatDate(warehouse.updatedAt || warehouse.createdAt),
    ]
  })
  const csv = [headers, ...rows]
    .map((row) => row.map(escapeCsvValue).join(','))
    .join('\n')
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = 'Warehouses.csv'
  link.click()
  URL.revokeObjectURL(url)
}

function printWarehouses(warehouses, productsCatalog = []) {
  if (!warehouses || warehouses.length === 0) return

  const rows = warehouses.map((warehouse) => {
    const grouped = getGroupedWarehouseProducts(warehouse.products, productsCatalog)
    const prodsSummary = grouped.length > 0
      ? grouped.map((p) => `
        <div class="product-item">
          <span class="product-name">${escapeHtml(p.name)}</span>${p.sku ? ` <span class="product-sku">[${escapeHtml(p.sku)}]</span>` : ''} <span class="product-qty">(${escapeHtml(String(p.quantity))}${p.unit ? ` ${escapeHtml(p.unit)}` : ''})</span>
        </div>
      `).join('')
      : '<span class="empty-cell">No Products</span>'

    const contactList = Array.from(new Set([
      warehouse.phone,
      warehouse.contactPhone,
      warehouse.email,
      warehouse.contactEmail,
      warehouse.contact,
    ].filter(Boolean)))

    const contactHtml = contactList.length > 0
      ? contactList.map((item) => `<div class="contact-line">${escapeHtml(item)}</div>`).join('')
      : '<span class="empty-cell">-</span>'

    const managerDisplay = warehouse.managerName
      ? `<span class="manager-name">${escapeHtml(warehouse.managerName)}</span>`
      : '<span class="empty-cell">Not assigned</span>'

    const rawStatus = normalizeStatus(warehouse.status)
    const statusLabel = rawStatus.toLowerCase() === 'active' ? 'Active' : 'Inactive'
    const statusClass = rawStatus.toLowerCase() === 'active' ? 'status-active' : 'status-inactive'
    const statusBadge = `<span class="status-pill ${statusClass}">${escapeHtml(statusLabel)}</span>`

    const capacityDisplay = warehouse.capacity
      ? `${escapeHtml(String(warehouse.capacity))} sq ft`
      : '<span class="empty-cell">Not set</span>'

    const stockUnitsDisplay = escapeHtml(Number(warehouse.stockUnits || 0).toLocaleString('en-IN'))
    const rackBinDisplay = `${escapeHtml(String(Number(warehouse.rackCount || 0)))} / ${escapeHtml(String(Number(warehouse.binCount || 0)))}`
    const lastUpdatedDisplay = escapeHtml(formatDate(warehouse.updatedAt || warehouse.createdAt))

    return `
      <tr>
        <td class="col-warehouse">
          <strong class="warehouse-name">${escapeHtml(warehouse.name || 'Unnamed warehouse')}</strong>
          ${warehouse.warehouseCode ? `<span class="warehouse-code">${escapeHtml(warehouse.warehouseCode)}</span>` : ''}
        </td>
        <td class="col-products">${prodsSummary}</td>
        <td class="col-location">${escapeHtml(warehouse.location || 'Location not set')}</td>
        <td class="col-contact">${contactHtml}</td>
        <td class="col-capacity">${capacityDisplay}</td>
        <td class="col-stock">${stockUnitsDisplay}</td>
        <td class="col-rackbin">${rackBinDisplay}</td>
        <td class="col-manager">${managerDisplay}</td>
        <td class="col-status">${statusBadge}</td>
        <td class="col-updated">${lastUpdatedDisplay}</td>
      </tr>
    `
  }).join('')

  const printWindow = window.open('', '_blank')

  if (!printWindow) {
    return
  }

  printWindow.document.write(`<!doctype html><html><head><title>Warehouses Report</title><style>
    @page {
      size: landscape;
      margin: 10mm 12mm;
    }
    * {
      box-sizing: border-box;
    }
    body {
      margin: 0;
      padding: 16px 20px;
      color: #0f172a;
      background: #ffffff;
      font: 11.5px/1.45 -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
      -webkit-print-color-adjust: exact;
      print-color-adjust: exact;
    }
    .header {
      margin-bottom: 14px;
      padding-bottom: 10px;
      border-bottom: 2px solid #cbd5e1;
      display: flex;
      justify-content: space-between;
      align-items: flex-end;
    }
    h1 {
      margin: 0 0 4px;
      font-size: 18px;
      font-weight: 700;
      color: #0f172a;
      letter-spacing: -0.01em;
    }
    p.meta {
      margin: 0;
      color: #64748b;
      font-size: 11px;
    }
    table {
      width: 100%;
      border-collapse: collapse;
      table-layout: fixed;
      margin-top: 8px;
    }
    th, td {
      padding: 7px 8px;
      border: 1px solid #cbd5e1;
      font-size: 11px;
      line-height: 1.4;
      word-break: normal;
      overflow-wrap: break-word;
    }
    th {
      background: #f1f5f9;
      color: #1e293b;
      font-size: 10px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      white-space: nowrap;
      vertical-align: middle;
    }
    td {
      vertical-align: top;
      background: #ffffff;
    }
    tbody tr:nth-child(even) td {
      background-color: #f8fafc;
    }
    tr {
      page-break-inside: avoid;
    }

    /* Column Widths and Alignments */
    .col-warehouse { width: 13%; text-align: left; }
    .col-products  { width: 23%; text-align: left; }
    .col-location  { width: 10%; text-align: left; white-space: nowrap; }
    .col-contact   { width: 14%; text-align: left; }
    .col-capacity  { width: 7%; text-align: right; white-space: nowrap; }
    .col-stock     { width: 7%; text-align: right; white-space: nowrap; font-variant-numeric: tabular-nums; }
    .col-rackbin   { width: 6%; text-align: center; white-space: nowrap; }
    .col-manager   { width: 8%; text-align: left; white-space: nowrap; }
    .col-status    { width: 6%; text-align: center; white-space: nowrap; }
    .col-updated   { width: 6%; text-align: center; white-space: nowrap; }

    /* Content Typography & Elements */
    .warehouse-name {
      display: block;
      font-weight: 650;
      color: #0f172a;
      line-height: 1.3;
    }
    .warehouse-code {
      display: block;
      color: #64748b;
      font-size: 10px;
      font-weight: 500;
      margin-top: 2px;
    }
    .product-item {
      margin-bottom: 3px;
      line-height: 1.35;
    }
    .product-item:last-child {
      margin-bottom: 0;
    }
    .product-name {
      font-weight: 500;
      color: #1e293b;
    }
    .product-sku {
      color: #64748b;
      font-size: 10px;
      margin-left: 2px;
    }
    .product-qty {
      color: #059669;
      font-weight: 600;
      font-size: 10.5px;
      white-space: nowrap;
      margin-left: 3px;
    }
    .contact-line {
      font-size: 10.5px;
      color: #334155;
      line-height: 1.35;
    }
    .manager-name {
      font-weight: 600;
      color: #0f172a;
    }
    .empty-cell {
      color: #94a3b8;
      font-style: italic;
    }
    .status-pill {
      display: inline-block;
      padding: 2px 7px;
      border-radius: 999px;
      font-size: 9.5px;
      font-weight: 600;
      line-height: 1.3;
      white-space: nowrap;
      text-transform: capitalize;
    }
    .status-active {
      background: #ecfdf5;
      color: #065f46;
      border: 1px solid #a7f3d0;
    }
    .status-inactive {
      background: #fef2f2;
      color: #991b1b;
      border: 1px solid #fecaca;
    }
  </style></head><body>
    <div class="header">
      <div class="header-left">
        <h1>Warehouses Report</h1>
        <p class="meta">Printed on ${new Date().toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })} • Total Records: ${warehouses.length}</p>
      </div>
    </div>
    <table>
      <thead>
        <tr>
          <th class="col-warehouse">Warehouse</th>
          <th class="col-products">Products</th>
          <th class="col-location">Location</th>
          <th class="col-contact">Contact</th>
          <th class="col-capacity">Capacity</th>
          <th class="col-stock">Stock Units</th>
          <th class="col-rackbin">Rack / Bin</th>
          <th class="col-manager">Manager</th>
          <th class="col-status">Status</th>
          <th class="col-updated">Last Updated</th>
        </tr>
      </thead>
      <tbody>${rows}</tbody>
    </table>
  </body></html>`)
  printWindow.document.close()
  printWindow.focus()
  setTimeout(() => {
    printWindow.print()
  }, 250)
}

function getGroupedWarehouseProducts(warehouseProducts, productsCatalog) {
  const map = {}
  
  // Filter products with quantity > 0
  const activeProducts = (warehouseProducts || []).filter((item) => Number(item.quantity || 0) > 0)
  
  activeProducts.forEach((item) => {
    const name = item.productName || ''
    if (!name) return
    
    const key = name.trim().toLowerCase()
    if (!map[key]) {
      // Look up in productsCatalog
      const catalogProd = (productsCatalog || []).find(
        (p) => String(p.name).trim().toLowerCase() === key
      )
      
      map[key] = {
        name,
        sku: item.sku || item.Sku || item.SKU || catalogProd?.sku || '',
        quantity: 0,
        unit: item.unit || item.Unit || catalogProd?.unit || '',
      }
    }
    
    map[key].quantity += Number(item.quantity || 0)
  })
  
  return Object.values(map)
}

function formatWarehouseProductTooltip(prod) {
  if (!prod) return ''
  const name = prod.name?.trim() || 'Unnamed Product'
  const sku = (prod.sku || prod.Sku || prod.SKU || '').trim() || 'N/A'
  const qty = prod.quantity !== undefined && prod.quantity !== null && prod.quantity !== ''
    ? Number(prod.quantity).toLocaleString('en-IN')
    : '0'
  const unit = (prod.unit || prod.Unit || '').trim() || 'Units'

  return `${name}\nSKU: ${sku}\nAvailable: ${qty} ${unit}`
}

export default function WarehousesTable({
  warehouses,
  products = [],
  loading = false,
  canEdit,
  canDelete,
  statusSavingId = '',
  onViewDetails,
  onEdit,
  onDelete,
  onStatusChange,
  onTransfer,
  onBinTransfer,
  emptyMessage = 'No warehouses available.',
}) {
  const [openStatusMenuId, setOpenStatusMenuId] = useState(null)
  const [selectedWarehouseIds, setSelectedWarehouseIds] = useState([])
  const [activeProductsModalWarehouse, setActiveProductsModalWarehouse] = useState(null)

  const selectedWarehouses = useMemo(() => {
    const selectedIdSet = new Set(selectedWarehouseIds.map(String))
    return warehouses.filter((warehouse) => selectedIdSet.has(String(warehouse.id || '')))
  }, [selectedWarehouseIds, warehouses])
  const hasSelectedWarehouses = selectedWarehouses.length > 0

  function clearSelection() {
    setSelectedWarehouseIds([])
  }

  async function handleBulkDelete() {
    if (!canDelete || selectedWarehouses.length === 0) {
      return
    }

    for (const warehouse of selectedWarehouses) {
      await onDelete?.(warehouse.id)
    }
    clearSelection()
  }

  useEffect(() => {
    function handlePointerDown(event) {
      if (!event.target.closest?.('[data-warehouse-status-menu="true"]')) {
        setOpenStatusMenuId(null)
      }
    }

    document.addEventListener('pointerdown', handlePointerDown)
    return () => document.removeEventListener('pointerdown', handlePointerDown)
  }, [])

  useEffect(() => {
    const visibleIds = new Set(warehouses.map((warehouse) => String(warehouse.id || '')))
    setSelectedWarehouseIds((currentValue) => currentValue.filter((id) => visibleIds.has(String(id))))
  }, [warehouses])

  const columns = useMemo(() => [
    {
      key: 'name',
      label: 'Warehouse Name',
      className: 'warehouses-col-name',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.name,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.name, minWidth: WAREHOUSE_COLUMN_WIDTHS.name, maxWidth: WAREHOUSE_COLUMN_WIDTHS.name },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.name, minWidth: WAREHOUSE_COLUMN_WIDTHS.name, maxWidth: WAREHOUSE_COLUMN_WIDTHS.name },
      sortable: true,
      mobilePrimary: true,
      mobileLabel: 'Warehouse',
      searchValue: (warehouse) =>
        [
          warehouse.name,
          warehouse.warehouseCode,
          warehouse.location,
          warehouse.stockUnits,
          warehouse.status,
        ].join(' '),
      render: (warehouse) => (
        <div className="warehouses-page__table-stack">
          <strong title={warehouse.name || 'Unnamed warehouse'}>
            {warehouse.name || 'Unnamed warehouse'}
          </strong>
          <span title={warehouse.warehouseCode || 'Code not set'}>
            {warehouse.warehouseCode || 'Code not set'}
          </span>
        </div>
      ),
    },
    {
      key: 'products',
      label: 'Products',
      className: 'warehouses-col-products',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.products,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.products, minWidth: WAREHOUSE_COLUMN_WIDTHS.products, maxWidth: WAREHOUSE_COLUMN_WIDTHS.products },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.products, minWidth: WAREHOUSE_COLUMN_WIDTHS.products, maxWidth: WAREHOUSE_COLUMN_WIDTHS.products },
      sortable: false,
      render: (warehouse) => {
        const grouped = getGroupedWarehouseProducts(warehouse.products, products)

        if (grouped.length === 0) {
          return <span className="warehouses-page__cell-text text-muted">No Products</span>
        }

        if (grouped.length <= 3) {
          return (
            <div className="warehouses-products-list">
              {grouped.map((prod, index) => (
                <div
                  key={index}
                  className="warehouses-product-item"
                  data-product-tooltip="true"
                  data-tooltip-title={formatWarehouseProductTooltip(prod)}
                >
                  {prod.name}
                </div>
              ))}
            </div>
          )
        }

        const firstTwo = grouped.slice(0, 2)
        const moreCount = grouped.length - 2

        return (
          <div className="warehouses-products-list">
            {firstTwo.map((prod, index) => (
              <div
                key={index}
                className="warehouses-product-item"
                data-product-tooltip="true"
                data-tooltip-title={formatWarehouseProductTooltip(prod)}
              >
                {prod.name}
              </div>
            ))}
            <button
              type="button"
              className="warehouses-more-link"
              onClick={(event) => {
                event.stopPropagation()
                setActiveProductsModalWarehouse(warehouse)
              }}
              data-tooltip-title={`Click to view all ${grouped.length} products in ${warehouse.name}`}
            >
              +{moreCount} more product{moreCount > 1 ? 's' : ''}
            </button>
          </div>
        )
      },
    },
    {
      key: 'location',
      label: 'Location',
      className: 'warehouses-col-location',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.location,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.location, minWidth: WAREHOUSE_COLUMN_WIDTHS.location, maxWidth: WAREHOUSE_COLUMN_WIDTHS.location },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.location, minWidth: WAREHOUSE_COLUMN_WIDTHS.location, maxWidth: WAREHOUSE_COLUMN_WIDTHS.location },
      sortable: true,
      render: (warehouse) => (
        <span className="warehouses-page__cell-text" title={warehouse.location || 'Location not set'}>
          {warehouse.location || 'Location not set'}
        </span>
      ),
    },
    {
      key: 'managerName',
      label: 'Manager',
      className: 'warehouses-col-manager',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.managerName,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.managerName, minWidth: WAREHOUSE_COLUMN_WIDTHS.managerName, maxWidth: WAREHOUSE_COLUMN_WIDTHS.managerName },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.managerName, minWidth: WAREHOUSE_COLUMN_WIDTHS.managerName, maxWidth: WAREHOUSE_COLUMN_WIDTHS.managerName },
      sortable: true,
      render: (warehouse) => (
        <span className="warehouses-page__cell-text" title={warehouse.managerName || 'Not assigned'}>
          {warehouse.managerName || 'Not assigned'}
        </span>
      ),
    },
    {
      key: 'contact',
      label: 'Contact',
      className: 'warehouses-col-contact',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.contact,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.contact, minWidth: WAREHOUSE_COLUMN_WIDTHS.contact, maxWidth: WAREHOUSE_COLUMN_WIDTHS.contact },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.contact, minWidth: WAREHOUSE_COLUMN_WIDTHS.contact, maxWidth: WAREHOUSE_COLUMN_WIDTHS.contact },
      sortable: true,
      searchValue: (warehouse) => [warehouse.phone, warehouse.email].join(' '),
      render: (warehouse) => {
        const contact = warehouse.phone || warehouse.email || '-'

        return (
          <span className="warehouses-page__cell-text" title={contact}>
            {contact}
          </span>
        )
      },
    },
    {
      key: 'stockUnits',
      label: 'Stock Units',
      className: 'warehouses-col-stock',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.stockUnits,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.stockUnits, minWidth: WAREHOUSE_COLUMN_WIDTHS.stockUnits, maxWidth: WAREHOUSE_COLUMN_WIDTHS.stockUnits },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.stockUnits, minWidth: WAREHOUSE_COLUMN_WIDTHS.stockUnits, maxWidth: WAREHOUSE_COLUMN_WIDTHS.stockUnits },
      sortable: true,
      sortValue: (warehouse) => Number(warehouse.stockUnits || 0),
      render: (warehouse) => (
        <span className="warehouses-page__numeric-cell">
          {Number(warehouse.stockUnits || 0).toLocaleString('en-IN')}
        </span>
      ),
    },
    {
      key: 'status',
      label: 'Status',
      className: 'warehouses-col-status',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.status,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.status, minWidth: WAREHOUSE_COLUMN_WIDTHS.status, maxWidth: WAREHOUSE_COLUMN_WIDTHS.status },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.status, minWidth: WAREHOUSE_COLUMN_WIDTHS.status, maxWidth: WAREHOUSE_COLUMN_WIDTHS.status },
      mobileStatus: true,
      sortable: true,
      render: (warehouse) => {
        const currentStatus = normalizeStatus(warehouse.status)
        const isMenuOpen = openStatusMenuId === warehouse.id
        const isSaving = String(statusSavingId) === String(warehouse.id)

        return (
          <div className="warehouses-status-menu" data-warehouse-status-menu="true">
            <StatusBadge
              status={isSaving ? 'Saving...' : currentStatus}
              disabled={!onStatusChange || isSaving}
              onClick={onStatusChange ? (event) => {
                event.stopPropagation()
                if (isSaving) return
                setOpenStatusMenuId((currentValue) => (
                  currentValue === warehouse.id ? null : warehouse.id
                ))
              } : undefined}
              onDoubleClick={onStatusChange ? (event) => {
                event.stopPropagation()
                if (isSaving) return
                setOpenStatusMenuId(warehouse.id)
              } : undefined}
              onKeyDown={onStatusChange ? (event) => {
                event.stopPropagation()

                if ((event.key === 'Enter' || event.key === ' ') && !isSaving) {
                  event.preventDefault()
                  setOpenStatusMenuId((currentValue) => (
                    currentValue === warehouse.id ? null : warehouse.id
                  ))
                }
              } : undefined}
              title={`Update status for ${warehouse.name || 'warehouse'}`}
            />

            {isMenuOpen ? (
              <div
                className="warehouses-status-menu__popover"
                role="menu"
                aria-label={`Update status for ${warehouse.name || 'warehouse'}`}
              >
                {STATUS_OPTIONS.map((status) => {
                  const isCurrent = currentStatus === status

                  return (
                    <button
                      key={status}
                      type="button"
                      className="warehouses-status-menu__option"
                      role="menuitem"
                      disabled={isCurrent || isSaving}
                      onClick={(event) => {
                        event.stopPropagation()
                        if (isCurrent || isSaving) return

                        setOpenStatusMenuId(null)
                        onStatusChange(warehouse, status)
                      }}
                    >
                      <StatusBadge status={status} />
                      {isCurrent ? (
                        <span className="warehouses-status-menu__current">
                          <Check size={13} />
                          Current
                        </span>
                      ) : null}
                    </button>
                  )
                })}
              </div>
            ) : null}
          </div>
        )
      },
    },
    {
      key: 'rackCount',
      label: 'Rack / Bin',
      className: 'warehouses-col-rack',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.rackCount,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.rackCount, minWidth: WAREHOUSE_COLUMN_WIDTHS.rackCount, maxWidth: WAREHOUSE_COLUMN_WIDTHS.rackCount },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.rackCount, minWidth: WAREHOUSE_COLUMN_WIDTHS.rackCount, maxWidth: WAREHOUSE_COLUMN_WIDTHS.rackCount },
      sortable: true,
      sortValue: (warehouse) => Number(warehouse.rackCount || 0) + Number(warehouse.binCount || 0),
      render: (warehouse) => {
        const rackCount = Number(warehouse.rackCount || 0)
        const binCount = Number(warehouse.binCount || 0)
        const hasStorage = rackCount > 0 || binCount > 0

        return (
          <span className={`warehouses-page__rack-bin ${hasStorage ? 'is-active' : 'is-empty'}`}>
            {rackCount}
            <span>/</span>
            {binCount}
          </span>
        )
      },
    },
    {
      key: 'updatedAt',
      label: 'Last Updated',
      className: 'warehouses-col-date',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.updatedAt,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.updatedAt, minWidth: WAREHOUSE_COLUMN_WIDTHS.updatedAt, maxWidth: WAREHOUSE_COLUMN_WIDTHS.updatedAt },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.updatedAt, minWidth: WAREHOUSE_COLUMN_WIDTHS.updatedAt, maxWidth: WAREHOUSE_COLUMN_WIDTHS.updatedAt },
      sortable: true,
      sortValue: (warehouse) => new Date(warehouse.updatedAt || warehouse.createdAt || 0).getTime() || 0,
      render: (warehouse) => formatDate(warehouse.updatedAt || warehouse.createdAt),
    },
    {
      key: 'actions',
      label: 'Actions',
      className: 'warehouses-col-actions',
      tableWidth: WAREHOUSE_COLUMN_WIDTHS.actions,
      style: { width: WAREHOUSE_COLUMN_WIDTHS.actions, minWidth: WAREHOUSE_COLUMN_WIDTHS.actions, maxWidth: WAREHOUSE_COLUMN_WIDTHS.actions },
      headerStyle: { width: WAREHOUSE_COLUMN_WIDTHS.actions, minWidth: WAREHOUSE_COLUMN_WIDTHS.actions, maxWidth: WAREHOUSE_COLUMN_WIDTHS.actions },
      searchable: false,
      render: (warehouse) => (
        <ActionMenu
          iconOnly
          label={`Actions for ${warehouse.name || 'warehouse'}`}
          className="warehouses-page__row-actions"
          actions={[
            onViewDetails ? {
              key: 'details',
              label: 'Details',
              icon: Eye,
              onClick: () => onViewDetails(warehouse),
            } : null,
            canEdit ? {
              key: 'edit',
              label: 'Edit',
              icon: Pencil,
              onClick: () => onEdit(warehouse),
            } : null,
            canDelete ? {
              key: 'delete',
              label: 'Delete',
              icon: Trash2,
              tone: 'danger',
              onClick: () => onDelete(warehouse.id),
            } : null,
          ]}
        />
      ),
    },
  ], [canDelete, canEdit, onDelete, onEdit, onStatusChange, onViewDetails, openStatusMenuId, statusSavingId, products])

  const selectedToolbarContent = hasSelectedWarehouses ? (
    <FilterBar className="warehouses-table__selection-actions" ariaLabel="Selected warehouse actions">
      <div className="warehouses-selection-summary" aria-live="polite">
        <Check size={15} />
        <strong>{selectedWarehouses.length} selected</strong>
      </div>
      <button
        type="button"
        className="button button-secondary warehouses-table__selection-button"
        onClick={() => exportWarehousesCsv(selectedWarehouses, products)}
      >
        <Download size={15} />
        Export
      </button>
      <button
        type="button"
        className="button button-secondary warehouses-table__selection-button"
        onClick={() => printWarehouses(selectedWarehouses, products)}
      >
        <Printer size={15} />
        Print
      </button>
      {canDelete ? (
        <button
          type="button"
          className="button button-secondary warehouses-table__selection-button warehouses-table__selection-button--danger"
          onClick={handleBulkDelete}
        >
          <Trash2 size={15} />
          Delete
        </button>
      ) : null}
    </FilterBar>
  ) : null

  const toolbarContent = !hasSelectedWarehouses ? (
    <div className="warehouses-page__toolbar-actions">
      <button type="button" className="button button-secondary" onClick={onTransfer}>
        <MoveRight size={15} />
        Transfer Stock
      </button>
      <button type="button" className="button button-secondary" onClick={onBinTransfer}>
        <ArrowRightLeft size={15} />
        Bin Transfer
      </button>
    </div>
  ) : null

  return (
    <div className="card warehouses-page__table-card">
      <DataTable
        className="warehouses-data-table--compact"
        rows={warehouses}
        columns={columns}
        loading={loading}
        searchKeys={['name', 'warehouseCode', 'location', 'managerName', 'phone', 'email', 'status']}
        searchPlaceholder="Search warehouses"
        emptyMessage={emptyMessage}
        defaultPageSize={20}
        defaultSortKey=""
        showSearch={true}
        hideSelectionSummary={hasSelectedWarehouses}
        fitExplicitColumnsToContainer
        splitToolbar
        filterContent={selectedToolbarContent}
        columnStorageKey={WAREHOUSE_COLUMNS_STORAGE_KEY}
        defaultVisibleColumnKeys={WAREHOUSE_DEFAULT_COLUMNS}
        toolbarContent={toolbarContent}
        enableRowSelection
        selectedRowKeys={selectedWarehouseIds}
        onSelectionChange={setSelectedWarehouseIds}
        keyField="id"
      />

      {activeProductsModalWarehouse ? (
        <Modal
          title={`Products in ${activeProductsModalWarehouse.name}`}
          subtitle="Complete list of products currently available in this warehouse."
          onClose={() => setActiveProductsModalWarehouse(null)}
        >
          <div className="warehouses-products-modal-content">
            <table className="warehouses-products-modal-table">
              <thead>
                <tr>
                  <th>Product Name</th>
                  <th>SKU</th>
                  <th style={{ textAlign: 'right' }}>Available Quantity</th>
                  <th>Unit</th>
                </tr>
              </thead>
              <tbody>
                {getGroupedWarehouseProducts(activeProductsModalWarehouse.products, products).map((prod, index) => (
                  <tr key={index}>
                    <td><strong>{prod.name}</strong></td>
                    <td>{prod.sku || '-'}</td>
                    <td style={{ textAlign: 'right' }}>
                      <strong>{Number(prod.quantity).toLocaleString('en-IN')}</strong>
                    </td>
                    <td>{prod.unit || '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Modal>
      ) : null}
    </div>
  )
}
