export const EMAIL_MAX_LENGTH = 254

const STRICT_EMAIL_PATTERN = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

function stripUnsafeText(value) {
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

export function sanitizeEmailInput(value) {
  return stripUnsafeText(value)
    .normalize('NFKC')
    .replace(/[<>]/g, '')
    .toLowerCase()
    .trim()
    .slice(0, EMAIL_MAX_LENGTH)
}

export function getEmailError(value, options = {}) {
  const opts = typeof options === 'string' ? { label: options } : options
  const { required = false, label = 'Email' } = opts
  const email = sanitizeEmailInput(value)

  if (!email) {
    return required ? `${label} is required.` : ''
  }

  if (email.length > EMAIL_MAX_LENGTH) {
    return `${label} cannot exceed 254 characters.`
  }

  if (email.includes('..')) {
    return `${label} cannot contain consecutive periods.`
  }

  const parts = email.split('@')
  if (parts.length !== 2) {
    return `Enter a valid ${label.toLowerCase()} address (e.g., name@example.com).`
  }

  const [localPart, domainPart] = parts
  if (!localPart || !domainPart) {
    return `Enter a valid ${label.toLowerCase()} address (e.g., name@example.com).`
  }

  if (localPart.length > 64) {
    return `${label} username cannot exceed 64 characters.`
  }

  if (!/[a-z0-9]/i.test(localPart)) {
    return `${label} username must contain letters or numbers.`
  }

  if (localPart.startsWith('.') || localPart.endsWith('.')) {
    return `${label} username cannot start or end with a period.`
  }

  if (!/^[a-z0-9._%+-]+$/i.test(localPart)) {
    return `${label} contains invalid characters.`
  }

  if (domainPart.startsWith('.') || domainPart.endsWith('.')) {
    return `Enter a valid ${label.toLowerCase()} domain.`
  }

  const domainParts = domainPart.split('.')
  const tld = domainParts[domainParts.length - 1]

  if (domainParts.length < 2 || !tld || tld.length < 2 || !/^[a-z]{2,}$/i.test(tld)) {
    return `Enter a valid ${label.toLowerCase()} domain with a valid extension (e.g., .com, .in, .org).`
  }

  return STRICT_EMAIL_PATTERN.test(email) ? '' : `Enter a valid ${label.toLowerCase()} address.`
}

export function isValidEmail(value, options = {}) {
  return !getEmailError(value, options)
}

export const emailInputProps = {
  type: 'email',
  inputMode: 'email',
  maxLength: EMAIL_MAX_LENGTH,
  autoComplete: 'email',
  spellCheck: false,
}
