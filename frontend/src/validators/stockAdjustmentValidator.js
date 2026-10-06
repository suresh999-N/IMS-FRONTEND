/**
 * Validation and preset options for Stock Adjustment reasons based on Adjustment Type.
 */

export const REASON_PRESETS = {
  increase: [
    'Low stock',
    'New purchase',
    'Correction',
    'Found inventory',
    'Audit surplus',
    'Inward discrepancy',
    'Opening stock',
    'Restock',
  ],
  decrease: [
    'Damage',
    'Expiry',
    'Shrinkage',
    'Theft / Loss',
    'Correction',
    'Breakage / Spoilage',
    'Write-off',
    'Audit deficit',
    'Outward discrepancy',
  ],
  recount: [
    'Physical count reconciliation',
    'Cycle count',
    'Audit recount',
    'Correction',
    'Discrepancy resolution',
  ],
}

// Patterns that are explicitly mismatched/forbidden for Increase adjustments
const INCREASE_FORBIDDEN_PATTERNS = [
  {
    regex: /\b(?:high stock|overstock|excess stock)\b/i,
    message: "Reason 'high stock' is invalid for Increase adjustment. Increase adjustments are for replenishments or positive corrections.",
  },
  {
    regex: /\b(?:damage|damaged|breakage|broken|crushed)\b/i,
    message: "Damage causes inventory reduction and cannot be used as a reason for an Increase adjustment.",
  },
  {
    regex: /\b(?:expiry|expired|spoiled|spoilage|rotten|decayed)\b/i,
    message: "Expiry or spoilage causes inventory reduction and cannot be used as a reason for an Increase adjustment.",
  },
  {
    regex: /\b(?:shrinkage|theft|stolen|pilferage|loss|lost)\b/i,
    message: "Shrinkage or loss causes inventory reduction and cannot be used as a reason for an Increase adjustment.",
  },
  {
    regex: /\b(?:waste|scrap|scrapped|write-off|write off|written off|deficit|shortage|outward discrepancy)\b/i,
    message: "Waste, write-off, or deficit causes inventory reduction and cannot be used for an Increase adjustment.",
  },
]

// Keywords indicating valid reasons for Increase adjustments
const INCREASE_VALID_KEYWORDS = [
  'low stock',
  'new purchase',
  'purchase',
  'correction',
  'correct',
  'found',
  'surplus',
  'inward',
  'opening',
  'restock',
  'received extra',
  'excess found',
  'replenish',
  'return',
  'recount',
  'unaccounted',
  'addition',
  'reconciliation',
  'balance',
]

// Patterns that are explicitly mismatched/forbidden for Decrease adjustments
const DECREASE_FORBIDDEN_PATTERNS = [
  {
    regex: /\b(?:low stock)\b/i,
    message: "Reason 'low stock' is invalid for Decrease adjustment. Decrease adjustments are for reductions (e.g., damage, expiry, shrinkage).",
  },
  {
    regex: /\b(?:high stock|overstock|excess stock)\b/i,
    message: "Reason 'high stock' is invalid for Decrease adjustment. Please use a specific reduction reason like damage, expiry, shrinkage, or correction.",
  },
  {
    regex: /\b(?:new purchase|purchase order|vendor delivery|goods receipt)\b/i,
    message: "New purchase adds inventory and cannot be used as a reason for a Decrease adjustment.",
  },
  {
    regex: /\b(?:found inventory|found stock|found items|excess found|surplus|received extra|restock|replenish)\b/i,
    message: "Found inventory or surplus adds stock and cannot be used for a Decrease adjustment.",
  },
  {
    regex: /\b(?:inward discrepancy)\b/i,
    message: "Inward discrepancy represents excess receipt and cannot be used for a Decrease adjustment.",
  },
]

// Keywords indicating valid reasons for Decrease adjustments
const DECREASE_VALID_KEYWORDS = [
  'damage',
  'damaged',
  'expiry',
  'expired',
  'shrinkage',
  'theft',
  'stolen',
  'pilferage',
  'loss',
  'lost',
  'broken',
  'breakage',
  'crushed',
  'spoilage',
  'spoiled',
  'rotten',
  'decay',
  'waste',
  'scrap',
  'scrapped',
  'write-off',
  'write off',
  'written off',
  'correction',
  'correct',
  'deficit',
  'shortage',
  'outward discrepancy',
  'sample',
  'testing',
  'internal consumption',
  'recount',
  'reconciliation',
  'reduction',
  'return to supplier',
  'supplier return',
  'defect',
  'defective',
]

// Recount keywords
const RECOUNT_VALID_KEYWORDS = [
  'count',
  'physical count',
  'cycle count',
  'audit',
  'reconciliation',
  'correction',
  'correct',
  'discrepancy',
  'recount',
  'verification',
  'inventory check',
  'stock audit',
  'variance',
]

const RECOUNT_FORBIDDEN_PATTERNS = [
  {
    regex: /\b(?:high stock)\b/i,
    message: "Reason 'high stock' is invalid for Recount. Recount adjustments must reflect physical count reconciliation or audit variance.",
  },
  {
    regex: /\b(?:new purchase)\b/i,
    message: "New purchases should be recorded through Goods Receipts or Purchase Orders, not Recount.",
  },
]

/**
 * Validates a stock adjustment reason based on the selected adjustment type.
 * @param {string} reason - The reason entered by the user.
 * @param {string} adjustmentType - 'increase', 'decrease', or 'recount'.
 * @returns {string} Error message if invalid, or empty string if valid.
 */
export function validateStockAdjustmentReason(reason, adjustmentType = 'increase') {
  if (!reason || !String(reason).trim()) {
    return 'Reason is required.'
  }

  const rawReason = String(reason).trim()
  const cleanReason = rawReason.toLowerCase()
  const cleanType = String(adjustmentType || 'increase').trim().toLowerCase()

  if (cleanReason.length < 3) {
    return 'Reason must be at least 3 characters.'
  }

  // Check for special characters / symbols only (must contain at least 2 alphanumeric characters)
  const alphaNumericMatches = rawReason.match(/[a-zA-Z0-9]/g)
  if (!alphaNumericMatches || alphaNumericMatches.length < 2) {
    return 'Reason must contain valid words or text (e.g. Low stock, Damage, Correction).'
  }

  if (cleanType === 'increase') {
    // Check forbidden/contradictory patterns first
    for (const item of INCREASE_FORBIDDEN_PATTERNS) {
      if (item.regex.test(cleanReason)) {
        return item.message
      }
    }

    // Check if reason contains any recognized valid term
    const matchesValid = INCREASE_VALID_KEYWORDS.some((kw) => cleanReason.includes(kw))
    if (!matchesValid) {
      return 'Invalid reason for Increase adjustment. Valid reasons include: low stock, new purchase, correction, found inventory, or audit surplus.'
    }

    return ''
  }

  if (cleanType === 'decrease') {
    // Check forbidden/contradictory patterns first
    for (const item of DECREASE_FORBIDDEN_PATTERNS) {
      if (item.regex.test(cleanReason)) {
        return item.message
      }
    }

    // Check if reason contains any recognized valid term
    const matchesValid = DECREASE_VALID_KEYWORDS.some((kw) => cleanReason.includes(kw))
    if (!matchesValid) {
      return 'Invalid reason for Decrease adjustment. Valid reasons include: damage, expiry, shrinkage, theft, loss, breakage, or correction.'
    }

    return ''
  }

  if (cleanType === 'recount') {
    for (const item of RECOUNT_FORBIDDEN_PATTERNS) {
      if (item.regex.test(cleanReason)) {
        return item.message
      }
    }

    const matchesValid = RECOUNT_VALID_KEYWORDS.some((kw) => cleanReason.includes(kw))
    if (!matchesValid) {
      return 'Invalid reason for Recount adjustment. Valid reasons include: physical count reconciliation, cycle count, or audit correction.'
    }

    return ''
  }

  return ''
}

/**
 * Returns suggested preset reasons for the given adjustment type.
 */
export function getSuggestedReasons(adjustmentType = 'increase') {
  const cleanType = String(adjustmentType || 'increase').toLowerCase()
  return REASON_PRESETS[cleanType] || REASON_PRESETS.increase
}

/**
 * Returns helper guidance text for the given adjustment type.
 */
export function getReasonHelperText(adjustmentType = 'increase') {
  const cleanType = String(adjustmentType || 'increase').toLowerCase()
  if (cleanType === 'decrease') {
    return 'Allowed reasons: damage, expiry, shrinkage, theft, loss, breakage, or correction.'
  }
  if (cleanType === 'recount') {
    return 'Allowed reasons: physical count reconciliation, cycle count, or audit correction.'
  }
  return 'Allowed reasons: low stock, new purchase, correction, found inventory, or audit surplus.'
}

/**
 * Returns placeholder text for the given adjustment type.
 */
export function getReasonPlaceholder(adjustmentType = 'increase') {
  const cleanType = String(adjustmentType || 'increase').toLowerCase()
  if (cleanType === 'decrease') {
    return 'e.g., Damage during transit, Expiry of batch, Shrinkage, Correction'
  }
  if (cleanType === 'recount') {
    return 'e.g., Physical count reconciliation, Cycle count discrepancy, Correction'
  }
  return 'e.g., Low stock replenishment, New purchase, Inventory correction, Found stock'
}

/**
 * Discrete units (Pcs, Unit, Box, Pack, Set, Pair, Nos, Item, etc.) do NOT allow fractions.
 */
const DISCRETE_UNIT_PATTERNS = [
  /\b(?:pc|pcs|piece|pieces|unit|units|box|boxes|bag|bags|pack|packs|packet|packets|set|sets|pair|pairs|nos|no|item|items|carton|cartons|bundle|bundles|roll|rolls|drum|drums|bottle|bottles|can|cans|barrel|barrels|strip|strips)\b/i,
]

/**
 * Known continuous / decimal-supporting units (weight, volume, length, area, etc.)
 */
const FRACTIONAL_UNIT_PATTERNS = [
  /\b(?:kg|kilo|kilogram|kilograms)\b/i,
  /\b(?:g|gm|gram|grams)\b/i,
  /\b(?:mg|milligram|milligrams)\b/i,
  /\b(?:lb|lbs|pound|pounds)\b/i,
  /\b(?:oz|ounce|ounces)\b/i,
  /\b(?:t|ton|tons|tonne|tonnes|quintal|qtl)\b/i,
  /\b(?:l|ltr|liter|liters|litre|litres)\b/i,
  /\b(?:ml|milliliter|milliliters|millilitre|millitres|cc)\b/i,
  /\b(?:gal|gallon|gallons)\b/i,
  /\b(?:m|meter|meters|metre|metres)\b/i,
  /\b(?:cm|centimeter|centimeters)\b/i,
  /\b(?:mm|millimeter|millimeters)\b/i,
  /\b(?:ft|feet|foot)\b/i,
  /\b(?:in|inch|inches)\b/i,
  /\b(?:yd|yard|yards)\b/i,
  /\b(?:sqm|sqft|sqin|cum|cuft|cu\.m|sq\.m|sq\.ft)\b/i,
]

/**
 * Checks whether a given unit permits fractional/decimal quantities.
 * Discrete units (Pcs, Unit, Box, Pack, Set, Pair, Nos, Item, etc.) do NOT allow fractions.
 * Weight/volume/length units (kg, g, l, ml, m, etc.) allow fractions.
 */
export function isFractionalUnitAllowed(unitName, unitShortName) {
  const name = String(unitName || '').trim().toLowerCase()
  const short = String(unitShortName || '').trim().toLowerCase()

  if (!name && !short) {
    return false // Default: unit-based / discrete
  }

  // Check discrete patterns first: if it matches a known discrete unit, fractions are forbidden
  const isDiscrete = DISCRETE_UNIT_PATTERNS.some(
    (pattern) => pattern.test(name) || pattern.test(short),
  )
  if (isDiscrete) {
    return false
  }

  // Check continuous patterns
  return FRACTIONAL_UNIT_PATTERNS.some(
    (pattern) => pattern.test(name) || pattern.test(short),
  )
}

/**
 * Validates quantity for Stock Adjustment based on product unit of measure.
 * Unit-based products (e.g. Pcs, Units, Boxes, Water Pump) must be whole numbers.
 * Weight/volume-based products (e.g. kg, g, l, ml) allow decimals.
 */
export function validateStockAdjustmentQuantity(quantity, product, unit, { required = false } = {}) {
  if (quantity === undefined || quantity === null || String(quantity).trim() === '') {
    return required ? 'Quantity is required.' : ''
  }

  const num = Number(quantity)
  if (Number.isNaN(num) || !Number.isFinite(num)) {
    return 'Quantity must be a valid number.'
  }

  if (num <= 0) {
    return 'Quantity must be a positive number greater than 0.'
  }

  const unitName =
    unit?.name ||
    unit?.unitName ||
    unit?.Name ||
    product?.unitName ||
    product?.UnitName ||
    product?.unit_name ||
    (typeof product?.unit === 'string' ? product?.unit : product?.unit?.name || product?.unit?.Name) ||
    ''

  const unitShortName =
    unit?.shortName ||
    unit?.ShortName ||
    unit?.abbreviation ||
    unit?.unitShortName ||
    product?.unitShortName ||
    product?.UnitShortName ||
    product?.shortName ||
    (typeof product?.unit === 'object' ? product?.unit?.shortName || product?.unit?.ShortName : '') ||
    ''

  const allowDecimal = isFractionalUnitAllowed(unitName, unitShortName)

  if (!allowDecimal && !Number.isInteger(num)) {
    const displayUnit = unitShortName || unitName || 'unit'
    const productName = product?.name || product?.productName || product?.Name || 'this product'
    return `Quantity for '${productName}' must be a whole number for unit '${displayUnit}'. Fractional quantities (e.g. ${quantity}) are not allowed.`
  }

  return ''
}

