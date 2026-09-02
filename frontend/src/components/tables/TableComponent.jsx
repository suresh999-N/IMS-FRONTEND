import {
  ArrowUpDown,
  Check,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  LoaderCircle,
  SlidersHorizontal,
} from 'lucide-react'
import { Fragment, useEffect, useMemo, useRef, useState } from 'react'
import StateBlock from '../common/StateBlock'
import MobileEntityCard from '../masterData/MobileEntityCard'
import Pagination from '../erp/Pagination'
import SearchBar from '../erp/SearchBar'
import TableToolbar from '../erp/TableToolbar'
import TruncatedCellTooltip from './TruncatedCellTooltip'
import './TableComponent.css'

function getValueFromColumn(column, row) {
  if (!column || !row) return ''

  if (typeof column.sortValue === 'function') {
    return column.sortValue(row)
  }

  if (column.key) {
    let value
    if (typeof column.key === 'string' && column.key.includes('.')) {
      value = column.key.split('.').reduce((acc, part) => acc?.[part], row)
    } else {
      value = row[column.key]
    }

    if (value !== undefined && value !== null) {
      if (typeof value === 'object' && !Array.isArray(value) && !(value instanceof Date)) {
        return value.name ?? value.label ?? value.title ?? value.code ?? value.value ?? value.id ?? String(value)
      }
      return value
    }
  }

  if (typeof column.render === 'function') {
    try {
      const rendered = column.render(row)
      if (typeof rendered === 'string' || typeof rendered === 'number') {
        return rendered
      }
    } catch {
      // Ignore render errors during value extraction
    }
  }

  return ''
}

function getSearchableText(row, columns, searchKeys) {
  if (searchKeys.length > 0) {
    return searchKeys
      .map((key) => String(row[key] ?? '').toLowerCase())
      .join(' ')
  }

  return columns
    .filter((column) => column.searchable !== false)
    .map((column) => {
      if (typeof column.searchValue === 'function') {
        return String(column.searchValue(row) ?? '').toLowerCase()
      }

      if (column.key) {
        return String(row[column.key] ?? '').toLowerCase()
      }

      return ''
    })
    .join(' ')
}

function getRowKey(row, keyField, index) {
  return row?.[keyField] ?? row?.id ?? row?._id ?? row?.productId ?? `row-${index}`
}

function getColumnLabel(column) {
  if (typeof column.mobileLabel === 'string') {
    return column.mobileLabel
  }

  if (typeof column.label === 'string') {
    return column.label
  }

  return column.key ?? ''
}

function isActionsColumn(column) {
  return column.key === 'actions' || String(column.label ?? '').toLowerCase() === 'actions'
}

function isStatusColumn(column) {
  if (column.mobileStatus === true || column.format === 'status') {
    return true
  }

  const value = `${column.key ?? ''} ${column.label ?? ''}`.toLowerCase()
  return ['status', 'state', 'active', 'priority'].some((token) => value.includes(token))
}

function renderCellContent(column, row) {
  return typeof column.render === 'function'
    ? column.render(row)
    : row[column.key]
}

function getMobilePrimaryColumn(columns) {
  return columns.find((column) => column.mobilePrimary) ||
    columns.find((column) => !isActionsColumn(column) && column.mobileHidden !== true) ||
    columns[0]
}

function renderPlainText(value) {
  if (typeof value === 'string' || typeof value === 'number') {
    return value
  }

  return value
}

function isInteractiveTarget(target) {
  return Boolean(
    target?.closest?.(
      'button, a, input, select, textarea, summary, [role="button"], [data-row-click-ignore="true"]',
    ),
  )
}

function getVisiblePages(currentPage, totalPages) {
  const pages = []
  const maxVisiblePages = 5
  let startPage = Math.max(1, currentPage - 2)
  const endPage = Math.min(totalPages, startPage + maxVisiblePages - 1)

  if (endPage - startPage + 1 < maxVisiblePages) {
    startPage = Math.max(1, endPage - maxVisiblePages + 1)
  }

  for (let pageNumber = startPage; pageNumber <= endPage; pageNumber += 1) {
    pages.push(pageNumber)
  }

  return pages
}

function getColumnKey(column, index) {
  return String(column.key || column.label || `column-${index}`)
}

function getInitialVisibleColumnKeys(columns, defaultVisibleColumnKeys, storageKey) {
  const allKeys = columns.map(getColumnKey)
  const fallbackKeys = Array.isArray(defaultVisibleColumnKeys) && defaultVisibleColumnKeys.length > 0
    ? defaultVisibleColumnKeys.filter((key) => allKeys.includes(key))
    : allKeys

  if (!storageKey || typeof window === 'undefined') {
    return fallbackKeys.length > 0 ? fallbackKeys : allKeys
  }

  try {
    const rawValue = window.localStorage.getItem(storageKey)
    const parsedValue = rawValue ? JSON.parse(rawValue) : null

    if (Array.isArray(parsedValue)) {
      const storedKeys = parsedValue.filter((key) => allKeys.includes(key))
      return storedKeys.length > 0 ? storedKeys : fallbackKeys
    }
  } catch {
    // Ignore storage failures and keep table controls usable.
  }

  return fallbackKeys.length > 0 ? fallbackKeys : allKeys
}

function areStringArraysEqual(firstItems = [], secondItems = []) {
  return firstItems.length === secondItems.length &&
    firstItems.every((item, index) => item === secondItems[index])
}

function getColumnWidth(column) {
  const widthValue =
    column?.tableWidth ??
    column?.width ??
    column?.headerStyle?.width ??
    column?.style?.width ??
    column?.headerStyle?.minWidth ??
    column?.style?.minWidth

  if (typeof widthValue === 'number' && Number.isFinite(widthValue)) {
    return `${widthValue}px`
  }

  if (typeof widthValue === 'string' && widthValue.trim()) {
    return widthValue
  }

  return ''
}

function getColumnWidthNumber(column) {
  const width = getColumnWidth(column)
  const parsedWidth = Number.parseFloat(width)

  return Number.isFinite(parsedWidth) && width.endsWith('px') ? parsedWidth : 0
}

export default function TableComponent({
  title,
  subtitle,
  rows,
  columns,
  keyField = 'id',
  searchKeys = [],
  searchPlaceholder = 'Search by name, ID, or category',
  showSearch = true,
  emptyMessage = 'No records available.',
  loading = false,
  defaultPageSize = 6,
  hideSelectionSummary = false,
  defaultSortKey = '',
  defaultSortDirection = 'asc',
  showSubtitle = false,
  toolbarContent = null,
  primaryActionContent = null,
  datasetContent = null,
  footerContent = null,
  filterContent = null,
  splitToolbar = false,
  rowClassName,
  onRowClick,
  renderMobileCard,
  allowSortReset = false,
  showColumnControls = true,
  defaultVisibleColumnKeys = [],
  lockedColumnKeys = [],
  minVisibleColumnCount = 1,
  columnStorageKey = '',
  enableRowSelection = false,
  selectedRowKeys,
  onSelectionChange,
  fitExplicitColumnsToContainer = true,
  showHorizontalScrollbar = false,
}) {
  const columnMenuRef = useRef(null)
  const tableContainerRef = useRef(null)
  const horizontalScrollbarRef = useRef(null)
  const selectAllCheckboxRef = useRef(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(defaultPageSize)
  const [isColumnMenuOpen, setIsColumnMenuOpen] = useState(false)
  const [internalSelectedKeys, setInternalSelectedKeys] = useState([])
  const resolvedColumnStorageKey = useMemo(() => {
    if (columnStorageKey) {
      return columnStorageKey
    }

    if (typeof window === 'undefined') {
      return ''
    }

    const columnSignature = columns.map(getColumnKey).join('-')
    return `ims.table.visibleColumns.${window.location.pathname}.${keyField}.${columnSignature}`
  }, [columnStorageKey, columns, keyField])
  const [visibleColumnKeys, setVisibleColumnKeys] = useState(() =>
    getInitialVisibleColumnKeys(columns, defaultVisibleColumnKeys, resolvedColumnStorageKey),
  )
  const [sortConfig, setSortConfig] = useState({
    key: defaultSortKey,
    direction: defaultSortDirection,
  })
  const selectedKeys = Array.isArray(selectedRowKeys) ? selectedRowKeys : internalSelectedKeys
  const minimumVisibleColumnCount = Math.max(1, Number(minVisibleColumnCount) || 1)

  const effectiveLockedColumnKeys = useMemo(() => {
    const lockedKeys = new Set(lockedColumnKeys)

    columns.forEach((column, index) => {
      const key = getColumnKey(column, index)

      if (column.hideable === false || column.mobilePrimary || isActionsColumn(column)) {
        lockedKeys.add(key)
      }
    })

    return [...lockedKeys]
  }, [columns, lockedColumnKeys])

  useEffect(() => {
    function handlePointerDown(event) {
      if (!columnMenuRef.current?.contains(event.target)) {
        setIsColumnMenuOpen(false)
      }
    }

    document.addEventListener('pointerdown', handlePointerDown)
    return () => document.removeEventListener('pointerdown', handlePointerDown)
  }, [])

  useEffect(() => {
    const allowedKeys = columns.map(getColumnKey)

    setVisibleColumnKeys((currentValue) => {
      const nextKeys = currentValue.filter((key) => allowedKeys.includes(key))

      effectiveLockedColumnKeys.forEach((key) => {
        if (allowedKeys.includes(key) && !nextKeys.includes(key)) {
          nextKeys.push(key)
        }
      })

      const resolvedKeys = nextKeys.length > 0 ? nextKeys : allowedKeys
      return areStringArraysEqual(currentValue, resolvedKeys)
        ? currentValue
        : resolvedKeys
    })
  }, [columns, effectiveLockedColumnKeys])

  useEffect(() => {
    if (!resolvedColumnStorageKey || typeof window === 'undefined') {
      return
    }

    try {
      window.localStorage.setItem(resolvedColumnStorageKey, JSON.stringify(visibleColumnKeys))
    } catch {
      // Ignore storage failures and keep the table usable.
    }
  }, [resolvedColumnStorageKey, visibleColumnKeys])

  useEffect(() => {
    setPage(1)
  }, [searchTerm, pageSize, rows.length])

  useEffect(() => {
    if (!tableContainerRef.current) {
      return
    }

    tableContainerRef.current.scrollLeft = 0
  }, [visibleColumnKeys])

  const displayColumns = useMemo(() => {
    if (!showColumnControls) {
      return columns
    }

    const visibleKeySet = new Set(visibleColumnKeys)
    return columns.filter((column, index) => visibleKeySet.has(getColumnKey(column, index)))
  }, [columns, showColumnControls, visibleColumnKeys])

  const filteredRows = useMemo(() => {
    const normalizedSearch = searchTerm.trim().toLowerCase()

    if (!normalizedSearch) {
      return rows
    }

    return rows.filter((row) =>
      getSearchableText(row, columns, searchKeys).includes(normalizedSearch),
    )
  }, [columns, rows, searchKeys, searchTerm])

  const sortedRows = useMemo(() => {
    if (!sortConfig.key) {
      return filteredRows
    }

    const activeColumn = columns.find(
      (column) => (column.key || column.label) === sortConfig.key || column.key === sortConfig.key,
    )

    if (!activeColumn) {
      return filteredRows
    }

    return [...filteredRows].sort((firstRow, secondRow) => {
      const rawFirst = getValueFromColumn(activeColumn, firstRow)
      const rawSecond = getValueFromColumn(activeColumn, secondRow)

      const isFirstNil = rawFirst === null || rawFirst === undefined || rawFirst === ''
      const isSecondNil = rawSecond === null || rawSecond === undefined || rawSecond === ''

      if (isFirstNil && isSecondNil) return 0
      if (isFirstNil) return 1
      if (isSecondNil) return -1

      const mult = sortConfig.direction === 'asc' ? 1 : -1

      // Booleans
      if (typeof rawFirst === 'boolean' || typeof rawSecond === 'boolean') {
        return (Number(Boolean(rawFirst)) - Number(Boolean(rawSecond))) * mult
      }

      // Numbers or Numeric strings (e.g. 10 vs 2, or "10" vs "2")
      const numFirst = Number(rawFirst)
      const numSecond = Number(rawSecond)
      const isFirstNumeric =
        typeof rawFirst === 'number' ||
        (typeof rawFirst === 'string' && rawFirst.trim() !== '' && !Number.isNaN(numFirst))
      const isSecondNumeric =
        typeof rawSecond === 'number' ||
        (typeof rawSecond === 'string' && rawSecond.trim() !== '' && !Number.isNaN(numSecond))

      if (isFirstNumeric && isSecondNumeric) {
        return (numFirst - numSecond) * mult
      }

      // Dates (ISO timestamp or Date object)
      const parseDate = (val) => {
        if (val instanceof Date) return val.getTime()
        if (typeof val === 'string' && val.length >= 8) {
          const parsed = Date.parse(val)
          if (!Number.isNaN(parsed)) return parsed
        }
        return null
      }

      const dateFirst = parseDate(rawFirst)
      const dateSecond = parseDate(rawSecond)

      if (dateFirst !== null && dateSecond !== null) {
        return (dateFirst - dateSecond) * mult
      }

      // Natural String comparison with localeCompare
      const strFirst = String(rawFirst)
      const strSecond = String(rawSecond)

      return strFirst.localeCompare(strSecond, undefined, { numeric: true, sensitivity: 'base' }) * mult
    })
  }, [columns, filteredRows, sortConfig])

  const totalPages = Math.max(1, Math.ceil(sortedRows.length / pageSize))
  const currentPage = Math.min(page, totalPages)
  const visiblePages = getVisiblePages(currentPage, totalPages)
  const paginatedRows = sortedRows.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize,
  )
  const mobilePrimaryColumn = getMobilePrimaryColumn(displayColumns)
  const mobileActionColumn = displayColumns.find(isActionsColumn)
  const mobileDetailColumns = displayColumns.filter((column) =>
    column !== mobilePrimaryColumn &&
    column !== mobileActionColumn &&
    column.mobileHidden !== true &&
    !isStatusColumn(column),
  )
  const mobileStatusColumns = displayColumns.filter((column) =>
    column !== mobilePrimaryColumn &&
    column !== mobileActionColumn &&
    column.mobileHidden !== true &&
    isStatusColumn(column),
  )
  const hasManualSelectionColumn = displayColumns.some((column) => column.key === 'selection')
  const shouldShowSelection = enableRowSelection && !hasManualSelectionColumn
  const pageRowKeys = paginatedRows.map((row, index) => String(getRowKey(row, keyField, index)))
  const selectedKeySet = new Set(selectedKeys.map(String))
  const selectedPageCount = pageRowKeys.filter((key) => selectedKeySet.has(key)).length
  const isPageSelected = pageRowKeys.length > 0 && selectedPageCount === pageRowKeys.length
  const isPagePartiallySelected = selectedPageCount > 0 && selectedPageCount < pageRowKeys.length

  useEffect(() => {
    if (selectAllCheckboxRef.current) {
      selectAllCheckboxRef.current.indeterminate = false
      selectAllCheckboxRef.current.checked = isPageSelected
    }
  }, [isPageSelected])
  const hideableColumns = columns.filter((column, index) =>
    typeof column.label === 'string' &&
    !effectiveLockedColumnKeys.includes(getColumnKey(column, index)))
  const canShowColumnControls = showColumnControls && hideableColumns.length > 0
  const columnWidths = useMemo(() => displayColumns.map(getColumnWidth), [displayColumns])
  const explicitTableWidth = useMemo(() => {
    const displayColumnWidth = displayColumns.reduce(
      (totalWidth, column) => totalWidth + getColumnWidthNumber(column),
      0,
    )
    const selectionWidth = shouldShowSelection ? 44 : 0
    const totalWidth = displayColumnWidth + selectionWidth

    return totalWidth > 0 ? totalWidth : 0
  }, [displayColumns, shouldShowSelection])
  const tableStyle = explicitTableWidth > 0
    ? {
        '--table-explicit-width': `${explicitTableWidth}px`,
        width: fitExplicitColumnsToContainer ? undefined : `${explicitTableWidth}px`,
        minWidth: fitExplicitColumnsToContainer
          ? `max(${explicitTableWidth}px, 100%)`
          : `${explicitTableWidth}px`,
        tableLayout: 'fixed',
      }
    : undefined

  const handleTableScroll = () => {
    if (showHorizontalScrollbar && horizontalScrollbarRef.current && tableContainerRef.current) {
      horizontalScrollbarRef.current.scrollLeft = tableContainerRef.current.scrollLeft
    }
  }

  const handleHorizontalScrollbarScroll = () => {
    if (horizontalScrollbarRef.current && tableContainerRef.current) {
      tableContainerRef.current.scrollLeft = horizontalScrollbarRef.current.scrollLeft
    }
  }

  function handleSort(column) {
    if (!column.sortable) {
      return
    }

    const columnKey = column.key || column.label

    setSortConfig((currentValue) => {
      if (allowSortReset && currentValue.key === columnKey && currentValue.direction === 'desc') {
        return { key: '', direction: 'asc' }
      }

      return {
        key: columnKey,
        direction:
          currentValue.key === columnKey && currentValue.direction === 'asc'
            ? 'desc'
            : 'asc',
      }
    })
  }

  function handleRowClick(row, event) {
    if (!onRowClick || isInteractiveTarget(event.target)) {
      return
    }

    onRowClick(row)
  }

  function handleRowKeyDown(row, event) {
    if (!onRowClick || isInteractiveTarget(event.target)) {
      return
    }

    if (event.key !== 'Enter' && event.key !== ' ') {
      return
    }

    event.preventDefault()
    onRowClick(row)
  }

  function updateSelection(nextKeys) {
    const normalizedKeys = [...new Set(nextKeys.map(String))]

    if (!Array.isArray(selectedRowKeys)) {
      setInternalSelectedKeys(normalizedKeys)
    }

    onSelectionChange?.(normalizedKeys)
  }

  function handleToggleRowSelection(rowKey) {
    const normalizedKey = String(rowKey)
    updateSelection(
      selectedKeySet.has(normalizedKey)
        ? selectedKeys.filter((key) => String(key) !== normalizedKey)
        : [...selectedKeys, normalizedKey],
    )
  }

  function handleTogglePageSelection() {
    if (isPageSelected || isPagePartiallySelected) {
      updateSelection(selectedKeys.filter((key) => !pageRowKeys.includes(String(key))))
      return
    }

    updateSelection([...selectedKeys, ...pageRowKeys])
  }

  function handleToggleColumn(columnKey) {
    if (effectiveLockedColumnKeys.includes(columnKey)) {
      return
    }

    setVisibleColumnKeys((currentValue) => {
      const hasColumn = currentValue.includes(columnKey)

      if (hasColumn && currentValue.length <= minimumVisibleColumnCount) {
        return currentValue
      }

      const nextKeys = hasColumn
        ? currentValue.filter((key) => key !== columnKey)
        : [...currentValue, columnKey]

      effectiveLockedColumnKeys.forEach((key) => {
        if (!nextKeys.includes(key)) {
          nextKeys.push(key)
        }
      })

      return nextKeys
    })
  }

  function handleResetColumns() {
    const allKeys = columns.map(getColumnKey)
    setVisibleColumnKeys(allKeys)
  }

  const selectionSummary = selectedKeys.length > 0 && shouldShowSelection && !hideSelectionSummary ? (
    <div className="table-component__selection-summary" aria-live="polite">
      <Check size={14} />
      <strong>{selectedKeys.length} selected</strong>
      <button type="button" onClick={() => updateSelection([])}>
        Clear
      </button>
    </div>
  ) : null

  const searchControl = showSearch ? (
    <SearchBar
      value={searchTerm}
      onChange={setSearchTerm}
      placeholder={searchPlaceholder}
    />
  ) : null

  const columnControls = canShowColumnControls ? (
    <div className="table-component__columns-menu" ref={columnMenuRef}>
      <button
        type="button"
        className={`button button-secondary table-component__columns-trigger ${isColumnMenuOpen ? 'is-open' : ''}`.trim()}
        aria-haspopup="menu"
        aria-expanded={isColumnMenuOpen}
        onClick={() => setIsColumnMenuOpen((currentValue) => !currentValue)}
      >
        <SlidersHorizontal size={15} />
        Columns
      </button>

      {isColumnMenuOpen ? (
        <div className="table-component__columns-popover" role="menu">
          <div className="table-component__columns-header">
            <strong>Visible columns</strong>
            <button type="button" onClick={handleResetColumns}>
              Reset
            </button>
          </div>
          <div className="table-component__columns-options">
            {columns.map((column, index) => {
              const columnKey = getColumnKey(column, index)
              const isLocked = effectiveLockedColumnKeys.includes(columnKey)
              const isChecked = visibleColumnKeys.includes(columnKey)
              const wouldBreakMinimum = isChecked && visibleColumnKeys.length <= minimumVisibleColumnCount

              if (typeof column.label !== 'string') {
                return null
              }

              return (
                <label key={columnKey} className="table-component__columns-option">
                  <input
                    type="checkbox"
                    checked={isChecked}
                    disabled={isLocked || wouldBreakMinimum}
                    onChange={() => handleToggleColumn(columnKey)}
                  />
                  <span className="table-component__columns-check" aria-hidden="true">
                    {isChecked ? <Check size={12} /> : null}
                  </span>
                  <span>{column.label}</span>
                </label>
              )
            })}
          </div>
        </div>
      ) : null}
    </div>
  ) : null

  const hasLayeredToolbar = Boolean(primaryActionContent || datasetContent)
  const hasSplitToolbar = splitToolbar && !hasLayeredToolbar

  return (
    <div className="table-component">
      {title || toolbarContent || primaryActionContent || datasetContent || filterContent || showSearch || canShowColumnControls ? (
        <div className="table-component__header">
          {title || (showSubtitle && subtitle) ? (
            <div className="table-component__title-block">
              {title ? <h2 className="section-title">{title}</h2> : null}
              {showSubtitle && subtitle ? <p className="helper-text">{subtitle}</p> : null}
            </div>
          ) : null}

          <TableToolbar className={`table-component__toolbar ${hasLayeredToolbar ? 'table-component__toolbar--layered' : ''} ${hasSplitToolbar ? 'table-component__toolbar--split' : ''}`.trim()}>
            {hasLayeredToolbar ? (
              <>
                <div className="table-component__toolbar-row table-component__toolbar-row--primary">
                  <div className="table-component__toolbar-primary">
                    {selectionSummary}
                    {searchControl}
                  </div>
                  {primaryActionContent ? (
                    <div className="table-component__primary-actions">
                      {primaryActionContent}
                    </div>
                  ) : null}
                </div>
                {filterContent ? (
                  <div className="table-component__toolbar-row table-component__toolbar-row--filters">
                    {filterContent}
                  </div>
                ) : null}
                <div className="table-component__toolbar-row table-component__toolbar-row--dataset">
                  <div className="table-component__dataset-controls">
                    {datasetContent}
                  </div>
                  <div className="table-component__utility-actions">
                    {columnControls}
                    {toolbarContent}
                  </div>
                </div>
              </>
            ) : hasSplitToolbar ? (
              <>
                <div className="table-component__toolbar-left">
                  {selectionSummary}
                  {searchControl}
                  {filterContent}
                </div>
                <div className="table-component__toolbar-right">
                  {columnControls}
                  {toolbarContent}
                </div>
              </>
            ) : (
              <>
                {selectionSummary}
                {searchControl}
                {filterContent}
                {columnControls}
                {toolbarContent}
              </>
            )}
          </TableToolbar>
        </div>
      ) : null}

      {loading ? (
        <div className="table-component__loading" role="status" aria-live="polite">
          <div className="table-component__loading-title">
            <LoaderCircle size={18} className="animate-spin" />
            <span>Loading records...</span>
          </div>
          <div className="table-component__skeleton" aria-hidden="true">
            {Array.from({ length: 5 }).map((_, index) => (
              <span key={index} />
            ))}
          </div>
        </div>
      ) : paginatedRows.length === 0 ? (
        <StateBlock
          type="empty"
          title={emptyMessage}
          message="Try adjusting filters or create a new record when you are ready."
          compact
          className="table-component__empty"
        />
      ) : (
        <>
          <div className="table-container" ref={tableContainerRef} onScroll={handleTableScroll}>
            <table className="table table-component__table" style={tableStyle}>
              {explicitTableWidth > 0 ? (
                <colgroup>
                  {shouldShowSelection ? <col style={{ width: '44px' }} /> : null}
                  {displayColumns.map((column, index) => {
                    const width = columnWidths[index]

                    return (
                      <col
                        key={column.key || column.label || index}
                        style={width ? { width } : undefined}
                      />
                    )
                  })}
                </colgroup>
              ) : null}
              <thead>
                <tr>
                  {shouldShowSelection ? (
                    <th scope="col" className="table-component__selection-cell">
                      <input
                        ref={selectAllCheckboxRef}
                        type="checkbox"
                        checked={isPageSelected}
                        onChange={handleTogglePageSelection}
                        aria-label="Select all rows on this page"
                      />
                    </th>
                  ) : null}
                  {displayColumns.map((column) => (
                    <th
                      key={column.key || column.label}
                      scope="col"
                      className={column.headerClassName || column.className || ''}
                      style={column.headerStyle || column.style}
                    >
                      {column.sortable ? (
                        <button
                          type="button"
                          className={`table-component__sort-button ${sortConfig.key && (sortConfig.key === column.key || sortConfig.key === column.label) ? 'is-active' : ''}`}
                          onClick={() => handleSort(column)}
                        >
                          {column.label}
                          <ArrowUpDown size={14} />
                        </button>
                      ) : (
                        column.label
                      )}
                    </th>
                  ))}
                </tr>
              </thead>

              <tbody>
                {paginatedRows.map((row, index) => {
                  const rowKey = getRowKey(row, keyField, index)

                  return (
                  <tr
                    key={rowKey}
                    data-row-key={rowKey}
                    data-row-index={index}
                    className={`${
                      typeof rowClassName === 'function' ? rowClassName(row) : ''
                    } ${onRowClick ? 'is-clickable' : ''} ${
                      shouldShowSelection && selectedKeySet.has(String(rowKey)) ? 'is-selected' : ''
                    }`.trim()}
                    onClick={onRowClick ? (event) => handleRowClick(row, event) : undefined}
                    onKeyDown={onRowClick ? (event) => handleRowKeyDown(row, event) : undefined}
                    role={onRowClick ? 'button' : undefined}
                    tabIndex={onRowClick ? 0 : undefined}
                    aria-label={onRowClick ? `Open ${renderPlainText(renderCellContent(getMobilePrimaryColumn(displayColumns), row)) || 'record'}` : undefined}
                    aria-selected={shouldShowSelection && selectedKeySet.has(String(rowKey)) ? 'true' : undefined}
                  >
                    {shouldShowSelection ? (
                      <td className="table-component__selection-cell" data-column="selection" data-label="Select">
                        <input
                          type="checkbox"
                          checked={selectedKeySet.has(String(rowKey))}
                          onClick={(event) => event.stopPropagation()}
                          onChange={() => handleToggleRowSelection(rowKey)}
                          aria-label={`Select ${renderPlainText(renderCellContent(getMobilePrimaryColumn(displayColumns), row)) || 'row'}`}
                        />
                      </td>
                    ) : null}
                    {displayColumns.map((column) => {
                      const sNo = (currentPage - 1) * pageSize + index + 1
                      return (
                        <td
                          key={column.key || column.label}
                          data-label={getColumnLabel(column)}
                          data-column={column.key || getColumnLabel(column)}
                          className={column.className || ''}
                          style={column.style}
                        >
                          {typeof column.render === 'function'
                            ? column.render(row, index, sNo)
                            : row[column.key]}
                        </td>
                      )
                    })}
                  </tr>
                )})}
              </tbody>
            </table>
          </div>
          {showHorizontalScrollbar ? (
            <div
              className="table-component__horizontal-scrollbar"
              ref={horizontalScrollbarRef}
              onScroll={handleHorizontalScrollbarScroll}
              role="scrollbar"
              aria-label="Scroll table columns horizontally"
              aria-orientation="horizontal"
            >
              <div style={{ width: `${Math.max(explicitTableWidth, 1)}px` }} />
            </div>
          ) : null}
          <TruncatedCellTooltip containerRef={tableContainerRef} />

          <div className="table-component__mobile-list" aria-label={`${title || 'Records'} mobile list`}>
            {paginatedRows.map((row, index) => {
              const rowKey = getRowKey(row, keyField, index)
              const primaryContent = mobilePrimaryColumn
                ? renderCellContent(mobilePrimaryColumn, row)
                : rowKey
              const visibleDetails = mobileDetailColumns.slice(0, 4)
              const overflowDetails = mobileDetailColumns.slice(4)
              const primaryLabel = getColumnLabel(mobilePrimaryColumn || {})
              const statusContent = mobileStatusColumns.length > 0 ? (
                <>
                  {mobileStatusColumns.slice(0, 2).map((column) => (
                    <span key={column.key || column.label}>
                      {renderCellContent(column, row)}
                    </span>
                  ))}
                </>
              ) : null
              const descriptionColumn = mobileDetailColumns.find((column) => column.mobileDescription)
              const detailColumns = visibleDetails.filter((column) => column !== descriptionColumn)
              const mobileCard = renderMobileCard?.({
                row,
                rowKey,
                rowClassName: typeof rowClassName === 'function' ? rowClassName(row) : '',
                primaryContent,
                primaryLabel,
                statusContent,
                description: descriptionColumn ? renderCellContent(descriptionColumn, row) : null,
                metadata: detailColumns.map((column) => ({
                  key: column.key || column.label,
                  label: getColumnLabel(column),
                  value: renderCellContent(column, row),
                })),
                actions: mobileActionColumn ? renderCellContent(mobileActionColumn, row) : null,
              })

              if (mobileCard) {
                return <Fragment key={rowKey}>{mobileCard}</Fragment>
              }

              return (
                <MobileEntityCard
                  key={rowKey}
                  className={`table-component__mobile-card ${
                    typeof rowClassName === 'function' ? rowClassName(row) : ''
                  } ${onRowClick ? 'is-clickable' : ''}`.trim()}
                  onClick={onRowClick ? (event) => handleRowClick(row, event) : undefined}
                  onKeyDown={onRowClick ? (event) => handleRowKeyDown(row, event) : undefined}
                  eyebrow={primaryLabel}
                  title={renderPlainText(primaryContent)}
                  status={statusContent}
                  description={descriptionColumn ? renderCellContent(descriptionColumn, row) : null}
                  metadata={detailColumns.map((column) => ({
                    key: column.key || column.label,
                    label: getColumnLabel(column),
                    value: renderCellContent(column, row),
                  }))}
                  actions={mobileActionColumn ? renderCellContent(mobileActionColumn, row) : null}
                >
                  {overflowDetails.length > 0 ? (
                    <details className="table-component__mobile-more">
                      <summary>More details</summary>
                      <dl className="table-component__mobile-meta">
                        {overflowDetails.map((column) => (
                          <div key={column.key || column.label}>
                            <dt>{getColumnLabel(column)}</dt>
                            <dd>{renderCellContent(column, row)}</dd>
                          </div>
                        ))}
                      </dl>
                    </details>
                  ) : null}
                </MobileEntityCard>
              )
            })}
          </div>

          <Pagination className="table-component__pagination">
            <div className="table-component__pagination-metrics">
              <label className="table-component__rows-control">
                <span>Rows</span>
                <select
                  value={pageSize}
                  onChange={(event) => setPageSize(Number(event.target.value))}
                >
                  {[8, 10, 15, 20, 25].map((size) => (
                    <option key={size} value={size}>
                      {size}
                    </option>
                  ))}
                </select>
              </label>
              <span className="table-component__status">
                Showing {(currentPage - 1) * pageSize + 1}-
                {Math.min(currentPage * pageSize, sortedRows.length)} of {sortedRows.length}
              </span>
            </div>

            <div className="table-component__page-controls">
              <button
                type="button"
                className="button button-secondary"
                onClick={() => setPage(1)}
                disabled={currentPage === 1}
                aria-label="Go to first page"
              >
                <ChevronsLeft size={16} />
                First
              </button>
              <button
                type="button"
                className="button button-secondary"
                onClick={() => setPage((currentValue) => Math.max(currentValue - 1, 1))}
                disabled={currentPage === 1}
                aria-label="Go to previous page"
              >
                <ChevronLeft size={16} />
                Previous
              </button>
              {visiblePages.map((pageNumber) => (
                <button
                  key={pageNumber}
                  type="button"
                  className={`table-component__page-number ${pageNumber === currentPage ? 'is-active' : ''}`.trim()}
                  onClick={() => setPage(pageNumber)}
                  aria-current={pageNumber === currentPage ? 'page' : undefined}
                  aria-label={`Go to page ${pageNumber}`}
                >
                  {pageNumber}
                </button>
              ))}
              <button
                type="button"
                className="button button-secondary"
                onClick={() =>
                  setPage((currentValue) =>
                    Math.min(currentValue + 1, totalPages),
                  )
                }
                disabled={currentPage === totalPages}
                aria-label="Go to next page"
              >
                Next
                <ChevronRight size={16} />
              </button>
              <button
                type="button"
                className="button button-secondary"
                onClick={() => setPage(totalPages)}
                disabled={currentPage === totalPages}
                aria-label="Go to last page"
              >
                Last
                <ChevronsRight size={16} />
              </button>
            </div>
          </Pagination>
        </>
      )}

      {footerContent}
    </div>
  )
}
