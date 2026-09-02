-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: ims
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20260427121016_InitialCreate','8.0.0'),('20260710181130_UpdateLoginHistory','8.0.0'),('20260711115855_AddAccountLockout','8.0.0'),('20260711131140_AddEmailVerification','8.0.0'),('20260713044929_AddPurchaseIndentModule','8.0.0'),('20260724142758_AddPendingUsers','8.0.0'),('20260730050421_AddPurchaseReturnModule','8.0.0'),('20260730074105_AddGrnNumber','8.0.0'),('20260731072247_AddLoginOtpSupport','8.0.0'),('20260806075627_AddSupplierInvoiceFields','8.0.0'),('20260806082243_AddDiscountTaxLineTotalToGoodsReceiptItems','8.0.0'),('20260806083928_AddDiscountAndTaxToPurchaseOrderItems','8.0.0');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `attribute_values`
--

DROP TABLE IF EXISTS `attribute_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `attribute_values` (
  `value_id` int NOT NULL AUTO_INCREMENT,
  `attribute_id` int DEFAULT NULL,
  `value` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`value_id`),
  KEY `attribute_id` (`attribute_id`),
  CONSTRAINT `attribute_values_ibfk_1` FOREIGN KEY (`attribute_id`) REFERENCES `attributes` (`attribute_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `attribute_values`
--

LOCK TABLES `attribute_values` WRITE;
/*!40000 ALTER TABLE `attribute_values` DISABLE KEYS */;
/*!40000 ALTER TABLE `attribute_values` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `attributes`
--

DROP TABLE IF EXISTS `attributes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `attributes` (
  `attribute_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`attribute_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `attributes`
--

LOCK TABLES `attributes` WRITE;
/*!40000 ALTER TABLE `attributes` DISABLE KEYS */;
INSERT INTO `attributes` VALUES (1,'Black (colour may vary)');
/*!40000 ALTER TABLE `attributes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `audit_logs`
--

DROP TABLE IF EXISTS `audit_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `audit_logs` (
  `log_id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `action` varchar(100) DEFAULT NULL,
  `module` varchar(100) DEFAULT NULL,
  `record_id` int DEFAULT NULL,
  `description` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `table_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`log_id`),
  KEY `idx_audit_logs_created_at` (`created_at` DESC)
) ENGINE=InnoDB AUTO_INCREMENT=406 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `audit_logs`
--

LOCK TABLES `audit_logs` WRITE;
/*!40000 ALTER TABLE `audit_logs` DISABLE KEYS */;
INSERT INTO `audit_logs` VALUES (1,105,'CREATE_PURCHASE_INDENT','PurchaseIndents',1,'Suresh Purchase Indent Created : IND-20260817-001','2026-08-17 01:31:18','purchase_indents'),(2,105,'CREATE_PURCHASE_INDENT','PurchaseIndents',2,'Suresh Purchase Indent Created : IND-20260817-001','2026-08-17 01:41:13','purchase_indents'),(3,105,'CREATE_PURCHASE_INDENT','PurchaseIndents',3,'Suresh Purchase Indent Created : IND-20260817-002','2026-08-17 01:50:23','purchase_indents'),(4,105,'CREATE_PURCHASE_INDENT','PurchaseIndents',4,'Suresh Purchase Indent Created : IND-20260817-001','2026-08-17 03:29:20','purchase_indents'),(5,NULL,'UPDATE','Permissions',38,'Permissions updated for Dashboard. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-17 04:35:33','role_permissions'),(6,NULL,'UPDATE','Permissions',63,'Permissions updated for Products. View:True->False, Add:True->False, Edit:True->False, Delete:True->False','2026-08-17 04:35:34','role_permissions'),(7,NULL,'UPDATE','Permissions',23,'Permissions updated for Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(8,NULL,'UPDATE','Permissions',143,'Permissions updated for Sub Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(9,NULL,'UPDATE','Permissions',18,'Permissions updated for Brands. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(10,NULL,'UPDATE','Permissions',163,'Permissions updated for Units. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(11,NULL,'UPDATE','Permissions',58,'Permissions updated for Product Attributes. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(12,NULL,'UPDATE','Permissions',68,'Permissions updated for Product Variants. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(13,NULL,'UPDATE','Permissions',98,'Permissions updated for Stock. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(14,NULL,'UPDATE','Permissions',128,'Permissions updated for Stock Movements. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(15,NULL,'UPDATE','Permissions',123,'Permissions updated for Stock Ledger. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(16,NULL,'UPDATE','Permissions',108,'Permissions updated for Stock Adjustments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(17,NULL,'UPDATE','Permissions',103,'Permissions updated for Stock Adjustment Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(18,NULL,'UPDATE','Permissions',138,'Permissions updated for Stock Transfers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(19,NULL,'UPDATE','Permissions',133,'Permissions updated for Stock Transfer Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(20,NULL,'UPDATE','Permissions',118,'Permissions updated for Stock Audits. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(21,NULL,'UPDATE','Permissions',113,'Permissions updated for Stock Audit Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(22,NULL,'UPDATE','Permissions',43,'Permissions updated for Goods Receipts. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(23,NULL,'UPDATE','Permissions',73,'Permissions updated for Purchases. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(24,NULL,'UPDATE','Permissions',93,'Permissions updated for Sales. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(25,NULL,'UPDATE','Permissions',48,'Permissions updated for Inventory Audit. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(26,NULL,'UPDATE','Permissions',13,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(27,NULL,'UPDATE','Permissions',153,'Permissions updated for Suppliers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(28,NULL,'UPDATE','Permissions',33,'Permissions updated for Customers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(29,NULL,'UPDATE','Permissions',28,'Permissions updated for Customer Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(30,NULL,'UPDATE','Permissions',148,'Permissions updated for Supplier Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(31,NULL,'UPDATE','Permissions',173,'Permissions updated for Warehouses. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(32,NULL,'UPDATE','Permissions',78,'Permissions updated for Reports. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(33,NULL,'UPDATE','Permissions',53,'Permissions updated for Notifications. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(34,NULL,'UPDATE','Permissions',3,'Permissions updated for Accounting. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(35,NULL,'UPDATE','Permissions',83,'Permissions updated for Returns & Exchanges. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(36,NULL,'UPDATE','Permissions',168,'Permissions updated for Users. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(37,NULL,'UPDATE','Permissions',88,'Permissions updated for Roles. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(38,NULL,'UPDATE','Permissions',8,'Permissions updated for Audit Logs. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(39,NULL,'UPDATE','Permissions',158,'Permissions updated for System Settings. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(40,NULL,'UPDATE','Permissions',182,'Permissions updated for Purchase Indent. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(41,NULL,'UPDATE','Permissions',183,'Permissions updated for Purchase Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(42,NULL,'UPDATE','Permissions',184,'Permissions updated for Sales Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-17 04:35:34','role_permissions'),(43,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:True->False, Add:True->False, Edit:True->False, Delete:True->False','2026-08-17 04:35:34','role_permissions'),(44,109,'CREATE_PURCHASE_INDENT','PurchaseIndents',5,'nandhitha Purchase Indent Created : IND-20260817-001','2026-08-17 04:58:00','purchase_indents'),(45,109,'APPROVE_PURCHASE_INDENT','PurchaseIndents',5,'nandhitha Purchase Indent Approved : IND-20260817-001','2026-08-17 04:58:26','purchase_indents'),(46,109,'CONVERT_PURCHASE_ORDER','PurchaseOrders',1,'nandhitha Purchase Order created from IND-20260817-001','2026-08-17 04:58:47','purchase_orders'),(47,109,'CONVERT_PURCHASE_INDENT','PurchaseIndents',5,'nandhitha Purchase Indent IND-20260817-001 converted to Purchase Order PO-20260817-001','2026-08-17 04:58:47','purchase_indents'),(48,109,'APPROVE_PURCHASE_ORDER','Purchases',1,'nandhitha Purchase Order PO-20260817-001 approved','2026-08-17 05:01:18','purchase_orders'),(49,109,'CREATE_GOODS_RECEIPT','Inventory',1,'nandhitha created GRN-1','2026-08-17 05:01:19','goods_receipts'),(50,109,'CREATE_CATEGORY','Categories',3,'nandhitha created MOBILES','2026-08-17 05:29:28','categories'),(51,109,'Create','Purchase Return',1,'nandhitha Purchase Return #PRR-IXPJ46 created for Supplier Universal Farm Equipment Pvt. Ltd.','2026-08-17 05:34:44','purchase_returns'),(52,109,'CREATE_INVOICE','Sales',1,'nandhitha created INV-20260817-001','2026-08-17 05:45:27','invoices'),(53,109,'CUSTOMER_PAYMENT_CREATED_FROM_INVOICE','Payments',1,'nandhitha Opening payment recorded for invoice INV-20260817-001','2026-08-17 05:45:27','customer_payments'),(54,103,'Update','Profile',103,'Profile updated by Kurapati Bhargava','2026-08-18 12:03:53','Users'),(55,103,'CREATE_INVOICE','Sales',2,'Kurapati Bhargava created INV-20260819-001','2026-08-19 00:30:57','invoices'),(56,109,'CREATE_PURCHASE_INDENT','PurchaseIndents',6,'nandhitha Purchase Indent Created : IND-20260819-001','2026-08-19 01:29:33','purchase_indents'),(57,109,'APPROVE_PURCHASE_INDENT','PurchaseIndents',6,'nandhitha Purchase Indent Approved : IND-20260819-001','2026-08-19 01:29:39','purchase_indents'),(58,109,'CONVERT_PURCHASE_ORDER','PurchaseOrders',2,'nandhitha Purchase Order created from IND-20260819-001','2026-08-19 01:29:44','purchase_orders'),(59,109,'CONVERT_PURCHASE_INDENT','PurchaseIndents',6,'nandhitha Purchase Indent IND-20260819-001 converted to Purchase Order PO-20260819-001','2026-08-19 01:29:44','purchase_indents'),(60,109,'APPROVE_PURCHASE_ORDER','Purchases',2,'nandhitha Purchase Order PO-20260819-001 approved','2026-08-19 01:30:41','purchase_orders'),(61,109,'CREATE_GOODS_RECEIPT','Inventory',2,'nandhitha created GRN-2','2026-08-19 01:30:42','goods_receipts'),(62,103,'Create','Sales Return',1,'Kurapati Bhargava Sales Return SRR-1001 created for Customer Nandhitha Sri','2026-08-19 03:41:57','sales_returns'),(63,103,'Create','Purchase Return',2,'Kurapati Bhargava Purchase Return #PRR-SIC3BV created for Supplier Universal Farm Equipment Pvt. Ltd.','2026-08-19 06:45:25','purchase_returns'),(64,105,'Update','Profile',105,'Profile updated by gfgkhjhtguh','2026-08-19 12:24:19','Users'),(65,105,'Update','Profile',105,'Profile updated by gfgkhjhtguh','2026-08-19 12:28:13','Users'),(66,105,'Update','Profile',105,'Profile updated by jghgtyghfgyg','2026-08-19 12:28:27','Users'),(67,105,'Update','Profile',105,'Profile updated by Suresh','2026-08-19 12:33:53','Users'),(68,NULL,'UPDATE','Permissions',217,'Permissions updated for Accounting. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(69,NULL,'UPDATE','Permissions',221,'Permissions updated for Audit Logs. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(70,NULL,'UPDATE','Permissions',209,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(71,NULL,'UPDATE','Permissions',192,'Permissions updated for Brands. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(72,NULL,'UPDATE','Permissions',190,'Permissions updated for Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(73,NULL,'UPDATE','Permissions',212,'Permissions updated for Customer Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(74,NULL,'UPDATE','Permissions',211,'Permissions updated for Customers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(75,NULL,'UPDATE','Permissions',188,'Permissions updated for Dashboard. View:False->True, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(76,NULL,'UPDATE','Permissions',205,'Permissions updated for Goods Receipts. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(77,NULL,'UPDATE','Permissions',208,'Permissions updated for Inventory Audit. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(78,NULL,'UPDATE','Permissions',216,'Permissions updated for Notifications. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(79,NULL,'UPDATE','Permissions',194,'Permissions updated for Product Attributes. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(80,NULL,'UPDATE','Permissions',195,'Permissions updated for Product Variants. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(81,NULL,'UPDATE','Permissions',189,'Permissions updated for Products. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(82,NULL,'UPDATE','Permissions',223,'Permissions updated for Purchase Indent. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(83,NULL,'UPDATE','Permissions',224,'Permissions updated for Purchase Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(84,NULL,'UPDATE','Permissions',206,'Permissions updated for Purchases. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(85,NULL,'UPDATE','Permissions',215,'Permissions updated for Reports. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(86,NULL,'UPDATE','Permissions',218,'Permissions updated for Returns & Exchanges. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(87,NULL,'UPDATE','Permissions',220,'Permissions updated for Roles. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(88,NULL,'UPDATE','Permissions',207,'Permissions updated for Sales. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(89,NULL,'UPDATE','Permissions',225,'Permissions updated for Sales Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(90,NULL,'UPDATE','Permissions',196,'Permissions updated for Stock. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(91,NULL,'UPDATE','Permissions',200,'Permissions updated for Stock Adjustment Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(92,NULL,'UPDATE','Permissions',199,'Permissions updated for Stock Adjustments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(93,NULL,'UPDATE','Permissions',204,'Permissions updated for Stock Audit Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(94,NULL,'UPDATE','Permissions',203,'Permissions updated for Stock Audits. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(95,NULL,'UPDATE','Permissions',198,'Permissions updated for Stock Ledger. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(96,NULL,'UPDATE','Permissions',197,'Permissions updated for Stock Movements. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(97,NULL,'UPDATE','Permissions',202,'Permissions updated for Stock Transfer Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(98,NULL,'UPDATE','Permissions',201,'Permissions updated for Stock Transfers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(99,NULL,'UPDATE','Permissions',191,'Permissions updated for Sub Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(100,NULL,'UPDATE','Permissions',213,'Permissions updated for Supplier Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(101,NULL,'UPDATE','Permissions',210,'Permissions updated for Suppliers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(102,NULL,'UPDATE','Permissions',222,'Permissions updated for System Settings. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(103,NULL,'UPDATE','Permissions',193,'Permissions updated for Units. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(104,NULL,'UPDATE','Permissions',219,'Permissions updated for Users. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(105,NULL,'UPDATE','Permissions',214,'Permissions updated for Warehouses. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(106,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:44:35','role_permissions'),(107,NULL,'UPDATE','Permissions',217,'Permissions updated for Accounting. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(108,NULL,'UPDATE','Permissions',221,'Permissions updated for Audit Logs. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(109,NULL,'UPDATE','Permissions',209,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(110,NULL,'UPDATE','Permissions',192,'Permissions updated for Brands. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(111,NULL,'UPDATE','Permissions',190,'Permissions updated for Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(112,NULL,'UPDATE','Permissions',212,'Permissions updated for Customer Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(113,NULL,'UPDATE','Permissions',211,'Permissions updated for Customers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(114,NULL,'UPDATE','Permissions',188,'Permissions updated for Dashboard. View:True->True, Add:False->True, Edit:False->True, Delete:False->True','2026-08-20 06:45:31','role_permissions'),(115,NULL,'UPDATE','Permissions',205,'Permissions updated for Goods Receipts. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(116,NULL,'UPDATE','Permissions',208,'Permissions updated for Inventory Audit. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(117,NULL,'UPDATE','Permissions',216,'Permissions updated for Notifications. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(118,NULL,'UPDATE','Permissions',194,'Permissions updated for Product Attributes. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(119,NULL,'UPDATE','Permissions',195,'Permissions updated for Product Variants. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(120,NULL,'UPDATE','Permissions',189,'Permissions updated for Products. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(121,NULL,'UPDATE','Permissions',223,'Permissions updated for Purchase Indent. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(122,NULL,'UPDATE','Permissions',224,'Permissions updated for Purchase Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(123,NULL,'UPDATE','Permissions',206,'Permissions updated for Purchases. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(124,NULL,'UPDATE','Permissions',215,'Permissions updated for Reports. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(125,NULL,'UPDATE','Permissions',218,'Permissions updated for Returns & Exchanges. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(126,NULL,'UPDATE','Permissions',220,'Permissions updated for Roles. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(127,NULL,'UPDATE','Permissions',207,'Permissions updated for Sales. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(128,NULL,'UPDATE','Permissions',225,'Permissions updated for Sales Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(129,NULL,'UPDATE','Permissions',196,'Permissions updated for Stock. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(130,NULL,'UPDATE','Permissions',200,'Permissions updated for Stock Adjustment Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(131,NULL,'UPDATE','Permissions',199,'Permissions updated for Stock Adjustments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(132,NULL,'UPDATE','Permissions',204,'Permissions updated for Stock Audit Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(133,NULL,'UPDATE','Permissions',203,'Permissions updated for Stock Audits. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(134,NULL,'UPDATE','Permissions',198,'Permissions updated for Stock Ledger. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(135,NULL,'UPDATE','Permissions',197,'Permissions updated for Stock Movements. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(136,NULL,'UPDATE','Permissions',202,'Permissions updated for Stock Transfer Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(137,NULL,'UPDATE','Permissions',201,'Permissions updated for Stock Transfers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(138,NULL,'UPDATE','Permissions',191,'Permissions updated for Sub Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(139,NULL,'UPDATE','Permissions',213,'Permissions updated for Supplier Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(140,NULL,'UPDATE','Permissions',210,'Permissions updated for Suppliers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(141,NULL,'UPDATE','Permissions',222,'Permissions updated for System Settings. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(142,NULL,'UPDATE','Permissions',193,'Permissions updated for Units. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(143,NULL,'UPDATE','Permissions',219,'Permissions updated for Users. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(144,NULL,'UPDATE','Permissions',214,'Permissions updated for Warehouses. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(145,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:45:31','role_permissions'),(146,NULL,'UPDATE','Permissions',217,'Permissions updated for Accounting. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(147,NULL,'UPDATE','Permissions',221,'Permissions updated for Audit Logs. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(148,NULL,'UPDATE','Permissions',209,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(149,NULL,'UPDATE','Permissions',192,'Permissions updated for Brands. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(150,NULL,'UPDATE','Permissions',190,'Permissions updated for Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(151,NULL,'UPDATE','Permissions',212,'Permissions updated for Customer Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(152,NULL,'UPDATE','Permissions',211,'Permissions updated for Customers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(153,NULL,'UPDATE','Permissions',188,'Permissions updated for Dashboard. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-20 06:47:34','role_permissions'),(154,NULL,'UPDATE','Permissions',205,'Permissions updated for Goods Receipts. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(155,NULL,'UPDATE','Permissions',208,'Permissions updated for Inventory Audit. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(156,NULL,'UPDATE','Permissions',216,'Permissions updated for Notifications. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(157,NULL,'UPDATE','Permissions',194,'Permissions updated for Product Attributes. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(158,NULL,'UPDATE','Permissions',195,'Permissions updated for Product Variants. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(159,NULL,'UPDATE','Permissions',189,'Permissions updated for Products. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(160,NULL,'UPDATE','Permissions',223,'Permissions updated for Purchase Indent. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(161,NULL,'UPDATE','Permissions',224,'Permissions updated for Purchase Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(162,NULL,'UPDATE','Permissions',206,'Permissions updated for Purchases. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(163,NULL,'UPDATE','Permissions',215,'Permissions updated for Reports. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(164,NULL,'UPDATE','Permissions',218,'Permissions updated for Returns & Exchanges. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(165,NULL,'UPDATE','Permissions',220,'Permissions updated for Roles. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(166,NULL,'UPDATE','Permissions',207,'Permissions updated for Sales. View:False->True, Add:False->False, Edit:False->False, Delete:False->True','2026-08-20 06:47:34','role_permissions'),(167,NULL,'UPDATE','Permissions',225,'Permissions updated for Sales Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(168,NULL,'UPDATE','Permissions',196,'Permissions updated for Stock. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(169,NULL,'UPDATE','Permissions',200,'Permissions updated for Stock Adjustment Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(170,NULL,'UPDATE','Permissions',199,'Permissions updated for Stock Adjustments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(171,NULL,'UPDATE','Permissions',204,'Permissions updated for Stock Audit Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(172,NULL,'UPDATE','Permissions',203,'Permissions updated for Stock Audits. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(173,NULL,'UPDATE','Permissions',198,'Permissions updated for Stock Ledger. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(174,NULL,'UPDATE','Permissions',197,'Permissions updated for Stock Movements. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(175,NULL,'UPDATE','Permissions',202,'Permissions updated for Stock Transfer Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(176,NULL,'UPDATE','Permissions',201,'Permissions updated for Stock Transfers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(177,NULL,'UPDATE','Permissions',191,'Permissions updated for Sub Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(178,NULL,'UPDATE','Permissions',213,'Permissions updated for Supplier Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(179,NULL,'UPDATE','Permissions',210,'Permissions updated for Suppliers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(180,NULL,'UPDATE','Permissions',222,'Permissions updated for System Settings. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(181,NULL,'UPDATE','Permissions',193,'Permissions updated for Units. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(182,NULL,'UPDATE','Permissions',219,'Permissions updated for Users. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(183,NULL,'UPDATE','Permissions',214,'Permissions updated for Warehouses. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(184,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:34','role_permissions'),(185,NULL,'UPDATE','Permissions',217,'Permissions updated for Accounting. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(186,NULL,'UPDATE','Permissions',221,'Permissions updated for Audit Logs. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(187,NULL,'UPDATE','Permissions',209,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(188,NULL,'UPDATE','Permissions',192,'Permissions updated for Brands. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(189,NULL,'UPDATE','Permissions',190,'Permissions updated for Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(190,NULL,'UPDATE','Permissions',212,'Permissions updated for Customer Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(191,NULL,'UPDATE','Permissions',211,'Permissions updated for Customers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(192,NULL,'UPDATE','Permissions',188,'Permissions updated for Dashboard. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-20 06:47:44','role_permissions'),(193,NULL,'UPDATE','Permissions',205,'Permissions updated for Goods Receipts. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(194,NULL,'UPDATE','Permissions',208,'Permissions updated for Inventory Audit. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(195,NULL,'UPDATE','Permissions',216,'Permissions updated for Notifications. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(196,NULL,'UPDATE','Permissions',194,'Permissions updated for Product Attributes. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(197,NULL,'UPDATE','Permissions',195,'Permissions updated for Product Variants. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(198,NULL,'UPDATE','Permissions',189,'Permissions updated for Products. View:False->True, Add:False->False, Edit:False->False, Delete:False->True','2026-08-20 06:47:44','role_permissions'),(199,NULL,'UPDATE','Permissions',223,'Permissions updated for Purchase Indent. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(200,NULL,'UPDATE','Permissions',224,'Permissions updated for Purchase Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(201,NULL,'UPDATE','Permissions',206,'Permissions updated for Purchases. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(202,NULL,'UPDATE','Permissions',215,'Permissions updated for Reports. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(203,NULL,'UPDATE','Permissions',218,'Permissions updated for Returns & Exchanges. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(204,NULL,'UPDATE','Permissions',220,'Permissions updated for Roles. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(205,NULL,'UPDATE','Permissions',207,'Permissions updated for Sales. View:True->True, Add:False->False, Edit:False->False, Delete:True->True','2026-08-20 06:47:44','role_permissions'),(206,NULL,'UPDATE','Permissions',225,'Permissions updated for Sales Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(207,NULL,'UPDATE','Permissions',196,'Permissions updated for Stock. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(208,NULL,'UPDATE','Permissions',200,'Permissions updated for Stock Adjustment Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(209,NULL,'UPDATE','Permissions',199,'Permissions updated for Stock Adjustments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(210,NULL,'UPDATE','Permissions',204,'Permissions updated for Stock Audit Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(211,NULL,'UPDATE','Permissions',203,'Permissions updated for Stock Audits. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(212,NULL,'UPDATE','Permissions',198,'Permissions updated for Stock Ledger. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(213,NULL,'UPDATE','Permissions',197,'Permissions updated for Stock Movements. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(214,NULL,'UPDATE','Permissions',202,'Permissions updated for Stock Transfer Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(215,NULL,'UPDATE','Permissions',201,'Permissions updated for Stock Transfers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(216,NULL,'UPDATE','Permissions',191,'Permissions updated for Sub Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(217,NULL,'UPDATE','Permissions',213,'Permissions updated for Supplier Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(218,NULL,'UPDATE','Permissions',210,'Permissions updated for Suppliers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(219,NULL,'UPDATE','Permissions',222,'Permissions updated for System Settings. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(220,NULL,'UPDATE','Permissions',193,'Permissions updated for Units. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(221,NULL,'UPDATE','Permissions',219,'Permissions updated for Users. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(222,NULL,'UPDATE','Permissions',214,'Permissions updated for Warehouses. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(223,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:47:44','role_permissions'),(224,NULL,'UPDATE','Permissions',217,'Permissions updated for Accounting. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(225,NULL,'UPDATE','Permissions',221,'Permissions updated for Audit Logs. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(226,NULL,'UPDATE','Permissions',209,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(227,NULL,'UPDATE','Permissions',192,'Permissions updated for Brands. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(228,NULL,'UPDATE','Permissions',190,'Permissions updated for Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(229,NULL,'UPDATE','Permissions',212,'Permissions updated for Customer Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(230,NULL,'UPDATE','Permissions',211,'Permissions updated for Customers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(231,NULL,'UPDATE','Permissions',188,'Permissions updated for Dashboard. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-20 06:48:16','role_permissions'),(232,NULL,'UPDATE','Permissions',205,'Permissions updated for Goods Receipts. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(233,NULL,'UPDATE','Permissions',208,'Permissions updated for Inventory Audit. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(234,NULL,'UPDATE','Permissions',216,'Permissions updated for Notifications. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(235,NULL,'UPDATE','Permissions',194,'Permissions updated for Product Attributes. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(236,NULL,'UPDATE','Permissions',195,'Permissions updated for Product Variants. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(237,NULL,'UPDATE','Permissions',189,'Permissions updated for Products. View:True->False, Add:False->False, Edit:False->False, Delete:True->False','2026-08-20 06:48:16','role_permissions'),(238,NULL,'UPDATE','Permissions',223,'Permissions updated for Purchase Indent. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(239,NULL,'UPDATE','Permissions',224,'Permissions updated for Purchase Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(240,NULL,'UPDATE','Permissions',206,'Permissions updated for Purchases. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(241,NULL,'UPDATE','Permissions',215,'Permissions updated for Reports. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(242,NULL,'UPDATE','Permissions',218,'Permissions updated for Returns & Exchanges. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(243,NULL,'UPDATE','Permissions',220,'Permissions updated for Roles. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(244,NULL,'UPDATE','Permissions',207,'Permissions updated for Sales. View:True->False, Add:False->False, Edit:False->False, Delete:True->False','2026-08-20 06:48:16','role_permissions'),(245,NULL,'UPDATE','Permissions',225,'Permissions updated for Sales Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(246,NULL,'UPDATE','Permissions',196,'Permissions updated for Stock. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(247,NULL,'UPDATE','Permissions',200,'Permissions updated for Stock Adjustment Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(248,NULL,'UPDATE','Permissions',199,'Permissions updated for Stock Adjustments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(249,NULL,'UPDATE','Permissions',204,'Permissions updated for Stock Audit Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(250,NULL,'UPDATE','Permissions',203,'Permissions updated for Stock Audits. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(251,NULL,'UPDATE','Permissions',198,'Permissions updated for Stock Ledger. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(252,NULL,'UPDATE','Permissions',197,'Permissions updated for Stock Movements. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(253,NULL,'UPDATE','Permissions',202,'Permissions updated for Stock Transfer Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(254,NULL,'UPDATE','Permissions',201,'Permissions updated for Stock Transfers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(255,NULL,'UPDATE','Permissions',191,'Permissions updated for Sub Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(256,NULL,'UPDATE','Permissions',213,'Permissions updated for Supplier Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(257,NULL,'UPDATE','Permissions',210,'Permissions updated for Suppliers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(258,NULL,'UPDATE','Permissions',222,'Permissions updated for System Settings. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(259,NULL,'UPDATE','Permissions',193,'Permissions updated for Units. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(260,NULL,'UPDATE','Permissions',219,'Permissions updated for Users. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(261,NULL,'UPDATE','Permissions',214,'Permissions updated for Warehouses. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(262,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-20 06:48:16','role_permissions'),(263,NULL,'ROLE_STATUS_CHANGE','Roles',3,'Role WH Manager status changed to Inactive','2026-08-25 05:15:46','roles'),(264,NULL,'ROLE_STATUS_CHANGE','Roles',3,'Role WH Manager status changed to Active','2026-08-25 05:15:49','roles'),(265,NULL,'ROLE_STATUS_CHANGE','Roles',3,'Role WH Manager status changed to Inactive','2026-08-25 05:15:50','roles'),(266,NULL,'ROLE_STATUS_CHANGE','Roles',3,'Role WH Manager status changed to Active','2026-08-25 05:15:52','roles'),(267,NULL,'ROLE_STATUS_CHANGE','Roles',5,'Role manager status changed to Inactive','2026-08-25 05:15:57','roles'),(268,NULL,'ROLE_STATUS_CHANGE','Roles',5,'Role manager status changed to Active','2026-08-25 05:51:41','roles'),(269,NULL,'ROLE_STATUS_CHANGE','Roles',5,'Role manager status changed to Inactive','2026-08-25 05:51:48','roles'),(270,NULL,'UPDATE','Permissions',5,'Permissions updated for Accounting. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(271,NULL,'UPDATE','Permissions',10,'Permissions updated for Audit Logs. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(272,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:57:53','role_permissions'),(273,NULL,'UPDATE','Permissions',20,'Permissions updated for Brands. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(274,NULL,'UPDATE','Permissions',25,'Permissions updated for Categories. View:True->True, Add:True->False, Edit:True->False, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(275,NULL,'UPDATE','Permissions',30,'Permissions updated for Customer Payments. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(276,NULL,'UPDATE','Permissions',35,'Permissions updated for Customers. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(277,NULL,'UPDATE','Permissions',40,'Permissions updated for Dashboard. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(278,NULL,'UPDATE','Permissions',45,'Permissions updated for Goods Receipts. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(279,NULL,'UPDATE','Permissions',50,'Permissions updated for Inventory Audit. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(280,NULL,'UPDATE','Permissions',55,'Permissions updated for Notifications. View:True->True, Add:True->False, Edit:True->False, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(281,NULL,'UPDATE','Permissions',60,'Permissions updated for Product Attributes. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(282,NULL,'UPDATE','Permissions',70,'Permissions updated for Product Variants. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(283,NULL,'UPDATE','Permissions',65,'Permissions updated for Products. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(284,NULL,'UPDATE','Permissions',176,'Permissions updated for Purchase Indent. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(285,NULL,'UPDATE','Permissions',177,'Permissions updated for Purchase Returns. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(286,NULL,'UPDATE','Permissions',75,'Permissions updated for Purchases. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(287,NULL,'UPDATE','Permissions',80,'Permissions updated for Reports. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(288,NULL,'UPDATE','Permissions',85,'Permissions updated for Returns & Exchanges. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(289,NULL,'UPDATE','Permissions',90,'Permissions updated for Roles. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(290,NULL,'UPDATE','Permissions',95,'Permissions updated for Sales. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(291,NULL,'UPDATE','Permissions',178,'Permissions updated for Sales Returns. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(292,NULL,'UPDATE','Permissions',100,'Permissions updated for Stock. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(293,NULL,'UPDATE','Permissions',105,'Permissions updated for Stock Adjustment Items. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:53','role_permissions'),(294,NULL,'UPDATE','Permissions',110,'Permissions updated for Stock Adjustments. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(295,NULL,'UPDATE','Permissions',115,'Permissions updated for Stock Audit Items. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(296,NULL,'UPDATE','Permissions',120,'Permissions updated for Stock Audits. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(297,NULL,'UPDATE','Permissions',125,'Permissions updated for Stock Ledger. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(298,NULL,'UPDATE','Permissions',130,'Permissions updated for Stock Movements. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(299,NULL,'UPDATE','Permissions',135,'Permissions updated for Stock Transfer Items. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(300,NULL,'UPDATE','Permissions',140,'Permissions updated for Stock Transfers. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(301,NULL,'UPDATE','Permissions',145,'Permissions updated for Sub Categories. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(302,NULL,'UPDATE','Permissions',150,'Permissions updated for Supplier Payments. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(303,NULL,'UPDATE','Permissions',155,'Permissions updated for Suppliers. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(304,NULL,'UPDATE','Permissions',160,'Permissions updated for System Settings. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(305,NULL,'UPDATE','Permissions',165,'Permissions updated for Units. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(306,NULL,'UPDATE','Permissions',170,'Permissions updated for Users. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(307,NULL,'UPDATE','Permissions',175,'Permissions updated for Warehouses. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:57:54','role_permissions'),(308,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:57:54','role_permissions'),(309,NULL,'UPDATE','Permissions',5,'Permissions updated for Accounting. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(310,NULL,'UPDATE','Permissions',10,'Permissions updated for Audit Logs. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(311,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:58:05','role_permissions'),(312,NULL,'UPDATE','Permissions',20,'Permissions updated for Brands. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(313,NULL,'UPDATE','Permissions',25,'Permissions updated for Categories. View:True->True, Add:False->True, Edit:False->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(314,NULL,'UPDATE','Permissions',30,'Permissions updated for Customer Payments. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(315,NULL,'UPDATE','Permissions',35,'Permissions updated for Customers. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(316,NULL,'UPDATE','Permissions',40,'Permissions updated for Dashboard. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(317,NULL,'UPDATE','Permissions',45,'Permissions updated for Goods Receipts. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(318,NULL,'UPDATE','Permissions',50,'Permissions updated for Inventory Audit. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(319,NULL,'UPDATE','Permissions',55,'Permissions updated for Notifications. View:True->True, Add:False->True, Edit:False->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(320,NULL,'UPDATE','Permissions',60,'Permissions updated for Product Attributes. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:05','role_permissions'),(321,NULL,'UPDATE','Permissions',70,'Permissions updated for Product Variants. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(322,NULL,'UPDATE','Permissions',65,'Permissions updated for Products. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(323,NULL,'UPDATE','Permissions',176,'Permissions updated for Purchase Indent. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(324,NULL,'UPDATE','Permissions',177,'Permissions updated for Purchase Returns. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(325,NULL,'UPDATE','Permissions',75,'Permissions updated for Purchases. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(326,NULL,'UPDATE','Permissions',80,'Permissions updated for Reports. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(327,NULL,'UPDATE','Permissions',85,'Permissions updated for Returns & Exchanges. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(328,NULL,'UPDATE','Permissions',90,'Permissions updated for Roles. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(329,NULL,'UPDATE','Permissions',95,'Permissions updated for Sales. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(330,NULL,'UPDATE','Permissions',178,'Permissions updated for Sales Returns. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(331,NULL,'UPDATE','Permissions',100,'Permissions updated for Stock. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(332,NULL,'UPDATE','Permissions',105,'Permissions updated for Stock Adjustment Items. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(333,NULL,'UPDATE','Permissions',110,'Permissions updated for Stock Adjustments. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(334,NULL,'UPDATE','Permissions',115,'Permissions updated for Stock Audit Items. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(335,NULL,'UPDATE','Permissions',120,'Permissions updated for Stock Audits. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(336,NULL,'UPDATE','Permissions',125,'Permissions updated for Stock Ledger. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(337,NULL,'UPDATE','Permissions',130,'Permissions updated for Stock Movements. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(338,NULL,'UPDATE','Permissions',135,'Permissions updated for Stock Transfer Items. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:06','role_permissions'),(339,NULL,'UPDATE','Permissions',140,'Permissions updated for Stock Transfers. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(340,NULL,'UPDATE','Permissions',145,'Permissions updated for Sub Categories. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(341,NULL,'UPDATE','Permissions',150,'Permissions updated for Supplier Payments. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(342,NULL,'UPDATE','Permissions',155,'Permissions updated for Suppliers. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(343,NULL,'UPDATE','Permissions',160,'Permissions updated for System Settings. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(344,NULL,'UPDATE','Permissions',165,'Permissions updated for Units. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(345,NULL,'UPDATE','Permissions',170,'Permissions updated for Users. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(346,NULL,'UPDATE','Permissions',175,'Permissions updated for Warehouses. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:58:07','role_permissions'),(347,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:58:07','role_permissions'),(348,NULL,'ROLE_STATUS_CHANGE','Roles',5,'Role manager status changed to Active','2026-08-25 05:58:27','roles'),(349,NULL,'UPDATE','Permissions',3,'Permissions updated for Accounting. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(350,NULL,'UPDATE','Permissions',8,'Permissions updated for Audit Logs. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(351,NULL,'UPDATE','Permissions',13,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(352,NULL,'UPDATE','Permissions',18,'Permissions updated for Brands. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(353,NULL,'UPDATE','Permissions',23,'Permissions updated for Categories. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(354,NULL,'UPDATE','Permissions',28,'Permissions updated for Customer Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(355,NULL,'UPDATE','Permissions',33,'Permissions updated for Customers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(356,NULL,'UPDATE','Permissions',38,'Permissions updated for Dashboard. View:True->True, Add:True->True, Edit:True->True, Delete:True->True','2026-08-25 05:59:04','role_permissions'),(357,NULL,'UPDATE','Permissions',43,'Permissions updated for Goods Receipts. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(358,NULL,'UPDATE','Permissions',48,'Permissions updated for Inventory Audit. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(359,NULL,'UPDATE','Permissions',53,'Permissions updated for Notifications. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(360,NULL,'UPDATE','Permissions',58,'Permissions updated for Product Attributes. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(361,NULL,'UPDATE','Permissions',68,'Permissions updated for Product Variants. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(362,NULL,'UPDATE','Permissions',63,'Permissions updated for Products. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(363,NULL,'UPDATE','Permissions',182,'Permissions updated for Purchase Indent. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(364,NULL,'UPDATE','Permissions',183,'Permissions updated for Purchase Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(365,NULL,'UPDATE','Permissions',73,'Permissions updated for Purchases. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:04','role_permissions'),(366,NULL,'UPDATE','Permissions',78,'Permissions updated for Reports. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(367,NULL,'UPDATE','Permissions',83,'Permissions updated for Returns & Exchanges. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(368,NULL,'UPDATE','Permissions',88,'Permissions updated for Roles. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(369,NULL,'UPDATE','Permissions',93,'Permissions updated for Sales. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(370,NULL,'UPDATE','Permissions',184,'Permissions updated for Sales Returns. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(371,NULL,'UPDATE','Permissions',98,'Permissions updated for Stock. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(372,NULL,'UPDATE','Permissions',103,'Permissions updated for Stock Adjustment Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(373,NULL,'UPDATE','Permissions',108,'Permissions updated for Stock Adjustments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(374,NULL,'UPDATE','Permissions',113,'Permissions updated for Stock Audit Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(375,NULL,'UPDATE','Permissions',118,'Permissions updated for Stock Audits. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(376,NULL,'UPDATE','Permissions',123,'Permissions updated for Stock Ledger. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(377,NULL,'UPDATE','Permissions',128,'Permissions updated for Stock Movements. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(378,NULL,'UPDATE','Permissions',133,'Permissions updated for Stock Transfer Items. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(379,NULL,'UPDATE','Permissions',138,'Permissions updated for Stock Transfers. View:False->True, Add:False->False, Edit:False->False, Delete:False->True','2026-08-25 05:59:05','role_permissions'),(380,NULL,'UPDATE','Permissions',143,'Permissions updated for Sub Categories. View:False->True, Add:False->False, Edit:False->False, Delete:False->True','2026-08-25 05:59:05','role_permissions'),(381,NULL,'UPDATE','Permissions',148,'Permissions updated for Supplier Payments. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(382,NULL,'UPDATE','Permissions',153,'Permissions updated for Suppliers. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(383,NULL,'UPDATE','Permissions',158,'Permissions updated for System Settings. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(384,NULL,'UPDATE','Permissions',163,'Permissions updated for Units. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(385,NULL,'UPDATE','Permissions',168,'Permissions updated for Users. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(386,NULL,'UPDATE','Permissions',173,'Permissions updated for Warehouses. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(387,NULL,'UPDATE','Permissions',15,'Permissions updated for Barcode / QR. View:False->False, Add:False->False, Edit:False->False, Delete:False->False','2026-08-25 05:59:05','role_permissions'),(388,NULL,'ROLE_STATUS_CHANGE','Roles',7,'Role Assistant Manager status changed to Inactive','2026-08-25 06:04:07','roles'),(389,116,'UPDATE_CATEGORY','Categories',1,'Lasya updated Farm and Garden','2026-08-25 23:03:21','categories'),(390,NULL,'ROLE_STATUS_CHANGE','Roles',7,'Role Assistant Manager status changed to Active','2026-08-25 23:26:12','roles'),(391,NULL,'ROLE_STATUS_CHANGE','Roles',7,'Role Assistant Manager status changed to Inactive','2026-08-25 23:26:15','roles'),(392,116,'CREATE_CATEGORY','Categories',4,'Lasya created Pipes And Grids','2026-08-26 03:07:21','categories'),(393,NULL,'ROLE_STATUS_CHANGE','Roles',7,'Role Assistant Manager status changed to Active','2026-08-26 06:20:58','roles'),(394,NULL,'ROLE_STATUS_CHANGE','Roles',7,'Role Assistant Manager status changed to Inactive','2026-08-26 06:21:03','roles'),(395,NULL,'ROLE_STATUS_CHANGE','Roles',7,'Role Assistant Manager status changed to Active','2026-08-26 06:21:07','roles'),(396,NULL,'ROLE_STATUS_CHANGE','Roles',7,'Role Assistant Manager status changed to Inactive','2026-08-26 06:21:18','roles'),(397,116,'UPDATE_SUBCATEGORY','Sub Categories',3,'Lasya updated Domestic Pumps','2026-08-27 04:06:05','sub_categories'),(398,116,'UPDATE_SUBCATEGORY','Sub Categories',2,'Lasya updated Special Farm Tools','2026-08-27 04:06:26','sub_categories'),(399,116,'UPDATE_SUBCATEGORY','Sub Categories',2,'Lasya updated Special Farm Tools','2026-08-27 04:07:14','sub_categories'),(400,116,'UPDATE_SUBCATEGORY','Sub Categories',2,'Lasya updated Special Farm Tools','2026-08-27 04:07:29','sub_categories'),(401,116,'CREATE_SUBCATEGORY','Sub Categories',4,'Lasya created Tiles And Marbles','2026-08-27 04:26:29','sub_categories'),(402,116,'UPDATE_SUBCATEGORY','Sub Categories',4,'Lasya updated Tiles And Marbles','2026-08-27 04:26:56','sub_categories'),(403,116,'UPDATE_SUBCATEGORY','Sub Categories',4,'Lasya updated Tiles And Marbles','2026-08-27 04:27:05','sub_categories'),(404,118,'UPDATE_PRODUCT','Products',3,'sukanya Product updated: FESTEL Telescopic Pole with Fruit Picking Basket, 7 to 24 feet Extendable Pole','2026-08-27 06:23:56','products'),(405,118,'UPDATE_PRODUCT','Products',5,'sukanya Product updated: Premium Quality 0.5 HP Single Phase Mini Openwell Pump 72 feet max head with Control Panel','2026-08-27 06:27:54','products');
/*!40000 ALTER TABLE `audit_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `barcodes`
--

DROP TABLE IF EXISTS `barcodes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `barcodes` (
  `barcode_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `code_value` varchar(255) DEFAULT NULL,
  `code_type` varchar(50) DEFAULT NULL,
  `image_url` text,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`barcode_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `barcodes`
--

LOCK TABLES `barcodes` WRITE;
/*!40000 ALTER TABLE `barcodes` DISABLE KEYS */;
/*!40000 ALTER TABLE `barcodes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bin_stock`
--

DROP TABLE IF EXISTS `bin_stock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bin_stock` (
  `bin_stock_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `bin_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT '0.00',
  PRIMARY KEY (`bin_stock_id`),
  UNIQUE KEY `product_id` (`product_id`,`variant_id`,`bin_id`),
  KEY `variant_id` (`variant_id`),
  KEY `warehouse_id` (`warehouse_id`),
  KEY `bin_id` (`bin_id`),
  CONSTRAINT `bin_stock_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `bin_stock_ibfk_2` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`),
  CONSTRAINT `bin_stock_ibfk_3` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`),
  CONSTRAINT `bin_stock_ibfk_4` FOREIGN KEY (`bin_id`) REFERENCES `bins` (`bin_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bin_stock`
--

LOCK TABLES `bin_stock` WRITE;
/*!40000 ALTER TABLE `bin_stock` DISABLE KEYS */;
INSERT INTO `bin_stock` VALUES (1,3,NULL,2,1,99.00);
/*!40000 ALTER TABLE `bin_stock` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bin_transfer_audits`
--

DROP TABLE IF EXISTS `bin_transfer_audits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bin_transfer_audits` (
  `bin_transfer_audit_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `warehouse_id` int NOT NULL,
  `from_bin_id` int NOT NULL,
  `to_bin_id` int NOT NULL,
  `quantity` decimal(18,2) NOT NULL,
  `user_id` int DEFAULT NULL,
  `user_name` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  PRIMARY KEY (`bin_transfer_audit_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bin_transfer_audits`
--

LOCK TABLES `bin_transfer_audits` WRITE;
/*!40000 ALTER TABLE `bin_transfer_audits` DISABLE KEYS */;
/*!40000 ALTER TABLE `bin_transfer_audits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bins`
--

DROP TABLE IF EXISTS `bins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bins` (
  `bin_id` int NOT NULL AUTO_INCREMENT,
  `warehouse_id` int DEFAULT NULL,
  `rack_id` int DEFAULT NULL,
  `bin_code` varchar(50) DEFAULT NULL,
  `capacity` decimal(10,2) DEFAULT NULL,
  `status` enum('active','inactive') DEFAULT 'active',
  PRIMARY KEY (`bin_id`),
  KEY `warehouse_id` (`warehouse_id`),
  KEY `rack_id` (`rack_id`),
  CONSTRAINT `bins_ibfk_1` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`),
  CONSTRAINT `bins_ibfk_2` FOREIGN KEY (`rack_id`) REFERENCES `racks` (`rack_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bins`
--

LOCK TABLES `bins` WRITE;
/*!40000 ALTER TABLE `bins` DISABLE KEYS */;
INSERT INTO `bins` VALUES (1,2,1,'BIN-A1',100.00,'active'),(2,2,1,'BIN-A2',100.00,'active');
/*!40000 ALTER TABLE `bins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `brands`
--

DROP TABLE IF EXISTS `brands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `brands` (
  `brand_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(150) NOT NULL,
  `description` text,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`brand_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `brands`
--

LOCK TABLES `brands` WRITE;
/*!40000 ALTER TABLE `brands` DISABLE KEYS */;
INSERT INTO `brands` VALUES (1,'Generic','',0),(2,'Sunya','',0),(3,'FESTEL','',0);
/*!40000 ALTER TABLE `brands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categories` (
  `category_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(150) NOT NULL,
  `parent_id` int DEFAULT NULL,
  `description` text,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`category_id`),
  KEY `parent_id` (`parent_id`),
  CONSTRAINT `categories_ibfk_1` FOREIGN KEY (`parent_id`) REFERENCES `categories` (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
INSERT INTO `categories` VALUES (1,'Farm and Garden',NULL,'shbshskjffsjfs',0),(2,'Pumps and Motors',NULL,'',0),(3,'MOBILES',NULL,'',0),(4,'Pipes And Grids',2,'Used for agriculture purpose',0);
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_activity`
--

DROP TABLE IF EXISTS `customer_activity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_activity` (
  `activity_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int NOT NULL,
  `activity_type` varchar(50) DEFAULT NULL,
  `description` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`activity_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_activity_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=184 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_activity`
--

LOCK TABLES `customer_activity` WRITE;
/*!40000 ALTER TABLE `customer_activity` DISABLE KEYS */;
INSERT INTO `customer_activity` VALUES (11,8,'CREATE','Customer Rohith Sharma created','2026-05-20 12:44:27'),(12,8,'NOTE','Regular customer','2026-05-20 12:44:27'),(13,8,'INVOICE','Invoice INV-20260520-001 created','2026-05-20 12:48:51'),(14,8,'UPDATE','Customer Rohith Sharma updated','2026-05-20 13:01:50'),(15,8,'NOTE','Regular customer','2026-05-20 13:01:50'),(16,8,'INVOICE','Invoice INV-20260521-001 created','2026-05-20 23:22:20'),(17,8,'UPDATE','Customer Rohith Sharma updated','2026-05-26 07:27:15'),(18,8,'NOTE','Regular customer','2026-05-26 07:27:15'),(19,9,'CREATE','Customer Hardik pandya created','2026-05-26 10:13:31'),(20,10,'CREATE','Customer Jasprith Bumrah created','2026-05-26 10:15:05'),(21,11,'CREATE','Customer Tilak verma created','2026-05-26 10:16:08'),(22,12,'CREATE','Customer Naman dhir created','2026-05-26 10:17:12'),(23,13,'CREATE','Customer Quinton de kock created','2026-05-26 10:19:31'),(24,14,'CREATE','Customer Ryan reckolton created','2026-05-26 10:21:56'),(25,8,'UPDATE','Customer Rohith Sharma updated','2026-05-27 23:58:43'),(26,8,'NOTE','Regular customer','2026-05-27 23:58:43'),(27,15,'CREATE','Customer Deepaka Chahar created','2026-05-28 00:01:20'),(28,16,'CREATE','Customer Shardul Thakur created','2026-05-28 03:03:20'),(29,15,'STATUS','Customer status changed from Active to Inactive','2026-05-28 04:33:40'),(30,15,'STATUS','Customer status changed from Inactive to Active','2026-05-28 04:33:53'),(32,15,'STATUS','Customer status changed from Active to Inactive','2026-05-28 05:08:55'),(34,9,'STATUS','Customer status changed from Active to Inactive','2026-05-28 05:11:41'),(35,9,'STATUS','Customer status changed from Inactive to Active','2026-05-28 05:17:16'),(36,15,'STATUS','Customer status changed from Inactive to Active','2026-05-28 22:44:57'),(37,15,'STATUS','Customer status changed from Active to Inactive','2026-05-28 22:45:10'),(38,16,'STATUS','Customer status changed from Active to Inactive','2026-05-28 23:00:18'),(39,9,'STATUS','Customer status changed from Active to Inactive','2026-05-28 23:16:24'),(40,10,'STATUS','Customer status changed from Active to Inactive','2026-06-02 07:55:34'),(41,15,'STATUS','Customer status changed from Inactive to Active','2026-06-02 10:27:36'),(42,9,'STATUS','Customer status changed from Inactive to Active','2026-06-02 12:20:46'),(43,12,'STATUS','Customer status changed from Active to Inactive','2026-06-02 12:21:30'),(44,10,'STATUS','Customer status changed from Inactive to Active','2026-06-02 12:38:13'),(45,12,'STATUS','Customer status changed from Inactive to Active','2026-06-02 12:38:24'),(46,16,'STATUS','Customer status changed from Inactive to Active','2026-06-02 12:38:32'),(47,15,'STATUS','Customer status changed from Active to Inactive','2026-06-02 13:10:32'),(48,17,'CREATE','Customer Ramesh Kumar created','2026-06-03 00:18:49'),(49,17,'INVOICE','Invoice INV-20260603-001 created','2026-06-03 00:20:14'),(50,9,'INVOICE','Invoice INV-20260603-002 created','2026-06-03 03:03:06'),(51,11,'INVOICE','Invoice INV-20260605-001 created','2026-06-04 21:27:07'),(64,30,'CREATE','Customer QA Atomic Traders created','2026-06-05 04:03:18'),(65,31,'CREATE','Customer QA Atomic Traders created','2026-06-05 04:03:42'),(75,41,'CREATE','Customer MSD supply& trades created','2026-06-05 04:38:40'),(76,42,'CREATE','Customer virat logistics created','2026-06-05 04:40:14'),(97,15,'UPDATE','Customer Deepaka Chahar updated','2026-06-05 05:44:04'),(98,9,'INVOICE','Invoice INV-20260605-002 created','2026-06-05 09:28:49'),(99,14,'INVOICE','Invoice INV-20260605-003 created','2026-06-05 09:44:12'),(100,41,'INVOICE','Invoice INV-20260605-004 created','2026-06-05 09:55:26'),(101,12,'PAYMENT','Opening payment of 50.00 recorded for invoice INV-20260605-005','2026-06-05 10:16:08'),(102,12,'INVOICE','Invoice INV-20260605-005 created','2026-06-05 10:16:08'),(103,11,'PAYMENT','Opening payment of 100000.00 recorded for invoice INV-20260608-001','2026-06-08 00:57:46'),(104,11,'INVOICE','Invoice INV-20260608-001 created','2026-06-08 00:57:46'),(105,9,'PAYMENT','Opening payment of 50000.00 recorded for invoice INV-20260609-001','2026-06-09 07:02:37'),(106,9,'INVOICE','Invoice INV-20260609-001 created','2026-06-09 07:02:37'),(107,12,'PAYMENT','Opening payment of 100.00 recorded for invoice INV-20260611-001','2026-06-11 07:50:53'),(108,12,'INVOICE','Invoice INV-20260611-001 created','2026-06-11 07:50:53'),(109,10,'PAYMENT','Opening payment of 150.00 recorded for invoice INV-20260611-002','2026-06-11 07:56:45'),(110,10,'INVOICE','Invoice INV-20260611-002 created','2026-06-11 07:56:45'),(111,16,'PAYMENT','Opening payment of 80.00 recorded for invoice INV-20260611-003','2026-06-11 10:36:20'),(112,16,'INVOICE','Invoice INV-20260611-003 created','2026-06-11 10:36:20'),(113,14,'PAYMENT','Opening payment of 50000.00 recorded for invoice INV-20260612-001','2026-06-11 20:40:48'),(114,14,'INVOICE','Invoice INV-20260612-001 created','2026-06-11 20:40:48'),(115,42,'PAYMENT','Opening payment of 50000.00 recorded for invoice INV-20260616-001','2026-06-15 23:10:42'),(116,42,'INVOICE','Invoice INV-20260616-001 created','2026-06-15 23:10:42'),(117,9,'PAYMENT','Opening payment of 10000.00 recorded for invoice INV-20260618-001','2026-06-17 22:55:46'),(118,9,'INVOICE','Invoice INV-20260618-001 created','2026-06-17 22:55:46'),(119,15,'PAYMENT','Opening payment of 200000.00 recorded for invoice INV-20260622-001','2026-06-22 02:10:15'),(120,15,'INVOICE','Invoice INV-20260622-001 created','2026-06-22 02:10:15'),(121,1,'CREATE','Customer Rohith sharma created','2026-06-25 05:38:20'),(122,1,'PAYMENT','Opening payment of 1350.00 recorded for invoice INV-20260625-001','2026-06-25 06:24:41'),(123,1,'INVOICE','Invoice INV-20260625-001 created','2026-06-25 06:24:41'),(124,2,'CREATE','Customer ravi kiran created','2026-06-26 05:26:26'),(125,2,'PAYMENT','Opening payment of 1000.00 recorded for invoice INV-20260626-001','2026-06-26 05:35:22'),(126,2,'INVOICE','Invoice INV-20260626-001 created','2026-06-26 05:35:22'),(127,2,'PAYMENT','Opening payment of 1000.00 recorded for invoice INV-20260702-001','2026-07-01 22:43:34'),(128,2,'INVOICE','Invoice INV-20260702-001 created','2026-07-01 22:43:34'),(129,1,'INVOICE','Invoice INV-20260702-002 created','2026-07-01 23:54:17'),(130,3,'CREATE','Customer nandhu created','2026-07-01 23:56:25'),(131,3,'INVOICE','Invoice INV-20260702-003 created','2026-07-01 23:56:50'),(132,3,'PAYMENT','Opening payment of 500.00 recorded for invoice INV-20260702-004','2026-07-02 00:11:48'),(133,3,'INVOICE','Invoice INV-20260702-004 created','2026-07-02 00:11:48'),(134,3,'INVOICE','Invoice INV-20260703-001 created','2026-07-02 23:47:32'),(135,3,'INVOICE','Invoice INV-20260703-002 created','2026-07-03 00:04:23'),(136,2,'INVOICE','Invoice INV-20260706-001 created','2026-07-06 03:36:20'),(137,3,'INVOICE','Invoice INV-20260706-002 created','2026-07-06 03:55:45'),(138,3,'INVOICE','Invoice INV-20260706-003 created','2026-07-06 04:09:50'),(139,1,'INVOICE','Invoice INV-20260706-004 created','2026-07-06 04:11:00'),(140,1,'INVOICE','Invoice INV-20260706-005 created','2026-07-06 04:13:35'),(141,3,'INVOICE','Invoice INV-20260709-001 created','2026-07-09 00:02:56'),(142,2,'INVOICE','Invoice INV-20260714-001 created','2026-07-14 05:53:15'),(143,1,'INVOICE','Invoice INV-20260714-002 created','2026-07-14 06:10:10'),(144,4,'CREATE','Customer Shiva kumar G created','2026-07-15 05:24:16'),(145,4,'INVOICE','Invoice INV-20260715-001 created','2026-07-15 05:24:59'),(146,4,'INVOICE','Invoice INV-20260715-002 created','2026-07-15 05:43:53'),(147,4,'INVOICE','Invoice INV-20260715-003 created','2026-07-15 06:33:25'),(148,5,'CREATE','Customer nandhitha sri created','2026-07-21 04:08:59'),(149,5,'INVOICE','Invoice INV-20260721-001 created','2026-07-21 04:24:34'),(150,5,'INVOICE','Invoice INV-20260721-002 created','2026-07-21 04:55:29'),(151,6,'CREATE','Customer suresh created','2026-07-22 01:36:36'),(152,5,'INVOICE','Invoice INV-20260722-001 created','2026-07-22 03:22:29'),(153,1,'CREATE','Customer suresh created','2026-07-31 05:07:39'),(154,1,'CREATE','Customer ravikiran created','2026-08-04 01:22:25'),(155,1,'INVOICE','Invoice INV-20260804-001 created','2026-08-04 01:24:03'),(156,1,'INVOICE','Invoice INV-20260804-002 created','2026-08-04 01:33:42'),(157,1,'INVOICE','Invoice INV-20260804-003 created','2026-08-04 03:38:03'),(158,1,'CREATE','Customer ravikiran created','2026-08-05 02:30:36'),(159,2,'CREATE','Customer rajesh created','2026-08-05 02:31:02'),(160,2,'INVOICE','Invoice INV-20260806-001 created','2026-08-06 00:53:11'),(161,1,'PAYMENT','Opening payment of 1000.00 recorded for invoice INV-20260806-002','2026-08-06 00:56:39'),(162,1,'INVOICE','Invoice INV-20260806-002 created','2026-08-06 00:56:39'),(163,2,'INVOICE','Invoice INV-20260806-003 created','2026-08-06 01:17:45'),(164,1,'PAYMENT','Opening payment of 3000.00 recorded for invoice INV-20260806-004','2026-08-06 01:35:33'),(165,1,'INVOICE','Invoice INV-20260806-004 created','2026-08-06 01:35:33'),(166,3,'CREATE','Customer Sridhar created','2026-08-07 04:51:35'),(167,1,'INVOICE','Invoice INV-20260808-001 created','2026-08-08 03:09:06'),(168,2,'UPDATE','Customer RRajesh updated','2026-08-09 07:53:47'),(169,4,'CREATE','Customer nandhitha created','2026-08-10 01:10:10'),(170,4,'INVOICE','Invoice INV-20260812-001 created','2026-08-12 04:39:26'),(171,4,'INVOICE','Invoice INV-20260812-002 created','2026-08-12 05:06:41'),(172,2,'INVOICE','Invoice INV-20260812-003 created','2026-08-12 05:09:19'),(173,4,'INVOICE','Invoice INV-20260813-001 created','2026-08-13 02:02:57'),(174,4,'INVOICE','Invoice INV-20260813-002 created','2026-08-13 02:27:24'),(175,1,'CREATE','Customer Ravi Kumar created','2026-08-13 06:53:31'),(176,2,'CREATE','Customer Suresh reddy created','2026-08-13 06:55:02'),(177,3,'CREATE','Customer Nandhitha Sri created','2026-08-13 06:55:23'),(178,4,'CREATE','Customer Priya Devi created','2026-08-13 06:56:31'),(179,4,'INVOICE','Invoice INV-20260814-001 created','2026-08-13 22:20:00'),(180,4,'STATUS','Customer status changed from Active to Inactive','2026-08-13 23:15:48'),(181,1,'PAYMENT','Opening payment of 50000.00 recorded for invoice INV-20260817-001','2026-08-17 05:45:27'),(182,1,'INVOICE','Invoice INV-20260817-001 created','2026-08-17 05:45:27'),(183,3,'INVOICE','Invoice INV-20260819-001 created','2026-08-19 00:30:56');
/*!40000 ALTER TABLE `customer_activity` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_addresses`
--

DROP TABLE IF EXISTS `customer_addresses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_addresses` (
  `address_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int NOT NULL,
  `address_type` enum('billing','shipping') DEFAULT 'billing',
  `address_line` text,
  `city` varchar(100) DEFAULT NULL,
  `state` varchar(100) DEFAULT NULL,
  `country` varchar(100) DEFAULT NULL,
  `pincode` varchar(20) DEFAULT NULL,
  `address_line2` varchar(255) DEFAULT NULL,
  `is_primary` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`address_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_addresses_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_addresses`
--

LOCK TABLES `customer_addresses` WRITE;
/*!40000 ALTER TABLE `customer_addresses` DISABLE KEYS */;
INSERT INTO `customer_addresses` VALUES (13,15,'billing','2nd Street , Mumbai','Mumbai','Maharashtra','India','','',1),(14,1,'billing','12-45, Main Road, Guntur, Andhra Pradesh','Guntur','Andhra Pradesh','India','','',1),(15,2,'billing','8-21 ,market road, vijayawada, Andhra Pradesh','Vijayawada','Andhra Pradesh','India','','',1),(16,3,'billing','Ratnalakunta, Eluru','Eluru','Andhra Pradesh','India','','',1),(17,4,'billing','15-8, railway road, Nellore','Nellore','Andhra Pradesh','India','','',1);
/*!40000 ALTER TABLE `customer_addresses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_bank_details`
--

DROP TABLE IF EXISTS `customer_bank_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_bank_details` (
  `bank_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int NOT NULL,
  `account_name` varchar(150) DEFAULT NULL,
  `account_number` varchar(50) DEFAULT NULL,
  `bank_name` varchar(150) DEFAULT NULL,
  `ifsc_code` varchar(20) DEFAULT NULL,
  `branch` varchar(100) DEFAULT NULL,
  `is_primary` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`bank_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_bank_details_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_bank_details`
--

LOCK TABLES `customer_bank_details` WRITE;
/*!40000 ALTER TABLE `customer_bank_details` DISABLE KEYS */;
INSERT INTO `customer_bank_details` VALUES (13,15,'Deepak Chahar','78765434549','State Bank Of India','SBIN0003028','ITI AIE MAHADEVAPUR',1),(15,2,'Prasad','20378218328','SBI','SBIN0000981','PATTIKONDA',1);
/*!40000 ALTER TABLE `customer_bank_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_contacts`
--

DROP TABLE IF EXISTS `customer_contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_contacts` (
  `contact_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int NOT NULL,
  `name` varchar(150) DEFAULT NULL,
  `designation` varchar(100) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT '0',
  `role` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`contact_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_contacts_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_contacts`
--

LOCK TABLES `customer_contacts` WRITE;
/*!40000 ALTER TABLE `customer_contacts` DISABLE KEYS */;
INSERT INTO `customer_contacts` VALUES (13,15,'Deepaka Chahar','','7898235543','chahar@gmail.com',1,'Sales'),(15,2,'ravi kiran','','6798543876','ravikiran@gmail.com',1,'Sales'),(16,1,'Ravi Kumar','','9876543210','ravikumar@gmail.com',1,'Sales'),(17,2,'Suresh reddy','','9123456780','sureshreddy@gmail.com',1,'Sales'),(18,3,'Nandhitha Sri','','9491755559','nadhithachebattina@gmail.com',1,'Sales');
/*!40000 ALTER TABLE `customer_contacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_ledger`
--

DROP TABLE IF EXISTS `customer_ledger`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_ledger` (
  `ledger_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int NOT NULL,
  `transaction_type` enum('invoice','payment','return','return_credit_note','return_refund','payment_edit','payment_void','invoice_reversal','invoice_cancel') DEFAULT NULL,
  `transaction_id` int DEFAULT NULL,
  `debit` decimal(12,2) DEFAULT '0.00',
  `credit` decimal(12,2) DEFAULT '0.00',
  `balance` decimal(12,2) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ledger_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_ledger_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=150 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_ledger`
--

LOCK TABLES `customer_ledger` WRITE;
/*!40000 ALTER TABLE `customer_ledger` DISABLE KEYS */;
INSERT INTO `customer_ledger` VALUES (5,8,'invoice',14,149000.00,0.00,149000.00,'2026-05-20 12:48:51'),(8,8,'payment',9,0.00,50000.00,99000.00,'2026-05-20 21:50:38'),(9,8,'payment',10,0.00,99000.00,0.00,'2026-05-20 22:03:00'),(10,8,'invoice',15,74500.00,0.00,74500.00,'2026-05-20 23:22:20'),(11,17,'invoice',16,95000.00,95000.00,0.00,'2026-06-03 00:20:14'),(12,9,'invoice',17,285000.00,185000.00,100000.00,'2026-06-03 03:03:06'),(13,9,'payment',11,0.00,50000.00,50000.00,'2026-06-03 03:15:11'),(14,8,'payment',12,0.00,50000.00,24500.00,'2026-06-03 04:14:39'),(15,11,'invoice',18,600000.00,300000.00,300000.00,'2026-06-04 21:27:07'),(16,11,'payment',13,0.00,300000.00,0.00,'2026-06-04 21:32:48'),(17,9,'payment',14,0.00,50000.00,0.00,'2026-06-05 08:23:44'),(18,8,'payment',15,0.00,24500.00,0.00,'2026-06-05 08:26:04'),(19,9,'invoice',19,20.00,20.00,0.00,'2026-06-05 09:28:49'),(20,14,'invoice',20,375.00,100.00,275.00,'2026-06-05 09:44:12'),(21,14,'payment',16,0.00,100.00,175.00,'2026-06-05 09:46:08'),(22,14,'payment',17,0.00,175.00,0.00,'2026-06-05 09:48:35'),(23,41,'invoice',21,110000.00,50000.00,60000.00,'2026-06-05 09:55:26'),(24,12,'invoice',22,200.00,0.00,200.00,'2026-06-05 10:16:08'),(25,12,'payment',18,0.00,50.00,150.00,'2026-06-05 10:16:08'),(26,11,'invoice',23,345000.00,0.00,345000.00,'2026-06-08 00:57:46'),(27,11,'payment',19,0.00,100000.00,245000.00,'2026-06-08 00:57:46'),(28,11,'payment',20,0.00,50000.00,195000.00,'2026-06-08 01:00:52'),(29,11,'payment',21,0.00,95000.00,100000.00,'2026-06-08 01:02:01'),(30,11,'payment',22,0.00,100000.00,0.00,'2026-06-08 01:04:44'),(31,9,'invoice',24,540000.00,0.00,540000.00,'2026-06-09 07:02:37'),(32,9,'payment',23,0.00,50000.00,490000.00,'2026-06-09 07:02:37'),(33,9,'payment',24,0.00,100000.00,390000.00,'2026-06-09 07:05:40'),(34,9,'payment',25,0.00,390000.00,0.00,'2026-06-09 07:06:38'),(35,12,'invoice',25,375.00,0.00,525.00,'2026-06-11 07:50:53'),(36,12,'payment',26,0.00,100.00,425.00,'2026-06-11 07:50:53'),(37,10,'invoice',26,375.00,0.00,375.00,'2026-06-11 07:56:45'),(38,10,'payment',27,0.00,150.00,225.00,'2026-06-11 07:56:45'),(39,16,'invoice',27,200.00,0.00,200.00,'2026-06-11 10:36:20'),(40,16,'payment',28,0.00,80.00,120.00,'2026-06-11 10:36:20'),(41,16,'return_credit_note',8,0.00,60.00,60.00,'2026-06-11 12:19:21'),(42,16,'return_refund',8,60.00,0.00,60.00,'2026-06-11 12:23:12'),(43,14,'invoice',28,360000.00,0.00,360000.00,'2026-06-11 20:40:48'),(44,14,'payment',29,0.00,50000.00,310000.00,'2026-06-11 20:40:48'),(45,42,'invoice',29,990000.00,0.00,990000.00,'2026-06-15 23:10:42'),(46,42,'payment',30,0.00,50000.00,940000.00,'2026-06-15 23:10:42'),(47,42,'payment',31,0.00,940000.00,0.00,'2026-06-15 23:15:01'),(48,9,'invoice',30,875000.00,0.00,875000.00,'2026-06-17 22:55:46'),(49,9,'payment',32,0.00,10000.00,865000.00,'2026-06-17 22:55:46'),(50,9,'payment',33,0.00,865000.00,0.00,'2026-06-17 22:57:32'),(51,9,'return_credit_note',11,0.00,0.00,0.00,'2026-06-19 05:08:54'),(52,15,'invoice',31,200000.00,0.00,200000.00,'2026-06-22 02:10:15'),(53,15,'payment',34,0.00,200000.00,0.00,'2026-06-22 02:10:15'),(54,14,'payment_void',16,100.00,0.00,310100.00,'2026-06-24 02:59:32'),(55,8,'payment_void',15,24500.00,0.00,24500.00,'2026-06-24 02:59:32'),(56,11,'payment_void',13,300000.00,0.00,300000.00,'2026-06-24 02:59:32'),(57,8,'payment_void',10,99000.00,0.00,99000.00,'2026-06-24 02:59:32'),(58,9,'payment_void',11,50000.00,0.00,50000.00,'2026-06-24 02:59:32'),(61,12,'payment_void',18,50.00,0.00,475.00,'2026-06-24 02:59:32'),(62,12,'payment_void',26,100.00,0.00,525.00,'2026-06-24 02:59:32'),(63,11,'payment_void',19,100000.00,0.00,100000.00,'2026-06-24 02:59:32'),(65,9,'payment_void',14,50000.00,0.00,100000.00,'2026-06-24 02:59:32'),(66,10,'payment_void',27,150.00,0.00,375.00,'2026-06-24 02:59:32'),(68,11,'payment_void',20,50000.00,0.00,350000.00,'2026-06-24 02:59:32'),(69,9,'payment_void',25,390000.00,0.00,440000.00,'2026-06-24 02:59:32'),(70,16,'payment_void',28,80.00,0.00,140.00,'2026-06-24 02:59:32'),(71,11,'payment_void',22,100000.00,0.00,200000.00,'2026-06-24 02:59:32'),(72,14,'payment_void',17,175.00,0.00,310275.00,'2026-06-24 02:59:32'),(73,9,'payment_void',24,100000.00,0.00,200000.00,'2026-06-24 02:59:32'),(74,8,'payment_void',9,50000.00,0.00,74500.00,'2026-06-24 02:59:38'),(75,9,'payment_void',23,50000.00,0.00,250000.00,'2026-06-24 02:59:38'),(76,8,'payment_void',12,50000.00,0.00,124500.00,'2026-06-24 02:59:38'),(77,11,'payment_void',21,95000.00,0.00,295000.00,'2026-06-24 02:59:38'),(78,1,'invoice',1,1350.00,0.00,1350.00,'2026-06-25 06:24:41'),(79,1,'payment',35,0.00,1350.00,0.00,'2026-06-25 06:24:41'),(80,2,'invoice',2,6750.00,0.00,6750.00,'2026-06-26 05:35:22'),(81,2,'payment',36,0.00,1000.00,5750.00,'2026-06-26 05:35:22'),(82,2,'invoice',3,1350.00,0.00,7100.00,'2026-07-01 22:43:34'),(83,2,'payment',37,0.00,1000.00,6100.00,'2026-07-01 22:43:34'),(84,1,'invoice',4,1350.00,0.00,1350.00,'2026-07-01 23:54:17'),(85,3,'invoice',5,1350.00,0.00,1350.00,'2026-07-01 23:56:50'),(86,3,'invoice',6,1350.00,0.00,2700.00,'2026-07-02 00:11:48'),(87,3,'payment',38,0.00,500.00,2200.00,'2026-07-02 00:11:48'),(88,3,'payment',39,0.00,850.00,1350.00,'2026-07-02 00:13:51'),(89,3,'invoice',7,1350.00,0.00,2700.00,'2026-07-02 23:47:32'),(90,3,'invoice',8,66865.00,0.00,69565.00,'2026-07-03 00:04:23'),(91,2,'invoice',9,1274.40,0.00,7374.40,'2026-07-06 03:36:20'),(92,3,'invoice',10,3292.20,0.00,72857.20,'2026-07-06 03:55:45'),(93,3,'invoice',11,3292.20,0.00,76149.40,'2026-07-06 04:09:50'),(94,1,'invoice',12,3292.20,0.00,4642.20,'2026-07-06 04:11:00'),(95,1,'invoice',13,13804.82,0.00,18447.02,'2026-07-06 04:13:35'),(96,3,'invoice',14,3292.20,0.00,79441.60,'2026-07-09 00:02:56'),(97,2,'invoice',15,8850.00,0.00,16224.40,'2026-07-14 05:53:15'),(98,1,'invoice',16,1593.00,0.00,20040.02,'2026-07-14 06:10:10'),(99,1,'invoice_cancel',4,0.00,1350.00,18690.02,'2026-07-14 06:44:36'),(100,3,'invoice_cancel',5,0.00,1350.00,78091.60,'2026-07-14 06:44:56'),(101,2,'payment_void',36,1000.00,0.00,17224.40,'2026-07-14 06:46:41'),(103,1,'return_credit_note',4,0.00,1350.00,17340.02,'2026-07-15 05:19:13'),(104,4,'invoice',17,3186.00,0.00,3186.00,'2026-07-15 05:24:59'),(105,4,'return_credit_note',5,0.00,2700.00,486.00,'2026-07-15 05:26:35'),(106,4,'return_refund',5,2700.00,0.00,486.00,'2026-07-15 05:32:24'),(107,4,'invoice',18,24426.00,0.00,24912.00,'2026-07-15 05:43:53'),(108,4,'return_credit_note',6,0.00,24426.00,486.00,'2026-07-15 05:53:54'),(109,4,'return_refund',6,24426.00,0.00,486.00,'2026-07-15 05:53:55'),(110,3,'return_credit_note',7,0.00,1350.00,76741.60,'2026-07-15 06:18:24'),(111,3,'return_credit_note',8,0.00,2790.00,73951.60,'2026-07-15 06:18:55'),(112,4,'invoice',19,138048.20,0.00,138534.20,'2026-07-15 06:33:25'),(113,4,'return_credit_note',9,0.00,138048.20,486.00,'2026-07-15 06:35:28'),(114,4,'return_refund',9,138048.20,0.00,486.00,'2026-07-15 06:35:28'),(115,3,'return_credit_note',10,0.00,3292.20,70659.40,'2026-07-15 06:40:03'),(116,3,'return_credit_note',11,0.00,2790.00,67869.40,'2026-07-15 06:40:34'),(117,2,'return_credit_note',12,0.00,1274.40,15950.00,'2026-07-15 06:51:35'),(118,5,'invoice',20,3492.80,0.00,3492.80,'2026-07-21 04:24:34'),(119,5,'payment',40,0.00,1500.00,1992.80,'2026-07-21 04:27:25'),(120,5,'invoice',21,34928.00,0.00,36920.80,'2026-07-21 04:55:29'),(121,5,'payment',41,0.00,10000.00,26920.80,'2026-07-21 04:57:47'),(122,5,'payment',42,0.00,24928.00,1992.80,'2026-07-21 04:58:24'),(123,5,'invoice',22,12978.82,0.00,14971.62,'2026-07-22 03:22:29'),(124,1,'payment_void',35,1350.00,0.00,1350.00,'2026-08-02 22:23:09'),(125,1,'invoice',1,11851.92,0.00,11851.92,'2026-08-04 01:24:03'),(126,1,'invoice_cancel',1,0.00,11851.92,0.00,'2026-08-04 01:28:07'),(127,1,'invoice',2,3292.20,0.00,3292.20,'2026-08-04 01:33:42'),(128,1,'invoice_cancel',2,0.00,3292.20,0.00,'2026-08-04 01:58:33'),(129,1,'invoice',3,2790.00,0.00,2790.00,'2026-08-04 03:38:03'),(130,1,'return_credit_note',1,0.00,2790.00,0.00,'2026-08-04 04:11:15'),(131,2,'invoice',1,1500.00,0.00,1500.00,'2026-08-06 00:53:11'),(132,1,'invoice',2,1500.00,0.00,1500.00,'2026-08-06 00:56:39'),(133,1,'payment',43,0.00,1000.00,500.00,'2026-08-06 00:56:39'),(134,2,'invoice',3,1500.00,0.00,3000.00,'2026-08-06 01:17:45'),(135,1,'invoice',4,4500.00,0.00,5000.00,'2026-08-06 01:35:33'),(136,1,'payment',44,0.00,3000.00,2000.00,'2026-08-06 01:35:33'),(137,1,'invoice',5,80000.00,0.00,82000.00,'2026-08-08 03:09:06'),(138,1,'payment',45,0.00,80000.00,2000.00,'2026-08-08 10:51:57'),(139,1,'payment',46,0.00,1500.00,500.00,'2026-08-08 10:53:47'),(140,1,'payment',47,0.00,500.00,0.00,'2026-08-10 07:13:22'),(141,4,'invoice',6,177000.00,0.00,177000.00,'2026-08-12 04:39:26'),(142,4,'invoice',7,3555340.00,0.00,3732340.00,'2026-08-12 05:06:41'),(143,2,'invoice',8,885000.00,0.00,885000.00,'2026-08-12 05:09:19'),(144,4,'invoice',9,177000.00,0.00,3909340.00,'2026-08-13 02:02:57'),(145,4,'invoice',10,2714000.00,0.00,6623340.00,'2026-08-13 02:27:24'),(146,4,'invoice',1,4425.00,0.00,4425.00,'2026-08-13 22:20:00'),(147,1,'invoice',1,58100.00,0.00,58100.00,'2026-08-17 05:45:27'),(148,1,'payment',1,0.00,50000.00,8100.00,'2026-08-17 05:45:27'),(149,3,'invoice',2,5900.00,0.00,5900.00,'2026-08-19 00:30:56');
/*!40000 ALTER TABLE `customer_ledger` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_payment_terms`
--

DROP TABLE IF EXISTS `customer_payment_terms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_payment_terms` (
  `term_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int NOT NULL,
  `credit_days` int DEFAULT '0',
  `credit_limit` decimal(12,2) DEFAULT '0.00',
  `payment_mode` varchar(50) DEFAULT NULL,
  `notes` text,
  `payment_method` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`term_id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `customer_payment_terms_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_payment_terms`
--

LOCK TABLES `customer_payment_terms` WRITE;
/*!40000 ALTER TABLE `customer_payment_terms` DISABLE KEYS */;
INSERT INTO `customer_payment_terms` VALUES (12,15,0,0.00,NULL,'','Bank Transfer'),(13,1,0,0.00,NULL,'','Bank Transfer'),(15,3,0,0.00,NULL,'','Bank Transfer'),(16,4,0,0.00,NULL,'','Bank Transfer'),(17,5,0,0.00,NULL,'','Bank Transfer'),(18,6,0,0.00,NULL,'','Bank Transfer'),(19,1,0,0.00,NULL,'','Bank Transfer'),(20,1,0,0.00,NULL,'','Bank Transfer'),(21,1,0,0.00,NULL,'','Bank Transfer'),(23,3,0,0.00,NULL,'','Bank Transfer'),(24,2,0,0.00,NULL,'','Bank Transfer'),(25,4,0,0.00,NULL,'','Bank Transfer'),(26,1,0,0.00,NULL,'','Bank Transfer'),(27,2,0,0.00,NULL,'','Bank Transfer'),(28,3,0,0.00,NULL,'','Bank Transfer'),(29,4,0,0.00,NULL,'','Bank Transfer');
/*!40000 ALTER TABLE `customer_payment_terms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_payments`
--

DROP TABLE IF EXISTS `customer_payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_payments` (
  `payment_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int DEFAULT NULL,
  `invoice_id` int DEFAULT NULL,
  `amount` decimal(12,2) DEFAULT NULL,
  `payment_date` datetime DEFAULT NULL,
  `payment_method` varchar(50) DEFAULT NULL,
  `reference_number` varchar(100) DEFAULT NULL,
  `notes` text,
  `is_cancelled` tinyint(1) NOT NULL DEFAULT '0',
  `cancelled_at` datetime DEFAULT NULL,
  `cancellation_reason` text,
  PRIMARY KEY (`payment_id`),
  KEY `customer_id` (`customer_id`),
  KEY `invoice_id` (`invoice_id`),
  CONSTRAINT `customer_payments_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `customer_payments_ibfk_2` FOREIGN KEY (`invoice_id`) REFERENCES `invoices` (`invoice_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_payments`
--

LOCK TABLES `customer_payments` WRITE;
/*!40000 ALTER TABLE `customer_payments` DISABLE KEYS */;
INSERT INTO `customer_payments` VALUES (1,1,1,50000.00,'2026-08-17 00:00:00','Bank Transfer',NULL,'Opening payment recorded during invoice INV-20260817-001 creation.',0,NULL,NULL);
/*!40000 ALTER TABLE `customer_payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customers` (
  `customer_id` int NOT NULL AUTO_INCREMENT,
  `customer_code` varchar(50) DEFAULT NULL,
  `name` varchar(255) NOT NULL,
  `gst_number` varchar(50) DEFAULT NULL,
  `pan_number` varchar(20) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `status` enum('active','inactive') DEFAULT 'active',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `company` varchar(150) DEFAULT NULL,
  `city` varchar(100) DEFAULT NULL,
  `credit_limit` decimal(12,2) DEFAULT '0.00',
  `outstanding_balance` decimal(12,2) DEFAULT '0.00',
  PRIMARY KEY (`customer_id`),
  UNIQUE KEY `customer_code` (`customer_code`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (1,'CUST-000001','Ravi Kumar','','','9876543210','ravikumar@gmail.com','active','2026-08-13 06:53:31','2026-08-17 05:45:27','','12-45, Main Road, Guntur, Andhra Pradesh',0.00,8100.00),(2,'CUST-000002','Suresh reddy','','','9123456780','sureshreddy@gmail.com','active','2026-08-13 06:55:02','2026-08-13 12:25:01','','8-21 ,market road, vijayawada, Andhra Pradesh',0.00,0.00),(3,'CUST-000003','Nandhitha Sri','','','9491755559','nadhithachebattina@gmail.com','active','2026-08-13 06:55:23','2026-08-19 00:30:56','','Ratnalakunta, Eluru',0.00,5900.00),(4,'CUST-000004','Priya Devi','','','9012345678','priyadevi@gmail.com','inactive','2026-08-13 06:56:31','2026-08-13 23:15:48','','15-8, railway road, Nellore',0.00,4425.00);
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cycle_counts`
--

DROP TABLE IF EXISTS `cycle_counts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cycle_counts` (
  `cycle_id` int NOT NULL AUTO_INCREMENT,
  `warehouse_id` int DEFAULT NULL,
  `frequency` enum('daily','weekly','monthly') DEFAULT NULL,
  `last_run` date DEFAULT NULL,
  `next_run` date DEFAULT NULL,
  `status` enum('active','inactive') DEFAULT 'active',
  PRIMARY KEY (`cycle_id`),
  KEY `warehouse_id` (`warehouse_id`),
  CONSTRAINT `cycle_counts_ibfk_1` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cycle_counts`
--

LOCK TABLES `cycle_counts` WRITE;
/*!40000 ALTER TABLE `cycle_counts` DISABLE KEYS */;
/*!40000 ALTER TABLE `cycle_counts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `goods_receipt_items`
--

DROP TABLE IF EXISTS `goods_receipt_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `goods_receipt_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `grn_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity_received` decimal(10,2) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `discount` decimal(65,30) DEFAULT NULL,
  `line_total` decimal(65,30) DEFAULT NULL,
  `tax` decimal(65,30) DEFAULT NULL,
  `tax_percentage` decimal(10,2) NOT NULL DEFAULT '0.00',
  `taxable_amount` decimal(10,2) NOT NULL DEFAULT '0.00',
  `tax_amount` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `grn_id` (`grn_id`),
  CONSTRAINT `goods_receipt_items_ibfk_1` FOREIGN KEY (`grn_id`) REFERENCES `goods_receipts` (`grn_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `goods_receipt_items`
--

LOCK TABLES `goods_receipt_items` WRITE;
/*!40000 ALTER TABLE `goods_receipt_items` DISABLE KEYS */;
INSERT INTO `goods_receipt_items` VALUES (1,1,3,NULL,100.00,3559.00,0.000000000000000000000000000000,355900.000000000000000000000000000000,0.000000000000000000000000000000,0.00,355900.00,0.00),(2,1,2,NULL,50.00,499.00,0.000000000000000000000000000000,24950.000000000000000000000000000000,0.000000000000000000000000000000,0.00,24950.00,0.00),(3,1,1,NULL,50.00,1500.00,0.000000000000000000000000000000,75000.000000000000000000000000000000,0.000000000000000000000000000000,0.00,75000.00,0.00),(4,2,1,NULL,1.00,1500.00,0.000000000000000000000000000000,1500.000000000000000000000000000000,0.000000000000000000000000000000,0.00,1500.00,0.00),(5,2,3,NULL,1.00,3559.00,0.000000000000000000000000000000,3559.000000000000000000000000000000,0.000000000000000000000000000000,0.00,3559.00,0.00);
/*!40000 ALTER TABLE `goods_receipt_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `goods_receipts`
--

DROP TABLE IF EXISTS `goods_receipts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `goods_receipts` (
  `grn_id` int NOT NULL AUTO_INCREMENT,
  `po_id` int DEFAULT NULL,
  `supplier_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `receipt_date` datetime DEFAULT NULL,
  `status` enum('pending','completed') DEFAULT 'pending',
  `notes` text,
  `is_cancelled` tinyint(1) NOT NULL DEFAULT '0',
  `cancelled_at` datetime DEFAULT NULL,
  `cancellation_reason` text,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `grn_number` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `SupplierInvoice` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SupplierInvoiceDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`grn_id`),
  UNIQUE KEY `IX_goods_receipts_grn_number` (`grn_number`),
  KEY `po_id` (`po_id`),
  KEY `supplier_id` (`supplier_id`),
  CONSTRAINT `goods_receipts_ibfk_1` FOREIGN KEY (`po_id`) REFERENCES `purchase_orders` (`po_id`),
  CONSTRAINT `goods_receipts_ibfk_2` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `goods_receipts`
--

LOCK TABLES `goods_receipts` WRITE;
/*!40000 ALTER TABLE `goods_receipts` DISABLE KEYS */;
INSERT INTO `goods_receipts` VALUES (1,1,1,2,'2026-08-17 00:00:00','completed','',0,NULL,NULL,0,NULL,NULL,'GRN-000001',NULL,NULL),(2,2,2,2,'2026-08-19 00:00:00','completed','',0,NULL,NULL,0,NULL,NULL,'GRN-000002',NULL,NULL);
/*!40000 ALTER TABLE `goods_receipts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoice_items`
--

DROP TABLE IF EXISTS `invoice_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoice_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `invoice_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `total` decimal(12,2) DEFAULT NULL,
  `tax_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `tax_percent` decimal(18,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `invoice_id` (`invoice_id`),
  KEY `product_id` (`product_id`),
  KEY `variant_id` (`variant_id`),
  CONSTRAINT `invoice_items_ibfk_1` FOREIGN KEY (`invoice_id`) REFERENCES `invoices` (`invoice_id`),
  CONSTRAINT `invoice_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `invoice_items_ibfk_3` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoice_items`
--

LOCK TABLES `invoice_items` WRITE;
/*!40000 ALTER TABLE `invoice_items` DISABLE KEYS */;
INSERT INTO `invoice_items` VALUES (1,1,3,NULL,10.00,5000.00,50000.00,8100.00,18.00),(2,2,3,NULL,1.00,5000.00,5000.00,900.00,18.00);
/*!40000 ALTER TABLE `invoice_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoices`
--

DROP TABLE IF EXISTS `invoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoices` (
  `invoice_id` int NOT NULL AUTO_INCREMENT,
  `so_id` int DEFAULT NULL,
  `customer_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `invoice_number` varchar(100) DEFAULT NULL,
  `invoice_date` datetime DEFAULT NULL,
  `due_date` date DEFAULT NULL,
  `status` varchar(32) NOT NULL DEFAULT 'Sent',
  `total_amount` decimal(12,2) DEFAULT NULL,
  `paid_amount` decimal(12,2) DEFAULT '0.00',
  `balance_amount` decimal(12,2) DEFAULT NULL,
  `is_cancelled` tinyint(1) NOT NULL DEFAULT '0',
  `cancelled_at` datetime DEFAULT NULL,
  `cancellation_reason` text,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`invoice_id`),
  UNIQUE KEY `invoice_number` (`invoice_number`),
  KEY `so_id` (`so_id`),
  KEY `customer_id` (`customer_id`),
  KEY `idx_invoices_is_cancelled_customer` (`is_cancelled`,`customer_id`),
  KEY `idx_invoices_warehouse_id` (`warehouse_id`),
  CONSTRAINT `fk_invoices_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`),
  CONSTRAINT `invoices_ibfk_1` FOREIGN KEY (`so_id`) REFERENCES `sales_orders` (`so_id`),
  CONSTRAINT `invoices_ibfk_2` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoices`
--

LOCK TABLES `invoices` WRITE;
/*!40000 ALTER TABLE `invoices` DISABLE KEYS */;
INSERT INTO `invoices` VALUES (1,NULL,1,NULL,'INV-20260817-001','2026-08-17 00:00:00','2026-09-01','Partially Paid',58100.00,50000.00,8100.00,0,NULL,NULL,0,NULL,NULL),(2,NULL,3,NULL,'INV-20260819-001','2026-08-19 00:00:00','2026-09-03','Sent',5900.00,0.00,5900.00,0,NULL,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `invoices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `location_movements`
--

DROP TABLE IF EXISTS `location_movements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `location_movements` (
  `movement_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `from_bin_id` int DEFAULT NULL,
  `to_bin_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  `movement_date` datetime DEFAULT NULL,
  PRIMARY KEY (`movement_id`),
  KEY `product_id` (`product_id`),
  KEY `variant_id` (`variant_id`),
  KEY `warehouse_id` (`warehouse_id`),
  KEY `from_bin_id` (`from_bin_id`),
  KEY `to_bin_id` (`to_bin_id`),
  CONSTRAINT `location_movements_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `location_movements_ibfk_2` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`),
  CONSTRAINT `location_movements_ibfk_3` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`),
  CONSTRAINT `location_movements_ibfk_4` FOREIGN KEY (`from_bin_id`) REFERENCES `bins` (`bin_id`),
  CONSTRAINT `location_movements_ibfk_5` FOREIGN KEY (`to_bin_id`) REFERENCES `bins` (`bin_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `location_movements`
--

LOCK TABLES `location_movements` WRITE;
/*!40000 ALTER TABLE `location_movements` DISABLE KEYS */;
/*!40000 ALTER TABLE `location_movements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `login_history`
--

DROP TABLE IF EXISTS `login_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `login_history` (
  `LoginHistoryId` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `LoginTime` datetime DEFAULT CURRENT_TIMESTAMP,
  `DeviceInfo` varchar(255) DEFAULT NULL,
  `IpAddress` varchar(100) DEFAULT NULL,
  `LogoutTime` datetime DEFAULT NULL,
  `Browser` varchar(100) DEFAULT NULL,
  `OperatingSystem` varchar(100) DEFAULT NULL,
  `LogoutType` varchar(50) DEFAULT NULL,
  `IsCurrentSession` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`LoginHistoryId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `login_history_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2183 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `login_history`
--

LOCK TABLES `login_history` WRITE;
/*!40000 ALTER TABLE `login_history` DISABLE KEYS */;
INSERT INTO `login_history` VALUES (1060,44,'2026-07-11 13:48:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1061,44,'2026-07-11 13:56:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1063,44,'2026-07-11 14:10:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1064,44,'2026-07-11 14:11:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1067,44,'2026-07-11 16:46:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1068,44,'2026-07-11 17:01:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1069,44,'2026-07-11 17:21:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1070,44,'2026-07-11 17:39:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1071,44,'2026-07-12 04:22:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1072,44,'2026-07-12 10:23:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1073,44,'2026-07-12 10:37:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-12 10:38:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1074,44,'2026-07-12 10:38:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1075,44,'2026-07-12 11:18:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1076,44,'2026-07-12 12:30:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1077,44,'2026-07-12 13:06:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1078,44,'2026-07-12 13:43:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1079,44,'2026-07-12 14:54:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1080,44,'2026-07-12 15:29:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1081,44,'2026-07-12 15:55:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1082,44,'2026-07-12 16:26:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1083,44,'2026-07-12 16:53:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1084,44,'2026-07-12 18:14:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1085,44,'2026-07-12 19:01:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1086,44,'2026-07-12 19:05:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1087,44,'2026-07-12 19:20:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1088,44,'2026-07-13 04:03:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:04:32','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1089,44,'2026-07-13 04:04:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:59:58','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1090,44,'2026-07-13 04:08:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:59:58','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1091,44,'2026-07-13 04:41:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:59:58','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1092,44,'2026-07-13 04:41:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:59:58','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1093,44,'2026-07-13 04:41:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-13 04:59:58','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1094,44,'2026-07-13 04:51:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:59:58','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1095,44,'2026-07-13 04:57:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-13 04:59:58','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1096,44,'2026-07-13 04:59:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-13 04:59:58','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1097,44,'2026-07-13 05:00:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1098,44,'2026-07-13 05:00:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1099,44,'2026-07-13 05:37:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1100,44,'2026-07-13 05:59:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1101,44,'2026-07-13 06:03:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1102,44,'2026-07-13 06:07:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1103,44,'2026-07-13 06:35:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1104,44,'2026-07-13 06:36:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1105,44,'2026-07-13 06:46:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1106,44,'2026-07-13 06:53:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1107,44,'2026-07-13 08:57:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1108,44,'2026-07-13 09:06:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1109,44,'2026-07-13 09:12:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1110,44,'2026-07-13 09:14:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1111,44,'2026-07-13 09:41:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1112,44,'2026-07-13 09:52:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1113,44,'2026-07-13 10:13:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1114,44,'2026-07-13 10:16:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1115,44,'2026-07-13 10:22:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1116,44,'2026-07-13 10:23:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1117,44,'2026-07-13 10:33:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1118,44,'2026-07-13 10:43:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1119,44,'2026-07-13 10:43:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1120,44,'2026-07-13 10:43:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1121,44,'2026-07-13 10:48:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1122,44,'2026-07-13 10:49:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1125,44,'2026-07-13 11:00:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1127,44,'2026-07-13 11:04:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1128,44,'2026-07-13 11:05:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1129,44,'2026-07-13 11:26:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1130,44,'2026-07-13 11:26:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1132,44,'2026-07-13 11:26:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1133,44,'2026-07-13 11:58:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1135,44,'2026-07-13 12:16:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1136,44,'2026-07-13 12:32:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1150,44,'2026-07-14 03:31:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1152,44,'2026-07-14 03:54:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1154,44,'2026-07-14 04:39:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1156,44,'2026-07-14 05:12:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1158,44,'2026-07-14 05:24:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1160,44,'2026-07-14 06:02:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1162,44,'2026-07-14 06:17:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1165,44,'2026-07-14 08:00:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1166,44,'2026-07-14 08:25:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1169,44,'2026-07-14 09:07:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1173,44,'2026-07-14 09:44:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1175,44,'2026-07-14 09:59:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1177,44,'2026-07-14 10:42:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1178,44,'2026-07-14 10:58:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1180,44,'2026-07-14 11:15:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1181,44,'2026-07-14 11:31:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1183,44,'2026-07-14 11:49:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1186,44,'2026-07-14 12:13:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1187,44,'2026-07-14 12:33:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1189,44,'2026-07-14 13:45:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1191,44,'2026-07-14 14:03:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1193,44,'2026-07-14 14:20:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1199,44,'2026-07-14 14:33:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1208,44,'2026-07-15 03:04:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1209,44,'2026-07-15 03:41:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1212,44,'2026-07-15 04:45:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1214,44,'2026-07-15 05:02:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1216,44,'2026-07-15 05:21:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1218,44,'2026-07-15 05:49:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1219,44,'2026-07-15 06:38:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1220,44,'2026-07-15 06:59:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1222,44,'2026-07-15 07:14:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1223,44,'2026-07-15 07:33:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1224,44,'2026-07-15 07:48:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1225,44,'2026-07-15 08:05:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1228,44,'2026-07-15 09:02:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1229,44,'2026-07-15 09:12:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1231,44,'2026-07-15 09:19:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1232,44,'2026-07-15 09:19:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1234,44,'2026-07-15 09:44:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1236,44,'2026-07-15 09:53:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1238,44,'2026-07-15 09:57:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1239,44,'2026-07-15 10:13:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1240,44,'2026-07-15 10:13:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1242,44,'2026-07-15 10:14:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1244,44,'2026-07-15 10:31:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1245,44,'2026-07-15 10:33:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1246,44,'2026-07-15 10:36:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1248,44,'2026-07-15 10:46:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1249,44,'2026-07-15 10:53:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1250,44,'2026-07-15 10:57:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1252,44,'2026-07-15 11:03:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1253,44,'2026-07-15 11:13:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1254,44,'2026-07-15 11:16:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1256,44,'2026-07-15 11:19:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1258,44,'2026-07-15 11:30:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1260,44,'2026-07-15 11:35:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1261,44,'2026-07-15 11:39:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1263,44,'2026-07-15 11:50:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1265,44,'2026-07-15 11:51:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1266,44,'2026-07-15 11:52:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1267,44,'2026-07-15 11:54:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1269,44,'2026-07-15 12:11:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1270,44,'2026-07-15 12:18:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1273,44,'2026-07-15 12:32:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1274,44,'2026-07-15 12:42:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1276,44,'2026-07-15 12:47:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1278,44,'2026-07-15 13:06:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1279,44,'2026-07-15 13:07:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1281,44,'2026-07-15 13:13:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1282,44,'2026-07-15 13:22:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1283,44,'2026-07-15 13:22:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1284,44,'2026-07-15 13:29:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1285,44,'2026-07-15 13:42:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1286,44,'2026-07-15 13:57:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1287,44,'2026-07-15 14:18:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1289,44,'2026-07-15 14:41:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1290,44,'2026-07-15 15:14:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1291,44,'2026-07-15 15:31:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1292,44,'2026-07-15 15:58:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1293,44,'2026-07-15 16:18:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1295,44,'2026-07-15 16:24:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1296,44,'2026-07-15 16:34:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1297,44,'2026-07-15 16:39:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1298,44,'2026-07-15 16:50:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1299,44,'2026-07-15 16:54:06','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1300,44,'2026-07-15 16:59:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1301,44,'2026-07-15 17:07:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1302,44,'2026-07-15 17:10:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1303,44,'2026-07-15 17:14:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1305,44,'2026-07-15 17:26:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1306,44,'2026-07-15 17:32:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1307,44,'2026-07-15 17:33:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1308,44,'2026-07-15 17:43:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1309,44,'2026-07-15 17:58:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1310,44,'2026-07-15 17:59:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1311,44,'2026-07-15 18:07:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1312,44,'2026-07-15 18:08:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1313,44,'2026-07-15 18:17:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1314,44,'2026-07-15 18:32:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1315,44,'2026-07-15 18:48:06','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1316,44,'2026-07-15 19:03:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1317,44,'2026-07-15 19:18:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1318,44,'2026-07-15 19:34:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1319,44,'2026-07-15 19:52:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1320,44,'2026-07-15 20:08:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1321,44,'2026-07-16 03:38:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1323,44,'2026-07-16 03:45:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1325,44,'2026-07-16 03:55:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1326,44,'2026-07-16 04:03:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1328,44,'2026-07-16 04:14:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1329,44,'2026-07-16 04:23:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1330,44,'2026-07-16 04:41:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1331,44,'2026-07-16 04:44:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1333,44,'2026-07-16 05:23:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1334,44,'2026-07-16 05:24:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1336,44,'2026-07-16 05:40:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1337,44,'2026-07-16 05:40:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1339,44,'2026-07-16 05:57:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1340,44,'2026-07-16 06:00:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1341,44,'2026-07-16 06:02:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1342,44,'2026-07-16 06:02:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1343,44,'2026-07-16 06:39:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1345,44,'2026-07-16 06:43:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1346,44,'2026-07-16 06:54:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1347,44,'2026-07-16 06:59:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1348,44,'2026-07-16 07:06:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1349,44,'2026-07-16 07:11:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1352,44,'2026-07-16 07:18:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1353,44,'2026-07-16 07:20:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1355,44,'2026-07-16 07:34:06','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1356,44,'2026-07-16 08:40:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1357,44,'2026-07-16 08:40:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1358,44,'2026-07-16 08:41:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1360,44,'2026-07-16 08:54:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1361,44,'2026-07-16 08:57:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1362,44,'2026-07-16 08:57:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1364,44,'2026-07-16 09:13:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1365,44,'2026-07-16 09:18:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1366,44,'2026-07-16 09:29:06','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1367,44,'2026-07-16 09:33:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1368,44,'2026-07-16 09:41:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1369,44,'2026-07-16 09:48:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1370,44,'2026-07-16 09:59:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1371,44,'2026-07-16 10:05:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1372,44,'2026-07-16 10:21:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1373,44,'2026-07-16 10:22:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1374,44,'2026-07-16 10:36:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1375,44,'2026-07-16 10:39:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1376,44,'2026-07-16 10:52:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1377,44,'2026-07-16 11:04:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1378,44,'2026-07-16 11:04:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1379,44,'2026-07-16 11:13:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1380,44,'2026-07-16 11:21:58','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1381,44,'2026-07-16 11:24:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1382,44,'2026-07-16 11:35:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1383,44,'2026-07-16 11:43:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1384,44,'2026-07-16 11:45:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1385,44,'2026-07-16 11:50:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1386,44,'2026-07-16 11:59:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1387,44,'2026-07-16 12:06:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1388,44,'2026-07-16 12:10:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1389,44,'2026-07-20 09:46:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1390,44,'2026-07-20 09:59:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1391,44,'2026-07-20 10:09:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1392,44,'2026-07-20 10:48:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-20 11:40:07','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1393,44,'2026-07-20 10:58:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1394,44,'2026-07-20 11:39:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:40:07','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1395,44,'2026-07-20 11:40:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-20 11:50:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1396,44,'2026-07-20 11:55:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1397,44,'2026-07-20 12:15:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1398,44,'2026-07-21 04:18:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1399,44,'2026-07-21 04:48:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1400,44,'2026-07-21 05:33:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1401,44,'2026-07-21 06:17:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1402,44,'2026-07-21 06:20:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1403,44,'2026-07-21 06:21:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1404,44,'2026-07-21 06:22:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1405,44,'2026-07-21 06:23:23','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1406,44,'2026-07-21 06:24:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1407,44,'2026-07-21 06:25:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1408,44,'2026-07-21 06:25:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1409,44,'2026-07-21 06:35:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1410,44,'2026-07-21 06:38:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1411,44,'2026-07-21 06:53:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1412,44,'2026-07-21 06:56:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1413,44,'2026-07-21 06:58:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1414,44,'2026-07-21 07:17:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1415,44,'2026-07-21 07:21:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1416,44,'2026-07-21 07:36:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1417,44,'2026-07-21 07:55:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1418,44,'2026-07-21 08:05:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1419,44,'2026-07-21 08:13:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1420,44,'2026-07-21 08:19:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1421,44,'2026-07-21 08:30:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1422,44,'2026-07-21 08:47:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1423,44,'2026-07-21 08:52:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1424,44,'2026-07-21 09:12:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1425,44,'2026-07-21 09:14:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1426,44,'2026-07-21 09:29:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1427,44,'2026-07-21 09:39:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1428,44,'2026-07-21 09:45:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1429,44,'2026-07-21 09:56:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1430,44,'2026-07-21 10:00:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1431,44,'2026-07-21 10:15:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1432,44,'2026-07-21 10:30:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1433,44,'2026-07-21 11:04:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1434,44,'2026-07-21 11:11:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1435,44,'2026-07-21 11:13:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1436,44,'2026-07-21 11:22:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1438,44,'2026-07-21 11:26:58','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1441,44,'2026-07-21 11:59:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1443,44,'2026-07-21 12:19:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1446,44,'2026-07-22 03:32:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1447,44,'2026-07-22 03:46:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1448,44,'2026-07-22 04:05:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1449,44,'2026-07-22 04:13:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1450,44,'2026-07-22 04:17:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1451,44,'2026-07-22 04:23:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1452,44,'2026-07-22 05:59:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1453,44,'2026-07-22 05:59:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1454,44,'2026-07-22 06:09:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1455,44,'2026-07-22 06:14:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1456,44,'2026-07-22 06:17:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1458,44,'2026-07-22 06:24:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1462,44,'2026-07-22 06:29:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1464,44,'2026-07-22 06:31:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 08:42:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1465,44,'2026-07-22 07:25:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1466,44,'2026-07-22 07:26:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1467,44,'2026-07-22 07:26:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1468,44,'2026-07-22 07:29:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 08:42:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1470,44,'2026-07-22 08:42:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:00:31','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1472,44,'2026-07-22 08:43:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:00:31','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1473,44,'2026-07-22 08:44:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:00:31','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1474,44,'2026-07-22 08:45:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:00:31','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1475,44,'2026-07-22 09:00:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:00:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1476,44,'2026-07-22 09:00:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:00:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1477,44,'2026-07-22 09:00:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:00:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1478,44,'2026-07-22 09:01:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:01:18','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1479,44,'2026-07-22 09:01:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:01:18','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1480,44,'2026-07-22 09:01:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:01:18','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1481,44,'2026-07-22 09:01:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:01:51','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1482,44,'2026-07-22 09:01:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:01:51','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1483,44,'2026-07-22 09:01:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:01:51','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1484,44,'2026-07-22 09:02:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:02:53','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1485,44,'2026-07-22 09:02:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:02:53','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1486,44,'2026-07-22 09:02:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:02:53','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1487,44,'2026-07-22 09:02:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:03:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1488,44,'2026-07-22 09:02:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 09:03:41','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1489,44,'2026-07-22 09:03:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 10:10:11','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1490,44,'2026-07-22 09:03:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 10:10:11','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1491,44,'2026-07-22 09:07:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:14:42','Google Chrome','Windows','Manual',_binary '\0'),(1492,44,'2026-07-22 09:15:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 09:17:13','Google Chrome','Windows','Manual',_binary '\0'),(1494,44,'2026-07-22 10:01:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 10:10:11','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1495,44,'2026-07-22 10:10:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 12:26:27','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1496,44,'2026-07-22 10:54:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-22 12:26:27','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1497,44,'2026-07-22 10:54:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-22 12:26:27','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1499,44,'2026-07-22 16:23:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1500,44,'2026-07-22 17:24:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1501,44,'2026-07-22 17:55:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1502,44,'2026-07-22 18:03:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1503,44,'2026-07-23 05:23:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1504,44,'2026-07-23 05:24:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1505,44,'2026-07-23 05:25:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1506,44,'2026-07-23 05:29:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1507,44,'2026-07-23 05:31:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1508,44,'2026-07-23 05:32:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1509,44,'2026-07-23 05:33:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1510,44,'2026-07-23 05:45:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-23 05:45:19','Microsoft Edge','Windows','Manual',_binary '\0'),(1511,44,'2026-07-23 05:45:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1512,44,'2026-07-23 05:47:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-23 05:48:20','Microsoft Edge','Windows','Manual',_binary '\0'),(1513,44,'2026-07-23 05:48:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1514,44,'2026-07-23 05:52:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1515,44,'2026-07-23 06:07:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1516,44,'2026-07-23 06:18:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1517,44,'2026-07-23 06:25:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1518,44,'2026-07-23 06:39:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1519,44,'2026-07-23 06:41:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1520,44,'2026-07-23 06:58:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1521,44,'2026-07-23 06:59:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1522,44,'2026-07-23 07:08:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1523,44,'2026-07-23 07:10:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1524,44,'2026-07-23 07:14:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1525,44,'2026-07-23 07:23:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1526,44,'2026-07-23 07:30:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1527,44,'2026-07-23 07:33:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1528,44,'2026-07-23 07:52:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1529,44,'2026-07-23 08:01:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1530,44,'2026-07-23 08:01:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1531,44,'2026-07-23 08:29:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1532,44,'2026-07-23 12:31:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1533,44,'2026-07-23 12:33:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1534,44,'2026-07-23 12:41:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1535,44,'2026-07-23 12:42:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1536,44,'2026-07-23 12:49:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1537,44,'2026-07-23 12:50:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1538,44,'2026-07-23 12:52:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1539,44,'2026-07-23 13:04:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1541,44,'2026-07-23 13:05:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1542,44,'2026-07-24 03:05:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1543,44,'2026-07-24 03:06:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1544,44,'2026-07-24 03:58:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1545,44,'2026-07-24 04:03:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1546,44,'2026-07-24 04:13:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1547,44,'2026-07-24 04:21:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 04:27:09','Google Chrome','Windows','Manual',_binary '\0'),(1548,44,'2026-07-24 04:27:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 04:28:06','Google Chrome','Windows','Manual',_binary '\0'),(1549,44,'2026-07-24 04:30:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1550,44,'2026-07-24 04:46:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1551,44,'2026-07-24 05:31:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1552,44,'2026-07-24 05:32:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:23:50','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1553,44,'2026-07-24 06:23:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:23:50','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1554,44,'2026-07-24 06:25:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:53:43','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1555,44,'2026-07-24 06:37:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:53:43','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1556,44,'2026-07-24 06:38:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 06:53:43','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1557,44,'2026-07-24 06:53:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 06:53:43','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1559,44,'2026-07-24 07:31:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 07:31:28','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1560,44,'2026-07-24 07:39:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 09:26:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1561,44,'2026-07-24 07:39:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 09:26:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1562,44,'2026-07-24 07:40:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 09:26:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1563,44,'2026-07-24 07:43:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 09:26:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1564,44,'2026-07-24 07:44:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 09:26:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1565,44,'2026-07-24 07:49:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 09:26:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1566,44,'2026-07-24 07:50:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 09:26:36','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1567,44,'2026-07-24 09:26:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 09:26:36','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1568,44,'2026-07-24 09:26:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 09:27:02','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1570,44,'2026-07-24 09:30:58','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 11:28:54','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1571,44,'2026-07-24 10:06:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 11:28:54','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1572,44,'2026-07-24 11:28:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 11:28:54','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1573,44,'2026-07-24 11:30:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 12:17:56','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1574,44,'2026-07-24 11:51:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 12:17:56','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1575,44,'2026-07-24 11:59:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 12:17:56','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1576,44,'2026-07-24 12:00:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-24 12:17:56','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1577,44,'2026-07-24 12:14:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 12:17:56','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1578,44,'2026-07-24 12:20:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 12:33:31','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1580,44,'2026-07-24 12:35:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 12:41:56','Google Chrome','Windows','Manual',_binary '\0'),(1581,44,'2026-07-24 12:46:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1583,44,'2026-07-24 12:46:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-24 12:46:42','Google Chrome','Windows','Manual',_binary '\0'),(1584,44,'2026-07-24 12:49:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1585,44,'2026-07-24 15:03:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1586,44,'2026-07-24 15:04:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1587,44,'2026-07-24 15:04:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1588,44,'2026-07-27 03:42:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1589,44,'2026-07-27 03:44:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1590,44,'2026-07-27 03:46:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1591,44,'2026-07-27 04:02:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1592,44,'2026-07-27 04:08:23','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1593,44,'2026-07-27 04:11:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1594,44,'2026-07-27 04:14:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 04:20:29','Microsoft Edge','Windows','Manual',_binary '\0'),(1595,44,'2026-07-27 04:20:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 04:20:44','Google Chrome','Windows','Manual',_binary '\0'),(1598,44,'2026-07-27 04:27:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1600,44,'2026-07-27 04:28:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 04:30:07','Google Chrome','Windows','Manual',_binary '\0'),(1604,44,'2026-07-27 04:44:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1605,44,'2026-07-27 04:45:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1606,44,'2026-07-27 04:46:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1607,44,'2026-07-27 04:48:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 04:48:17','Google Chrome','Windows','Manual',_binary '\0'),(1608,44,'2026-07-27 04:49:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 04:49:34','Google Chrome','Windows','Manual',_binary '\0'),(1609,44,'2026-07-27 05:28:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1610,44,'2026-07-27 05:28:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1611,44,'2026-07-27 05:30:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1612,44,'2026-07-27 05:30:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1613,44,'2026-07-27 05:30:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1614,44,'2026-07-27 05:31:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1615,44,'2026-07-27 05:39:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:39:22','Google Chrome','Windows','Manual',_binary '\0'),(1616,44,'2026-07-27 05:45:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:55:30','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1617,44,'2026-07-27 05:46:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 05:46:01','Google Chrome','Windows','Manual',_binary '\0'),(1618,44,'2026-07-27 05:47:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 05:55:30','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1621,44,'2026-07-27 06:01:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 06:01:51','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1622,44,'2026-07-27 06:04:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 06:04:38','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1623,44,'2026-07-27 06:13:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1624,44,'2026-07-27 06:24:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 09:11:09','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1625,44,'2026-07-27 06:24:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 09:11:09','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1626,44,'2026-07-27 06:32:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1627,44,'2026-07-27 06:32:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 06:32:44','Google Chrome','Windows','Manual',_binary '\0'),(1628,44,'2026-07-27 06:35:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1629,44,'2026-07-27 06:36:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1630,44,'2026-07-27 06:37:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-27 06:38:45','Google Chrome','Windows','Manual',_binary '\0'),(1634,44,'2026-07-27 07:17:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1635,44,'2026-07-27 07:18:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1636,44,'2026-07-27 07:27:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1637,44,'2026-07-27 07:28:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1638,44,'2026-07-27 07:33:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1639,44,'2026-07-27 07:33:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1640,44,'2026-07-27 07:34:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1641,44,'2026-07-27 08:25:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1642,44,'2026-07-27 08:25:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1645,44,'2026-07-27 08:42:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1646,44,'2026-07-27 08:51:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1647,44,'2026-07-27 08:52:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1649,44,'2026-07-27 08:58:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:11:09','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1651,44,'2026-07-27 09:11:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:30:52','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1652,44,'2026-07-27 09:14:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:30:52','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1653,44,'2026-07-27 09:15:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:30:52','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1656,44,'2026-07-27 09:26:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:30:52','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1657,44,'2026-07-27 09:31:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-27 09:32:14','Microsoft Edge','Windows','Manual',_binary '\0'),(1661,44,'2026-07-27 09:38:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1662,44,'2026-07-27 09:54:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1663,44,'2026-07-27 10:09:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1664,44,'2026-07-27 10:24:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1666,44,'2026-07-27 10:44:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1667,44,'2026-07-27 11:00:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1669,44,'2026-07-27 11:16:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1671,44,'2026-07-27 11:32:58','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1673,44,'2026-07-27 12:04:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1677,44,'2026-07-28 04:34:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1678,44,'2026-07-28 04:35:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1680,44,'2026-07-28 06:42:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1681,44,'2026-07-28 06:43:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1682,44,'2026-07-28 06:44:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1683,44,'2026-07-28 07:25:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1684,44,'2026-07-28 07:37:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1685,44,'2026-07-28 07:43:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-07-28 07:57:04','Microsoft Edge','Windows','Manual',_binary '\0'),(1692,44,'2026-07-28 08:14:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-07-28 08:16:43','Google Chrome','Windows','Manual',_binary '\0'),(1697,44,'2026-07-28 09:45:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1698,44,'2026-07-28 09:46:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1699,44,'2026-07-28 10:04:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1700,44,'2026-07-28 10:05:23','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1701,44,'2026-07-28 10:08:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1704,44,'2026-07-28 10:36:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1705,44,'2026-07-28 10:37:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1706,44,'2026-07-28 10:38:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1707,44,'2026-07-28 10:39:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1708,44,'2026-07-28 11:58:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1709,44,'2026-07-28 11:59:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1710,44,'2026-07-28 12:14:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1711,44,'2026-07-28 12:15:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1712,44,'2026-07-28 12:18:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1713,44,'2026-07-28 12:19:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1714,44,'2026-07-29 03:30:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1716,44,'2026-07-29 07:31:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1717,44,'2026-07-29 07:58:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1718,44,'2026-07-29 07:59:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1719,44,'2026-07-29 08:01:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1720,44,'2026-07-29 08:03:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1721,44,'2026-07-29 08:09:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1722,44,'2026-07-29 08:09:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1732,44,'2026-07-29 09:55:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1733,44,'2026-07-29 10:36:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1742,44,'2026-07-31 07:53:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1743,44,'2026-07-31 08:49:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1744,44,'2026-07-31 08:50:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1745,44,'2026-07-31 08:51:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1746,44,'2026-07-31 08:51:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1747,44,'2026-07-31 08:56:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1748,44,'2026-07-31 09:44:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1749,44,'2026-07-31 09:58:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1750,44,'2026-07-31 09:59:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1751,44,'2026-07-31 11:54:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36 Edg/150.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1752,44,'2026-07-31 13:11:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1753,44,'2026-07-31 13:12:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1754,44,'2026-08-03 01:52:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1755,44,'2026-08-03 01:54:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1756,44,'2026-08-03 01:57:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1757,44,'2026-08-03 02:00:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 02:28:52','Google Chrome','Windows','Manual',_binary '\0'),(1758,44,'2026-08-03 02:29:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1759,44,'2026-08-03 02:30:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1760,44,'2026-08-03 03:51:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-03 13:08:24','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1761,44,'2026-08-03 05:25:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1762,44,'2026-08-03 05:26:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1763,44,'2026-08-03 05:27:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-03 06:16:56','Microsoft Edge','Windows','Manual',_binary '\0'),(1764,44,'2026-08-03 06:17:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1765,44,'2026-08-03 06:25:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1766,44,'2026-08-03 06:26:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 07:16:48','Google Chrome','Windows','Manual',_binary '\0'),(1767,44,'2026-08-03 07:53:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 10:14:16','Google Chrome','Windows','Manual',_binary '\0'),(1768,44,'2026-08-03 10:14:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1769,44,'2026-08-03 10:14:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1770,44,'2026-08-03 10:15:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 10:40:30','Google Chrome','Windows','Manual',_binary '\0'),(1771,44,'2026-08-03 10:40:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1772,44,'2026-08-03 10:46:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1773,44,'2026-08-03 10:47:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 11:42:51','Google Chrome','Windows','Manual',_binary '\0'),(1774,44,'2026-08-03 11:43:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:08:24','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1775,44,'2026-08-03 13:08:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-03 13:13:21','Google Chrome','Windows','Manual',_binary '\0'),(1776,44,'2026-08-03 13:13:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1777,44,'2026-08-03 13:15:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1778,44,'2026-08-03 13:25:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1779,44,'2026-08-03 13:26:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1780,44,'2026-08-04 03:49:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1781,44,'2026-08-04 03:50:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1782,44,'2026-08-04 03:50:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1783,44,'2026-08-04 04:27:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-05 08:09:47','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1784,44,'2026-08-04 04:55:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1785,44,'2026-08-04 06:11:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1786,44,'2026-08-04 08:31:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1787,44,'2026-08-04 08:31:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1788,44,'2026-08-04 08:34:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1789,44,'2026-08-04 08:35:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1790,44,'2026-08-04 08:51:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-04 10:35:28','Google Chrome','Windows','Manual',_binary '\0'),(1791,44,'2026-08-04 10:41:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-04 10:52:02','Google Chrome','Windows','Manual',_binary '\0'),(1792,44,'2026-08-04 10:57:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1793,44,'2026-08-05 04:19:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1794,44,'2026-08-05 04:28:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-05 08:09:47','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1795,44,'2026-08-05 04:57:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1796,44,'2026-08-05 04:58:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1797,44,'2026-08-05 05:19:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:09:47','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1798,103,'2026-08-05 08:11:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-05 08:11:33','Google Chrome','Windows','Manual',_binary '\0'),(1799,44,'2026-08-05 08:11:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1800,44,'2026-08-05 08:57:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1801,44,'2026-08-05 09:10:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1802,44,'2026-08-05 09:24:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1803,44,'2026-08-05 11:57:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1804,44,'2026-08-06 04:01:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1805,44,'2026-08-06 04:02:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1806,44,'2026-08-06 04:02:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1807,44,'2026-08-06 05:24:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1808,44,'2026-08-06 05:26:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1809,44,'2026-08-06 05:42:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1810,44,'2026-08-06 05:47:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1811,44,'2026-08-06 05:48:06','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1812,44,'2026-08-06 12:04:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1813,44,'2026-08-06 12:05:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1814,44,'2026-08-06 12:08:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1815,44,'2026-08-06 12:23:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1816,44,'2026-08-06 12:24:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1817,44,'2026-08-06 12:39:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1818,44,'2026-08-06 12:40:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1819,44,'2026-08-06 12:46:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1820,44,'2026-08-06 13:38:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1821,44,'2026-08-06 13:40:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1822,44,'2026-08-07 03:39:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1823,44,'2026-08-07 03:43:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1824,44,'2026-08-07 03:44:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1825,44,'2026-08-07 03:49:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1826,44,'2026-08-07 03:51:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1827,44,'2026-08-07 04:10:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1828,44,'2026-08-07 04:23:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1829,44,'2026-08-07 04:25:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1830,44,'2026-08-07 04:26:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1831,44,'2026-08-07 05:25:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1832,44,'2026-08-07 05:26:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1833,44,'2026-08-07 08:40:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1834,44,'2026-08-07 08:41:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-07 09:19:32','Google Chrome','Windows','Manual',_binary '\0'),(1835,44,'2026-08-07 09:42:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-07 09:47:00','Microsoft Edge','Windows','Manual',_binary '\0'),(1836,44,'2026-08-07 09:49:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-07 09:51:23','Microsoft Edge','Windows','Manual',_binary '\0'),(1837,44,'2026-08-07 10:15:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1838,44,'2026-08-07 10:22:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-07 10:31:56','Microsoft Edge','Windows','Manual',_binary '\0'),(1839,44,'2026-08-07 10:42:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1840,44,'2026-08-07 10:51:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1841,44,'2026-08-07 10:52:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1842,44,'2026-08-07 10:58:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1843,44,'2026-08-07 11:40:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1844,44,'2026-08-07 11:51:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1845,44,'2026-08-07 11:52:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1846,44,'2026-08-07 11:53:23','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1847,44,'2026-08-07 12:26:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1848,44,'2026-08-07 12:27:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1849,44,'2026-08-07 16:26:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1850,44,'2026-08-07 16:27:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1851,44,'2026-08-08 04:57:23','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1852,44,'2026-08-08 14:00:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1853,44,'2026-08-08 14:27:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1','2026-08-08 16:02:14','Google Chrome','Windows','Manual',_binary '\0'),(1854,44,'2026-08-08 16:08:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1855,44,'2026-08-08 16:57:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1856,44,'2026-08-09 03:18:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1857,44,'2026-08-09 11:19:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1858,44,'2026-08-09 19:43:23','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1859,44,'2026-08-09 20:18:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1860,44,'2026-08-09 21:16:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1861,44,'2026-08-10 03:54:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1862,44,'2026-08-10 03:55:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1863,44,'2026-08-10 03:56:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1864,44,'2026-08-10 04:05:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1865,103,'2026-08-10 04:05:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-10 04:07:22','Google Chrome','Windows','Manual',_binary '\0'),(1866,103,'2026-08-10 04:07:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-11 13:25:42','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1867,44,'2026-08-10 04:12:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1868,44,'2026-08-10 04:13:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1869,44,'2026-08-10 05:03:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1870,44,'2026-08-10 05:04:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1871,44,'2026-08-10 05:13:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1872,44,'2026-08-10 05:13:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1873,44,'2026-08-10 05:30:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1874,44,'2026-08-10 05:31:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1875,44,'2026-08-10 05:39:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-10 05:50:19','Google Chrome','Windows','Manual',_binary '\0'),(1876,44,'2026-08-10 05:50:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1877,44,'2026-08-10 05:50:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1878,44,'2026-08-10 05:59:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1879,44,'2026-08-10 06:00:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1880,44,'2026-08-10 06:04:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1881,44,'2026-08-10 06:05:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1882,44,'2026-08-10 07:05:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1883,44,'2026-08-10 07:06:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1884,44,'2026-08-10 08:51:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1885,44,'2026-08-10 09:52:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1886,44,'2026-08-10 09:53:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1887,44,'2026-08-10 10:29:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1888,44,'2026-08-10 10:30:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1889,44,'2026-08-10 11:56:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1890,44,'2026-08-11 04:35:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1891,44,'2026-08-11 04:36:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1892,103,'2026-08-11 05:15:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-11 13:25:42','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1893,44,'2026-08-11 07:17:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1894,44,'2026-08-11 07:25:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1895,44,'2026-08-11 08:18:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1896,44,'2026-08-11 08:20:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1897,44,'2026-08-11 09:18:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1898,44,'2026-08-11 09:19:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1899,44,'2026-08-11 09:35:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 09:45:55','Microsoft Edge','Windows','Manual',_binary '\0'),(1900,105,'2026-08-11 09:37:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 11:40:52','Microsoft Edge','Windows','Manual',_binary '\0'),(1901,44,'2026-08-11 09:58:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 10:13:33','Microsoft Edge','Windows','Manual',_binary '\0'),(1902,44,'2026-08-11 10:17:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 10:21:21','Microsoft Edge','Windows','Manual',_binary '\0'),(1903,44,'2026-08-11 10:24:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1904,44,'2026-08-11 10:25:06','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1905,44,'2026-08-11 10:33:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1906,44,'2026-08-11 11:02:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1907,105,'2026-08-11 11:40:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 11:41:07','Microsoft Edge','Windows','Manual',_binary '\0'),(1908,105,'2026-08-11 11:41:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 11:56:23','Microsoft Edge','Windows','Manual',_binary '\0'),(1909,105,'2026-08-11 11:56:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 11:56:42','Microsoft Edge','Windows','Manual',_binary '\0'),(1910,105,'2026-08-11 11:56:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 12:04:41','Microsoft Edge','Windows','Manual',_binary '\0'),(1911,44,'2026-08-11 12:04:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 13:28:11','Microsoft Edge','Windows','Manual',_binary '\0'),(1912,105,'2026-08-11 12:04:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 12:06:26','Microsoft Edge','Windows','Manual',_binary '\0'),(1913,105,'2026-08-11 12:07:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 12:08:46','Microsoft Edge','Windows','Manual',_binary '\0'),(1914,105,'2026-08-11 12:08:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 12:10:37','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1915,105,'2026-08-11 12:10:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 12:37:58','Microsoft Edge','Windows','Manual',_binary '\0'),(1916,105,'2026-08-11 12:38:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 12:39:41','Microsoft Edge','Windows','Manual',_binary '\0'),(1917,105,'2026-08-11 12:39:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 12:59:19','Microsoft Edge','Windows','Manual',_binary '\0'),(1918,105,'2026-08-11 12:59:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 13:12:49','Microsoft Edge','Windows','Manual',_binary '\0'),(1919,105,'2026-08-11 13:12:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-11 13:14:58','Microsoft Edge','Windows','Manual',_binary '\0'),(1920,105,'2026-08-11 13:15:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 13:09:46','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1921,103,'2026-08-11 13:17:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-11 13:17:53','Google Chrome','Windows','Manual',_binary '\0'),(1922,103,'2026-08-11 13:17:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-11 13:25:42','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1923,103,'2026-08-11 13:25:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-12 12:09:04','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1924,44,'2026-08-11 13:28:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1925,44,'2026-08-12 04:11:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1926,44,'2026-08-12 04:12:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1927,44,'2026-08-12 04:12:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1928,103,'2026-08-12 04:14:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-12 12:09:04','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1929,44,'2026-08-12 04:16:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1930,44,'2026-08-12 04:28:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-12 05:31:52','Google Chrome','Windows','Manual',_binary '\0'),(1931,105,'2026-08-12 05:58:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 13:09:46','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1932,44,'2026-08-12 07:10:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1933,44,'2026-08-12 08:04:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1934,44,'2026-08-12 08:15:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1935,44,'2026-08-12 09:46:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1936,44,'2026-08-12 09:47:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1937,44,'2026-08-12 10:52:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1938,44,'2026-08-12 10:53:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1939,44,'2026-08-12 10:54:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1940,44,'2026-08-12 11:53:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1941,44,'2026-08-12 11:54:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1942,103,'2026-08-12 12:09:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-13 10:29:01','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1943,44,'2026-08-13 04:38:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1944,44,'2026-08-13 04:39:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1945,44,'2026-08-13 04:40:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1946,103,'2026-08-13 04:46:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-13 10:29:01','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1947,105,'2026-08-13 06:43:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 13:09:46','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1948,44,'2026-08-13 07:11:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-13 09:54:59','Microsoft Edge','Windows','Manual',_binary '\0'),(1950,103,'2026-08-13 10:29:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-14 06:27:38','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1951,44,'2026-08-13 10:52:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1952,44,'2026-08-13 11:09:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1953,44,'2026-08-13 11:10:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1954,44,'2026-08-13 11:39:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-13 11:44:20','Google Chrome','Windows','Manual',_binary '\0'),(1955,44,'2026-08-13 11:44:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-13 11:48:43','Google Chrome','Windows','Manual',_binary '\0'),(1956,44,'2026-08-13 12:08:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1957,44,'2026-08-13 12:15:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1958,44,'2026-08-13 12:17:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-13 12:36:42','Google Chrome','Windows','Manual',_binary '\0'),(1959,44,'2026-08-13 13:11:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1960,44,'2026-08-14 03:40:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1961,44,'2026-08-14 03:41:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1962,44,'2026-08-14 03:42:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1963,44,'2026-08-14 03:47:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 05:30:09','Microsoft Edge','Windows','Manual',_binary '\0'),(1964,103,'2026-08-14 03:58:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-14 06:27:38','Google Chrome','Windows','Logout All Devices',_binary '\0'),(1965,105,'2026-08-14 04:26:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 10:58:09','Microsoft Edge','Windows','Manual',_binary '\0'),(1966,107,'2026-08-14 05:30:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-14 09:01:31','Google Chrome','Windows','Manual',_binary '\0'),(1967,44,'2026-08-14 05:56:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1968,103,'2026-08-14 06:27:44','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-14 08:03:08','Google Chrome','Windows','Manual',_binary '\0'),(1969,44,'2026-08-14 06:35:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1970,103,'2026-08-14 08:03:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(1971,44,'2026-08-14 09:01:49','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-14 09:25:23','Google Chrome','Windows','Manual',_binary '\0'),(1972,44,'2026-08-14 09:25:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1973,44,'2026-08-14 10:35:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1974,44,'2026-08-14 10:36:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1975,105,'2026-08-14 11:04:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 11:04:13','Microsoft Edge','Windows','Manual',_binary '\0'),(1976,105,'2026-08-14 11:25:58','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 11:26:03','Microsoft Edge','Windows','Manual',_binary '\0'),(1977,105,'2026-08-14 11:26:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 11:56:15','Microsoft Edge','Windows','Manual',_binary '\0'),(1978,44,'2026-08-14 11:36:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1979,44,'2026-08-14 11:37:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1980,105,'2026-08-14 11:56:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 12:21:19','Microsoft Edge','Windows','Manual',_binary '\0'),(1981,44,'2026-08-14 12:13:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(1982,44,'2026-08-14 12:14:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1983,44,'2026-08-14 12:14:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-14 12:15:16','Google Chrome','Windows','Manual',_binary '\0'),(1984,44,'2026-08-14 12:15:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 12:16:20','Microsoft Edge','Windows','Manual',_binary '\0'),(1985,44,'2026-08-14 12:18:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 12:19:25','Microsoft Edge','Windows','Manual',_binary '\0'),(1986,109,'2026-08-14 12:19:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-17 05:58:51','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1987,105,'2026-08-14 12:28:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 12:31:58','Microsoft Edge','Windows','Manual',_binary '\0'),(1988,110,'2026-08-14 12:55:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 13:07:39','Microsoft Edge','Windows','Manual',_binary '\0'),(1989,105,'2026-08-14 13:07:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 13:10:49','Microsoft Edge','Windows','Manual',_binary '\0'),(1990,105,'2026-08-14 13:10:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-14 13:18:22','Microsoft Edge','Windows','Manual',_binary '\0'),(1991,105,'2026-08-14 13:18:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 13:09:46','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(1992,44,'2026-08-14 15:08:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1993,44,'2026-08-14 15:09:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1994,44,'2026-08-14 15:20:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1995,44,'2026-08-14 15:21:58','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1996,44,'2026-08-14 15:31:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1997,44,'2026-08-14 15:32:13','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1998,44,'2026-08-17 04:21:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(1999,44,'2026-08-17 04:22:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2000,44,'2026-08-17 04:23:06','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2001,105,'2026-08-17 06:02:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-17 09:49:45','Microsoft Edge','Windows','Manual',_binary '\0'),(2002,44,'2026-08-17 06:14:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2003,44,'2026-08-17 06:15:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2004,44,'2026-08-17 08:52:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2005,44,'2026-08-17 08:54:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2006,44,'2026-08-17 09:16:23','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2007,44,'2026-08-17 09:17:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2009,109,'2026-08-17 09:59:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-17 12:11:33','Microsoft Edge','Windows','Manual',_binary '\0'),(2010,109,'2026-08-17 12:11:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:10:35','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(2011,44,'2026-08-18 04:20:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2012,44,'2026-08-18 04:21:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2013,109,'2026-08-18 04:23:43','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:10:35','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(2014,105,'2026-08-18 06:19:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 13:09:46','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(2015,103,'2026-08-18 07:59:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(2016,44,'2026-08-18 11:47:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2017,44,'2026-08-18 11:48:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2018,109,'2026-08-18 12:32:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:10:35','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(2019,44,'2026-08-18 12:34:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2020,44,'2026-08-18 12:35:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2021,44,'2026-08-19 03:46:34','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2022,109,'2026-08-19 03:47:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:10:35','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(2023,44,'2026-08-19 03:47:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2024,103,'2026-08-19 03:55:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(2025,44,'2026-08-19 04:50:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2026,44,'2026-08-19 04:50:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2027,109,'2026-08-19 07:18:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:10:35','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(2028,105,'2026-08-19 09:12:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 12:39:30','Microsoft Edge','Windows','Manual',_binary '\0'),(2029,109,'2026-08-19 10:10:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:10:50','Microsoft Edge','Windows','Manual',_binary '\0'),(2030,109,'2026-08-19 10:10:59','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:11:12','Microsoft Edge','Windows','Manual',_binary '\0'),(2031,109,'2026-08-19 10:11:21','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:11:37','Microsoft Edge','Windows','Manual',_binary '\0'),(2032,109,'2026-08-19 10:13:39','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2033,109,'2026-08-19 10:15:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 10:17:06','Microsoft Edge','Windows','Manual',_binary '\0'),(2034,109,'2026-08-19 11:21:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2035,103,'2026-08-19 11:55:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(2036,44,'2026-08-19 12:12:33','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2037,44,'2026-08-19 12:13:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2038,105,'2026-08-19 12:41:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 12:51:17','Microsoft Edge','Windows','Manual',_binary '\0'),(2039,105,'2026-08-19 12:51:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 12:51:49','Microsoft Edge','Windows','Manual',_binary '\0'),(2040,105,'2026-08-19 12:52:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 13:09:46','Microsoft Edge','Windows','Logout All Devices',_binary '\0'),(2041,105,'2026-08-19 13:13:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-19 13:13:14','Microsoft Edge','Windows','Manual',_binary '\0'),(2043,105,'2026-08-19 13:15:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2044,44,'2026-08-20 03:35:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2045,44,'2026-08-20 03:37:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2046,109,'2026-08-20 03:37:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 04:48:08','Microsoft Edge','Windows','Manual',_binary '\0'),(2047,44,'2026-08-20 04:27:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2048,44,'2026-08-20 04:28:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2049,109,'2026-08-20 04:54:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2050,105,'2026-08-20 04:54:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 06:37:27','Microsoft Edge','Windows','Manual',_binary '\0'),(2051,103,'2026-08-20 05:03:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-20 05:03:29','Google Chrome','Windows','Manual',_binary '\0'),(2052,103,'2026-08-20 05:19:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(2053,103,'2026-08-20 06:05:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-20 09:27:40','Google Chrome','Windows','Manual',_binary '\0'),(2054,44,'2026-08-20 06:31:37','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2055,44,'2026-08-20 06:32:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2056,105,'2026-08-20 06:37:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 06:44:57','Microsoft Edge','Windows','Manual',_binary '\0'),(2057,105,'2026-08-20 06:46:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 06:50:51','Microsoft Edge','Windows','Manual',_binary '\0'),(2058,105,'2026-08-20 07:00:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 08:51:24','Microsoft Edge','Windows','Manual',_binary '\0'),(2059,44,'2026-08-20 07:45:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2060,44,'2026-08-20 07:46:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2061,105,'2026-08-20 09:24:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 10:20:07','Microsoft Edge','Windows','Manual',_binary '\0'),(2062,103,'2026-08-20 09:53:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-20 09:53:12','Google Chrome','Windows','Manual',_binary '\0'),(2063,103,'2026-08-20 10:36:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-20 10:57:45','Google Chrome','Windows','Manual',_binary '\0'),(2064,105,'2026-08-20 10:38:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 11:15:31','Microsoft Edge','Windows','Manual',_binary '\0'),(2065,116,'2026-08-20 11:04:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 11:13:20','Microsoft Edge','Windows','Manual',_binary '\0'),(2066,103,'2026-08-20 11:07:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-20 11:07:43','Google Chrome','Windows','Manual',_binary '\0'),(2067,116,'2026-08-20 11:13:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 11:14:01','Microsoft Edge','Windows','Manual',_binary '\0'),(2068,109,'2026-08-20 11:14:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2069,105,'2026-08-20 11:15:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 11:20:02','Microsoft Edge','Windows','Manual',_binary '\0'),(2070,105,'2026-08-20 11:30:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 12:12:12','Microsoft Edge','Windows','Manual',_binary '\0'),(2071,103,'2026-08-20 11:32:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(2072,44,'2026-08-20 11:59:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2073,44,'2026-08-20 12:00:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2074,105,'2026-08-20 12:12:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-20 12:13:31','Microsoft Edge','Windows','Manual',_binary '\0'),(2078,105,'2026-08-20 12:18:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2079,44,'2026-08-21 03:29:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2080,44,'2026-08-21 03:30:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2081,44,'2026-08-21 03:57:58','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-21 03:58:25','Microsoft Edge','Windows','Manual',_binary '\0'),(2082,44,'2026-08-21 04:00:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2083,44,'2026-08-21 04:24:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2084,44,'2026-08-21 04:29:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2085,103,'2026-08-21 04:36:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 06:19:55','Google Chrome','Windows','Manual',_binary '\0'),(2086,118,'2026-08-21 05:39:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 05:40:39','Google Chrome','Windows','Manual',_binary '\0'),(2087,118,'2026-08-21 05:40:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 05:49:46','Google Chrome','Windows','Manual',_binary '\0'),(2088,105,'2026-08-21 05:46:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-21 06:07:09','Microsoft Edge','Windows','Manual',_binary '\0'),(2089,118,'2026-08-21 05:49:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 08:42:15','Google Chrome','Windows','Manual',_binary '\0'),(2090,105,'2026-08-21 06:07:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2091,44,'2026-08-21 07:32:16','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2092,44,'2026-08-21 07:33:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2093,103,'2026-08-21 07:35:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 10:01:16','Google Chrome','Windows','Manual',_binary '\0'),(2094,44,'2026-08-21 07:39:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2095,105,'2026-08-21 07:39:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-21 08:39:36','Microsoft Edge','Windows','Manual',_binary '\0'),(2096,44,'2026-08-21 07:41:17','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2097,44,'2026-08-21 07:58:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2098,44,'2026-08-21 08:01:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2099,44,'2026-08-21 08:03:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2100,44,'2026-08-21 08:12:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2101,44,'2026-08-21 08:14:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2102,118,'2026-08-21 08:42:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 05:10:08','Google Chrome','Windows','Password Reset',_binary '\0'),(2103,105,'2026-08-21 08:44:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-21 12:04:40','Microsoft Edge','Windows','Manual',_binary '\0'),(2104,44,'2026-08-21 09:28:36','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2105,44,'2026-08-21 09:29:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2106,118,'2026-08-21 09:33:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 11:52:35','Google Chrome','Windows','Manual',_binary '\0'),(2107,103,'2026-08-21 10:01:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 10:38:59','Google Chrome','Windows','Manual',_binary '\0'),(2108,103,'2026-08-21 10:43:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 10:44:05','Google Chrome','Windows','Manual',_binary '\0'),(2109,103,'2026-08-21 10:48:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 11:18:22','Google Chrome','Windows','Manual',_binary '\0'),(2110,103,'2026-08-21 11:19:54','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(2111,118,'2026-08-21 11:52:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 11:52:55','Google Chrome','Windows','Manual',_binary '\0'),(2112,118,'2026-08-21 11:53:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-21 11:53:19','Google Chrome','Windows','Manual',_binary '\0'),(2113,105,'2026-08-21 12:04:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2114,44,'2026-08-24 03:41:22','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2115,103,'2026-08-24 03:41:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 03:42:05','Google Chrome','Windows','Manual',_binary '\0'),(2116,103,'2026-08-24 03:42:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 03:49:49','Google Chrome','Windows','Manual',_binary '\0'),(2117,44,'2026-08-24 03:42:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2118,105,'2026-08-24 03:48:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-24 04:51:52','Microsoft Edge','Windows','Manual',_binary '\0'),(2119,103,'2026-08-24 03:50:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 04:13:58','Google Chrome','Windows','Manual',_binary '\0'),(2120,103,'2026-08-24 04:14:03','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 04:21:13','Google Chrome','Windows','Manual',_binary '\0'),(2121,103,'2026-08-24 04:21:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 04:22:05','Google Chrome','Windows','Manual',_binary '\0'),(2122,103,'2026-08-24 04:22:08','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 04:25:43','Google Chrome','Windows','Manual',_binary '\0'),(2123,103,'2026-08-24 04:25:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 05:37:09','Google Chrome','Windows','Manual',_binary '\0'),(2124,105,'2026-08-24 04:52:07','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-24 04:52:11','Microsoft Edge','Windows','Manual',_binary '\0'),(2125,105,'2026-08-24 04:53:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-24 05:51:16','Microsoft Edge','Windows','Manual',_binary '\0'),(2126,103,'2026-08-24 05:37:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 05:48:41','Google Chrome','Windows','Manual',_binary '\0'),(2127,103,'2026-08-24 05:49:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 05:58:16','Google Chrome','Windows','Manual',_binary '\0'),(2128,105,'2026-08-24 05:55:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-24 05:57:02','Microsoft Edge','Windows','Manual',_binary '\0'),(2129,103,'2026-08-24 05:58:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:03:24','Google Chrome','Windows','Manual',_binary '\0'),(2130,105,'2026-08-24 05:59:00','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2131,103,'2026-08-24 06:08:14','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:08:25','Google Chrome','Windows','Manual',_binary '\0'),(2132,103,'2026-08-24 06:08:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:11:39','Google Chrome','Windows','Manual',_binary '\0'),(2133,103,'2026-08-24 06:12:10','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:15:31','Google Chrome','Windows','Manual',_binary '\0'),(2134,103,'2026-08-24 06:23:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:24:44','Google Chrome','Windows','Manual',_binary '\0'),(2135,103,'2026-08-24 06:27:18','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:38:04','Google Chrome','Windows','Password Reset',_binary '\0'),(2136,44,'2026-08-24 06:34:46','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2137,44,'2026-08-24 06:35:24','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2138,103,'2026-08-24 06:35:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:36:07','Google Chrome','Windows','Manual',_binary '\0'),(2139,116,'2026-08-24 06:37:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2140,103,'2026-08-24 06:38:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:39:43','Google Chrome','Windows','Manual',_binary '\0'),(2141,103,'2026-08-24 06:39:47','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 06:39:51','Google Chrome','Windows','Manual',_binary '\0'),(2142,103,'2026-08-24 06:39:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(2143,118,'2026-08-24 06:59:51','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-24 07:04:53','Google Chrome','Windows','Manual',_binary '\0'),(2144,126,'2026-08-24 07:10:32','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-24 07:12:14','Microsoft Edge','Windows','Manual',_binary '\0'),(2145,126,'2026-08-24 07:12:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-24 07:17:58','Microsoft Edge','Windows','Manual',_binary '\0'),(2146,126,'2026-08-24 07:18:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2147,44,'2026-08-25 04:12:28','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2148,44,'2026-08-25 04:13:45','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2149,44,'2026-08-25 04:24:56','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2150,126,'2026-08-25 05:04:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2151,118,'2026-08-25 05:10:12','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 06:58:29','Google Chrome','Windows','Manual',_binary '\0'),(2152,116,'2026-08-25 05:39:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2153,118,'2026-08-25 06:58:42','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 06:58:52','Google Chrome','Windows','Manual',_binary '\0'),(2154,118,'2026-08-25 09:03:40','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 09:05:44','Google Chrome','Windows','Manual',_binary '\0'),(2155,118,'2026-08-25 09:17:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 09:19:57','Google Chrome','Windows','Manual',_binary '\0'),(2156,118,'2026-08-25 09:24:04','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 09:47:03','Google Chrome','Windows','Manual',_binary '\0'),(2157,118,'2026-08-25 09:47:35','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 10:01:17','Google Chrome','Windows','Manual',_binary '\0'),(2158,127,'2026-08-25 10:01:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(2159,103,'2026-08-25 11:17:05','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 12:39:08','Google Chrome','Windows','Manual',_binary '\0'),(2160,103,'2026-08-25 12:39:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1','2026-08-25 12:53:03','Google Chrome','Windows','Manual',_binary '\0'),(2161,103,'2026-08-25 12:53:15','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(2162,44,'2026-08-26 03:33:31','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2163,44,'2026-08-26 03:34:30','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2164,118,'2026-08-26 04:03:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2165,103,'2026-08-26 04:13:26','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/152.0.0.0 Safari/537.36','::1','2026-08-26 04:40:14','Google Chrome','Windows','Manual',_binary '\0'),(2166,105,'2026-08-26 04:17:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2167,116,'2026-08-26 04:21:01','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-26 10:05:07','Microsoft Edge','Windows','Manual',_binary '\0'),(2168,126,'2026-08-26 04:34:41','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2169,103,'2026-08-26 04:40:19','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/152.0.0.0 Safari/537.36','::1',NULL,'Google Chrome','Windows',NULL,_binary ''),(2170,44,'2026-08-26 04:40:48','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2171,116,'2026-08-26 10:05:20','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-26 10:05:27','Microsoft Edge','Windows','Manual',_binary '\0'),(2172,116,'2026-08-26 10:05:55','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-26 10:06:06','Microsoft Edge','Windows','Manual',_binary '\0'),(2173,116,'2026-08-26 10:31:29','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2174,116,'2026-08-26 10:32:25','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1','2026-08-26 10:44:16','Microsoft Edge','Windows','Manual',_binary '\0'),(2175,116,'2026-08-26 10:47:50','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2176,44,'2026-08-26 11:11:11','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2177,44,'2026-08-26 11:11:53','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2178,44,'2026-08-27 03:49:09','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2179,44,'2026-08-27 03:49:57','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2180,126,'2026-08-27 04:37:38','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2181,118,'2026-08-27 08:40:02','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary ''),(2182,116,'2026-08-27 09:35:27','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','::1',NULL,'Microsoft Edge','Windows',NULL,_binary '');
/*!40000 ALTER TABLE `login_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `modules`
--

DROP TABLE IF EXISTS `modules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `modules` (
  `ModuleId` int NOT NULL AUTO_INCREMENT,
  `ModuleKey` varchar(100) NOT NULL,
  `ModuleName` varchar(100) NOT NULL,
  `Category` varchar(100) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `DisplayOrder` int DEFAULT '0',
  `IsActive` bit(1) DEFAULT b'1',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ModuleId`),
  UNIQUE KEY `ModuleKey` (`ModuleKey`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `modules`
--

LOCK TABLES `modules` WRITE;
/*!40000 ALTER TABLE `modules` DISABLE KEYS */;
INSERT INTO `modules` VALUES (1,'dashboard','Dashboard','Dashboard','Dashboard module',1,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(2,'products','Products','Masters','Manage products',2,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(3,'categories','Categories','Masters','Manage categories',3,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(4,'subCategories','Sub Categories','Masters','Manage sub categories',4,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(5,'brands','Brands','Masters','Manage brands',5,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(6,'units','Units','Masters','Manage units',6,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(7,'productAttributes','Product Attributes','Masters','Manage product attributes',7,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(8,'productVariants','Product Variants','Masters','Manage product variants',8,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(9,'stock','Stock','Inventory','Stock register',9,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(10,'stockMovements','Stock Movements','Inventory','Stock movement history',10,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(11,'stockLedger','Stock Ledger','Inventory','Stock ledger',11,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(12,'stockAdjustments','Stock Adjustments','Inventory','Stock adjustments',12,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(13,'stockAdjustmentItems','Stock Adjustment Items','Inventory','Stock adjustment items',13,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(14,'stockTransfers','Stock Transfers','Inventory','Stock transfers',14,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(15,'stockTransferItems','Stock Transfer Items','Inventory','Stock transfer items',15,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(16,'stockAudits','Stock Audits','Inventory','Stock audits',16,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(17,'stockAuditItems','Stock Audit Items','Inventory','Stock audit items',17,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(18,'goodsReceipts','Goods Receipts','Inventory','Goods receipt',18,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(19,'purchases','Purchases','Inventory','Purchase management',19,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(20,'sales','Sales','Inventory','Sales management',20,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(21,'inventoryAudit','Inventory Audit','Inventory','Inventory audit',21,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(22,'barcode','Barcode / QR','Inventory','Barcode management',22,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(23,'suppliers','Suppliers','Masters','Supplier management',23,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(24,'customers','Customers','Masters','Customer management',24,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(25,'customerPayments','Customer Payments','Billing','Customer payments',25,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(26,'supplierPayments','Supplier Payments','Billing','Supplier payments',26,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(27,'warehouses','Warehouses','Management','Warehouse management',27,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(28,'reports','Reports','Management','Reports',28,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(29,'notifications','Notifications','Management','Notifications',29,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(30,'accounting','Accounting','Management','Accounting',30,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(31,'returns','Returns & Exchanges','Returns','Returns management',31,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(32,'users','Users','Administration','User management',32,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(33,'roles','Roles','Administration','Role management',33,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(34,'auditLogs','Audit Logs','Administration','Audit logs',34,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(35,'systemSettings','System Settings','Administration','System settings',35,_binary '','2026-07-04 00:17:08','2026-07-04 00:17:08'),(36,'purchaseIndent','Purchase Indent','Inventory',NULL,36,_binary '','2026-08-14 20:26:29','2026-08-14 20:26:29'),(37,'purchaseReturns','Purchase Returns','Returns',NULL,37,_binary '','2026-08-14 20:26:52','2026-08-14 20:26:52'),(38,'salesReturns','Sales Returns','Returns',NULL,38,_binary '','2026-08-14 20:27:04','2026-08-14 20:27:04');
/*!40000 ALTER TABLE `modules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `notification_id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `message` text,
  `type` varchar(50) DEFAULT NULL,
  `is_read` tinyint(1) DEFAULT '0',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`notification_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
INSERT INTO `notifications` VALUES (1,'Invoice Created','Invoice INV-20260817-001 created for Ravi Kumar.','action',1,'2026-08-17 05:45:27'),(2,'Invoice Created','Invoice INV-20260819-001 created for Nandhitha Sri.','action',1,'2026-08-19 00:30:56');
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `otps`
--

DROP TABLE IF EXISTS `otps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `otps` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ExpiryTime` datetime(6) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  `IsUsed` tinyint(1) NOT NULL DEFAULT '0',
  `Purpose` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_otps_UserId` (`UserId`),
  CONSTRAINT `FK_otps_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `otps`
--

LOCK TABLES `otps` WRITE;
/*!40000 ALTER TABLE `otps` DISABLE KEYS */;
INSERT INTO `otps` VALUES (1,'pathipriya94@gmail.com','685232','2026-08-17 10:03:39.943772','2026-08-17 09:53:39.943707',1,'EmailVerification',NULL),(2,'shriya123@gmail.com','380838','2026-08-19 07:23:02.015792','2026-08-19 07:13:02.015719',0,'EmailVerification',NULL),(7,'sureshnuthangi989@gmail.com','415690','2026-08-19 13:20:26.025500','2026-08-19 13:10:26.025462',0,'EmailVerification',NULL),(8,'sowjanyagurram245@gmail.com','299065','2026-08-19 13:24:12.009461','2026-08-19 13:14:12.009460',1,'EmailVerification',NULL),(9,'sai.lalitha@pirnav.in','687685','2026-08-20 11:13:06.659667','2026-08-20 11:03:06.659619',1,'EmailVerification',NULL),(16,'hemasrisai.varma@pirnav.com','118168','2026-08-24 07:19:37.336124','2026-08-24 07:09:37.336123',1,'EmailVerification',NULL),(19,'sukanyapucheti@gmail.com','296785','2026-08-25 09:52:25.766542','2026-08-25 09:47:25.766542',0,'PasswordReset',NULL),(20,'fdgfdh@pin.co','182181','2026-08-26 10:32:03.679182','2026-08-26 10:22:03.679098',0,'EmailVerification',NULL);
/*!40000 ALTER TABLE `otps` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pendingusers`
--

DROP TABLE IF EXISTS `pendingusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pendingusers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Email` varchar(256) NOT NULL,
  `PhoneNumber` varchar(10) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EmailVerificationToken` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EmailVerificationTokenExpiry` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pendingusers`
--

LOCK TABLES `pendingusers` WRITE;
/*!40000 ALTER TABLE `pendingusers` DISABLE KEYS */;
INSERT INTO `pendingusers` VALUES (15,'TestTemp3','testtemp3@gmail.com','9999999993','$2a$11$7S.Qx3NlNWyNFiwzMz3M5egyXwgEi9oITU.c3wGGUoh.uow5Kj67G','User','dae3636cb166423bb44d1b2be03fe924','2026-07-28 09:19:10.666361'),(20,'Test Admin','testadmin@example.com','9876543210','$2a$11$Oxfnrp9HJIPaIUr6bh8ny.TThlIx3c8f2c3MP6MbPPrd91XgyWg3y','User','883e3addcf654d168252b1b55d816d41','2026-08-01 11:07:19.645984'),(22,'1234566789','reddyssiva07@gail.com','0000000000','$2a$11$2iR6lRGa6AQZhtDYdY12r.D0kQO0907gCymJTjchonjRrFjUv9wgq','User','154d73b4-c582-418e-afb8-44ad65e3107a','2026-08-07 09:28:44.156029'),(23,'Rohith Sharma','shashikala61200@gmail.com','9098999999','$2a$11$ZpV0Qfeo05EFSG3ThOCr9eCFXef4dw2g1OMe/8wxAQZR4vvCJQvkG','User','51aafb76-8060-4c8f-8945-8e289c68fe73','2026-08-08 16:13:55.235575'),(24,'Rohith Sharma','mpk@gmail.com','6899887777','$2a$11$oiy44RAWhk2kqfvNiVjK7.grV4c24Hl17juN8l/mzPTI1RWKSUEv6','User','addae948-c3f5-4564-b819-d38f3a519310','2026-08-08 16:16:13.816508'),(25,'1234567890','siva1@gmail.com','1234567890','$2a$11$yuBtzQbDSuXC77RIX8IABODwKwf7LJo0Dg/DCtFWjvgSJkaVXyhNe','User','37e3e626-960c-4f5a-919a-6efb2c017d43','2026-08-10 05:11:09.349965'),(26,'288 Rithvik Elugam','elugamrithvik123@gmail.com','8897918860','$2a$11$6e8r/qhTx5hmFxHOdn.Ae.S3fFOK81YeKccMCCJeVGrBwW5hL4Wsu','User','1fe72dd8-53e1-422f-b54b-5106d8adfa73','2026-08-12 05:42:24.209961'),(27,'Sowjanya Gurram','sowjanyagurram897@gmail.com','9441991897','$2a$11$jhOpf8Z4IYuunewtBAOv7u5O.3G8ERgrmtrKhpcFXFokPSLv8i1Wq','User','5b8441b4-e8db-4367-8bc3-4f3ff16f4797','2026-08-14 11:09:49.563258'),(29,'nandhitha','nandhithachebattina@gmail.com','9491755559','$2a$11$lk/Nbnc39Pj0T4NOpfHfmOZWpqfwAAR8tVvdQajrLm6ubsqygJGeW','User','4fc04e30-e168-4fcc-aec3-0b012d1a3f22','2026-08-14 12:27:01.105422'),(31,'Test Bot','test_bot_1786711428080@example.com','9999999999','$2a$11$V/2qJmxj6FP7sHKM3J9UuOzb1nkoahlUj8uhpxzQt.gKnGsjmE2xy','User','a428a474-5f7b-43fe-9e07-ad75b826e2c0','2026-08-14 12:53:49.214582'),(34,'shriya singh','shriya123@gmail.com','7364375872','$2a$11$1MYmfG8pbI3MjBIt/1GoWOkmcjXvuoD6KC.c92nEC2CbQjs5zps3.','User','f3c51a32-5e75-41b5-9402-82f693ca038f','2026-08-19 07:23:01.946938'),(39,'Suresh Darling','sureshnuthangi989@gmail.com','7981882515','$2a$11$HYHApLTPd8O.OQk8LHR/nexuCxTMtRfeHPYOC1eYKzYA2XKupIo36','User','f39eab0f-e92d-4e64-a9e8-8529175ef101','2026-08-19 13:20:26.010082'),(44,'Adgfsdhffagdfhg','fdgfdh@pin.co','8763725842','$2a$11$J4JrVAW/Fxg9shhRTFYbDOex.KSzEyLEgQxQwwrD8RO7sd2qtW6eK','User','f902ebb7-a51c-4f8e-856a-e9dedca0e008','2026-08-26 10:32:03.598722');
/*!40000 ALTER TABLE `pendingusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_images`
--

DROP TABLE IF EXISTS `product_images`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_images` (
  `image_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `image_url` varchar(255) DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`image_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `product_images_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_images`
--

LOCK TABLES `product_images` WRITE;
/*!40000 ALTER TABLE `product_images` DISABLE KEYS */;
/*!40000 ALTER TABLE `product_images` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_variants`
--

DROP TABLE IF EXISTS `product_variants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_variants` (
  `variant_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `variant_name` varchar(255) DEFAULT NULL,
  `sku` varchar(100) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `cost_price` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`variant_id`),
  UNIQUE KEY `sku` (`sku`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `product_variants_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_variants`
--

LOCK TABLES `product_variants` WRITE;
/*!40000 ALTER TABLE `product_variants` DISABLE KEYS */;
INSERT INTO `product_variants` VALUES (1,1,'Default','SD-DAP-20230947',6000.00,1500.00),(2,2,'Default','SD-HGT-2021475',2180.00,499.00),(3,3,'Default','TI-F7-16521',5000.00,3559.00),(4,4,'Default','AD-IN-20220908',9000.00,7500.00),(5,5,'Default','PH-SK-20220642',6500.00,4500.00),(6,6,'Default','JJHDKJFHJKSD',1500.00,1000.00);
/*!40000 ALTER TABLE `product_variants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `product_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `sku` varchar(100) NOT NULL,
  `category_id` int DEFAULT NULL,
  `brand_id` int DEFAULT NULL,
  `unit_id` int DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `cost_price` decimal(10,2) DEFAULT NULL,
  `barcode` varchar(100) DEFAULT NULL,
  `description` text,
  `status` enum('active','inactive') DEFAULT 'active',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `reorder_level` int DEFAULT NULL,
  `stock` int DEFAULT NULL,
  `supplier_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `is_deleted` bit(1) DEFAULT b'0',
  `image_url` varchar(500) DEFAULT NULL,
  `sub_category_id` int DEFAULT NULL,
  `is_archived` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`product_id`),
  UNIQUE KEY `sku` (`sku`),
  KEY `category_id` (`category_id`),
  KEY `brand_id` (`brand_id`),
  KEY `unit_id` (`unit_id`),
  KEY `fk_products_subcategory` (`sub_category_id`),
  KEY `idx_products_is_archived` (`is_archived`),
  KEY `idx_products_is_deleted_archived` (`is_deleted`,`is_archived`),
  KEY `idx_products_supplier_id` (`supplier_id`),
  KEY `idx_products_warehouse_id` (`warehouse_id`),
  CONSTRAINT `fk_products_subcategory` FOREIGN KEY (`sub_category_id`) REFERENCES `sub_categories` (`sub_category_id`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`category_id`) REFERENCES `categories` (`category_id`),
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`brand_id`) REFERENCES `brands` (`brand_id`),
  CONSTRAINT `products_ibfk_3` FOREIGN KEY (`unit_id`) REFERENCES `units` (`unit_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'Sunya 680019 Heavy Duty 24 Inch Top Saw and Bottom Garden Knife','SD-DAP-20230947',1,2,3,2000.00,1500.00,'BAR-20260813-180902896','This Cutter is ergonomically designed for perfect grip and better use. Utmost care has given to the construction of the knife to ensure safety. The weight is checked for a better swing and the handles are comfortable and extended spiked tang is good for pounding and scraping. This saw can be used in multiple ways such as, chopping, cutting unwanted grass, branches, weeds, and stems of plants. This tool is also convenient for usage in farming and camping.','active','2026-08-13 07:12:28','2026-08-19 01:30:42',10,NULL,NULL,NULL,_binary '\0','/uploads/products/fef5f99d-6425-4109-ad32-411e0ee96179.png',1,0),(2,'Heavy Duty Carbon Steel Sickle with Wooden Handle','SD-HGT-2021475',1,1,3,1090.00,499.00,'BAR-20260813-181414180','This Heavy Duty Carbon Steel Sickle, functioning as a robust bill hook, is designed for demanding cutting and clearing tasks. Featuring a sharp, curved carbon steel blade, it efficiently cuts through dense grass, stubborn weeds, and light brush. The sturdy wooden handle provides a comfortable and secure grip for prolonged use, ensuring powerful cutting action. Ideal for farmers, landscapers, and gardeners, this durable implement offers reliable performance for harvesting, clearing fields, and maintaining pathways, making it an indispensable tool for heavy outdoor work.','active','2026-08-13 07:16:31','2026-08-17 05:01:18',10,NULL,NULL,NULL,_binary '\0','/uploads/products/691150b5-5d69-4a37-9e40-f1d3a0a52178.png',1,0),(3,'FESTEL Telescopic Pole with Fruit Picking Basket, 7 to 24 feet Extendable Pole','TI-F7-16521',1,3,3,5000.00,3559.00,'BAR-20260813-181749250','The FESTEL Telescopic Pole with Fruit Picking Basket is designed for safe and easy harvesting of mangoes, guavas, coconuts, and other fruits. Its lightweight yet durable design pole extends from 7 to 24 feet, featuring a shock-proof handle and strong basket to ensure effortless fruit collection without damage.','inactive','2026-08-13 07:20:56','2026-08-27 06:23:56',10,NULL,NULL,NULL,_binary '\0','/uploads/products/2c25b415-b08e-4aa5-b3e1-c018b56e68cd.png',1,1),(4,'Heavy Duty 0.25 HP Single Stage Vacuum Pump, 240 V','AD-IN-20220908',2,1,1,9000.00,7500.00,'BAR-20260813-182251522','A 0.25 HP single-stage vacuum pump is a machine used to remove air or gas molecules from a sealed space to create a vacuum. A 0.25 HP single-stage vacuum pump is a reliable and efficient machine that can be used for a range of vacuum applications, from laboratory experiments to industrial processes.\nIt is mainly used for the purpose to clean and sealing. The Vacuum Pump is easy to install and simple to maintain. The maintenance cost of the pump is low and results in giving higher efficiency. It is mainly seen in industries as well as in homes too. A Vacuum Pump is used to move various fluids, including chemicals, water waste, and fuel between locations efficiently.','active','2026-08-13 07:25:17','2026-08-13 12:55:17',10,NULL,NULL,NULL,_binary '\0','/uploads/products/5e700e26-9236-4850-b5e3-c3dba8575eaa.png',3,0),(5,'Premium Quality 0.5 HP Single Phase Mini Openwell Pump 72 feet max head with Control Panel','PH-SK-20220642',2,1,1,6500.00,4500.00,'BAR-20260813-182613286','','active','2026-08-13 07:27:22','2026-08-27 06:27:54',10,NULL,NULL,NULL,_binary '\0','/uploads/products/7e5ed40b-496f-4437-a37b-84e0e1abe30f.png',4,0),(6,'None','JJHDKJFHJKSD',1,3,2,1500.00,1000.00,'BAR-20260813-182704347','','active','2026-08-13 07:54:21','2026-08-13 23:00:41',1,NULL,NULL,NULL,_binary '','/uploads/products/c1cbf98e-c6eb-49e1-a31e-cbab84689723.png',2,0);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_indent_items`
--

DROP TABLE IF EXISTS `purchase_indent_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_indent_items` (
  `purchase_indent_item_id` int NOT NULL AUTO_INCREMENT,
  `purchase_indent_id` int NOT NULL,
  `product_id` int NOT NULL,
  `required_qty` decimal(18,2) NOT NULL,
  `unit_id` int NOT NULL,
  `available_stock` decimal(18,2) NOT NULL,
  `required_date` datetime(6) NOT NULL,
  `remarks` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`purchase_indent_item_id`),
  KEY `IX_purchase_indent_items_purchase_indent_id` (`purchase_indent_id`),
  CONSTRAINT `FK_purchase_indent_items_purchase_indents_purchase_indent_id` FOREIGN KEY (`purchase_indent_id`) REFERENCES `purchase_indents` (`purchase_indent_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_indent_items`
--

LOCK TABLES `purchase_indent_items` WRITE;
/*!40000 ALTER TABLE `purchase_indent_items` DISABLE KEYS */;
INSERT INTO `purchase_indent_items` VALUES (10,5,3,100.00,3,0.00,'2026-08-17 00:00:00.000000',''),(11,5,2,50.00,3,0.00,'2026-08-17 00:00:00.000000',''),(12,5,1,50.00,3,0.00,'2026-08-17 00:00:00.000000',''),(13,6,1,1.00,3,50.00,'2026-08-19 00:00:00.000000',''),(14,6,3,1.00,3,79.00,'2026-08-19 00:00:00.000000','');
/*!40000 ALTER TABLE `purchase_indent_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_indents`
--

DROP TABLE IF EXISTS `purchase_indents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_indents` (
  `purchase_indent_id` int NOT NULL AUTO_INCREMENT,
  `indent_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `indent_date` datetime(6) NOT NULL,
  `required_date` datetime(6) NOT NULL,
  `requested_by` int NOT NULL,
  `department_id` int NOT NULL,
  `supplier_id` int DEFAULT NULL,
  `approved_by` int DEFAULT NULL,
  `priority` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `remarks` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `total_items` int NOT NULL,
  `total_quantity` decimal(18,2) NOT NULL,
  `created_at` datetime(6) DEFAULT NULL,
  `updated_at` datetime(6) DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `deleted_at` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`purchase_indent_id`),
  UNIQUE KEY `IX_purchase_indents_indent_number` (`indent_number`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_indents`
--

LOCK TABLES `purchase_indents` WRITE;
/*!40000 ALTER TABLE `purchase_indents` DISABLE KEYS */;
INSERT INTO `purchase_indents` VALUES (5,'IND-20260817-001','2026-08-17 00:00:00.000000','2026-08-17 00:00:00.000000',106,3,1,109,'High','Converted','',3,200.00,'2026-08-17 10:27:59.902712','2026-08-17 10:28:47.473473',0,NULL),(6,'IND-20260819-001','2026-08-19 00:00:00.000000','2026-08-19 00:00:00.000000',110,2,2,109,'Medium','Converted','',2,2.00,'2026-08-19 06:59:32.537851','2026-08-19 06:59:44.308688',0,NULL);
/*!40000 ALTER TABLE `purchase_indents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_order_items`
--

DROP TABLE IF EXISTS `purchase_order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_order_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `po_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  `received_quantity` decimal(10,2) DEFAULT '0.00',
  `price` decimal(10,2) DEFAULT NULL,
  `total` decimal(12,2) DEFAULT NULL,
  `discount` decimal(65,30) DEFAULT NULL,
  `tax` decimal(65,30) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `po_id` (`po_id`),
  CONSTRAINT `purchase_order_items_ibfk_1` FOREIGN KEY (`po_id`) REFERENCES `purchase_orders` (`po_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_order_items`
--

LOCK TABLES `purchase_order_items` WRITE;
/*!40000 ALTER TABLE `purchase_order_items` DISABLE KEYS */;
INSERT INTO `purchase_order_items` VALUES (1,1,3,NULL,100.00,100.00,3559.00,355900.00,NULL,NULL),(2,1,2,NULL,50.00,50.00,499.00,24950.00,NULL,NULL),(3,1,1,NULL,50.00,50.00,1500.00,75000.00,NULL,NULL),(4,2,1,NULL,1.00,1.00,1500.00,1500.00,NULL,NULL),(5,2,3,NULL,1.00,1.00,3559.00,3559.00,NULL,NULL);
/*!40000 ALTER TABLE `purchase_order_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_orders`
--

DROP TABLE IF EXISTS `purchase_orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_orders` (
  `po_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `po_number` varchar(100) DEFAULT NULL,
  `order_date` date DEFAULT NULL,
  `expected_date` date DEFAULT NULL,
  `status` varchar(50) DEFAULT NULL,
  `total_amount` decimal(12,2) DEFAULT NULL,
  `notes` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `is_cancelled` tinyint(1) NOT NULL DEFAULT '0',
  `cancelled_at` datetime DEFAULT NULL,
  `cancellation_reason` text,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  `receiving_status` varchar(50) NOT NULL DEFAULT 'pending',
  `payment_status` varchar(50) NOT NULL DEFAULT 'Unpaid',
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`po_id`),
  UNIQUE KEY `po_number` (`po_number`),
  KEY `supplier_id` (`supplier_id`),
  KEY `idx_po_is_cancelled_supplier` (`is_cancelled`,`supplier_id`),
  CONSTRAINT `purchase_orders_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_orders`
--

LOCK TABLES `purchase_orders` WRITE;
/*!40000 ALTER TABLE `purchase_orders` DISABLE KEYS */;
INSERT INTO `purchase_orders` VALUES (1,1,'PO-20260817-001','2026-08-17','2026-08-17','Received',455850.00,'Source Purchase Indent: IND-20260817-001 (ID 5)','2026-08-17 04:58:47',0,NULL,NULL,0,'Received','Unpaid',NULL),(2,2,'PO-20260819-001','2026-08-19','2026-08-19','Received',5059.00,'Source Purchase Indent: IND-20260819-001 (ID 6)','2026-08-19 01:29:44',0,NULL,NULL,0,'Received','Unpaid',NULL);
/*!40000 ALTER TABLE `purchase_orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_return_items`
--

DROP TABLE IF EXISTS `purchase_return_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_return_items` (
  `purchase_return_item_id` int NOT NULL AUTO_INCREMENT,
  `purchase_return_id` int NOT NULL,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `received_quantity` decimal(18,3) NOT NULL,
  `return_quantity` decimal(18,3) NOT NULL,
  `price` decimal(18,2) NOT NULL,
  `total` decimal(18,2) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`purchase_return_item_id`),
  KEY `fk_purchase_return_items_return` (`purchase_return_id`),
  CONSTRAINT `fk_purchase_return_items_return` FOREIGN KEY (`purchase_return_id`) REFERENCES `purchase_returns` (`purchase_return_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_return_items`
--

LOCK TABLES `purchase_return_items` WRITE;
/*!40000 ALTER TABLE `purchase_return_items` DISABLE KEYS */;
INSERT INTO `purchase_return_items` VALUES (1,1,3,NULL,100.000,10.000,3559.00,35590.00,'2026-08-17 11:04:44'),(2,2,3,NULL,100.000,10.000,3559.00,35590.00,'2026-08-19 12:15:25'),(3,3,1,NULL,50.000,5.000,1500.00,7500.00,'2026-08-21 15:28:46'),(4,4,1,NULL,1.000,1.000,1500.00,1500.00,'2026-08-21 16:47:45');
/*!40000 ALTER TABLE `purchase_return_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_return_items_backup_20260821`
--

DROP TABLE IF EXISTS `purchase_return_items_backup_20260821`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_return_items_backup_20260821` (
  `purchase_return_item_id` int NOT NULL DEFAULT '0',
  `purchase_return_id` int NOT NULL,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `received_quantity` decimal(18,3) NOT NULL,
  `return_quantity` decimal(18,3) NOT NULL,
  `price` decimal(18,2) NOT NULL,
  `total` decimal(18,2) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_return_items_backup_20260821`
--

LOCK TABLES `purchase_return_items_backup_20260821` WRITE;
/*!40000 ALTER TABLE `purchase_return_items_backup_20260821` DISABLE KEYS */;
INSERT INTO `purchase_return_items_backup_20260821` VALUES (1,1,3,NULL,100.000,10.000,3559.00,35590.00,'2026-08-17 11:04:44'),(2,2,3,NULL,100.000,10.000,3559.00,35590.00,'2026-08-19 12:15:25');
/*!40000 ALTER TABLE `purchase_return_items_backup_20260821` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_returns`
--

DROP TABLE IF EXISTS `purchase_returns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_returns` (
  `purchase_return_id` int NOT NULL AUTO_INCREMENT,
  `return_number` varchar(50) NOT NULL,
  `supplier_id` int NOT NULL,
  `grn_id` int NOT NULL,
  `return_date` date NOT NULL,
  `reason` text NOT NULL,
  `total_return_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `status` varchar(30) NOT NULL DEFAULT 'Draft',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`purchase_return_id`),
  UNIQUE KEY `return_number` (`return_number`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_returns`
--

LOCK TABLES `purchase_returns` WRITE;
/*!40000 ALTER TABLE `purchase_returns` DISABLE KEYS */;
INSERT INTO `purchase_returns` VALUES (1,'#PRR-IXPJ46',1,1,'2026-08-17','damaged',35590.00,'Completed','2026-08-17 11:04:44','2026-08-17 16:34:44'),(2,'#PRR-SIC3BV',1,1,'2026-08-19','damaged',35590.00,'Completed','2026-08-19 12:15:25','2026-08-19 17:45:25'),(3,'PR-00003',1,1,'2026-08-21','Defective / Damaged Item',7500.00,'Draft','2026-08-21 15:28:46','2026-08-21 15:28:45'),(4,'PR-00004',2,2,'2026-08-21','Wrong Product Delivered',1500.00,'Draft','2026-08-21 16:47:45','2026-08-21 16:47:44');
/*!40000 ALTER TABLE `purchase_returns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_returns_backup_20260821`
--

DROP TABLE IF EXISTS `purchase_returns_backup_20260821`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_returns_backup_20260821` (
  `purchase_return_id` int NOT NULL DEFAULT '0',
  `return_number` varchar(50) NOT NULL,
  `supplier_id` int NOT NULL,
  `grn_id` int NOT NULL,
  `return_date` date NOT NULL,
  `reason` text NOT NULL,
  `total_return_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `status` varchar(30) NOT NULL DEFAULT 'Draft',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_returns_backup_20260821`
--

LOCK TABLES `purchase_returns_backup_20260821` WRITE;
/*!40000 ALTER TABLE `purchase_returns_backup_20260821` DISABLE KEYS */;
INSERT INTO `purchase_returns_backup_20260821` VALUES (1,'#PRR-IXPJ46',1,1,'2026-08-17','damaged',35590.00,'Completed','2026-08-17 11:04:44','2026-08-17 16:34:44'),(2,'#PRR-SIC3BV',1,1,'2026-08-19','damaged',35590.00,'Completed','2026-08-19 12:15:25','2026-08-19 17:45:25');
/*!40000 ALTER TABLE `purchase_returns_backup_20260821` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `putaway_audits`
--

DROP TABLE IF EXISTS `putaway_audits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `putaway_audits` (
  `putaway_audit_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `warehouse_id` int NOT NULL,
  `rack_id` int NOT NULL,
  `bin_id` int NOT NULL,
  `quantity` decimal(18,2) NOT NULL,
  `user_id` int DEFAULT NULL,
  `user_name` varchar(256) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  PRIMARY KEY (`putaway_audit_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `putaway_audits`
--

LOCK TABLES `putaway_audits` WRITE;
/*!40000 ALTER TABLE `putaway_audits` DISABLE KEYS */;
INSERT INTO `putaway_audits` VALUES (1,3,NULL,2,1,1,100.00,109,'nandhitha','2026-08-17 10:45:21');
/*!40000 ALTER TABLE `putaway_audits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `racks`
--

DROP TABLE IF EXISTS `racks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `racks` (
  `rack_id` int NOT NULL AUTO_INCREMENT,
  `warehouse_id` int DEFAULT NULL,
  `zone_id` int DEFAULT NULL,
  `rack_code` varchar(50) DEFAULT NULL,
  `description` text,
  PRIMARY KEY (`rack_id`),
  KEY `warehouse_id` (`warehouse_id`),
  KEY `zone_id` (`zone_id`),
  CONSTRAINT `racks_ibfk_1` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`),
  CONSTRAINT `racks_ibfk_2` FOREIGN KEY (`zone_id`) REFERENCES `warehouse_zones` (`zone_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `racks`
--

LOCK TABLES `racks` WRITE;
/*!40000 ALTER TABLE `racks` DISABLE KEYS */;
INSERT INTO `racks` VALUES (1,2,NULL,'RACK-A','');
/*!40000 ALTER TABLE `racks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `refresh_tokens`
--

DROP TABLE IF EXISTS `refresh_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `refresh_tokens` (
  `RefreshTokenId` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Token` varchar(500) NOT NULL,
  `ExpiresAt` datetime NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `RevokedAt` datetime DEFAULT NULL,
  `CreatedByIp` varchar(100) DEFAULT NULL,
  `DeviceName` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`RefreshTokenId`),
  KEY `IX_RefreshTokens_UserId` (`UserId`),
  KEY `IX_RefreshTokens_Token` (`Token`),
  CONSTRAINT `FK_RefreshTokens_Users` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=180 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refresh_tokens`
--

LOCK TABLES `refresh_tokens` WRITE;
/*!40000 ALTER TABLE `refresh_tokens` DISABLE KEYS */;
INSERT INTO `refresh_tokens` VALUES (79,44,'46HRsNnbxLrfZCUm0s8RYENFcU6R83hPfZ8d8swt2GYTczez02SSXEGkCRfNaq4uOn775IVvfZUbkW3Yz7BI9A==','2026-08-28 04:00:31','2026-08-21 04:00:31',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(80,44,'AeyHehfj/MOJzmop1U3zfvdiEg8N3PqZiYcXN+qQQVGH/ArGpDRKbTtfhAScUkFzxxAo4Yt/qUfRpCkfJ769sw==','2026-08-28 04:24:36','2026-08-21 04:24:36',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(81,44,'ar3tBAta8WYkHM3NFPlAnKyIhh4qEWWufmYrAR3w9xFyAY73JUeuEZ+siWiKm/gn1WFziEN6myCYHH4gsq5a3g==','2026-08-28 04:29:22','2026-08-21 04:29:22',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(88,44,'3W1z8jwFu+lP3zNfUjRkW0zeNj2oRycx1+IB9xZF9CblyrH8VcCbjsiNzzu5AMRsauoaNnJRQqmeD2O3bejebw==','2026-08-28 07:32:16','2026-08-21 07:32:16',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(89,44,'6Gx7jxghtVtqQDIEeLRCPFTmSoqsUNNJQvF/LPGUpRsIM9r+sTX20/W7a8APACwRCN8xlVAOcjqDNOEkc2Dmww==','2026-08-28 07:33:00','2026-08-21 07:33:00',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(91,44,'Z1Db6SkArZ0UTRhPHfc/ahlJUpkmQ0aVUGngZyCp4AuM2Z0teAknoPpGm1muQwBuV/AmpTcw47bUsJiYRrts9g==','2026-08-28 07:39:12','2026-08-21 07:39:12',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(93,44,'JnWj8DCqKkUegOP90O+cQBJEWCRx+plo5V7bHPM0d2WKC9MqLrzNDEOgnRHl9f0fFccdXagpC4wLYCz1lDZr8w==','2026-08-28 07:41:17','2026-08-21 07:41:17',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(94,44,'EFratCxKEuHnKmXUFstiBZ55bRp26tT1V7ksISJeIQMFxCdvp8B4PDDNjUob81AYItjgK3LhDFAT4DWnpgiIBw==','2026-08-28 07:58:18','2026-08-21 07:58:18',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(95,44,'Fg3J8VJiKTg7/j/XFUD60GiCoA8rNF30neg40q4VzERD/H+bllA5pC9/88QNFMnKXJgb6hpfUc3Q5nNfHxHm0Q==','2026-08-28 08:01:48','2026-08-21 08:01:48',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(96,44,'ucgCKsGBMrdZs0E5kKgh5Fa5K7nv/z2m/JSc6Gx3xNfXg8zEHM9LKzYyyK24zgx3nVs8T0OM9PTHcTbdbqXXyA==','2026-08-28 08:03:55','2026-08-21 08:03:55',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(97,44,'J//J6+d0dPk383kvQPOSmbYlMWFciePyWxGaYDb41peKujiBGzsNY0nziY8qUKSasKACBv38Xw6fH4/sHCJdBA==','2026-08-28 08:12:42','2026-08-21 08:12:42',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(98,44,'X4JlIWN1mGRoAa439NJeK5o5zakBogMEpF8TLK9iruSMPnyJrITmnfRyXwksi2lMzuM7XdUzXJLijG7+kjnJtw==','2026-08-28 08:14:01','2026-08-21 08:14:01',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(101,44,'6Qhqhe4LYSxV5SajIp/mlPxFgNSJOOlj3aa20GYHHjePhadKaeQ8BC06JML+SUgef+AXOIbDB0fhaKY18fOZpQ==','2026-08-28 09:28:37','2026-08-21 09:28:37',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(102,44,'ESTvcgKA7QWSY3ZyYJLCAns2yyA4h0eoOvTZVSauIktyJesO11Kvds3fA+G0vROMLtQ/lqV11drAANYZ5YQrcQ==','2026-08-28 09:29:40','2026-08-21 09:29:40',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(111,44,'fPlaptsrpCaMggJBwswKovO6dPL/ZuQhJnj0CfK88h4rSUPj9dgRVc3q0pJh00n3T/YPf/JXKCNoPfyLMEem2g==','2026-08-31 03:41:23','2026-08-24 03:41:23',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(114,44,'I/bHJJSNl6JNMSoB/tSWLhP5DJka6LyZ+TxLTB3/Vjm4JVBiaH1HkFztYQEqzs83nPAPa2N30IS+8kBjySiNqA==','2026-08-31 03:42:15','2026-08-24 03:42:15',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(127,105,'kUcU35dBsJuf7dlpbUkIuXgKlXGkOReCMFzhVi3cRU4LoNIr98GAG5J9ZAZK2u/F1hKxRedbX9zWsIREHsUYVg==','2026-08-31 05:59:01','2026-08-24 05:59:01',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(133,44,'uQF7XJ1X9zgRFE7bc7wN1X5V/woknCkFb9uN04PgBmlwJnCjceg7i5kaFP1E8AvGQC/B3qV4lpjGcQHHVVDQ8A==','2026-08-31 06:34:46','2026-08-24 06:34:46',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(134,44,'gzzwL0G3t6ukiMza8BcEDh6l3VCK4yn7yjUiM1D8oiTWhesLqbn29wVdee/Ycoa7ZhcB2TkeKg2DPbGkI5p4qA==','2026-08-31 06:35:24','2026-08-24 06:35:24',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(143,126,'MATevRV881WDjuGYoQezRhCj2CYYe3hIkCcSmVRMZqIqWssqx4HS3nJvYnJh/TUq1SqjuhniBCLAUHuix4V94g==','2026-08-31 07:18:04','2026-08-24 07:18:04',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(144,44,'j5uabfshfzIFJoVxHoVtJrhB/671fQyyc2yY8VGYnXkwsAVHtUQy+Fhn2gU3USl5xW0zyyxhe6ERQvrwD7Suog==','2026-09-01 04:12:28','2026-08-25 04:12:28',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(145,44,'JWuI4u5y/gvWzCrbuI3HCBeN2duAkAXDdFn0Dvk7+fZqWM2qbmoFbwJWZjxsG4tKsoEawdcOmOPdYhd8wUxFSA==','2026-09-01 04:13:45','2026-08-25 04:13:45',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(146,44,'ehMn1DPTtPf5pb3QbjJuqHIxC5iF3MDB2ddcGT5g48Ib1DBeyfAeS09By9twG659sH1PYxApgBjpN9GuBE7Uew==','2026-09-01 04:24:56','2026-08-25 04:24:56',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(147,126,'/Zwhw+x353dJr+CVvJkRSDYkA5Y+MZl9a+dgM+gSLfxSFH4UwbJDB8tF/QkaRS/LX/HPRAo2rlJekfsnF6twYw==','2026-09-01 05:04:30','2026-08-25 05:04:30',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(155,127,'HWcRP6/gJeA45ktNKgqsFsfNAG4/0DiFW2No2qLYXZ/tuQ6ASvRCP/a6Jt0uTejsI6vSmcpjn5BFgQMhbgWG7w==','2026-09-01 10:01:41','2026-08-25 10:01:41',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36'),(159,44,'TC4tsWkBU5kUErrIEWsoD5SOykgvjgilYO5wqBDyS/4geLOCwkQYO98o6o5fV20j6hLDdbIDbIMK/crvWqIe3w==','2026-09-02 03:33:32','2026-08-26 03:33:32',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(160,44,'sdAI+Ht46g3sjgY9+g7qBuWUUDMsfnLhufUKvpGko+Wlbi4502avWpv0KKRt/oJZZSF6KZ5pba8Rxp7R1Ef5PQ==','2026-09-02 03:34:30','2026-08-26 03:34:30',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(161,118,'8+YiEDDqWyYhQdVVplcW2xASuK85YqpYylnCQqqq4jIFJTIplZuqPiibEVZDwQXnNL5GgBPYlo1bIiYOC6Um1Q==','2026-09-02 04:03:38','2026-08-26 04:03:38',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(163,105,'269rBDvdVuPrWAVIMPay29XK0sXj4jF2P3oRKGFFrK856Mt5o+3nLHsifvYaPcYr9T4tcIaBmTgFSbQ4YI2dCw==','2026-09-02 04:17:09','2026-08-26 04:17:09',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(165,126,'lCl28Je4Pdk0VFqlcxYQV/LiMeo52VyoUL+kfllJ4CAO0EmMJUZGSZ1jI/Q6i7sNPKuzUsZ/MPJ71BTWOR9EXw==','2026-09-02 04:34:41','2026-08-26 04:34:41',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(166,103,'43/90uFPlJUBj2/NP1zDhkOhP4oDfst4no4MZBluC2mXbdgSY1/nzhB3/9k4Fhn3OA0ps28/KWD40+J1lIW43g==','2026-09-02 04:40:19','2026-08-26 04:40:19',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/152.0.0.0 Safari/537.36'),(167,44,'Ip3fJe6kjU2DIedruZrMBw6mrXPTOIm7jjcVSTkhesqijqhtHsNufvJeJnpYIgMX1hQ13aOaQpljcwc6wfR5HA==','2026-09-02 04:40:48','2026-08-26 04:40:48',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(172,116,'gmqnU1NtayA1P5ByOgb1M1Z6P7TCwxx6RNcE9Ny5DXWuLrMR+7Fs7ZVIZjTOdFFef0ha+RMSldVHRlqHqEsl7w==','2026-09-02 10:47:50','2026-08-26 10:47:50',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(173,44,'wOLdfshROKXmQPPaNgmDUjfiSh3Nb2nnfCYuShi1ggRwCqPCeU1I3+lsuZJTuLhosq0i4bMvzx4zntfyVaz6xQ==','2026-09-02 11:11:11','2026-08-26 11:11:11',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(174,44,'amV2Oy51fTUy/Z1wgOnQNDxc/9K9XSA0WrxwwSrgRl0a459rBnqp3w3gpJl5a1bU9f2ObsaEJCE9eqqzKwEOEg==','2026-09-02 11:11:53','2026-08-26 11:11:53',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(175,44,'omho00OW5+/2dXzvG8sJIib/QHk2f8j1/0Xa7IJ6MKGuXJRXM3nRxm0vh4dGK0W5wGvkDLYITMOzlt+WvtVdmQ==','2026-09-03 03:49:10','2026-08-27 03:49:10',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(176,44,'D+TtJOVNqKfAzpTK6bHZ3Xdwx2ZQPDGyGflaxyKZWZoLNoIcl7Q/VZxFUS0IjbEjzU3sWvbIvK5yVfFtQ9l2Rw==','2026-09-03 03:49:57','2026-08-27 03:49:57',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(177,126,'9OIeCAtODSMTIqwBonQIqfcjKhvTTkDK9XPoHm+rgaD4OpHp+CqJfiNvj8IuguzXpPmLLw1Jh7fXV24bVbwDjg==','2026-09-03 04:37:38','2026-08-27 04:37:38',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(178,118,'8o4EdIDcc1Q+5DlYjMqlpssiIZ3+ds5HTcz6EflV7DwuqlY0VURbIxLygAEuBrZAfQ8SXn3dRuJvuCckxTZUVQ==','2026-09-03 08:40:02','2026-08-27 08:40:02',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0'),(179,116,'vTJIUP3zmy5jvz/3gfVyqjdb0JUtS4/b5k9Uly3fHzUrlJtnCr21Pik9gcNJc1eYSYC/reiBTYtuW5PWGAeAIw==','2026-09-03 09:35:27','2026-08-27 09:35:27',NULL,'::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0');
/*!40000 ALTER TABLE `refresh_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role_permissions`
--

DROP TABLE IF EXISTS `role_permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role_permissions` (
  `PermissionId` int NOT NULL AUTO_INCREMENT,
  `RoleId` int NOT NULL,
  `ModuleId` int NOT NULL,
  `CanView` bit(1) DEFAULT b'0',
  `CanAdd` bit(1) DEFAULT b'0',
  `CanEdit` bit(1) DEFAULT b'0',
  `CanDelete` bit(1) DEFAULT b'0',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`PermissionId`),
  KEY `FK_RolePermissions_Roles` (`RoleId`),
  KEY `FK_RolePermissions_Modules` (`ModuleId`),
  CONSTRAINT `FK_RolePermissions_Modules` FOREIGN KEY (`ModuleId`) REFERENCES `modules` (`ModuleId`) ON DELETE CASCADE,
  CONSTRAINT `FK_RolePermissions_Roles` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`role_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=378 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_permissions`
--

LOCK TABLES `role_permissions` WRITE;
/*!40000 ALTER TABLE `role_permissions` DISABLE KEYS */;
INSERT INTO `role_permissions` VALUES (1,3,30,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(3,5,30,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(4,7,30,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(5,2,30,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(6,3,34,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(8,5,34,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(9,7,34,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(10,2,34,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(11,3,22,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(13,5,22,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(14,7,22,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(15,2,22,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-08-17 15:35:33'),(16,3,5,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(18,5,5,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(19,7,5,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(20,2,5,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-14 19:43:27'),(21,3,3,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(23,5,3,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(24,7,3,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-27 15:15:30'),(25,2,3,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-08-25 16:58:07'),(26,3,25,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(28,5,25,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(29,7,25,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(30,2,25,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(31,3,24,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(33,5,24,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(34,7,24,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(35,2,24,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(36,3,1,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-27 14:51:27'),(38,5,1,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-07 09:58:46'),(39,7,1,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-21 17:32:35'),(40,2,1,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-21 13:26:48'),(41,3,18,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(43,5,18,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(44,7,18,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(45,2,18,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(46,3,21,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(48,5,21,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(49,7,21,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(50,2,21,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(51,3,29,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(53,5,29,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(54,7,29,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(55,2,29,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-08-25 16:58:07'),(56,3,7,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(58,5,7,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(59,7,7,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(60,2,7,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(61,3,2,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(63,5,2,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-08-17 15:35:33'),(64,7,2,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 20:02:47'),(65,2,2,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-21 13:34:45'),(66,3,8,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(68,5,8,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(69,7,8,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(70,2,8,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(71,3,19,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(73,5,19,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(74,7,19,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(75,2,19,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-23 12:35:45'),(76,3,28,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(78,5,28,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(79,7,28,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(80,2,28,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-06 14:51:51'),(81,3,31,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(83,5,31,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(84,7,31,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(85,2,31,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(86,3,33,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(88,5,33,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(89,7,33,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(90,2,33,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(91,3,20,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(93,5,20,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(94,7,20,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(95,2,20,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(96,3,9,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(98,5,9,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(99,7,9,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(100,2,9,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-06 10:31:44'),(101,3,13,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(103,5,13,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(104,7,13,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(105,2,13,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(106,3,12,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(108,5,12,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(109,7,12,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(110,2,12,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(111,3,17,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(113,5,17,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(114,7,17,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(115,2,17,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(116,3,16,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(118,5,16,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(119,7,16,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(120,2,16,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(121,3,11,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(123,5,11,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(124,7,11,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(125,2,11,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(126,3,10,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(128,5,10,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(129,7,10,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(130,2,10,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(131,3,15,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(133,5,15,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(134,7,15,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(135,2,15,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(136,3,14,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(138,5,14,_binary '',_binary '\0',_binary '\0',_binary '','2026-07-04 07:33:52','2026-08-25 16:59:04'),(139,7,14,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(140,2,14,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(141,3,4,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(143,5,4,_binary '',_binary '\0',_binary '\0',_binary '','2026-07-04 07:33:52','2026-08-25 16:59:04'),(144,7,4,_binary '',_binary '\0',_binary '\0',_binary '','2026-07-04 07:33:52','2026-07-21 17:32:35'),(145,2,4,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-09 09:34:24'),(146,3,26,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(148,5,26,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(149,7,26,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(150,2,26,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(151,3,23,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(153,5,23,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(154,7,23,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(155,2,23,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-06 10:29:57'),(156,3,35,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(158,5,35,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(159,7,35,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-27 15:15:49'),(160,2,35,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-05 23:04:30'),(161,3,6,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(163,5,6,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(164,7,6,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(165,2,6,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(166,3,32,_binary '',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-09 10:12:05'),(168,5,32,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(169,7,32,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(170,2,32,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-06 14:51:35'),(171,3,27,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(173,5,27,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(174,7,27,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-07-04 07:33:52','2026-07-04 07:33:52'),(175,2,27,_binary '',_binary '',_binary '',_binary '','2026-07-04 07:33:52','2026-07-04 07:35:37'),(176,2,36,_binary '',_binary '',_binary '',_binary '','2026-08-14 20:37:56','2026-08-17 11:47:06'),(177,2,37,_binary '',_binary '',_binary '',_binary '','2026-08-14 20:37:56','2026-08-17 11:47:06'),(178,2,38,_binary '',_binary '',_binary '',_binary '','2026-08-14 20:37:56','2026-08-17 11:47:06'),(179,3,36,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(180,3,37,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(181,3,38,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(182,5,36,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(183,5,37,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(184,5,38,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(185,7,36,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(186,7,37,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(187,7,38,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(188,18,1,_binary '',_binary '',_binary '',_binary '','2026-08-14 20:37:56','2026-08-20 17:45:30'),(189,18,2,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-20 17:48:16'),(190,18,3,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(191,18,4,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(192,18,5,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(193,18,6,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(194,18,7,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(195,18,8,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(196,18,9,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(197,18,10,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(198,18,11,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(199,18,12,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(200,18,13,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(201,18,14,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(202,18,15,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(203,18,16,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(204,18,17,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(205,18,18,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(206,18,19,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(207,18,20,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-20 17:48:16'),(208,18,21,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(209,18,22,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(210,18,23,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(211,18,24,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(212,18,25,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(213,18,26,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(214,18,27,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(215,18,28,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(216,18,29,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(217,18,30,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(218,18,31,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(219,18,32,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(220,18,33,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(221,18,34,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(222,18,35,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(223,18,36,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(224,18,37,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56'),(225,18,38,_binary '\0',_binary '\0',_binary '\0',_binary '\0','2026-08-14 20:37:56','2026-08-14 20:37:56');
/*!40000 ALTER TABLE `role_permissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role_permissions_backup`
--

DROP TABLE IF EXISTS `role_permissions_backup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role_permissions_backup` (
  `PermissionId` int NOT NULL DEFAULT '0',
  `role_id` int NOT NULL,
  `ModuleName` varchar(100) NOT NULL,
  `ModuleDescription` varchar(255) DEFAULT NULL,
  `CanView` tinyint(1) DEFAULT '0',
  `CanAdd` tinyint(1) DEFAULT '0',
  `CanEdit` tinyint(1) DEFAULT '0',
  `CanDelete` tinyint(1) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_permissions_backup`
--

LOCK TABLES `role_permissions_backup` WRITE;
/*!40000 ALTER TABLE `role_permissions_backup` DISABLE KEYS */;
INSERT INTO `role_permissions_backup` VALUES (1,2,'Dashboard','Dashboard',1,1,1,1),(2,2,'Products','Products',1,1,1,1),(3,2,'Categories','Categories',1,1,1,1),(4,2,'SubCategories','SubCategories',1,1,1,1),(5,2,'Brands','Brands',1,1,1,1),(6,2,'Units','Units',1,1,1,1),(7,2,'Warehouses','Warehouses',1,1,1,1),(8,2,'Suppliers','Suppliers',1,1,1,1),(9,2,'Customers','Customers',1,1,1,1),(10,2,'Invoices','Invoices',1,1,1,1),(11,2,'Reports','Reports',1,1,1,1),(12,2,'Settings','Settings',1,1,1,1),(13,2,'Users','Users',1,1,1,1);
/*!40000 ALTER TABLE `role_permissions_backup` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `role_id` int NOT NULL AUTO_INCREMENT,
  `role_name` varchar(100) NOT NULL,
  `description` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `IsActive` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`role_id`),
  UNIQUE KEY `role_name` (`role_name`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (2,'Admin','System Administrator','2026-06-24 07:52:04',1),(3,'WH Manager','Warehouse Manager manages warehouse operations, inventory, stock movement, receiving, storage, and dispatch to ensure efficient inventory control.','2026-06-26 03:42:31',1),(5,'manager','Manager oversees overall business operations, manages inventory, sales, purchases, payments, reports, and supervises staff to ensure smooth business operations.','2026-07-02 00:46:45',1),(7,'Assistant Manager','Assistant Manager oversees daily inventory operations, manages stock, purchases, sales, payments, and reports while ensuring smooth business operations and team coordination.','2026-07-02 06:07:05',0),(18,'Store Keeper','Manages the stock','2026-08-08 01:51:18',1);
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_order_items`
--

DROP TABLE IF EXISTS `sales_order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_order_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `so_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  `delivered_quantity` decimal(10,2) DEFAULT '0.00',
  `price` decimal(10,2) DEFAULT NULL,
  `total` decimal(12,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `so_id` (`so_id`),
  KEY `product_id` (`product_id`),
  KEY `variant_id` (`variant_id`),
  CONSTRAINT `sales_order_items_ibfk_1` FOREIGN KEY (`so_id`) REFERENCES `sales_orders` (`so_id`),
  CONSTRAINT `sales_order_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `sales_order_items_ibfk_3` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_order_items`
--

LOCK TABLES `sales_order_items` WRITE;
/*!40000 ALTER TABLE `sales_order_items` DISABLE KEYS */;
/*!40000 ALTER TABLE `sales_order_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_orders`
--

DROP TABLE IF EXISTS `sales_orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_orders` (
  `so_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int DEFAULT NULL,
  `so_number` varchar(100) DEFAULT NULL,
  `order_date` date DEFAULT NULL,
  `status` enum('draft','confirmed','shipped','delivered','cancelled') DEFAULT 'draft',
  `total_amount` decimal(12,2) DEFAULT NULL,
  `notes` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`so_id`),
  UNIQUE KEY `so_number` (`so_number`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `sales_orders_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_orders`
--

LOCK TABLES `sales_orders` WRITE;
/*!40000 ALTER TABLE `sales_orders` DISABLE KEYS */;
/*!40000 ALTER TABLE `sales_orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_return_items`
--

DROP TABLE IF EXISTS `sales_return_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_return_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `sales_return_id` int NOT NULL,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `invoiced_quantity` decimal(18,3) NOT NULL,
  `return_quantity` decimal(18,3) NOT NULL,
  `price` decimal(18,2) NOT NULL,
  `tax` decimal(18,2) NOT NULL DEFAULT '0.00',
  `tax_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `discount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `total` decimal(18,2) NOT NULL DEFAULT '0.00',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `FK_SalesReturnItems_SalesReturns` (`sales_return_id`),
  CONSTRAINT `FK_SalesReturnItems_SalesReturns` FOREIGN KEY (`sales_return_id`) REFERENCES `sales_returns` (`return_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_return_items`
--

LOCK TABLES `sales_return_items` WRITE;
/*!40000 ALTER TABLE `sales_return_items` DISABLE KEYS */;
INSERT INTO `sales_return_items` VALUES (1,1,3,NULL,1.000,1.000,5000.00,0.00,0.00,0.00,5000.00,'2026-08-19 09:11:57'),(2,5,3,NULL,10.000,1.000,5000.00,18.00,900.00,0.00,5900.00,'2026-08-21 09:58:14');
/*!40000 ALTER TABLE `sales_return_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_return_items_backup`
--

DROP TABLE IF EXISTS `sales_return_items_backup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_return_items_backup` (
  `id` int NOT NULL DEFAULT '0',
  `return_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_return_items_backup`
--

LOCK TABLES `sales_return_items_backup` WRITE;
/*!40000 ALTER TABLE `sales_return_items_backup` DISABLE KEYS */;
/*!40000 ALTER TABLE `sales_return_items_backup` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_return_items_backup_20260821`
--

DROP TABLE IF EXISTS `sales_return_items_backup_20260821`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_return_items_backup_20260821` (
  `SalesReturnItemId` int NOT NULL DEFAULT '0',
  `SalesReturnId` int NOT NULL,
  `ProductId` int NOT NULL,
  `VariantId` int DEFAULT NULL,
  `InvoicedQuantity` decimal(18,3) NOT NULL,
  `ReturnQuantity` decimal(18,3) NOT NULL,
  `Price` decimal(18,2) NOT NULL,
  `Total` decimal(18,2) NOT NULL DEFAULT '0.00',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_return_items_backup_20260821`
--

LOCK TABLES `sales_return_items_backup_20260821` WRITE;
/*!40000 ALTER TABLE `sales_return_items_backup_20260821` DISABLE KEYS */;
INSERT INTO `sales_return_items_backup_20260821` VALUES (1,1,3,NULL,1.000,1.000,5000.00,5000.00,'2026-08-19 09:11:57');
/*!40000 ALTER TABLE `sales_return_items_backup_20260821` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_returns`
--

DROP TABLE IF EXISTS `sales_returns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_returns` (
  `return_id` int NOT NULL AUTO_INCREMENT,
  `return_number` varchar(50) NOT NULL,
  `customer_id` int NOT NULL,
  `warehouse_id` int DEFAULT NULL,
  `invoice_id` int NOT NULL,
  `return_date` date NOT NULL,
  `reason` text NOT NULL,
  `rejection_reason` text,
  `approved_by` varchar(128) DEFAULT NULL,
  `approved_at` datetime DEFAULT NULL,
  `refund_method` varchar(64) DEFAULT NULL,
  `refund_reference` varchar(128) DEFAULT NULL,
  `refund_date` datetime DEFAULT NULL,
  `notes` text,
  `total_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `tax_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `discount_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `grand_total` decimal(18,2) NOT NULL DEFAULT '0.00',
  `refund_amount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `status` varchar(30) NOT NULL DEFAULT 'Draft',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`return_id`),
  UNIQUE KEY `ReturnNumber` (`return_number`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_returns`
--

LOCK TABLES `sales_returns` WRITE;
/*!40000 ALTER TABLE `sales_returns` DISABLE KEYS */;
INSERT INTO `sales_returns` VALUES (1,'SRR-1001',3,NULL,2,'2026-08-19','Defective / Damaged Item',NULL,NULL,NULL,NULL,NULL,NULL,NULL,5000.00,0.00,0.00,5000.00,0.00,'Draft','2026-08-19 09:11:57',NULL,0),(5,'SRET-000005',1,2,1,'2026-08-21','Wrong Product Received',NULL,NULL,NULL,NULL,NULL,NULL,'',5000.00,900.00,0.00,5900.00,5900.00,'Pending Approval','2026-08-21 09:58:14','2026-08-21 09:58:14',0);
/*!40000 ALTER TABLE `sales_returns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_returns_backup`
--

DROP TABLE IF EXISTS `sales_returns_backup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_returns_backup` (
  `return_id` int NOT NULL DEFAULT '0',
  `invoice_id` int DEFAULT NULL,
  `customer_id` int DEFAULT NULL,
  `return_date` datetime DEFAULT NULL,
  `total_amount` decimal(12,2) DEFAULT NULL,
  `reason` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_returns_backup`
--

LOCK TABLES `sales_returns_backup` WRITE;
/*!40000 ALTER TABLE `sales_returns_backup` DISABLE KEYS */;
/*!40000 ALTER TABLE `sales_returns_backup` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_returns_backup_20260821`
--

DROP TABLE IF EXISTS `sales_returns_backup_20260821`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales_returns_backup_20260821` (
  `SalesReturnId` int NOT NULL DEFAULT '0',
  `ReturnNumber` varchar(50) NOT NULL,
  `CustomerId` int NOT NULL,
  `InvoiceId` int NOT NULL,
  `ReturnDate` date NOT NULL,
  `Reason` text NOT NULL,
  `TotalReturnAmount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `Status` varchar(30) NOT NULL DEFAULT 'Draft',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_returns_backup_20260821`
--

LOCK TABLES `sales_returns_backup_20260821` WRITE;
/*!40000 ALTER TABLE `sales_returns_backup_20260821` DISABLE KEYS */;
INSERT INTO `sales_returns_backup_20260821` VALUES (1,'SRR-1001',3,2,'2026-08-19','Defective / Damaged Item',5000.00,'Draft','2026-08-19 09:11:57',NULL);
/*!40000 ALTER TABLE `sales_returns_backup_20260821` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `settings`
--

DROP TABLE IF EXISTS `settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `settings` (
  `SettingId` int NOT NULL AUTO_INCREMENT,
  `CompanyName` varchar(150) DEFAULT NULL,
  `CompanyLogo` varchar(255) DEFAULT NULL,
  `EmailAddress` varchar(150) DEFAULT NULL,
  `PhoneNumber` varchar(20) DEFAULT NULL,
  `Address` text,
  `LowStockAlertLimit` int DEFAULT '10',
  `DefaultUnitType` varchar(50) DEFAULT 'Pieces',
  `BarcodeManagement` tinyint(1) DEFAULT '1',
  `AutoStockUpdate` tinyint(1) DEFAULT '1',
  `LowStockAlerts` tinyint(1) DEFAULT '0',
  `OrderNotifications` tinyint(1) DEFAULT '0',
  `SupplierPaymentReminder` tinyint(1) DEFAULT '0',
  `TwoStepVerification` tinyint(1) DEFAULT '0',
  `ThemeMode` varchar(50) DEFAULT 'Light',
  `UpdatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `Language` varchar(50) DEFAULT 'English',
  `CollapseSidebar` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`SettingId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `settings`
--

LOCK TABLES `settings` WRITE;
/*!40000 ALTER TABLE `settings` DISABLE KEYS */;
/*!40000 ALTER TABLE `settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock`
--

DROP TABLE IF EXISTS `stock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock` (
  `stock_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT '0.00',
  `reserved_quantity` decimal(10,2) DEFAULT '0.00',
  `available_quantity` decimal(10,2) GENERATED ALWAYS AS ((`quantity` - `reserved_quantity`)) STORED,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`stock_id`),
  UNIQUE KEY `product_id` (`product_id`,`variant_id`,`warehouse_id`),
  KEY `variant_id` (`variant_id`),
  KEY `warehouse_id` (`warehouse_id`),
  KEY `idx_stock_product_quantity` (`product_id`,`quantity`),
  CONSTRAINT `stock_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `stock_ibfk_2` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`),
  CONSTRAINT `stock_ibfk_3` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock`
--

LOCK TABLES `stock` WRITE;
/*!40000 ALTER TABLE `stock` DISABLE KEYS */;
INSERT INTO `stock` (`stock_id`, `product_id`, `variant_id`, `warehouse_id`, `quantity`, `reserved_quantity`, `is_deleted`, `created_at`, `updated_at`) VALUES (1,3,NULL,2,69.00,0.00,0,NULL,NULL),(2,2,NULL,2,50.00,0.00,0,NULL,NULL),(3,1,NULL,2,51.00,0.00,0,NULL,NULL),(4,5,5,2,1.00,0.00,0,NULL,NULL),(5,3,NULL,1,1.00,0.00,0,NULL,NULL);
/*!40000 ALTER TABLE `stock` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_adjustment_items`
--

DROP TABLE IF EXISTS `stock_adjustment_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_adjustment_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `adjustment_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `adjustment_id` (`adjustment_id`),
  KEY `product_id` (`product_id`),
  KEY `variant_id` (`variant_id`),
  CONSTRAINT `stock_adjustment_items_ibfk_1` FOREIGN KEY (`adjustment_id`) REFERENCES `stock_adjustments` (`adjustment_id`),
  CONSTRAINT `stock_adjustment_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `stock_adjustment_items_ibfk_3` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_adjustment_items`
--

LOCK TABLES `stock_adjustment_items` WRITE;
/*!40000 ALTER TABLE `stock_adjustment_items` DISABLE KEYS */;
INSERT INTO `stock_adjustment_items` VALUES (1,1,5,5,1.00);
/*!40000 ALTER TABLE `stock_adjustment_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_adjustments`
--

DROP TABLE IF EXISTS `stock_adjustments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_adjustments` (
  `adjustment_id` int NOT NULL AUTO_INCREMENT,
  `warehouse_id` int DEFAULT NULL,
  `adjustment_type` enum('increase','decrease') DEFAULT NULL,
  `reason` varchar(255) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`adjustment_id`),
  KEY `warehouse_id` (`warehouse_id`),
  CONSTRAINT `stock_adjustments_ibfk_1` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_adjustments`
--

LOCK TABLES `stock_adjustments` WRITE;
/*!40000 ALTER TABLE `stock_adjustments` DISABLE KEYS */;
INSERT INTO `stock_adjustments` VALUES (1,2,'increase','10 items','2026-08-19 12:28:30');
/*!40000 ALTER TABLE `stock_adjustments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_audit_items`
--

DROP TABLE IF EXISTS `stock_audit_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_audit_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `audit_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `bin_id` int DEFAULT NULL,
  `system_quantity` decimal(10,2) DEFAULT NULL,
  `physical_quantity` decimal(10,2) DEFAULT NULL,
  `difference` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `audit_id` (`audit_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `stock_audit_items_ibfk_1` FOREIGN KEY (`audit_id`) REFERENCES `stock_audits` (`audit_id`),
  CONSTRAINT `stock_audit_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_audit_items`
--

LOCK TABLES `stock_audit_items` WRITE;
/*!40000 ALTER TABLE `stock_audit_items` DISABLE KEYS */;
/*!40000 ALTER TABLE `stock_audit_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_audits`
--

DROP TABLE IF EXISTS `stock_audits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_audits` (
  `audit_id` int NOT NULL AUTO_INCREMENT,
  `warehouse_id` int DEFAULT NULL,
  `audit_date` date DEFAULT NULL,
  `audit_type` enum('Cycle Count','Full Audit','Spot Check') DEFAULT NULL,
  `status` enum('Draft','Pending','Approved','Posted','Cancelled') DEFAULT NULL,
  `created_by` varchar(255) DEFAULT NULL,
  `approved_by` varchar(255) DEFAULT NULL,
  `notes` text,
  PRIMARY KEY (`audit_id`),
  KEY `warehouse_id` (`warehouse_id`),
  CONSTRAINT `stock_audits_ibfk_1` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_audits`
--

LOCK TABLES `stock_audits` WRITE;
/*!40000 ALTER TABLE `stock_audits` DISABLE KEYS */;
INSERT INTO `stock_audits` VALUES (1,2,'2026-08-17','Cycle Count','Approved','','','');
/*!40000 ALTER TABLE `stock_audits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_ledger`
--

DROP TABLE IF EXISTS `stock_ledger`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_ledger` (
  `ledger_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `opening_qty` decimal(10,2) DEFAULT NULL,
  `change_qty` decimal(10,2) DEFAULT NULL,
  `closing_qty` decimal(10,2) DEFAULT NULL,
  `transaction_type` varchar(50) DEFAULT NULL,
  `transaction_id` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `is_cancelled` tinyint(1) NOT NULL DEFAULT '0',
  `cancelled_at` datetime DEFAULT NULL,
  `cancellation_reason` varchar(255) DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`ledger_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_ledger`
--

LOCK TABLES `stock_ledger` WRITE;
/*!40000 ALTER TABLE `stock_ledger` DISABLE KEYS */;
INSERT INTO `stock_ledger` VALUES (1,3,NULL,2,90.00,-10.00,80.00,'sale',1,'2026-08-17 05:45:27',0,NULL,NULL,0),(2,3,NULL,2,80.00,-1.00,79.00,'sale',2,'2026-08-19 00:30:56',0,NULL,NULL,0),(3,5,5,2,0.00,1.00,1.00,'ADJUSTMENT_IN',1,'2026-08-19 06:58:30',0,NULL,NULL,0),(4,3,NULL,2,70.00,-1.00,69.00,'Warehouse Transfer Out',1,'2026-08-19 06:59:24',0,NULL,NULL,0),(5,3,NULL,1,0.00,1.00,1.00,'Warehouse Transfer In',1,'2026-08-19 06:59:24',0,NULL,NULL,0);
/*!40000 ALTER TABLE `stock_ledger` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_movements`
--

DROP TABLE IF EXISTS `stock_movements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_movements` (
  `movement_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `warehouse_id` int DEFAULT NULL,
  `movement_type` varchar(50) DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  `reference_id` int DEFAULT NULL,
  `reference_type` varchar(50) DEFAULT NULL,
  `notes` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `is_cancelled` tinyint(1) NOT NULL DEFAULT '0',
  `cancelled_at` datetime DEFAULT NULL,
  `cancellation_reason` varchar(255) DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`movement_id`),
  KEY `product_id` (`product_id`),
  KEY `variant_id` (`variant_id`),
  KEY `warehouse_id` (`warehouse_id`),
  CONSTRAINT `stock_movements_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `stock_movements_ibfk_2` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`),
  CONSTRAINT `stock_movements_ibfk_3` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_movements`
--

LOCK TABLES `stock_movements` WRITE;
/*!40000 ALTER TABLE `stock_movements` DISABLE KEYS */;
INSERT INTO `stock_movements` VALUES (1,3,NULL,2,'PURCHASE',100.00,1,'goods_receipt','Stock added from goods receipt','2026-08-17 05:01:18',0,NULL,NULL,0),(2,2,NULL,2,'PURCHASE',50.00,1,'goods_receipt','Stock added from goods receipt','2026-08-17 05:01:18',0,NULL,NULL,0),(3,1,NULL,2,'PURCHASE',50.00,1,'goods_receipt','Stock added from goods receipt','2026-08-17 05:01:18',0,NULL,NULL,0),(4,3,NULL,2,'Putaway',100.00,NULL,'putaway','Putaway Product 3, Variant  to Rack RACK-A / Bin BIN-A1','2026-08-17 05:15:21',0,NULL,NULL,0),(5,3,NULL,2,'PURCHASE_RETURN',10.00,1,'purchase_return','Stock reduced for purchase return #PRR-IXPJ46','2026-08-17 05:34:44',0,NULL,NULL,0),(6,3,NULL,2,'sale',10.00,1,'invoice','Stock deducted for invoice INV-20260817-001','2026-08-17 05:45:27',0,NULL,NULL,0),(7,3,NULL,2,'sale',1.00,2,'invoice','Stock deducted for invoice INV-20260819-001','2026-08-19 00:30:56',0,NULL,NULL,0),(8,1,NULL,2,'PURCHASE',1.00,2,'goods_receipt','Stock added from goods receipt','2026-08-19 01:30:42',0,NULL,NULL,0),(9,3,NULL,2,'PURCHASE',1.00,2,'goods_receipt','Stock added from goods receipt','2026-08-19 01:30:42',0,NULL,NULL,0),(10,3,NULL,2,'PURCHASE_RETURN',10.00,2,'purchase_return','Stock reduced for purchase return #PRR-SIC3BV','2026-08-19 06:45:25',0,NULL,NULL,0),(11,5,5,2,'ADJUSTMENT',1.00,1,'adjustment_in','10 items','2026-08-19 06:58:30',0,NULL,NULL,0),(12,3,NULL,2,'Transfer Out',1.00,1,'Warehouse Transfer','Transferred to Warehouse 1. Picked from source bins: Bin 1: 1','2026-08-19 06:59:24',0,NULL,NULL,0),(13,3,NULL,1,'Transfer In',1.00,1,'Warehouse Transfer','Received from Warehouse 2 as unallocated stock pending putaway','2026-08-19 06:59:24',0,NULL,NULL,0);
/*!40000 ALTER TABLE `stock_movements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_transfer_items`
--

DROP TABLE IF EXISTS `stock_transfer_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_transfer_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `transfer_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `transfer_id` (`transfer_id`),
  KEY `product_id` (`product_id`),
  KEY `variant_id` (`variant_id`),
  CONSTRAINT `stock_transfer_items_ibfk_1` FOREIGN KEY (`transfer_id`) REFERENCES `stock_transfers` (`transfer_id`),
  CONSTRAINT `stock_transfer_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `stock_transfer_items_ibfk_3` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_transfer_items`
--

LOCK TABLES `stock_transfer_items` WRITE;
/*!40000 ALTER TABLE `stock_transfer_items` DISABLE KEYS */;
INSERT INTO `stock_transfer_items` VALUES (1,1,3,NULL,1.00);
/*!40000 ALTER TABLE `stock_transfer_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_transfers`
--

DROP TABLE IF EXISTS `stock_transfers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_transfers` (
  `transfer_id` int NOT NULL AUTO_INCREMENT,
  `from_warehouse_id` int DEFAULT NULL,
  `to_warehouse_id` int DEFAULT NULL,
  `transfer_date` datetime DEFAULT NULL,
  `status` enum('pending','completed','cancelled') DEFAULT 'pending',
  PRIMARY KEY (`transfer_id`),
  KEY `from_warehouse_id` (`from_warehouse_id`),
  KEY `to_warehouse_id` (`to_warehouse_id`),
  CONSTRAINT `stock_transfers_ibfk_1` FOREIGN KEY (`from_warehouse_id`) REFERENCES `warehouses` (`warehouse_id`),
  CONSTRAINT `stock_transfers_ibfk_2` FOREIGN KEY (`to_warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_transfers`
--

LOCK TABLES `stock_transfers` WRITE;
/*!40000 ALTER TABLE `stock_transfers` DISABLE KEYS */;
INSERT INTO `stock_transfers` VALUES (1,2,1,'2026-08-19 00:00:00','pending');
/*!40000 ALTER TABLE `stock_transfers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sub_categories`
--

DROP TABLE IF EXISTS `sub_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sub_categories` (
  `sub_category_id` int NOT NULL AUTO_INCREMENT,
  `category_id` int NOT NULL,
  `name` varchar(150) NOT NULL,
  `description` text,
  `status` varchar(50) DEFAULT 'active',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`sub_category_id`),
  KEY `fk_sub_category_category` (`category_id`),
  CONSTRAINT `fk_sub_category_category` FOREIGN KEY (`category_id`) REFERENCES `categories` (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sub_categories`
--

LOCK TABLES `sub_categories` WRITE;
/*!40000 ALTER TABLE `sub_categories` DISABLE KEYS */;
INSERT INTO `sub_categories` VALUES (1,1,'Garden Tools','',NULL,'2026-08-13 12:03:24',0),(2,3,'Special Farm Tools','',NULL,'2026-08-13 12:03:51',0),(3,1,'Domestic Pumps','',NULL,'2026-08-13 12:52:00',0),(4,2,'Tiles And Marbles','',NULL,'2026-08-27 09:56:29',0);
/*!40000 ALTER TABLE `sub_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_addresses`
--

DROP TABLE IF EXISTS `supplier_addresses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_addresses` (
  `address_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `address_type` enum('billing','shipping','office') DEFAULT 'office',
  `address_line` text,
  `city` varchar(100) DEFAULT NULL,
  `state` varchar(100) DEFAULT NULL,
  `country` varchar(100) DEFAULT NULL,
  `pincode` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`address_id`),
  KEY `supplier_id` (`supplier_id`),
  CONSTRAINT `supplier_addresses_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_addresses`
--

LOCK TABLES `supplier_addresses` WRITE;
/*!40000 ALTER TABLE `supplier_addresses` DISABLE KEYS */;
INSERT INTO `supplier_addresses` VALUES (1,1,'billing','sssss','Kkkkk','Andhra Pradesh','India',''),(2,1,'billing','Ratnalakunta','Eluru','Andhra Pradesh','India','534475'),(3,2,'billing','yadigirigutta','Yadigirgutta','Telangana','India',''),(4,3,'billing','Kukatpally','Hyderabad','Telangana','India','500074');
/*!40000 ALTER TABLE `supplier_addresses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_bank_details`
--

DROP TABLE IF EXISTS `supplier_bank_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_bank_details` (
  `bank_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `account_name` varchar(150) DEFAULT NULL,
  `account_number` varchar(50) DEFAULT NULL,
  `bank_name` varchar(150) DEFAULT NULL,
  `ifsc_code` varchar(20) DEFAULT NULL,
  `branch` varchar(100) DEFAULT NULL,
  `bank_state` varchar(100) DEFAULT NULL,
  `bank_city` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`bank_id`),
  KEY `supplier_id` (`supplier_id`),
  CONSTRAINT `supplier_bank_details_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_bank_details`
--

LOCK TABLES `supplier_bank_details` WRITE;
/*!40000 ALTER TABLE `supplier_bank_details` DISABLE KEYS */;
INSERT INTO `supplier_bank_details` VALUES (1,1,'Ststsss','20278218328','State Bank of India','SBIN0008987','CHATLI','MADHYA PRADESH','BADWANI');
/*!40000 ALTER TABLE `supplier_bank_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_contacts`
--

DROP TABLE IF EXISTS `supplier_contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_contacts` (
  `contact_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `name` varchar(150) DEFAULT NULL,
  `designation` varchar(100) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT '0',
  `department` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`contact_id`),
  KEY `supplier_id` (`supplier_id`),
  CONSTRAINT `supplier_contacts_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_contacts`
--

LOCK TABLES `supplier_contacts` WRITE;
/*!40000 ALTER TABLE `supplier_contacts` DISABLE KEYS */;
INSERT INTO `supplier_contacts` VALUES (3,3,'Ingco','','9084710199','indiaservice@ingco.com',1,''),(4,4,'KPT Industries','','9876543210','sales@kptindustries.com',1,''),(5,5,'Bharat Agricultural Tools Pvt Ltd','','9845378765','sales@bharatagritools.in',1,''),(12,3,'Kiran','','9786754345','info@shaktiagro.in',1,''),(13,1,'Raju','','9986767543','',1,''),(14,2,'Jai Jawaan','','8186898399','prasadkumar.madiga@pirnav.com',1,''),(15,1,'Universal Farm Equipment Pvtltd','','9878666789','support@universalfarm.in',1,''),(16,2,'Shakti Agro Implements Pvtltd','','7688999837','info@shaktiagro.in',1,''),(17,3,'Greenfield Farm Equipment Pvtltd','','9998766772','support@greenfieldfarm.in',1,'');
/*!40000 ALTER TABLE `supplier_contacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_documents`
--

DROP TABLE IF EXISTS `supplier_documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_documents` (
  `document_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `display_name` varchar(150) DEFAULT NULL,
  `file_path` varchar(255) DEFAULT NULL,
  `uploaded_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `document_type` varchar(50) DEFAULT NULL,
  `original_file_name` varchar(255) DEFAULT NULL,
  `stored_file_name` varchar(255) DEFAULT NULL,
  `content_type` varchar(100) DEFAULT NULL,
  `file_size_bytes` bigint DEFAULT NULL,
  `status` varchar(50) DEFAULT 'uploaded',
  `is_deleted` tinyint(1) DEFAULT '0',
  `deleted_at` timestamp NULL DEFAULT NULL,
  `is_temporary` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`document_id`),
  KEY `supplier_id` (`supplier_id`),
  KEY `idx_supplier_documents_supplier` (`supplier_id`),
  CONSTRAINT `supplier_documents_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_documents`
--

LOCK TABLES `supplier_documents` WRITE;
/*!40000 ALTER TABLE `supplier_documents` DISABLE KEYS */;
/*!40000 ALTER TABLE `supplier_documents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_payment_terms`
--

DROP TABLE IF EXISTS `supplier_payment_terms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_payment_terms` (
  `term_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `credit_days` int DEFAULT '0',
  `credit_limit` decimal(12,2) DEFAULT '0.00',
  `payment_method` varchar(50) DEFAULT NULL,
  `notes` text,
  PRIMARY KEY (`term_id`),
  KEY `supplier_id` (`supplier_id`),
  CONSTRAINT `supplier_payment_terms_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_payment_terms`
--

LOCK TABLES `supplier_payment_terms` WRITE;
/*!40000 ALTER TABLE `supplier_payment_terms` DISABLE KEYS */;
INSERT INTO `supplier_payment_terms` VALUES (3,3,30,NULL,'Bank Transfer',''),(4,4,30,NULL,'Bank Transfer',''),(5,5,30,NULL,'Bank Transfer',''),(12,3,30,NULL,'Bank Transfer',''),(13,1,30,NULL,'Bank Transfer',''),(14,2,30,NULL,'Bank Transfer',''),(15,1,30,NULL,'Bank Transfer',''),(16,2,30,NULL,'Bank Transfer',''),(17,3,30,NULL,'Bank Transfer','');
/*!40000 ALTER TABLE `supplier_payment_terms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_payments`
--

DROP TABLE IF EXISTS `supplier_payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_payments` (
  `payment_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `po_id` int DEFAULT NULL,
  `amount` decimal(12,2) DEFAULT NULL,
  `payment_date` datetime DEFAULT NULL,
  `payment_method` varchar(50) DEFAULT NULL,
  `reference_number` varchar(100) DEFAULT NULL,
  `notes` text,
  `is_cancelled` tinyint(1) NOT NULL DEFAULT '0',
  `cancelled_at` datetime DEFAULT NULL,
  `cancellation_reason` text,
  PRIMARY KEY (`payment_id`),
  KEY `supplier_id` (`supplier_id`),
  KEY `po_id` (`po_id`),
  CONSTRAINT `supplier_payments_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`),
  CONSTRAINT `supplier_payments_ibfk_2` FOREIGN KEY (`po_id`) REFERENCES `purchase_orders` (`po_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_payments`
--

LOCK TABLES `supplier_payments` WRITE;
/*!40000 ALTER TABLE `supplier_payments` DISABLE KEYS */;
/*!40000 ALTER TABLE `supplier_payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier_performance`
--

DROP TABLE IF EXISTS `supplier_performance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier_performance` (
  `performance_id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `total_orders` int DEFAULT '0',
  `on_time_deliveries` int DEFAULT '0',
  `delayed_deliveries` int DEFAULT '0',
  `rating` decimal(3,2) DEFAULT NULL,
  `last_updated` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`performance_id`),
  KEY `supplier_id` (`supplier_id`),
  CONSTRAINT `supplier_performance_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier_performance`
--

LOCK TABLES `supplier_performance` WRITE;
/*!40000 ALTER TABLE `supplier_performance` DISABLE KEYS */;
/*!40000 ALTER TABLE `supplier_performance` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `supplier_id` int NOT NULL AUTO_INCREMENT,
  `supplier_code` varchar(50) DEFAULT NULL,
  `name` varchar(255) NOT NULL,
  `gst_number` varchar(50) DEFAULT NULL,
  `pan_number` varchar(20) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `website` varchar(150) DEFAULT NULL,
  `status` varchar(20) NOT NULL DEFAULT 'active',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `category` varchar(100) DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`supplier_id`),
  UNIQUE KEY `supplier_code` (`supplier_code`),
  KEY `idx_suppliers_is_deleted` (`is_deleted`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers` VALUES (1,'SUP-007','Universal Farm Equipment Pvt. Ltd.','','','9878666789','support@universalfarm.in','','active','2026-08-13 06:48:12',NULL,'Farm and Garden',0,NULL),(2,'SUP-002','Shakti Agro Implements Pvt. Ltd.','','','7688999837','info@shaktiagro.in','','active','2026-08-13 06:49:28',NULL,'Farm and Garden',0,NULL),(3,'SUP-003','Greenfield Farm Equipment Pvt. Ltd','','','9998766772','support@greenfieldfarm.in','','active','2026-08-13 06:51:00',NULL,'Pumps and Motors',0,NULL);
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `system_setting_rules`
--

DROP TABLE IF EXISTS `system_setting_rules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_setting_rules` (
  `rule_id` int NOT NULL AUTO_INCREMENT,
  `section_id` int NOT NULL,
  `rule_key` varchar(150) NOT NULL,
  `rule_name` varchar(200) NOT NULL,
  `rule_description` varchar(500) DEFAULT NULL,
  `rule_type` varchar(50) NOT NULL,
  `rule_value` varchar(500) DEFAULT NULL,
  `default_value` varchar(500) DEFAULT NULL,
  `is_enabled` tinyint(1) DEFAULT '1',
  `display_order` int NOT NULL,
  `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`rule_id`),
  UNIQUE KEY `rule_key` (`rule_key`),
  KEY `section_id` (`section_id`),
  CONSTRAINT `system_setting_rules_ibfk_1` FOREIGN KEY (`section_id`) REFERENCES `system_setting_sections` (`section_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=102 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `system_setting_rules`
--

LOCK TABLES `system_setting_rules` WRITE;
/*!40000 ALTER TABLE `system_setting_rules` DISABLE KEYS */;
INSERT INTO `system_setting_rules` VALUES (1,1,'auto_generate_product_code','Auto Generate Product Code','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,1,'2026-07-27 15:03:02'),(2,1,'product_code_prefix','Product Code Prefix','Enter the default text value used for this rule.','text','PRDCT-','PRD-',1,2,'2026-07-27 15:03:02'),(3,1,'sku_prefix','SKU Prefix','Enter the default text value used for this rule.','text','NAD-','SKU-',1,3,'2026-07-27 15:03:02'),(4,1,'allow_product_variants','Allow Product Variants','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',1,4,'2026-07-27 15:03:02'),(5,1,'brand_required','Brand Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,5,'2026-07-27 15:03:02'),(6,1,'category_required','Category Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,6,'2026-07-27 15:03:02'),(7,1,'subcategory_required','SubCategory Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,7,'2026-07-27 15:03:02'),(8,1,'attribute_required_for_variants','Attribute Required For Variants','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',1,8,'2026-07-27 15:03:02'),(9,1,'hsn_code_required','HSN Code Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,9,'2026-07-27 15:03:02'),(10,1,'product_image_required','Product Image Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,10,'2026-07-27 15:03:02'),(11,1,'duplicate_product_name_allowed','Duplicate Product Name Allowed','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,11,'2026-07-27 15:03:02'),(12,2,'purchase_order_prefix','Purchase Order Prefix','Enter the default text value used for this rule.','text','PO-','PO-',1,1,'2026-08-10 12:46:07'),(13,2,'auto_generate_po_number','Auto Generate PO Number','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,2,'2026-08-10 12:46:07'),(14,2,'default_purchase_status','Default Purchase Status','Choose the default option used by this module.','dropdown','Pending','Pending',1,3,'2026-08-10 12:46:07'),(15,2,'purchase_approval_required','Purchase Approval Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,4,'2026-08-10 12:46:07'),(16,2,'supplier_approval_required','Supplier Approval Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,5,'2026-08-10 12:46:07'),(17,2,'goods_receipt_approval_required','Goods Receipt Approval Required','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,6,'2026-08-10 12:46:07'),(18,2,'allow_partial_goods_receipt','Allow Partial Goods Receipt','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,7,'2026-08-10 12:46:07'),(19,2,'allow_purchase_without_supplier','Allow Purchase Without Supplier','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,8,'2026-08-10 12:46:07'),(20,2,'allow_purchase_price_override','Allow Purchase Price Override','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',0,9,'2026-08-10 12:46:07'),(21,2,'require_attachment_for_purchase','Require Attachment For Purchase','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,10,'2026-08-10 12:46:07'),(22,3,'invoice_prefix','Invoice Prefix','Enter the default text value used for this rule.','text','INV-','INV-',1,1,'2026-07-16 14:30:36'),(23,3,'auto_generate_invoice_number','Auto Generate Invoice Number','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',1,2,'2026-07-16 14:30:36'),(24,3,'default_sales_status','Default Sales Status','Choose the default option used by this module.','dropdown','Pending','Pending',1,3,'2026-07-16 14:30:36'),(25,3,'default_payment_status','Default Payment Status','Choose the default option used by this module.','dropdown','Unpaid','Unpaid',1,4,'2026-07-16 14:30:36'),(26,3,'default_payment_terms','Default Payment Terms','Enter the default text value used for this rule.','text','Immediate','Immediate',1,5,'2026-07-16 14:30:36'),(27,3,'require_customer_for_sale','Require Customer For Sale','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',1,6,'2026-07-16 14:30:36'),(28,3,'allow_partial_payment','Allow Partial Payment','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',1,7,'2026-07-16 14:30:36'),(29,3,'allow_sales_without_stock','Allow Sales Without Stock','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',1,8,'2026-07-16 14:30:36'),(30,3,'discount_limit_percentage','Discount Limit Percentage','Enter the numeric value used for this rule.','number','10','10',1,9,'2026-07-16 14:30:36'),(31,3,'require_approval_for_high_discount','Require Approval For High Discount','Turn this rule on or off for the IMS workflow.','toggle',NULL,'true',1,10,'2026-07-16 14:30:36'),(32,4,'allow_sales_return','Allow Sales Return','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,1,'2026-07-14 20:00:50'),(33,4,'sales_return_days','Sales Return Days','Enter the numeric value used for this rule.','number','7','7',0,2,'2026-07-14 20:00:50'),(34,4,'return_approval_required','Return Approval Required','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,3,'2026-07-14 20:00:50'),(35,4,'refund_approval_required','Refund Approval Required','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,4,'2026-07-14 20:00:50'),(36,4,'auto_restock_returned_items','Auto Restock Returned Items','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,5,'2026-07-14 20:00:50'),(37,4,'require_return_reason','Require Return Reason','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,6,'2026-07-14 20:00:50'),(38,4,'allow_partial_return','Allow Partial Return','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,7,'2026-07-14 20:00:50'),(39,4,'return_number_prefix','Return Number Prefix','Enter the default text value used for this rule.','text','RET-','RET-',0,8,'2026-07-14 20:00:50'),(40,6,'gst_enabled','GST Enabled','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,1,'2026-07-14 20:00:50'),(41,6,'default_gst_percentage','Default GST Percentage','Enter the numeric value used for this rule.','number','18','18',0,2,'2026-07-14 20:00:50'),(42,6,'cgst_percentage','CGST Percentage','Enter the numeric value used for this rule.','number','9','9',0,3,'2026-07-14 20:00:50'),(43,6,'sgst_percentage','SGST Percentage','Enter the numeric value used for this rule.','number','9','9',0,4,'2026-07-14 20:00:50'),(44,6,'igst_percentage','IGST Percentage','Enter the numeric value used for this rule.','number','18','18',0,5,'2026-07-14 20:00:50'),(45,6,'tax_inclusive_pricing','Tax Inclusive Pricing','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,6,'2026-07-14 20:00:50'),(46,6,'show_tax_on_invoice','Show Tax On Invoice','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,7,'2026-07-14 20:00:50'),(47,6,'round_off_invoice_amount','Round Off Invoice Amount','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,8,'2026-07-14 20:00:50'),(48,6,'decimal_places_for_amount','Decimal Places For Amount','Enter the numeric value used for this rule.','number','2','2',0,9,'2026-07-14 20:00:50'),(49,7,'allow_negative_stock','Allow Negative Stock','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,1,'2026-07-14 20:00:51'),(50,7,'stock_adjustment_approval_required','Stock Adjustment Approval Required','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,2,'2026-07-14 20:00:51'),(51,7,'stock_transfer_approval_required','Stock Transfer Approval Required','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,3,'2026-07-14 20:00:51'),(52,7,'stock_audit_approval_required','Stock Audit Approval Required','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,4,'2026-07-14 20:00:51'),(53,7,'batch_number_required','Batch Number Required','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,5,'2026-07-14 20:00:51'),(54,7,'expiry_tracking_enabled','Expiry Tracking Enabled','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,6,'2026-07-14 20:00:51'),(55,7,'damage_stock_tracking_enabled','Damage Stock Tracking Enabled','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,7,'2026-07-14 20:00:51'),(56,7,'require_reason_for_stock_adjustment','Require Reason For Stock Adjustment','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,8,'2026-07-14 20:00:51'),(57,7,'allow_backdated_stock_entry','Allow Backdated Stock Entry','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,9,'2026-07-14 20:00:51'),(58,7,'stock_movement_lock_after_days','Stock Movement Lock After Days','Enter the numeric value used for this rule.','number','7','7',0,10,'2026-07-14 20:00:51'),(59,8,'warehouse_required_for_stock','Warehouse Required For Stock','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,1,'2026-07-14 20:00:53'),(60,8,'bin_required_for_stock','Bin Required For Stock','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,2,'2026-07-14 20:00:53'),(61,8,'rack_required_for_stock','Rack Required For Stock','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,3,'2026-07-14 20:00:53'),(62,8,'auto_assign_bin','Auto Assign Bin','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,4,'2026-07-14 20:00:53'),(63,8,'auto_putaway_enabled','Auto Putaway Enabled','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,5,'2026-07-14 20:00:53'),(64,8,'allow_inter_warehouse_transfer','Allow Inter-Warehouse Transfer','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,6,'2026-07-14 20:00:53'),(65,8,'require_approval_for_warehouse_transfer','Require Approval For Warehouse Transfer','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,7,'2026-07-14 20:00:53'),(66,8,'allow_stock_in_inactive_warehouse','Allow Stock In Inactive Warehouse','Turn this rule on or off for the IMS workflow.','toggle','false','false',0,8,'2026-07-14 20:00:53'),(67,9,'enable_audit_logs','Enable Audit Logs','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,1,'2026-07-14 20:00:54'),(68,9,'track_user_login','Track User Login','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,2,'2026-07-14 20:00:54'),(69,9,'track_product_changes','Track Product Changes','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,3,'2026-07-14 20:00:54'),(70,9,'track_stock_changes','Track Stock Changes','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,4,'2026-07-14 20:00:54'),(71,9,'track_purchase_changes','Track Purchase Changes','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,5,'2026-07-14 20:00:54'),(72,9,'track_sales_changes','Track Sales Changes','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,6,'2026-07-14 20:00:54'),(73,9,'track_payment_changes','Track Payment Changes','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,7,'2026-07-14 20:00:54'),(74,9,'track_settings_changes','Track Settings Changes','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,8,'2026-07-14 20:00:54'),(75,9,'log_retention_days','Log Retention Days','Enter the numeric value used for this rule.','number','180','180',0,9,'2026-07-14 20:00:54'),(76,9,'allow_export_logs','Allow Export Logs','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,10,'2026-07-14 20:00:54'),(77,10,'default_report_date_range','Default Report Date Range','Choose the default option used by this module.','dropdown','This Month','This Month',0,1,'2026-07-14 20:00:55'),(78,10,'default_export_format','Default Export Format','Choose the default option used by this module.','dropdown','Excel','Excel',0,2,'2026-07-14 20:00:55'),(79,10,'enable_pdf_export','Enable PDF Export','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,3,'2026-07-14 20:00:55'),(80,10,'enable_excel_export','Enable Excel Export','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,4,'2026-07-14 20:00:55'),(81,10,'show_company_details_on_reports','Show Company Details On Reports','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,5,'2026-07-14 20:00:55'),(82,10,'show_tax_details_on_reports','Show Tax Details On Reports','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,6,'2026-07-14 20:00:55'),(83,10,'report_decimal_places','Report Decimal Places','Enter the numeric value used for this rule.','number','2','2',0,7,'2026-07-14 20:00:55'),(84,10,'allow_report_download','Allow Report Download','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,8,'2026-07-14 20:00:55'),(85,11,'session_timeout_minutes','Session Timeout Minutes','Enter the numeric value used for this rule.','number','30','30',0,1,'2026-07-14 20:00:56'),(86,11,'max_login_attempts','Max Login Attempts','Enter the numeric value used for this rule.','number','5','5',0,2,'2026-07-14 20:00:56'),(87,11,'account_lock_duration_minutes','Account Lock Duration Minutes','Enter the numeric value used for this rule.','number','15','15',0,3,'2026-07-14 20:00:56'),(88,11,'password_expiry_days','Password Expiry Days','Enter the numeric value used for this rule.','number','90','90',0,4,'2026-07-14 20:00:56'),(89,11,'minimum_password_length','Minimum Password Length','Enter the numeric value used for this rule.','number','8','8',0,5,'2026-07-14 20:00:56'),(90,11,'require_strong_password','Require Strong Password','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,6,'2026-07-14 20:00:56'),(91,11,'auto_logout_on_inactivity','Auto Logout On Inactivity','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,7,'2026-07-14 20:00:56'),(92,11,'force_relogin_after_password_change','Force Re-login After Password Change','Turn this rule on or off for the IMS workflow.','toggle','true','true',1,8,'2026-07-14 20:00:56'),(93,12,'pos_integration_enabled','POS Integration Enabled','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,1,'2026-08-10 12:47:53'),(94,12,'payment_gateway_enabled','Payment Gateway Enabled','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,2,'2026-08-10 12:47:53'),(95,12,'email_smtp_enabled','Email SMTP Enabled','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,3,'2026-08-10 12:47:53'),(96,12,'sms_gateway_enabled','SMS Gateway Enabled','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,4,'2026-08-10 12:47:53'),(97,12,'whatsapp_notification_enabled','WhatsApp Notification Enabled','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,5,'2026-08-10 12:47:53'),(98,12,'external_sync_enabled','External Sync Enabled','Turn this rule on or off for the IMS workflow.','toggle',NULL,'false',0,6,'2026-08-10 12:47:53'),(99,12,'sync_frequency','Sync Frequency','Enter the default text value used for this rule.','text','Daily','Daily',1,7,'2026-08-10 12:47:53'),(100,12,'api_key','API Key','Enter the default text value used for this rule.','text',NULL,'',0,8,'2026-08-10 12:47:53'),(101,12,'webhook_url','Webhook URL','Enter the default text value used for this rule.','text',NULL,'',0,9,'2026-08-10 12:47:53');
/*!40000 ALTER TABLE `system_setting_rules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `system_setting_sections`
--

DROP TABLE IF EXISTS `system_setting_sections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_setting_sections` (
  `section_id` int NOT NULL AUTO_INCREMENT,
  `section_key` varchar(100) NOT NULL,
  `section_name` varchar(150) NOT NULL,
  `display_order` int NOT NULL,
  `is_active` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`section_id`),
  UNIQUE KEY `section_key` (`section_key`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `system_setting_sections`
--

LOCK TABLES `system_setting_sections` WRITE;
/*!40000 ALTER TABLE `system_setting_sections` DISABLE KEYS */;
INSERT INTO `system_setting_sections` VALUES (1,'product_rules','Product Rules',1,1),(2,'purchase_goods_receipt','Purchase & Goods Receipt',2,1),(3,'sales_invoice','Sales & Invoice',3,1),(4,'return_refund','Return & Refund',4,1),(6,'tax_billing','Tax & Billing',5,1),(7,'advanced_stock_control','Advanced Stock Control',6,1),(8,'warehouse_bin_rack','Warehouse / Bin / Rack',7,1),(9,'audit_log_rules','Audit Log Rules',8,1),(10,'report_export','Report & Export',9,1),(11,'system_security_policy','System Security Policy',10,1),(12,'integration_settings','Integration Settings',11,1);
/*!40000 ALTER TABLE `system_setting_sections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `system_settings`
--

DROP TABLE IF EXISTS `system_settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_settings` (
  `setting_id` int NOT NULL AUTO_INCREMENT,
  `company_name` varchar(255) DEFAULT NULL,
  `company_email` varchar(255) DEFAULT NULL,
  `company_phone` varchar(50) DEFAULT NULL,
  `company_address` text,
  `gst_number` varchar(100) DEFAULT NULL,
  `currency` varchar(20) DEFAULT NULL,
  `timezone` varchar(100) DEFAULT NULL,
  `invoice_prefix` varchar(20) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `allow_negative_stock` tinyint(1) DEFAULT '0',
  `default_reorder_level` int DEFAULT '10',
  `stock_valuation_method` varchar(20) DEFAULT 'FIFO',
  `invoice_start_number` int DEFAULT '1000',
  `enable_audit_logs` tinyint(1) DEFAULT '1',
  `audit_retention_days` int DEFAULT '365',
  `low_stock_alert` tinyint(1) DEFAULT '1',
  `default_unit_type` varchar(100) DEFAULT NULL,
  `enable_barcode` tinyint(1) NOT NULL DEFAULT '0',
  `auto_stock_update` tinyint(1) NOT NULL DEFAULT '0',
  `email_notifications` tinyint(1) NOT NULL DEFAULT '1',
  `low_stock_notifications` tinyint(1) NOT NULL DEFAULT '1',
  `purchase_notifications` tinyint(1) NOT NULL DEFAULT '1',
  `sales_notifications` tinyint(1) NOT NULL DEFAULT '1',
  `system_alerts` tinyint(1) NOT NULL DEFAULT '1',
  `enable_two_factor_auth` tinyint(1) NOT NULL DEFAULT '0',
  `company_logo` varchar(500) DEFAULT NULL,
  `theme_mode` varchar(50) DEFAULT NULL,
  `language` varchar(50) DEFAULT NULL,
  `collapse_sidebar` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`setting_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `system_settings`
--

LOCK TABLES `system_settings` WRITE;
/*!40000 ALTER TABLE `system_settings` DISABLE KEYS */;
/*!40000 ALTER TABLE `system_settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `units`
--

DROP TABLE IF EXISTS `units`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `units` (
  `unit_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `short_name` varchar(20) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`unit_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `units`
--

LOCK TABLES `units` WRITE;
/*!40000 ALTER TABLE `units` DISABLE KEYS */;
INSERT INTO `units` VALUES (1,'Boxes','Box',0),(2,'Bags','Bag',0),(3,'Pieces','Pieces',0);
/*!40000 ALTER TABLE `units` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_tokens`
--

DROP TABLE IF EXISTS `user_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_tokens` (
  `TokenId` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Token` text NOT NULL,
  `IsActive` tinyint(1) DEFAULT '1',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`TokenId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `user_tokens_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_tokens`
--

LOCK TABLES `user_tokens` WRITE;
/*!40000 ALTER TABLE `user_tokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `PhoneNumber` varchar(10) DEFAULT NULL,
  `EmployeeId` varchar(50) DEFAULT NULL,
  `Department` varchar(100) DEFAULT NULL,
  `Warehouse` varchar(100) DEFAULT NULL,
  `ProfilePhoto` varchar(255) DEFAULT NULL,
  `LastLogin` datetime DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `TokenVersion` int NOT NULL DEFAULT '1',
  `FailedLoginAttempts` int NOT NULL DEFAULT '0',
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `EmailVerificationToken` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `EmailVerificationTokenExpiry` datetime(6) DEFAULT NULL,
  `IsEmailVerified` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=128 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (44,'SK','m.prasad4595@gmail.com','$2a$11$.ToW0gbmGe4yZh6xlfq9M.OgAXSv8qsxKbvSCbA6DqqJzaAxaCkLO','Admin',1,'8186898399','IMS-ADM-001','Inventory Management','Main Warehouse - Hyderabad',NULL,NULL,NULL,'2026-08-21 09:28:25',31,0,NULL,NULL,NULL,1),(103,'Kurapati Bhargava','bhargavakurapati49@gmail.com','$2a$11$rmaw2aJWI7VgvmLfLYcJHeEzza1WU52Q4WJ/zgCtaeiU65EdnrMF.','Admin',1,'7386999881','IMS-ADM-001','Inventory Management','Main Warehouse - Hyderabad',NULL,NULL,'2026-08-05 08:11:09','2026-08-26 10:10:14',35,0,NULL,NULL,NULL,1),(105,'Suresh','sureshnuthangi999@gmail.com','$2a$11$ikwocAvgNzev4CXfQCoQcuhz6es8XWnPQhzTEz85Q.bOfUO7fV1LO','Admin',1,'7981882513','IMS-ADM-001','Inventory Management','Main Warehouse - Hyderabad',NULL,NULL,'2026-08-10 10:00:24','2026-08-24 11:27:01',16,0,NULL,NULL,NULL,1),(109,'nandhitha','nandhithachebattina@gmail.com','$2a$11$QGKhgFVVXctLojrUohZ66uTHiUYd0BDMXhlO70ZyQX7ENmEa5xRDG','Admin',1,'9491755559',NULL,NULL,NULL,NULL,NULL,'2026-08-14 12:19:15','2026-08-21 12:31:18',3,0,NULL,NULL,NULL,1),(110,'Jagadeesh','v.saijagadeeshkumar@gmail.com','$2a$11$dOlYFEXBFwditAUBAZoT4uz0eecXuGJOnWZW7v0pXKbnL6QkMkury','User',0,'7799005833',NULL,NULL,NULL,NULL,NULL,'2026-08-14 12:55:29','2026-08-21 10:21:49',1,0,NULL,NULL,NULL,1),(116,'Lasya','sai.lalitha@pirnav.in','$2a$11$3gCsXp.V01PpJ30Zg5R/A.5MPOAuJs1O/fleNYcI1cYPcA0TebkaO','Admin',1,'6301007986',NULL,NULL,NULL,NULL,NULL,'2026-08-20 11:03:38','2026-08-26 16:14:15',7,0,NULL,NULL,NULL,1),(118,'sukanya','sukanyapucheti@gmail.com','$2a$11$V1XDdZnPKR6ChhgEakhLneUzArgrsguzruUVO7mb72jICj/vKBWOq','Admin',1,'7981745289',NULL,NULL,NULL,NULL,NULL,'2026-08-21 05:39:52','2026-08-25 15:31:17',16,0,NULL,NULL,NULL,1),(120,'roopa','roopa@gmail.com','$2a$11$FlL6//rxkQL2T8rw80B7t..oJcH.RVAWcmdqyHKjXt7PMjDr1NbxO','Admin',1,'6785487576',NULL,NULL,NULL,NULL,NULL,'2026-08-21 09:49:48','2026-08-21 09:49:48',1,0,NULL,NULL,NULL,1),(123,'raagini','ragini@gmail.cm','$2a$11$vX8qDHlKdzirJm9.2pu61.orsXc3ltR3lu7T.6zzVtnhlKtj8QP9y','manager',1,'7786896787',NULL,NULL,NULL,NULL,NULL,'2026-08-21 11:17:52','2026-08-21 11:25:43',1,0,NULL,NULL,NULL,1),(124,'john','john@gmail.cm','$2a$11$FXzexOCPJnXGrYFQGSVPnO8VgVzu9m7r6A68c13yakZGU.qA7UIpG','manager',1,'6765879657',NULL,NULL,NULL,NULL,NULL,'2026-08-21 11:26:46','2026-08-21 12:31:01',1,0,NULL,NULL,NULL,1),(125,'suresh','bhargav@gmail.com','$2a$11$VK0A7T7XUIk.UaaS5rJT0.Krr.aw0Qp6eDMxeOiB97I38iU6CzvnO','Admin',1,'8734587699',NULL,NULL,NULL,NULL,NULL,'2026-08-21 11:59:14','2026-08-21 11:59:14',1,0,NULL,NULL,NULL,1),(126,'Puli Hemasri Sai Varma','hemasrisai.varma@pirnav.com','$2a$11$1RM0zEU.xi7RDkO0TGdWm.BqZ3wM5doh8WNNdLQcLzCkavS08qPNC','Assistant Manager',1,'7386746551',NULL,NULL,NULL,NULL,NULL,'2026-08-24 07:10:24','2026-08-24 12:47:58',3,0,NULL,NULL,NULL,1),(127,'Prathil','sukanyapucheti15@gmail.com','$2a$11$ZrGCR57WYaH.N5UPTiC7/..OEkWFDEf1SodMhxC.si22iomyBBtVq','Admin',1,'7678987657',NULL,NULL,NULL,NULL,NULL,'2026-08-25 10:01:08','2026-08-25 10:01:08',1,0,NULL,NULL,NULL,1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `variant_attribute_values`
--

DROP TABLE IF EXISTS `variant_attribute_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `variant_attribute_values` (
  `id` int NOT NULL AUTO_INCREMENT,
  `variant_id` int DEFAULT NULL,
  `attribute_id` int DEFAULT NULL,
  `value_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `variant_id` (`variant_id`),
  KEY `attribute_id` (`attribute_id`),
  KEY `value_id` (`value_id`),
  CONSTRAINT `variant_attribute_values_ibfk_1` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`variant_id`),
  CONSTRAINT `variant_attribute_values_ibfk_2` FOREIGN KEY (`attribute_id`) REFERENCES `attributes` (`attribute_id`),
  CONSTRAINT `variant_attribute_values_ibfk_3` FOREIGN KEY (`value_id`) REFERENCES `attribute_values` (`value_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `variant_attribute_values`
--

LOCK TABLES `variant_attribute_values` WRITE;
/*!40000 ALTER TABLE `variant_attribute_values` DISABLE KEYS */;
/*!40000 ALTER TABLE `variant_attribute_values` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `warehouse_transfer_audits`
--

DROP TABLE IF EXISTS `warehouse_transfer_audits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `warehouse_transfer_audits` (
  `warehouse_transfer_audit_id` int NOT NULL AUTO_INCREMENT,
  `transfer_id` int NOT NULL,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `from_warehouse_id` int NOT NULL,
  `to_warehouse_id` int NOT NULL,
  `quantity` decimal(18,2) NOT NULL,
  `user_id` int DEFAULT NULL,
  `user_name` varchar(256) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`warehouse_transfer_audit_id`),
  KEY `idx_warehouse_transfer_audits_transfer` (`transfer_id`),
  KEY `idx_warehouse_transfer_audits_product` (`product_id`),
  KEY `idx_warehouse_transfer_audits_from_warehouse` (`from_warehouse_id`),
  KEY `idx_warehouse_transfer_audits_to_warehouse` (`to_warehouse_id`),
  KEY `idx_warehouse_transfer_audits_created_at` (`created_at`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `warehouse_transfer_audits`
--

LOCK TABLES `warehouse_transfer_audits` WRITE;
/*!40000 ALTER TABLE `warehouse_transfer_audits` DISABLE KEYS */;
INSERT INTO `warehouse_transfer_audits` VALUES (1,1,3,NULL,2,1,1.00,103,'Kurapati Bhargava','2026-08-19 12:29:24');
/*!40000 ALTER TABLE `warehouse_transfer_audits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `warehouse_users`
--

DROP TABLE IF EXISTS `warehouse_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `warehouse_users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `warehouse_id` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `role` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `warehouse_id` (`warehouse_id`),
  CONSTRAINT `warehouse_users_ibfk_1` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `warehouse_users`
--

LOCK TABLES `warehouse_users` WRITE;
/*!40000 ALTER TABLE `warehouse_users` DISABLE KEYS */;
/*!40000 ALTER TABLE `warehouse_users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `warehouse_zones`
--

DROP TABLE IF EXISTS `warehouse_zones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `warehouse_zones` (
  `zone_id` int NOT NULL AUTO_INCREMENT,
  `warehouse_id` int DEFAULT NULL,
  `zone_name` varchar(100) DEFAULT NULL,
  `description` text,
  PRIMARY KEY (`zone_id`),
  KEY `warehouse_id` (`warehouse_id`),
  CONSTRAINT `warehouse_zones_ibfk_1` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses` (`warehouse_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `warehouse_zones`
--

LOCK TABLES `warehouse_zones` WRITE;
/*!40000 ALTER TABLE `warehouse_zones` DISABLE KEYS */;
/*!40000 ALTER TABLE `warehouse_zones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `warehouses`
--

DROP TABLE IF EXISTS `warehouses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `warehouses` (
  `warehouse_id` int NOT NULL AUTO_INCREMENT,
  `warehouse_code` varchar(50) NOT NULL,
  `name` varchar(150) NOT NULL,
  `location` varchar(255) NOT NULL,
  `manager_name` varchar(150) NOT NULL,
  `phone` varchar(20) NOT NULL,
  `email` varchar(255) NOT NULL,
  `status` enum('active','inactive') DEFAULT 'active',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL,
  `is_deleted` bit(1) NOT NULL DEFAULT b'0',
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`warehouse_id`),
  UNIQUE KEY `uq_warehouse_name` (`name`),
  UNIQUE KEY `uq_warehouse_code` (`warehouse_code`),
  UNIQUE KEY `uq_warehouse_email` (`email`),
  KEY `idx_warehouse_status` (`status`),
  KEY `idx_warehouse_location` (`location`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `warehouses`
--

LOCK TABLES `warehouses` WRITE;
/*!40000 ALTER TABLE `warehouses` DISABLE KEYS */;
INSERT INTO `warehouses` VALUES (1,'WH-VIJ-429','Vijayawada main warehouse','Vijayawada','Nandhitha','9491755559','vijayawada@gmail.com','active','2026-08-17 01:12:29',NULL,_binary '\0',NULL),(2,'WH-HYD-809','Hyderabad central warehouse','Hyderabad','Bhargava','7386999881','hyderabad@gmail.com','active','2026-08-17 01:13:33',NULL,_binary '\0',NULL),(3,'WH-VIZ-625','Vizag big warehouse','Vizag','Suresh','7981882513','vizagbigwarehouse@gmail.com','active','2026-08-17 01:15:09',NULL,_binary '\0',NULL);
/*!40000 ALTER TABLE `warehouses` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-08-31  9:10:23
