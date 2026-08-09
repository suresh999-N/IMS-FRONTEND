export const PHONE_MAX_LENGTH = 10

const CONTROL_KEYS = new Set([
  'Backspace',
  'Delete',
  'ArrowLeft',
  'ArrowRight',
  'ArrowUp',
  'ArrowDown',
  'Home',
  'End',
  'Tab',
  'Enter',
])

export function sanitizePhoneInput(value, maxLength = PHONE_MAX_LENGTH) {
  return String(value ?? '').replace(/\D/g, '').slice(0, maxLength)
}

export function getPhoneDigits(value) {
  return sanitizePhoneInput(value)
}

export function getPhoneError(value, label = 'Phone') {
  const phone = sanitizePhoneInput(value)

  if (!phone) {
    return `${label} is required.`
  }

  if (phone.length !== PHONE_MAX_LENGTH) {
    return 'Phone number must contain exactly 10 digits.'
  }

  return ''
}

export function isValidPhone(value) {
  return !getPhoneError(value)
}

export function blockInvalidPhoneKey(event) {
  if (
    CONTROL_KEYS.has(event.key) ||
    event.ctrlKey ||
    event.metaKey
  ) {
    return
  }

  if (!/^\d$/.test(event.key)) {
    event.preventDefault()
  }
}

export const phoneInputProps = {
  type: 'tel',
  inputMode: 'numeric',
  pattern: '[0-9]*',
  maxLength: PHONE_MAX_LENGTH,
  onKeyDown: blockInvalidPhoneKey,
}
