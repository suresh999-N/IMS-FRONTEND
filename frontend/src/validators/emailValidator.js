export const EMAIL_MAX_LENGTH = 150

const VALID_TLDS = new Set([
  'com', 'org', 'net', 'edu', 'gov', 'mil', 'int', 'info', 'biz', 'co', 'in', 'io', 'ai',
  'app', 'dev', 'tech', 'store', 'online', 'site', 'xyz', 'me', 'tv', 'cc', 'mobi', 'asia',
  'name', 'pro', 'tel', 'travel', 'museum', 'uk', 'us', 'ca', 'de', 'fr', 'jp', 'cn', 'nl',
  'se', 'no', 'fi', 'es', 'it', 'ru', 'mx', 'br', 'za', 'sg', 'hk', 'tw', 'kr', 'nz', 'ch',
  'at', 'be', 'dk', 'pl', 'pt', 'cz', 'ro', 'gr', 'hu', 'ie', 'il', 'my', 'ph', 'th', 'vn',
  'id', 'ae', 'sa', 'cl', 'ar', 'pe', 'cloud', 'digital', 'email', 'group', 'help', 'global',
  'life', 'live', 'link', 'media', 'news', 'space', 'today', 'world', 'works', 'zone',
  'design', 'studio', 'agency', 'solutions', 'services', 'systems', 'network', 'company',
  'management', 'center', 'directory', 'shop', 'blog', 'club', 'fun', 'icu', 'one', 'top',
  'vip', 'work', 'fit', 'art', 'law', 'pub', 'bar', 'ink', 'win', 'bid', 'cam', 'run', 'red',
  'ren', 'kim', 'mom', 'men', 'dad', 'day', 'fan', 'foo', 'gop', 'how', 'moe', 'new', 'now',
  'ooo', 'owl', 'rip', 'sky', 'tax', 'tea', 'uno', 'wtf', 'zip', 'berlin', 'london', 'nyc',
  'tokyo', 'paris', 'amsterdam', 'software', 'technology', 'systems', 'academy', 'education',
  'foundation', 'institute', 'international', 'organization'
])

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
  const { required = true, label = 'Email' } = opts
  const raw = String(value ?? '')
  const trimmed = raw.trim()

  if (!trimmed) {
    return required ? `${label} is required.` : ''
  }

  const INVALID_MSG = 'Please enter a valid email address.'

  if (/\s/.test(trimmed) || trimmed.length > EMAIL_MAX_LENGTH) {
    return INVALID_MSG
  }

  if (trimmed.includes('..')) {
    return INVALID_MSG
  }

  const parts = trimmed.split('@')
  if (parts.length !== 2) {
    return INVALID_MSG
  }

  const [localPart, domainPart] = parts
  if (!localPart || !domainPart) {
    return INVALID_MSG
  }

  if (localPart.length < 1 || localPart.length > 64) {
    return INVALID_MSG
  }

  if (localPart.startsWith('.') || localPart.endsWith('.')) {
    return INVALID_MSG
  }

  if (!/^[a-z0-9._%+-]+$/i.test(localPart)) {
    return INVALID_MSG
  }

  if (domainPart.startsWith('.') || domainPart.endsWith('.') || domainPart.startsWith('-') || domainPart.endsWith('-')) {
    return INVALID_MSG
  }

  const domainParts = domainPart.split('.')
  if (domainParts.length < 2) {
    return INVALID_MSG
  }

  for (const part of domainParts) {
    if (!part || part.startsWith('-') || part.endsWith('-') || !/^[a-z0-9-]+$/i.test(part) || part.length > 63) {
      return INVALID_MSG
    }
    // Reject 4 or more repeated identical characters in any domain label (e.g. gmailllllll, commmmmmmmmm)
    if (/([a-z0-9])\1{3,}/i.test(part)) {
      return INVALID_MSG
    }
  }

  const tld = domainParts[domainParts.length - 1].toLowerCase()
  if (!tld || !/^[a-z]+$/i.test(tld) || tld.length < 2) {
    return INVALID_MSG
  }

  if (!VALID_TLDS.has(tld)) {
    return INVALID_MSG
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
