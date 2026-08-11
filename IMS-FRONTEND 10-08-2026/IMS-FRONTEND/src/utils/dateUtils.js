/**
 * Centralized Date Utility Module
 * Handles date-only values (YYYY-MM-DD) and timestamps (ISO 8601) in a timezone-safe manner.
 * Prevents 1-day date shifting caused by UTC conversions on local date-only strings.
 */

const DATE_ONLY_REGEX = /^\d{4}-\d{2}-\d{2}/

/**
 * Returns today's local date formatted as YYYY-MM-DD without UTC timezone shifting.
 * @returns {string} e.g. "2026-08-05"
 */
export function getLocalTodayDate() {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

/**
 * Converts any date representation (string, Date object) into a safe YYYY-MM-DD format for <input type="date">.
 * @param {string|Date|null|undefined} value
 * @returns {string} e.g. "2026-08-05" or ""
 */
export function toDateInputValue(value) {
  if (value === null || value === undefined || value === '') {
    return ''
  }

  if (value instanceof Date) {
    if (Number.isNaN(value.getTime())) return ''
    const year = value.getFullYear()
    const month = String(value.getMonth() + 1).padStart(2, '0')
    const day = String(value.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  const strVal = String(value).trim()
  if (!strVal) return ''

  // If already YYYY-MM-DD or starts with YYYY-MM-DD
  if (DATE_ONLY_REGEX.test(strVal)) {
    return strVal.slice(0, 10)
  }

  // Fallback parsing
  const parsed = new Date(strVal)
  if (Number.isNaN(parsed.getTime())) return ''

  const year = parsed.getFullYear()
  const month = String(parsed.getMonth() + 1).padStart(2, '0')
  const day = String(parsed.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

/**
 * Formats a date-only or timestamp value into DD-MM-YYYY for UI display.
 * @param {string|Date|null|undefined} value
 * @param {string} fallback
 * @returns {string} e.g. "05-08-2026" or "-"
 */
export function formatDateForDisplay(value, fallback = '-') {
  const dateInput = toDateInputValue(value)
  if (!dateInput) return fallback

  const parts = dateInput.split('-')
  if (parts.length !== 3) return fallback

  return `${parts[2]}-${parts[1]}-${parts[0]}`
}

/**
 * Formats a date-time timestamp into DD-MM-YYYY hh:mm A for UI display.
 * @param {string|Date|null|undefined} value
 * @param {string} fallback
 * @returns {string} e.g. "05-08-2026 05:15 PM" or "-"
 */
export function formatDateTimeForDisplay(value, fallback = '-') {
  if (!value) return fallback

  const parsed = value instanceof Date ? value : new Date(value)
  if (Number.isNaN(parsed.getTime())) return fallback

  const day = String(parsed.getDate()).padStart(2, '0')
  const month = String(parsed.getMonth() + 1).padStart(2, '0')
  const year = parsed.getFullYear()

  let hours = parsed.getHours()
  const minutes = String(parsed.getMinutes()).padStart(2, '0')
  const ampm = hours >= 12 ? 'PM' : 'AM'
  hours = hours % 12
  hours = hours ? hours : 12
  const formattedHours = String(hours).padStart(2, '0')

  return `${day}-${month}-${year} ${formattedHours}:${minutes} ${ampm}`
}

/**
 * Normalizes date-only values for API payloads.
 * Keeps YYYY-MM-DD format intact without converting to UTC ISO string (which shifts dates).
 * @param {string|Date|null|undefined} value
 * @returns {string|null} e.g. "2026-08-05" or null
 */
export function normaliseDateOnlyPayload(value) {
  const inputVal = toDateInputValue(value)
  return inputVal || null
}

/**
 * Checks whether a given value is a valid date.
 * @param {any} value
 * @returns {boolean}
 */
export function isValidDateValue(value) {
  if (!value) return false
  const inputVal = toDateInputValue(value)
  return Boolean(inputVal)
}

/**
 * Compares two date-only values.
 * Returns -1 if first < second, 1 if first > second, 0 if equal.
 * @param {string|Date} first
 * @param {string|Date} second
 * @returns {number}
 */
export function compareDateOnly(first, second) {
  const d1 = toDateInputValue(first)
  const d2 = toDateInputValue(second)

  if (!d1 || !d2) return 0
  if (d1 < d2) return -1
  if (d1 > d2) return 1
  return 0
}
