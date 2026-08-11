import { useCallback, useEffect, useMemo, useState } from 'react'
import {
  SlidersHorizontal,
  Pencil,
  Plus,
  RefreshCw,
  Save,
  Trash2,
} from 'lucide-react'
import {
  listResource,
  createResource,
  updateResource,
  deleteResource,
} from '../../../api/resourceApi'
import { getAttributeValues } from '../../../api/productApi'
import { RESOURCE_CONFIGS } from '../../ResourceCenter/resourceConfigs'
import FormModal from '../../../layouts/FormModal'
import StateBlock from '../../../components/common/StateBlock'
import InputField from '../../../components/InputField'
import { ActionMenu, DataTable, FilterBar } from '../../../components/erp'
import { showToast } from '../../../components/common/toast'
import { useAuth } from '../../../hooks/useAuth'
import './Attributes.css'

const ATTRIBUTE_COLUMNS_STORAGE_KEY = 'ims.attributes.table.visibleColumns.v1'
const ATTRIBUTE_DEFAULT_COLUMNS = ['name', 'actions']

const config = RESOURCE_CONFIGS.productAttributes

export default function Attributes() {
  const { hasPermission } = useAuth()
  const [attributes, setAttributes] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [error, setError] = useState('')
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingItem, setEditingItem] = useState(null)
  const [deleteTarget, setDeleteTarget] = useState(null)

  const [formValues, setFormValues] = useState({ name: '' })
  const [serverErrors, setServerErrors] = useState({})

  const canCreate = hasPermission('productAttributes', 'create')
  const canEdit = hasPermission('productAttributes', 'edit')
  const canDelete = hasPermission('productAttributes', 'delete')

  // ── Load Data ──────────────────────────────────────────────────────────────
  const loadData = useCallback(async (options = {}) => {
    const force = Boolean(options.force)
    const shouldShowLoading = options.showLoading ?? force

    if (shouldShowLoading) {
      setIsLoading(true)
    }
    setError('')

    try {
      const [attrResponse, valResponse] = await Promise.all([
        listResource(config, { page: 1, pageSize: 500 }, { force }),
        getAttributeValues('', { force }),
      ])

      if (!attrResponse.success) {
        throw new Error(attrResponse.error || 'Failed to load attributes.')
      }

      const attrList = attrResponse.data ?? []
      const valList = valResponse.success ? (valResponse.data ?? []) : []

      // Map values to attributes
      const normalized = attrList.map((attr) => {
        const attributeId = attr.attributeId ?? attr.id
        const matchedValues = valList
          .filter((v) => String(v.attributeId) === String(attributeId))
          .map((v) => v.value || v.name || '')

        return {
          ...attr,
          id: attributeId,
          attributeId,
          values: matchedValues,
        }
      })

      setAttributes(normalized)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to load attributes data.')
      setAttributes([])
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    loadData({ showLoading: true })
  }, [loadData])

  useEffect(() => {
    const refresh = () => loadData({ force: true, showLoading: false })
    const handleVisibilityChange = () => {
      if (document.visibilityState === 'visible') {
        refresh()
      }
    }
    const intervalId = window.setInterval(refresh, 20000)

    window.addEventListener('focus', refresh)
    document.addEventListener('visibilitychange', handleVisibilityChange)

    return () => {
      window.clearInterval(intervalId)
      window.removeEventListener('focus', refresh)
      document.removeEventListener('visibilitychange', handleVisibilityChange)
    }
  }, [loadData])

  // ── Actions ────────────────────────────────────────────────────────────────
  function handleOpenCreate() {
    setEditingItem(null)
    setFormValues({ name: '' })
    setServerErrors({})
    setIsFormOpen(true)
  }

  function handleOpenEdit(item) {
    setEditingItem(item)
    setFormValues({ name: item.name || '' })
    setServerErrors({})
    setIsFormOpen(true)
  }

  function handleCloseForm() {
    setIsFormOpen(false)
    setEditingItem(null)
    setFormValues({ name: '' })
    setServerErrors({})
  }

  async function handleFormSubmit(e) {
    e.preventDefault()
    if (!formValues.name.trim()) {
      setServerErrors({ name: 'Attribute name is required.' })
      return
    }

    setIsSaving(true)
    setServerErrors({})

    const isEdit = Boolean(editingItem)
    const payload = { name: formValues.name.trim() }

    try {
      let response
      if (isEdit) {
        response = await updateResource(config, editingItem.attributeId, payload, payload)
      } else {
        response = await createResource(config, payload)
      }

      if (!response.success) {
        throw new Error(response.error || 'Failed to save attribute.')
      }

      showToast({
        type: 'success',
        title: 'Attributes',
        message: `Attribute ${isEdit ? 'updated' : 'created'} successfully.`,
      })

      setIsFormOpen(false)
      setEditingItem(null)
      setFormValues({ name: '' })
      await loadData({ force: true, showLoading: false })
    } catch (err) {
      showToast({
        type: 'error',
        title: 'Attributes',
        message: err instanceof Error ? err.message : 'Unable to save attribute.',
      })
    } finally {
      setIsSaving(false)
    }
  }

  async function confirmDelete() {
    if (!deleteTarget) return
    setIsDeleting(true)

    try {
      const response = await deleteResource(config, deleteTarget.attributeId)
      if (!response.success) {
        throw new Error(response.error || 'Failed to delete attribute.')
      }

      showToast({
        type: 'success',
        title: 'Attributes',
        message: 'Attribute deleted successfully.',
      })

      setDeleteTarget(null)
      await loadData({ force: true, showLoading: false })
    } catch (err) {
      showToast({
        type: 'error',
        title: 'Attributes',
        message: err instanceof Error ? err.message : 'Unable to delete attribute.',
      })
    } finally {
      setIsDeleting(false)
    }
  }

  // ── Column Definitions ─────────────────────────────────────────────────────
  const columns = useMemo(
    () => [

      {
        key: 'name',
        label: 'Attribute Name',
        sortable: true,
        mobilePrimary: true,
        className: 'attributes-col-name',
        searchValue: (item) => `${item.name} ${item.values?.join(' ') || ''}`,
        render: (item) => (
          <div className="attributes__identity">
            <strong title={item.name || 'Unnamed Attribute'}>
              {item.name || 'Unnamed Attribute'}
            </strong>
          </div>
        ),
      },
      {
        key: 'actions',
        label: 'Actions',
        className: 'attributes-col-actions',
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
            <div className="attributes-page__row-actions">
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
    [canEdit, canDelete],
  )

  // ── Toolbar Rendering ──────────────────────────────────────────────────────
  const primaryToolbarContent = null

  const toolbarContent = useMemo(
    () => (
      <FilterBar className="attributes__toolbar-actions" ariaLabel="Attribute table actions">
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
    ),
    [isLoading, loadData],
  )

  // ── Modal Forms ────────────────────────────────────────────────────────────
  const isFormValid = formValues.name.trim().length >= 2
  const hasFormChanges = editingItem ? formValues.name.trim() !== (editingItem.name || '') : formValues.name.trim() !== ''

  // ── Render ─────────────────────────────────────────────────────────────────
  if (isLoading && attributes.length === 0) {
    return (
      <div className="page resource-center">
        <StateBlock type="loading" message="Loading attributes..." />
      </div>
    )
  }

  if (error && attributes.length === 0) {
    return (
      <div className="page resource-center">
        <StateBlock
          type="error"
          message={error}
          actionLabel="Retry"
          onAction={() => loadData({ force: true, showLoading: true })}
        />
      </div>
    )
  }

  return (
    <div className="page resource-center">
      <div className="resource-center__page resource-center__page--attributes">
        {/* Header */}
        <header className="resource-center__inventory-header" aria-label="Attributes summary">
          <div className="resource-center__inventory-header-main">
            <h1>Product Attributes</h1>
            <div className="resource-center__inventory-metrics" aria-label="Attributes metrics">
              <span className="resource-center__inventory-metric resource-center__inventory-metric--success">
                {attributes.length} Attributes
              </span>
            </div>
          </div>
          <div className="resource-center__inventory-header-actions">
            {canCreate ? (
              <button className="button button-primary" onClick={handleOpenCreate}>
                <Plus size={16} />
                Add Attribute
              </button>
            ) : null}
          </div>
        </header>

        {/* Table Card */}
        <div className="card resource-center__inventory-table-card">
          <DataTable
            className="resource-center__inventory-table"
            columns={columns}
            rows={attributes}
            keyField="attributeId"
            searchPlaceholder="Search Attributes..."
            loading={isLoading}
            showSearch={true}
            splitToolbar
            fitExplicitColumnsToContainer
            filterContent={primaryToolbarContent}
            toolbarContent={toolbarContent}
            columnStorageKey={ATTRIBUTE_COLUMNS_STORAGE_KEY}
            defaultVisibleColumns={ATTRIBUTE_DEFAULT_COLUMNS}
          />
        </div>
      </div>

      {/* Save Modal */}
      {isFormOpen ? (
        <FormModal
          title={editingItem ? 'Edit Attribute' : 'Add Attribute'}
          onClose={handleCloseForm}
          className="form-modal--attributes"
          dialogClassName="form-modal__dialog--attributes"
          bodyClassName="form-modal__body--attributes"
        >
          <form onSubmit={handleFormSubmit} className="resource-form" noValidate>
            <div className="resource-form__section">
              <div className="field">
                <InputField
                  label="Attribute Name *"
                  name="name"
                  value={formValues.name}
                  onChange={(e) => setFormValues({ name: e.target.value })}
                  error={serverErrors.name}
                  required
                  placeholder="e.g. Size, Color, Material"
                  disabled={isSaving}
                />
              </div>
            </div>

            <div className="resource-form__footer">
              <button
                type="submit"
                className="button button-primary"
                disabled={!isFormValid || !hasFormChanges || isSaving}
              >
                <Save size={16} />
                {isSaving ? 'Saving...' : 'Save Attribute'}
              </button>
              <button
                type="button"
                className="button"
                onClick={handleCloseForm}
                disabled={isSaving}
              >
                Cancel
              </button>
            </div>
          </form>
        </FormModal>
      ) : null}

      {/* Delete Confirmation Modal */}
      {deleteTarget ? (
        <FormModal
          title="Delete Attribute"
          onClose={() => setDeleteTarget(null)}
          className="form-modal--delete-confirm"
        >
          <p>
            Are you sure you want to delete{' '}
            <strong>{deleteTarget.name || deleteTarget.attributeName || 'this attribute'}</strong>?
            This action cannot be undone.
          </p>
          <div className="resource-form__footer">
            <button
              type="button"
              className="button button-secondary"
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
              {isDeleting ? 'Deleting...' : 'Delete'}
            </button>
          </div>
        </FormModal>
      ) : null}
    </div>
  )
}
