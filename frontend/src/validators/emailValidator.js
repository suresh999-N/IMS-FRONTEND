export const EMAIL_MAX_LENGTH = 254

const VALID_TLDS = new Set([
  'com', 'in', 'org', 'net', 'edu', 'gov', 'io', 'co', 'info', 'biz', 'tech',
  'app', 'dev', 'store', 'online', 'me', 'site', 'ca', 'uk', 'au', 'us', 'de',
  'fr', 'jp', 'sg', 'ae', 'cn', 'ru', 'br', 'nl', 'se', 'no', 'fi', 'dk', 'pl',
  'it', 'es', 'mx', 'za', 'nz', 'ch', 'at', 'be', 'ph', 'id', 'my', 'th', 'vn',
  'live', 'cloud', 'digital', 'global', 'systems', 'solutions', 'agency', 'group',
  'services', 'co.in', 'net.in', 'org.in', 'edu.in', 'gov.in', 'ac.in', 'co.uk',
  'com.au', 'co.jp', 'or.jp', 'ne.jp', 'ac.uk', 'gov.uk'
])

const COMMON_DOMAIN_TYPOS = {
  'gmail.cm': 'gmail.com',
  'gmail.co': 'gmail.com',
  'gmail.commm': 'gmail.com',
  'gmail.comm': 'gmail.com',
  'gmail.coom': 'gmail.com',
  'gmai.com': 'gmail.com',
  'gmai.co': 'gmail.com',
  'gamil.com': 'gmail.com',
  'gamil.co': 'gmail.com',
  'gmial.com': 'gmail.com',
  'gmial.co': 'gmail.com',
  'gmaill.com': 'gmail.com',
  'yahoo.cm': 'yahoo.com',
  'yahoo.co': 'yahoo.com',
  'yaho.com': 'yahoo.com',
  'hotmail.cm': 'hotmail.com',
  'hotmail.co': 'hotmail.com',
  'hotmial.com': 'hotmail.com',
  'outlook.cm': 'outlook.com',
  'outlook.co': 'outlook.com',
  'outlok.com': 'outlook.com',
}

const PUBLIC_PROVIDERS = new Set([
  'gmail', 'yahoo', 'hotmail', 'outlook', 'icloud', 'rediffmail', 'live', 'aol', 'msn', 'ymail', 'protonmail', 'zoho'
])

const TYPO_TLDS = new Set(['co', 'cm', 'c', 'coom', 'comm', 'commm', 'con', 'cmm', 'gma', 'gmai'])

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
  const { required = true, label = 'Email address' } = opts
  const raw = String(value ?? '')
  const trimmed = raw.trim()

  if (!trimmed) {
    return required ? `${label} is required.` : ''
  }

  if (/\s/.test(trimmed) || trimmed.length > EMAIL_MAX_LENGTH) {
    return 'Enter a valid email address.'
  }

  if (trimmed.includes('..')) {
    return 'Enter a valid email address.'
  }

  const parts = trimmed.split('@')
  if (parts.length !== 2) {
    return 'Enter a valid email address.'
  }

  const [localPart, domainPart] = parts
  if (!localPart || !domainPart) {
    return 'Enter a valid email address.'
  }

  if (localPart.length < 1 || localPart.length > 64) {
    return 'Enter a valid email address.'
  }

  if (localPart.startsWith('.') || localPart.endsWith('.')) {
    return 'Enter a valid email address.'
  }

  if (!/^[a-z0-9._%+-]+$/i.test(localPart)) {
    return 'Enter a valid email address.'
  }

  if (domainPart.startsWith('.') || domainPart.endsWith('.') || domainPart.startsWith('-') || domainPart.endsWith('-')) {
    return 'Enter a valid email address.'
  }

  if (COMMON_DOMAIN_TYPOS[domainPart.toLowerCase()]) {
    return `Invalid domain "${domainPart}". Did you mean "${COMMON_DOMAIN_TYPOS[domainPart.toLowerCase()]}"?`
  }

  const domainParts = domainPart.split('.')
  if (domainParts.length < 2) {
    return 'Enter a valid email address.'
  }

  for (const part of domainParts) {
    if (!part || part.startsWith('-') || part.endsWith('-') || !/^[a-z0-9-]+$/i.test(part) || part.length > 63) {
      return 'Enter a valid email address.'
    }
  }

  const tld = domainParts[domainParts.length - 1]
  if (!tld || !/^[a-z]+$/i.test(tld) || tld.length < 2 || tld.length > 24) {
    return 'Enter a valid email address.'
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
