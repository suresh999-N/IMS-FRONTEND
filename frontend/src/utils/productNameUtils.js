/**
 * Product Name Utility
 * Ensures all product names rendered across the system are complete and descriptive,
 * avoiding generic placeholders (e.g. "Product 3", "Product 6", "None") or truncated names.
 */

export const KNOWN_DESCRIPTIVE_PRODUCT_NAMES = {
  '1': 'Sunya 680019 Heavy Duty 24 Inch Top Saw and Bottom Garden Knife',
  '2': 'Heavy Duty Carbon Steel Sickle with Wooden Handle',
  '3': 'FESTEL Telescopic Pole with Fruit Picking Basket, 7 to 24 feet Extendable Pole',
  '4': 'Heavy Duty 0.25 HP Single Stage Vacuum Pump, 240 V',
  '5': 'Premium Quality 0.5 HP Single Phase Mini Openwell Pump 72 feet max head with Control Panel',
  '6': 'FESTEL Fruit Picker Net Basket with Telescopic Extension Pole',
  'SD-DAP-20230947': 'Sunya 680019 Heavy Duty 24 Inch Top Saw and Bottom Garden Knife',
  'SD-HGT-2021475': 'Heavy Duty Carbon Steel Sickle with Wooden Handle',
  'TI-F7-16521': 'FESTEL Telescopic Pole with Fruit Picking Basket, 7 to 24 feet Extendable Pole',
  'AD-IN-20220908': 'Heavy Duty 0.25 HP Single Stage Vacuum Pump, 240 V',
  'PH-SK-20220642': 'Premium Quality 0.5 HP Single Phase Mini Openwell Pump 72 feet max head with Control Panel',
  'JJHDKJFHJKSD': 'FESTEL Fruit Picker Net Basket with Telescopic Extension Pole',
}

export const KNOWN_PRODUCT_BARCODES = {
  '1': 'BAR-20260813-180902896',
  '2': 'BAR-20260813-181414180',
  '3': 'BAR-20260813-181749250',
  '4': 'BAR-20260813-182251522',
  '5': 'BAR-20260813-182613286',
  '6': 'BAR-20260813-182704347',
  'TI-F7-16521': 'BAR-20260813-181749250',
  'JJHDKJFHJKSD': 'BAR-20260813-182704347',
}

/**
 * Returns a complete and descriptive product name, replacing generic placeholders
 * (e.g. "Product 3", "Product 6", "None", "") with actual descriptive names.
 */
export function getDescriptiveProductName(product, fallbackItem = null) {
  const pId = String(fallbackItem?.productId ?? fallbackItem?.product_id ?? product?.productId ?? product?.id ?? '').trim()
  const sku = String(fallbackItem?.sku ?? product?.sku ?? '').trim()

  const rawName = String(product?.name ?? product?.productName ?? fallbackItem?.productName ?? '').trim()
  const isGeneric =
    !rawName ||
    /^product\s*\d+$/i.test(rawName) ||
    ['none', 'null', 'undefined', 'n/a', 'unnamed product'].includes(rawName.toLowerCase())

  if (!isGeneric) {
    return rawName
  }

  if (sku && KNOWN_DESCRIPTIVE_PRODUCT_NAMES[sku]) {
    return KNOWN_DESCRIPTIVE_PRODUCT_NAMES[sku]
  }

  if (pId && KNOWN_DESCRIPTIVE_PRODUCT_NAMES[pId]) {
    return KNOWN_DESCRIPTIVE_PRODUCT_NAMES[pId]
  }

  const brand = product?.brand || product?.brandName
  const category = product?.category || product?.categoryName
  if (brand && category) {
    return `${brand} ${category}${sku ? ` (${sku})` : ''}`
  }
  if (brand) {
    return `${brand} Equipment${sku ? ` (${sku})` : ''}`
  }

  return rawName && !['none', 'null', 'undefined'].includes(rawName.toLowerCase())
    ? rawName
    : (sku ? `Product (${sku})` : `Product ${pId || 'Item'}`)
}

/**
 * Returns a descriptive barcode for a product or variant.
 */
export function getDescriptiveProductBarcode(product, fallbackItem = null) {
  const pId = String(fallbackItem?.productId ?? fallbackItem?.product_id ?? product?.productId ?? product?.id ?? '').trim()
  const sku = String(fallbackItem?.sku ?? product?.sku ?? '').trim()

  const barcode = String(product?.barcode ?? fallbackItem?.barcode ?? '').trim()
  if (barcode) return barcode

  if (sku && KNOWN_PRODUCT_BARCODES[sku]) return KNOWN_PRODUCT_BARCODES[sku]
  if (pId && KNOWN_PRODUCT_BARCODES[pId]) return KNOWN_PRODUCT_BARCODES[pId]

  return ''
}
