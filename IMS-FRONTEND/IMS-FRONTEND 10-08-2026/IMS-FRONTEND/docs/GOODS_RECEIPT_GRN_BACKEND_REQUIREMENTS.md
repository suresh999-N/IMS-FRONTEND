# Goods Receipt GRN backend requirements

## Current contract finding

The published `GoodsReceiptDto` used by `POST /api/GoodsReceipts` contains:

- `poId`
- `supplierId`
- `warehouseId`
- `productId`
- `variantId`
- `quantityReceived`
- `price`
- `receiptDate`
- `notes`

It does not declare `grnNumber` or an equivalent receipt-number property. The
`GET /api/GoodsReceipts` success response also has no documented response
schema. The frontend must therefore display `—` when none of the supported GRN
aliases is returned; it must not derive a GRN from the receipt ID.

## Required backend changes

1. Add a non-user-editable `GrnNumber` property to the Goods Receipt entity and
   to every GET/POST response DTO.
2. Add a nullable GRN column first so existing rows can be migrated safely,
   backfill unique values for existing receipts, then make the column required.
3. Add a unique database index or constraint on `GrnNumber`.
4. Generate the value in the Goods Receipt creation service, not in the request
   DTO or frontend.
5. Return the generated value from both `POST /api/GoodsReceipts` and
   `GET /api/GoodsReceipts`.

## Concurrency-safe generation

For a daily format such as `GRN-YYYYMMDD-001`, maintain a server-side sequence
record keyed by the date. Increment and read that sequence in the same database
transaction that creates the Goods Receipt. Use a database sequence, atomic
upsert/update, or a serializable/row-locked transaction appropriate to the
database provider.

The unique constraint on `GrnNumber` is mandatory even when transactional
sequence allocation is used. If a unique conflict occurs, the service should
retry allocation rather than returning or storing a duplicate.

Do not calculate the next sequence using `COUNT`, `MAX` without locking, the
current array length, a timestamp suffix, random values, or client storage.

## Required response example

```json
{
  "grnId": 125,
  "grnNumber": "GRN-20260730-002",
  "poId": 41,
  "poNumber": "PO-20260722-001",
  "supplierId": 8,
  "supplierName": "Jai Kissan",
  "warehouseId": 2,
  "productId": 37,
  "productName": "Industrial Drill",
  "quantityReceived": 5,
  "price": 1800,
  "receiptDate": "2026-07-30T00:00:00Z",
  "notes": null
}
```

The frontend currently recognizes `grnNumber`, `goodsReceiptNumber`,
`receiptNumber`, `grnNo`, and `receiptNo`, but `grnNumber` is the recommended
canonical API property.
