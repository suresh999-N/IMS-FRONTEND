import { formatIndentNumber, getLogicalRequiredDate } from '../../../utils/helpers.js'
import { KNOWN_PRODUCT_PRICING } from '../../../utils/productNameUtils.js'

function getPathValue(source, path) {
  return String(path)
    .split('.')
    .reduce((value, key) => value?.[key], source)
}

function firstValue(sources, paths, fallback = '') {
  const sourceList = Array.isArray(sources) ? sources : [sources]

  for (const source of sourceList) {
    for (const path of paths) {
      const value = getPathValue(source, path)
      if (value !== undefined && value !== null && value !== '') {
        return value
      }
    }
  }

  return fallback
}

function textValue(sources, paths, fallback = '') {
  const value = firstValue(sources, paths, fallback)
  return value === undefined || value === null ? fallback : String(value).trim()
}

function numberValue(sources, paths, fallback = 0) {
  const value = Number(firstValue(sources, paths, fallback))
  return Number.isFinite(value) ? value : fallback
}

function normalizeAddress(value) {
  if (!value) return ''
  if (typeof value === 'string') return value.trim()

  if (typeof value === 'object') {
    const directAddress = textValue(value, [
      'fullAddress',
      'FullAddress',
      'formattedAddress',
      'FormattedAddress',
      'address',
      'Address',
    ])

    if (directAddress) return directAddress

    return [
      textValue(value, ['line1', 'Line1', 'addressLine1', 'AddressLine1']),
      textValue(value, ['line2', 'Line2', 'addressLine2', 'AddressLine2']),
      textValue(value, ['city', 'City']),
      textValue(value, ['state', 'State']),
      textValue(value, ['postalCode', 'PostalCode', 'pinCode', 'PinCode', 'zip', 'Zip']),
      textValue(value, ['country', 'Country']),
    ].filter(Boolean).join(', ')
  }

  return String(value)
}

function getIndentId(indent) {
  return firstValue(indent, [
    'purchaseIndentId',
    'PurchaseIndentId',
    'indentId',
    'IndentId',
    'id',
    'Id',
  ])
}

function buildCompany(indent, companyProfile) {
  const embeddedCompany = firstValue(indent, [
    'companyDetails',
    'CompanyDetails',
    'company',
    'Company',
    'organization',
    'Organization',
  ], {})
  const sources = [companyProfile || {}, embeddedCompany || {}, indent]

  return {
    name: textValue(sources, [
      'companyName',
      'CompanyName',
      'businessName',
      'BusinessName',
      'organizationName',
      'OrganizationName',
      'name',
      'Name',
    ], 'IMS Inventory Management System'),
    logoUrl: textValue(sources, [
      'companyLogo',
      'CompanyLogo',
      'logoUrl',
      'LogoUrl',
      'logo',
      'Logo',
      'imageUrl',
      'ImageUrl',
    ], '/ims-package-favicon-v3.png'),
    address: normalizeAddress(firstValue(sources, [
      'companyAddress',
      'CompanyAddress',
      'businessAddress',
      'BusinessAddress',
      'address',
      'Address',
    ])),
    gstNumber: textValue(sources, [
      'companyGstin',
      'CompanyGstin',
      'companyGstNumber',
      'CompanyGstNumber',
      'gstin',
      'GSTIN',
      'gstNumber',
      'GSTNumber',
    ]),
    phone: textValue(sources, [
      'companyPhone',
      'CompanyPhone',
      'phoneNumber',
      'PhoneNumber',
      'phone',
      'Phone',
    ]),
    email: textValue(sources, [
      'companyEmail',
      'CompanyEmail',
      'emailAddress',
      'EmailAddress',
      'email',
      'Email',
    ]),
  }
}

function buildSupplier(indent, supplierRecord) {
  const embeddedSupplier = firstValue(indent, [
    'supplier',
    'Supplier',
    'vendor',
    'Vendor',
    'supplierDetails',
    'SupplierDetails',
  ], {})
  const sources = [supplierRecord || {}, embeddedSupplier || {}, indent]

  return {
    name: textValue([indent, supplierRecord || {}, embeddedSupplier || {}], [
      'supplierDisplay',
      'supplierName',
      'SupplierName',
      'vendorName',
      'VendorName',
      'companyName',
      'CompanyName',
      'name',
      'Name',
    ]),
    companyName: textValue(sources, [
      'companyName',
      'CompanyName',
      'supplierName',
      'SupplierName',
      'vendorName',
      'VendorName',
      'name',
      'Name',
    ]),
    address: normalizeAddress(firstValue(sources, [
      'supplierAddress',
      'SupplierAddress',
      'vendorAddress',
      'VendorAddress',
      'billingAddress',
      'BillingAddress',
      'address',
      'Address',
    ])),
    gstNumber: textValue(sources, [
      'supplierGstin',
      'SupplierGstin',
      'gstin',
      'GSTIN',
      'gstNumber',
      'GSTNumber',
      'taxNumber',
      'TaxNumber',
    ]),
    phone: textValue(sources, [
      'supplierPhone',
      'SupplierPhone',
      'phoneNumber',
      'PhoneNumber',
      'mobile',
      'Mobile',
      'phone',
      'Phone',
    ]),
    email: textValue(sources, [
      'supplierEmail',
      'SupplierEmail',
      'emailAddress',
      'EmailAddress',
      'email',
      'Email',
    ]),
  }
}

function buildItem(item, index, indent, options = {}) {
  const quantity = numberValue(item, [
    'requiredQty',
    'RequiredQty',
    'quantity',
    'Quantity',
    'qty',
    'Qty',
  ])
  const candidateRates = [
    item.unitPrice,
    item.UnitPrice,
    item.estimatedRate,
    item.EstimatedRate,
    item.rate,
    item.Rate,
    item.costPrice,
    item.CostPrice,
    item.price,
    item.Price,
    item.purchasePrice,
    item.PurchasePrice,
    item.cost,
    item.Cost,
    item['product.costPrice'],
    item['product.price'],
  ]
  const positiveItemRate = candidateRates.find((val) => val !== undefined && val !== null && Number(val) > 0)
  let unitPrice = positiveItemRate !== undefined ? Number(positiveItemRate) : 0

  if (unitPrice <= 0) {
    const productId = String(item.productId ?? item.ProductId ?? item.id ?? '').trim()
    const productSku = String(item.sku ?? item.SKU ?? item.productSku ?? item.ProductSku ?? '').trim().toLowerCase()
    const productName = String(item.productName ?? item.ProductName ?? item.name ?? item.Name ?? '').trim().toLowerCase()

    let product = null
    if (productId && options.productMap?.has(productId)) {
      product = options.productMap.get(productId)
    }
    if (!product && Array.isArray(options.products)) {
      product = options.products.find(
        (p) =>
          (productId && String(p.id ?? p.productId ?? p.Id ?? p.ProductId ?? '').trim() === productId) ||
          (productSku && String(p.sku ?? p.SKU ?? '').trim().toLowerCase() === productSku) ||
          (productName && String(p.name ?? p.Name ?? '').trim().toLowerCase() === productName)
      )
    }

    if (product) {
      const productRateCandidates = [
        product.costPrice,
        product.CostPrice,
        product.cost_price,
        product.cost,
        product.Cost,
        product.purchasePrice,
        product.PurchasePrice,
        product.price,
        product.Price,
      ]
      const foundProductRate = productRateCandidates.find((val) => val !== undefined && val !== null && Number(val) > 0)
      if (foundProductRate !== undefined) {
        unitPrice = Number(foundProductRate)
      }
    }

    if (unitPrice <= 0) {
      const knownPricing = KNOWN_PRODUCT_PRICING[productId] || (productSku ? KNOWN_PRODUCT_PRICING[productSku] || KNOWN_PRODUCT_PRICING[item.sku || item.SKU] : null)
      if (knownPricing) {
        unitPrice = Number(knownPricing.costPrice || knownPricing.price || 0)
      }
    }
  }

  const directAmount = numberValue(item, [
    'amount',
    'Amount',
    'estimatedAmount',
    'EstimatedAmount',
    'lineTotal',
    'LineTotal',
    'totalAmount',
    'TotalAmount',
    'total',
    'Total',
  ])
  const computedAmount = quantity * unitPrice
  const amount = directAmount > 0 ? directAmount : computedAmount

  return {
    id: String(firstValue(item, [
      'purchaseIndentItemId',
      'PurchaseIndentItemId',
      'indentItemId',
      'IndentItemId',
      'id',
      'Id',
    ], `indent-item-${index + 1}`)),
    serialNumber: index + 1,
    productName: textValue(item, [
      'productName',
      'ProductName',
      'itemName',
      'ItemName',
      'name',
      'Name',
    ], textValue(indent, ['productName', 'ProductName'], '-')),
    sku: textValue(item, [
      'sku',
      'SKU',
      'productSku',
      'ProductSku',
      'productCode',
      'ProductCode',
      'itemCode',
      'ItemCode',
      'code',
      'Code',
    ]),
    availableStock: firstValue(item, ['availableStock', 'AvailableStock', 'stock', 'Stock'], ''),
    quantity,
    unit: textValue(item, [
      'unitName',
      'UnitName',
      'unit',
      'Unit',
      'uom',
      'Uom',
      'UOM',
    ]),
    unitPrice,
    hasUnitPrice: unitPrice > 0,
    amount,
    hasAmount: amount > 0,
    requiredDate: getLogicalRequiredDate(
      firstValue(item, [
        'requiredDate',
        'RequiredDate',
        'expectedDeliveryDate',
        'ExpectedDeliveryDate',
      ], firstValue(indent, [
        'requiredDate',
        'RequiredDate',
        'expectedDeliveryDate',
        'ExpectedDeliveryDate',
      ])),
      firstValue(indent, [
        'indentDate',
        'IndentDate',
        'requestDate',
        'RequestDate',
        'requestedDate',
        'RequestedDate',
        'createdAt',
        'CreatedAt',
      ])
    ),
    notes: textValue(item, [
      'remarks',
      'Remarks',
      'notes',
      'Notes',
      'description',
      'Description',
    ]) || (Number(firstValue(item, ['availableStock', 'AvailableStock', 'stock', 'Stock'], -1)) === 0 ? textValue(indent, ['remarks', 'Remarks', 'notes', 'Notes']) : ''),
  }
}

function buildApprovalActivity(indent) {
  const createdPerson = textValue(indent, ['createdByDisplay', 'createdByName', 'CreatedByName', 'createdBy', 'CreatedBy'])
  const createdDate = firstValue(indent, ['createdAt', 'CreatedAt', 'createdOn', 'CreatedOn', 'createdDate', 'CreatedDate'])

  const approvedPerson = textValue(indent, ['approvedByDisplay', 'approvedByName', 'ApprovedByName', 'approvedBy', 'ApprovedBy'])
  const approvedDate = firstValue(indent, ['approvedAt', 'ApprovedAt', 'approvedOn', 'ApprovedOn', 'approvedDate', 'ApprovedDate'])

  const rejectedPerson = textValue(indent, ['rejectedByDisplay', 'rejectedByName', 'RejectedByName', 'rejectedBy', 'RejectedBy'])
  const rejectedDate = firstValue(indent, ['rejectedAt', 'RejectedAt', 'rejectedOn', 'RejectedOn', 'rejectedDate', 'RejectedDate'])

  const directUpdatedPerson = textValue(indent, ['updatedByDisplay', 'updatedByName', 'UpdatedByName', 'modifiedByName', 'ModifiedByName', 'updatedBy', 'UpdatedBy'])
  const directUpdatedDate = firstValue(indent, ['updatedAt', 'UpdatedAt', 'modifiedAt', 'ModifiedAt', 'updatedOn', 'UpdatedOn', 'modifiedOn', 'ModifiedOn'])

  let updatedPerson = directUpdatedPerson
  let updatedDate = directUpdatedDate

  if (!updatedPerson || updatedPerson === 'Not Updated') {
    if (approvedPerson) {
      updatedPerson = approvedPerson
    } else if (rejectedPerson) {
      updatedPerson = rejectedPerson
    } else if (updatedDate) {
      updatedPerson = createdPerson
    }
  }

  if (!updatedDate) {
    if (approvedDate) {
      updatedDate = approvedDate
    } else if (rejectedDate) {
      updatedDate = rejectedDate
    }
  }

  return [
    {
      key: 'created',
      label: 'Created',
      person: createdPerson,
      date: createdDate,
    },
    {
      key: 'updated',
      label: 'Updated',
      person: updatedPerson,
      date: updatedDate,
    },
    {
      key: 'approved',
      label: 'Approved',
      person: approvedPerson,
      date: approvedDate,
    },
    {
      key: 'rejected',
      label: 'Rejected',
      person: rejectedPerson,
      date: rejectedDate,
    },
    {
      key: 'converted',
      label: 'Converted',
      person: textValue(indent, ['convertedByDisplay', 'convertedByName', 'ConvertedByName', 'convertedBy', 'ConvertedBy']),
      date: firstValue(indent, ['convertedAt', 'ConvertedAt', 'convertedOn', 'ConvertedOn', 'convertedDate', 'ConvertedDate']),
    },
  ]
}

function normalizeTerms(value) {
  if (Array.isArray(value)) {
    return value.map((entry) => String(entry || '').trim()).filter(Boolean)
  }

  return String(value || '')
    .split(/\r?\n|;/)
    .map((entry) => entry.trim())
    .filter(Boolean)
}

export function formatPurchaseIndentCurrency(value) {
  const amount = Number(value)
  if (!Number.isFinite(amount)) return '-'

  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount)
}

export function buildPurchaseIndentDocumentModel(indent = {}, options = {}) {
  const rawItems = Array.isArray(indent.items) ? indent.items : []
  const items = (rawItems.length > 0 ? rawItems : [indent])
    .map((item, index) => buildItem(item, index, indent, options))
  const directEstimatedValue = numberValue(indent, [
    'estimatedValue',
    'EstimatedValue',
    'totalAmount',
    'TotalAmount',
    'amount',
    'Amount',
  ])
  const calculatedEstimatedValue = items.reduce((sum, item) => sum + (item.amount || 0), 0)
  const estimatedValue = calculatedEstimatedValue > 0 ? calculatedEstimatedValue : (directEstimatedValue > 0 ? directEstimatedValue : 0)
  const supplier = buildSupplier(indent, options.supplier)
  const status = textValue(indent, ['status', 'Status'], 'Pending')
  const approvedBy = textValue(indent, [
    'approvedByDisplay',
    'approvedByName',
    'ApprovedByName',
    'approvedBy',
    'ApprovedBy',
  ])

  return {
    id: getIndentId(indent),
    indentNumber: formatIndentNumber(
      textValue(indent, [
        'indentNumber',
        'IndentNumber',
        'indentNo',
        'IndentNo',
        'purchaseIndentNumber',
        'PurchaseIndentNumber',
      ], String(getIndentId(indent) || ''))
    ),
    indentDate: firstValue(indent, [
      'indentDate',
      'IndentDate',
      'requestDate',
      'RequestDate',
      'requestedDate',
      'RequestedDate',
      'createdAt',
      'CreatedAt',
    ]),
    requiredDate: getLogicalRequiredDate(
      firstValue(indent, [
        'requiredDate',
        'RequiredDate',
        'expectedDeliveryDate',
        'ExpectedDeliveryDate',
      ], items[0]?.requiredDate || ''),
      firstValue(indent, [
        'indentDate',
        'IndentDate',
        'requestDate',
        'RequestDate',
        'requestedDate',
        'RequestedDate',
        'createdAt',
        'CreatedAt',
      ])
    ),
    status,
    priority: textValue(indent, ['priority', 'Priority'], 'Medium'),
    reference: textValue(indent, [
      'reference',
      'Reference',
      'referenceNumber',
      'ReferenceNumber',
      'purchaseOrderNumber',
      'PurchaseOrderNumber',
      'poNumber',
      'PoNumber',
    ]),
    department: textValue(indent, [
      'departmentDisplay',
      'departmentName',
      'DepartmentName',
      'department',
      'Department',
    ]),
    requestedBy: textValue(indent, [
      'requestedByDisplay',
      'requestedByName',
      'RequestedByName',
      'requesterName',
      'RequesterName',
      'requestedBy',
      'RequestedBy',
    ]),
    company: buildCompany(indent, options.companyProfile),
    supplier,
    items,
    summary: {
      itemCount: items.length,
      totalQuantity: items.reduce((sum, item) => sum + item.quantity, 0),
      estimatedValue,
      hasEstimatedValue: estimatedValue > 0,
    },
    remarks: textValue(indent, ['remarks', 'Remarks', 'notes', 'Notes']),
    terms: normalizeTerms(firstValue(indent, [
      'terms',
      'Terms',
      'termsAndConditions',
      'TermsAndConditions',
    ])),
    approvalActivity: buildApprovalActivity(indent),
    authorizedBy: approvedBy || textValue(indent, [
      'authorizedBy',
      'AuthorizedBy',
      'createdByDisplay',
      'createdByName',
      'CreatedByName',
    ]),
    signatureUrl: textValue(indent, [
      'signatureUrl',
      'SignatureUrl',
      'approvalSignatureUrl',
      'ApprovalSignatureUrl',
    ]),
    generatedBy: options.generatedBy || '',
  }
}

export function validatePurchaseIndentDocumentModel(model) {
  if (!model?.indentNumber) {
    return 'Purchase Indent number is unavailable. The document was not generated.'
  }

  if (!Array.isArray(model.items) || model.items.length === 0) {
    return 'Purchase Indent line items are unavailable. The document was not generated.'
  }

  return ''
}

export function getPurchaseIndentPdfFilename(model) {
  const cleanValue = String(model?.indentNumber || 'purchase-indent')
    .trim()
    .split('')
    .filter((character) => character.charCodeAt(0) >= 32)
    .join('')
    .replace(/[<>:"/\\|?*]/g, '-')
    .replace(/\s+/g, '-')

  return `${cleanValue || 'purchase-indent'}.pdf`
}
