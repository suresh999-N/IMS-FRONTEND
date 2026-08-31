# IMS Application Audit Report

Audit date: 08-Jun-2026

## Goal

Every IMS module must behave like one product. If a feature exists in one comparable module, the same feature, style, placement, and interaction rule must exist in all comparable modules.

This report focuses on product consistency, not one-page redesign.

## Global Standard Required

| Area | Required IMS Standard |
| --- | --- |
| Table toolbar | Search, Filters, Columns, Export, Refresh, Add button, in that order where applicable. |
| Pagination | Rows selector, Showing x-y of z, First, Previous, page numbers, Next, Last. |
| Columns | All major tables must support a consistent Columns menu when there are optional columns. |
| Row selection | Only modules with real bulk actions should show selection checkboxes. Checkbox size must be compact and identical. |
| Status editing | If status is editable in table view for one master module, comparable master modules must support the same interaction or all should be display-only. |
| Actions | Same icon size, same button size, same order: View, Edit, Delete, Download/Print where relevant. |
| Export | Same export placement and naming rules. Data export is separate from document PDF download. |
| Drawers | Detail/receipt drawers must share one shell: sticky header, single scroll body, sticky footer, same width rules. |
| Modals | Create/edit/delete/status confirmation must use the same modal shell and button order. |
| Badges | Same color system for Active, Inactive, Pending, Paid, Partial, Failed, Cancelled, Overdue. |
| Empty/loading states | Same language, same visual pattern, same table skeleton/loading behavior. |

## Module Audit Matrix

| Module | Issues Found | Severity | Required Fix |
| --- | --- | --- | --- |
| Customers | Missing Export. Has Columns menu but implementation must be shared. Inline status editing enabled. Pagination must match all modules. Actions use compact icon buttons. | Critical | Use shared table toolbar. Add Export if comparable master modules have export. Keep or remove inline status editing based on global status policy. |
| Suppliers | Different toolbar. Different actions. Different table density/layout. Status is display-only while Customers has inline editing. | Critical | Match Customers-style toolbar/actions/table density. Apply same status policy. Add Columns/Export if master modules require it. |
| Products | Different export system. No row selection. Different action pattern. Product export is local HTML/XLS/PDF style. | High | Move export into shared Export action pattern. Add row selection only if bulk actions exist. Standardize actions to View/Edit/Delete/Export where applicable. |
| Categories | Different hierarchy table behavior. No consistent Columns/Export policy. Status display differs from Customers. | High | Keep hierarchy behavior, but use same toolbar, pagination, status badge, action button, and export policy. |
| Brands | Uses custom ReferenceManager table. Does not fully match DataTable style. No Columns/Export consistency. | High | Refactor ReferenceManager to shared DataTable while preserving reference workflow. |
| Units | Uses custom ReferenceManager table. Does not fully match DataTable style. No Columns/Export consistency. | High | Refactor ReferenceManager to shared DataTable while preserving reference workflow. |
| Attributes | ResourceCenter-backed but must be checked for same toolbar/action/status behavior as Brands/Units. | Medium | Apply same ResourceCenter table standard and ensure Columns/Export/Refresh/Add ordering. |
| Product Variants | ResourceCenter-backed. Needs same toolbar/action/status behavior as Attributes and catalog modules. | Medium | Apply same ResourceCenter table standard. |
| Stock | Some screens use ModalComponent. Current Stock and movement tables need same toolbar/filter/action pattern. | High | Use one modal/drawer policy. Standardize filters and actions. |
| Goods Receipts | ResourceCenter-backed. Needs validation against Purchases/Stock table behavior. | Medium | Ensure same table toolbar, status badge, pagination, actions, and export policy. |
| Purchases | Table mostly standardized but status/action/export behavior must match Sales and Goods Receipts. | Medium | Apply common transaction module toolbar and export/print policy. |
| Sales | Invoice table now uses shared table, but page-level toolbar/search differs from other tables. Status is display-only. | High | Move page search/actions into shared toolbar pattern or clearly define as page-level exception. |
| Inventory Audit | Filter placement differs. No export/bulk policy. | Medium | Use shared filter dropdown pattern and add Export only if audit logs require it. |
| Barcode QR | Specialized preview column. No consistent export/columns policy. | Medium | Keep preview specialization, standardize toolbar, Columns, Export, pagination. |
| Customers Payments | Different checkbox implementation. Different drawer. Different filters. Has Columns/Export/Refresh/New Payment. Receipt drawer is custom. | Critical | Keep controlled bulk-selection only here if bulk actions exist. Standardize checkbox visual style. Move receipt drawer shell to shared Drawer. |
| Supplier Payments | Must match Customer Payments exactly except labels/data source. | Critical | Same columns, filters, selection, export, drawer, PDF, print, pagination, action behavior. |
| Warehouses | Inline status editing behavior differs from Customers. Details use modal-style panels. Multiple nested tables. | High | Apply same editable-status interaction if status editing remains global. Use shared drawer/modal decision. |
| Reports | Export only applies to selected reports. Toolbar and tabs differ from transaction tables. | Medium | Define report export policy per report type. Keep tabs but standardize action placement. |
| Notifications | ResourceCenter route and legacy module patterns differ. | Medium | Keep one implementation path. Standardize alert tables and actions. |
| Accounting | ResourceCenter route and legacy accounting module differ. Register/history tables must use same table behavior. | High | Choose ResourceCenter or legacy Accounting as source of truth. Standardize payment/status actions. |
| Returns | Table is shared but actions/export/status policy differs from Sales/Purchases. | Medium | Match transaction module standards. |
| Users | ResourceCenter-backed route, legacy table exists. Needs admin table standard. | Medium | Use ResourceCenter standard only or retire legacy table. |
| Roles | ResourceCenter-backed route, legacy table exists. Different permission matrix behavior is acceptable but table shell should match. | Medium | Standardize table and modal shell. |
| Audit Logs | ResourceCenter-backed. Requires strict pagination/export/search consistency. | High | Add consistent export/search/filter pattern for admin logs. |
| System Settings | ResourceCenter-backed. Screenshot shows table with no Columns/Export and different controls. | Critical | Apply shared toolbar: Search, Columns where optional, Refresh/Edit/Add as applicable. No fake row selection unless bulk actions exist. |

## Cross-Module Problems

### Critical

- Pagination was not identical across all modules.
- Columns menu existed in some modules but not all comparable tables.
- Checkbox selection appeared in some modules and not others, with inconsistent visual size.
- Status double-click editing existed in Customers but not comparable status tables.
- Payment drawer and other detail views use different shells.

### High

- Export behavior is inconsistent across Products, Payments, Reports, and ResourceCenter modules.
- Some modules use shared DataTable, others still have specialized table behavior through ReferenceManager or document-generation tables.
- Toolbar action order is inconsistent.
- Search placement differs between modules.
- Modal/drawer choice is inconsistent.

### Medium

- Empty states and loading states still vary in secondary/detail screens.
- Action button labels/icons are inconsistent in some modules.
- Some modules have Refresh while others do not.
- Some tables allow column hiding while others do not.

## Required Implementation Policy

### Table Policy

All business tables must use shared `DataTable`.

Required props/policies:

- `showColumnControls`: true only when the table has optional columns.
- `enableRowSelection`: true only when the module has real bulk actions.
- `columnStorageKey`: required for important user-facing tables.
- `lockedColumnKeys`: required for primary identity and action columns.
- `filterContent`: used for filters before Columns.
- `toolbarContent`: used for Export, Refresh, Add, and custom actions after Columns.

### Status Policy

Choose one:

1. Editable status in all applicable master tables.
2. Display-only status in all tables, with edit handled through forms.

Do not mix without a product reason.

Applicable editable modules:

- Customers
- Suppliers
- Products
- Categories
- Warehouses
- Users

### Export Policy

- Master data export: CSV/XLS/PDF from table toolbar.
- Financial receipt/invoice documents: PDF only.
- Reports: report-specific export buttons, same placement and naming.
- Audit logs: CSV/XLS export only if permission allows.

### Drawer/Modal Policy

- View/detail/receipt: Drawer.
- Create/edit forms: FormModal.
- Delete/status confirmation: Confirmation modal.
- Large operational workspace: Full page or drawer, not nested modal tables.

## Immediate Fix Order

1. Freeze final table toolbar standard.
2. Fix pagination globally: First, Previous, page numbers, Next, Last.
3. Make row selection opt-in only.
4. Standardize Columns menu through shared DataTable.
5. Create a shared ExportMenu.
6. Decide status-editing policy and apply consistently.
7. Refactor ReferenceManager tables.
8. Standardize Payment drawer shell with shared Drawer.
9. Standardize modal/drawer usage in Stock and Warehouses.
10. Run visual QA module by module before demo.

## Fixes Applied In Current Pass

| Area | Fix Applied | Files Modified | Reason |
| --- | --- | --- | --- |
| Export standard | Added shared `ExportMenu` component. | `src/components/erp/ExportMenu.jsx`, `src/components/erp/index.js`, `src/components/erp/ERPComponents.css` | Export access must be one reusable product pattern. |
| Confirmation standard | Added shared `ConfirmationDialog` component. | `src/components/erp/ConfirmationDialog.jsx`, `src/components/erp/index.js` | Delete/archive/status confirmations need one shell and button order. |
| Loading standard | Added shared `LoadingState` wrapper. | `src/components/erp/LoadingState.jsx`, `src/components/erp/index.js` | Page/panel loading states should not be module-specific. |
| Products export | Replaced separate Excel/PDF buttons with shared `ExportMenu`. | `src/modules/Products/components/ProductsTable.jsx` | Products must follow shared export entry point without changing export logic. |
| Reports export | Replaced separate report export buttons with shared `ExportMenu`. | `src/modules/Reports/Reports.jsx` | Report exports must use the same export component while keeping report-specific export actions. |
| Row selection policy | Ensured `DataTable` selection is opt-in only. | `src/components/erp/DataTable.jsx` | Tables without bulk actions must not show checkbox columns. |
| ResourceCenter toolbar | Moved Refresh/Add actions from page header into the shared table toolbar. | `src/modules/ResourceCenter/ResourceCenter.jsx` | ResourceCenter-backed modules must follow Search, Filters, Columns, Export, Refresh, Add ordering. |

## Pending Fixes Not Yet Applied

| Priority | Pending Work | Reason |
| --- | --- | --- |
| Critical | Decide global status policy and apply to Customers, Suppliers, Products, Warehouses, Categories, Users. | Mixed editable/display-only status behavior is the clearest cross-module UX inconsistency. |
| Critical | Add export capability flags to eligible ResourceCenter modules. | Toolbar placement is standardized; export availability still needs per-resource policy. |
| High | Refactor ReferenceManager to `DataTable`. | Brands and Units still have a custom table. |
| High | Move Payment receipt drawer shell to shared `Drawer`. | Payment drawer body is strong, but shell behavior must match other View drawers. |
| High | Replace remaining `ModalComponent` usage in Stock and Warehouses. | Modal policy must be consistent. |
| High | Add `ExportMenu` to all eligible master/transaction tables. | Products and Reports are migrated; other modules need capability-specific export actions. |

## Demo Acceptance Checklist

Before the next demo, verify:

- Customers, Suppliers, Products have same toolbar pattern.
- Customer Payments and Supplier Payments are identical except party type.
- System Settings and other ResourceCenter tables have matching pagination and Columns behavior.
- No table shows selection checkboxes unless bulk actions exist.
- Any editable status cell behaves the same on double click and keyboard.
- All pagination footers show the same controls.
- Export buttons are consistently placed and named.
- Drawers do not overlap content and share header/footer behavior.
