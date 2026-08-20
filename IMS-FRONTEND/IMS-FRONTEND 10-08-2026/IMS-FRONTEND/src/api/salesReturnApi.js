import { apiRequest, getResponseData, getResponseList } from './apiClient'
import { API_ENDPOINTS } from './endpoints'

/**
 * Normalizes raw API response item for a Sales Return header and items based strictly on:
 * - sales_returns (return_id, invoice_id, customer_id, return_date, total_amount, reason)
 * - sales_return_items (id, return_id, product_id, variant_id, quantity, price)
 */
export function normalizeSalesReturn(item = {}) {
  if (!item || typeof item !== 'object') return null

  let source = item
  if (Array.isArray(item)) {
    if (item.length === 0) return null
    source = item[0]
  }

  if (source && typeof source === 'object') {
    if (source.salesReturn && typeof source.salesReturn === 'object') source = source.salesReturn
    else if (source.SalesReturn && typeof source.SalesReturn === 'object') source = source.SalesReturn
    else if (source.return && typeof source.return === 'object') source = source.return
    else if (source.Return && typeof source.Return === 'object') source = source.Return
    else if (source.returnDetails && typeof source.returnDetails === 'object') source = source.returnDetails
    else if (source.ReturnDetails && typeof source.ReturnDetails === 'object') source = source.ReturnDetails
    else if (source.data && typeof source.data === 'object' && !Array.isArray(source.data)) source = source.data
  }

  if (!source || typeof source !== 'object' || Object.keys(source).length === 0) return null

  const salesReturnId =
    source.salesReturnId ??
    source.SalesReturnId ??
    source.returnId ??
    source.ReturnId ??
    source.id ??
    source.Id ??
    source.ID ??
    source.return_id ??
    source.sales_return_id ??
    source.salesReturnID ??
    source.returnID ??
    source.srrId ??
    source.SrrId ??
    source.SRR_Id ??
    ''

  const returnNumber =
    source.returnNumber ??
    source.ReturnNumber ??
    source.return_number ??
    source.returnNo ??
    source.ReturnNo ??
    source.return_no ??
    source.returnReference ??
    source.ReturnReference ??
    source.referenceNumber ??
    source.ReferenceNumber ??
    (salesReturnId ? `SRR-${String(salesReturnId).padStart(6, '0')}` : '')

  if (!salesReturnId && !returnNumber) return null

  const id = String(salesReturnId || returnNumber.replace(/\D/g, '') || source.id || '')
  const returnId = id || returnNumber

  const invoiceId = source.invoiceId ?? source.InvoiceId ?? source.invoice_id ?? source.invoice?.id ?? ''
  const invoiceNumber =
    source.invoiceNumber ??
    source.InvoiceNumber ??
    source.invoice_number ??
    source.invoiceNo ??
    source.InvoiceNo ??
    source.invoice?.invoiceNumber ??
    (invoiceId ? `INV-${String(invoiceId).padStart(6, '0')}` : '')

  const customerId = source.customerId ?? source.CustomerId ?? source.customer_id ?? source.customer?.id ?? ''
  const customerName =
    source.customerName ??
    source.CustomerName ??
    source.customer_name ??
    source.customer?.name ??
    source.customer?.customerName ??
    source.customer?.displayName ??
    ''

  const returnDate =
    source.returnDate ??
    source.ReturnDate ??
    source.return_date ??
    source.createdAt ??
    source.CreatedAt ??
    source.created_at ??
    source.date ??
    ''

  const totalAmount = Number(
    source.totalReturnAmount ??
    source.TotalReturnAmount ??
    source.totalAmount ??
    source.TotalAmount ??
    source.total_amount ??
    source.total_return_amount ??
    source.amount ??
    source.Amount ??
    0
  )

  const reason = String(
    source.reason ??
    source.Reason ??
    source.returnReason ??
    source.ReturnReason ??
    source.return_reason ??
    ''
  ).trim()

  const status = String(source.status ?? source.Status ?? 'Completed').trim()

  const rawItems = Array.isArray(source.items)
    ? source.items
    : Array.isArray(source.Items)
    ? source.Items
    : Array.isArray(source.salesReturnItems)
    ? source.salesReturnItems
    : Array.isArray(source.SalesReturnItems)
    ? source.SalesReturnItems
    : Array.isArray(source.sales_return_items)
    ? source.sales_return_items
    : Array.isArray(source.returnItems)
    ? source.returnItems
    : Array.isArray(source.ReturnItems)
    ? source.ReturnItems
    : Array.isArray(source.return_items)
    ? source.return_items
    : Array.isArray(source.lineItems)
    ? source.lineItems
    : Array.isArray(source.LineItems)
    ? source.LineItems
    : Array.isArray(source.lines)
    ? source.lines
    : Array.isArray(source.Lines)
    ? source.Lines
    : Array.isArray(source.details)
    ? source.details
    : Array.isArray(source.Details)
    ? source.Details
    : []

  const items = rawItems.map((line, idx) => {
    const lineQty = Number(
      line.returnQuantity ??
      line.ReturnQuantity ??
      line.quantity ??
      line.Quantity ??
      line.qty ??
      line.Qty ??
      line.returnedQuantity ??
      0
    )
    const linePrice = Number(
      line.price ??
      line.Price ??
      line.unitPrice ??
      line.UnitPrice ??
      line.salePrice ??
      line.unit_price ??
      0
    )
    const invoicedQty = Number(
      line.invoicedQuantity ??
      line.InvoicedQuantity ??
      line.receivedQuantity ??
      line.ReceivedQuantity ??
      line.invoicedQty ??
      lineQty
    )
    const lineTotal = Number(
      line.total ??
      line.Total ??
      line.lineTotal ??
      line.LineTotal ??
      line.amount ??
      line.Amount ??
      lineQty * linePrice
    )

    const prodId = line.productId ?? line.ProductId ?? line.product_id ?? line.product?.id ?? ''
    const prodName =
      line.productName ??
      line.ProductName ??
      line.product_name ??
      line.product?.name ??
      line.product?.productName ??
      (prodId ? `Product #${prodId}` : '')

    const varId = line.variantId ?? line.VariantId ?? line.variant_id ?? line.variant?.id ?? null
    const varName =
      line.variantName ??
      line.VariantName ??
      line.variant_name ??
      line.variant?.name ??
      line.variant?.variantName ??
      line.variant?.title ??
      ''

    return {
      id: line.salesReturnItemId ?? line.SalesReturnItemId ?? line.returnItemId ?? line.ReturnItemId ?? line.id ?? line.Id ?? idx + 1,
      salesReturnItemId: line.salesReturnItemId ?? line.SalesReturnItemId ?? line.returnItemId ?? line.ReturnItemId ?? line.id ?? line.Id ?? idx + 1,
      returnId: line.returnId ?? line.ReturnId ?? line.return_id ?? id,
      productId: prodId,
      productName: prodName,
      variantId: varId,
      variantName: varName,
      invoicedQuantity: invoicedQty,
      receivedQuantity: invoicedQty,
      returnQuantity: lineQty,
      quantity: lineQty,
      price: linePrice,
      total: lineTotal,
    }
  })

  return {
    ...source,
    id,
    salesReturnId: id,
    returnId: id,
    returnNumber,
    invoiceId,
    invoiceNumber,
    customerId,
    customerName,
    returnDate,
    totalAmount,
    totalReturnAmount: totalAmount,
    reason,
    status,
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
    data: list.map(normalizeSalesReturn).filter(Boolean),
  }
}

/**
 * Fetch a single sales return by ID.
 */
export async function getSalesReturnById(id) {
  if (!id) {
    return {
      success: false,
      data: null,
      error: 'Sales return ID is required.',
    }
  }

  const cleanId = String(id).trim()
  const numericId = cleanId.replace(/\D/g, '')

  // 1. Try direct GET request by ID
  let response = await apiRequest(API_ENDPOINTS.salesReturns.byId(cleanId))

  // 2. If direct call failed and cleanId contains formatting (e.g. "SRR-000001"), try numeric ID
  if ((!response || !response.success) && numericId && numericId !== cleanId) {
    const numResponse = await apiRequest(API_ENDPOINTS.salesReturns.byId(numericId))
    if (numResponse && numResponse.success) {
      response = numResponse
    }
  }

  // 3. If direct call succeeded, try normalizing the returned payload
  if (response && response.success) {
    const data = getResponseData(response, null)
    const normalized = normalizeSalesReturn(data)
    if (normalized) {
      return {
        ...response,
        data: normalized,
      }
    }
  }

  // 4. Fallback: Search in full list of Sales Returns if direct lookup failed or yielded non-normalized data
  try {
    const listRes = await getSalesReturns()
    if (listRes && listRes.success && Array.isArray(listRes.data)) {
      const targetId = cleanId.toLowerCase()
      const targetNumId = numericId

      const matched = listRes.data.find((item) => {
        if (!item) return false
        const itemId = String(item.salesReturnId || item.id || item.returnId || '').trim()
        const itemNum = String(item.returnNumber || '').trim().toLowerCase()

        return (
          itemId.toLowerCase() === targetId ||
          (targetNumId && itemId === targetNumId) ||
          itemNum === targetId ||
          (itemNum && targetId.includes(itemNum)) ||
          (itemNum && itemNum.includes(targetId))
        )
      })

      if (matched) {
        return {
          success: true,
          data: matched,
          error: null,
        }
      }
    }
  } catch (fallbackErr) {
    console.error('Fallback lookup for sales return failed:', fallbackErr)
  }

  return {
    success: false,
    data: null,
    error: response?.error || 'Sales return record not found on server.',
  }
}

/**
 * Fetch invoice items for a sales return (with invoiced qty, price, and remaining returnable qty).
 */
export async function getSalesReturnInvoiceItems(invoiceId) {
  const response = await apiRequest(API_ENDPOINTS.salesReturns.invoiceItems(invoiceId))
  if (!response.success) return response
  const list = getResponseList(response)
  return {
    ...response,
    data: list,
  }
}

/**
 * Fetch customers dropdown for sales return.
 */
export async function getSalesReturnCustomers() {
  const response = await apiRequest(API_ENDPOINTS.salesReturns.customers)
  if (!response.success) return response
  const list = getResponseList(response)
  return {
    ...response,
    data: list,
  }
}

/**
 * Fetch invoices for customer.
 */
export async function getSalesReturnInvoices(customerId) {
  const response = await apiRequest(API_ENDPOINTS.salesReturns.customerInvoices(customerId))
  if (!response.success) return response
  const list = getResponseList(response)
  return {
    ...response,
    data: list,
  }
}

/**
 * Create a new sales return.
 */
export async function createSalesReturn(payload) {
  const customerId = Number(payload.customerId ?? payload.CustomerId ?? payload.customer_id)
  const invoiceId = Number(payload.invoiceId ?? payload.InvoiceId ?? payload.invoice_id)
  const returnDate = payload.returnDate ?? payload.ReturnDate ?? payload.return_date ?? new Date().toISOString()
  const reason = String(payload.reason ?? payload.Reason ?? '').trim()

  const rawItems = Array.isArray(payload.items) ? payload.items : Array.isArray(payload.Items) ? payload.Items : []
  const items = rawItems.map((item) => {
    const qty = Number(item.returnQuantity ?? item.ReturnQuantity ?? item.quantity ?? item.qty ?? 0)
    const varId = item.variantId || item.VariantId || item.variant_id ? Number(item.variantId ?? item.VariantId ?? item.variant_id) : null
    const pId = Number(item.productId ?? item.ProductId ?? item.product_id)

    return {
      productId: pId,
      variantId: varId,
      returnQuantity: qty,
    }
  })

  const body = {
    customerId,
    invoiceId,
    returnDate,
    reason,
    items,
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
  const customerId = Number(payload.customerId ?? payload.CustomerId ?? payload.customer_id)
  const invoiceId = Number(payload.invoiceId ?? payload.InvoiceId ?? payload.invoice_id)
  const returnDate = payload.returnDate ?? payload.ReturnDate ?? payload.return_date
  const reason = String(payload.reason ?? payload.Reason ?? '').trim()

  const rawItems = Array.isArray(payload.items) ? payload.items : Array.isArray(payload.Items) ? payload.Items : []
  const items = rawItems.map((item) => {
    const qty = Number(item.returnQuantity ?? item.ReturnQuantity ?? item.quantity ?? item.qty ?? 0)
    const varId = item.variantId || item.VariantId || item.variant_id ? Number(item.variantId ?? item.VariantId ?? item.variant_id) : null
    const pId = Number(item.productId ?? item.ProductId ?? item.product_id)

    return {
      productId: pId,
      variantId: varId,
      returnQuantity: qty,
    }
  })

  const body = {
    customerId,
    invoiceId,
    returnDate,
    reason,
    items,
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
