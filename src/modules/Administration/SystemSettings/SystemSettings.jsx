import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import {
  AlertCircle,
  Bell,
  Boxes,
  Building2,
  CheckCircle2,
  DatabaseBackup,
  FileText,
  Loader2,
  LockKeyhole,
  Package,
  ReceiptText,
  RefreshCw,
  RotateCcw,
  Save,
  Settings2,
  ShieldCheck,
  Truck,
  WalletCards,
} from 'lucide-react'
import {
  fetchAllSystemSettings,
  resetSectionSettings,
  saveSectionSettings,
  SYSTEM_SETTINGS_UPDATED_EVENT,
} from '../../../api/systemSettingsApi'
import { showToast } from '../../../components/common/toast'
import './SystemSettings.css'

// ---------------------------------------------------------------------------
// Section definitions â€” sectionKey must exactly match the key in
// API_ENDPOINTS.systemSettings  (endpoints.js)
// ---------------------------------------------------------------------------
const sections = [
  {
    key: 'productRules',
    title: 'Product Rules',
    subtitle: 'Control product code, SKU, variants and required product fields.',
    icon: Package,
    fields: [
      { name: 'autoGenerateProductCode', label: 'Auto Generate Product Code', type: 'toggle', defaultValue: true },
      { name: 'productCodePrefix', label: 'Product Code Prefix', type: 'text', defaultValue: 'PRD-' },
      { name: 'skuPrefix', label: 'SKU Prefix', type: 'text', defaultValue: 'SKU-' },
      { name: 'allowProductVariants', label: 'Allow Product Variants', type: 'toggle', defaultValue: true },
      { name: 'brandRequired', label: 'Brand Required', type: 'toggle', defaultValue: true },
      { name: 'categoryRequired', label: 'Category Required', type: 'toggle', defaultValue: true },
      { name: 'subCategoryRequired', label: 'SubCategory Required', type: 'toggle', defaultValue: false },
      { name: 'attributeRequiredForVariants', label: 'Attribute Required For Variants', type: 'toggle', defaultValue: true },
      { name: 'hsnCodeRequired', label: 'HSN Code Required', type: 'toggle', defaultValue: false },
      { name: 'productImageRequired', label: 'Product Image Required', type: 'toggle', defaultValue: false },
      { name: 'duplicateProductNameAllowed', label: 'Duplicate Product Name Allowed', type: 'toggle', defaultValue: false },
    ],
  },
  {
    key: 'purchaseRules',
    title: 'Purchase & Goods Receipt',
    subtitle: 'Control purchase order, supplier approval and goods receipt rules.',
    icon: Truck,
    fields: [
      { name: 'purchaseOrderPrefix', label: 'Purchase Order Prefix', type: 'text', defaultValue: 'PO-' },
      { name: 'autoGeneratePoNumber', label: 'Auto Generate PO Number', type: 'toggle', defaultValue: true },
      {
        name: 'defaultPurchaseStatus', label: 'Default Purchase Status', type: 'select', defaultValue: 'pending',
        options: [
          { value: 'draft', label: 'Draft' },
          { value: 'pending', label: 'Pending' },
          { value: 'approved', label: 'Approved' },
          { value: 'posted', label: 'Posted' },
        ],
      },
      { name: 'purchaseApprovalRequired', label: 'Purchase Approval Required', type: 'toggle', defaultValue: true },
      { name: 'supplierApprovalRequired', label: 'Supplier Approval Required', type: 'toggle', defaultValue: false },
      { name: 'goodsReceiptApprovalRequired', label: 'Goods Receipt Approval Required', type: 'toggle', defaultValue: true },
      { name: 'allowPartialGoodsReceipt', label: 'Allow Partial Goods Receipt', type: 'toggle', defaultValue: true },
      { name: 'allowPurchaseWithoutSupplier', label: 'Allow Purchase Without Supplier', type: 'toggle', defaultValue: false },
      { name: 'allowPurchasePriceOverride', label: 'Allow Purchase Price Override', type: 'toggle', defaultValue: true },
      { name: 'requireAttachmentForPurchase', label: 'Require Attachment For Purchase', type: 'toggle', defaultValue: false },
    ],
  },
  {
    key: 'salesRules',
    title: 'Sales & Invoice',
    subtitle: 'Control invoice number, sales status, payments and discount approval.',
    icon: ReceiptText,
    fields: [
      { name: 'invoicePrefix', label: 'Invoice Prefix', type: 'text', defaultValue: 'INV-' },
      { name: 'autoGenerateInvoiceNumber', label: 'Auto Generate Invoice Number', type: 'toggle', defaultValue: true },
      {
        name: 'defaultSalesStatus', label: 'Default Sales Status', type: 'select', defaultValue: 'pending',
        options: [
          { value: 'draft', label: 'Draft' },
          { value: 'pending', label: 'Pending' },
          { value: 'confirmed', label: 'Confirmed' },
          { value: 'completed', label: 'Completed' },
        ],
      },
      {
        name: 'defaultPaymentStatus', label: 'Default Payment Status', type: 'select', defaultValue: 'unpaid',
        options: [
          { value: 'unpaid', label: 'Unpaid' },
          { value: 'partial', label: 'Partial' },
          { value: 'paid', label: 'Paid' },
        ],
      },
      { name: 'defaultPaymentTerms', label: 'Default Payment Terms', type: 'text', defaultValue: 'Immediate' },
      { name: 'requireCustomerForSale', label: 'Require Customer For Sale', type: 'toggle', defaultValue: true },
      { name: 'allowPartialPayment', label: 'Allow Partial Payment', type: 'toggle', defaultValue: true },
      { name: 'allowSalesWithoutStock', label: 'Allow Sales Without Stock', type: 'toggle', defaultValue: false },
      { name: 'discountLimitPercentage', label: 'Discount Limit Percentage', type: 'number', defaultValue: 10 },
      { name: 'requireApprovalForHighDiscount', label: 'Require Approval For High Discount', type: 'toggle', defaultValue: true },
    ],
  },
  {
    key: 'returnRules',
    title: 'Return & Refund',
    subtitle: 'Control sales returns, refund approval and returned stock behavior.',
    icon: RotateCcw,
    fields: [
      { name: 'allowSalesReturn', label: 'Allow Sales Return', type: 'toggle', defaultValue: true },
      { name: 'salesReturnDays', label: 'Sales Return Days', type: 'number', defaultValue: 7 },
      { name: 'returnApprovalRequired', label: 'Return Approval Required', type: 'toggle', defaultValue: true },
      { name: 'refundApprovalRequired', label: 'Refund Approval Required', type: 'toggle', defaultValue: true },
      { name: 'autoRestockReturnedItems', label: 'Auto Restock Returned Items', type: 'toggle', defaultValue: false },
      { name: 'requireReturnReason', label: 'Require Return Reason', type: 'toggle', defaultValue: true },
      { name: 'allowPartialReturn', label: 'Allow Partial Return', type: 'toggle', defaultValue: true },
      { name: 'returnNumberPrefix', label: 'Return Number Prefix', type: 'text', defaultValue: 'RET-' },
    ],
  },
  {
    key: 'taxRules',
    title: 'Tax & Billing',
    subtitle: 'Control GST, tax calculation, rounding and invoice tax display.',
    icon: WalletCards,
    fields: [
      { name: 'gstEnabled', label: 'GST Enabled', type: 'toggle', defaultValue: true },
      { name: 'defaultGstPercentage', label: 'Default GST Percentage', type: 'number', defaultValue: 18 },
      { name: 'cgstPercentage', label: 'CGST Percentage', type: 'number', defaultValue: 9 },
      { name: 'sgstPercentage', label: 'SGST Percentage', type: 'number', defaultValue: 9 },
      { name: 'igstPercentage', label: 'IGST Percentage', type: 'number', defaultValue: 18 },
      { name: 'taxInclusivePricing', label: 'Tax Inclusive Pricing', type: 'toggle', defaultValue: false },
      { name: 'showTaxOnInvoice', label: 'Show Tax On Invoice', type: 'toggle', defaultValue: true },
      { name: 'roundOffInvoiceAmount', label: 'Round Off Invoice Amount', type: 'toggle', defaultValue: true },
      { name: 'decimalPlacesForAmount', label: 'Decimal Places For Amount', type: 'number', defaultValue: 2 },
    ],
  },
  {
    key: 'stockRules',
    title: 'Advanced Stock Control',
    subtitle: 'Control stock adjustment, transfer, audit, batch, expiry and damage rules.',
    icon: Boxes,
    fields: [
      { name: 'allowNegativeStock', label: 'Allow Negative Stock', type: 'toggle', defaultValue: false },
      { name: 'stockAdjustmentApprovalRequired', label: 'Stock Adjustment Approval Required', type: 'toggle', defaultValue: true },
      { name: 'stockTransferApprovalRequired', label: 'Stock Transfer Approval Required', type: 'toggle', defaultValue: true },
      { name: 'stockAuditApprovalRequired', label: 'Stock Audit Approval Required', type: 'toggle', defaultValue: true },
      { name: 'batchNumberRequired', label: 'Batch Number Required', type: 'toggle', defaultValue: false },
      { name: 'expiryTrackingEnabled', label: 'Expiry Tracking Enabled', type: 'toggle', defaultValue: false },
      { name: 'damageStockTrackingEnabled', label: 'Damage Stock Tracking Enabled', type: 'toggle', defaultValue: true },
      { name: 'requireReasonForStockAdjustment', label: 'Require Reason For Stock Adjustment', type: 'toggle', defaultValue: true },
      { name: 'allowBackdatedStockEntry', label: 'Allow Backdated Stock Entry', type: 'toggle', defaultValue: false },
      { name: 'stockMovementLockAfterDays', label: 'Stock Movement Lock After Days', type: 'number', defaultValue: 7 },
    ],
  },
  {
    key: 'warehouseRules',
    title: 'Warehouse / Bin / Rack',
    subtitle: 'Control warehouse, bin, rack and stock location rules.',
    icon: Building2,
    fields: [
      { name: 'warehouseRequiredForStock', label: 'Warehouse Required For Stock', type: 'toggle', defaultValue: true },
      { name: 'binRequiredForStock', label: 'Bin Required For Stock', type: 'toggle', defaultValue: false },
      { name: 'rackRequiredForStock', label: 'Rack Required For Stock', type: 'toggle', defaultValue: false },
      { name: 'autoAssignBin', label: 'Auto Assign Bin', type: 'toggle', defaultValue: false },
      { name: 'autoPutawayEnabled', label: 'Auto Putaway Enabled', type: 'toggle', defaultValue: false },
      { name: 'allowInterWarehouseTransfer', label: 'Allow Inter-Warehouse Transfer', type: 'toggle', defaultValue: true },
      { name: 'requireApprovalForWarehouseTransfer', label: 'Require Approval For Warehouse Transfer', type: 'toggle', defaultValue: true },
      { name: 'allowStockInInactiveWarehouse', label: 'Allow Stock In Inactive Warehouse', type: 'toggle', defaultValue: false },
    ],
  },
  {
    key: 'auditRules',
    title: 'Audit Log Rules',
    subtitle: 'Control audit tracking, settings changes and log retention.',
    icon: ShieldCheck,
    fields: [
      { name: 'enableAuditLogs', label: 'Enable Audit Logs', type: 'toggle', defaultValue: true },
      { name: 'trackUserLogin', label: 'Track User Login', type: 'toggle', defaultValue: true },
      { name: 'trackProductChanges', label: 'Track Product Changes', type: 'toggle', defaultValue: true },
      { name: 'trackStockChanges', label: 'Track Stock Changes', type: 'toggle', defaultValue: true },
      { name: 'trackPurchaseChanges', label: 'Track Purchase Changes', type: 'toggle', defaultValue: true },
      { name: 'trackSalesChanges', label: 'Track Sales Changes', type: 'toggle', defaultValue: true },
      { name: 'trackPaymentChanges', label: 'Track Payment Changes', type: 'toggle', defaultValue: true },
      { name: 'trackSettingsChanges', label: 'Track Settings Changes', type: 'toggle', defaultValue: true },
      { name: 'logRetentionDays', label: 'Log Retention Days', type: 'number', defaultValue: 180 },
      { name: 'allowExportLogs', label: 'Allow Export Logs', type: 'toggle', defaultValue: true },
    ],
  },
  {
    key: 'reportRules',
    title: 'Report & Export',
    subtitle: 'Control report exports, default date range and document formatting.',
    icon: FileText,
    fields: [
      {
        name: 'defaultReportDateRange', label: 'Default Report Date Range', type: 'select', defaultValue: 'thisMonth',
        options: [
          { value: 'today', label: 'Today' },
          { value: 'thisWeek', label: 'This Week' },
          { value: 'thisMonth', label: 'This Month' },
          { value: 'thisYear', label: 'This Year' },
        ],
      },
      {
        name: 'exportFormat', label: 'Default Export Format', type: 'select', defaultValue: 'xlsx',
        options: [
          { value: 'pdf', label: 'PDF' },
          { value: 'xlsx', label: 'Excel' },
          { value: 'csv', label: 'CSV' },
        ],
      },
      { name: 'enablePdfExport', label: 'Enable PDF Export', type: 'toggle', defaultValue: true },
      { name: 'enableExcelExport', label: 'Enable Excel Export', type: 'toggle', defaultValue: true },
      { name: 'showCompanyDetailsOnReports', label: 'Show Company Details On Reports', type: 'toggle', defaultValue: true },
      { name: 'showTaxDetailsOnReports', label: 'Show Tax Details On Reports', type: 'toggle', defaultValue: true },
      { name: 'reportDecimalPlaces', label: 'Report Decimal Places', type: 'number', defaultValue: 2 },
      { name: 'allowReportDownload', label: 'Allow Report Download', type: 'toggle', defaultValue: true },
    ],
  },
  {
    key: 'securityPolicy',
    title: 'System Security Policy',
    subtitle: 'Control global session, login lock and password policy rules.',
    icon: LockKeyhole,
    fields: [
      { name: 'sessionTimeoutMinutes', label: 'Session Timeout Minutes', type: 'number', defaultValue: 30 },
      { name: 'maxLoginAttempts', label: 'Max Login Attempts', type: 'number', defaultValue: 5 },
      { name: 'accountLockDuration', label: 'Account Lock Duration Minutes', type: 'number', defaultValue: 15 },
      { name: 'passwordExpiryDays', label: 'Password Expiry Days', type: 'number', defaultValue: 90 },
      { name: 'minimumPasswordLength', label: 'Minimum Password Length', type: 'number', defaultValue: 8 },
      { name: 'requireStrongPassword', label: 'Require Strong Password', type: 'toggle', defaultValue: true },
      { name: 'autoLogoutOnInactivity', label: 'Auto Logout On Inactivity', type: 'toggle', defaultValue: true },
      { name: 'forceReLoginAfterPasswordChange', label: 'Force Re-login After Password Change', type: 'toggle', defaultValue: true },
    ],
  },
  {
    key: 'integrationSettings',
    title: 'Integration Settings',
    subtitle: 'Control external integrations, API key, webhook and sync frequency.',
    icon: Bell,
    fields: [
      { name: 'posIntegrationEnabled', label: 'POS Integration Enabled', type: 'toggle', defaultValue: false },
      { name: 'paymentGatewayEnabled', label: 'Payment Gateway Enabled', type: 'toggle', defaultValue: false },
      { name: 'emailSmtpEnabled', label: 'Email SMTP Enabled', type: 'toggle', defaultValue: false },
      { name: 'smsGatewayEnabled', label: 'SMS Gateway Enabled', type: 'toggle', defaultValue: false },
      { name: 'whatsappNotificationEnabled', label: 'WhatsApp Notification Enabled', type: 'toggle', defaultValue: false },
      { name: 'externalSyncEnabled', label: 'External Sync Enabled', type: 'toggle', defaultValue: false },
      { name: 'syncFrequency', label: 'Sync Frequency', type: 'text', defaultValue: 'Daily' },
      { name: 'apiKey', label: 'API Key', type: 'text', defaultValue: '' },
      { name: 'webhookUrl', label: 'Webhook URL', type: 'text', defaultValue: '' },
    ],
  },
]

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function buildSectionDefaults(section) {
  return section.fields.reduce((acc, field) => {
    acc[field.name] = field.defaultValue
    return acc
  }, {})
}

function buildAllDefaults() {
  return sections.reduce((acc, section) => {
    acc[section.key] = buildSectionDefaults(section)
    return acc
  }, {})
}

/**
 * Merge API response data with local defaults so that any field the backend
 * does not yet return still has a sensible value.
 */
function mergeWithDefaults(apiData) {
  const defaults = buildAllDefaults()
  return sections.reduce((acc, section) => {
    acc[section.key] = {
      ...defaults[section.key],
      ...(apiData?.[section.key] ?? {}),
    }
    return acc
  }, {})
}

// ---------------------------------------------------------------------------
// FieldControl
// ---------------------------------------------------------------------------
function FieldControl({ field, value, onChange }) {
  if (field.type === 'toggle') {
    return (
      <label className="ims-setting-toggle" aria-label={field.label}>
        <input
          type="checkbox"
          checked={Boolean(value)}
          onChange={(e) => onChange(field.name, e.target.checked)}
          tabIndex={-1}
          onMouseDown={(e) => e.preventDefault()}
        />
        <span className="ims-setting-toggle__track">
          <span className="ims-setting-toggle__thumb" />
        </span>
      </label>
    )
  }

  if (field.type === 'select') {
    return (
      <select
        className="ims-setting-input"
        value={value ?? ''}
        onChange={(e) => onChange(field.name, e.target.value)}
      >
        {(field.options ?? []).map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
    )
  }

  return (
    <input
      className="ims-setting-input"
      type={field.type === 'number' ? 'number' : 'text'}
      value={value ?? ''}
      onChange={(e) => {
        const next = field.type === 'number'
          ? e.target.value === '' ? '' : Number(e.target.value)
          : e.target.value
        onChange(field.name, next)
      }}
    />
  )
}

// ---------------------------------------------------------------------------
// Main component
// ---------------------------------------------------------------------------
export default function SystemSettingsScreen() {
  const [activeSectionKey, setActiveSectionKey] = useState(sections[0].key)
  const [settings, setSettings] = useState(buildAllDefaults)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isResetting, setIsResetting] = useState(false)
  const [loadError, setLoadError] = useState('')

  // Track unsaved changes per section
  const [dirtyKeys, setDirtyKeys] = useState(new Set())

  const isMountedRef = useRef(true)
  useEffect(() => {
    isMountedRef.current = true
    return () => { isMountedRef.current = false }
  }, [])

  // ----- Derived -----
  const activeSection = useMemo(
    () => sections.find((s) => s.key === activeSectionKey) ?? sections[0],
    [activeSectionKey],
  )

  const ActiveSectionIcon = activeSection.icon
  const activeValues = settings[activeSection.key] ?? {}
  const isActiveDirty = dirtyKeys.has(activeSection.key)
  const activeEnabledRules = activeSection.fields.filter(
    (field) => field.type === 'toggle' && Boolean(activeValues[field.name]),
  ).length

  // ----- Load all settings on mount -----
  const loadSettings = useCallback(async () => {
    setIsLoading(true)
    setLoadError('')

    const { success, data, errors } = await fetchAllSystemSettings()

    if (!isMountedRef.current) return

    if (success) {
      setSettings(mergeWithDefaults(data))
      setDirtyKeys(new Set())

      // Warn about any partial failures
      const failedSections = Object.keys(errors ?? {})
      if (failedSections.length > 0) {
        showToast({
          type: 'warning',
          title: 'System Settings',
          message: `Some sections could not be loaded: ${failedSections.join(', ')}. Defaults used.`,
        })
      }
    } else {
      setLoadError('Unable to load settings from the server. Showing defaults.')
      showToast({
        type: 'error',
        title: 'System Settings',
        message: 'Failed to load settings. Showing local defaults.',
      })
    }

    setIsLoading(false)
  }, [])

  useEffect(() => {
    loadSettings()
  }, [loadSettings])

  // ----- Handlers -----
  const handleFieldChange = useCallback((fieldName, value) => {
    setSettings((prev) => ({
      ...prev,
      [activeSectionKey]: {
        ...prev[activeSectionKey],
        [fieldName]: value,
      },
    }))
    setDirtyKeys((prev) => new Set([...prev, activeSectionKey]))
  }, [activeSectionKey])

  const handleSave = useCallback(async () => {
    setIsSaving(true)

    const response = await saveSectionSettings(activeSection.key, activeValues)

    if (!isMountedRef.current) return
    setIsSaving(false)

    if (response.success) {
      window.dispatchEvent(
        new CustomEvent(SYSTEM_SETTINGS_UPDATED_EVENT, {
          detail: { sectionKey: activeSection.key, values: activeValues },
        }),
      )
      setDirtyKeys((prev) => {
        const next = new Set(prev)
        next.delete(activeSection.key)
        return next
      })
      showToast({
        type: 'success',
        title: 'System Settings',
        message: `${activeSection.title} saved successfully.`,
      })
    } else {
      showToast({
        type: 'error',
        title: 'System Settings',
        message: response.error || `Failed to save ${activeSection.title}.`,
      })
    }
  }, [activeSection, activeValues])

  const handleResetSection = useCallback(async () => {
    setIsResetting(true)

    // resetSectionSettings does POST /reset then re-fetches automatically
    // response.data is the fresh { fieldName: value } map after reset
    const response = await resetSectionSettings(activeSection.key)

    if (!isMountedRef.current) return
    setIsResetting(false)

    if (response.success) {
      const nextValues = {
        ...buildSectionDefaults(activeSection),
        ...(response.data ?? {}),
      }

      setSettings((prev) => ({
        ...prev,
        [activeSection.key]: nextValues,
      }))

      window.dispatchEvent(
        new CustomEvent(SYSTEM_SETTINGS_UPDATED_EVENT, {
          detail: { sectionKey: activeSection.key, values: nextValues },
        }),
      )

      setDirtyKeys((prev) => {
        const next = new Set(prev)
        next.delete(activeSection.key)
        return next
      })

      showToast({
        type: 'success',
        title: 'System Settings',
        message: `${activeSection.title} reset to defaults.`,
      })
    } else {
      showToast({
        type: 'error',
        title: 'System Settings',
        message: response.error || `Failed to reset ${activeSection.title}.`,
      })
    }
  }, [activeSection])

  const handleRefresh = useCallback(async () => {
    await loadSettings()
    showToast({
      type: 'success',
      title: 'System Settings',
      message: 'Settings refreshed from server.',
    })
  }, [loadSettings])

  const handleSectionChange = useCallback((key) => {
    setActiveSectionKey(key)
  }, [])

  // ----- Render -----
  return (
    <section className="ims-system-settings-page">
      <header className="ims-system-settings-header">
        <div className="ims-system-settings-header__content">
          <div className="ims-system-settings-header__title-row">
            <span className="ims-system-settings-header__icon">
              <Settings2 size={20} />
            </span>
            <h1>System Settings</h1>
          </div>
        </div>
      </header>

      {/* Global load error banner */}
      {loadError && !isLoading && (
        <div className="ims-system-settings-error-banner page-error-banner" role="alert">
          <AlertCircle size={16} />
          <span>{loadError}</span>
        </div>
      )}

      <nav className="ims-system-settings-tabs" aria-label="System settings sections">
          {sections.map((section) => {
            const Icon = section.icon
            const isActive = section.key === activeSection.key
            const isDirty = dirtyKeys.has(section.key)

            return (
              <button
                key={section.key}
                type="button"
                className={`ims-system-settings-menu__item ${isActive ? 'is-active' : ''} ${isDirty ? 'is-dirty' : ''}`}
                onClick={() => handleSectionChange(section.key)}
                title={section.title}
                aria-label={section.title}
              >
                <span className="ims-system-settings-menu__icon">
                  <Icon size={16} />
                </span>
                <span className="ims-system-settings-menu__item-title">
                  {section.title}
                  {isDirty && <span className="ims-settings-dirty-dot" title="Unsaved changes" />}
                </span>
              </button>
            )
          })}
      </nav>

      <div className="ims-system-settings-shell">
        {/* Panel */}
        <main className="ims-system-settings-panel">
          <div className="ims-system-settings-panel__header">
            <div className="ims-system-settings-panel__identity">
              <div className="ims-system-settings-panel__icon">
                <ActiveSectionIcon size={22} />
              </div>
              <div className="ims-system-settings-panel__copy">
                <div className={`ims-system-settings-panel__eyebrow ${isActiveDirty ? 'is-dirty' : 'is-synced'}`}>
                  <CheckCircle2 size={14} />
                  {isActiveDirty ? 'Unsaved Configuration' : 'Synced Configuration'}
                </div>
                <h2>{activeSection.title}</h2>
              </div>
            </div>

            <div className="ims-system-settings-panel__actions">
              <button
                type="button"
                className="ims-settings-button"
                onClick={handleRefresh}
                disabled={isLoading || isSaving || isResetting}
                title="Reload all settings from server"
              >
                <RefreshCw size={15} className={isLoading ? 'ims-spin' : ''} />
                Refresh
              </button>

              <button
                type="button"
                className="ims-settings-button"
                onClick={handleResetSection}
                disabled={isLoading || isSaving || isResetting}
                title="Reset this section to server defaults"
              >
                {isResetting
                  ? <Loader2 size={15} className="ims-spin" />
                  : <RotateCcw size={15} />
                }
                Reset
              </button>

              <button
                type="button"
                className="ims-settings-button ims-settings-button--primary"
                onClick={handleSave}
                disabled={isLoading || isSaving || isResetting || !isActiveDirty}
                title="Save this section's settings"
              >
                {isSaving
                  ? <Loader2 size={15} className="ims-spin" />
                  : <Save size={15} />
                }
                Save Changes
              </button>
            </div>
          </div>

          <div className="ims-system-settings-panel__meta" aria-label="Active section summary">
            <span><strong>{activeSection.fields.length}</strong> Rules</span>
            <span><strong>{activeEnabledRules}</strong> On</span>
            <span className={isActiveDirty ? 'is-dirty' : 'is-synced'}>
              {isActiveDirty ? 'Unsaved' : 'Saved'}
            </span>
          </div>

          {/* Loading overlay */}
          {isLoading ? (
            <div className="ims-system-settings-loading">
              <Loader2 size={32} className="ims-spin" />
              <span>Loading settings...</span>
            </div>
          ) : (
            <div className="ims-system-settings-grid">
              {activeSection.fields.map((field) => {
                const isToggle = field.type === 'toggle'
                const isEnabled = isToggle && Boolean(activeValues[field.name])

                return (
                  <div
                    key={field.name}
                    className={[
                      'ims-setting-card',
                      isToggle ? 'ims-setting-card--toggle' : 'ims-setting-card--value',
                      isToggle ? (isEnabled ? 'is-enabled' : 'is-disabled') : '',
                    ].filter(Boolean).join(' ')}
                  >
                    <div className="ims-setting-card__copy">
                      <div className="ims-setting-card__title-row">
                        <label>{field.label}</label>
                      </div>
                    </div>

                    <FieldControl
                      field={field}
                      value={activeValues[field.name]}
                      onChange={handleFieldChange}
                    />
                  </div>
                )
              })}
            </div>
          )}


        </main>
      </div>
    </section>
  )
}

