CREATE TABLE IF NOT EXISTS putaway_audits (
  putaway_audit_id INT AUTO_INCREMENT PRIMARY KEY,
  product_id INT NOT NULL,
  variant_id INT NULL,
  warehouse_id INT NOT NULL,
  rack_id INT NOT NULL,
  bin_id INT NOT NULL,
  quantity DECIMAL(18, 2) NOT NULL,
  user_id INT NULL,
  user_name VARCHAR(256) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_putaway_audits_product_warehouse (product_id, warehouse_id),
  INDEX idx_putaway_audits_bin (bin_id),
  INDEX idx_putaway_audits_created_at (created_at)
);

CREATE TABLE IF NOT EXISTS bin_transfer_audits (
  bin_transfer_audit_id INT AUTO_INCREMENT PRIMARY KEY,
  product_id INT NOT NULL,
  variant_id INT NULL,
  warehouse_id INT NOT NULL,
  from_bin_id INT NOT NULL,
  to_bin_id INT NOT NULL,
  quantity DECIMAL(18, 2) NOT NULL,
  user_id INT NULL,
  user_name VARCHAR(256) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_bin_transfer_audits_product_warehouse (product_id, warehouse_id),
  INDEX idx_bin_transfer_audits_from_bin (from_bin_id),
  INDEX idx_bin_transfer_audits_to_bin (to_bin_id),
  INDEX idx_bin_transfer_audits_created_at (created_at)
);

CREATE TABLE IF NOT EXISTS warehouse_transfer_audits (
  warehouse_transfer_audit_id INT AUTO_INCREMENT PRIMARY KEY,
  transfer_id INT NOT NULL,
  product_id INT NOT NULL,
  variant_id INT NULL,
  from_warehouse_id INT NOT NULL,
  to_warehouse_id INT NOT NULL,
  quantity DECIMAL(18, 2) NOT NULL,
  user_id INT NULL,
  user_name VARCHAR(256) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_warehouse_transfer_audits_transfer (transfer_id),
  INDEX idx_warehouse_transfer_audits_product (product_id),
  INDEX idx_warehouse_transfer_audits_from_warehouse (from_warehouse_id),
  INDEX idx_warehouse_transfer_audits_to_warehouse (to_warehouse_id),
  INDEX idx_warehouse_transfer_audits_created_at (created_at)
);
