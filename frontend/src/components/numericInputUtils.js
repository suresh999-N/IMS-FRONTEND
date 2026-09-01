export function stripGrouping(value) {
  return String(value ?? '').replace(/,/g, '').trim()
}

export function sanitizeNumericInput(value, { allowDecimal = true, allowNegative = false } = {}) {
  const source = stripGrouping(value)
  let nextValue = ''
  let hasDecimal = false
  let hasSign = false

  for (const char of source) {
    if (/\d/.test(char)) {
      nextValue += char
      continue
    }

    if (allowDecimal && char === '.' && !hasDecimal) {
      nextValue += char
      hasDecimal = true
      continue
    }

    if (allowNegative && char === '-' && !hasSign && nextValue.length === 0) {
      nextValue += char
      hasSign = true
    }
  }

  return nextValue
}
