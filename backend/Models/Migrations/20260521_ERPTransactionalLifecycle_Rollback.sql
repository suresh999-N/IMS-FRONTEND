-- Rollback for 20260521_ERPTransactionalLifecycle.sql.
-- Use only if the application code has also been rolled back.

ALTER TABLE supplier_payments
    DROP COLUMN cancellation_reason,
    DROP COLUMN cancelled_at,
    DROP COLUMN is_cancelled;

ALTER TABLE customer_payments
    DROP COLUMN cancellation_reason,
    DROP COLUMN cancelled_at,
    DROP COLUMN is_cancelled;

ALTER TABLE goods_receipts
    DROP COLUMN cancellation_reason,
    DROP COLUMN cancelled_at,
    DROP COLUMN is_cancelled;

ALTER TABLE purchase_orders
    DROP COLUMN receiving_status,
    DROP COLUMN cancellation_reason,
    DROP COLUMN cancelled_at,
    DROP COLUMN is_cancelled;

ALTER TABLE invoices
    DROP COLUMN cancellation_reason,
    DROP COLUMN cancelled_at,
    DROP COLUMN is_cancelled;
