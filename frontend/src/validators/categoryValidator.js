/**
 * Category Name format and validity validator
 */

export const VALID_CATEGORY_ACRONYMS = new Set([
  'IT', 'PC', 'TV', 'AC', 'DC', 'RO', 'HD', 'FHD', 'UHD', '4K', '8K', '2D', '3D', '4G', '5G',
  'POS', 'CCTV', 'GPS', 'SKU', 'SIM', 'VIP', 'LED', 'LCD', 'USB', 'RAM', 'SSD', 'HDD', 'CPU', 'GPU',
  'DVD', 'CD', 'VCR', 'FM', 'AM', 'RF', 'NFC', 'RFID', 'VGA', 'DVI', 'HDMI', 'LAN', 'WAN', 'WIFI',
  'PVC', 'ABS', 'MS', 'SS', 'GI', 'TMT', 'UPVC', 'CPVC', 'PPR', 'HDPE', 'LDPE', 'PPE',
  'OTG', 'OEM', 'ODM', 'DIY', 'AI', 'IOT', 'EV', 'AV', 'DJ', 'PA', 'UPS', 'SMPS', 'PCB',
])

export const VALID_SHORT_NUM_PREFIX = new Set([
  'D', 'K', 'G', 'P', 'V', 'W', 'M', 'L', 'B', 'T', 'HZ', 'GHZ', 'MHZ', 'MP',
])

export const KEYBOARD_PATTERNS = [
  'qwerty', 'qwert', 'werty', 'asdfgh', 'asdfg', 'asdf', 'sdfg', 'dfgh', 'fghj', 'ghjk', 'hjkl',
  'zxcvbn', 'zxcvb', 'zxcv', 'xcvbn', 'cvbnm', 'yuiop', 'ghjkl', 'fghjk',
  'ytrewq', 'gfdsa', 'lkjhg', 'lkjh', 'kjhg', 'jhgf', 'hgfd', 'gfds', 'fdsa', 'mnbvc', 'nbvcx', 'bvcxz',
  '12345', '23456', '34567', '45678', '56789', '67890', '09876', '54321', '43210',
]

export function isGibberishOrKeySmash(text) {
  if (!text || typeof text !== 'string') return false
  const trimmed = text.trim()
  if (!trimmed) return false

  // 1. 3 or more identical consecutive characters (e.g. "aaaa", "hhhh", "111")
  if (/(.)\1{2,}/i.test(trimmed)) {
    return true
  }

  // 2. Repeated character blocks/pairs (e.g. "asdfasdf", "ababab", "gghh")
  if (/(.{2,4})\1{2,}/i.test(trimmed) || /([a-z])\1([a-z])\2/i.test(trimmed.toLowerCase())) {
    return true
  }

  // 3. Repeated keyboard sequences or home-row patterns
  const lower = trimmed.toLowerCase()
  if (KEYBOARD_PATTERNS.some((pattern) => lower.includes(pattern))) {
    return true
  }

  // 4. Mixed digits and letters alternating like 1a2b3c
  if (/([a-z]\d[a-z]\d|\d[a-z]\d[a-z])/i.test(trimmed)) {
    return true
  }

  // 5. Inspect individual tokens/words
  const words = trimmed.split(/[\s\-/&,.]+/).filter(Boolean)
  let hasMeaningfulWord = false

  for (const word of words) {
    const lettersOnly = word.replace(/[^a-zA-Z]/g, '')
    const upperWord = word.toUpperCase()

    // Pure number tokens (e.g. "1" in "Grade 1" or "2026")
    if (lettersOnly.length === 0) {
      continue
    }

    // Recognized uppercase acronyms (e.g. "POS", "IT", "CCTV", "USB")
    if (VALID_CATEGORY_ACRONYMS.has(upperWord) || VALID_CATEGORY_ACRONYMS.has(lettersOnly.toUpperCase())) {
      hasMeaningfulWord = true
      continue
    }

    // Digits immediately concatenated with letters (e.g. "45xjdg", "12xyz")
    const numLetterMatch = word.match(/^(\d+)([a-zA-Z]+)$/)
    if (numLetterMatch) {
      const alphaPart = numLetterMatch[2].toUpperCase()
      if (VALID_CATEGORY_ACRONYMS.has(alphaPart) || VALID_SHORT_NUM_PREFIX.has(alphaPart)) {
        hasMeaningfulWord = true
        continue
      }
      return true
    }

    // 5 or more consecutive consonants (e.g. "bcdfgh", "jhjjh")
    if (/[bcdfghjklmnpqrstvwxz]{5,}/i.test(lettersOnly)) {
      return true
    }

    // Words with 2+ letters and NO vowels (and not a recognized acronym)
    if (lettersOnly.length >= 2 && !/[aeiouy]/i.test(lettersOnly)) {
      return true
    }

    // Words >= 7 letters with unusually low vowel ratio (< 15%)
    if (lettersOnly.length >= 7) {
      const vowels = lettersOnly.match(/[aeiouy]/gi) || []
      if (vowels.length / lettersOnly.length < 0.15) {
        return true
      }
    }

    if (lettersOnly.length > 25) {
      return true
    }

    hasMeaningfulWord = true
  }

  return !hasMeaningfulWord
}

export function validateCategoryName(name, existingCategories = [], currentId = null, parentId = undefined) {
  if (!name || typeof name !== 'string' || !name.trim()) {
    return 'Category name is required.'
  }

  const trimmed = name.trim()

  if (trimmed.length < 2) {
    return 'Category name must be at least 2 characters.'
  }

  if (trimmed.length > 50) {
    return 'Category name cannot exceed 50 characters.'
  }

  // Must contain only allowed characters (letters, numbers, spaces, &, /, -, ., ( ), ', ,)
  if (!/^[a-zA-Z0-9\s&/\-().',]+$/.test(trimmed)) {
    return 'Category name can only contain letters, numbers, spaces, and standard punctuation.'
  }

  // Must contain at least one letter
  if (!/[a-zA-Z]/.test(trimmed)) {
    return 'Category name must contain at least one letter.'
  }

  // Check gibberish / key smash / nonsensical input
  if (isGibberishOrKeySmash(trimmed)) {
    return 'Please enter a valid, meaningful category name.'
  }

  // Check duplicate
  if (Array.isArray(existingCategories) && existingCategories.length > 0) {
    const lower = trimmed.toLowerCase()
    const isDuplicate = existingCategories.some((cat) => {
      if (currentId && String(cat.id || cat.categoryId) === String(currentId)) {
        return false
      }
      if (parentId !== undefined && parentId !== null) {
        const catParentId = cat.parentId ?? cat.parentCategoryId ?? null
        const normCatParent = (catParentId === null || catParentId === '' || catParentId === 0 || catParentId === '0') ? '' : String(catParentId)
        const normParent = (parentId === null || parentId === '' || parentId === 0 || parentId === '0') ? '' : String(parentId)
        if (normCatParent !== normParent) {
          return false
        }
      }
      const existingName = (cat.name || cat.label || '').trim().toLowerCase()
      return existingName === lower
    })

    if (isDuplicate) {
      return 'A category with this name already exists under the selected parent.'
    }
  }

  return ''
}
