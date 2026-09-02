ALTER TABLE customer_payments
  ADD COLUMN payment_number VARCHAR(32) NULL,
  ADD COLUMN status VARCHAR(32) NULL,
  ADD COLUMN outstanding_before DECIMAL(18, 2) NOT NULL DEFAULT 0,
  ADD COLUMN outstanding_after DECIMAL(18, 2) NOT NULL DEFAULT 0,
  ADD COLUMN created_by VARCHAR(160) NULL,
  ADD COLUMN created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

UPDATE customer_payments
SET
  payment_number = CONCAT('PAY-', DATE_FORMAT(payment_date, '%Y%m%d'), '-', LPAD(payment_id, 3, '0')),
  status = CASE
    WHEN is_cancelled = 1 THEN 'Cancelled'
    WHEN reference_number IS NULL OR TRIM(reference_number) = '' THEN 'Pending'
    ELSE 'Reconciled'
  END,
  outstanding_before = amount,
  outstanding_after = 0,
  created_by = COALESCE(created_by, 'System')
WHERE payment_number IS NULL;

CREATE UNIQUE INDEX ux_customer_payments_payment_number
  ON customer_payments (payment_number);
