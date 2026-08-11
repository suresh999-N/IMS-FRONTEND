import { apiRequest, getResponseData, getResponseList } from './apiClient'
import { API_ENDPOINTS } from './endpoints'

/**
 * Normalizes raw API response item for a Sales Return header and items based strictly on:
 * - sales_returns (return_id, invoice_id, customer_id, return_date, total_amount, reason)
 * - sales_return_items (id, return_id, product_id, variant_id, quantity, price)
 */
export function normalizeSalesReturn(item = {}) {
  if (!item || typeof item !== 'object') return null

  const returnId = item.returnId ?? item.return_id ?? item.id ?? ''
  const invoiceId = item.invoiceId ?? item.invoice_id ?? ''
  const customerId = item.customerId ?? item.customer_id ?? ''
  const returnDate = item.returnDate ?? item.return_date ?? ''
  const totalAmount = Number(item.totalAmount ?? item.total_amount ?? 0)
  const reason = String(item.reason ?? '').trim()

  const rawItems = Array.isArray(item.items)
    ? item.items
    : Array.isArray(item.salesReturnItems)
    ? item.salesReturnItems
    : Array.isArray(item.sales_return_items)
    ? item.sales_return_items
    : []

  const items = rawItems.map((line, idx) => {
    const lineQty = Number(line.quantity ?? line.qty ?? line.returnQuantity ?? 0)
    const linePrice = Number(line.price ?? line.unitPrice ?? 0)
    return {
      id: line.id ?? line.returnItemId ?? idx + 1,
      returnId: line.returnId ?? line.return_id ?? returnId,
      productId: line.productId ?? line.product_id ?? '',
      variantId: line.variantId ?? line.variant_id ?? null,
      receivedQuantity: Number(line.receivedQuantity ?? line.invoicedQuantity ?? line.quantity ?? 0),
      returnQuantity: lineQty,
      quantity: lineQty,
      price: linePrice,
      total: lineQty * linePrice,
    }
  })

  return {
    ...item,
    id: returnId,
    returnId,
    invoiceId,
    invoiceNumber: item.invoiceNumber ?? item.invoice_number ?? (invoiceId ? `SINV-${invoiceId}` : ''),
    customerId,
    customerName: item.customerName ?? item.customer_name ?? (item.customer?.name || ''),
    returnDate,
    totalAmount,
    reason,
    items,
  }
}

/**
 * Fetch all sales returns.
 */
export async function getSalesReturns(query = {}) {
  const response = await apiRequest(API_ENDPOINTS.salesReturns.list, { query })
  if (!response.success) return response

  const list = getResponseList(response)
  return {
    ...response,
    data: list.map(normalizeSalesReturn),
  }
}

/**
 * Fetch a single sales return by ID.
 */
export async function getSalesReturnById(id) {
  const response = await apiRequest(API_ENDPOINTS.salesReturns.byId(id))
  if (!response.success) return response

  const data = getResponseData(response, {})
  return {
    ...response,
    data: normalizeSalesReturn(data),
  }
}

/**
 * Create a new sales return.
 * Payload matches: { invoice_id, customer_id, return_date, total_amount, reason, items: [{ product_id, variant_id, quantity, price }] }
 */
export async function createSalesReturn(payload) {
  const body = {
    invoice_id: Number(payload.invoiceId ?? payload.invoice_id),
    customer_id: Number(payload.customerId ?? payload.customer_id),
    return_date: payload.returnDate ?? payload.return_date ?? new Date().toISOString(),
    total_amount: Number(payload.totalAmount ?? payload.total_amount ?? 0),
    reason: payload.reason || '',
    items: (payload.items || []).map((item) => ({
      product_id: Number(item.productId ?? item.product_id),
      variant_id: item.variantId || item.variant_id ? Number(item.variantId ?? item.variant_id) : null,
      quantity: Number(item.quantity ?? item.returnQuantity),
      price: Number(item.price),
    })),
  }

  return apiRequest(API_ENDPOINTS.salesReturns.list, {
    method: 'POST',
    body,
  })
}

/**
 * Update an existing sales return.
 */
export async function updateSalesReturn(id, payload) {
  const body = {
    invoice_id: Number(payload.invoiceId ?? payload.invoice_id),
    customer_id: Number(payload.customerId ?? payload.customer_id),
    return_date: payload.returnDate ?? payload.return_date,
    total_amount: Number(payload.totalAmount ?? payload.total_amount ?? 0),
    reason: payload.reason || '',
    items: (payload.items || []).map((item) => ({
      product_id: Number(item.productId ?? item.product_id),
      variant_id: item.variantId || item.variant_id ? Number(item.variantId ?? item.variant_id) : null,
      quantity: Number(item.quantity ?? item.returnQuantity),
      price: Number(item.price),
    })),
  }

  return apiRequest(API_ENDPOINTS.salesReturns.byId(id), {
    method: 'PUT',
    body,
  })
}

/**
 * Delete a sales return.
 */
export async function deleteSalesReturn(id) {
  return apiRequest(API_ENDPOINTS.salesReturns.byId(id), {
    method: 'DELETE',
  })
}
