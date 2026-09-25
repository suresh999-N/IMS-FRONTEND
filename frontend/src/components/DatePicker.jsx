import { CalendarDays, ChevronLeft, ChevronRight } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
import InputField from './InputField'

const WEEKDAYS = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su']

function formatDisplayDate(value) {
  const match = String(value ?? '').match(/^(\d{4})-(\d{2})-(\d{2})$/)
  if (!match) return String(value ?? '')
  return `${match[3]}-${match[2]}-${match[1]}`
}

function parseDisplayDate(value) {
  const trimmedValue = String(value ?? '').trim()
  const isoMatch = trimmedValue.match(/^(\d{4})-(\d{2})-(\d{2})$/)
  if (isoMatch) return trimmedValue
  const displayMatch = trimmedValue.match(/^(\d{2})[\/\-](\d{2})[\/\-](\d{4})$/)
  if (!displayMatch) return ''
  const [, day, month, year] = displayMatch
  const isoValue = `${year}-${month}-${day}`
  const parsedDate = new Date(`${isoValue}T00:00:00`)
  return Number.isNaN(parsedDate.getTime()) ? '' : isoValue
}

function parseIsoDate(value) {
  const parsedValue = parseDisplayDate(value)
  if (!parsedValue) return null
  const parsedDate = new Date(`${parsedValue}T00:00:00`)
  return Number.isNaN(parsedDate.getTime()) ? null : parsedDate
}

function toIsoDate(date) {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

function getMonthLabel(date) {
  return date.toLocaleString('en-US', { month: 'long', year: 'numeric' })
}

function getCalendarDays(viewDate) {
  const firstOfMonth = new Date(viewDate.getFullYear(), viewDate.getMonth(), 1)
  const lastOfMonth = new Date(viewDate.getFullYear(), viewDate.getMonth() + 1, 0)
  const startDate = new Date(firstOfMonth)
  const mondayOffset = (firstOfMonth.getDay() + 6) % 7
  const requiredCells = mondayOffset + lastOfMonth.getDate()
  const calendarLength = requiredCells > 35 ? 42 : 35
  startDate.setDate(firstOfMonth.getDate() - mondayOffset)
  return Array.from({ length: calendarLength }, (_, index) => {
    const date = new Date(startDate)
    date.setDate(startDate.getDate() + index)
    return date
  })
}

function isSameDay(firstDate, secondDate) {
  return Boolean(firstDate && secondDate) && toIsoDate(firstDate) === toIsoDate(secondDate)
}

export default function DatePicker(props) {
  const {
    value,
    onChange,
    onBlur,
    name,
    label,
    placeholder = 'DD-MM-YYYY',
    className = '',
    icon = CalendarDays,
    minDate,
    min,
    maxDate,
    max,
    ...restProps
  } = props

  const wrapperRef = useRef(null)
  const popoverRef = useRef(null)
  const instanceIdRef = useRef(`datepicker-${Math.random().toString(36).substring(2, 9)}`)
  const [displayValue, setDisplayValue] = useState(() => formatDisplayDate(value))
  const [isOpen, setIsOpen] = useState(false)
  const [viewDate, setViewDate] = useState(() => parseIsoDate(value) || new Date())
  const [popoverStyle, setPopoverStyle] = useState({})
  const [portalElement, setPortalElement] = useState(null)
  const selectedDate = parseIsoDate(value)
  const today = new Date()

  const rawMin = minDate || min
  const rawMax = maxDate || max
  const resolvedMin = typeof rawMin === 'function' ? rawMin() : rawMin
  const resolvedMax = typeof rawMax === 'function' ? rawMax() : rawMax
  const minDateIso = resolvedMin ? (parseDisplayDate(resolvedMin) || String(resolvedMin).slice(0, 10)) : ''
  const maxDateIso = resolvedMax ? (parseDisplayDate(resolvedMax) || String(resolvedMax).slice(0, 10)) : ''

  function isDateDisabled(date) {
    if (!date) return false
    const iso = toIsoDate(date)
    if (minDateIso && iso < minDateIso) return true
    if (maxDateIso && iso > maxDateIso) return true
    return false
  }

  useEffect(() => {
    if (typeof document === 'undefined') return undefined
    const element = document.createElement('div')
    element.className = 'date-picker-portal-root'
    document.body.appendChild(element)
    setPortalElement(element)
    return () => {
      setPortalElement(null)
      element.remove()
    }
  }, [])

  useEffect(() => {
    function handleGlobalDropdownOpened(event) {
      if (event.detail?.id !== instanceIdRef.current) {
        setIsOpen(false)
      }
    }
    window.addEventListener('ims:dropdown-opened', handleGlobalDropdownOpened)
    return () => window.removeEventListener('ims:dropdown-opened', handleGlobalDropdownOpened)
  }, [])

  useEffect(() => {
    setDisplayValue(formatDisplayDate(value))
    setViewDate(parseIsoDate(value) || new Date())
  }, [value])

  useEffect(() => {
    if (!isOpen) return undefined

    function updatePopoverPosition() {
      const rect = wrapperRef.current?.getBoundingClientRect()
      if (!rect || typeof window === 'undefined') return
      const popoverWidth = 248
      const popoverHeight = 244
      const gutter = 10
      const left = Math.max(gutter, Math.min(rect.left, window.innerWidth - popoverWidth - gutter))
      let top = rect.bottom + 7
      if (restProps.forceDownward === false && top + popoverHeight > window.innerHeight - gutter) {
        top = Math.max(gutter, rect.top - popoverHeight - 7)
      }
      setPopoverStyle({
        position: 'fixed',
        left: `${left}px`,
        top: `${top}px`,
        zIndex: 2147483647,
      })
    }

    function handlePointerDown(event) {
      if (
        wrapperRef.current?.contains(event.target) ||
        popoverRef.current?.contains(event.target)
      ) {
        return
      }
      setIsOpen(false)
    }

    function handleKeyDown(event) {
      if (event.key === 'Escape') setIsOpen(false)
    }

    updatePopoverPosition()
    document.addEventListener('mousedown', handlePointerDown)
    document.addEventListener('pointerdown', handlePointerDown)
    document.addEventListener('keydown', handleKeyDown)
    window.addEventListener('resize', updatePopoverPosition)
    window.addEventListener('scroll', updatePopoverPosition, true)

    return () => {
      document.removeEventListener('mousedown', handlePointerDown)
      document.removeEventListener('pointerdown', handlePointerDown)
      document.removeEventListener('keydown', handleKeyDown)
      window.removeEventListener('resize', updatePopoverPosition)
      window.removeEventListener('scroll', updatePopoverPosition, true)
    }
  }, [isOpen, restProps.forceDownward])

  function emitChange(nextValue) {
    onChange?.({ target: { name, value: nextValue } })
  }

  function handleChange(event) {
    const nextDisplayValue = event.target.value
    const parsedValue = parseDisplayDate(nextDisplayValue)
    setDisplayValue(nextDisplayValue)
    emitChange(parsedValue || nextDisplayValue)
  }

  function handleBlur() {
    const parsedValue = parseDisplayDate(displayValue)
    if (parsedValue) {
      if (minDateIso && parsedValue < minDateIso) {
        setDisplayValue(formatDisplayDate(minDateIso))
        emitChange(minDateIso)
      } else if (maxDateIso && parsedValue > maxDateIso) {
        setDisplayValue(formatDisplayDate(maxDateIso))
        emitChange(maxDateIso)
      } else {
        setDisplayValue(formatDisplayDate(parsedValue))
        emitChange(parsedValue)
      }
    }

    onBlur?.({
      target: {
        name,
        value: (minDateIso && parsedValue && parsedValue < minDateIso)
          ? minDateIso
          : ((maxDateIso && parsedValue && parsedValue > maxDateIso) ? maxDateIso : (parsedValue || displayValue)),
      },
    })
  }

  function openCalendar() {
    setViewDate(parseIsoDate(value) || new Date())
    window.dispatchEvent(new CustomEvent('ims:dropdown-opened', { detail: { id: instanceIdRef.current } }))
    setIsOpen(true)
  }

  function shiftMonth(offset) {
    setViewDate((d) => new Date(d.getFullYear(), d.getMonth() + offset, 1))
  }

  function selectDate(date) {
    if (isDateDisabled(date)) {
      return
    }
    const nextValue = toIsoDate(date)
    if (maxDateIso && nextValue > maxDateIso) {
      return
    }
    if (minDateIso && nextValue < minDateIso) {
      return
    }
    setDisplayValue(formatDisplayDate(nextValue))
    emitChange(nextValue)
    setIsOpen(false)
  }

  const popover = isOpen ? (
    <div
      ref={popoverRef}
      className="date-picker-popover"
      role="dialog"
      aria-label={`${label || 'Date'} calendar`}
      style={popoverStyle}
    >
      <div className="date-picker-popover__header">
        <button type="button" onClick={() => shiftMonth(-1)} aria-label="Previous month">
          <ChevronLeft size={16} />
        </button>
        <span>
          <small>{label ? `${label} date` : 'Date'}</small>
          <strong>{getMonthLabel(viewDate)}</strong>
        </span>
        <button type="button" onClick={() => shiftMonth(1)} aria-label="Next month">
          <ChevronRight size={16} />
        </button>
      </div>
      <div className="date-picker-popover__weekdays" aria-hidden="true">
        {WEEKDAYS.map((day) => (
          <span key={day}>{day}</span>
        ))}
      </div>
      <div className="date-picker-popover__grid">
        {getCalendarDays(viewDate).map((date) => {
          const isoValue = toIsoDate(date)
          const isMuted = date.getMonth() !== viewDate.getMonth()
          const isSelected = isSameDay(date, selectedDate)
          const isToday = isSameDay(date, today)
          const isDisabled = isDateDisabled(date)

          return (
            <button
              key={isoValue}
              type="button"
              disabled={isDisabled}
              aria-disabled={isDisabled}
              className={[
                'date-picker-popover__day',
                isMuted ? 'is-muted' : '',
                isSelected ? 'is-selected' : '',
                isToday ? 'is-today' : '',
                isDisabled ? 'is-disabled' : '',
              ].filter(Boolean).join(' ')}
              onClick={() => !isDisabled && selectDate(date)}
            >
              {date.getDate()}
            </button>
          )
        })}
      </div>
      <div className="date-picker-popover__footer">
        <button type="button" disabled={isDateDisabled(today)} onClick={() => selectDate(today)}>Today</button>
        <button
          type="button"
          onClick={() => {
            setDisplayValue('')
            emitChange('')
            setIsOpen(false)
          }}
        >
          Clear
        </button>
      </div>
    </div>
  ) : null

  return (
    <div className={`date-picker-shell ${isOpen ? 'is-open' : ''}`} ref={wrapperRef}>
      <InputField
        icon={icon}
        onIconClick={openCalendar}
        iconLabel={`Open ${label || 'date'} calendar`}
        label={label}
        type="text"
        name={name}
        value={displayValue}
        onChange={handleChange}
        onBlur={handleBlur}
        onFocus={openCalendar}
        placeholder={placeholder}
        className={`date-picker-field ${className}`.trim()}
        {...restProps}
      />
      {popover && portalElement ? createPortal(popover, portalElement) : null}
    </div>
  )
}
