import { apiRequest, getResponseData, getResponseList } from './apiClient'
import { API_ENDPOINTS } from './endpoints'

export function getAllBarcodes() {
  return apiRequest(API_ENDPOINTS.barcode.list)
}

export function deduplicateBarcodes(barcodes = []) {
  const seen = new Set()
  const result = []

  for (const item of barcodes) {
    const key = String(item.productId || item.productName || '').trim().toLowerCase()
    if (!key) {
      result.push(item)
      continue
    }

    if (!seen.has(key)) {
      seen.add(key)
      result.push(item)
    }
  }

  return result
}

export function findExistingBarcode(productId, productName, barcodes = []) {
  const targetId = String(productId || '').trim()
  const targetName = String(productName || '').trim().toLowerCase()

  return barcodes.find((item) => {
    if (targetId && String(item.productId) === targetId) {
      return true
    }
    if (targetName && item.productName && item.productName.trim().toLowerCase() === targetName) {
      return true
    }
    return false
  }) || null
}

export async function getBarcodes(products = []) {
  const response = await getAllBarcodes()

  if (!response.success) {
    return response
  }

  const rawBarcodes = getResponseList(response)
  const normalizedBarcodes = rawBarcodes.map((item) => normalizeBarcode(item, products))

  return {
    ...response,
    data: deduplicateBarcodes(normalizedBarcodes),
  }
}

export async function generateBarcode(productId, products = []) {
  const response = await apiRequest(API_ENDPOINTS.barcode.generate, {
    method: 'POST',
    query: {
      productId: Number(productId) || productId,
    },
  })

  if (!response.success) {
    return response
  }

  const rawBarcode = getResponseData(response, {})
  const normalizedBarcode = normalizeBarcode(rawBarcode, products)

  return {
    ...response,
    data: normalizedBarcode,
  }
}

export function normalizeBarcode(item = {}, products = []) {
  const id = String(item.id || item.barcodeId || item.BarcodeId || '')
  let productId = String(item.productId || item.ProductId || '')
  const rawProductName = item.productName || item.ProductName || ''
  const product = products.find(
    (p) =>
      (productId && (String(p.id) === productId || String(p.productId) === productId)) ||
      (rawProductName && p.name && p.name.trim().toLowerCase() === rawProductName.trim().toLowerCase())
  )

  if (!productId && product?.id) {
    productId = String(product.id)
  }

  const productName = rawProductName || product?.name || 'Unknown Product'
  const rawValue = item.value || item.Value || item.code || item.Code || ''
  const codeType = item.codeType || item.CodeType || (rawValue.startsWith('QR:') ? 'QR Code' : 'Barcode')
  const dateVal = item.date || item.Date || item.createdAt || item.CreatedAt || new Date().toISOString().split('T')[0]
  const date = String(dateVal).split('T')[0]

  return {
    ...item,
    id,
    productId,
    productName,
    codeType,
    value: rawValue,
    preview: codeType === 'QR Code' ? `[ QR ] ${productName}` : `|||| ${rawValue} ||||`,
    date,
  }
}

