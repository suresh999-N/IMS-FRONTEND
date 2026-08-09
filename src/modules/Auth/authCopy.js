export function getAuthErrorMessage(message, fallback = 'We could not complete that request. Try again.') {
  const text = String(message ?? '').trim()
  const normalizedText = text.toLowerCase()

  if (!text) return fallback
  if (normalizedText.includes('email') && normalizedText.includes('valid')) {
    return 'Enter a valid work email.'
  }
  if (normalizedText.includes('password') && normalizedText.includes('required')) {
    return 'Enter your password.'
  }
  if (normalizedText.includes('invalid') && normalizedText.includes('password')) {
    return 'Check your email and password.'
  }
  if (normalizedText.includes('invalid') && normalizedText.includes('otp')) {
    return 'Enter the 6-digit code.'
  }
  if (normalizedText.includes('token')) {
    return 'We could not start your session. Please sign in again.'
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
  if (normalizedText.includes('network') || normalizedText.includes('server')) {
    return 'Connection issue. Please try again.'
  }
  if (text.length > 120) return fallback

  return text
}
