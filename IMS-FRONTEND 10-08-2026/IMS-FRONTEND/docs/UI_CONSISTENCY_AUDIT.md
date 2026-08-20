# IMS Product Consistency Audit

Audit date: 08-Jun-2026

## Executive Summary

The IMS frontend has been audited as one ERP product, not as isolated pages. The major consistency problem was that modules had similar patterns implemented through different local wrappers, local tables, local cards, local badges, and custom table markup. That makes the app feel uneven even when every individual screen is functional.

This pass created and applied a shared ERP component layer and removed direct module-level dependency on the low-level table engine. Core module tables now consume the same `DataTable`, `StatusBadge`, `ActionButtons`, `FilterBar`, `Pagination`, `SearchBar`, `TableToolbar`, `Drawer`, and `StatisticsCard` standards from `src/components/erp`.

Business logic, APIs, routes, permissions, payment allocation logic, creation flows, edit flows, and export APIs were not changed.

## Audited Scope

Audited routed modules and screens:

| Area | Screens / modules | Current consistency state |
| --- | --- | --- |
| Dashboard | Dashboard, low-stock watchlist | Low-stock table uses shared `DataTable`; dashboard metric widgets still need a later full card-system pass. |
| Catalog | Categories, Brands, Units, Subcategories, Attributes, Product Variants | Categories use shared table/cards/filter bar. Subcategories, Attributes, and Variants are covered by `ResourceCenter`. Brands and Units use `ReferenceManager`, which still has a custom reference table. |
| Products | Products table, import/export actions, create/edit modals | Table, action group, filter bar, and status badges are shared. Print/export HTML tables remain intentionally separate document-generation markup. |
| Stock | Stock operations, current stock, recent movements | Tables use shared `DataTable`; Stock still uses `ModalComponent` for stock movement form. |
| Goods Receipts | ResourceCenter-backed goods receipts | Uses shared ResourceCenter table, actions, filter bar, status badges, forms, and statistics cards. |
| Purchases | Purchase orders, create/edit/delete workflow | Main table/actions/status/stat cards are shared. FormModal workflow is consistent. |
| Sales | Invoice list, invoice actions, create/delete workflow | Live invoice table was migrated to shared `DataTable`; status badges and actions are shared. Page-level search toolbar remains custom but behavior is preserved. |
| Inventory Audit | Audit table and warehouse filter | Table uses shared `DataTable`; warehouse filter is wrapped in shared `FilterBar`. |
| Barcode QR | Generated code table and barcode form | Table uses shared `DataTable`; preview cell remains specialized. |
| Suppliers | Supplier master, detail tabs, payment/purchase history | Main table/actions/status/stat cards and detail tab tables are shared. Supplier detail page still has domain-specific cards. |
| Customers | Customer master, details panel, status/delete dialogs | Main table/actions/filter/data table are shared. Customer still has a local interactive status badge pending final migration to shared interactive badge policy. |
| Customer Payments | Payment list, receipt drawer, receipt PDF, forms | Table/actions/filter/stat cards are shared. Receipt drawer is intentionally custom and mature, but should later mount through shared `Drawer` for shell consistency. |
| Supplier Payments | Payment list, forms | Covered by shared PaymentModule table/actions/filter/stat cards. |
| Warehouses | Warehouse table, warehouse details, racks, bins, bin stock, transfers | Main/detail tables, status badges, filter bar, and stat cards are shared. Warehouse details and transfer flows still use `ModalComponent` in places. |
| Reports | Report tabs, report tables, report cards, exports | Tables and status badges are shared; report summary cards now use shared statistics card. Export coverage is still uneven by report type. |
| Notifications | ResourceCenter route plus legacy notification module | ResourceCenter route is shared. Legacy notifications table/cards were also migrated to shared components where safe. |
| Accounting | ResourceCenter route plus legacy accounting module | ResourceCenter route is shared. Legacy accounting register/history tables and metric cards were migrated to shared components where safe. |
| Returns | Sales returns table/cards/forms | Table/actions/status/stat cards are shared. |
| Admin | Users, Roles, Audit Logs, System Settings | Current routes are ResourceCenter-backed. Legacy Users/Roles tables were migrated to shared `DataTable` for consistency if reused. |

## Shared Design System Created

Location: `src/components/erp`

- `DataTable`
- `TableToolbar`
- `SearchBar`
- `FilterBar`
- `FilterDropdown`
- `Pagination`
- `StatusBadge`
- `ActionButtons`
- `Drawer`
- `StatisticsCard`

These components sit above the older low-level primitives and define the ERP-facing usage contract. Modules should import from `src/components/erp`, not from low-level table or badge files.

## Application Standards

### Page Header

- Use `PageHeader` or the module's existing header wrapper only when it matches the same structure.
- Required order: icon, page title, short operational description, page actions.
- Page actions should be right-aligned and use icon plus text for primary commands.

### Statistics Cards

- Use `StatisticsCard` for all summary metrics.
- Standard height: compact ERP card, not oversized dashboard tiles.
- Use neutral, success, warning, and danger tones only.
- Avoid module-local `SummaryCard` markup unless it is a thin wrapper around `StatisticsCard`.

### Search Toolbar

Target order for table toolbars:

1. Search
2. Filters
3. Columns
4. Export
5. Refresh
6. Add button

Current state: search, filters, export, refresh, and add actions exist in several modules. A universal Columns control is still a product follow-up.

### Filters

- Custom filters should be wrapped in `FilterBar`.
- Select-only filters should move to `FilterDropdown` where suitable.
- Filter placement should be inside the table toolbar unless it affects the whole page.

### Tables

- Modules must use `DataTable`.
- Modules must not import `TableComponent` directly.
- Standard behavior: shared search, sort, loading, empty state, pagination, mobile card rendering, row click keyboard support.
- Standard density: 36px row target, compact headers, 16px icons, 30px action buttons, 16px to 18px checkboxes.

### Pagination

Use the shared pagination pattern everywhere:

- Rows per page selector
- Showing x-y of z
- Previous
- Page numbers
- Next

### Status Badges

Shared status color policy:

- Paid, Success, Active, Completed, Received: green
- Partial, Warning, Ordered: amber
- Pending, Draft, Unpaid: gray
- Failed, Cancelled, Blocked, Overdue, Inactive: red
- Info and neutral states: blue/gray based on context

### Status Editing

Current inconsistency: Customers and Warehouses support inline status changes, while many comparable modules are display-only.

Standard:

- Display-only status uses shared `StatusBadge`.
- Editable status should use the shared interactive badge API.
- Editable status must include permission checks, disabled saving state, keyboard activation, and menu close-on-outside-click behavior.

### Row Actions

- Use `ActionButtons` for all row-level actions.
- Icon size should be 16px.
- Icon-only buttons must include `aria-label`, `title`, or tooltip text.
- Destructive actions require confirmation where data is removed or archived.

### Export System

Current inconsistency: export coverage and formats vary by module.

Standard:

- Table-level export belongs in the table toolbar.
- Customer-facing financial documents must be PDF, not CSV/XLS.
- Spreadsheet exports should remain top-level data exports only.
- File names should use stable document identifiers.

### Drawer System

- Use shared `Drawer` for side panels.
- Sticky header and sticky footer are required for action-heavy drawers.
- The drawer body should have a single scroll region.
- Width target: 35 percent to 40 percent viewport with max width around 520px for finance/detail receipts.

### Modal System

- Use `FormModal` for create/edit/delete/status confirmation workflows.
- Existing `ModalComponent` usage should be migrated when touched.
- Avoid mixing `ModalComponent` and `FormModal` inside the same module unless the modal is genuinely not a form workflow.

### Empty And Loading States

- Table empty and loading states should come from `DataTable`.
- Non-table empty states should use `StateBlock` where practical.
- Loading states should include role/status semantics and avoid blank panels.

## Inconsistencies Found

| Priority | Area | Finding | Modules affected | Recommended fix |
| --- | --- | --- | --- | --- |
| Critical | Design dependency boundary | Modules imported low-level `TableComponent` directly, which allowed inconsistent defaults and bypassed ERP-level standards. | Legacy Warehouses, Users, Roles, Returns, Barcode, Dashboard, Notifications, Accounting, InventoryAudit | Completed: module imports now use `DataTable`. |
| High | Table implementation | Sales and Accounting had hand-built live data tables with different pagination, empty, row, and mobile behavior. | Sales, Accounting | Completed: live invoice list, accounting payment history, and accounting invoice register now use `DataTable`. |
| High | Reference tables | Brands and Units still use `ReferenceManager` custom HTML table instead of `DataTable`. | Brands, Units | Refactor `ReferenceManager` to `DataTable` while preserving its reference-card visual identity. |
| High | Modal shell fragmentation | Some modules mix `ModalComponent` and `FormModal`. | Stock, Warehouses | Migrate create/edit/transfer/detail forms to `FormModal` or shared `Drawer` based on workflow type. |
| High | Drawer shell fragmentation | Payment receipt drawer is custom and does not yet mount through the shared `Drawer` shell. | Customer Payments, Supplier Payments | Keep receipt body and calculations, but move outer shell to `Drawer` in a safe follow-up. |
| High | Status edit inconsistency | Inline status editing exists in selected master-data modules but not uniformly across comparable modules. | Customers, Warehouses, Suppliers, Products, Categories | Define which modules are editable, then implement through shared interactive `StatusBadge`. |
| High | Export inconsistency | Export availability, format, naming, and placement differ by module. | Products, Payments, Reports, ResourceCenter-backed modules | Create shared `ExportMenu` policy and add per-module capability flags. |
| High | Column visibility | Some modules expose richer table controls than others, but column visibility is not global. | Customers, Payments, Products, Reports, ResourceCenter modules | Add a shared Columns control to `DataTable`. |
| Medium | Filter controls | Filters are visually closer now, but many selects remain module-specific. | Sales, Reports, Warehouses, Stock, Inventory Audit | Move select filters to `FilterDropdown` as modules are touched. |
| Medium | Search behavior | Some pages use external search bars while others use `DataTable` search. | Sales, Roles, a few detail views | Keep external search only when it searches multiple page areas; otherwise use table search. |
| Medium | Empty states | Data tables are standardized, but some detail panels still have bespoke empty states. | Warehouses, Suppliers, Payments, Accounting detail sections | Use `StateBlock` for non-table empty states. |
| Medium | Loading states | Table loading is standardized, but page-level panels still vary. | Dashboard, Warehouses, Reports, Suppliers | Create shared page/panel loading skeletons. |
| Medium | Bulk actions | Bulk selection and bulk actions are not consistently available across similar master-data tables. | Customers, Products, Suppliers, Payments, Categories | Add optional bulk selection to `DataTable` with capability flags. |
| Medium | Confirmation dialogs | Delete/archive/status confirmations use different copy and layouts. | Customers, Suppliers, Products, Warehouses, Returns, Payments | Standardize confirmation dialog copy, severity color, and button order. |
| Low | CSS naming drift | Module class names remain necessary, but some older classes duplicate ERP component styles. | Most modules | Gradually remove dead module-level table/card CSS after visual QA. |
| Low | Typography micro-drift | Some module-local helper text, card labels, and section headings differ slightly. | Reports, Dashboard, ReferenceManager, Warehouses | Move typography tokens into shared CSS utilities. |

## UX Problems Found

- Too many historical one-off table surfaces made pagination, loading, and row density feel different between modules.
- Some modules expose search outside the table while others expose it inside the table toolbar.
- Some action clusters used text buttons while others used compact icon buttons.
- Some module detail workflows use modals where a drawer would better preserve context.
- Reference tables for Brands and Units feel visually different from standard table modules.
- Some detail sections still rely on local empty states rather than shared empty-state language.
- Export actions do not yet follow one cross-module placement and format policy.
- Bulk actions and column visibility are not applied consistently across comparable tables.

## Design System Violations Found

- Local `SummaryCard`, `MetricCard`, and notification metric card implementations existed across modules.
- Customer-specific status badge was imported outside the customer module in earlier code.
- Several modules previously imported `TableComponent` directly instead of shared ERP `DataTable`.
- Action button wrappers had inconsistent gaps, button sizes, and row-click ignore behavior.
- Filter controls were often plain div wrappers instead of a shared toolbar/filter wrapper.
- Some live tables were custom HTML tables while similar modules used the shared table.
- `ModalComponent` and `FormModal` are both used in operational modules.

## Functional Inconsistencies Found

- Inline status editing is available in some master data modules but display-only in similar modules.
- Export formats are not consistent by document type and table type.
- Report exports are available for selected reports only.
- Column selection is not a universal table capability.
- Bulk actions are not a universal table capability.
- Search can be global to the table in one module and external to the page in another.
- Some modules support refresh at table level while others refresh only by route/load.

## Bug Risks Found

- Custom tables increase risk of missing accessibility, keyboard row activation, mobile rendering, and pagination behavior.
- Module-specific status menus increase risk of inconsistent saving/disabled states.
- Custom export/print windows need consistent failure feedback and file naming checks.
- Detail modals with large tables can still create overflow or nested-scroll issues if they do not use shared drawer/table regions.
- Some non-table empty states are plain text and may be missed by assistive technology.
- Mixed modal systems increase the chance of focus-trap, close behavior, and z-index drift.

## Refactor Completed In This Pass

- Created the shared ERP component layer in `src/components/erp`.
- Standardized `TableComponent` internals to use shared `TableToolbar`, `SearchBar`, and `Pagination`.
- Standardized table pagination to rows selector, showing count, previous, page numbers, and next.
- Standardized table density, hover states, selected rows, checkbox sizing, action button sizing, and page number controls.
- Added interactive capabilities to the base `StatusBadge` so editable status flows can migrate to a shared API.
- Standardized status color mapping globally.
- Replaced remaining module-level direct `TableComponent` imports with `DataTable`.
- Migrated Sales invoice list from a custom HTML table to `DataTable`.
- Migrated Accounting payment history and invoice register from custom HTML tables to `DataTable`.
- Migrated Warehouses main/detail tables, status badges, filter bar, and stat cards to shared ERP components.
- Migrated Users, Roles, Returns, Barcode, Dashboard low-stock, Notifications, Accounting legacy table, Inventory Audit, and Reports tables to shared ERP components where applicable.
- Migrated Reports, Returns, Notifications, Accounting, Warehouses, Customers, Suppliers, Categories, Payments, Purchases, Sales, Stock, and ResourceCenter metric wrappers to `StatisticsCard` or thin wrappers around it.
- Wrapped primary row action clusters in `ActionButtons` across master-data and transaction modules.
- Wrapped primary custom filter toolbars in `FilterBar`.

## Remaining Product Roadmap

### Critical Before Production Hardening

1. Define a single status-editing policy per module.
2. Define one export policy: table export, document PDF, naming, permissions, and feedback.
3. Add global column visibility and optional bulk selection to `DataTable`.
4. Migrate all destructive confirmations to one confirmation dialog contract.

### High Priority

1. Refactor `ReferenceManager` to use `DataTable` for Brands and Units.
2. Move Payment receipt drawer shell onto shared `Drawer`.
3. Replace `ModalComponent` usage in Stock and Warehouses where the workflow is a form or detail drawer.
4. Create shared `ExportMenu` and `ColumnVisibilityMenu`.
5. Move customer interactive status badge to shared `StatusBadge` action API.

### Medium Priority

1. Convert remaining select filters to `FilterDropdown`.
2. Standardize Dashboard metric cards and report widgets.
3. Standardize non-table empty states with `StateBlock`.
4. Add shared page/panel loading skeletons.
5. Audit all modals for focus behavior, Escape handling, and scroll containment.

### Low Priority

1. Remove dead module-level CSS that duplicates ERP component styles.
2. Normalize helper text copy style.
3. Tighten icon-only button tooltip coverage in secondary views.
4. Document component usage examples in a design-system README.

## Acceptance Criteria Status

- Same table behavior across primary modules: complete for routed primary and legacy reusable tables.
- Same pagination everywhere shared tables are used: complete.
- Same status color interaction baseline: complete for display; editable policy still needs final product decision.
- Same action button wrapper: mostly complete for primary row actions; secondary/detail actions remain a follow-up.
- Same filter placement: improved through `FilterBar`; full `FilterDropdown` migration remains.
- Same drawer layout: shared `Drawer` exists; Payment drawer shell migration remains.
- Same modal layout: `FormModal` is canonical; `ModalComponent` migration remains.

## Final Recommendation

IMS is now much closer to a unified ERP frontend because the core tables, pagination, status badges, statistics cards, filter wrappers, and row action groups have a shared dependency boundary. The next highest-value work is not more one-screen redesign. It is finishing the policy-level pieces: global column visibility, bulk selection, unified exports, unified status editing, shared drawer shell usage, and ReferenceManager migration.
