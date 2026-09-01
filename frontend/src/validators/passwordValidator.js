export const PASSWORD_MIN_LENGTH = 8

export function getPasswordError(password, options = {}) {
  const { required = true, label = 'Password' } = options
  const val = String(password ?? '')

  if (!val) {
    return required ? `${label} is required.` : ''
  }

  if (val.length < PASSWORD_MIN_LENGTH) {
    return 'Password must be at least 8 characters.'
  }

  if (!/[A-Z]/.test(val)) {
    return 'Password must include at least one uppercase letter.'
  }

  if (!/[a-z]/.test(val)) {
    return 'Password must include at least one lowercase letter.'
  }

  if (!/[0-9]/.test(val)) {
    return 'Password must include at least one number.'
  }

  if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(val)) {
    return 'Password must include at least one special character.'
  }

  return ''
}

export function isValidPassword(password) {
  return !getPasswordError(password)
}
