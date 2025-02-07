-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: localhost    Database: apatormetrix
-- ------------------------------------------------------
-- Server version	8.0.35

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
-- Table structure for table `payment_card`
--

DROP TABLE IF EXISTS `payment_card`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_card` (
  `Payment_Card_ID` int NOT NULL AUTO_INCREMENT,
  `Owner_Account_Number` varchar(19) DEFAULT NULL,
  `Pin` varchar(4) DEFAULT NULL,
  `Card_Serial_Number` varchar(20) DEFAULT NULL,
  `UCID` varchar(32) DEFAULT NULL,
  `Card_Number` text,
  `ExpiryDate` date DEFAULT NULL,
  `CVV` varchar(3) DEFAULT NULL,
  PRIMARY KEY (`Payment_Card_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_card`
--

LOCK TABLES `payment_card` WRITE;
/*!40000 ALTER TABLE `payment_card` DISABLE KEYS */;
INSERT INTO `payment_card` VALUES (12,'12345','1','1222','e6f7e06d99e34bfb9d154eb9823eb48e','9876987698769876','2025-03-01','123'),(17,'12121212','1111','1212121212121','90cc20d9cc934cc5aefde3c22432d9e6','9565665656565656','2026-09-09','456');
/*!40000 ALTER TABLE `payment_card` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment_card_owner`
--

DROP TABLE IF EXISTS `payment_card_owner`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_card_owner` (
  `Payment_Card_Owner_ID` int NOT NULL AUTO_INCREMENT,
  `Payment_Card_ID` int DEFAULT NULL,
  `Name` text,
  `Surname` text,
  `Active` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Payment_Card_Owner_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment_card_owner`
--

LOCK TABLES `payment_card_owner` WRITE;
/*!40000 ALTER TABLE `payment_card_owner` DISABLE KEYS */;
INSERT INTO `payment_card_owner` VALUES (1,12,'Krystian','Kowalski',1),(2,170,'Krystian','Kowalski',0);
/*!40000 ALTER TABLE `payment_card_owner` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-02-07 12:53:40
