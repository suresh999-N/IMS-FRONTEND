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
  const { required = true, label = 'Name', min = 2, max = NAME_MAX_LENGTH, allowAmpersand = false } = opts
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

  // Reject numeric-only strings and strings without alphabetic characters
  if (/^\d+$/.test(cleanValue) || !/[A-Za-z]/.test(cleanValue)) {
    return `${label} must contain alphabetic characters and cannot contain only numbers.`
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
  const pattern = allowAmpersand 
    ? /^(?=.*[\p{L}a-zA-Z])[\p{L}a-zA-Z0-9 .&'-]+$/u
    : /^(?=.*[\p{L}a-zA-Z])[\p{L}a-zA-Z .'-]+$/u

  if (!pattern.test(cleanValue)) {
    return `${label} can contain letters, spaces, hyphens, and apostrophes only.`
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
