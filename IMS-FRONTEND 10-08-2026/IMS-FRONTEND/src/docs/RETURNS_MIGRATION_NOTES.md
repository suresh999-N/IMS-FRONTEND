# Purchase Return & Sales Return Migration

This frontend keeps the existing New Frontend project structure and replaces the Purchase Return and Sales Return UI flows with the working implementations from the supplied old project.

## Migrated Purchase Return
- `src/modules/PurchaseReturns/`
- `src/api/purchaseReturnsApi.js`
- Purchase Return routes support both the existing `/inventory/purchase-returns...` URLs and the legacy `/purchase-returns...` URLs.
- Purchase Return API endpoints required by the old working flow were added to `src/api/endpoints.js`.

## Migrated Sales Return
- `src/modules/POS/ReturnsDamage/SalesReturnsList.jsx`
- `src/modules/POS/ReturnsDamage/CreateSalesReturn.jsx`
- `src/modules/POS/ReturnsDamage/SalesReturnDetails.jsx`
- `src/modules/POS/ReturnsDamage/SalesReturns.css`
- The existing newer `returnsExchangeApi.js` was preserved. Legacy-compatible Sales Return API wrappers were added so the migrated old UI can keep its original request contract.
- Sales Return routes remain under `/pos/returns`.

## Backend
No backend files were changed in this frontend migration. The migrated UI continues to call the Purchase Return and Sales Return API contracts used by the old working frontend.

## Run
1. Extract the project.
2. Open the `IMS-FRONTEND` folder.
3. Run `npm install` if `node_modules` is not present.
4. Configure the `.env` values for the backend.
5. Run `npm run dev`.
