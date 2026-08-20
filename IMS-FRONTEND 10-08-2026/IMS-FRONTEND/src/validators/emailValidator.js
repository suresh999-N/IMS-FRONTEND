export const EMAIL_MAX_LENGTH = 254

const KEYBOARD_WALK_PATTERNS = [
  'qwerty', 'asdfgh', 'zxcvbn', '123456', '234567', '345678', '456789', '567890',
  'qwer', 'asdf', 'zxcv', 'hjkl', 'yuiop'
]

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

function hasGibberishLocalPart(localPart) {
  const clean = localPart.toLowerCase().replace(/[^a-z]/g, '')
  if (clean.length > 8) {
    const vowels = clean.match(/[aeiou]/g)
    if (!vowels || vowels.length === 0) {
      return true
    }
    if (/[bcdfghjklmnpqrstvwxyz]{8,}/.test(clean)) {
      return true
    }
  }

  const lower = localPart.toLowerCase()
  return KEYBOARD_WALK_PATTERNS.some((pattern) => lower.includes(pattern))
}

export function sanitizeEmailInput(value) {
  return stripUnsafeText(value)
    .normalize('NFKC')
    .replace(/[<>]/g, '')
    .toLowerCase()
    .replace(/\s/g, '')
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

  if (/([!#$%&'*+/=?^_`{|}~.-])\1{2,}/.test(email)) {
    return 'Email contains repeated symbols.'
  }

  const parts = email.split('@')
  if (parts.length !== 2) {
    return 'Enter a valid email address (e.g. name@domain.com).'
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

  if (hasGibberishLocalPart(localPart)) {
    return 'Please enter a valid email address.'
  }

  // Domain & TLD validation
  const domainParts = domainPart.split('.')
  if (domainParts.length < 2) {
    return 'Email must include a valid domain extension (e.g. .com, .org).'
  }

  const tld = domainParts[domainParts.length - 1].toLowerCase()
  if (tld.length < 2 || tld.length > 24) {
    return 'Email contains an invalid domain extension.'
  }

  if (!/^[a-z]+$/.test(tld)) {
    return 'Email domain extension must contain only letters.'
  }

  const DOMAIN_REGEX = /^[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)*\.[a-z]{2,24}$/i
  if (!DOMAIN_REGEX.test(domainPart)) {
    return 'Enter a valid email domain name.'
  }

  return ''
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
