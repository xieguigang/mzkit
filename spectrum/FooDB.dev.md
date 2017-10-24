# MySQL development docs
Mysql database field attributes notes:

> AI: Auto Increment; B: Binary; NN: Not Null; PK: Primary Key; UQ: Unique; UN: Unsigned; ZF: Zero Fill

## compound_alternate_parents


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|name|VarChar (255)|||
|compound_id|Int64 (11)|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|``NN``||
|updated_at|DateTime|``NN``||

```SQL
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
```



## compound_external_descriptors


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|external_id|VarChar (255)|||
|annotations|VarChar (255)|||
|compound_id|Int64 (11)|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|``NN``||
|updated_at|DateTime|``NN``||

```SQL
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
```



## compound_substituents


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|name|VarChar (255)|||
|compound_id|Int64 (11)|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|``NN``||
|updated_at|DateTime|``NN``||

```SQL
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
```



## compound_synonyms


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|synonym|VarChar (255)|``NN``||
|synonym_source|VarChar (255)|``NN``||
|created_at|DateTime|||
|updated_at|DateTime|||
|source_id|Int64 (11)|||
|source_type|VarChar (255)|||

```SQL
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
```



## compounds


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|legacy_id|Int64 (11)|||
|type|VarChar (255)|``NN``||
|public_id|VarChar (9)|``NN``||
|name|VarChar (255)|``NN``||
|export|Int64 (1)|||
|state|VarChar (255)|||
|annotation_quality|VarChar (255)|||
|description|Text|||
|cas_number|VarChar (255)|||
|melting_point|Text|||
|protein_formula|VarChar (255)|||
|protein_weight|VarChar (255)|||
|experimental_solubility|VarChar (255)|||
|experimental_logp|VarChar (255)|||
|hydrophobicity|VarChar (255)|||
|isoelectric_point|VarChar (255)|||
|metabolism|Text|||
|kegg_compound_id|VarChar (255)|||
|pubchem_compound_id|VarChar (255)|||
|pubchem_substance_id|VarChar (255)|||
|chebi_id|VarChar (255)|||
|het_id|VarChar (255)|||
|uniprot_id|VarChar (255)|||
|uniprot_name|VarChar (255)|||
|genbank_id|VarChar (255)|||
|wikipedia_id|VarChar (255)|||
|synthesis_citations|Text|||
|general_citations|Text|||
|comments|Text|||
|protein_structure_file_name|VarChar (255)|||
|protein_structure_content_type|VarChar (255)|||
|protein_structure_file_size|Int64 (11)|||
|protein_structure_updated_at|DateTime|||
|msds_file_name|VarChar (255)|||
|msds_content_type|VarChar (255)|||
|msds_file_size|Int64 (11)|||
|msds_updated_at|DateTime|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|||
|updated_at|DateTime|||
|phenolexplorer_id|Int64 (11)|||
|dfc_id|VarChar (255)|||
|hmdb_id|VarChar (255)|||
|duke_id|VarChar (255)|||
|drugbank_id|VarChar (255)|||
|bigg_id|Int64 (11)|||
|eafus_id|VarChar (255)|||
|knapsack_id|VarChar (255)|||
|boiling_point|VarChar (255)|||
|boiling_point_reference|VarChar (255)|||
|charge|VarChar (255)|||
|charge_reference|VarChar (255)|||
|density|VarChar (255)|||
|density_reference|VarChar (255)|||
|optical_rotation|VarChar (255)|||
|optical_rotation_reference|VarChar (255)|||
|percent_composition|VarChar (255)|||
|percent_composition_reference|VarChar (255)|||
|physical_description|Text|||
|physical_description_reference|Text|||
|refractive_index|VarChar (255)|||
|refractive_index_reference|VarChar (255)|||
|uv_index|VarChar (255)|||
|uv_index_reference|VarChar (255)|||
|experimental_pka|VarChar (255)|||
|experimental_pka_reference|VarChar (255)|||
|experimental_solubility_reference|VarChar (255)|||
|experimental_logp_reference|VarChar (255)|||
|hydrophobicity_reference|VarChar (255)|||
|isoelectric_point_reference|VarChar (255)|||
|melting_point_reference|VarChar (255)|||
|moldb_alogps_logp|VarChar (255)|||
|moldb_logp|VarChar (255)|||
|moldb_alogps_logs|VarChar (255)|||
|moldb_smiles|Text|||
|moldb_pka|VarChar (255)|||
|moldb_formula|VarChar (255)|||
|moldb_average_mass|VarChar (255)|||
|moldb_inchi|Text|||
|moldb_mono_mass|VarChar (255)|||
|moldb_inchikey|VarChar (255)|||
|moldb_alogps_solubility|VarChar (255)|||
|moldb_id|Int64 (11)|||
|moldb_iupac|Text|||
|structure_source|VarChar (255)|||
|duplicate_id|VarChar (255)|||
|old_dfc_id|VarChar (255)|||
|dfc_name|Text|||
|compound_source|VarChar (255)|||
|flavornet_id|VarChar (255)|||
|goodscent_id|VarChar (255)|||
|superscent_id|VarChar (255)|||
|phenolexplorer_metabolite_id|Int64 (11)|||
|kingdom|VarChar (255)|||
|superklass|VarChar (255)|||
|klass|VarChar (255)|||
|subklass|VarChar (255)|||
|direct_parent|VarChar (255)|||
|molecular_framework|VarChar (255)|||
|chembl_id|VarChar (255)|||
|chemspider_id|VarChar (255)|||
|meta_cyc_id|VarChar (255)|||
|foodcomex|Int64 (1)|||
|phytohub_id|VarChar (255)|||

```SQL
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
```



## compounds_enzymes


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|compound_id|Int64 (11)|``NN``||
|enzyme_id|Int64 (11)|``NN``||
|citations|Text|``NN``||
|created_at|DateTime|||
|updated_at|DateTime|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||

```SQL
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
```



## compounds_flavors


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|compound_id|Int64 (11)|``NN``||
|flavor_id|Int64 (11)|``NN``||
|citations|Text|``NN``||
|created_at|DateTime|||
|updated_at|DateTime|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|source_id|Int64 (11)|||
|source_type|VarChar (255)|||

```SQL
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
```



## compounds_health_effects


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|compound_id|Int64 (11)|``NN``||
|health_effect_id|Int64 (11)|``NN``||
|orig_health_effect_name|VarChar (255)|||
|orig_compound_name|VarChar (255)|||
|orig_citation|Text|||
|citation|Text|``NN``||
|citation_type|VarChar (255)|``NN``||
|created_at|DateTime|||
|updated_at|DateTime|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|source_id|Int64 (11)|||
|source_type|VarChar (255)|||

```SQL
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
```



## compounds_pathways


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|compound_id|Int64 (11)|||
|pathway_id|Int64 (11)|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|``NN``||
|updated_at|DateTime|``NN``||

```SQL
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
```



## contents


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|source_id|Int64 (11)|||
|source_type|VarChar (255)|||
|food_id|Int64 (11)|``NN``||
|orig_food_id|VarChar (255)|||
|orig_food_common_name|VarChar (255)|||
|orig_food_scientific_name|VarChar (255)|||
|orig_food_part|VarChar (255)|||
|orig_source_id|VarChar (255)|||
|orig_source_name|VarChar (255)|||
|orig_content|Decimal|||
|orig_min|Decimal|||
|orig_max|Decimal|||
|orig_unit|VarChar (255)|||
|orig_citation|Text|||
|citation|Text|``NN``||
|citation_type|VarChar (255)|``NN``||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|||
|updated_at|DateTime|||
|orig_method|VarChar (255)|||
|orig_unit_expression|VarChar (255)|||
|standard_content|Decimal|||

```SQL
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
```



## enzymes


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|name|VarChar (255)|``NN``||
|gene_name|VarChar (255)|||
|description|Text|||
|go_classification|Text|||
|general_function|Text|||
|specific_function|Text|||
|pathway|Text|||
|reaction|Text|||
|cellular_location|VarChar (255)|||
|signals|Text|||
|transmembrane_regions|Text|||
|molecular_weight|Decimal|||
|theoretical_pi|Decimal|||
|locus|VarChar (255)|||
|chromosome|VarChar (255)|||
|uniprot_name|VarChar (255)|||
|uniprot_id|VarChar (100)|||
|pdb_id|VarChar (10)|||
|genbank_protein_id|VarChar (20)|||
|genbank_gene_id|VarChar (20)|||
|genecard_id|VarChar (20)|||
|genatlas_id|VarChar (20)|||
|hgnc_id|VarChar (20)|||
|hprd_id|VarChar (255)|||
|organism|VarChar (255)|||
|general_citations|Text|||
|comments|Text|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|||
|updated_at|DateTime|||

```SQL
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
```



## flavors


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|name|VarChar (255)|``NN``||
|flavor_group|VarChar (255)|||
|category|VarChar (255)|||
|created_at|DateTime|||
|updated_at|DateTime|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||

```SQL
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
```



## food_taxonomies


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|food_id|Int64 (11)|||
|ncbi_taxonomy_id|Int64 (11)|||
|classification_name|VarChar (255)|||
|classification_order|Int64 (11)|||
|created_at|DateTime|``NN``||
|updated_at|DateTime|``NN``||

```SQL
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
```



## foodcomex_compound_providers


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|foodcomex_compound_id|Int64 (11)|``NN``||
|provider_id|Int64 (11)|``NN``||
|created_at|DateTime|||
|updated_at|DateTime|||

```SQL
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
```



## foodcomex_compounds


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|compound_id|Int64 (11)|``NN``||
|origin|VarChar (255)|||
|storage_form|VarChar (255)|||
|maximum_quantity|VarChar (255)|||
|storage_condition|VarChar (255)|||
|contact_name|VarChar (255)|||
|contact_address|Text|||
|contact_email|VarChar (255)|||
|created_at|DateTime|``NN``||
|updated_at|DateTime|``NN``||
|export|Int64 (1)|||
|purity|Text|||
|description|Text|||
|spectra_details|VarChar (255)|||
|delivery_time|VarChar (255)|||
|stability|VarChar (255)|||
|admin_user_id|Int64 (11)|||
|public_id|VarChar (255)|``NN``||
|cas_number|VarChar (255)|||
|taxonomy_class|VarChar (255)|||
|taxonomy_family|VarChar (255)|||
|experimental_logp|VarChar (255)|||
|experimental_solubility|VarChar (255)|||
|melting_point|VarChar (255)|||
|food_of_origin|VarChar (255)|||
|production_method_reference_text|Text|||
|production_method_reference_file_name|VarChar (255)|||
|production_method_reference_content_type|VarChar (255)|||
|production_method_reference_file_size|Int64 (11)|||
|production_method_reference_updated_at|DateTime|||
|elemental_formula|VarChar (255)|||
|minimum_quantity|VarChar (255)|||
|quantity_units|VarChar (255)|||
|available_spectra|Text|||
|storage_conditions|Text|||

```SQL
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
```



## foods


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|name|VarChar (255)|``NN``||
|name_scientific|VarChar (255)|||
|description|Text|||
|itis_id|VarChar (255)|||
|wikipedia_id|VarChar (255)|||
|picture_file_name|VarChar (255)|||
|picture_content_type|VarChar (255)|||
|picture_file_size|Int64 (11)|||
|picture_updated_at|DateTime|||
|legacy_id|Int64 (11)|||
|food_group|VarChar (255)|||
|food_subgroup|VarChar (255)|||
|food_type|VarChar (255)|``NN``||
|created_at|DateTime|||
|updated_at|DateTime|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|export_to_afcdb|Int64 (1)|``NN``||
|category|VarChar (255)|||
|ncbi_taxonomy_id|Int64 (11)|||
|export_to_foodb|Int64 (1)|||

```SQL
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
```



## health_effects


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|name|VarChar (255)|``NN``||
|description|Text|||
|chebi_name|VarChar (255)|||
|chebi_id|VarChar (255)|||
|created_at|DateTime|||
|updated_at|DateTime|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|chebi_definition|Text|||

```SQL
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
```



## nutrients


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|legacy_id|Int64 (11)|||
|type|VarChar (255)|``NN``||
|public_id|VarChar (9)|``NN``||
|name|VarChar (255)|``NN``||
|export|Int64 (1)|||
|state|VarChar (255)|||
|annotation_quality|VarChar (255)|||
|description|Text|||
|wikipedia_id|VarChar (255)|||
|comments|Text|||
|dfc_id|VarChar (255)|||
|duke_id|VarChar (255)|||
|eafus_id|VarChar (255)|||
|dfc_name|Text|||
|compound_source|VarChar (255)|||
|metabolism|Text|||
|synthesis_citations|Text|||
|general_citations|Text|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|||
|updated_at|DateTime|||

```SQL
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
```



## pathways


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|smpdb_id|VarChar (255)|||
|kegg_map_id|VarChar (255)|||
|name|VarChar (255)|||
|created_at|DateTime|||
|updated_at|DateTime|||

```SQL
CREATE TABLE `pathways` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `smpdb_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `kegg_map_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=98 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
```



## references


|field|type|attributes|description|
|-----|----|----------|-----------|
|id|Int64 (11)|``AI``, ``NN``||
|ref_type|VarChar (255)|||
|text|Text|||
|pubmed_id|VarChar (255)|||
|link|VarChar (255)|||
|title|VarChar (255)|||
|creator_id|Int64 (11)|||
|updater_id|Int64 (11)|||
|created_at|DateTime|``NN``||
|updated_at|DateTime|``NN``||
|source_id|Int64 (11)|||
|source_type|VarChar (255)|||

```SQL
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
) ENGINE=InnoDB AUTO_INCREMENT=31792

