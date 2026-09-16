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

  if (
    element.scrollWidth > element.clientWidth + 1 ||
    element.scrollHeight > element.clientHeight + 1 ||
    element.offsetWidth < element.scrollWidth
  ) {
    return true
  }

  try {
    const range = document.createRange()
    range.selectNodeContents(element)
    const rangeWidth = range.getBoundingClientRect().width
    const elementRect = element.getBoundingClientRect()
    const style = window.getComputedStyle(element)
    const paddingLeft = parseFloat(style.paddingLeft) || 0
    const paddingRight = parseFloat(style.paddingRight) || 0
    const availableWidth = elementRect.width - paddingLeft - paddingRight
    if (rangeWidth > availableWidth + 1) {
      return true
    }
  } catch {
    // Ignore range errors
  }

  return false
}

function findTruncatedElement(cell, target) {
  if (target && cell.contains(target)) {
    const titledTarget = target.closest?.('[title], [data-tooltip-title]')
    if (titledTarget && cell.contains(titledTarget)) {
      const titleVal = (titledTarget.getAttribute('title') || titledTarget.getAttribute('data-tooltip-title'))?.trim()
      if (isActionElement(titledTarget, cell)) {
        if (titleVal || isTruncated(titledTarget)) {
          return titledTarget
        }
      } else if (isTruncated(titledTarget) || (titleVal && isTruncated(cell))) {
        return titledTarget
      }
    }

    const truncatedTarget = target.closest?.('*')
    if (truncatedTarget && cell.contains(truncatedTarget) && isTruncated(truncatedTarget)) {
      return truncatedTarget
    }
  }

  const titleElement = cell.querySelector('[title], [data-tooltip-title]')
  if (titleElement && (isTruncated(titleElement) || isTruncated(cell))) {
    return titleElement
  }

  const descendants = [...cell.querySelectorAll('*')]
  const truncatedDescendant = descendants.find((element) => isTruncated(element))
  if (truncatedDescendant) return truncatedDescendant

  if (isTruncated(cell)) return cell

  return null
}

function isActionElement(element, cell) {
  const cellEl = cell || element?.closest?.('td') || element
  return Boolean(
    element?.closest?.('button, a, .erp-action-menu__trigger, .warehouses-action-button') ||
    cellEl?.classList?.contains('table-component__actions-cell') ||
    cellEl?.getAttribute?.('data-column-id') === 'actions'
  )
}

function getTooltipPosition(element, cell) {
  const targetRect = element.getBoundingClientRect()
  const cellEl = cell || element.closest?.('td') || element
  const cellRect = cellEl.getBoundingClientRect()
  const isAction = isActionElement(element, cellEl)

  const maxWidth = Math.min(MAX_TOOLTIP_WIDTH, window.innerWidth - 2 * VIEWPORT_GUTTER)

  if (isAction) {
    const spaceLeft = targetRect.left - VIEWPORT_GUTTER
    const spaceRight = window.innerWidth - targetRect.right - VIEWPORT_GUTTER

    let placement = 'left'
    let left = targetRect.left - TOOLTIP_GAP
    let top = targetRect.top + targetRect.height / 2

    // If space to the left is cramped but plenty on the right
    if (spaceLeft < 110 && spaceRight >= 110) {
      placement = 'right'
      left = targetRect.right + TOOLTIP_GAP
    }

    return {
      left,
      maxWidth: 200,
      top,
      placement,
      isAction: true,
      anchorTop: targetRect.top,
      anchorBottom: targetRect.bottom,
      anchorLeft: targetRect.left,
      anchorRight: targetRect.right,
      targetTop: targetRect.top,
      targetLeft: targetRect.left,
      targetWidth: targetRect.width,
      targetHeight: targetRect.height,
    }
  }

  const spaceAbove = cellRect.top - VIEWPORT_GUTTER
  const spaceBelow = window.innerHeight - cellRect.bottom - VIEWPORT_GUTTER

  const ESTIMATED_HEIGHT = 80

  let placement = 'below'
  let top = cellRect.bottom + TOOLTIP_GAP
  let left = Math.max(VIEWPORT_GUTTER, targetRect.left)

  if (spaceBelow >= ESTIMATED_HEIGHT + TOOLTIP_GAP && spaceBelow >= spaceAbove) {
    placement = 'below'
    top = cellRect.bottom + TOOLTIP_GAP
  } else if (spaceAbove >= ESTIMATED_HEIGHT + TOOLTIP_GAP) {
    placement = 'above'
    top = cellRect.top - TOOLTIP_GAP
  } else {
    placement = spaceAbove > spaceBelow ? 'above' : 'below'
    top = spaceAbove > spaceBelow ? cellRect.top - TOOLTIP_GAP : cellRect.bottom + TOOLTIP_GAP
  }

  return {
    left,
    maxWidth,
    top,
    placement,
    isAction: false,
    anchorTop: cellRect.top,
    anchorBottom: cellRect.bottom,
    anchorLeft: cellRect.left,
    anchorRight: cellRect.right,
    targetTop: targetRect.top,
    targetLeft: targetRect.left,
    targetWidth: targetRect.width,
    targetHeight: targetRect.height,
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
    const { anchorTop, anchorBottom, anchorLeft, anchorRight, targetTop, targetLeft, targetWidth, targetHeight, isAction } = tooltip

    if (isAction) {
      const tooltipWidth = tooltipRect.width
      const tooltipHeight = tooltipRect.height

      const fitsLeft = (anchorLeft - TOOLTIP_GAP - tooltipWidth) >= VIEWPORT_GUTTER
      const fitsRight = (anchorRight + TOOLTIP_GAP + tooltipWidth) <= (window.innerWidth - VIEWPORT_GUTTER)

      let nextPlacement = 'left'
      let nextLeft = anchorLeft - TOOLTIP_GAP

      if (!fitsLeft && fitsRight) {
        nextPlacement = 'right'
        nextLeft = anchorRight + TOOLTIP_GAP
      } else if (!fitsLeft && !fitsRight) {
        // Fallback above centered
        nextPlacement = 'above'
        nextLeft = Math.max(VIEWPORT_GUTTER, Math.min(anchorLeft + (targetWidth || 30) / 2 - tooltipWidth / 2, window.innerWidth - tooltipWidth - VIEWPORT_GUTTER))
      }

      let nextTop = targetTop + (targetHeight || 30) / 2
      if (nextPlacement === 'left' || nextPlacement === 'right') {
        const halfHeight = tooltipHeight / 2
        nextTop = Math.max(VIEWPORT_GUTTER + halfHeight, Math.min(nextTop, window.innerHeight - VIEWPORT_GUTTER - halfHeight))
      } else if (nextPlacement === 'above') {
        nextTop = anchorTop - TOOLTIP_GAP
      }

      if (
        nextPlacement !== tooltip.placement ||
        Math.abs(nextTop - tooltip.top) > 1 ||
        Math.abs(nextLeft - tooltip.left) > 1
      ) {
        setTooltip((prev) =>
          prev
            ? {
                ...prev,
                placement: nextPlacement,
                top: nextTop,
                left: nextLeft,
              }
            : null
        )
      }
      return
    }

    // Regular cells
    const fitsBelow = (anchorBottom + TOOLTIP_GAP + tooltipRect.height) <= (window.innerHeight - VIEWPORT_GUTTER)
    const fitsAbove = (anchorTop - TOOLTIP_GAP - tooltipRect.height) >= VIEWPORT_GUTTER
    const fitsRight = (window.innerWidth - anchorRight - VIEWPORT_GUTTER) >= (tooltipRect.width + TOOLTIP_GAP)
    const fitsLeft = (anchorLeft - VIEWPORT_GUTTER) >= (tooltipRect.width + TOOLTIP_GAP)

    let nextPlacement = tooltip.placement
    let nextTop = tooltip.top
    let nextLeft = Math.max(VIEWPORT_GUTTER, targetLeft)

    if (tooltip.placement === 'below' && !fitsBelow) {
      if (fitsAbove) {
        nextPlacement = 'above'
        nextTop = anchorTop - TOOLTIP_GAP
      } else if (fitsRight) {
        nextPlacement = 'right'
        nextLeft = anchorRight + TOOLTIP_GAP
        nextTop = Math.max(VIEWPORT_GUTTER, Math.min(targetTop ?? anchorTop, window.innerHeight - tooltipRect.height - VIEWPORT_GUTTER))
      } else if (fitsLeft) {
        nextPlacement = 'left'
        nextLeft = anchorLeft - TOOLTIP_GAP
        nextTop = Math.max(VIEWPORT_GUTTER, Math.min(targetTop ?? anchorTop, window.innerHeight - tooltipRect.height - VIEWPORT_GUTTER))
      }
    } else if (tooltip.placement === 'above' && !fitsAbove) {
      if (fitsBelow) {
        nextPlacement = 'below'
        nextTop = anchorBottom + TOOLTIP_GAP
      } else if (fitsRight) {
        nextPlacement = 'right'
        nextLeft = anchorRight + TOOLTIP_GAP
        nextTop = Math.max(VIEWPORT_GUTTER, Math.min(targetTop ?? anchorTop, window.innerHeight - tooltipRect.height - VIEWPORT_GUTTER))
      } else if (fitsLeft) {
        nextPlacement = 'left'
        nextLeft = anchorLeft - TOOLTIP_GAP
        nextTop = Math.max(VIEWPORT_GUTTER, Math.min(targetTop ?? anchorTop, window.innerHeight - tooltipRect.height - VIEWPORT_GUTTER))
      }
    }

    // Horizontal clamping for above and below placements using actual measured width
    if (nextPlacement === 'above' || nextPlacement === 'below') {
      const maxLeft = window.innerWidth - tooltipRect.width - VIEWPORT_GUTTER
      nextLeft = Math.max(VIEWPORT_GUTTER, Math.min(targetLeft, maxLeft))
    }

    if (
      nextPlacement !== tooltip.placement ||
      Math.abs(nextTop - tooltip.top) > 1 ||
      Math.abs(nextLeft - tooltip.left) > 1
    ) {
      setTooltip((prev) =>
        prev
          ? {
              ...prev,
              placement: nextPlacement,
              top: nextTop,
              left: nextLeft,
            }
          : null
      )
    }
  }, [tooltip])

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
        (isAction.getAttribute('aria-expanded') === 'true' ||
         isAction.closest('[aria-expanded="true"]') ||
         Boolean(document.querySelector('.erp-action-menu__popover')))
      ) {
        hideTooltip()
        return
      }

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

        if (
          truncatedElement.getAttribute('aria-expanded') === 'true' ||
          truncatedElement.closest?.('[aria-expanded="true"]') ||
          Boolean(document.querySelector('.erp-action-menu__popover'))
        ) {
          hideTooltip()
          return
        }

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
          ...getTooltipPosition(truncatedElement, cell),
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

    function handleGlobalPointerDown() {
      hideTooltip()
    }

    container.addEventListener('pointerover', handlePointerOver)
    container.addEventListener('pointerout', handlePointerOut)
    container.addEventListener('scroll', hideTooltip, { passive: true })
    document.addEventListener('pointerdown', handleGlobalPointerDown, true)
    window.addEventListener('resize', hideTooltip)
    window.addEventListener('ims-action-menu-open', hideTooltip)
    window.addEventListener('ims:dropdown-opened', hideTooltip)

    return () => {
      hideTooltip()
      container.removeEventListener('pointerover', handlePointerOver)
      container.removeEventListener('pointerout', handlePointerOut)
      container.removeEventListener('scroll', hideTooltip)
      document.removeEventListener('pointerdown', handleGlobalPointerDown, true)
      window.removeEventListener('resize', hideTooltip)
      window.removeEventListener('ims-action-menu-open', hideTooltip)
      window.removeEventListener('ims:dropdown-opened', hideTooltip)
    }
  }, [containerRef])

  if (!tooltip || typeof document === 'undefined') return null

  return createPortal(
    <div
      ref={tooltipRef}
      className={`table-truncated-tooltip table-truncated-tooltip--${tooltip.placement} ${tooltip.isAction ? 'table-truncated-tooltip--action' : ''}`.trim()}
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
