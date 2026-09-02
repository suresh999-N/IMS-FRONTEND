/*
  Returns & Damage enterprise workflow schema upgrade for MySQL/MariaDB.

  Why this exists:
  The upgraded backend reads workflow columns from sales_returns. Older IMS
  databases only have return_id, invoice_id, customer_id, return_date,
  total_amount, and reason, which causes:
    Unknown column 's.status' in 'field list'

  This script is re-runnable. Existing returns are backfilled as Processed
  because legacy return creation already restored stock immediately.
*/

DELIMITER $$

DROP PROCEDURE IF EXISTS ims_add_column_if_missing $$
CREATE PROCEDURE ims_add_column_if_missing(
    IN table_name_in VARCHAR(64),
    IN column_name_in VARCHAR(64),
    IN column_definition_in TEXT
)
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = table_name_in
          AND COLUMN_NAME = column_name_in
    ) THEN
        SET @ddl = CONCAT(
            'ALTER TABLE `',
            table_name_in,
            '` ADD COLUMN `',
            column_name_in,
            '` ',
            column_definition_in
        );
        PREPARE stmt FROM @ddl;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END $$

DROP PROCEDURE IF EXISTS ims_add_index_if_missing $$
CREATE PROCEDURE ims_add_index_if_missing(
    IN table_name_in VARCHAR(64),
    IN index_name_in VARCHAR(64),
    IN index_definition_in TEXT
)
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = table_name_in
          AND INDEX_NAME = index_name_in
    ) THEN
        SET @ddl = CONCAT(
            'ALTER TABLE `',
            table_name_in,
            '` ADD INDEX `',
            index_name_in,
            '` ',
            index_definition_in
        );
        PREPARE stmt FROM @ddl;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END $$

DELIMITER ;

CALL ims_add_column_if_missing('sales_returns', 'status', 'VARCHAR(32) NOT NULL DEFAULT ''Processed''');
CALL ims_add_column_if_missing('sales_returns', 'created_by', 'VARCHAR(160) NULL');
CALL ims_add_column_if_missing('sales_returns', 'created_at', 'DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP');
CALL ims_add_column_if_missing('sales_returns', 'approved_by', 'VARCHAR(160) NULL');
CALL ims_add_column_if_missing('sales_returns', 'approved_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('sales_returns', 'rejected_by', 'VARCHAR(160) NULL');
CALL ims_add_column_if_missing('sales_returns', 'rejected_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('sales_returns', 'processed_by', 'VARCHAR(160) NULL');
CALL ims_add_column_if_missing('sales_returns', 'processed_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('sales_returns', 'refunded_by', 'VARCHAR(160) NULL');
CALL ims_add_column_if_missing('sales_returns', 'refunded_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('sales_returns', 'invoice_adjustment_amount', 'DECIMAL(18, 2) NOT NULL DEFAULT 0');
CALL ims_add_column_if_missing('sales_returns', 'refunded_amount', 'DECIMAL(18, 2) NOT NULL DEFAULT 0');

UPDATE sales_returns
SET
  status = CASE
    WHEN status IS NULL OR TRIM(status) = '' THEN 'Processed'
    WHEN LOWER(TRIM(status)) = 'processed' THEN 'Processed'
    WHEN LOWER(TRIM(status)) = 'pending' THEN 'Pending'
    WHEN LOWER(TRIM(status)) = 'approved' THEN 'Approved'
    WHEN LOWER(TRIM(status)) = 'rejected' THEN 'Rejected'
    WHEN LOWER(TRIM(status)) = 'refunded' THEN 'Refunded'
    ELSE status
  END,
  created_by = COALESCE(NULLIF(TRIM(created_by), ''), 'System'),
  processed_by = CASE
    WHEN LOWER(TRIM(status)) = 'processed' THEN COALESCE(NULLIF(TRIM(processed_by), ''), 'System')
    ELSE processed_by
  END,
  processed_at = CASE
    WHEN LOWER(TRIM(status)) = 'processed' THEN COALESCE(processed_at, return_date, created_at, CURRENT_TIMESTAMP)
    ELSE processed_at
  END,
  invoice_adjustment_amount = COALESCE(invoice_adjustment_amount, 0),
  refunded_amount = COALESCE(refunded_amount, 0);

CREATE TABLE IF NOT EXISTS refund_transactions (
  id INT NOT NULL AUTO_INCREMENT,
  return_id INT NOT NULL,
  customer_id INT NULL,
  invoice_id INT NULL,
  refund_amount DECIMAL(18, 2) NOT NULL,
  refund_method VARCHAR(64) NULL,
  refund_date DATETIME NOT NULL,
  status VARCHAR(32) NOT NULL DEFAULT 'Pending',
  notes VARCHAR(500) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CALL ims_add_index_if_missing('refund_transactions', 'ix_refund_transactions_return_id', '(return_id)');
CALL ims_add_index_if_missing('refund_transactions', 'ix_refund_transactions_customer_id', '(customer_id)');
CALL ims_add_index_if_missing('refund_transactions', 'ix_refund_transactions_invoice_id', '(invoice_id)');

CREATE TABLE IF NOT EXISTS return_history (
  id INT NOT NULL AUTO_INCREMENT,
  return_id INT NOT NULL,
  old_status VARCHAR(32) NULL,
  new_status VARCHAR(32) NOT NULL,
  action VARCHAR(64) NOT NULL,
  actor VARCHAR(160) NULL,
  comments VARCHAR(500) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CALL ims_add_index_if_missing('return_history', 'ix_return_history_return_id', '(return_id)');

CREATE TABLE IF NOT EXISTS customer_return_adjustments (
  id INT NOT NULL AUTO_INCREMENT,
  return_id INT NOT NULL,
  customer_id INT NULL,
  invoice_id INT NULL,
  adjustment_type VARCHAR(64) NOT NULL,
  amount DECIMAL(18, 2) NOT NULL,
  status VARCHAR(32) NOT NULL DEFAULT 'Completed',
  notes VARCHAR(500) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CALL ims_add_index_if_missing('customer_return_adjustments', 'ix_customer_return_adjustments_return_id', '(return_id)');
CALL ims_add_index_if_missing('customer_return_adjustments', 'ix_customer_return_adjustments_customer_id', '(customer_id)');
CALL ims_add_index_if_missing('customer_return_adjustments', 'ix_customer_return_adjustments_invoice_id', '(invoice_id)');

INSERT INTO return_history (return_id, old_status, new_status, action, actor, comments, created_at)
SELECT
  sr.return_id,
  NULL,
  sr.status,
  'Backfilled',
  COALESCE(NULLIF(TRIM(sr.created_by), ''), 'System'),
  'Existing return imported into workflow history.',
  COALESCE(sr.created_at, sr.return_date, CURRENT_TIMESTAMP)
FROM sales_returns sr
WHERE NOT EXISTS (
  SELECT 1
  FROM return_history rh
  WHERE rh.return_id = sr.return_id
);

DROP PROCEDURE IF EXISTS ims_add_index_if_missing;
DROP PROCEDURE IF EXISTS ims_add_column_if_missing;
