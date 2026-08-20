export const EMAIL_MAX_LENGTH = 254

const EMAIL_DOMAIN_PATTERN = /^(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]$/i

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
  const { required = false, label = 'Email' } = options
  const email = sanitizeEmailInput(value)

  if (!email) {
    return required ? `${label} is required.` : ''
  }

  if (email.length > EMAIL_MAX_LENGTH) {
    return 'Email cannot exceed 254 characters.'
  }

  if (email.includes('..')) {
    return 'Email cannot contain consecutive periods.'
  }

  const parts = email.split('@')
  if (parts.length !== 2) {
    return 'Enter a valid email address.'
  }

  const [localPart, domainPart] = parts
  if (!localPart || !domainPart) {
    return 'Enter a valid email address.'
  }

  if (localPart.length > 64) {
    return 'Email local part cannot exceed 64 characters.'
  }

  if (!/[a-z0-9]/i.test(localPart)) {
    return 'Email username must contain letters or numbers.'
  }

  if (localPart.startsWith('.') || localPart.endsWith('.')) {
    return 'Email username cannot start or end with a period.'
  }

  if (!/^[a-z0-9._%+-]+$/i.test(localPart)) {
    return 'Email contains invalid characters.'
  }

  if (domainPart.startsWith('.') || domainPart.endsWith('.')) {
    return 'Enter a valid email domain.'
  }

  return EMAIL_DOMAIN_PATTERN.test(domainPart) ? '' : 'Enter a valid email domain.'
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
