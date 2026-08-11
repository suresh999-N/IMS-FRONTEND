import { ChevronDown, Download, LoaderCircle } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import './ERPComponents.css'

export default function ExportMenu({
  actions = [],
  label = 'Export',
  className = '',
  disabled = false,
  align = 'right',
}) {
  const [isOpen, setIsOpen] = useState(false)
  const menuRef = useRef(null)
  const enabledActions = actions.filter(Boolean)
  const isDisabled = disabled || enabledActions.length === 0

  useEffect(() => {
    function handlePointerDown(event) {
      if (!menuRef.current?.contains(event.target)) {
        setIsOpen(false)
      }
    }

    function handleKeyDown(event) {
      if (event.key === 'Escape') {
        setIsOpen(false)
      }
    }

    document.addEventListener('pointerdown', handlePointerDown)
    document.addEventListener('keydown', handleKeyDown)
    return () => {
      document.removeEventListener('pointerdown', handlePointerDown)
      document.removeEventListener('keydown', handleKeyDown)
    }
  }, [])

  return (
    <div className={`erp-export-menu erp-export-menu--${align} ${className}`.trim()} ref={menuRef}>
      <button
        type="button"
        className="button button-secondary erp-export-menu__trigger"
        disabled={isDisabled}
        aria-haspopup="menu"
        aria-expanded={isOpen}
        onClick={() => setIsOpen((currentValue) => !currentValue)}
      >
        <Download size={15} />
        {label}
        <ChevronDown size={14} />
      </button>

      {isOpen ? (
        <div className="erp-export-menu__popover" role="menu" aria-label={label}>
          {enabledActions.map((action) => {
            const Icon = action.icon

            return (
              <button
                key={action.key || action.label}
                type="button"
                className="erp-export-menu__item"
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
                  <Download size={15} />
                )}
                <span>{action.label}</span>
              </button>
            )
          })}
        </div>
      ) : null}
    </div>
  )
}
