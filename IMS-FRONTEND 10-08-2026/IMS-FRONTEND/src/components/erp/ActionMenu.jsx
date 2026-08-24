import { ChevronDown, LoaderCircle, MoreHorizontal } from 'lucide-react'
import { useEffect, useId, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
import './ERPComponents.css'

const ACTION_MENU_OPEN_EVENT = 'ims-action-menu-open'

export default function ActionMenu({
  actions = [],
  label = 'Actions',
  iconOnly = false,
  align = 'right',
  className = '',
  disabled = false,
  menuKey = '',
}) {
  const [isOpen, setIsOpen] = useState(false)
  const [popoverStyle, setPopoverStyle] = useState(null)
  const menuRef = useRef(null)
  const triggerRef = useRef(null)
  const popoverRef = useRef(null)
  const uniqueId = useId()
  const menuIdRef = useRef(`action-menu-${uniqueId}`)
  const enabledActions = actions.filter(Boolean)
  const isDisabled = disabled || enabledActions.length === 0

  function updatePopoverPosition() {
    const trigger = triggerRef.current

    if (!trigger) {
      return
    }

    const rect = trigger.getBoundingClientRect()
    const defaultMinWidth = iconOnly ? 118 : 128
    const popoverWidth = Math.max(defaultMinWidth, rect.width)
    const popoverHeight = popoverRef.current?.offsetHeight || Math.min(260, 14 + enabledActions.length * 40)
    const gutter = 8
    const left = align === 'left'
      ? Math.min(rect.left, window.innerWidth - popoverWidth - gutter)
      : align === 'center'
      ? Math.max(gutter, Math.min(rect.left + rect.width / 2 - popoverWidth / 2, window.innerWidth - popoverWidth - gutter))
      : Math.max(gutter, rect.right - popoverWidth)
    const spaceBelow = window.innerHeight - rect.bottom - gutter
    const shouldOpenAbove = spaceBelow < popoverHeight && rect.top > popoverHeight
    const top = shouldOpenAbove
      ? Math.max(gutter, rect.top - popoverHeight - 6)
      : Math.min(rect.bottom + 6, window.innerHeight - popoverHeight - gutter)

    setPopoverStyle({
      position: 'fixed',
      top: `${Math.max(gutter, top)}px`,
      left: `${Math.max(gutter, left)}px`,
      minWidth: `${popoverWidth}px`,
      width: 'max-content',
      zIndex: 1600,
    })
  }

  useEffect(() => {
    function handlePointerDown(event) {
      if (
        !menuRef.current?.contains(event.target) &&
        !popoverRef.current?.contains(event.target)
      ) {
        setIsOpen(false)
      }
    }

    function handleKeyDown(event) {
      if (event.key === 'Escape') {
        setIsOpen(false)
      }
    }

    function handleMenuOpen(event) {
      if (event.detail?.menuId !== menuIdRef.current) {
        setIsOpen(false)
      }
    }

    document.addEventListener('pointerdown', handlePointerDown)
    document.addEventListener('keydown', handleKeyDown)
    window.addEventListener(ACTION_MENU_OPEN_EVENT, handleMenuOpen)

    return () => {
      document.removeEventListener('pointerdown', handlePointerDown)
      document.removeEventListener('keydown', handleKeyDown)
      window.removeEventListener(ACTION_MENU_OPEN_EVENT, handleMenuOpen)
    }
  }, [])

  useEffect(() => {
    if (!isOpen) {
      return undefined
    }

    updatePopoverPosition()

    function handleScroll() {
      setIsOpen(false)
    }

    window.addEventListener('resize', updatePopoverPosition)
    window.addEventListener('scroll', handleScroll, true)

    return () => {
      window.removeEventListener('resize', updatePopoverPosition)
      window.removeEventListener('scroll', handleScroll, true)
    }
  }, [isOpen, align])

  useEffect(() => {
    setIsOpen(false)
    setPopoverStyle(null)
  }, [menuKey, enabledActions.length, disabled])

  const popover = isOpen ? (
    <div
      className="erp-action-menu__popover erp-action-menu__popover--portal"
      role="menu"
      aria-label={label}
      ref={popoverRef}
      style={popoverStyle || undefined}
      data-row-click-ignore="true"
    >
      {enabledActions.map((action) => {
        const Icon = action.icon
        const isDanger = action.variant === 'danger' || action.tone === 'danger'

        return (
          <button
            key={action.key || action.label}
            type="button"
            className={`erp-action-menu__item ${isDanger ? 'erp-action-menu__item--danger' : ''}`.trim()}
            role="menuitem"
            disabled={action.disabled || action.loading}
            onClick={(event) => {
              event.stopPropagation()
              setIsOpen(false)
              action.onClick?.()
            }}
          >
            {action.loading ? (
              <LoaderCircle size={15} className="animate-spin" />
            ) : Icon ? (
              <Icon size={15} />
            ) : (
              <span className="erp-action-menu__item-dot" aria-hidden="true" />
            )}
            <span>{action.label}</span>
          </button>
        )
      })}
    </div>
  ) : null

  return (
    <div
      className={`erp-action-menu erp-action-menu--${align} ${className}`.trim()}
      ref={menuRef}
      data-row-click-ignore="true"
    >
      <button
        type="button"
        ref={triggerRef}
        className={`button button-secondary erp-action-menu__trigger ${iconOnly ? 'erp-action-menu__trigger--icon' : ''}`.trim()}
        disabled={isDisabled}
        aria-haspopup="menu"
        aria-expanded={isOpen}
        aria-label={iconOnly ? label : undefined}
        title={iconOnly ? label : undefined}
        onPointerDown={(event) => {
          event.stopPropagation()
        }}
        onClick={(event) => {
          event.stopPropagation()
          setIsOpen((currentValue) => {
            const nextIsOpen = !currentValue

            if (nextIsOpen) {
              window.dispatchEvent(new CustomEvent(ACTION_MENU_OPEN_EVENT, {
                detail: { menuId: menuIdRef.current },
              }))
            }

            return nextIsOpen
          })
        }}
      >
        <MoreHorizontal size={16} />
        {iconOnly ? null : <span>{label}</span>}
        {iconOnly ? null : <ChevronDown size={14} />}
      </button>

      {popover ? createPortal(popover, document.body) : null}
    </div>
  )
}
