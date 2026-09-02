import { useCallback, useEffect, useMemo, useState } from 'react'
import {
  ClipboardList,
  Pencil,
  Plus,
  RefreshCw,
  Save,
  Trash2,
} from 'lucide-react'
import {
  createUnit,
  deleteUnit,
  formatUnitTitle,
  getUnits,
  normalizeUnit,
  updateUnit,
} from '../../../api/productApi'
import FormModal from '../../../layouts/FormModal'
import StateBlock from '../../../components/common/StateBlock'
import InputField from '../../../components/InputField'
import { ActionMenu, DataTable, FilterBar, StatusBadge } from '../../../components/erp'
import { showToast } from '../../../components/common/toast'
import { useAuth } from '../../../hooks/useAuth'
import './Units.css'

const UNITS_COLUMNS_STORAGE_KEY = 'ims.units.table.visibleColumns.warehouseParity.v1'
const UNITS_DEFAULT_COLUMNS = ['name', 'shortName', 'status', 'actions']

export default function Units() {
  const { hasPermission } = useAuth()
  const [units, setUnits] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [error, setError] = useState('')

  // Modals state
  const [isCreateOpen, setCreateOpen] = useState(false)
  const [editingItem, setEditingItem] = useState(null)
  const [deleteTarget, setDeleteTarget] = useState(null)

  // Filters state
  const [searchTerm, setSearchTerm] = useState('')

  // Form state
  const [formValues, setFormValues] = useState({ name: '', shortName: '' })
  const [serverErrors, setServerErrors] = useState({})

  const canCreate = hasPermission('units', 'create')
  const canEdit = hasPermission('units', 'edit')
  const canDelete = hasPermission('units', 'delete')

  // ── Load Data ──────────────────────────────────────────────────────────────
  const loadData = useCallback(async (options = {}) => {
    const force = Boolean(options.force)
    const shouldShowLoading = options.showLoading ?? force

    if (shouldShowLoading) {
      setIsLoading(true)
    }
    setError('')

    try {
      const response = await getUnits({ force })

      if (!response.success) {
        throw new Error(response.error || 'Failed to load units.')
      }

      const rawList = response.data ?? []
      const normalized = rawList.map(normalizeUnit)

      setUnits(normalized)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to load units data.')
      setUnits([])
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    loadData({ showLoading: true })
  }, [loadData])

  // ── Handlers & Actions ──────────────────────────────────────────────────────
  const handleOpenCreate = () => {
    setEditingItem(null)
    setFormValues({ name: '', shortName: '' })
    setServerErrors({})
    setCreateOpen(true)
  }

  const handleOpenEdit = (item) => {
    setEditingItem(item)
    setFormValues({
      name: item.name || '',
      shortName: item.shortName || '',
    })
    setServerErrors({})
    setCreateOpen(true)
  }

  const handleCloseModal = () => {
    setCreateOpen(false)
    setEditingItem(null)
  }

  const handleSave = async (e) => {
    e.preventDefault()
    setServerErrors({})

    const name = formValues.name.trim()
    const shortName = formValues.shortName.trim()
    const errors = {}

    // Frontend validation - Empty check
    if (!name) {
      errors.name = 'Unit Name is required.'
    } else if (name.length < 2) {
      errors.name = 'Unit Name must be at least 2 characters.'
    } else if (name.length > 50) {
      errors.name = 'Unit Name cannot exceed 50 characters.'
    } else if (!/^[a-zA-Z0-9\s\-/°%()]+$/.test(name)) {
      errors.name = 'Unit Name contains invalid characters.'
    }

    if (!shortName) {
      errors.shortName = 'Abbreviation / Symbol is required.'
    } else if (shortName.length > 20) {
      errors.shortName = 'Abbreviation cannot exceed 20 characters.'
    } else if (!/^[a-zA-Z0-9\s\-/°%().]+$/.test(shortName)) {
      errors.shortName = 'Abbreviation contains invalid characters.'
    }

    // Check duplicates locally if format is valid
    if (!errors.name) {
      const isDuplicateName = units.some(
        (u) =>
          (!editingItem || String(u.id) !== String(editingItem.id)) &&
          u.name.toLowerCase() === name.toLowerCase(),
      )
      if (isDuplicateName) {
        errors.name = 'Unit Name already exists.'
      }
    }

    if (!errors.shortName) {
      const isDuplicateShortName = units.some(
        (u) =>
          (!editingItem || String(u.id) !== String(editingItem.id)) &&
          u.shortName.toLowerCase() === shortName.toLowerCase(),
      )
      if (isDuplicateShortName) {
        errors.shortName = 'Unit Abbreviation already exists.'
      }
    }

    if (Object.keys(errors).length > 0) {
      setServerErrors(errors)
      return
    }

    setIsSaving(true)

    const payload = {
      name,
      shortName,
      unitName: name,
      short_name: shortName,
      abbreviation: shortName,
      unitAbbreviation: shortName,
    }

    try {
      let response
      if (editingItem) {
        response = await updateUnit(editingItem.id, payload)
      } else {
        response = await createUnit(payload)
      }

      if (!response.success) {
        if (response.validationErrors) {
          setServerErrors(response.validationErrors)
        }
        throw new Error(response.error || 'Failed to save unit.')
      }

      showToast(editingItem ? 'Unit updated successfully.' : 'Unit created successfully.', 'success')
      handleCloseModal()
      loadData({ force: true, showLoading: false })
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'An error occurred while saving.', 'error')
    } finally {
      setIsSaving(false)
    }
  }

  const confirmDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)

    try {
      const response = await deleteUnit(deleteTarget.id)

      if (!response.success) {
        throw new Error(response.error || 'Failed to delete unit.')
      }

      showToast('Unit deleted successfully.', 'success')
      setDeleteTarget(null)
      loadData({ force: true, showLoading: false })
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'An error occurred while deleting.', 'error')
    } finally {
      setIsDeleting(false)
    }
  }

  // ── Filters logic ───────────────────────────────────────────────────────────
  const filteredUnits = useMemo(() => {
    if (!searchTerm) return units

    const query = searchTerm.toLowerCase()
    return units.filter(
      (u) =>
        u.name.toLowerCase().includes(query) ||
        u.shortName.toLowerCase().includes(query)
    )
  }, [units, searchTerm])

  // ── Columns config ──────────────────────────────────────────────────────────
  const columns = useMemo(
    () => [

      {
        key: 'name',
        label: 'Unit Name',
        sortable: true,
        render: (item) => (
          <div className="units__identity">
            <strong title={item.name}>{item.name}</strong>
          </div>
        ),
      },
      {
        key: 'shortName',
        label: 'Abbreviation',
        sortable: true,
        tableWidth: 180,
        style: { width: 180, minWidth: 180 },
        headerStyle: { width: 180, minWidth: 180 },
        render: (item) => <span className="font-mono text-sm">{item.shortName}</span>,
      },
      {
        key: 'status',
        label: 'Status',
        sortable: false,
        tableWidth: 100,
        style: { width: 100, minWidth: 100 },
        headerStyle: { width: 100, minWidth: 100 },
        render: () => <StatusBadge type="active">Active</StatusBadge>,
      },
      {
        key: 'actions',
        label: 'Actions',
        className: 'units-col-actions',
        tableWidth: 80,
        style: { width: 80, minWidth: 80 },
        headerStyle: { width: 80, minWidth: 80 },
        render: (item) => {
          const menuItems = [
            canEdit
              ? {
                  key: 'edit',
                  label: 'Edit',
                  icon: Pencil,
                  onClick: () => handleOpenEdit(item),
                }
              : null,
            canDelete
              ? {
                  key: 'delete',
                  label: 'Delete',
                  icon: Trash2,
                  variant: 'danger',
                  onClick: () => setDeleteTarget(item),
                }
              : null,
          ].filter(Boolean)

          if (menuItems.length === 0) return null

          return (
            <div className="units-page__row-actions">
              <ActionMenu
                iconOnly
                className="inventory-row-action-menu"
                label={`Actions for ${item.name}`}
                actions={menuItems}
              />
            </div>
          )
        },
      },
    ],
    [canEdit, canDelete]
  )

  // ── Render ──────────────────────────────────────────────────────────────────
  if (error && units.length === 0) {
    return (
      <div className="page resource-center__page--units">
        <StateBlock
          type="error"
          title="Failed to Load Units"
          description={error}
          actions={
            <button type="button" className="button button-primary" onClick={() => loadData({ force: true })}>
              Try Again
            </button>
          }
        />
      </div>
    )
  }

  return (
    <div className="page resource-center__page--units">
      {/* Page Header */}
      <div className="resource-center__inventory-header">
        <div className="resource-center__inventory-header-main">
          <h1>Units</h1>
          <div className="resource-center__inventory-metrics">
            <span className="resource-center__inventory-metric resource-center__inventory-metric--success">
              {filteredUnits.length} Units
            </span>
          </div>
        </div>
        {canCreate && (
          <div className="resource-center__inventory-header-actions">
            <button type="button" className="button button-primary" onClick={handleOpenCreate}>
              <Plus size={16} />
              Add Unit
            </button>
          </div>
        )}
      </div>

      {/* Main Table Card */}
      <div className="card resource-center__inventory-table-card">
        <DataTable
          className="resource-center__inventory-table"
          columns={columns}
          rows={filteredUnits}
          keyField="id"
          searchPlaceholder="Search by name or symbol..."
          loading={isLoading}
          showSearch={true}
          splitToolbar
          fitExplicitColumnsToContainer
          onSearchChange={setSearchTerm}
          searchValue={searchTerm}
          toolbarContent={
            <FilterBar className="units__toolbar-actions" ariaLabel="Unit table actions">
              <button
                type="button"
                className="button button-secondary"
                onClick={() => loadData({ force: true, showLoading: true })}
                disabled={isLoading}
              >
                <RefreshCw size={16} className={isLoading ? 'animate-spin' : ''} />
                Refresh
              </button>
            </FilterBar>
          }
          columnStorageKey={UNITS_COLUMNS_STORAGE_KEY}
          defaultVisibleColumns={UNITS_DEFAULT_COLUMNS}
        />
      </div>

      {/* Add / Edit Form Modal */}
      {isCreateOpen && (
        <FormModal
          title={editingItem ? 'Edit Unit' : 'Add Unit'}
          subtitle={editingItem ? 'Modify existing unit abbreviation or title.' : undefined}
          className="form-modal--units"
          onClose={handleCloseModal}
        >
          <form className="catalog-form" onSubmit={handleSave}>
            <div className="form-modal__body--units">
              <div className="resource-form__section">
                <div className="resource-form__grid-2">
                  <div className="resource-form__field">
                    <label htmlFor="unitName">Unit Name *</label>
                    <InputField
                      id="unitName"
                      name="name"
                      value={formValues.name}
                      onChange={(e) => {
                        const val = typeof e === 'object' && e !== null && 'target' in e ? e.target.value : e
                        setFormValues((prev) => ({ ...prev, name: val }))
                        if (serverErrors.name) setServerErrors((prev) => ({ ...prev, name: '' }))
                      }}
                      placeholder="e.g. Kilogram"
                      error={serverErrors.name}
                      showError={Boolean(serverErrors.name)}
                    />
                  </div>

                  <div className="resource-form__field">
                    <label htmlFor="unitAbbreviation">Abbreviation / Symbol *</label>
                    <InputField
                      id="unitAbbreviation"
                      name="shortName"
                      value={formValues.shortName}
                      onChange={(e) => {
                        const val = typeof e === 'object' && e !== null && 'target' in e ? e.target.value : e
                        setFormValues((prev) => ({ ...prev, shortName: val }))
                        if (serverErrors.shortName) setServerErrors((prev) => ({ ...prev, shortName: '' }))
                      }}
                      placeholder="e.g. kg"
                      error={serverErrors.shortName}
                      showError={Boolean(serverErrors.shortName)}
                    />
                  </div>
                </div>
              </div>
            </div>

            <div className="button-row form-modal__footer">
              <button type="submit" className="button button-primary" disabled={isSaving}>
                <Save size={16} />
                {isSaving ? 'Saving...' : 'Save Unit'}
              </button>
              <button className="button button-cancel button-secondary"
                type="button"
                onClick={handleCloseModal}
                disabled={isSaving}
              >
                Cancel
              </button>
            </div>
          </form>
        </FormModal>
      )}

      {/* Delete Confirmation Modal */}
      {deleteTarget && (
        <FormModal
          title="Delete Unit"
          onClose={() => {
            if (!isDeleting) {
              setDeleteTarget(null)
            }
          }}
        >
          <div className="resource-center__delete-dialog">
            <div className="delete-confirmation__copy">
              <p>
                Are you sure you want to delete <strong>{deleteTarget.name}</strong>{' '}
                (<code>{deleteTarget.shortName}</code>)?
              </p>
              <p className="delete-confirmation__warning">This action cannot be undone.</p>
            </div>
            <div className="button-row">
              <button className="button button-cancel button-secondary"
                type="button"
                onClick={() => setDeleteTarget(null)}
                disabled={isDeleting}
              >
                Cancel
              </button>
              <button
                type="button"
                className="button button-danger"
                onClick={confirmDelete}
                disabled={isDeleting}
              >
                <Trash2 size={16} />
                {isDeleting ? 'Deleting...' : 'Delete Unit'}
              </button>
            </div>
          </div>
        </FormModal>
      )}
    </div>
  )
}
