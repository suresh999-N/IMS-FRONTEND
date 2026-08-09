import { apiRequest, getResponseData, getResponseList } from './apiClient'
import { API_ENDPOINTS } from './endpoints'

/**
 * Normalizes raw API response item for a Purchase Return header and items.
 */
export function normalizePurchaseReturn(item = {}) {
  if (!item || typeof item !== 'object') return null

  const returnId = item.returnId ?? item.return_id ?? item.id ?? ''
  const supplierId = item.supplierId ?? item.supplier_id ?? ''
  const grnId = item.grnId ?? item.grn_id ?? ''
  const returnDate = item.returnDate ?? item.return_date ?? ''
  const totalAmount = Number(item.totalAmount ?? item.total_amount ?? 0)
  const reason = String(item.reason ?? '').trim()

  const rawItems = Array.isArray(item.items)
    ? item.items
    : Array.isArray(item.purchaseReturnItems)
    ? item.purchaseReturnItems
    : Array.isArray(item.purchase_return_items)
    ? item.purchase_return_items
    : []

  const items = rawItems.map((line, idx) => {
    const lineQty = Number(line.quantity ?? line.qty ?? 0)
    const linePrice = Number(line.price ?? line.unitPrice ?? 0)
    return {
      id: line.id ?? line.returnItemId ?? idx + 1,
      returnId: line.returnId ?? line.return_id ?? returnId,
      productId: line.productId ?? line.product_id ?? '',
      variantId: line.variantId ?? line.variant_id ?? null,
      quantity: lineQty,
      price: linePrice,
      total: lineQty * linePrice,
    }
  })

  return {
    ...item,
    id: returnId,
    returnId,
    supplierId,
    supplierName: item.supplierName ?? item.supplier_name ?? (item.supplier?.name || ''),
    grnId,
    grnNumber: item.grnNumber ?? item.grn_number ?? (grnId ? `GRN-${grnId}` : ''),
    returnDate,
    totalAmount,
    reason,
    items,
  }
}

/**
 * Fetch all purchase returns.
 */
export async function getPurchaseReturns(query = {}) {
  const response = await apiRequest(API_ENDPOINTS.purchaseReturns.list, { query })
  if (!response.success) return response

  const list = getResponseList(response)
  return {
    ...response,
    data: list.map(normalizePurchaseReturn),
  }
}

/**
 * Fetch a single purchase return by ID.
 */
export async function getPurchaseReturnById(id) {
  const response = await apiRequest(API_ENDPOINTS.purchaseReturns.byId(id))
  if (!response.success) return response

  const data = getResponseData(response, {})
  return {
    ...response,
    data: normalizePurchaseReturn(data),
  }
}

/**
 * Create a new purchase return.
 * Payload matches: { supplier_id, grn_id, return_date, total_amount, reason, items: [{ product_id, variant_id, quantity, price }] }
 */
export async function createPurchaseReturn(payload) {
  const body = {
    supplier_id: Number(payload.supplierId ?? payload.supplier_id),
    grn_id: Number(payload.grnId ?? payload.grn_id),
    return_date: payload.returnDate ?? payload.return_date ?? new Date().toISOString(),
    total_amount: Number(payload.totalAmount ?? payload.total_amount ?? 0),
    reason: payload.reason || '',
    items: (payload.items || []).map((item) => ({
      product_id: Number(item.productId ?? item.product_id),
      variant_id: item.variantId || item.variant_id ? Number(item.variantId ?? item.variant_id) : null,
      quantity: Number(item.quantity),
      price: Number(item.price),
    })),
  }

  return apiRequest(API_ENDPOINTS.purchaseReturns.list, {
    method: 'POST',
    body,
  })
}

/**
 * Update an existing purchase return.
 */
export async function updatePurchaseReturn(id, payload) {
  const body = {
    supplier_id: Number(payload.supplierId ?? payload.supplier_id),
    grn_id: Number(payload.grnId ?? payload.grn_id),
    return_date: payload.returnDate ?? payload.return_date,
    total_amount: Number(payload.totalAmount ?? payload.total_amount ?? 0),
    reason: payload.reason || '',
    items: (payload.items || []).map((item) => ({
      product_id: Number(item.productId ?? item.product_id),
      variant_id: item.variantId || item.variant_id ? Number(item.variantId ?? item.variant_id) : null,
      quantity: Number(item.quantity),
      price: Number(item.price),
    })),
  }

  return apiRequest(API_ENDPOINTS.purchaseReturns.byId(id), {
    method: 'PUT',
    body,
  })
}

/**
 * Delete a purchase return.
 */
export async function deletePurchaseReturn(id) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.byId(id), {
    method: 'DELETE',
  })
}
