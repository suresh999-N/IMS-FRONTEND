export function getAuthErrorMessage(message, fallback = 'We could not complete that request. Try again.') {
  const text = String(message ?? '').trim()
  const normalizedText = text.toLowerCase()

  if (!text) return fallback

  // If text is a specific field validation error message, preserve it directly!
  if (
    normalizedText.includes('name') ||
    normalizedText.includes('email') ||
    normalizedText.includes('phone') ||
    normalizedText.includes('password') ||
    normalizedText.includes('match')
  ) {
    if (normalizedText.includes('already') || normalizedText.includes('exists')) {
      if (normalizedText.includes('phone')) return 'An account with this phone number already exists.'
      return 'An account with this email already exists.'
    }
    if (normalizedText.includes('not found') || normalizedText.includes('no user')) {
      return 'No account matches that email.'
    }
    if (text.length <= 150) {
      return text
    }
  }

  // Check credential failures first to avoid matching "invalid" as "valid" under email checks
  if (normalizedText.includes('invalid') && normalizedText.includes('password')) {
    if (normalizedText.includes('remaining') || normalizedText.includes('attempt') || normalizedText.includes('lock')) {
      return text
    }
    return 'Check your email and password.'
  }

  if (normalizedText.includes('email') && normalizedText.includes('valid')) {
    return 'Enter a valid email address.'
  }
  if (normalizedText.includes('password') && normalizedText.includes('required')) {
    return 'Enter your password.'
  }
  if (normalizedText.includes('invalid') && normalizedText.includes('otp')) {
    return 'Enter the 6-digit code.'
  }
  if (normalizedText.includes('token')) {
    return 'We could not start your session. Please sign in again.'
  }
  if (normalizedText.includes('phone') && (normalizedText.includes('already') || normalizedText.includes('exists'))) {
    return 'An account with this phone number already exists.'
  }
  if (normalizedText.includes('already') || normalizedText.includes('exists')) {
    return 'An account with this email already exists.'
  }
  if (normalizedText.includes('not found') || normalizedText.includes('no user')) {
    return 'No account matches that email.'
  }
  if (normalizedText.includes('expired')) {
    return 'Your reset session has expired. Request a new code.'
  }
  if (normalizedText.includes('network') || normalizedText.includes('server') || normalizedText.includes('connect')) {
    return 'Unable to connect to the server.'
  }
  if (text.length > 150) return fallback

  return text
}
