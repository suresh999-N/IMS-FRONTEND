/**
 * Centralized Date Utility Module
 * Handles date-only values (YYYY-MM-DD) and timestamps (ISO 8601) in a timezone-safe manner.
 * Prevents 1-day date shifting caused by UTC conversions on local date-only strings.
 */

const DATE_ONLY_REGEX = /^\d{4}-\d{2}-\d{2}/
const ACTIVITY_TIME_ZONE = 'Asia/Kolkata'
const ACTIVITY_TIME_ZONE_LABEL = 'IST'
const activityDateTimeFormatter = new Intl.DateTimeFormat('en-US', {
  timeZone: ACTIVITY_TIME_ZONE,
  month: 'short',
  day: '2-digit',
  year: 'numeric',
  hour: 'numeric',
  minute: '2-digit',
  hour12: true,
})

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

/**
 * Safely parses any date/timestamp representation (string, Date object, numeric epoch) into a valid Date object.
 * Does not force UTC suffixes on local ISO date-time strings.
 * @param {string|Date|number|null|undefined} value
 * @returns {Date|null}
 */
export function parseDateValue(value) {
  if (value === null || value === undefined || value === '') {
    return null
  }

  if (value instanceof Date) {
    return Number.isNaN(value.getTime()) ? null : value
  }

  const strVal = String(value).trim()
  if (!strVal) {
    return null
  }

  // Handle epoch timestamps (numeric)
  if (/^\d+$/.test(strVal)) {
    const num = Number(strVal)
    const ms = strVal.length === 10 ? num * 1000 : num
    const date = new Date(ms)
    return Number.isNaN(date.getTime()) ? null : date
  }

  // Parse ISO string or date string directly without modifying timezone
  const date = new Date(strVal)
  if (!Number.isNaN(date.getTime())) {
    return date
  }

  // Fallback for space-separated date-time strings (e.g. "2026-08-14 17:00:00")
  const normalized = strVal.replace(/^(\d{4}-\d{2}-\d{2})\s+(\d{2}:\d{2}(?::\d{2})?)/, '$1T$2')
  const fallbackDate = new Date(normalized)
  return Number.isNaN(fallbackDate.getTime()) ? null : fallbackDate
}

/**
 * Formats an activity timestamp in the application's display timezone.
 * The parsed Date remains the original instant; timezone conversion is display-only.
 * @param {string|Date|number|null|undefined} value
 * @returns {string}
 */
export function formatActivityDateTime(value) {
  const date = parseDateValue(value)
  if (!date) {
    return ''
  }

  const parts = Object.fromEntries(
    activityDateTimeFormatter
      .formatToParts(date)
      .filter(({ type }) => type !== 'literal')
      .map(({ type, value: partValue }) => [type, partValue]),
  )

  return `${parts.month} ${parts.day}, ${parts.year}, ${parts.hour}:${parts.minute} ${parts.dayPeriod.toUpperCase()} ${ACTIVITY_TIME_ZONE_LABEL}`
}

/**
 * Calculates a natural, past-oriented relative time for activity timestamps.
 * Future timestamps are treated as clock skew and displayed as "just now".
 * @param {string|Date|number|null|undefined} value
 * @param {string|Date|number} [now]
 * @returns {string}
 */
export function formatRelativeTime(value, now = Date.now()) {
  const date = parseDateValue(value)
  const currentDate = parseDateValue(now)

  if (!date || !currentDate) {
    return 'Recently'
  }

  const diffSeconds = Math.floor((currentDate.getTime() - date.getTime()) / 1000)

  if (diffSeconds < 60) {
    return 'just now'
  }

  const minutes = Math.floor(diffSeconds / 60)
  if (minutes < 60) {
    return `${minutes} ${minutes === 1 ? 'minute' : 'minutes'} ago`
  }

  const hours = Math.floor(minutes / 60)
  if (hours < 24) {
    return `${hours} ${hours === 1 ? 'hour' : 'hours'} ago`
  }

  const days = Math.floor(hours / 24)
  if (days === 1) {
    return 'yesterday'
  }

  if (days < 14) {
    return `${days} days ago`
  }

  if (days < 60) {
    const weeks = Math.floor(days / 7)
    return `${weeks} ${weeks === 1 ? 'week' : 'weeks'} ago`
  }

  if (days < 365) {
    const months = Math.floor(days / 30)
    return `${months} ${months === 1 ? 'month' : 'months'} ago`
  }

  const years = Math.floor(days / 365)
  return `${years} ${years === 1 ? 'year' : 'years'} ago`
}

/**
 * Formats the exact IST activity time and elapsed time as a single display value.
 * @param {string|Date|number|null|undefined} value
 * @param {string|Date|number} [now]
 * @returns {string}
 */
export function formatActivityTimestamp(value, now = Date.now()) {
  const exactDateTime = formatActivityDateTime(value)
  if (!exactDateTime) {
    return 'Recently'
  }

  return `${exactDateTime} · ${formatRelativeTime(value, now)}`
}
