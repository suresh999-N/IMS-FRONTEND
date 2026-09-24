/**
 * Role Name format and validity validator
 */

export const ROLE_NAME_MAX_LENGTH = 50
export const ROLE_NAME_MIN_LENGTH = 2

// Common keyboard mash patterns (home-row and sequence walks)
const KEYBOARD_MASH_PATTERNS = [
  'qwerty', 'asdfgh', 'zxcvbn', 'qwertz', 'azerty',
  'qwer', 'wert', 'erty', 'rtyu', 'tyui', 'yuio', 'uiop',
  'asdf', 'sdfg', 'dfgh', 'fghj', 'ghjk', 'hjkl',
  'zxcv', 'xcvb', 'cvbn', 'vbnm',
  'abcd', 'bcde', 'cdef', 'defg', 'efgh', 'fghi', 'ghij',
  'yuiop', 'ghjkl', 'fghjk', 'xcvbn',
  'hjk', 'kuj', 'jkuj', 'ujk', 'fhfj', 'dfjk', 'jkjf', 'sdkj',
  'jfdk', 'fdkj', 'difj', 'ksjf', 'slkf', 'hgsd', 'gsdi', 'sdif',
  'ifjh', 'fjhk', 'jhkj', 'hkjf', 'kjfd', 'dkjf', 'kjfh',
  'hjsh', 'jshg', 'shgd', 'hgdk', 'gdkj', 'dkja', 'kjaf', 'jafa',
]

// Known valid uppercase/business acronyms that don't need vowels
const VALID_ACRONYMS = new Set([
  'hr', 'it', 'pr', 'qa', 'qc', 'vp', 'ceo', 'cfo', 'coo', 'cto',
  'cio', 'cmo', 'gm', 'pm', 'md', 'am', 'l1', 'l2', 'l3', 'devops',
])

// Generic or placeholder test role names
const MEANINGLESS_ROLE_NAMES = new Set([
  'test', 'testing', 'tester', 'dummy', 'sample', 'temp', 'fake',
  'trial', 'demo', 'foo', 'bar', 'baz', 'asdf', 'qwerty', 'admin123',
  'testrole', 'role', 'random', 'check', 'checking', 'placeholder',
  'qwertyuiop', 'asdfghjkl', 'zxcvbnm',
])

/**
 * Check if the text is random gibberish, key smashing, or an invalid sequence
 */
export function isGibberishRoleName(text) {
  if (!text || typeof text !== 'string') return false
  const trimmed = text.trim().toLowerCase()

  // 0. Meaningless placeholder words
  if (MEANINGLESS_ROLE_NAMES.has(trimmed) || MEANINGLESS_ROLE_NAMES.has(trimmed.replace(/\s+/g, ''))) {
    return true
  }

  // 1. 3 or more identical consecutive characters (e.g. "aaaa", "hhhh")
  if (/(.)\1{2,}/i.test(trimmed)) {
    return true
  }

  // 2. Repeated short patterns (e.g. "asdfasdf", "ababab", "xyzxyz")
  if (/(.{2,4})\1{2,}/i.test(trimmed)) {
    return true
  }

  // 3. Double-pair key smash (e.g. "gghh", "aass", "ddff", "hhkk")
  if (/([a-z])\1([a-z])\2/i.test(trimmed)) {
    return true
  }

  // 4. 5 or more consecutive consonants (e.g. "fhgsd", "fjhkjfdkjfh", "hgjshgd")
  if (/[bcdfghjklmnpqrstvwxyz]{5,}/i.test(trimmed)) {
    return true
  }

  // 5. Repeated keyboard sequences
  if (KEYBOARD_MASH_PATTERNS.some((pattern) => trimmed.includes(pattern))) {
    return true
  }

  // 6. Word-level analysis
  const words = trimmed.split(/[\s\-/&()]+/).filter(Boolean)
  for (const word of words) {
    const lettersOnly = word.replace(/[^a-z]/gi, '')

    // Words of 3+ letters with 0 vowels unless recognized acronym
    if (lettersOnly.length >= 3 && !/[aeiouy]/i.test(lettersOnly) && !VALID_ACRONYMS.has(lettersOnly)) {
      return true
    }

    // Words of 5+ characters with 0 vowels
    if (lettersOnly.length >= 5 && !/[aeiouy]/i.test(lettersOnly)) {
      return true
    }

    // Words of 7+ characters with < 20% vowels (e.g. "fhgsdifjhkjfdkjfh" has 2/17 = 11.7%)
    const vowels = lettersOnly.match(/[aeiouy]/gi) || []
    if (lettersOnly.length >= 7 && vowels.length / lettersOnly.length < 0.20) {
      return true
    }

    // High home-row concentration in words >= 7 letters (mashing a,s,d,f,g,h,j,k,l)
    const homeRowKeys = lettersOnly.match(/[asdfghjkl]/gi) || []
    if (lettersOnly.length >= 7 && homeRowKeys.length / lettersOnly.length >= 0.85 && vowels.length <= 2) {
      return true
    }

    // Reject single words longer than 15 characters
    if (lettersOnly.length > 15) {
      return true
    }
  }

  return false
}

/**
 * Sanitize role name input
 */
export function sanitizeRoleInput(value, maxLength = ROLE_NAME_MAX_LENGTH) {
  if (typeof value !== 'string') return ''
  return value
    .replace(/[<>]/g, '')
    .slice(0, maxLength)
}

/**
 * Validate role name and return an error message, or empty string if valid
 */
export function getRoleNameError(value, options = {}) {
  const opts = typeof options === 'string' ? { label: options } : options
  const {
    required = true,
    label = 'Role Name',
    min = ROLE_NAME_MIN_LENGTH,
    max = ROLE_NAME_MAX_LENGTH,
  } = opts

  const rawValue = String(value ?? '')
  const cleanValue = rawValue.trim()

  if (!cleanValue) {
    return required ? `${label} is required.` : ''
  }

  if (cleanValue.length < min) {
    return `${label} must be at least ${min} characters.`
  }

  if (cleanValue.length > max) {
    return `${label} cannot exceed ${max} characters.`
  }

  // Must start with a letter
  if (!/^[a-zA-Z]/.test(cleanValue)) {
    return `${label} must start with a letter.`
  }

  // Must only contain letters, numbers, spaces, and supported punctuation (&, /, -, (, ))
  if (!/^[a-zA-Z0-9\s&/\-().]+$/.test(cleanValue)) {
    return `${label} contains invalid characters. Only letters, numbers, spaces, and hyphens are allowed.`
  }

  // Must contain letters, cannot be numbers only
  if (!/[a-zA-Z]/.test(cleanValue)) {
    return `${label} cannot contain numbers only.`
  }

  // Cannot end with a symbol or hyphen
  if (/[-&/(]$/.test(cleanValue)) {
    return `${label} cannot end with a symbol or hyphen.`
  }

  // Reject consecutive punctuation marks (e.g. "--", "//", "&&")
  if (/[-&/.()]{2,}/.test(cleanValue)) {
    return `${label} contains invalid punctuation patterns.`
  }

  // Reject 3 or more consecutive identical characters (e.g. "aaa", "111")
  if (/(.)\1{2,}/i.test(cleanValue)) {
    return `${label} contains invalid repeated characters.`
  }

  // Reject single words longer than 15 characters
  const words = cleanValue.split(/[\s\-/&()]+/).filter(Boolean)
  if (words.some((word) => word.length > 15)) {
    return `${label} cannot contain words longer than 15 characters.`
  }

  // Reject random character strings / keyboard mashing
  if (isGibberishRoleName(cleanValue)) {
    return 'Please enter a valid business role name.'
  }

  return ''
}

export function isValidRoleName(value, options = {}) {
  return !getRoleNameError(value, options)
}
