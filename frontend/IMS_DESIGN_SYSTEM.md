# IMS Design System

Audit date: 08-Jun-2026

IMS uses a premium enterprise SaaS design foundation inspired by Stripe Dashboard, Linear, Ramp, Brex, Vercel, Notion, and Atlassian Cloud: neutral surfaces, clear hierarchy, subtle depth, minimal borders, accessible controls, and one reusable ERP component layer.

## Token Source

Design tokens live in:

`src/styles/design-tokens.css`

Import path:

`src/index.css`

All new UI must use tokens instead of hard-coded colors, spacing, shadows, radius, or component sizes.

## Token Groups

| Group | Token examples | Purpose |
| --- | --- | --- |
| Color | `--color-neutral-*`, `--color-brand-*`, `--color-success-*`, `--color-warning-*`, `--color-danger-*`, `--color-info-*` | Product palette and semantic states. |
| App aliases | `--background`, `--surface`, `--text`, `--muted`, `--primary`, `--border` | Backward-compatible aliases for existing modules. |
| Typography | `--font-sans`, `--font-size-xs`, `--font-size-sm`, `--font-size-md`, `--font-weight-semibold` | Consistent labels, tables, forms, and headings. |
| Spacing | `--space-1` to `--space-10` | Shared vertical rhythm and component gaps. |
| Radius | `--radius-sm`, `--radius-md`, `--radius-lg`, `--radius-card`, `--radius-modal` | Consistent shape language. |
| Shadows | `--shadow-xs`, `--shadow-sm`, `--shadow-popover`, `--erp-shadow-soft` | Subtle SaaS depth. |
| Layout | `--ims-sidebar-width`, `--ims-header-height`, `--erp-table-viewport-min-height` | Shell, table, drawer, and responsive sizing. |

## Core Theme

| Decision | Token / value |
| --- | --- |
| Primary | `#0F766E` |
| Primary hover | `#115E59` |
| Primary light | `#CCFBF1` |
| Background | `#F8FAFC` |
| Surface | `#FFFFFF` |
| Text primary | `#0F172A` |
| Text secondary | `#475569` |
| Text muted | `#94A3B8` |
| Success | `#22C55E` |
| Warning | `#F59E0B` |
| Danger | `#EF4444` |
| Info | `#3B82F6` |
| Font | Inter |
| Page title | `36px` |
| Section title | `24px` |
| Card value | `32px` |
| Body | `14px` |
| Button/input radius | `12px` |
| Card radius | `16px` |
| Modal radius | `20px` |
| Spacing scale | `4, 8, 12, 16, 24, 32, 48, 64` |

## Component Architecture

Shared ERP-facing components live in:

`src/components/erp`

Exports:

- `PageHeader`
- `KpiCard`
- `StatisticsCard`
- `DataTable`
- `TableToolbar`
- `SearchBar`
- `FilterToolbar`
- `FilterBar`
- `FilterDropdown`
- `Pagination`
- `StatusBadge`
- `Button`
- `Input`
- `Select`
- `ActionButtons`
- `ActionMenu`
- `ExportMenu`
- `FormControl`
- `Modal`
- `Drawer`
- `EmptyState`
- `SkeletonLoader`
- `ConfirmationDialog`
- `LoadingState`

Modules should import from `src/components/erp` when possible. Low-level components such as `TableComponent` should remain implementation details.

## Standard Page Layout

Every business module should follow this structure:

1. `PageHeader`
2. KPI section using `KpiCard` or `StatisticsCard`
3. `FilterToolbar` or the `DataTable` toolbar
4. Content section with `DataTable`, drawer, or workflow panel

Avoid creating local card, table, status badge, search bar, pagination, or action button systems.

## Table Standard

All business tables should use `DataTable`.

Toolbar order:

1. Search
2. Filters
3. Columns
4. Export
5. Refresh
6. Add button

Table rules:

- Row height: premium enterprise density, with room for identity, metadata, and status.
- Header style: uppercase, muted, semi-bold.
- Checkbox size: 16px.
- Action buttons: compact icon buttons inside `ActionButtons` or `ActionMenu`.
- Pagination: rows selector, showing count, first, previous, page numbers, next, last.
- Column visibility: use the shared columns menu where optional columns exist.
- Row selection: enable only when the module has real bulk actions.

## Status Standard

Use `StatusBadge` everywhere.

| Status family | Color |
| --- | --- |
| Paid, Success, Active, Completed, Received | Green |
| Partial, Warning, Ordered | Amber |
| Pending, Draft, Unpaid | Gray |
| Failed, Cancelled, Blocked, Disabled, Inactive, Overdue | Red |
| Confirmed, Info | Blue/neutral info |

Editable status must use the shared interactive badge pattern with permission checks, disabled saving state, keyboard access, and consistent menu behavior.

## Sidebar Standard

The sidebar uses the app shell in `src/components/layout`.

Rules:

- One expanded top-level group at a time.
- Active route is visually obvious with a left accent and tinted background.
- Sidebar scrolls independently from the main page.
- Collapsed sidebar uses instant tooltips.
- Keyboard focus must remain visible.
- Mobile drawer behavior must remain intact.

## Drawer And Modal Standard

Use:

- `Drawer` for view/detail/receipt panels.
- `FormModal` for create/edit forms.
- `ConfirmationDialog` for destructive or irreversible actions.

Drawer requirements:

- Sticky header.
- Single scrollable body.
- Sticky footer for actions.
- Width target: 35 percent to 40 percent viewport, max around 520px for standard detail drawers.

Modal requirements:

- Shared header, body, close button, and footer spacing.
- Mobile bottom-sheet behavior.
- No nested card-heavy forms unless the workflow genuinely needs grouping.

## Forms

Use `FormControl` for new fields.

Rules:

- Labels use shared typography.
- Help and error text have consistent placement.
- Required fields use the shared required marker.
- Inputs inherit global tokenized focus, border, and radius styles.

## Export Standard

Use `ExportMenu` for table/data exports.

Rules:

- CSV/Excel is for table data only.
- Customer-facing financial documents must be PDF.
- Receipt download buttons should not use spreadsheet export behavior.
- Export actions belong in the table toolbar unless they are report-level actions.

## Folder Structure

```text
src/
  styles/
    design-tokens.css
  components/
    erp/
      ActionButtons.jsx
      ActionMenu.jsx
      Button.jsx
      ConfirmationDialog.jsx
      DataTable.jsx
      Drawer.jsx
      EmptyState.jsx
      ExportMenu.jsx
      FilterBar.jsx
      FilterDropdown.jsx
      FilterToolbar.jsx
      FormControl.jsx
      Input.jsx
      KpiCard.jsx
      LoadingState.jsx
      Modal.jsx
      PageHeader.jsx
      Pagination.jsx
      SearchBar.jsx
      Select.jsx
      SkeletonLoader.jsx
      StatisticsCard.jsx
      StatusBadge.jsx
      TableToolbar.jsx
      index.js
      ERPComponents.css
    common/
      PageHeader.jsx
      PageHeader.css
    layout/
      AppLayout.jsx
      AppLayout.css
  modules/
    <Domain>/
      <Domain>.jsx
      components/
```

## Implementation Status

Completed in this design-system pass:

- Centralized design tokens in `src/styles/design-tokens.css`.
- Wired global CSS to the token layer.
- Tokenized global cards, typography, status colors, tables, pagination, page headers, modals, drawer shell, KPI cards, action menus, and form controls.
- Added shared `Button`, `Input`, `Select`, `FilterToolbar`, `ActionMenu`, `FormControl`, `KpiCard`, `Modal`, `EmptyState`, `SkeletonLoader`, and ERP `PageHeader` exports.
- Redesigned Products as the flagship module standard: page header, KPI cards, filter toolbar, premium data table, product details drawer, and kebab row actions.
- Corrected shared `StatusBadge` aliases so warning/partial/inactive render consistently.
- Removed temporary route debug logs while preserving the fixed navigation architecture.

Still recommended for a later module-by-module migration:

- Replace remaining local table/card CSS that duplicates ERP components.
- Move all eligible module action menus to `ActionMenu`.
- Move new form fields to `FormControl`.
- Move all detail panels to shared `Drawer`.
- Finish one global editable-status policy across Customers, Suppliers, Products, Categories, Warehouses, and Users.
