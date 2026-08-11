/*
  Stock movement type safety migration for MySQL/MariaDB.

  Use this if product creation still reports:
  "Data truncated for column 'movement_type'"

  Root cause: movement_type is commonly deployed as an ENUM or too-short column.
  The application now writes these canonical values:
  IN, OUT, SALE, PURCHASE, ADJUSTMENT, TRANSFER, OPENING.
*/

START TRANSACTION;

ALTER TABLE stock_movements
    MODIFY COLUMN movement_type VARCHAR(20) NULL;

UPDATE stock_movements
SET movement_type = CASE UPPER(TRIM(COALESCE(movement_type, '')))
    WHEN 'STOCK_IN' THEN 'IN'
    WHEN 'RETURN_IN' THEN 'IN'
    WHEN 'STOCK_OUT' THEN 'OUT'
    WHEN 'RETURN_OUT' THEN 'OUT'
    WHEN 'OPENING_STOCK' THEN 'OPENING'
    WHEN 'OPENING' THEN 'OPENING'
    WHEN 'IN' THEN 'IN'
    WHEN 'OUT' THEN 'OUT'
    WHEN 'SALE' THEN 'SALE'
    WHEN 'PURCHASE' THEN 'PURCHASE'
    WHEN 'ADJUSTMENT' THEN 'ADJUSTMENT'
    WHEN 'TRANSFER' THEN 'TRANSFER'
    ELSE 'ADJUSTMENT'
END;

ALTER TABLE stock_movements
    MODIFY COLUMN movement_type VARCHAR(20) NOT NULL;

COMMIT;
