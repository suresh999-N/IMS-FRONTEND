import { useEffect, useMemo, useRef, useState } from 'react'
import { renderFormLabel } from '../utils/labelUtils'
import { sanitizeNumericInput } from './numericInputUtils'

const currencyFormatters = {
  INR: new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }),
}

function getCurrencyFormatter(currency) {
  return currencyFormatters[currency] ?? new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })
}

function getRawValue(value) {
  return sanitizeNumericInput(value, { allowDecimal: true, allowNegative: false })
}

function getFormattedValue(value, currency) {
  const rawValue = getRawValue(value)

  if (!rawValue || rawValue === '.') {
    return ''
  }

  const numericValue = Number(rawValue)

  if (!Number.isFinite(numericValue)) {
    return rawValue
  }

  return getCurrencyFormatter(currency).format(numericValue)
}

function normalizeCompletedCurrencyValue(value) {
  const rawValue = getRawValue(value)
  const numericValue = Number(rawValue)

  if (!rawValue || !Number.isFinite(numericValue)) {
    return rawValue
  }

  return String(numericValue)
}

export default function CurrencyInput({
  id,
  label,
  icon: Icon,
  prefix,
  name,
  value,
  onChange,
  onBlur,
  placeholder,
  error,
  className = '',
  currency = 'INR',
  onFocus,
  ...props
}) {
  const inputRef = useRef(null)
  const [isFocused, setIsFocused] = useState(false)
  const [displayValue, setDisplayValue] = useState(() => getFormattedValue(value, currency))
  const resolvedPrefix = prefix
  const describedBy = error ? `${id}-error` : undefined

  const formattedValue = useMemo(
    () => getFormattedValue(value, currency),
    [currency, value],
  )

  useEffect(() => {
    if (isFocused) {
      return
    }

    setDisplayValue(formattedValue)
  }, [formattedValue, isFocused])

  function emitChange(nextValue) {
    onChange?.({
      target: {
        name,
        value: nextValue,
      },
    })
  }

  function handleFocus(event) {
    const rawValue = getRawValue(value)
    setIsFocused(true)
    setDisplayValue(rawValue)
    onFocus?.(event)
  }

  function handleChange(event) {
    const rawInputValue = event.target.value
    const caretPosition = event.target.selectionStart ?? rawInputValue.length
    const beforeCaret = rawInputValue.slice(0, caretPosition)
    const nextValue = getRawValue(rawInputValue)
    const nextCaret = getRawValue(beforeCaret).length

    setDisplayValue(nextValue)
    emitChange(nextValue)

    if (nextValue !== rawInputValue) {
      window.requestAnimationFrame(() => {
        inputRef.current?.setSelectionRange(nextCaret, nextCaret)
      })
    }
  }

  function handleBlur(event) {
    const nextValue = normalizeCompletedCurrencyValue(event.target.value)
    setIsFocused(false)
    setDisplayValue(getFormattedValue(nextValue, currency))
    emitChange(nextValue)
    onBlur?.({
      target: {
        name,
        value: nextValue,
      },
    })
  }

  return (
    <div className={`field ${className}`.trim()}>
      <label htmlFor={id}>{renderFormLabel(label)}</label>
      <div className={`input-with-icon ${error ? 'field--error' : ''}`.trim()}>
        {Icon ? <Icon size={18} /> : null}
        {resolvedPrefix ? <span className="input-prefix">{resolvedPrefix}</span> : null}
        <input
          ref={inputRef}
          id={id}
          name={name}
          type="text"
          inputMode="decimal"
          value={displayValue}
          placeholder={placeholder}
          {...props}
          style={{ textAlign: 'right', ...props.style }}
          onFocus={handleFocus}
          onChange={handleChange}
          onBlur={handleBlur}
          aria-invalid={Boolean(error)}
          aria-describedby={describedBy}
          autoComplete="off"
        />
      </div>
      {error ? <span id={`${id}-error`} className="field-error" role="alert">{error}</span> : null}
    </div>
  )
}
