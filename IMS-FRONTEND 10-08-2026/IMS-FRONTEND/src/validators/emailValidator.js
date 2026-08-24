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

  if (localPart.length < 2) {
    return `${label} username must be at least 2 characters.`
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

  if (COMMON_DOMAIN_TYPOS[domainPart]) {
    return `Invalid domain "${domainPart}". Did you mean "${COMMON_DOMAIN_TYPOS[domainPart]}"?`
  }

  const domainParts = domainPart.split('.')
  const mainDomain = domainParts[0]
  const tld = domainParts.slice(1).join('.')

  if (!mainDomain || mainDomain.length < 2) {
    return `Enter a valid ${label.toLowerCase()} domain (e.g., gmail.com, company.in).`
  }

  if (PUBLIC_PROVIDERS.has(mainDomain) && (TYPO_TLDS.has(tld) || !VALID_TLDS.has(tld))) {
    return `Invalid domain "${domainPart}". Did you mean "${mainDomain}.com"?`
  }

  if (domainParts.length < 2 || !tld || !VALID_TLDS.has(tld)) {
    return `Enter a valid ${label.toLowerCase()} extension (e.g., .com, .in, .org, .net).`
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
