CREATE DATABASE  IF NOT EXISTS `rvcardb` /*!40100 DEFAULT CHARACTER SET utf8 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `rvcardb`;
-- MySQL dump 10.13  Distrib 8.0.19, for Linux (x86_64)
--
-- Host: localhost    Database: rvcardb
-- ------------------------------------------------------
-- Server version	8.0.19

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
-- Table structure for table `patient`
--

DROP TABLE IF EXISTS `patient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patient` (
  `pat_id` int NOT NULL AUTO_INCREMENT,
  `pat_name` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `pat_cpf` varchar(11) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `pat_phone` varchar(15) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `pat_email` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `pat_note` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `pat_gender` char(1) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `pat_birthday` date DEFAULT NULL,
  `pat_status` tinyint DEFAULT NULL,
  PRIMARY KEY (`pat_id`),
  UNIQUE KEY `pat_cpf_UNIQUE` (`pat_cpf`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patient`
--

LOCK TABLES `patient` WRITE;
/*!40000 ALTER TABLE `patient` DISABLE KEYS */;
INSERT INTO `patient` VALUES (1,'Igor','123','123123123','123123','paciente zero bala','F','1997-12-07',1),(2,'Pacientezao Novo ','45134','123123123','22222','paciente com problema','M','2000-03-06',1),(3,'Teste Desativado','2312312','22123','22222','','F','1999-06-01',0);
/*!40000 ALTER TABLE `patient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `psychologist`
--

DROP TABLE IF EXISTS `psychologist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `psychologist` (
  `psyc_id` int NOT NULL AUTO_INCREMENT,
  `psyc_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `psyc_cpf` varchar(11) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `psyc_email` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `psyc_phone` varchar(15) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `psyc_birthday` date DEFAULT NULL,
  `psyc_gender` char(1) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `psyc_crp` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `psyc_status` tinyint DEFAULT NULL,
  `psyc_password` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`psyc_id`),
  UNIQUE KEY `psyc_cpf_UNIQUE` (`psyc_cpf`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `psychologist`
--

LOCK TABLES `psychologist` WRITE;
/*!40000 ALTER TABLE `psychologist` DISABLE KEYS */;
INSERT INTO `psychologist` VALUES (1,'Igor Brito','453','igor@igor','123123','1997-06-04','M','123321',1,'igor123'),(2,'Psic','22222','emailemail','456789','2000-01-01','M','123456789',0,'igor123');
/*!40000 ALTER TABLE `psychologist` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-02-21 13:14:28
