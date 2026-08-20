-- Supplier master status workflow alignment.
-- Safe for existing IMS rows: converts current status values to the canonical
-- lowercase values used by the Supplier API and allows blocked/pending statuses.

ALTER TABLE suppliers
  MODIFY COLUMN status VARCHAR(20) NULL DEFAULT 'active';

UPDATE suppliers
SET status = CASE
  WHEN status IS NULL OR TRIM(status) = '' THEN 'active'
  WHEN LOWER(TRIM(status)) IN ('active', 'blocked', 'inactive', 'pending') THEN LOWER(TRIM(status))
  ELSE 'active'
END;

ALTER TABLE suppliers
  MODIFY COLUMN status VARCHAR(20) NOT NULL DEFAULT 'active';
