/*
  Invoice status lifecycle alignment for MySQL/MariaDB.

  Root cause fixed:
  Older IMS databases use invoices.status as ENUM('unpaid','partial','paid').
  The ERP workflow now writes: Sent, Partially Paid, Paid, Overdue, Cancelled.
  MySQL raises "Data truncated for column 'status'" when a value such as
  "Partially Paid" is inserted into the old enum.
*/

START TRANSACTION;

ALTER TABLE invoices
  MODIFY COLUMN status VARCHAR(32) NULL;

UPDATE invoices
SET status = CASE LOWER(TRIM(COALESCE(status, '')))
  WHEN 'unpaid' THEN 'Sent'
  WHEN 'partial' THEN 'Partially Paid'
  WHEN 'partially paid' THEN 'Partially Paid'
  WHEN 'paid' THEN 'Paid'
  WHEN 'overdue' THEN 'Overdue'
  WHEN 'cancelled' THEN 'Cancelled'
  WHEN 'canceled' THEN 'Cancelled'
  WHEN 'draft' THEN 'Draft'
  WHEN 'sent' THEN 'Sent'
  ELSE 'Sent'
END;

ALTER TABLE invoices
  MODIFY COLUMN status VARCHAR(32) NOT NULL DEFAULT 'Sent';

COMMIT;
