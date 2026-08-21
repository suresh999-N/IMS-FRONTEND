import { formatDate } from '../../../utils/helpers'
import { currency } from './purchaseReturnStore'

export function exportRowsToCsv(rows = [], filename = 'purchase-returns.csv') {
  if (!rows || rows.length === 0) return

  const headers = ['Return Number', 'Goods Receipt', 'Supplier Name', 'Return Date', 'Items Count', 'Total Amount']
  const csvLines = [headers.join(',')]

  rows.forEach((row) => {
    const line = [
      `"${row.returnNumber || `PR-${row.returnId}`}"`,
      `"${row.grnNumber || `GRN-${row.grnId}`}"`,
      `"${(row.supplierName || '').replace(/"/g, '""')}"`,
      `"${formatDate(row.returnDate)}"`,
      row.itemCount || row.items?.length || 0,
      `"${currency(row.totalAmount ?? row.totalReturnAmount)}"`,
    ]
    csvLines.push(line.join(','))
  })

  const blob = new Blob([csvLines.join('\n')], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.setAttribute('href', url)
  link.setAttribute('download', filename)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}
