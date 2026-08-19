export const NAME_MAX_LENGTH = 50

export function stripUnsafeNameText(value) {
  return Array.from(String(value ?? '')).filter((character) => {
    const code = character.charCodeAt(0)
    return !(
      code <= 31 ||
      (code >= 127 && code <= 159) ||
      (code >= 0x200B && code <= 0x200D) ||
      code === 0xFEFF
    )
  }).join('')
}

export function sanitizeNameInput(value, maxLength = NAME_MAX_LENGTH) {
  return stripUnsafeNameText(value)
    .normalize('NFKC')
    .replace(/[<>]/g, '')
    .replace(/^\s+/, '')
    .replace(/\s{2,}/g, ' ')
    .slice(0, maxLength)
}

export function getNameError(value, options = {}) {
  const opts = typeof options === 'string' ? { label: options } : options
  const {
    required = true,
    label = 'Name',
    min = 2,
    max = NAME_MAX_LENGTH,
    allowAmpersand = false,
    allowNumbers = false,
  } = opts
  const cleanValue = sanitizeNameInput(value, max).trim()

  if (!cleanValue) {
    return required ? `${label} is required.` : ''
  }

  if (cleanValue.length < min) {
    return `${label} must be at least ${min} characters.`
  }

  if (cleanValue.length > max) {
    return `${label} cannot exceed ${max} characters.`
  }

  // Reject numeric characters when allowNumbers is false (default for name fields)
  if (!allowNumbers && /\d/.test(cleanValue)) {
    return `${label} must contain alphabetic characters only and cannot contain numbers.`
  }

  // Reject strings without alphabetic characters
  if (!/[A-Za-z]/.test(cleanValue)) {
    return `${label} must contain alphabetic characters.`
  }

  // Reject 3 or more consecutive identical characters (e.g. "aaa")
  if (/([\p{L}a-zA-Z])\1\1/u.test(cleanValue)) {
    return `${label} contains invalid repeated characters.`
  }

  // Reject single words longer than 15 characters (person names don't have single words > 15 chars)
  const words = cleanValue.split(/[\s'-]+/)
  if (words.some(word => word.length > 15)) {
    return `${label} cannot contain words longer than 15 characters.`
  }

  // General valid characters for person names: letters, spaces, hyphens (-), apostrophes ('), periods (.)
  const pattern = (allowNumbers || allowAmpersand)
    ? /^(?=.*[\p{L}a-zA-Z])[\p{L}a-zA-Z0-9 .&'-]+$/u
    : /^(?=.*[\p{L}a-zA-Z])[\p{L}a-zA-Z .'-]+$/u
  if (!pattern.test(cleanValue)) {
    return `${label} can contain letters, spaces, hyphens, and apostrophes only.`
  }

  // Gibberish / keyboard mashing validation
  const lower = cleanValue.toLowerCase()

  // Reject 4 or more repeated identical characters (e.g., "aaaa", "zzzz")
  if (/(.)\1{3,}/i.test(cleanValue)) {
    return `Enter a valid ${label.toLowerCase()}.`
  }

  // Reject repeated character blocks (e.g., "asdfasdf", "ababab")
  if (/(.{2,4})\1{2,}/i.test(cleanValue)) {
    return `Enter a valid ${label.toLowerCase()}.`
  }

  // Keyboard row walks
  const keyboardWalks = ['qwerty', 'asdfgh', 'zxcvbn', 'qwertz', 'azerty', 'yuiop', 'ghjkl', 'fghjk', 'xcvbn']
  if (keyboardWalks.some((walk) => lower.includes(walk))) {
    return `Enter a valid ${label.toLowerCase()}.`
  }

  // Reject words with 5+ characters that have no vowels
  const words = cleanValue.split(/\s+/)
  for (const word of words) {
    const lettersOnly = word.replace(/[^A-Za-z]/g, '')
    if (lettersOnly.length >= 5 && !/[aeiouyAEIOUY]/.test(lettersOnly)) {
      return `Enter a valid ${label.toLowerCase()}.`
    }
  }

  return ''
}

export function isValidName(value, options = {}) {
  return !getNameError(value, options)
}

export const nameInputProps = {
  type: 'text',
  autoComplete: 'name',
  maxLength: NAME_MAX_LENGTH,
}
