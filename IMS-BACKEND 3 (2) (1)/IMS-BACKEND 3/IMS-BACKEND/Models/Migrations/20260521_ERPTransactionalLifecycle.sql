-- Safe ERP lifecycle migration for the existing IMS MySQL schema.
-- Re-runnable: each column is added only when it is missing.

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

DELIMITER ;

CALL ims_add_column_if_missing('invoices', 'is_cancelled', 'BOOLEAN NOT NULL DEFAULT FALSE');
CALL ims_add_column_if_missing('invoices', 'cancelled_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('invoices', 'cancellation_reason', 'TEXT NULL');

CALL ims_add_column_if_missing('purchase_orders', 'is_cancelled', 'BOOLEAN NOT NULL DEFAULT FALSE');
CALL ims_add_column_if_missing('purchase_orders', 'cancelled_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('purchase_orders', 'cancellation_reason', 'TEXT NULL');
CALL ims_add_column_if_missing('purchase_orders', 'receiving_status', 'ENUM(''pending'',''partial'',''received'') NOT NULL DEFAULT ''pending''');

CALL ims_add_column_if_missing('goods_receipts', 'is_cancelled', 'BOOLEAN NOT NULL DEFAULT FALSE');
CALL ims_add_column_if_missing('goods_receipts', 'cancelled_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('goods_receipts', 'cancellation_reason', 'TEXT NULL');

-- Payments also need the same lightweight lifecycle flag so voided payments can be
-- preserved without hard delete. No payment_status column is added.
CALL ims_add_column_if_missing('customer_payments', 'is_cancelled', 'BOOLEAN NOT NULL DEFAULT FALSE');
CALL ims_add_column_if_missing('customer_payments', 'cancelled_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('customer_payments', 'cancellation_reason', 'TEXT NULL');

CALL ims_add_column_if_missing('supplier_payments', 'is_cancelled', 'BOOLEAN NOT NULL DEFAULT FALSE');
CALL ims_add_column_if_missing('supplier_payments', 'cancelled_at', 'DATETIME NULL');
CALL ims_add_column_if_missing('supplier_payments', 'cancellation_reason', 'TEXT NULL');

UPDATE purchase_orders
SET receiving_status = CASE
    WHEN LOWER(COALESCE(status, '')) IN ('received') THEN 'received'
    WHEN LOWER(COALESCE(status, '')) IN ('partially_received', 'partial received', 'partial') THEN 'partial'
    ELSE 'pending'
END
WHERE receiving_status IS NULL OR receiving_status = 'pending';

DROP PROCEDURE IF EXISTS ims_add_column_if_missing;
