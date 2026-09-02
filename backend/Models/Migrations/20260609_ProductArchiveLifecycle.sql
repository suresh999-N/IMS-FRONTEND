/*
  Product archive lifecycle.
  Adds a separate archive flag so archived products can be restored without
  using the permanent delete flag.
*/

SET @add_is_archived = (
  SELECT IF(
    COUNT(*) = 0,
    'ALTER TABLE products ADD COLUMN is_archived TINYINT(1) NOT NULL DEFAULT 0',
    'SELECT 1'
  )
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'products'
    AND COLUMN_NAME = 'is_archived'
);

PREPARE stmt FROM @add_is_archived;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

UPDATE products
SET is_archived = 1
WHERE is_deleted = 0
  AND LOWER(status) IN ('inactive', 'archived');

SET @add_archive_index = (
  SELECT IF(
    COUNT(*) = 0,
    'CREATE INDEX idx_products_is_archived ON products (is_archived)',
    'SELECT 1'
  )
  FROM INFORMATION_SCHEMA.STATISTICS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'products'
    AND INDEX_NAME = 'idx_products_is_archived'
);

PREPARE stmt FROM @add_archive_index;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
