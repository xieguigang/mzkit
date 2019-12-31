#Region "Microsoft.VisualBasic::c922111663270fe91b503e8727825f82, DATA\FooDB\FooDB\mysql\compounds.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Class compounds
    ' 
    '     Properties: annotation_quality, bigg_id, boiling_point, boiling_point_reference, cas_number
    '                 charge, charge_reference, chebi_id, chembl_id, chemspider_id
    '                 comments, compound_source, created_at, creator_id, density
    '                 density_reference, description, dfc_id, dfc_name, direct_parent
    '                 drugbank_id, duke_id, duplicate_id, eafus_id, experimental_logp
    '                 experimental_logp_reference, experimental_pka, experimental_pka_reference, experimental_solubility, experimental_solubility_reference
    '                 export, flavornet_id, foodcomex, genbank_id, general_citations
    '                 goodscent_id, het_id, hmdb_id, hydrophobicity, hydrophobicity_reference
    '                 id, isoelectric_point, isoelectric_point_reference, kegg_compound_id, kingdom
    '                 klass, knapsack_id, legacy_id, melting_point, melting_point_reference
    '                 meta_cyc_id, metabolism, moldb_alogps_logp, moldb_alogps_logs, moldb_alogps_solubility
    '                 moldb_average_mass, moldb_formula, moldb_id, moldb_inchi, moldb_inchikey
    '                 moldb_iupac, moldb_logp, moldb_mono_mass, moldb_pka, moldb_smiles
    '                 molecular_framework, msds_content_type, msds_file_name, msds_file_size, msds_updated_at
    '                 name, old_dfc_id, optical_rotation, optical_rotation_reference, percent_composition
    '                 percent_composition_reference, phenolexplorer_id, phenolexplorer_metabolite_id, physical_description, physical_description_reference
    '                 phytohub_id, protein_formula, protein_structure_content_type, protein_structure_file_name, protein_structure_file_size
    '                 protein_structure_updated_at, protein_weight, pubchem_compound_id, pubchem_substance_id, public_id
    '                 refractive_index, refractive_index_reference, state, structure_source, subklass
    '                 superklass, superscent_id, synthesis_citations, type, uniprot_id
    '                 uniprot_name, updated_at, updater_id, uv_index, uv_index_reference
    '                 wikipedia_id
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 11:01:41


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace TMIC.FooDB.mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `compounds`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `compounds` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `legacy_id` int(11) DEFAULT NULL,
'''   `type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `public_id` varchar(9) COLLATE utf8_unicode_ci NOT NULL,
'''   `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `export` tinyint(1) DEFAULT '0',
'''   `state` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `annotation_quality` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `description` mediumtext COLLATE utf8_unicode_ci,
'''   `cas_number` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `melting_point` mediumtext COLLATE utf8_unicode_ci,
'''   `protein_formula` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `protein_weight` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `experimental_solubility` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `experimental_logp` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `hydrophobicity` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `isoelectric_point` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `metabolism` mediumtext COLLATE utf8_unicode_ci,
'''   `kegg_compound_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `pubchem_compound_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `pubchem_substance_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `chebi_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `het_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `uniprot_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `uniprot_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `genbank_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `wikipedia_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `synthesis_citations` mediumtext COLLATE utf8_unicode_ci,
'''   `general_citations` mediumtext COLLATE utf8_unicode_ci,
'''   `comments` mediumtext COLLATE utf8_unicode_ci,
'''   `protein_structure_file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `protein_structure_content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `protein_structure_file_size` int(11) DEFAULT NULL,
'''   `protein_structure_updated_at` datetime DEFAULT NULL,
'''   `msds_file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `msds_content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `msds_file_size` int(11) DEFAULT NULL,
'''   `msds_updated_at` datetime DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   `phenolexplorer_id` int(11) DEFAULT NULL,
'''   `dfc_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `hmdb_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `duke_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `drugbank_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `bigg_id` int(11) DEFAULT NULL,
'''   `eafus_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `knapsack_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `boiling_point` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `boiling_point_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `charge` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `charge_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `density` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `density_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `optical_rotation` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `optical_rotation_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `percent_composition` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `percent_composition_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `physical_description` mediumtext COLLATE utf8_unicode_ci,
'''   `physical_description_reference` mediumtext COLLATE utf8_unicode_ci,
'''   `refractive_index` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `refractive_index_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `uv_index` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `uv_index_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `experimental_pka` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `experimental_pka_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `experimental_solubility_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `experimental_logp_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `hydrophobicity_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `isoelectric_point_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `melting_point_reference` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_alogps_logp` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_logp` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_alogps_logs` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_smiles` mediumtext COLLATE utf8_unicode_ci,
'''   `moldb_pka` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_formula` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_average_mass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_inchi` mediumtext COLLATE utf8_unicode_ci,
'''   `moldb_mono_mass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_inchikey` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_alogps_solubility` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `moldb_id` int(11) DEFAULT NULL,
'''   `moldb_iupac` mediumtext COLLATE utf8_unicode_ci,
'''   `structure_source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `duplicate_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `old_dfc_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `dfc_name` mediumtext COLLATE utf8_unicode_ci,
'''   `compound_source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `flavornet_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `goodscent_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `superscent_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `phenolexplorer_metabolite_id` int(11) DEFAULT NULL,
'''   `kingdom` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `superklass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `klass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `subklass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `direct_parent` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `molecular_framework` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `chembl_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `chemspider_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `meta_cyc_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `foodcomex` tinyint(1) DEFAULT NULL,
'''   `phytohub_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `index_compounds_on_public_id` (`public_id`),
'''   UNIQUE KEY `index_compounds_on_name` (`name`),
'''   UNIQUE KEY `index_compounds_on_name_and_public_id` (`name`,`public_id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=31528 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compounds", Database:="foodb", SchemaSQL:="
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
) ENGINE=InnoDB AUTO_INCREMENT=31528 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;")>
Public Class compounds: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("legacy_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="legacy_id")> Public Property legacy_id As Long
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="type")> Public Property type As String
    <DatabaseField("public_id"), NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="public_id")> Public Property public_id As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="name")> Public Property name As String
    <DatabaseField("export"), DataType(MySqlDbType.Boolean, "1"), Column(Name:="export")> Public Property export As Boolean
    <DatabaseField("state"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="state")> Public Property state As String
    <DatabaseField("annotation_quality"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="annotation_quality")> Public Property annotation_quality As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
    <DatabaseField("cas_number"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="cas_number")> Public Property cas_number As String
    <DatabaseField("melting_point"), DataType(MySqlDbType.Text), Column(Name:="melting_point")> Public Property melting_point As String
    <DatabaseField("protein_formula"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="protein_formula")> Public Property protein_formula As String
    <DatabaseField("protein_weight"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="protein_weight")> Public Property protein_weight As String
    <DatabaseField("experimental_solubility"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_solubility")> Public Property experimental_solubility As String
    <DatabaseField("experimental_logp"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_logp")> Public Property experimental_logp As String
    <DatabaseField("hydrophobicity"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="hydrophobicity")> Public Property hydrophobicity As String
    <DatabaseField("isoelectric_point"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="isoelectric_point")> Public Property isoelectric_point As String
    <DatabaseField("metabolism"), DataType(MySqlDbType.Text), Column(Name:="metabolism")> Public Property metabolism As String
    <DatabaseField("kegg_compound_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="kegg_compound_id")> Public Property kegg_compound_id As String
    <DatabaseField("pubchem_compound_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="pubchem_compound_id")> Public Property pubchem_compound_id As String
    <DatabaseField("pubchem_substance_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="pubchem_substance_id")> Public Property pubchem_substance_id As String
    <DatabaseField("chebi_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="chebi_id")> Public Property chebi_id As String
    <DatabaseField("het_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="het_id")> Public Property het_id As String
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("uniprot_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="uniprot_name")> Public Property uniprot_name As String
    <DatabaseField("genbank_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="genbank_id")> Public Property genbank_id As String
    <DatabaseField("wikipedia_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="wikipedia_id")> Public Property wikipedia_id As String
    <DatabaseField("synthesis_citations"), DataType(MySqlDbType.Text), Column(Name:="synthesis_citations")> Public Property synthesis_citations As String
    <DatabaseField("general_citations"), DataType(MySqlDbType.Text), Column(Name:="general_citations")> Public Property general_citations As String
    <DatabaseField("comments"), DataType(MySqlDbType.Text), Column(Name:="comments")> Public Property comments As String
    <DatabaseField("protein_structure_file_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="protein_structure_file_name")> Public Property protein_structure_file_name As String
    <DatabaseField("protein_structure_content_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="protein_structure_content_type")> Public Property protein_structure_content_type As String
    <DatabaseField("protein_structure_file_size"), DataType(MySqlDbType.Int64, "11"), Column(Name:="protein_structure_file_size")> Public Property protein_structure_file_size As Long
    <DatabaseField("protein_structure_updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="protein_structure_updated_at")> Public Property protein_structure_updated_at As Date
    <DatabaseField("msds_file_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="msds_file_name")> Public Property msds_file_name As String
    <DatabaseField("msds_content_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="msds_content_type")> Public Property msds_content_type As String
    <DatabaseField("msds_file_size"), DataType(MySqlDbType.Int64, "11"), Column(Name:="msds_file_size")> Public Property msds_file_size As Long
    <DatabaseField("msds_updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="msds_updated_at")> Public Property msds_updated_at As Date
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("phenolexplorer_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="phenolexplorer_id")> Public Property phenolexplorer_id As Long
    <DatabaseField("dfc_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="dfc_id")> Public Property dfc_id As String
    <DatabaseField("hmdb_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="hmdb_id")> Public Property hmdb_id As String
    <DatabaseField("duke_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="duke_id")> Public Property duke_id As String
    <DatabaseField("drugbank_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="drugbank_id")> Public Property drugbank_id As String
    <DatabaseField("bigg_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="bigg_id")> Public Property bigg_id As Long
    <DatabaseField("eafus_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="eafus_id")> Public Property eafus_id As String
    <DatabaseField("knapsack_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="knapsack_id")> Public Property knapsack_id As String
    <DatabaseField("boiling_point"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="boiling_point")> Public Property boiling_point As String
    <DatabaseField("boiling_point_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="boiling_point_reference")> Public Property boiling_point_reference As String
    <DatabaseField("charge"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="charge")> Public Property charge As String
    <DatabaseField("charge_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="charge_reference")> Public Property charge_reference As String
    <DatabaseField("density"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="density")> Public Property density As String
    <DatabaseField("density_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="density_reference")> Public Property density_reference As String
    <DatabaseField("optical_rotation"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="optical_rotation")> Public Property optical_rotation As String
    <DatabaseField("optical_rotation_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="optical_rotation_reference")> Public Property optical_rotation_reference As String
    <DatabaseField("percent_composition"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="percent_composition")> Public Property percent_composition As String
    <DatabaseField("percent_composition_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="percent_composition_reference")> Public Property percent_composition_reference As String
    <DatabaseField("physical_description"), DataType(MySqlDbType.Text), Column(Name:="physical_description")> Public Property physical_description As String
    <DatabaseField("physical_description_reference"), DataType(MySqlDbType.Text), Column(Name:="physical_description_reference")> Public Property physical_description_reference As String
    <DatabaseField("refractive_index"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="refractive_index")> Public Property refractive_index As String
    <DatabaseField("refractive_index_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="refractive_index_reference")> Public Property refractive_index_reference As String
    <DatabaseField("uv_index"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="uv_index")> Public Property uv_index As String
    <DatabaseField("uv_index_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="uv_index_reference")> Public Property uv_index_reference As String
    <DatabaseField("experimental_pka"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_pka")> Public Property experimental_pka As String
    <DatabaseField("experimental_pka_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_pka_reference")> Public Property experimental_pka_reference As String
    <DatabaseField("experimental_solubility_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_solubility_reference")> Public Property experimental_solubility_reference As String
    <DatabaseField("experimental_logp_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_logp_reference")> Public Property experimental_logp_reference As String
    <DatabaseField("hydrophobicity_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="hydrophobicity_reference")> Public Property hydrophobicity_reference As String
    <DatabaseField("isoelectric_point_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="isoelectric_point_reference")> Public Property isoelectric_point_reference As String
    <DatabaseField("melting_point_reference"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="melting_point_reference")> Public Property melting_point_reference As String
    <DatabaseField("moldb_alogps_logp"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_alogps_logp")> Public Property moldb_alogps_logp As String
    <DatabaseField("moldb_logp"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_logp")> Public Property moldb_logp As String
    <DatabaseField("moldb_alogps_logs"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_alogps_logs")> Public Property moldb_alogps_logs As String
    <DatabaseField("moldb_smiles"), DataType(MySqlDbType.Text), Column(Name:="moldb_smiles")> Public Property moldb_smiles As String
    <DatabaseField("moldb_pka"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_pka")> Public Property moldb_pka As String
    <DatabaseField("moldb_formula"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_formula")> Public Property moldb_formula As String
    <DatabaseField("moldb_average_mass"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_average_mass")> Public Property moldb_average_mass As String
    <DatabaseField("moldb_inchi"), DataType(MySqlDbType.Text), Column(Name:="moldb_inchi")> Public Property moldb_inchi As String
    <DatabaseField("moldb_mono_mass"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_mono_mass")> Public Property moldb_mono_mass As String
    <DatabaseField("moldb_inchikey"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_inchikey")> Public Property moldb_inchikey As String
    <DatabaseField("moldb_alogps_solubility"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="moldb_alogps_solubility")> Public Property moldb_alogps_solubility As String
    <DatabaseField("moldb_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="moldb_id")> Public Property moldb_id As Long
    <DatabaseField("moldb_iupac"), DataType(MySqlDbType.Text), Column(Name:="moldb_iupac")> Public Property moldb_iupac As String
    <DatabaseField("structure_source"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="structure_source")> Public Property structure_source As String
    <DatabaseField("duplicate_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="duplicate_id")> Public Property duplicate_id As String
    <DatabaseField("old_dfc_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="old_dfc_id")> Public Property old_dfc_id As String
    <DatabaseField("dfc_name"), DataType(MySqlDbType.Text), Column(Name:="dfc_name")> Public Property dfc_name As String
    <DatabaseField("compound_source"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="compound_source")> Public Property compound_source As String
    <DatabaseField("flavornet_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="flavornet_id")> Public Property flavornet_id As String
    <DatabaseField("goodscent_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="goodscent_id")> Public Property goodscent_id As String
    <DatabaseField("superscent_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="superscent_id")> Public Property superscent_id As String
    <DatabaseField("phenolexplorer_metabolite_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="phenolexplorer_metabolite_id")> Public Property phenolexplorer_metabolite_id As Long
    <DatabaseField("kingdom"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="kingdom")> Public Property kingdom As String
    <DatabaseField("superklass"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="superklass")> Public Property superklass As String
    <DatabaseField("klass"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="klass")> Public Property klass As String
    <DatabaseField("subklass"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="subklass")> Public Property subklass As String
    <DatabaseField("direct_parent"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="direct_parent")> Public Property direct_parent As String
    <DatabaseField("molecular_framework"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="molecular_framework")> Public Property molecular_framework As String
    <DatabaseField("chembl_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="chembl_id")> Public Property chembl_id As String
    <DatabaseField("chemspider_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="chemspider_id")> Public Property chemspider_id As String
    <DatabaseField("meta_cyc_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="meta_cyc_id")> Public Property meta_cyc_id As String
    <DatabaseField("foodcomex"), DataType(MySqlDbType.Boolean, "1"), Column(Name:="foodcomex")> Public Property foodcomex As Boolean
    <DatabaseField("phytohub_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="phytohub_id")> Public Property phytohub_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `compounds` (`legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `compounds` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}', '{105}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `compounds` (`legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `compounds` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}', '{105}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `compounds` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `compounds` SET `id`='{0}', `legacy_id`='{1}', `type`='{2}', `public_id`='{3}', `name`='{4}', `export`='{5}', `state`='{6}', `annotation_quality`='{7}', `description`='{8}', `cas_number`='{9}', `melting_point`='{10}', `protein_formula`='{11}', `protein_weight`='{12}', `experimental_solubility`='{13}', `experimental_logp`='{14}', `hydrophobicity`='{15}', `isoelectric_point`='{16}', `metabolism`='{17}', `kegg_compound_id`='{18}', `pubchem_compound_id`='{19}', `pubchem_substance_id`='{20}', `chebi_id`='{21}', `het_id`='{22}', `uniprot_id`='{23}', `uniprot_name`='{24}', `genbank_id`='{25}', `wikipedia_id`='{26}', `synthesis_citations`='{27}', `general_citations`='{28}', `comments`='{29}', `protein_structure_file_name`='{30}', `protein_structure_content_type`='{31}', `protein_structure_file_size`='{32}', `protein_structure_updated_at`='{33}', `msds_file_name`='{34}', `msds_content_type`='{35}', `msds_file_size`='{36}', `msds_updated_at`='{37}', `creator_id`='{38}', `updater_id`='{39}', `created_at`='{40}', `updated_at`='{41}', `phenolexplorer_id`='{42}', `dfc_id`='{43}', `hmdb_id`='{44}', `duke_id`='{45}', `drugbank_id`='{46}', `bigg_id`='{47}', `eafus_id`='{48}', `knapsack_id`='{49}', `boiling_point`='{50}', `boiling_point_reference`='{51}', `charge`='{52}', `charge_reference`='{53}', `density`='{54}', `density_reference`='{55}', `optical_rotation`='{56}', `optical_rotation_reference`='{57}', `percent_composition`='{58}', `percent_composition_reference`='{59}', `physical_description`='{60}', `physical_description_reference`='{61}', `refractive_index`='{62}', `refractive_index_reference`='{63}', `uv_index`='{64}', `uv_index_reference`='{65}', `experimental_pka`='{66}', `experimental_pka_reference`='{67}', `experimental_solubility_reference`='{68}', `experimental_logp_reference`='{69}', `hydrophobicity_reference`='{70}', `isoelectric_point_reference`='{71}', `melting_point_reference`='{72}', `moldb_alogps_logp`='{73}', `moldb_logp`='{74}', `moldb_alogps_logs`='{75}', `moldb_smiles`='{76}', `moldb_pka`='{77}', `moldb_formula`='{78}', `moldb_average_mass`='{79}', `moldb_inchi`='{80}', `moldb_mono_mass`='{81}', `moldb_inchikey`='{82}', `moldb_alogps_solubility`='{83}', `moldb_id`='{84}', `moldb_iupac`='{85}', `structure_source`='{86}', `duplicate_id`='{87}', `old_dfc_id`='{88}', `dfc_name`='{89}', `compound_source`='{90}', `flavornet_id`='{91}', `goodscent_id`='{92}', `superscent_id`='{93}', `phenolexplorer_metabolite_id`='{94}', `kingdom`='{95}', `superklass`='{96}', `klass`='{97}', `subklass`='{98}', `direct_parent`='{99}', `molecular_framework`='{100}', `chembl_id`='{101}', `chemspider_id`='{102}', `meta_cyc_id`='{103}', `foodcomex`='{104}', `phytohub_id`='{105}' WHERE `id` = '{106}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `compounds` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `compounds` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}', '{105}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, cas_number, melting_point, protein_formula, protein_weight, experimental_solubility, experimental_logp, hydrophobicity, isoelectric_point, metabolism, kegg_compound_id, pubchem_compound_id, pubchem_substance_id, chebi_id, het_id, uniprot_id, uniprot_name, genbank_id, wikipedia_id, synthesis_citations, general_citations, comments, protein_structure_file_name, protein_structure_content_type, protein_structure_file_size, MySqlScript.ToMySqlDateTimeString(protein_structure_updated_at), msds_file_name, msds_content_type, msds_file_size, MySqlScript.ToMySqlDateTimeString(msds_updated_at), creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), phenolexplorer_id, dfc_id, hmdb_id, duke_id, drugbank_id, bigg_id, eafus_id, knapsack_id, boiling_point, boiling_point_reference, charge, charge_reference, density, density_reference, optical_rotation, optical_rotation_reference, percent_composition, percent_composition_reference, physical_description, physical_description_reference, refractive_index, refractive_index_reference, uv_index, uv_index_reference, experimental_pka, experimental_pka_reference, experimental_solubility_reference, experimental_logp_reference, hydrophobicity_reference, isoelectric_point_reference, melting_point_reference, moldb_alogps_logp, moldb_logp, moldb_alogps_logs, moldb_smiles, moldb_pka, moldb_formula, moldb_average_mass, moldb_inchi, moldb_mono_mass, moldb_inchikey, moldb_alogps_solubility, moldb_id, moldb_iupac, structure_source, duplicate_id, old_dfc_id, dfc_name, compound_source, flavornet_id, goodscent_id, superscent_id, phenolexplorer_metabolite_id, kingdom, superklass, klass, subklass, direct_parent, molecular_framework, chembl_id, chemspider_id, meta_cyc_id, foodcomex, phytohub_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `compounds` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}', '{105}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, legacy_id, type, public_id, name, export, state, annotation_quality, description, cas_number, melting_point, protein_formula, protein_weight, experimental_solubility, experimental_logp, hydrophobicity, isoelectric_point, metabolism, kegg_compound_id, pubchem_compound_id, pubchem_substance_id, chebi_id, het_id, uniprot_id, uniprot_name, genbank_id, wikipedia_id, synthesis_citations, general_citations, comments, protein_structure_file_name, protein_structure_content_type, protein_structure_file_size, MySqlScript.ToMySqlDateTimeString(protein_structure_updated_at), msds_file_name, msds_content_type, msds_file_size, MySqlScript.ToMySqlDateTimeString(msds_updated_at), creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), phenolexplorer_id, dfc_id, hmdb_id, duke_id, drugbank_id, bigg_id, eafus_id, knapsack_id, boiling_point, boiling_point_reference, charge, charge_reference, density, density_reference, optical_rotation, optical_rotation_reference, percent_composition, percent_composition_reference, physical_description, physical_description_reference, refractive_index, refractive_index_reference, uv_index, uv_index_reference, experimental_pka, experimental_pka_reference, experimental_solubility_reference, experimental_logp_reference, hydrophobicity_reference, isoelectric_point_reference, melting_point_reference, moldb_alogps_logp, moldb_logp, moldb_alogps_logs, moldb_smiles, moldb_pka, moldb_formula, moldb_average_mass, moldb_inchi, moldb_mono_mass, moldb_inchikey, moldb_alogps_solubility, moldb_id, moldb_iupac, structure_source, duplicate_id, old_dfc_id, dfc_name, compound_source, flavornet_id, goodscent_id, superscent_id, phenolexplorer_metabolite_id, kingdom, superklass, klass, subklass, direct_parent, molecular_framework, chembl_id, chemspider_id, meta_cyc_id, foodcomex, phytohub_id)
        Else
        Return String.Format(INSERT_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, cas_number, melting_point, protein_formula, protein_weight, experimental_solubility, experimental_logp, hydrophobicity, isoelectric_point, metabolism, kegg_compound_id, pubchem_compound_id, pubchem_substance_id, chebi_id, het_id, uniprot_id, uniprot_name, genbank_id, wikipedia_id, synthesis_citations, general_citations, comments, protein_structure_file_name, protein_structure_content_type, protein_structure_file_size, MySqlScript.ToMySqlDateTimeString(protein_structure_updated_at), msds_file_name, msds_content_type, msds_file_size, MySqlScript.ToMySqlDateTimeString(msds_updated_at), creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), phenolexplorer_id, dfc_id, hmdb_id, duke_id, drugbank_id, bigg_id, eafus_id, knapsack_id, boiling_point, boiling_point_reference, charge, charge_reference, density, density_reference, optical_rotation, optical_rotation_reference, percent_composition, percent_composition_reference, physical_description, physical_description_reference, refractive_index, refractive_index_reference, uv_index, uv_index_reference, experimental_pka, experimental_pka_reference, experimental_solubility_reference, experimental_logp_reference, hydrophobicity_reference, isoelectric_point_reference, melting_point_reference, moldb_alogps_logp, moldb_logp, moldb_alogps_logs, moldb_smiles, moldb_pka, moldb_formula, moldb_average_mass, moldb_inchi, moldb_mono_mass, moldb_inchikey, moldb_alogps_solubility, moldb_id, moldb_iupac, structure_source, duplicate_id, old_dfc_id, dfc_name, compound_source, flavornet_id, goodscent_id, superscent_id, phenolexplorer_metabolite_id, kingdom, superklass, klass, subklass, direct_parent, molecular_framework, chembl_id, chemspider_id, meta_cyc_id, foodcomex, phytohub_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{legacy_id}', '{type}', '{public_id}', '{name}', '{export}', '{state}', '{annotation_quality}', '{description}', '{cas_number}', '{melting_point}', '{protein_formula}', '{protein_weight}', '{experimental_solubility}', '{experimental_logp}', '{hydrophobicity}', '{isoelectric_point}', '{metabolism}', '{kegg_compound_id}', '{pubchem_compound_id}', '{pubchem_substance_id}', '{chebi_id}', '{het_id}', '{uniprot_id}', '{uniprot_name}', '{genbank_id}', '{wikipedia_id}', '{synthesis_citations}', '{general_citations}', '{comments}', '{protein_structure_file_name}', '{protein_structure_content_type}', '{protein_structure_file_size}', '{protein_structure_updated_at}', '{msds_file_name}', '{msds_content_type}', '{msds_file_size}', '{msds_updated_at}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}', '{phenolexplorer_id}', '{dfc_id}', '{hmdb_id}', '{duke_id}', '{drugbank_id}', '{bigg_id}', '{eafus_id}', '{knapsack_id}', '{boiling_point}', '{boiling_point_reference}', '{charge}', '{charge_reference}', '{density}', '{density_reference}', '{optical_rotation}', '{optical_rotation_reference}', '{percent_composition}', '{percent_composition_reference}', '{physical_description}', '{physical_description_reference}', '{refractive_index}', '{refractive_index_reference}', '{uv_index}', '{uv_index_reference}', '{experimental_pka}', '{experimental_pka_reference}', '{experimental_solubility_reference}', '{experimental_logp_reference}', '{hydrophobicity_reference}', '{isoelectric_point_reference}', '{melting_point_reference}', '{moldb_alogps_logp}', '{moldb_logp}', '{moldb_alogps_logs}', '{moldb_smiles}', '{moldb_pka}', '{moldb_formula}', '{moldb_average_mass}', '{moldb_inchi}', '{moldb_mono_mass}', '{moldb_inchikey}', '{moldb_alogps_solubility}', '{moldb_id}', '{moldb_iupac}', '{structure_source}', '{duplicate_id}', '{old_dfc_id}', '{dfc_name}', '{compound_source}', '{flavornet_id}', '{goodscent_id}', '{superscent_id}', '{phenolexplorer_metabolite_id}', '{kingdom}', '{superklass}', '{klass}', '{subklass}', '{direct_parent}', '{molecular_framework}', '{chembl_id}', '{chemspider_id}', '{meta_cyc_id}', '{foodcomex}', '{phytohub_id}')"
        Else
            Return $"('{legacy_id}', '{type}', '{public_id}', '{name}', '{export}', '{state}', '{annotation_quality}', '{description}', '{cas_number}', '{melting_point}', '{protein_formula}', '{protein_weight}', '{experimental_solubility}', '{experimental_logp}', '{hydrophobicity}', '{isoelectric_point}', '{metabolism}', '{kegg_compound_id}', '{pubchem_compound_id}', '{pubchem_substance_id}', '{chebi_id}', '{het_id}', '{uniprot_id}', '{uniprot_name}', '{genbank_id}', '{wikipedia_id}', '{synthesis_citations}', '{general_citations}', '{comments}', '{protein_structure_file_name}', '{protein_structure_content_type}', '{protein_structure_file_size}', '{protein_structure_updated_at}', '{msds_file_name}', '{msds_content_type}', '{msds_file_size}', '{msds_updated_at}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}', '{phenolexplorer_id}', '{dfc_id}', '{hmdb_id}', '{duke_id}', '{drugbank_id}', '{bigg_id}', '{eafus_id}', '{knapsack_id}', '{boiling_point}', '{boiling_point_reference}', '{charge}', '{charge_reference}', '{density}', '{density_reference}', '{optical_rotation}', '{optical_rotation_reference}', '{percent_composition}', '{percent_composition_reference}', '{physical_description}', '{physical_description_reference}', '{refractive_index}', '{refractive_index_reference}', '{uv_index}', '{uv_index_reference}', '{experimental_pka}', '{experimental_pka_reference}', '{experimental_solubility_reference}', '{experimental_logp_reference}', '{hydrophobicity_reference}', '{isoelectric_point_reference}', '{melting_point_reference}', '{moldb_alogps_logp}', '{moldb_logp}', '{moldb_alogps_logs}', '{moldb_smiles}', '{moldb_pka}', '{moldb_formula}', '{moldb_average_mass}', '{moldb_inchi}', '{moldb_mono_mass}', '{moldb_inchikey}', '{moldb_alogps_solubility}', '{moldb_id}', '{moldb_iupac}', '{structure_source}', '{duplicate_id}', '{old_dfc_id}', '{dfc_name}', '{compound_source}', '{flavornet_id}', '{goodscent_id}', '{superscent_id}', '{phenolexplorer_metabolite_id}', '{kingdom}', '{superklass}', '{klass}', '{subklass}', '{direct_parent}', '{molecular_framework}', '{chembl_id}', '{chemspider_id}', '{meta_cyc_id}', '{foodcomex}', '{phytohub_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `compounds` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}', '{105}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, cas_number, melting_point, protein_formula, protein_weight, experimental_solubility, experimental_logp, hydrophobicity, isoelectric_point, metabolism, kegg_compound_id, pubchem_compound_id, pubchem_substance_id, chebi_id, het_id, uniprot_id, uniprot_name, genbank_id, wikipedia_id, synthesis_citations, general_citations, comments, protein_structure_file_name, protein_structure_content_type, protein_structure_file_size, MySqlScript.ToMySqlDateTimeString(protein_structure_updated_at), msds_file_name, msds_content_type, msds_file_size, MySqlScript.ToMySqlDateTimeString(msds_updated_at), creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), phenolexplorer_id, dfc_id, hmdb_id, duke_id, drugbank_id, bigg_id, eafus_id, knapsack_id, boiling_point, boiling_point_reference, charge, charge_reference, density, density_reference, optical_rotation, optical_rotation_reference, percent_composition, percent_composition_reference, physical_description, physical_description_reference, refractive_index, refractive_index_reference, uv_index, uv_index_reference, experimental_pka, experimental_pka_reference, experimental_solubility_reference, experimental_logp_reference, hydrophobicity_reference, isoelectric_point_reference, melting_point_reference, moldb_alogps_logp, moldb_logp, moldb_alogps_logs, moldb_smiles, moldb_pka, moldb_formula, moldb_average_mass, moldb_inchi, moldb_mono_mass, moldb_inchikey, moldb_alogps_solubility, moldb_id, moldb_iupac, structure_source, duplicate_id, old_dfc_id, dfc_name, compound_source, flavornet_id, goodscent_id, superscent_id, phenolexplorer_metabolite_id, kingdom, superklass, klass, subklass, direct_parent, molecular_framework, chembl_id, chemspider_id, meta_cyc_id, foodcomex, phytohub_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `compounds` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `cas_number`, `melting_point`, `protein_formula`, `protein_weight`, `experimental_solubility`, `experimental_logp`, `hydrophobicity`, `isoelectric_point`, `metabolism`, `kegg_compound_id`, `pubchem_compound_id`, `pubchem_substance_id`, `chebi_id`, `het_id`, `uniprot_id`, `uniprot_name`, `genbank_id`, `wikipedia_id`, `synthesis_citations`, `general_citations`, `comments`, `protein_structure_file_name`, `protein_structure_content_type`, `protein_structure_file_size`, `protein_structure_updated_at`, `msds_file_name`, `msds_content_type`, `msds_file_size`, `msds_updated_at`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `phenolexplorer_id`, `dfc_id`, `hmdb_id`, `duke_id`, `drugbank_id`, `bigg_id`, `eafus_id`, `knapsack_id`, `boiling_point`, `boiling_point_reference`, `charge`, `charge_reference`, `density`, `density_reference`, `optical_rotation`, `optical_rotation_reference`, `percent_composition`, `percent_composition_reference`, `physical_description`, `physical_description_reference`, `refractive_index`, `refractive_index_reference`, `uv_index`, `uv_index_reference`, `experimental_pka`, `experimental_pka_reference`, `experimental_solubility_reference`, `experimental_logp_reference`, `hydrophobicity_reference`, `isoelectric_point_reference`, `melting_point_reference`, `moldb_alogps_logp`, `moldb_logp`, `moldb_alogps_logs`, `moldb_smiles`, `moldb_pka`, `moldb_formula`, `moldb_average_mass`, `moldb_inchi`, `moldb_mono_mass`, `moldb_inchikey`, `moldb_alogps_solubility`, `moldb_id`, `moldb_iupac`, `structure_source`, `duplicate_id`, `old_dfc_id`, `dfc_name`, `compound_source`, `flavornet_id`, `goodscent_id`, `superscent_id`, `phenolexplorer_metabolite_id`, `kingdom`, `superklass`, `klass`, `subklass`, `direct_parent`, `molecular_framework`, `chembl_id`, `chemspider_id`, `meta_cyc_id`, `foodcomex`, `phytohub_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}', '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}', '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}', '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}', '{105}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, legacy_id, type, public_id, name, export, state, annotation_quality, description, cas_number, melting_point, protein_formula, protein_weight, experimental_solubility, experimental_logp, hydrophobicity, isoelectric_point, metabolism, kegg_compound_id, pubchem_compound_id, pubchem_substance_id, chebi_id, het_id, uniprot_id, uniprot_name, genbank_id, wikipedia_id, synthesis_citations, general_citations, comments, protein_structure_file_name, protein_structure_content_type, protein_structure_file_size, MySqlScript.ToMySqlDateTimeString(protein_structure_updated_at), msds_file_name, msds_content_type, msds_file_size, MySqlScript.ToMySqlDateTimeString(msds_updated_at), creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), phenolexplorer_id, dfc_id, hmdb_id, duke_id, drugbank_id, bigg_id, eafus_id, knapsack_id, boiling_point, boiling_point_reference, charge, charge_reference, density, density_reference, optical_rotation, optical_rotation_reference, percent_composition, percent_composition_reference, physical_description, physical_description_reference, refractive_index, refractive_index_reference, uv_index, uv_index_reference, experimental_pka, experimental_pka_reference, experimental_solubility_reference, experimental_logp_reference, hydrophobicity_reference, isoelectric_point_reference, melting_point_reference, moldb_alogps_logp, moldb_logp, moldb_alogps_logs, moldb_smiles, moldb_pka, moldb_formula, moldb_average_mass, moldb_inchi, moldb_mono_mass, moldb_inchikey, moldb_alogps_solubility, moldb_id, moldb_iupac, structure_source, duplicate_id, old_dfc_id, dfc_name, compound_source, flavornet_id, goodscent_id, superscent_id, phenolexplorer_metabolite_id, kingdom, superklass, klass, subklass, direct_parent, molecular_framework, chembl_id, chemspider_id, meta_cyc_id, foodcomex, phytohub_id)
        Else
        Return String.Format(REPLACE_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, cas_number, melting_point, protein_formula, protein_weight, experimental_solubility, experimental_logp, hydrophobicity, isoelectric_point, metabolism, kegg_compound_id, pubchem_compound_id, pubchem_substance_id, chebi_id, het_id, uniprot_id, uniprot_name, genbank_id, wikipedia_id, synthesis_citations, general_citations, comments, protein_structure_file_name, protein_structure_content_type, protein_structure_file_size, MySqlScript.ToMySqlDateTimeString(protein_structure_updated_at), msds_file_name, msds_content_type, msds_file_size, MySqlScript.ToMySqlDateTimeString(msds_updated_at), creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), phenolexplorer_id, dfc_id, hmdb_id, duke_id, drugbank_id, bigg_id, eafus_id, knapsack_id, boiling_point, boiling_point_reference, charge, charge_reference, density, density_reference, optical_rotation, optical_rotation_reference, percent_composition, percent_composition_reference, physical_description, physical_description_reference, refractive_index, refractive_index_reference, uv_index, uv_index_reference, experimental_pka, experimental_pka_reference, experimental_solubility_reference, experimental_logp_reference, hydrophobicity_reference, isoelectric_point_reference, melting_point_reference, moldb_alogps_logp, moldb_logp, moldb_alogps_logs, moldb_smiles, moldb_pka, moldb_formula, moldb_average_mass, moldb_inchi, moldb_mono_mass, moldb_inchikey, moldb_alogps_solubility, moldb_id, moldb_iupac, structure_source, duplicate_id, old_dfc_id, dfc_name, compound_source, flavornet_id, goodscent_id, superscent_id, phenolexplorer_metabolite_id, kingdom, superklass, klass, subklass, direct_parent, molecular_framework, chembl_id, chemspider_id, meta_cyc_id, foodcomex, phytohub_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `compounds` SET `id`='{0}', `legacy_id`='{1}', `type`='{2}', `public_id`='{3}', `name`='{4}', `export`='{5}', `state`='{6}', `annotation_quality`='{7}', `description`='{8}', `cas_number`='{9}', `melting_point`='{10}', `protein_formula`='{11}', `protein_weight`='{12}', `experimental_solubility`='{13}', `experimental_logp`='{14}', `hydrophobicity`='{15}', `isoelectric_point`='{16}', `metabolism`='{17}', `kegg_compound_id`='{18}', `pubchem_compound_id`='{19}', `pubchem_substance_id`='{20}', `chebi_id`='{21}', `het_id`='{22}', `uniprot_id`='{23}', `uniprot_name`='{24}', `genbank_id`='{25}', `wikipedia_id`='{26}', `synthesis_citations`='{27}', `general_citations`='{28}', `comments`='{29}', `protein_structure_file_name`='{30}', `protein_structure_content_type`='{31}', `protein_structure_file_size`='{32}', `protein_structure_updated_at`='{33}', `msds_file_name`='{34}', `msds_content_type`='{35}', `msds_file_size`='{36}', `msds_updated_at`='{37}', `creator_id`='{38}', `updater_id`='{39}', `created_at`='{40}', `updated_at`='{41}', `phenolexplorer_id`='{42}', `dfc_id`='{43}', `hmdb_id`='{44}', `duke_id`='{45}', `drugbank_id`='{46}', `bigg_id`='{47}', `eafus_id`='{48}', `knapsack_id`='{49}', `boiling_point`='{50}', `boiling_point_reference`='{51}', `charge`='{52}', `charge_reference`='{53}', `density`='{54}', `density_reference`='{55}', `optical_rotation`='{56}', `optical_rotation_reference`='{57}', `percent_composition`='{58}', `percent_composition_reference`='{59}', `physical_description`='{60}', `physical_description_reference`='{61}', `refractive_index`='{62}', `refractive_index_reference`='{63}', `uv_index`='{64}', `uv_index_reference`='{65}', `experimental_pka`='{66}', `experimental_pka_reference`='{67}', `experimental_solubility_reference`='{68}', `experimental_logp_reference`='{69}', `hydrophobicity_reference`='{70}', `isoelectric_point_reference`='{71}', `melting_point_reference`='{72}', `moldb_alogps_logp`='{73}', `moldb_logp`='{74}', `moldb_alogps_logs`='{75}', `moldb_smiles`='{76}', `moldb_pka`='{77}', `moldb_formula`='{78}', `moldb_average_mass`='{79}', `moldb_inchi`='{80}', `moldb_mono_mass`='{81}', `moldb_inchikey`='{82}', `moldb_alogps_solubility`='{83}', `moldb_id`='{84}', `moldb_iupac`='{85}', `structure_source`='{86}', `duplicate_id`='{87}', `old_dfc_id`='{88}', `dfc_name`='{89}', `compound_source`='{90}', `flavornet_id`='{91}', `goodscent_id`='{92}', `superscent_id`='{93}', `phenolexplorer_metabolite_id`='{94}', `kingdom`='{95}', `superklass`='{96}', `klass`='{97}', `subklass`='{98}', `direct_parent`='{99}', `molecular_framework`='{100}', `chembl_id`='{101}', `chemspider_id`='{102}', `meta_cyc_id`='{103}', `foodcomex`='{104}', `phytohub_id`='{105}' WHERE `id` = '{106}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, legacy_id, type, public_id, name, export, state, annotation_quality, description, cas_number, melting_point, protein_formula, protein_weight, experimental_solubility, experimental_logp, hydrophobicity, isoelectric_point, metabolism, kegg_compound_id, pubchem_compound_id, pubchem_substance_id, chebi_id, het_id, uniprot_id, uniprot_name, genbank_id, wikipedia_id, synthesis_citations, general_citations, comments, protein_structure_file_name, protein_structure_content_type, protein_structure_file_size, MySqlScript.ToMySqlDateTimeString(protein_structure_updated_at), msds_file_name, msds_content_type, msds_file_size, MySqlScript.ToMySqlDateTimeString(msds_updated_at), creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), phenolexplorer_id, dfc_id, hmdb_id, duke_id, drugbank_id, bigg_id, eafus_id, knapsack_id, boiling_point, boiling_point_reference, charge, charge_reference, density, density_reference, optical_rotation, optical_rotation_reference, percent_composition, percent_composition_reference, physical_description, physical_description_reference, refractive_index, refractive_index_reference, uv_index, uv_index_reference, experimental_pka, experimental_pka_reference, experimental_solubility_reference, experimental_logp_reference, hydrophobicity_reference, isoelectric_point_reference, melting_point_reference, moldb_alogps_logp, moldb_logp, moldb_alogps_logs, moldb_smiles, moldb_pka, moldb_formula, moldb_average_mass, moldb_inchi, moldb_mono_mass, moldb_inchikey, moldb_alogps_solubility, moldb_id, moldb_iupac, structure_source, duplicate_id, old_dfc_id, dfc_name, compound_source, flavornet_id, goodscent_id, superscent_id, phenolexplorer_metabolite_id, kingdom, superklass, klass, subklass, direct_parent, molecular_framework, chembl_id, chemspider_id, meta_cyc_id, foodcomex, phytohub_id, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As compounds
                         Return DirectCast(MyClass.MemberwiseClone, compounds)
                     End Function
End Class


End Namespace
