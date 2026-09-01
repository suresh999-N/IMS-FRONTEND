import { formatDate } from '../../../utils/helpers'
import {
  formatPurchaseIndentCurrency,
} from './purchaseIndentDocumentModel'
import '../../POS/Sales/InvoiceDocument.css'

function hasText(value) {
  return String(value ?? '').trim() !== ''
}

function displayDate(value) {
  return value ? formatDate(value) : '-'
}

function statusClassName(status) {
  const normalizedStatus = String(status || '').toLowerCase()

  if (normalizedStatus.includes('approved') || normalizedStatus.includes('converted') || normalizedStatus.includes('ordered')) {
    return 'paid'
  }

  if (normalizedStatus.includes('reject') || normalizedStatus.includes('cancel')) {
    return 'unpaid'
  }

  return 'partially-paid'
}

function CompanyLogo({ company }) {
  if (company.logoUrl) {
    return (
      <img
        className="invoice-document__logo"
        src={company.logoUrl}
        alt={`${company.name || 'Company'} logo`}
      />
    )
  }

  if (!company.name) return null

  const initials = company.name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part.charAt(0).toUpperCase())
    .join('')

  return (
    <span className="invoice-document__logo invoice-document__logo--initials" aria-hidden="true">
      {initials}
    </span>
  )
}

function ContactLine({ label, value }) {
  if (!hasText(value)) return null
  return <p><strong>{label}:</strong> {value}</p>
}

function SupplierBlock({ supplier }) {
  const hasDetails = [
    supplier.companyName,
    supplier.name,
    supplier.gstNumber,
    supplier.phone,
    supplier.email,
    supplier.address,
  ].some(hasText)

  return (
    <section className="invoice-document__address-block">
      <h2>Supplier</h2>
      {!hasDetails ? <p>Not assigned</p> : null}
      {supplier.companyName ? (
        <p className="invoice-document__party-company">{supplier.companyName}</p>
      ) : null}
      {supplier.name && supplier.name !== supplier.companyName ? (
        <p className="invoice-document__party-name">{supplier.name}</p>
      ) : null}
      {supplier.address ? <p className="invoice-document__multiline">{supplier.address}</p> : null}
      <ContactLine label="GSTIN" value={supplier.gstNumber} />
      <ContactLine label="Phone" value={supplier.phone} />
      <ContactLine label="Email" value={supplier.email} />
    </section>
  )
}

function IndentInformationBlock({ model }) {
  return (
    <section className="invoice-document__address-block">
      <h2>Indent Information</h2>
      <p className="invoice-document__party-company">{model.department || 'Department not assigned'}</p>
      <ContactLine label="Requested by" value={model.requestedBy} />
      <ContactLine label="Priority" value={model.priority} />
      <ContactLine label="Required by" value={model.requiredDate ? displayDate(model.requiredDate) : ''} />
      <ContactLine label="Reference" value={model.reference} />
    </section>
  )
}

function SummaryRow({ label, value, strong = false }) {
  return (
    <div className={`invoice-document__summary-row${strong ? ' invoice-document__summary-row--strong' : ''}`}>
      <span>{label}</span>
      <span>{value}</span>
    </div>
  )
}

function ItemCurrency({ available, value }) {
  return available ? formatPurchaseIndentCurrency(value) : '-'
}

function displayNumericValue(value) {
  if (value === '' || value === undefined || value === null) return '-'
  const numericValue = Number(value)
  return Number.isFinite(numericValue) ? numericValue.toLocaleString('en-IN') : String(value)
}

export default function PurchaseIndentDocument({ model, printRoot = false }) {
  const { company, supplier, summary } = model
  const hasCompanyContact = company.address || company.gstNumber || company.phone || company.email
  const hasTermsOrRemarks = model.terms.length > 0 || model.remarks

  return (
    <article
      className={`invoice-document purchase-indent-document${printRoot ? ' invoice-document--print-root' : ''}`}
      aria-label={`Purchase Indent ${model.indentNumber}`}
    >
      <header className="invoice-document__header">
        <div className="invoice-document__brand">
          <CompanyLogo company={company} />
          {(company.name || hasCompanyContact) ? (
            <div className="invoice-document__company-details">
              {company.name ? <h1>{company.name}</h1> : null}
              {company.address ? <p className="invoice-document__multiline">{company.address}</p> : null}
              <div className="invoice-document__company-contact">
                <ContactLine label="GSTIN" value={company.gstNumber} />
                <ContactLine label="Phone" value={company.phone} />
                <ContactLine label="Email" value={company.email} />
              </div>
            </div>
          ) : null}
        </div>

        <div className="invoice-document__title-block">
          <p className="invoice-document__eyebrow">Internal purchase requisition</p>
          <h2>PURCHASE INDENT</h2>
          <span className={`invoice-document__status invoice-document__status--${statusClassName(model.status)}`}>
            {model.status}
          </span>
        </div>
      </header>

      <section className="invoice-document__meta" aria-label="Purchase Indent information">
        <div>
          <span>Indent Number</span>
          <strong>{model.indentNumber}</strong>
        </div>
        {model.indentDate ? (
          <div>
            <span>Request Date</span>
            <strong>{displayDate(model.indentDate)}</strong>
          </div>
        ) : null}
        {model.requiredDate ? (
          <div>
            <span>Required Date</span>
            <strong>{displayDate(model.requiredDate)}</strong>
          </div>
        ) : null}
        <div>
          <span>Priority</span>
          <strong>{model.priority}</strong>
        </div>
      </section>

      <div className="invoice-document__addresses">
        <SupplierBlock supplier={supplier} />
        <IndentInformationBlock model={model} />
      </div>

      <div className="invoice-document__items-wrap">
        <table className="invoice-document__items purchase-indent-document__items">
          <colgroup>
            <col className="purchase-indent-document__col-serial" />
            <col className="purchase-indent-document__col-product" />
            <col className="purchase-indent-document__col-sku" />
            <col className="purchase-indent-document__col-stock" />
            <col className="purchase-indent-document__col-quantity" />
            <col className="purchase-indent-document__col-unit" />
            <col className="purchase-indent-document__col-rate" />
            <col className="purchase-indent-document__col-amount" />
            <col className="purchase-indent-document__col-date" />
          </colgroup>
          <thead>
            <tr>
              <th className="invoice-document__center">S.No.</th>
              <th>Product Name</th>
              <th>SKU</th>
              <th className="invoice-document__numeric">Available</th>
              <th className="invoice-document__numeric">Quantity</th>
              <th>Unit</th>
              <th className="invoice-document__numeric">Rate</th>
              <th className="invoice-document__numeric">Amount</th>
              <th>Required Date</th>
            </tr>
          </thead>
          <tbody>
            {model.items.map((item) => (
              <tr key={item.id}>
                <td className="invoice-document__center">{item.serialNumber}</td>
                <td className="invoice-document__product-name">
                  {item.productName || '-'}
                  {item.notes ? <small className="purchase-indent-document__item-note">{item.notes}</small> : null}
                </td>
                <td>{item.sku || '-'}</td>
                <td className="invoice-document__numeric">
                  {displayNumericValue(item.availableStock)}
                </td>
                <td className="invoice-document__numeric">{item.quantity.toLocaleString('en-IN')}</td>
                <td>{item.unit || '-'}</td>
                <td className="invoice-document__numeric">
                  <ItemCurrency available={item.hasUnitPrice} value={item.unitPrice} />
                </td>
                <td className="invoice-document__numeric invoice-document__line-total">
                  <ItemCurrency available={item.hasAmount} value={item.amount} />
                </td>
                <td>{displayDate(item.requiredDate)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="invoice-document__closing">
        <section className="invoice-document__payment-overview">
          <h2>Requisition Summary</h2>
          <div className="invoice-document__payment-cards">
            <div>
              <span>Total Items</span>
              <strong>{summary.itemCount.toLocaleString('en-IN')}</strong>
            </div>
            <div>
              <span>Total Quantity</span>
              <strong>{summary.totalQuantity.toLocaleString('en-IN')}</strong>
            </div>
          </div>
        </section>

        <section className="invoice-document__summary" aria-label="Purchase Indent totals">
          <SummaryRow label="Line Items" value={summary.itemCount.toLocaleString('en-IN')} />
          <SummaryRow label="Total Quantity" value={summary.totalQuantity.toLocaleString('en-IN')} />
          <SummaryRow
            label="Estimated Value"
            value={summary.hasEstimatedValue ? formatPurchaseIndentCurrency(summary.estimatedValue) : '-'}
            strong
          />
        </section>
      </div>

      <section className="purchase-indent-document__approval">
        <h2>Approval Activity</h2>
        <div className="purchase-indent-document__approval-grid">
          {model.approvalActivity.map((activity) => (
            <div key={activity.key}>
              <span>{activity.label}</span>
              <strong>{activity.person || '-'}</strong>
              <small>{displayDate(activity.date)}</small>
            </div>
          ))}
        </div>
      </section>

      <footer className="invoice-document__footer">
        {hasTermsOrRemarks ? (
          <section className="invoice-document__terms">
            <h2>Notes and Terms</h2>
            {model.remarks ? <p><strong>Remarks:</strong> {model.remarks}</p> : null}
            {model.terms.length > 0 ? (
              <ol>
                {model.terms.map((term, index) => <li key={`${term}-${index}`}>{term}</li>)}
              </ol>
            ) : null}
          </section>
        ) : <div />}

        <section className="invoice-document__signature">
          {model.signatureUrl ? (
            <img src={model.signatureUrl} alt="Approval signature" />
          ) : <span className="invoice-document__signature-space" aria-hidden="true" />}
          <span className="invoice-document__signature-line" />
          <strong>{model.authorizedBy || 'Authorized Signatory'}</strong>
          {company.name ? <small>For {company.name}</small> : null}
        </section>
      </footer>
    </article>
  )
}
