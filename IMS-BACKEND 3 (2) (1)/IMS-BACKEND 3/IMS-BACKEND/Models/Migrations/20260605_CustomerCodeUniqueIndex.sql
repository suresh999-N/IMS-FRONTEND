-- Production safeguard for auto-generated customer identifiers.
-- New customers use the global sequential format CUST-000001, CUST-000002, ...
-- Run after resolving any existing duplicate customer_code values.

UPDATE customers
SET customer_code = CONCAT('CUST-', LPAD(customer_id, 6, '0'))
WHERE customer_code IS NULL OR TRIM(customer_code) = '';

ALTER TABLE customers
  MODIFY customer_code VARCHAR(64) NOT NULL;

CREATE UNIQUE INDEX ux_customers_customer_code
  ON customers (customer_code);
