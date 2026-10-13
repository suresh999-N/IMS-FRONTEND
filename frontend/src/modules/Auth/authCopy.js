export function getAuthErrorMessage(message, fallback = 'Unable to process your request. Please try again later.') {
  let text = String(message ?? '').trim()

  if (!text) return fallback

  let normalizedText = text.toLowerCase()

  // Handle raw JSON or ProblemDetails objects/strings
  if (
    (text.startsWith('{') && text.endsWith('}')) ||
    (text.startsWith('[') && text.endsWith(']')) ||
    normalizedText.includes('rfc9110') ||
    normalizedText.includes('tools.ietf.org') ||
    normalizedText.includes('traceid') ||
    normalizedText.includes('problemdetails')
  ) {
    try {
      const parsed = typeof message === 'object' && message !== null ? message : JSON.parse(text)
      if (parsed) {
        if (parsed.errors && typeof parsed.errors === 'object') {
          const firstErr = Object.values(parsed.errors).flat().find(Boolean)
          if (firstErr && typeof firstErr === 'string') {
            text = firstErr
            normalizedText = text.toLowerCase()
          }
        } else if (parsed.message) {
          text = parsed.message
          normalizedText = text.toLowerCase()
        } else if (parsed.title && !parsed.title.toLowerCase().includes('validation errors occurred')) {
          text = parsed.title
          normalizedText = text.toLowerCase()
        } else {
          return 'Invalid email or password.'
        }
      }
    } catch {
      return 'Unable to process your request. Please try again later.'
    }
  }

  // Detect technical errors / stack traces / database / internal details
  if (
    text.startsWith('{') ||
    text.startsWith('[') ||
    normalizedText.includes('rfc9110') ||
    normalizedText.includes('traceid') ||
    normalizedText.includes('exception') ||
    normalizedText.includes('stacktrace') ||
    normalizedText.includes('sql') ||
    normalizedText.includes('database') ||
    normalizedText.includes('mysql') ||
    normalizedText.includes('one or more validation errors') ||
    normalizedText.includes('the email field is required') ||
    normalizedText.includes('the emailorphobe field is required') ||
    normalizedText.includes('the password field is required') ||
    normalizedText.includes('dto')
  ) {
    if (normalizedText.includes('email') || normalizedText.includes('password') || normalizedText.includes('validation')) {
      return 'Invalid email or password.'
    }
    return 'Unable to process your request. Please try again later.'
  }

  // Detect server unavailability / network issues
  if (
    normalizedText.includes('service unavailable') ||
    normalizedText.includes('bad gateway') ||
    normalizedText.includes('gateway timeout') ||
    normalizedText.includes('econnrefused') ||
    normalizedText.includes('network') ||
    normalizedText.includes('connect') ||
    normalizedText.includes('failed to fetch') ||
    normalizedText.includes('upstream') ||
    normalizedText.includes('offline')
  ) {
    return 'Service is currently unavailable.'
  }

  // Account lockout
  if (normalizedText.includes('lock') || normalizedText.includes('locked')) {
    if (normalizedText.includes('15 minutes') || normalizedText.includes('attempt')) {
      return text
    }
    return 'Your account has been locked. Please try again later.'
  }

  // Verification requirements
  if (normalizedText.includes('verify') && (normalizedText.includes('email') || normalizedText.includes('mail'))) {
    return 'Please verify your email before logging in.'
  }

  // Account inactive / disabled
  if (
    normalizedText.includes('inactive') ||
    normalizedText.includes('disabled') ||
    normalizedText.includes('blocked')
  ) {
    return 'Your account is inactive. Please contact support.'
  }

  // Credential failures
  if (
    (normalizedText.includes('invalid') && (
      normalizedText.includes('email') ||
      normalizedText.includes('password') ||
      normalizedText.includes('credential') ||
      normalizedText.includes('user') ||
      normalizedText.includes('phone') ||
      normalizedText.includes('login')
    )) ||
    normalizedText.includes('incorrect password') ||
    normalizedText.includes('password is incorrect') ||
    normalizedText.includes('check your credentials') ||
    normalizedText.includes('check your email') ||
    (normalizedText.includes('not found') && (normalizedText.includes('user') || normalizedText.includes('account')))
  ) {
    return 'Invalid email or password.'
  }

  // Password reuse check
  if (
    (normalizedText.includes('same') || normalizedText.includes('reuse') || normalizedText.includes('used') || normalizedText.includes('history') || normalizedText.includes('previous')) &&
    (normalizedText.includes('old') || normalizedText.includes('previous') || normalizedText.includes('recent') || normalizedText.includes('current') || normalizedText.includes('password'))
  ) {
    return 'New password cannot be the same as the current password.'
  }

  // Duplicate accounts
  if (normalizedText.includes('already') || normalizedText.includes('exists') || normalizedText.includes('conflict') || normalizedText.includes('duplicate') || normalizedText.includes('registered')) {
    if (normalizedText.includes('phone') || normalizedText.includes('mobile')) return 'An account with this phone number already exists.'
    return 'An account with this email address already exists.'
  }

  // Specific user-facing prompts
  if (normalizedText.includes('working mail') || normalizedText.includes('working email') || normalizedText.includes('valid working')) {
    return 'Enter a valid email address.'
  }

  if (normalizedText.includes('otp') || normalizedText.includes('6-digit')) {
    return 'Enter the 6-digit code.'
  }

  if (normalizedText.includes('token') || normalizedText.includes('session has expired')) {
    return 'Your session has expired. Please sign in again.'
  }

  if (normalizedText.includes('expired')) {
    return 'Your reset session has expired. Request a new code.'
  }

  if (text.length > 150) {
    return 'Unable to process your request. Please try again later.'
  }

  return text
}
