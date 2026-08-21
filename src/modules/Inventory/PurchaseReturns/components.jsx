import { Download, Eye, Pencil, Plus, Search, Trash2 } from 'lucide-react'
import { Link, useNavigate } from 'react-router-dom'
import AppPageHeader from '../../../components/common/PageHeader'
import { showToast } from '../../../components/common/toast'
import { ActionMenu, EmptyState } from '../../../components/erp'
import { exportRowsToCsv } from './purchaseReturnExport'

export function PageHeader({ title, icon, children }) {
  return (
    <AppPageHeader
      icon={icon}
      title={title}
      actions={children}
      className="purchase-returns-page-header"
    />
  )
}

export function RowActions({ view, onEdit, onDelete, label = 'record' }) {
  const navigate = useNavigate()

  return (
    <ActionMenu
      iconOnly
      label={`Actions for ${label}`}
      menuKey={label}
      className="purchase-returns-row-actions"
      actions={[
        view && {
          key: 'view',
          label: 'View details',
          icon: Eye,
          onClick: () => navigate(view),
        },
        onEdit && {
          key: 'edit',
          label: 'Edit',
          icon: Pencil,
          onClick: onEdit,
        },
        onDelete && {
          key: 'delete',
          label: 'Delete',
          icon: Trash2,
          tone: 'danger',
          onClick: onDelete,
        },
      ]}
    />
  )
}

export function RegisterPageActions({
  rows = [],
  filename = 'purchase-returns.csv',
  primaryLabel = '',
  primaryTo = '',
}) {
  return (
    <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center' }}>
      <button
        type="button"
        className="button button-secondary"
        disabled={rows.length === 0}
        onClick={() => {
          exportRowsToCsv(rows, filename)
          showToast({
            type: 'success',
            title: 'Export Complete',
            message: `${rows.length} record(s) exported to CSV.`,
          })
        }}
      >
        <Download size={16} />
        Export
      </button>

      {primaryLabel && primaryTo ? (
        <Link className="button button-primary" to={primaryTo}>
          <Plus size={16} />
          {primaryLabel}
        </Link>
      ) : null}
    </div>
  )
}

export function SearchField({ value, onChange, placeholder = 'Search...' }) {
  return (
    <div style={{ position: 'relative', display: 'inline-flex', alignItems: 'center', width: '100%', maxWidth: '360px' }}>
      <Search size={16} style={{ position: 'absolute', left: '12px', color: '#9ca3af' }} />
      <input
        type="search"
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        style={{
          width: '100%',
          paddingLeft: '2.25rem',
          paddingRight: '0.75rem',
          paddingTop: '0.5rem',
          paddingBottom: '0.5rem',
          border: '1px solid #d1d5db',
          borderRadius: '8px',
          fontSize: '0.875rem',
          outline: 'none',
        }}
      />
    </div>
  )
}

export function EmptyTableRow({ colSpan, title = 'No records found', message = 'No return records available.' }) {
  return (
    <tr>
      <td colSpan={colSpan} style={{ textAlign: 'center', padding: '3rem 1rem' }}>
        <EmptyState title={title} message={message} />
      </td>
    </tr>
  )
}
