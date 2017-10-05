CREATE DATABASE  IF NOT EXISTS `maxmind_geolite2` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `maxmind_geolite2`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: maxmind_geolite2
-- ------------------------------------------------------
-- Server version	5.7.18-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `geographical_information_view`
--

DROP TABLE IF EXISTS `geographical_information_view`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geographical_information_view` (
  `geoname_id` int(11) NOT NULL,
  `latitude` double NOT NULL,
  `longitude` double NOT NULL,
  `country_iso_code` varchar(16) DEFAULT NULL,
  `country_name` varchar(64) DEFAULT NULL,
  `city_name` varchar(128) DEFAULT NULL,
  `subdivision_1_name` varchar(128) DEFAULT NULL,
  `subdivision_2_name` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`geoname_id`),
  UNIQUE KEY `geoname_id_UNIQUE` (`geoname_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geolite2_city_blocks_ipv4`
--

DROP TABLE IF EXISTS `geolite2_city_blocks_ipv4`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geolite2_city_blocks_ipv4` (
  `network` char(128) NOT NULL COMMENT 'This is the IPv4 or IPv6 network in CIDR format such as ''2.21.92.0/29'' or ''2001:4b0::/80''. We offer a utility to convert this column to start/end IPs or start/end integers. See the conversion utility section for details.',
  `geoname_id` int(11) DEFAULT NULL COMMENT 'A unique identifier for the network’s location as specified by GeoNames. This ID can be used to look up the location information in the Location file.',
  `registered_country_geoname_id` int(11) DEFAULT NULL COMMENT 'The registered country is the country in which the ISP has registered the network. This column contains a unique identifier for the network’s registered country as specified by GeoNames. This ID can be used to look up the location information in the Location file.',
  `represented_country_geoname_id` int(11) DEFAULT NULL COMMENT 'The represented country is the country which is represented by users of the IP\naddress. For instance, the country represented by an overseas military base. This column contains a unique identifier for the network’s registered country as specified by GeoNames. This ID can be used to look up the location information in the Location file.',
  `is_anonymous_proxy` int(11) DEFAULT NULL COMMENT 'A 1 if the network is an anonymous proxy, otherwise 0.',
  `is_satellite_provider` int(11) DEFAULT NULL COMMENT 'A 1 if the network is for a satellite provider that provides service to multiple countries, otherwise 0.',
  `postal_code` tinytext COMMENT 'The postal code associated with the IP address. These are available for some IP addresses in Australia, Canada, France, Germany, Italy, Spain, Switzerland, United Kingdom, and the US. We return the first 3 characters for Canadian postal codes. We return the the first 2-4 characters (outward code) for postal codes in the United Kingdom.',
  `latitude` double DEFAULT NULL COMMENT 'The latitude of the location associated with the network.',
  `longitude` double DEFAULT NULL COMMENT 'The longitude of the location associated with the network.',
  `accuracy_radius` double DEFAULT NULL,
  PRIMARY KEY (`network`),
  UNIQUE KEY `network_UNIQUE` (`network`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='								\n';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geolite2_city_blocks_ipv6`
--

DROP TABLE IF EXISTS `geolite2_city_blocks_ipv6`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geolite2_city_blocks_ipv6` (
  `network` char(128) NOT NULL COMMENT 'This is the IPv4 or IPv6 network in CIDR format such as ''2.21.92.0/29'' or ''2001:4b0::/80''. We offer a utility to convert this column to start/end IPs or start/end integers. See the conversion utility section for details.',
  `geoname_id` int(11) DEFAULT NULL COMMENT 'A unique identifier for the network’s location as specified by GeoNames. This ID can be used to look up the location information in the Location file.',
  `registered_country_geoname_id` int(11) DEFAULT NULL COMMENT 'The registered country is the country in which the ISP has registered the network. This column contains a unique identifier for the network’s registered country as specified by GeoNames. This ID can be used to look up the location information in the Location file.',
  `represented_country_geoname_id` int(11) DEFAULT NULL COMMENT 'The represented country is the country which is represented by users of the IP\naddress. For instance, the country represented by an overseas military base. This column contains a unique identifier for the network’s registered country as specified by GeoNames. This ID can be used to look up the location information in the Location file.',
  `is_anonymous_proxy` int(11) DEFAULT NULL COMMENT 'A 1 if the network is an anonymous proxy, otherwise 0.',
  `is_satellite_provider` int(11) DEFAULT NULL COMMENT 'A 1 if the network is for a satellite provider that provides service to multiple countries, otherwise 0.',
  `postal_code` tinytext COMMENT 'The postal code associated with the IP address. These are available for some IP addresses in Australia, Canada, France, Germany, Italy, Spain, Switzerland, United Kingdom, and the US. We return the first 3 characters for Canadian postal codes. We return the the first 2-4 characters (outward code) for postal codes in the United Kingdom.',
  `latitude` double DEFAULT NULL COMMENT 'The latitude of the location associated with the network.',
  `longitude` double DEFAULT NULL COMMENT 'The longitude of the location associated with the network.',
  `accuracy_radius` double DEFAULT NULL,
  PRIMARY KEY (`network`),
  UNIQUE KEY `network_UNIQUE` (`network`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='								\n';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geolite2_city_locations`
--

DROP TABLE IF EXISTS `geolite2_city_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geolite2_city_locations` (
  `geoname_id` int(11) NOT NULL COMMENT 'A unique identifier for the a location as specified by GeoNames. This ID can be used as a key for the Location file.',
  `locale_code` varchar(16) NOT NULL COMMENT 'The locale that the names in this row are in. This will always correspond to the locale name of the file.',
  `continent_code` varchar(32) DEFAULT NULL COMMENT 'The continent code for this location. Possible codes are:  AF - Africa  AS - Asia  EU - Europe \n NA - North America  OC - Oceania  SA - South America',
  `continent_name` varchar(512) DEFAULT NULL COMMENT 'The continent name for this location in the file’s locale.',
  `country_iso_code` varchar(512) DEFAULT NULL COMMENT 'A two-character ISO 3166-1 country code for the country associated with the location.',
  `country_name` varchar(512) DEFAULT NULL COMMENT 'The country name for this location in the file’s locale.',
  `subdivision_1_iso_code` varchar(512) DEFAULT NULL COMMENT 'A string of up to three characters containing the region-portion of the ISO 3166-2 code for the first level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the least specific. For example, in the United Kingdom this will be a country like ''England'', not a county like ''Devon''.',
  `subdivision_1_name` varchar(512) DEFAULT NULL COMMENT 'The subdivision name for this location in the file’s locale. As with the subdivision code, this is the least specific subdivision for the location.',
  `subdivision_2_iso_code` varchar(512) DEFAULT NULL COMMENT 'A string of up to three characters containing the region-portion of the ISO 3166-2 code for the second level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the most specific. For example, in the United Kingdom this will be a a county like ''Devon'', not a country like ''England''.',
  `subdivision_2_name` varchar(512) DEFAULT NULL COMMENT 'The subdivision name for this location in the file’s locale. As with the subdivision code, this is the most specific subdivision for the location.',
  `city_name` varchar(512) DEFAULT NULL COMMENT 'The city name for this location in the file’s locale.',
  `metro_code` int(11) DEFAULT NULL COMMENT 'The metro code associated with the IP address. These are only available for networks in the US. MaxMind provides the same metro codes as the Google AdWords API.',
  `time_zone` varchar(128) DEFAULT NULL COMMENT 'The time zone associated with location, as specified by the IANA Time Zone Database, e.g., ''America/New_York''.',
  PRIMARY KEY (`geoname_id`,`locale_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='												\n';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geolite2_country_blocks_ipv4`
--

DROP TABLE IF EXISTS `geolite2_country_blocks_ipv4`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geolite2_country_blocks_ipv4` (
  `network` varchar(32) NOT NULL,
  `geoname_id` varchar(45) DEFAULT NULL,
  `registered_country_geoname_id` varchar(45) DEFAULT NULL,
  `represented_country_geoname_id` varchar(45) DEFAULT NULL,
  `is_anonymous_proxy` varchar(45) DEFAULT NULL,
  `is_satellite_provider` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`network`),
  UNIQUE KEY `network_UNIQUE` (`network`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geolite2_country_blocks_ipv6`
--

DROP TABLE IF EXISTS `geolite2_country_blocks_ipv6`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geolite2_country_blocks_ipv6` (
  `network` varchar(128) NOT NULL,
  `geoname_id` varchar(45) DEFAULT NULL,
  `registered_country_geoname_id` varchar(45) DEFAULT NULL,
  `represented_country_geoname_id` varchar(45) DEFAULT NULL,
  `is_anonymous_proxy` varchar(45) DEFAULT NULL,
  `is_satellite_provider` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`network`),
  UNIQUE KEY `network_UNIQUE` (`network`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geolite2_country_locations`
--

DROP TABLE IF EXISTS `geolite2_country_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geolite2_country_locations` (
  `geoname_id` int(11) NOT NULL,
  `locale_code` varchar(45) NOT NULL,
  `continent_code` varchar(45) DEFAULT NULL,
  `continent_name` varchar(45) DEFAULT NULL,
  `country_iso_code` varchar(45) DEFAULT NULL,
  `country_name` tinytext,
  PRIMARY KEY (`geoname_id`,`locale_code`),
  UNIQUE KEY `geoname_id_UNIQUE` (`geoname_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'maxmind_geolite2'
--

--
-- Dumping routines for database 'maxmind_geolite2'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-05-26 22:46:12
