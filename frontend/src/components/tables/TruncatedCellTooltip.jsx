import { useEffect, useLayoutEffect, useRef, useState } from 'react'
import { createPortal } from 'react-dom'

const HOVER_DELAY_MS = 250
const VIEWPORT_GUTTER = 12
const TOOLTIP_GAP = 8
const MAX_TOOLTIP_WIDTH = 380

function hasVisibleText(element) {
  return Boolean(String(element?.innerText || element?.textContent || '').trim())
}

function isTruncated(element) {
  if (!(element instanceof HTMLElement) || !hasVisibleText(element)) return false

  return (
    element.scrollWidth > element.clientWidth + 1 ||
    element.scrollHeight > element.clientHeight + 1 ||
    element.offsetWidth < element.scrollWidth
  )
}

function findTruncatedElement(cell, target) {
  if (target && cell.contains(target)) {
    const titledTarget = target.closest?.('[title], [data-tooltip-title]')
    if (titledTarget && cell.contains(titledTarget)) {
      const titleVal = (titledTarget.getAttribute('title') || titledTarget.getAttribute('data-tooltip-title'))?.trim()
      if (titleVal || isTruncated(titledTarget)) {
        return titledTarget
      }
    }

    const truncatedTarget = target.closest?.('*')
    if (truncatedTarget && cell.contains(truncatedTarget) && isTruncated(truncatedTarget)) {
      return truncatedTarget
    }
  }

  const titleElement = cell.querySelector('[title], [data-tooltip-title]')
  if (titleElement && isTruncated(titleElement)) {
    return titleElement
  }

  const descendants = [...cell.querySelectorAll('*')]
  const truncatedDescendant = descendants.find((element) => isTruncated(element))
  if (truncatedDescendant) return truncatedDescendant

  if (isTruncated(cell)) return cell

  if (titleElement) {
    const titleVal = (titleElement.getAttribute('title') || titleElement.getAttribute('data-tooltip-title'))?.trim()
    if (titleVal) {
      return titleElement
    }
  }

  return null
}

function getTooltipPosition(element) {
  const rect = element.getBoundingClientRect()
  const maxWidth = Math.min(MAX_TOOLTIP_WIDTH, window.innerWidth - 2 * VIEWPORT_GUTTER)
  const left = Math.max(
    VIEWPORT_GUTTER,
    Math.min(rect.left, window.innerWidth - maxWidth - VIEWPORT_GUTTER)
  )

  const spaceAbove = rect.top - VIEWPORT_GUTTER
  const spaceBelow = window.innerHeight - rect.bottom - VIEWPORT_GUTTER

  const preferAbove = spaceAbove >= 90 || spaceAbove >= spaceBelow
  const placement = preferAbove ? 'above' : 'below'
  const top = placement === 'above'
    ? rect.top - TOOLTIP_GAP
    : rect.bottom + TOOLTIP_GAP

  return {
    left,
    maxWidth,
    top,
    placement,
    anchorTop: rect.top,
    anchorBottom: rect.bottom,
  }
}

export default function TruncatedCellTooltip({ containerRef }) {
  const timerRef = useRef(null)
  const activeElementRef = useRef(null)
  const tooltipRef = useRef(null)
  const [tooltip, setTooltip] = useState(null)

  useLayoutEffect(() => {
    if (!tooltip || !tooltipRef.current) return
    const el = tooltipRef.current
    const tooltipRect = el.getBoundingClientRect()

    if (tooltip.placement === 'above' && tooltipRect.top < VIEWPORT_GUTTER) {
      const spaceBelow = window.innerHeight - (tooltip.anchorBottom ?? 0) - VIEWPORT_GUTTER
      if (spaceBelow >= tooltipRect.height + TOOLTIP_GAP) {
        setTooltip((prev) =>
          prev
            ? {
                ...prev,
                placement: 'below',
                top: (prev.anchorBottom ?? prev.top) + TOOLTIP_GAP,
              }
            : null
        )
      } else {
        const clampedTop = VIEWPORT_GUTTER + tooltipRect.height
        if (Math.abs(tooltip.top - clampedTop) > 1) {
          setTooltip((prev) =>
            prev
              ? {
                  ...prev,
                  top: clampedTop,
                }
              : null
          )
        }
      }
    } else if (tooltip.placement === 'below' && tooltipRect.bottom > window.innerHeight - VIEWPORT_GUTTER) {
      const spaceAbove = (tooltip.anchorTop ?? 0) - VIEWPORT_GUTTER
      if (spaceAbove >= tooltipRect.height + TOOLTIP_GAP) {
        setTooltip((prev) =>
          prev
            ? {
                ...prev,
                placement: 'above',
                top: (prev.anchorTop ?? prev.top) - TOOLTIP_GAP,
              }
            : null
        )
      }
    }
  }, [tooltip?.text, tooltip?.top, tooltip?.placement])

  useEffect(() => {
    const container = containerRef.current
    if (!container) return undefined

    function clearTimer() {
      if (timerRef.current) {
        window.clearTimeout(timerRef.current)
        timerRef.current = null
      }
    }

    function restoreTitle(element) {
      if (!element) return
      if (element.hasAttribute('data-tooltip-title')) {
        element.setAttribute('title', element.getAttribute('data-tooltip-title'))
        element.removeAttribute('data-tooltip-title')
      }
    }

    function hideTooltip() {
      clearTimer()
      if (activeElementRef.current) {
        restoreTitle(activeElementRef.current)
        activeElementRef.current = null
      }
      setTooltip(null)
    }

    function handlePointerOver(event) {
      const cell = event.target.closest?.('td')
      if (!cell || !container.contains(cell)) return
      if (cell.classList.contains('table-component__selection-cell')) return

      const isAction = event.target.closest?.('button, a, input, select')
      if (
        isAction &&
        !isAction.hasAttribute('title') &&
        !isAction.hasAttribute('data-tooltip-title') &&
        !isTruncated(isAction)
      ) {
        hideTooltip()
        return
      }

      const truncatedElement = findTruncatedElement(cell, event.target)
      if (!truncatedElement) {
        hideTooltip()
        return
      }

      if (activeElementRef.current === truncatedElement) return

      hideTooltip()
      activeElementRef.current = truncatedElement

      timerRef.current = window.setTimeout(() => {
        if (activeElementRef.current !== truncatedElement) return

        const rawTitle =
          truncatedElement.getAttribute('title') || truncatedElement.getAttribute('data-tooltip-title')
        const text = String(rawTitle || truncatedElement.innerText || truncatedElement.textContent || '').trim()

        if (!text) return

        if (truncatedElement.hasAttribute('title')) {
          truncatedElement.setAttribute('data-tooltip-title', rawTitle)
          truncatedElement.removeAttribute('title')
        }

        setTooltip({
          text,
          ...getTooltipPosition(truncatedElement),
        })
      }, HOVER_DELAY_MS)
    }

    function handlePointerOut(event) {
      const currentCell = event.target.closest?.('td')
      const nextCell = event.relatedTarget?.closest?.('td')

      if (currentCell && currentCell !== nextCell) {
        hideTooltip()
        return
      }

      const currentItem = event.target.closest?.('[title], [data-tooltip-title]')
      const nextItem = event.relatedTarget?.closest?.('[title], [data-tooltip-title]')
      if (currentItem && currentItem !== nextItem) {
        hideTooltip()
      }
    }

    container.addEventListener('pointerover', handlePointerOver)
    container.addEventListener('pointerout', handlePointerOut)
    container.addEventListener('scroll', hideTooltip, { passive: true })
    window.addEventListener('resize', hideTooltip)

    return () => {
      hideTooltip()
      container.removeEventListener('pointerover', handlePointerOver)
      container.removeEventListener('pointerout', handlePointerOut)
      container.removeEventListener('scroll', hideTooltip)
      window.removeEventListener('resize', hideTooltip)
    }
  }, [containerRef])

  if (!tooltip || typeof document === 'undefined') return null

  return createPortal(
    <div
      ref={tooltipRef}
      className={`table-truncated-tooltip table-truncated-tooltip--${tooltip.placement}`}
      role="tooltip"
      style={{
        left: `${tooltip.left}px`,
        maxWidth: `${tooltip.maxWidth}px`,
        top: `${tooltip.top}px`,
      }}
    >
      {tooltip.text}
    </div>,
    document.body,
  )
}
