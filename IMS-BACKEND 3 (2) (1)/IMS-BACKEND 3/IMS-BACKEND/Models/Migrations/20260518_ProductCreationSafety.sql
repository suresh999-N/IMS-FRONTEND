/*
  Product creation safety migration.
  Review table/column names against the deployed database before running.
  This script is written for SQL Server because production was described as SQL Server.
*/

BEGIN TRANSACTION;

;WITH DuplicateBarcodes AS (
    SELECT
        product_id,
        barcode,
        ROW_NUMBER() OVER (
            PARTITION BY barcode
            ORDER BY product_id
        ) AS duplicate_number
    FROM products
    WHERE barcode IS NOT NULL
      AND LTRIM(RTRIM(barcode)) <> ''
)
UPDATE products
SET barcode = CONCAT(
    'BAR-',
    FORMAT(GETUTCDATE(), 'yyyyMMdd'),
    '-',
    RIGHT(CONCAT('000', product_id), 3)
)
WHERE product_id IN (
    SELECT product_id
    FROM DuplicateBarcodes
    WHERE duplicate_number > 1
);

UPDATE products
SET barcode = CONCAT(
    'BAR-',
    FORMAT(GETUTCDATE(), 'yyyyMMdd'),
    '-',
    RIGHT(CONCAT('000', product_id), 3)
)
WHERE barcode IS NULL
   OR LTRIM(RTRIM(barcode)) = '';

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_products_barcode'
      AND object_id = OBJECT_ID('products')
)
BEGIN
    CREATE UNIQUE INDEX UX_products_barcode
        ON products(barcode)
        WHERE barcode IS NOT NULL AND barcode <> '';
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_product_variants_product_sku'
      AND object_id = OBJECT_ID('product_variants')
)
BEGIN
    CREATE UNIQUE INDEX UX_product_variants_product_sku
        ON product_variants(product_id, sku)
        WHERE sku IS NOT NULL AND sku <> '';
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_variant_attribute_values_unique'
      AND object_id = OBJECT_ID('variant_attribute_values')
)
BEGIN
    CREATE UNIQUE INDEX UX_variant_attribute_values_unique
        ON variant_attribute_values(variant_id, attribute_id, value_id);
END;

COMMIT TRANSACTION;
