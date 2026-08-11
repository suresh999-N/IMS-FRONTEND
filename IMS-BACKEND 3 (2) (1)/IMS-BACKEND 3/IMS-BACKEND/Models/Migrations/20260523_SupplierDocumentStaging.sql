SET @supplier_document_is_temporary_exists = (
  SELECT COUNT(*)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'supplier_documents'
    AND column_name = 'is_temporary'
);

SET @supplier_document_is_temporary_sql = IF(
  @supplier_document_is_temporary_exists = 0,
  'ALTER TABLE supplier_documents ADD COLUMN is_temporary BOOLEAN NOT NULL DEFAULT FALSE',
  'SELECT 1'
);

PREPARE supplier_document_is_temporary_statement FROM @supplier_document_is_temporary_sql;
EXECUTE supplier_document_is_temporary_statement;
DEALLOCATE PREPARE supplier_document_is_temporary_statement;
