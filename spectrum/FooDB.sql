CREATE DATABASE  IF NOT EXISTS `foodb` /*!40100 DEFAULT CHARACTER SET gbk */;
USE `foodb`;
-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: foodb
-- ------------------------------------------------------
-- Server version	5.5.38

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
-- Table structure for table `compound_alternate_parents`
--

DROP TABLE IF EXISTS `compound_alternate_parents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compound_alternate_parents` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) DEFAULT NULL,
  `compound_id` int(11) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compound_alternate_parents_on_compound_id` (`compound_id`),
  CONSTRAINT `fk_rails_0aefaa1014` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=50721 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compound_external_descriptors`
--

DROP TABLE IF EXISTS `compound_external_descriptors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compound_external_descriptors` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `external_id` varchar(255) DEFAULT NULL,
  `annotations` varchar(255) DEFAULT NULL,
  `compound_id` int(11) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compound_external_descriptors_on_compound_id` (`compound_id`),
  CONSTRAINT `fk_rails_2395524b9a` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4011 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compound_substituents`
--

DROP TABLE IF EXISTS `compound_substituents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compound_substituents` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) DEFAULT NULL,
  `compound_id` int(11) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compound_substituents_on_compound_id` (`compound_id`),
  CONSTRAINT `fk_rails_1e68999a98` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=95356 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compound_synonyms`
--

DROP TABLE IF EXISTS `compound_synonyms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compound_synonyms` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `synonym` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `synonym_source` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_compound_synonyms_on_synonym` (`synonym`),
  KEY `index_compound_synonyms_on_source_id_and_source_type` (`source_id`,`source_type`)
) ENGINE=InnoDB AUTO_INCREMENT=251049 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compounds`
--

DROP TABLE IF EXISTS `compounds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compounds` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `legacy_id` int(11) DEFAULT NULL,
  `type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `public_id` varchar(9) COLLATE utf8_unicode_ci NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `export` tinyint(1) DEFAULT '0',
  `state` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `annotation_quality` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` mediumtext COLLATE utf8_unicode_ci,
  `cas_number` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `melting_point` mediumtext COLLATE utf8_unicode_ci,
  `protein_formula` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `protein_weight` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `experimental_solubility` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `experimental_logp` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `hydrophobicity` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `isoelectric_point` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `metabolism` mediumtext COLLATE utf8_unicode_ci,
  `kegg_compound_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `pubchem_compound_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `pubchem_substance_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `chebi_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `het_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `uniprot_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `uniprot_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `genbank_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `wikipedia_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `synthesis_citations` mediumtext COLLATE utf8_unicode_ci,
  `general_citations` mediumtext COLLATE utf8_unicode_ci,
  `comments` mediumtext COLLATE utf8_unicode_ci,
  `protein_structure_file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `protein_structure_content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `protein_structure_file_size` int(11) DEFAULT NULL,
  `protein_structure_updated_at` datetime DEFAULT NULL,
  `msds_file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `msds_content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `msds_file_size` int(11) DEFAULT NULL,
  `msds_updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `phenolexplorer_id` int(11) DEFAULT NULL,
  `dfc_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `hmdb_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `duke_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `drugbank_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `bigg_id` int(11) DEFAULT NULL,
  `eafus_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `knapsack_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `boiling_point` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `boiling_point_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `charge` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `charge_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `density` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `density_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `optical_rotation` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `optical_rotation_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `percent_composition` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `percent_composition_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `physical_description` mediumtext COLLATE utf8_unicode_ci,
  `physical_description_reference` mediumtext COLLATE utf8_unicode_ci,
  `refractive_index` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `refractive_index_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `uv_index` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `uv_index_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `experimental_pka` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `experimental_pka_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `experimental_solubility_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `experimental_logp_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `hydrophobicity_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `isoelectric_point_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `melting_point_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_alogps_logp` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_logp` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_alogps_logs` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_smiles` mediumtext COLLATE utf8_unicode_ci,
  `moldb_pka` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_formula` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_average_mass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_inchi` mediumtext COLLATE utf8_unicode_ci,
  `moldb_mono_mass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_inchikey` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_alogps_solubility` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moldb_id` int(11) DEFAULT NULL,
  `moldb_iupac` mediumtext COLLATE utf8_unicode_ci,
  `structure_source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `duplicate_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `old_dfc_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `dfc_name` mediumtext COLLATE utf8_unicode_ci,
  `compound_source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `flavornet_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `goodscent_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `superscent_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `phenolexplorer_metabolite_id` int(11) DEFAULT NULL,
  `kingdom` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `superklass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `klass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `subklass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `direct_parent` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `molecular_framework` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `chembl_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `chemspider_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `meta_cyc_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `foodcomex` tinyint(1) DEFAULT NULL,
  `phytohub_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_compounds_on_public_id` (`public_id`),
  UNIQUE KEY `index_compounds_on_name` (`name`),
  UNIQUE KEY `index_compounds_on_name_and_public_id` (`name`,`public_id`)
) ENGINE=InnoDB AUTO_INCREMENT=31528 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compounds_enzymes`
--

DROP TABLE IF EXISTS `compounds_enzymes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compounds_enzymes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) NOT NULL,
  `enzyme_id` int(11) NOT NULL,
  `citations` mediumtext COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_compounds_enzymes_on_compound_id_and_enzyme_id` (`compound_id`,`enzyme_id`)
) ENGINE=InnoDB AUTO_INCREMENT=105090 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compounds_flavors`
--

DROP TABLE IF EXISTS `compounds_flavors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compounds_flavors` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) NOT NULL,
  `flavor_id` int(11) NOT NULL,
  `citations` mediumtext COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_compounds_flavors_on_compound_id_and_flavor_id` (`compound_id`,`flavor_id`),
  KEY `index_compounds_flavors_on_source_id_and_source_type` (`source_id`,`source_type`)
) ENGINE=InnoDB AUTO_INCREMENT=11632 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compounds_health_effects`
--

DROP TABLE IF EXISTS `compounds_health_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compounds_health_effects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) NOT NULL,
  `health_effect_id` int(11) NOT NULL,
  `orig_health_effect_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `orig_compound_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `orig_citation` mediumtext COLLATE utf8_unicode_ci,
  `citation` mediumtext COLLATE utf8_unicode_ci NOT NULL,
  `citation_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compounds_health_effects_on_source_id_and_source_type` (`source_id`,`source_type`)
) ENGINE=InnoDB AUTO_INCREMENT=11093 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compounds_pathways`
--

DROP TABLE IF EXISTS `compounds_pathways`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compounds_pathways` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) DEFAULT NULL,
  `pathway_id` int(11) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compounds_pathways_on_compound_id` (`compound_id`),
  KEY `index_compounds_pathways_on_pathway_id` (`pathway_id`),
  CONSTRAINT `fk_rails_14c02acb79` FOREIGN KEY (`pathway_id`) REFERENCES `pathways` (`id`),
  CONSTRAINT `fk_rails_34b0bf14de` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1605 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `contents`
--

DROP TABLE IF EXISTS `contents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `contents` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) DEFAULT NULL,
  `food_id` int(11) NOT NULL,
  `orig_food_id` varchar(255) DEFAULT NULL,
  `orig_food_common_name` varchar(255) DEFAULT NULL,
  `orig_food_scientific_name` varchar(255) DEFAULT NULL,
  `orig_food_part` varchar(255) DEFAULT NULL,
  `orig_source_id` varchar(255) DEFAULT NULL,
  `orig_source_name` varchar(255) DEFAULT NULL,
  `orig_content` decimal(15,9) DEFAULT NULL,
  `orig_min` decimal(15,9) DEFAULT NULL,
  `orig_max` decimal(15,9) DEFAULT NULL,
  `orig_unit` varchar(255) DEFAULT NULL,
  `orig_citation` mediumtext,
  `citation` mediumtext NOT NULL,
  `citation_type` varchar(255) NOT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `orig_method` varchar(255) DEFAULT NULL,
  `orig_unit_expression` varchar(255) DEFAULT NULL,
  `standard_content` decimal(15,9) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `content_source_and_food_index` (`source_id`,`source_type`,`food_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1682258 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enzymes`
--

DROP TABLE IF EXISTS `enzymes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enzymes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `gene_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` mediumtext COLLATE utf8_unicode_ci,
  `go_classification` mediumtext COLLATE utf8_unicode_ci,
  `general_function` mediumtext COLLATE utf8_unicode_ci,
  `specific_function` mediumtext COLLATE utf8_unicode_ci,
  `pathway` mediumtext COLLATE utf8_unicode_ci,
  `reaction` mediumtext COLLATE utf8_unicode_ci,
  `cellular_location` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `signals` mediumtext COLLATE utf8_unicode_ci,
  `transmembrane_regions` mediumtext COLLATE utf8_unicode_ci,
  `molecular_weight` decimal(15,9) DEFAULT NULL,
  `theoretical_pi` decimal(15,9) DEFAULT NULL,
  `locus` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `chromosome` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `uniprot_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `uniprot_id` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `pdb_id` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `genbank_protein_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `genbank_gene_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `genecard_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `genatlas_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `hgnc_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `hprd_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `organism` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `general_citations` mediumtext COLLATE utf8_unicode_ci,
  `comments` mediumtext COLLATE utf8_unicode_ci,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_enzymes_on_name` (`name`),
  UNIQUE KEY `index_enzymes_on_gene_name` (`gene_name`),
  UNIQUE KEY `index_enzymes_on_uniprot_name` (`uniprot_name`),
  UNIQUE KEY `index_enzymes_on_uniprot_id` (`uniprot_id`),
  UNIQUE KEY `index_enzymes_on_pdb_id` (`pdb_id`),
  UNIQUE KEY `index_enzymes_on_genbank_protein_id` (`genbank_protein_id`),
  UNIQUE KEY `index_enzymes_on_genbank_gene_id` (`genbank_gene_id`),
  UNIQUE KEY `index_enzymes_on_genecard_id` (`genecard_id`),
  UNIQUE KEY `index_enzymes_on_genatlas_id` (`genatlas_id`),
  UNIQUE KEY `index_enzymes_on_hgnc_id` (`hgnc_id`),
  UNIQUE KEY `index_enzymes_on_hprd_id` (`hprd_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1745 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `flavors`
--

DROP TABLE IF EXISTS `flavors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `flavors` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `flavor_group` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `category` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_flavors_on_name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=857 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `food_taxonomies`
--

DROP TABLE IF EXISTS `food_taxonomies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `food_taxonomies` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `food_id` int(11) DEFAULT NULL,
  `ncbi_taxonomy_id` int(11) DEFAULT NULL,
  `classification_name` varchar(255) DEFAULT NULL,
  `classification_order` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=920 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `foodcomex_compound_providers`
--

DROP TABLE IF EXISTS `foodcomex_compound_providers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `foodcomex_compound_providers` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `foodcomex_compound_id` int(11) NOT NULL,
  `provider_id` int(11) NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_foodcomex_compound_providers_on_foodcomex_compound_id` (`foodcomex_compound_id`),
  KEY `index_foodcomex_compound_providers_on_provider_id` (`provider_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1090 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `foodcomex_compounds`
--

DROP TABLE IF EXISTS `foodcomex_compounds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `foodcomex_compounds` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) NOT NULL,
  `origin` varchar(255) DEFAULT NULL,
  `storage_form` varchar(255) DEFAULT NULL,
  `maximum_quantity` varchar(255) DEFAULT NULL,
  `storage_condition` varchar(255) DEFAULT NULL,
  `contact_name` varchar(255) DEFAULT NULL,
  `contact_address` text,
  `contact_email` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  `export` tinyint(1) DEFAULT NULL,
  `purity` text,
  `description` text,
  `spectra_details` varchar(255) DEFAULT NULL,
  `delivery_time` varchar(255) DEFAULT NULL,
  `stability` varchar(255) DEFAULT NULL,
  `admin_user_id` int(11) DEFAULT NULL,
  `public_id` varchar(255) NOT NULL,
  `cas_number` varchar(255) DEFAULT '',
  `taxonomy_class` varchar(255) DEFAULT '',
  `taxonomy_family` varchar(255) DEFAULT '',
  `experimental_logp` varchar(255) DEFAULT '',
  `experimental_solubility` varchar(255) DEFAULT '',
  `melting_point` varchar(255) DEFAULT '',
  `food_of_origin` varchar(255) DEFAULT '',
  `production_method_reference_text` text,
  `production_method_reference_file_name` varchar(255) DEFAULT NULL,
  `production_method_reference_content_type` varchar(255) DEFAULT NULL,
  `production_method_reference_file_size` int(11) DEFAULT NULL,
  `production_method_reference_updated_at` datetime DEFAULT NULL,
  `elemental_formula` varchar(255) DEFAULT '',
  `minimum_quantity` varchar(255) DEFAULT '',
  `quantity_units` varchar(255) DEFAULT '',
  `available_spectra` text,
  `storage_conditions` text,
  PRIMARY KEY (`id`),
  KEY `index_foodcomex_compounds_on_compound_id` (`compound_id`),
  KEY `index_foodcomex_compounds_on_admin_user_id` (`admin_user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1048 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `foods`
--

DROP TABLE IF EXISTS `foods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `foods` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `name_scientific` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` mediumtext COLLATE utf8_unicode_ci,
  `itis_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `wikipedia_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `picture_file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `picture_content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `picture_file_size` int(11) DEFAULT NULL,
  `picture_updated_at` datetime DEFAULT NULL,
  `legacy_id` int(11) DEFAULT NULL,
  `food_group` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `food_subgroup` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `food_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `export_to_afcdb` tinyint(1) NOT NULL DEFAULT '0',
  `category` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `ncbi_taxonomy_id` int(11) DEFAULT NULL,
  `export_to_foodb` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_foods_on_name` (`name`),
  KEY `index_foods_on_name_scientific` (`name_scientific`),
  KEY `index_foods_on_export_to_afcdb` (`export_to_afcdb`)
) ENGINE=InnoDB AUTO_INCREMENT=925 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `health_effects`
--

DROP TABLE IF EXISTS `health_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `health_effects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `description` mediumtext COLLATE utf8_unicode_ci,
  `chebi_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `chebi_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `chebi_definition` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_health_effects_on_name` (`name`),
  KEY `index_health_effects_on_chebi_name` (`chebi_name`),
  KEY `index_health_effects_on_chebi_id` (`chebi_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1436 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nutrients`
--

DROP TABLE IF EXISTS `nutrients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `nutrients` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `legacy_id` int(11) DEFAULT NULL,
  `type` varchar(255) NOT NULL,
  `public_id` varchar(9) NOT NULL,
  `name` varchar(255) NOT NULL,
  `export` tinyint(1) DEFAULT '0',
  `state` varchar(255) DEFAULT NULL,
  `annotation_quality` varchar(255) DEFAULT NULL,
  `description` mediumtext,
  `wikipedia_id` varchar(255) DEFAULT NULL,
  `comments` mediumtext,
  `dfc_id` varchar(255) DEFAULT NULL,
  `duke_id` varchar(255) DEFAULT NULL,
  `eafus_id` varchar(255) DEFAULT NULL,
  `dfc_name` mediumtext,
  `compound_source` varchar(255) DEFAULT NULL,
  `metabolism` mediumtext,
  `synthesis_citations` mediumtext,
  `general_citations` mediumtext,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_nutrients_on_name` (`name`),
  UNIQUE KEY `index_nutrients_on_public_id` (`public_id`),
  UNIQUE KEY `index_nutrients_on_name_and_public_id` (`name`,`public_id`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathways`
--

DROP TABLE IF EXISTS `pathways`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathways` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `smpdb_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `kegg_map_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=98 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `references`
--

DROP TABLE IF EXISTS `references`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `references` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ref_type` varchar(255) DEFAULT NULL,
  `text` text,
  `pubmed_id` varchar(255) DEFAULT NULL,
  `link` varchar(255) DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_references_on_source_type_and_source_id` (`source_type`,`source_id`)
) ENGINE=InnoDB AUTO_INCREMENT=31792 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-10-24  9:52:35
