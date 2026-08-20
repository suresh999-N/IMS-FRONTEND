-- MySQL migration for Customer master child sections.
-- Required by the Customer modal parity work with Supplier Contacts, Addresses and Banking.

SET @column_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'customer_contacts'
    AND COLUMN_NAME = 'role'
);
SET @ddl := IF(@column_exists = 0,
  'ALTER TABLE customer_contacts ADD COLUMN role VARCHAR(100) NULL',
  'SELECT 1');
PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @column_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'customer_addresses'
    AND COLUMN_NAME = 'address_line2'
);
SET @ddl := IF(@column_exists = 0,
  'ALTER TABLE customer_addresses ADD COLUMN address_line2 VARCHAR(255) NULL',
  'SELECT 1');
PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @column_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'customer_addresses'
    AND COLUMN_NAME = 'is_primary'
);
SET @ddl := IF(@column_exists = 0,
  'ALTER TABLE customer_addresses ADD COLUMN is_primary TINYINT(1) NOT NULL DEFAULT 0',
  'SELECT 1');
PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @column_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'customer_bank_details'
    AND COLUMN_NAME = 'is_primary'
);
SET @ddl := IF(@column_exists = 0,
  'ALTER TABLE customer_bank_details ADD COLUMN is_primary TINYINT(1) NOT NULL DEFAULT 0',
  'SELECT 1');
PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @column_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'customer_payment_terms'
    AND COLUMN_NAME = 'payment_method'
);
SET @ddl := IF(@column_exists = 0,
  'ALTER TABLE customer_payment_terms ADD COLUMN payment_method VARCHAR(100) NULL',
  'SELECT 1');
PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

UPDATE customer_addresses ca
JOIN (
  SELECT address_id,
         ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY address_id) AS row_number
  FROM customer_addresses
) ranked ON ranked.address_id = ca.address_id
SET ca.is_primary = CASE WHEN ranked.row_number = 1 THEN 1 ELSE 0 END
WHERE ca.is_primary = 0;

UPDATE customer_bank_details cbd
JOIN (
  SELECT bank_id,
         ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY bank_id) AS row_number
  FROM customer_bank_details
) ranked ON ranked.bank_id = cbd.bank_id
SET cbd.is_primary = CASE WHEN ranked.row_number = 1 THEN 1 ELSE 0 END
WHERE cbd.is_primary = 0;
