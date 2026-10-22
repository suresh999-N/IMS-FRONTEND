import { apiRequest, getResponseData, getResponseList } from './apiClient'
import { API_ENDPOINTS } from './endpoints'

export function getAllBarcodes() {
  return apiRequest(API_ENDPOINTS.barcode.list)
}

export function deduplicateBarcodes(barcodes = []) {
  const seen = new Set()
  const result = []

  for (const item of barcodes) {
    const prodKey = String(item.productId || item.productName || '').trim().toLowerCase()
    const typeKey = String(item.codeType || 'Barcode').trim().toLowerCase()
    const key = prodKey ? `${prodKey}_${typeKey}` : ''
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

export function findExistingBarcode(productId, productName, barcodes = [], codeType = 'Barcode') {
  const targetId = String(productId || '').trim()
  const targetName = String(productName || '').trim().toLowerCase()
  const targetType = String(codeType || 'Barcode').trim().toLowerCase()

  return barcodes.find((item) => {
    const itemType = String(item.codeType || 'Barcode').trim().toLowerCase()
    if (targetType && itemType !== targetType) {
      return false
    }

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

  let productList = Array.isArray(products) && products.length > 0 ? products : []
  if (productList.length === 0) {
    try {
      const prodRes = await apiRequest(API_ENDPOINTS.products.list)
      if (prodRes?.success) {
        productList = getResponseList(prodRes)
      }
    } catch {
      // Ignore fallback fetch error
    }
  }

  const rawBarcodes = getResponseList(response)
  const normalizedBarcodes = rawBarcodes.map((item) => normalizeBarcode(item, productList))

  return {
    ...response,
    data: deduplicateBarcodes(normalizedBarcodes),
  }
}

export async function generateBarcode(productId, codeTypeOrProducts = 'Barcode', products = []) {
  const normalizedType = typeof codeTypeOrProducts === 'string'
    ? (codeTypeOrProducts.toLowerCase().includes('qr') ? 'QR Code' : 'Barcode')
    : 'Barcode'

  const resolvedProducts = Array.isArray(codeTypeOrProducts)
    ? codeTypeOrProducts
    : (Array.isArray(products) ? products : [])

  const response = await apiRequest(API_ENDPOINTS.barcode.generate, {
    method: 'POST',
    query: {
      productId: Number(productId) || productId,
      codeType: normalizedType,
    },
  })

  if (!response.success) {
    return response
  }

  let productList = resolvedProducts.length > 0 ? resolvedProducts : []
  if (productList.length === 0) {
    try {
      const prodRes = await apiRequest(API_ENDPOINTS.products.list)
      if (prodRes?.success) {
        productList = getResponseList(prodRes)
      }
    } catch {
      // Ignore fallback fetch error
    }
  }

  const rawBarcode = getResponseData(response, {})
  const normalizedBarcode = normalizeBarcode(rawBarcode, productList)

  return {
    ...response,
    data: normalizedBarcode,
  }
}

export function normalizeBarcode(item = {}, products = []) {
  const rawId = String(item.id || item.barcodeId || item.BarcodeId || '')
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
  const rawValue =
    item.value ||
    item.Value ||
    item.codeValue ||
    item.CodeValue ||
    item.code_value ||
    item.code ||
    item.Code ||
    item.barcode ||
    item.Barcode ||
    product?.barcode ||
    product?.Barcode ||
    product?.sku ||
    product?.SKU ||
    product?.productCode ||
    product?.ProductCode ||
    (productId ? `PRD-${productId}` : '') ||
    (rawId ? `BC-${rawId}` : '') ||
    ''

  const rawCodeType = item.codeType || item.CodeType
  const codeType = rawCodeType
    ? (String(rawCodeType).trim().toLowerCase().includes('qr') ? 'QR Code' : 'Barcode')
    : (String(rawValue).startsWith('QR:') ? 'QR Code' : 'Barcode')

  const dateVal = item.date || item.Date || item.createdAt || item.CreatedAt || new Date().toISOString().split('T')[0]
  const date = String(dateVal).split('T')[0]
  const typeKey = codeType.toLowerCase().replace(/\s+/g, '-')
  const id = rawId || String(productId ? `${typeKey}-prod-${productId}` : `${productName}-${typeKey}-${rawValue || date}`)

  return {
    ...item,
    id,
    productId,
    productName,
    codeType,
    code: rawValue,
    value: rawValue,
    preview: codeType === 'QR Code' ? `[ QR ] ${productName}` : `|||| ${rawValue} ||||`,
    date,
  }
}

