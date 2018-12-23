#Region "Microsoft.VisualBasic::a3de5f6308b3fcb27377b08aed866bcc, Massbank\Public\TMIC\FooDB\mysql\enzymes.vb"

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

    ' Class enzymes
    ' 
    '     Properties: cellular_location, chromosome, comments, created_at, creator_id
    '                 description, genatlas_id, genbank_gene_id, genbank_protein_id, gene_name
    '                 genecard_id, general_citations, general_function, go_classification, hgnc_id
    '                 hprd_id, id, locus, molecular_weight, name
    '                 organism, pathway, pdb_id, reaction, signals
    '                 specific_function, theoretical_pi, transmembrane_regions, uniprot_id, uniprot_name
    '                 updated_at, updater_id
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
''' DROP TABLE IF EXISTS `enzymes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enzymes` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `gene_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `description` mediumtext COLLATE utf8_unicode_ci,
'''   `go_classification` mediumtext COLLATE utf8_unicode_ci,
'''   `general_function` mediumtext COLLATE utf8_unicode_ci,
'''   `specific_function` mediumtext COLLATE utf8_unicode_ci,
'''   `pathway` mediumtext COLLATE utf8_unicode_ci,
'''   `reaction` mediumtext COLLATE utf8_unicode_ci,
'''   `cellular_location` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `signals` mediumtext COLLATE utf8_unicode_ci,
'''   `transmembrane_regions` mediumtext COLLATE utf8_unicode_ci,
'''   `molecular_weight` decimal(15,9) DEFAULT NULL,
'''   `theoretical_pi` decimal(15,9) DEFAULT NULL,
'''   `locus` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `chromosome` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `uniprot_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `uniprot_id` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `pdb_id` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `genbank_protein_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `genbank_gene_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `genecard_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `genatlas_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `hgnc_id` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `hprd_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `organism` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `general_citations` mediumtext COLLATE utf8_unicode_ci,
'''   `comments` mediumtext COLLATE utf8_unicode_ci,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `index_enzymes_on_name` (`name`),
'''   UNIQUE KEY `index_enzymes_on_gene_name` (`gene_name`),
'''   UNIQUE KEY `index_enzymes_on_uniprot_name` (`uniprot_name`),
'''   UNIQUE KEY `index_enzymes_on_uniprot_id` (`uniprot_id`),
'''   UNIQUE KEY `index_enzymes_on_pdb_id` (`pdb_id`),
'''   UNIQUE KEY `index_enzymes_on_genbank_protein_id` (`genbank_protein_id`),
'''   UNIQUE KEY `index_enzymes_on_genbank_gene_id` (`genbank_gene_id`),
'''   UNIQUE KEY `index_enzymes_on_genecard_id` (`genecard_id`),
'''   UNIQUE KEY `index_enzymes_on_genatlas_id` (`genatlas_id`),
'''   UNIQUE KEY `index_enzymes_on_hgnc_id` (`hgnc_id`),
'''   UNIQUE KEY `index_enzymes_on_hprd_id` (`hprd_id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=1745 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enzymes", Database:="foodb", SchemaSQL:="
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
) ENGINE=InnoDB AUTO_INCREMENT=1745 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;")>
Public Class enzymes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="name")> Public Property name As String
    <DatabaseField("gene_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="gene_name")> Public Property gene_name As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
    <DatabaseField("go_classification"), DataType(MySqlDbType.Text), Column(Name:="go_classification")> Public Property go_classification As String
    <DatabaseField("general_function"), DataType(MySqlDbType.Text), Column(Name:="general_function")> Public Property general_function As String
    <DatabaseField("specific_function"), DataType(MySqlDbType.Text), Column(Name:="specific_function")> Public Property specific_function As String
    <DatabaseField("pathway"), DataType(MySqlDbType.Text), Column(Name:="pathway")> Public Property pathway As String
    <DatabaseField("reaction"), DataType(MySqlDbType.Text), Column(Name:="reaction")> Public Property reaction As String
    <DatabaseField("cellular_location"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="cellular_location")> Public Property cellular_location As String
    <DatabaseField("signals"), DataType(MySqlDbType.Text), Column(Name:="signals")> Public Property signals As String
    <DatabaseField("transmembrane_regions"), DataType(MySqlDbType.Text), Column(Name:="transmembrane_regions")> Public Property transmembrane_regions As String
    <DatabaseField("molecular_weight"), DataType(MySqlDbType.Decimal), Column(Name:="molecular_weight")> Public Property molecular_weight As Decimal
    <DatabaseField("theoretical_pi"), DataType(MySqlDbType.Decimal), Column(Name:="theoretical_pi")> Public Property theoretical_pi As Decimal
    <DatabaseField("locus"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="locus")> Public Property locus As String
    <DatabaseField("chromosome"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="chromosome")> Public Property chromosome As String
    <DatabaseField("uniprot_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="uniprot_name")> Public Property uniprot_name As String
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("pdb_id"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="pdb_id")> Public Property pdb_id As String
    <DatabaseField("genbank_protein_id"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="genbank_protein_id")> Public Property genbank_protein_id As String
    <DatabaseField("genbank_gene_id"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="genbank_gene_id")> Public Property genbank_gene_id As String
    <DatabaseField("genecard_id"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="genecard_id")> Public Property genecard_id As String
    <DatabaseField("genatlas_id"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="genatlas_id")> Public Property genatlas_id As String
    <DatabaseField("hgnc_id"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="hgnc_id")> Public Property hgnc_id As String
    <DatabaseField("hprd_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="hprd_id")> Public Property hprd_id As String
    <DatabaseField("organism"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="organism")> Public Property organism As String
    <DatabaseField("general_citations"), DataType(MySqlDbType.Text), Column(Name:="general_citations")> Public Property general_citations As String
    <DatabaseField("comments"), DataType(MySqlDbType.Text), Column(Name:="comments")> Public Property comments As String
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `enzymes` (`name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `enzymes` (`id`, `name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `enzymes` (`name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `enzymes` (`id`, `name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `enzymes` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `enzymes` SET `id`='{0}', `name`='{1}', `gene_name`='{2}', `description`='{3}', `go_classification`='{4}', `general_function`='{5}', `specific_function`='{6}', `pathway`='{7}', `reaction`='{8}', `cellular_location`='{9}', `signals`='{10}', `transmembrane_regions`='{11}', `molecular_weight`='{12}', `theoretical_pi`='{13}', `locus`='{14}', `chromosome`='{15}', `uniprot_name`='{16}', `uniprot_id`='{17}', `pdb_id`='{18}', `genbank_protein_id`='{19}', `genbank_gene_id`='{20}', `genecard_id`='{21}', `genatlas_id`='{22}', `hgnc_id`='{23}', `hprd_id`='{24}', `organism`='{25}', `general_citations`='{26}', `comments`='{27}', `creator_id`='{28}', `updater_id`='{29}', `created_at`='{30}', `updated_at`='{31}' WHERE `id` = '{32}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `enzymes` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzymes` (`id`, `name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, gene_name, description, go_classification, general_function, specific_function, pathway, reaction, cellular_location, signals, transmembrane_regions, molecular_weight, theoretical_pi, locus, chromosome, uniprot_name, uniprot_id, pdb_id, genbank_protein_id, genbank_gene_id, genecard_id, genatlas_id, hgnc_id, hprd_id, organism, general_citations, comments, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzymes` (`id`, `name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, name, gene_name, description, go_classification, general_function, specific_function, pathway, reaction, cellular_location, signals, transmembrane_regions, molecular_weight, theoretical_pi, locus, chromosome, uniprot_name, uniprot_id, pdb_id, genbank_protein_id, genbank_gene_id, genecard_id, genatlas_id, hgnc_id, hprd_id, organism, general_citations, comments, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        Else
        Return String.Format(INSERT_SQL, name, gene_name, description, go_classification, general_function, specific_function, pathway, reaction, cellular_location, signals, transmembrane_regions, molecular_weight, theoretical_pi, locus, chromosome, uniprot_name, uniprot_id, pdb_id, genbank_protein_id, genbank_gene_id, genecard_id, genatlas_id, hgnc_id, hprd_id, organism, general_citations, comments, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{name}', '{gene_name}', '{description}', '{go_classification}', '{general_function}', '{specific_function}', '{pathway}', '{reaction}', '{cellular_location}', '{signals}', '{transmembrane_regions}', '{molecular_weight}', '{theoretical_pi}', '{locus}', '{chromosome}', '{uniprot_name}', '{uniprot_id}', '{pdb_id}', '{genbank_protein_id}', '{genbank_gene_id}', '{genecard_id}', '{genatlas_id}', '{hgnc_id}', '{hprd_id}', '{organism}', '{general_citations}', '{comments}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}')"
        Else
            Return $"('{name}', '{gene_name}', '{description}', '{go_classification}', '{general_function}', '{specific_function}', '{pathway}', '{reaction}', '{cellular_location}', '{signals}', '{transmembrane_regions}', '{molecular_weight}', '{theoretical_pi}', '{locus}', '{chromosome}', '{uniprot_name}', '{uniprot_id}', '{pdb_id}', '{genbank_protein_id}', '{genbank_gene_id}', '{genecard_id}', '{genatlas_id}', '{hgnc_id}', '{hprd_id}', '{organism}', '{general_citations}', '{comments}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `enzymes` (`id`, `name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, gene_name, description, go_classification, general_function, specific_function, pathway, reaction, cellular_location, signals, transmembrane_regions, molecular_weight, theoretical_pi, locus, chromosome, uniprot_name, uniprot_id, pdb_id, genbank_protein_id, genbank_gene_id, genecard_id, genatlas_id, hgnc_id, hprd_id, organism, general_citations, comments, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `enzymes` (`id`, `name`, `gene_name`, `description`, `go_classification`, `general_function`, `specific_function`, `pathway`, `reaction`, `cellular_location`, `signals`, `transmembrane_regions`, `molecular_weight`, `theoretical_pi`, `locus`, `chromosome`, `uniprot_name`, `uniprot_id`, `pdb_id`, `genbank_protein_id`, `genbank_gene_id`, `genecard_id`, `genatlas_id`, `hgnc_id`, `hprd_id`, `organism`, `general_citations`, `comments`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, name, gene_name, description, go_classification, general_function, specific_function, pathway, reaction, cellular_location, signals, transmembrane_regions, molecular_weight, theoretical_pi, locus, chromosome, uniprot_name, uniprot_id, pdb_id, genbank_protein_id, genbank_gene_id, genecard_id, genatlas_id, hgnc_id, hprd_id, organism, general_citations, comments, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        Else
        Return String.Format(REPLACE_SQL, name, gene_name, description, go_classification, general_function, specific_function, pathway, reaction, cellular_location, signals, transmembrane_regions, molecular_weight, theoretical_pi, locus, chromosome, uniprot_name, uniprot_id, pdb_id, genbank_protein_id, genbank_gene_id, genecard_id, genatlas_id, hgnc_id, hprd_id, organism, general_citations, comments, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `enzymes` SET `id`='{0}', `name`='{1}', `gene_name`='{2}', `description`='{3}', `go_classification`='{4}', `general_function`='{5}', `specific_function`='{6}', `pathway`='{7}', `reaction`='{8}', `cellular_location`='{9}', `signals`='{10}', `transmembrane_regions`='{11}', `molecular_weight`='{12}', `theoretical_pi`='{13}', `locus`='{14}', `chromosome`='{15}', `uniprot_name`='{16}', `uniprot_id`='{17}', `pdb_id`='{18}', `genbank_protein_id`='{19}', `genbank_gene_id`='{20}', `genecard_id`='{21}', `genatlas_id`='{22}', `hgnc_id`='{23}', `hprd_id`='{24}', `organism`='{25}', `general_citations`='{26}', `comments`='{27}', `creator_id`='{28}', `updater_id`='{29}', `created_at`='{30}', `updated_at`='{31}' WHERE `id` = '{32}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, name, gene_name, description, go_classification, general_function, specific_function, pathway, reaction, cellular_location, signals, transmembrane_regions, molecular_weight, theoretical_pi, locus, chromosome, uniprot_name, uniprot_id, pdb_id, genbank_protein_id, genbank_gene_id, genecard_id, genatlas_id, hgnc_id, hprd_id, organism, general_citations, comments, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As enzymes
                         Return DirectCast(MyClass.MemberwiseClone, enzymes)
                     End Function
End Class


End Namespace
