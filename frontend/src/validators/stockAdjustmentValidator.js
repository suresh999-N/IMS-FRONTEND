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
    return '' // Let required validation handle empty check if needed
  }

  const cleanReason = String(reason).trim().toLowerCase()
  const cleanType = String(adjustmentType || 'increase').trim().toLowerCase()

  if (cleanReason.length < 3) {
    return 'Reason must be at least 3 characters.'
  }

  if (cleanType === 'increase') {
    // Check forbidden/mismatched patterns first
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
    // Check forbidden/mismatched patterns first
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
