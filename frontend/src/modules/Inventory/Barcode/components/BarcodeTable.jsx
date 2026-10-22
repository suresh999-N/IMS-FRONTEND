import { useMemo, useState } from 'react'
import { Barcode as BarcodeIcon, Printer, QrCode, RefreshCw, ScanLine } from 'lucide-react'
import CodePreview from './CodePreview'
import { DataTable, ActionMenu, FilterBar } from '../../../../components/erp'
import { formatDate } from '../../../../utils/helpers'
import { getBarcodeBars, getQrCells } from '../utils/preview'

function escapeHtml(value) {
  return String(value ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;')
}

function handlePrintBarcode(input) {
  if (!input) return
  const items = Array.isArray(input) ? input : [input]
  if (items.length === 0) return

  const printWindow = window.open('', '_blank')
  if (!printWindow) return

  const primaryItem = items[0]
  const title = items.length === 1
    ? `Print Code - ${escapeHtml(primaryItem.productName || 'Barcode')}`
    : `Print Codes (${items.length} items)`

  const cardsHtml = items.map((item) => {
    const isQr = item.codeType === 'QR Code'
    const itemValue =
      item.code ||
      item.value ||
      item.codeValue ||
      item.barcode ||
      item.sku ||
      (item.productId ? `PRD-${item.productId}` : '')
    let visualHtml = ''
    if (isQr) {
      const cells = getQrCells(itemValue)
      visualHtml = `
        <div class="code-box qr-box">
          <div class="qr-grid">
            ${cells.map(c => `<span class="qr-cell ${c.dark ? 'is-dark' : ''}"></span>`).join('')}
          </div>
        </div>
      `
    } else {
      const bars = getBarcodeBars(itemValue)
      visualHtml = `
        <div class="code-box barcode-box">
          <div class="barcode-bars">
            ${bars.map(b => `<span class="barcode-bar" style="width: ${Math.max(2, Math.round(b.width * 2.2))}px; height: ${Math.max(20, Math.round(b.height * 1.15))}px;"></span>`).join('')}
          </div>
        </div>
      `
    }

    return `
      <div class="label-card">
        <div class="label-header">
          <h2 class="product-title">${escapeHtml(item.productName || 'Product')}</h2>
          <span class="code-type-tag">${escapeHtml(item.codeType || 'Barcode')}</span>
        </div>
        <div class="visual-wrapper">
          ${visualHtml}
          ${itemValue ? `<div class="code-value">${escapeHtml(itemValue)}</div>` : ''}
        </div>
        <div class="label-footer">
          <span>Date: ${escapeHtml(item.date ? formatDate(item.date) : '')}</span>
          <span>IMS Inventory</span>
        </div>
      </div>
    `
  }).join('\n')

  printWindow.document.write(`<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <title>${escapeHtml(title)}</title>
  <style>
    * { box-sizing: border-box; }
    body {
      margin: 0;
      padding: 40px 20px;
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
      background: #ffffff;
      color: #0f172a;
      -webkit-print-color-adjust: exact !important;
      print-color-adjust: exact !important;
    }
    .labels-container {
      display: flex;
      flex-direction: column;
      gap: 32px;
      align-items: center;
    }
    .label-card {
      width: 100%;
      max-width: 360px;
      margin: 0 auto;
      border: 2px solid #0f172a;
      border-radius: 14px;
      padding: 24px;
      text-align: center;
      background: #ffffff;
      box-shadow: 0 4px 12px rgba(0,0,0,0.05);
    }
    .label-header {
      border-bottom: 1.5px solid #e2e8f0;
      padding-bottom: 14px;
      margin-bottom: 18px;
    }
    .product-title {
      font-size: 19px;
      font-weight: 800;
      color: #0f172a;
      margin: 0 0 10px;
      letter-spacing: -0.01em;
      line-height: 1.35;
    }
    .code-type-tag {
      display: inline-block;
      font-size: 11px;
      font-weight: 750;
      text-transform: uppercase;
      letter-spacing: 0.08em;
      color: #047857;
      background: #ecfdf5;
      padding: 4px 12px;
      border-radius: 9999px;
      border: 1px solid #a7f3d0;
    }
    .visual-wrapper {
      margin: 18px 0;
    }
    .code-box {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      background: #ffffff;
      border: 1px solid #cbd5e1;
      border-radius: 8px;
      margin: 0 auto;
      max-width: 100%;
      box-sizing: border-box;
      overflow: hidden;
    }
    .barcode-box {
      padding: 14px 20px;
    }
    .barcode-bars {
      display: flex;
      align-items: flex-end;
      justify-content: center;
      gap: 2px;
      box-sizing: border-box;
    }
    .barcode-bar {
      background-color: #000000;
      display: inline-block;
      border-radius: 0.5px;
    }
    .qr-box {
      padding: 12px;
    }
    .qr-grid {
      display: grid;
      grid-template-columns: repeat(21, 8px);
      gap: 1px;
      justify-content: center;
    }
    .qr-cell {
      width: 8px;
      height: 8px;
      background-color: #ffffff;
      display: block;
    }
    .qr-cell.is-dark {
      background-color: #000000;
    }
    .code-value {
      font-family: SFMono-Regular, Consolas, "Liberation Mono", Menlo, monospace;
      font-size: 15px;
      font-weight: 750;
      letter-spacing: 0.12em;
      color: #0f172a;
      margin-top: 12px;
      word-break: break-all;
    }
    .label-footer {
      border-top: 1.5px solid #e2e8f0;
      padding-top: 12px;
      margin-top: 18px;
      font-size: 11px;
      color: #64748b;
      display: flex;
      justify-content: space-between;
      font-weight: 600;
    }
    @media print {
      body { padding: 10mm; }
      .label-card {
        border-color: #000000;
        box-shadow: none;
        page-break-inside: avoid;
        break-inside: avoid;
      }
      .label-card:not(:last-child) {
        page-break-after: always;
        break-after: page;
        margin-bottom: 0;
      }
    }
  </style>
</head>
<body>
  <div class="labels-container">
    ${cardsHtml}
  </div>
</body>
</html>`)

  printWindow.document.close()
  printWindow.focus()
  printWindow.print()
}

export default function BarcodeTable({ barcodes = [], onRefresh, isLoading }) {
  const [selectedBarcodeIds, setSelectedBarcodeIds] = useState([])
  const [activeTab, setActiveTab] = useState('all')

  const barcodeCount = useMemo(
    () => barcodes.filter((b) => String(b.codeType || '').toLowerCase() !== 'qr code').length,
    [barcodes]
  )
  const qrCount = useMemo(
    () => barcodes.filter((b) => String(b.codeType || '').toLowerCase() === 'qr code').length,
    [barcodes]
  )
  const allCount = barcodes.length

  const filteredBarcodes = useMemo(() => {
    if (activeTab === 'barcode') {
      return barcodes.filter((b) => String(b.codeType || '').toLowerCase() !== 'qr code')
    }
    if (activeTab === 'qr') {
      return barcodes.filter((b) => String(b.codeType || '').toLowerCase() === 'qr code')
    }
    return barcodes
  }, [barcodes, activeTab])

  const selectedBarcodes = useMemo(() => {
    const selectedSet = new Set(selectedBarcodeIds.map(String))
    return filteredBarcodes.filter((item) => selectedSet.has(String(item.id)))
  }, [filteredBarcodes, selectedBarcodeIds])

  const toolbarContent = (
    <FilterBar className="barcode-page__toolbar-actions" ariaLabel="Barcode table refresh actions">
      {selectedBarcodes.length > 0 ? (
        <button
          type="button"
          className="button button-secondary"
          onClick={() => {
            handlePrintBarcode(selectedBarcodes)
          }}
        >
          <Printer size={16} />
          Print Selected ({selectedBarcodes.length})
        </button>
      ) : null}
      <button
        type="button"
        className="button button-secondary"
        onClick={() => onRefresh?.()}
        disabled={isLoading}
      >
        <RefreshCw size={16} className={isLoading ? 'animate-spin' : ''} />
        Refresh
      </button>
    </FilterBar>
  )

  // Migrate any existing saved columns from 'value' to 'code'
  if (typeof window !== 'undefined') {
    try {
      const v1 = window.localStorage.getItem('ims.barcode.table.visibleColumns.v1')
      if (v1 && !window.localStorage.getItem('ims.barcode.table.visibleColumns.v2')) {
        const parsed = JSON.parse(v1)
        if (Array.isArray(parsed)) {
          const migrated = parsed.map((k) => (k === 'value' ? 'code' : k))
          window.localStorage.setItem('ims.barcode.table.visibleColumns.v2', JSON.stringify(migrated))
        }
      }
    } catch {
      // Ignore localStorage errors
    }
  }

  const columns = [
    { key: 'date', label: 'Date', format: 'date', sortable: true, render: (item) => formatDate(item.date) },
    {
      key: 'productName',
      label: 'Product',
      sortable: true,
      mobilePrimary: true,
      searchValue: (item) =>
        `${item.productName} ${item.codeType} ${item.code || item.value || item.codeValue || item.barcode || item.sku || ''}`,
    },
    {
      key: 'codeType',
      label: 'Type',
      sortable: true,
      mobileStatus: true,
      render: (item) => {
        const isQr = String(item.codeType || '').toLowerCase().includes('qr')
        return (
          <span className={`barcode-type-pill ${isQr ? 'barcode-type-pill--qr' : 'barcode-type-pill--barcode'}`}>
            {isQr ? <QrCode size={13} /> : <BarcodeIcon size={13} />}
            <span>{isQr ? 'QR Code' : 'Barcode'}</span>
          </span>
        )
      },
    },
    {
      key: 'code',
      label: 'Code',
      sortable: true,
      className: 'barcode-page__code',
      searchValue: (item) =>
        item.code ||
        item.value ||
        item.codeValue ||
        item.barcode ||
        item.sku ||
        (item.productId ? `PRD-${item.productId}` : ''),
      sortValue: (item) =>
        item.code ||
        item.value ||
        item.codeValue ||
        item.barcode ||
        item.sku ||
        (item.productId ? `PRD-${item.productId}` : ''),
      render: (item) => {
        const displayValue =
          item.code ||
          item.value ||
          item.codeValue ||
          item.barcode ||
          item.sku ||
          (item.productId ? `PRD-${item.productId}` : '-')
        return <span className="font-mono">{displayValue}</span>
      },
    },
    {
      key: 'preview',
      label: 'Preview',
      searchable: false,
      mobileHidden: true,
      render: (item) => {
        const displayValue =
          item.code ||
          item.value ||
          item.codeValue ||
          item.barcode ||
          item.sku ||
          (item.productId ? `PRD-${item.productId}` : 'BARCODE')
        return <CodePreview codeType={item.codeType} value={displayValue} compact={true} />
      },
      className: 'barcode-page__preview',
    },
    {
      key: 'actions',
      label: 'Actions',
      searchable: false,
      hideable: false,
      tableWidth: 80,
      render: (item) => (
        <ActionMenu
          iconOnly
          showTooltip={false}
          label={`Actions for ${item.productName || 'Barcode'}`}
          actions={[
            {
              key: 'print',
              label: 'Print',
              icon: Printer,
              onClick: () => handlePrintBarcode(item),
            },
          ]}
        />
      ),
    },
  ]

  const emptyMessage =
    activeTab === 'qr'
      ? 'No QR code records available.'
      : activeTab === 'barcode'
      ? 'No barcode records available.'
      : 'No barcode or QR records available.'

  return (
    <div className="card barcode-page__table-card">
      <div className="barcode-tabs-wrapper">
        <div className="barcode-tabs" role="tablist" aria-label="Code type filters">
          <button
            type="button"
            role="tab"
            aria-selected={activeTab === 'all'}
            className={`barcode-tab ${activeTab === 'all' ? 'is-active' : ''}`}
            onClick={() => setActiveTab('all')}
          >
            <ScanLine size={15} />
            <span>All Codes</span>
            <span className="barcode-tab__badge">{allCount}</span>
          </button>
          <button
            type="button"
            role="tab"
            aria-selected={activeTab === 'barcode'}
            className={`barcode-tab ${activeTab === 'barcode' ? 'is-active' : ''}`}
            onClick={() => setActiveTab('barcode')}
          >
            <BarcodeIcon size={15} />
            <span>Barcodes</span>
            <span className="barcode-tab__badge">{barcodeCount}</span>
          </button>
          <button
            type="button"
            role="tab"
            aria-selected={activeTab === 'qr'}
            className={`barcode-tab ${activeTab === 'qr' ? 'is-active' : ''}`}
            onClick={() => setActiveTab('qr')}
          >
            <QrCode size={15} />
            <span>QR Codes</span>
            <span className="barcode-tab__badge">{qrCount}</span>
          </button>
        </div>
      </div>

      <DataTable
        rows={filteredBarcodes}
        columns={columns}
        loading={isLoading}
        defaultPageSize={8}
        splitToolbar
        showColumnControls={true}
        columnStorageKey="ims.barcode.table.visibleColumns.v2"
        defaultVisibleColumnKeys={['date', 'productName', 'codeType', 'code', 'preview', 'actions']}
        toolbarContent={toolbarContent}
        searchPlaceholder="Search product or code"
        emptyMessage={emptyMessage}
        enableRowSelection={true}
        selectedRowKeys={selectedBarcodeIds}
        onSelectionChange={setSelectedBarcodeIds}
        keyField="id"
      />
    </div>
  )
}
