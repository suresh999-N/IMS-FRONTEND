import { Printer, RefreshCw } from 'lucide-react'
import CodePreview from './CodePreview'
import { DataTable, ActionMenu, FilterBar } from '../../../../components/erp'
import { getBarcodeBars, getQrCells } from '../utils/preview'

function escapeHtml(value) {
  return String(value ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;')
}

function handlePrintBarcode(item) {
  if (!item) return

  const isQr = item.codeType === 'QR Code'
  const printWindow = window.open('', '_blank')
  if (!printWindow) return

  let visualHtml = ''
  if (isQr) {
    const cells = getQrCells(item.value)
    visualHtml = `
      <div style="display: grid; grid-template-columns: repeat(21, 8px); gap: 1px; justify-content: center; background: #ffffff; padding: 12px; border: 1px solid #cbd5e1; border-radius: 8px; margin: 0 auto; width: fit-content;">
        ${cells.map(c => `<span style="width: 8px; height: 8px; background-color: ${c.dark ? '#000000' : '#ffffff'}; display: block;"></span>`).join('')}
      </div>
    `
  } else {
    const bars = getBarcodeBars(item.value)
    visualHtml = `
      <div style="display: flex; align-items: flex-end; justify-content: center; gap: 2px; height: 64px; padding: 12px 16px; background: #ffffff; border: 1px solid #cbd5e1; border-radius: 8px; margin: 0 auto; width: fit-content;">
        ${bars.map(b => `<span style="width: ${Math.max(2, b.width * 2.2)}px; height: ${Math.max(20, b.height * 1.15)}px; background-color: #000000; display: inline-block;"></span>`).join('')}
      </div>
    `
  }

  printWindow.document.write(`<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <title>Print Code - ${escapeHtml(item.productName || 'Barcode')}</title>
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
    .label-card {
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
      margin-bottom: 18px;
    }
    .product-title {
      font-size: 19px;
      font-weight: 800;
      color: #0f172a;
      margin: 0 0 14px;
      letter-spacing: -0.01em;
      line-height: 1.35;
    }
    .code-type-divider {
      position: relative;
      text-align: center;
      margin: 14px 0 0;
    }
    .code-type-divider::before {
      content: "";
      position: absolute;
      top: 50%;
      left: 0;
      right: 0;
      border-top: 1px solid #cbd5e1;
      z-index: 1;
    }
    .code-type-divider span {
      position: relative;
      z-index: 2;
      background: #ffffff;
      padding: 0 10px;
      font-size: 14px;
      font-weight: 600;
      color: #0f172a;
    }
    .visual-wrapper {
      margin: 18px 0;
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
      .label-card { border-color: #000000; box-shadow: none; }
    }
  </style>
</head>
<body>
  <div class="label-card">
    <div class="label-header">
      <h2 class="product-title">${escapeHtml(item.productName || 'Product')}</h2>
      <div class="code-type-divider">
        <span>${escapeHtml(item.codeType || 'Barcode')}:</span>
      </div>
    </div>
    <div class="visual-wrapper">
      ${visualHtml}
      <div class="code-value">${escapeHtml(item.value || '')}</div>
    </div>
    <div class="label-footer">
      <span>Date: ${escapeHtml(item.date || '')}</span>
      <span>IMS Inventory</span>
    </div>
  </div>
</body>
</html>`)

  printWindow.document.close()
  printWindow.focus()
  printWindow.print()
}

export default function BarcodeTable({ barcodes, onRefresh, isLoading }) {
  const toolbarContent = (
    <FilterBar className="barcode-page__toolbar-actions" ariaLabel="Barcode table refresh actions">
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

  const columns = [
    { key: 'date', label: 'Date', sortable: true },
    {
      key: 'productName',
      label: 'Product',
      sortable: true,
      mobilePrimary: true,
      searchValue: (item) => `${item.productName} ${item.codeType} ${item.value}`,
    },
    { key: 'codeType', label: 'Type', sortable: true, mobileStatus: true },
    {
      key: 'value',
      label: 'Value',
      sortable: true,
      className: 'barcode-page__code',
    },
    {
      key: 'preview',
      label: 'Preview',
      searchable: false,
      mobileHidden: true,
      render: (item) => <CodePreview codeType={item.codeType} value={item.value} />,
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

  return (
    <div className="card barcode-page__table-card">
      <DataTable
        rows={barcodes}
        columns={columns}
        defaultPageSize={8}
        splitToolbar
        showColumnControls={true}
        columnStorageKey="ims.barcode.table.visibleColumns.v1"
        defaultVisibleColumnKeys={['date', 'productName', 'codeType', 'value', 'preview', 'actions']}
        toolbarContent={toolbarContent}
        searchPlaceholder="Search product or code"
        emptyMessage="No barcode or QR records available."
      />
    </div>
  )
}
