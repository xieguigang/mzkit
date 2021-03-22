#Region "Microsoft.VisualBasic::93116417cc5d6a04ec2f56be22f2e5ff, src\metadb\FooDB\FooDB\mysql\foodcomex_compounds.vb"

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

    ' Class foodcomex_compounds
    ' 
    '     Properties: admin_user_id, available_spectra, cas_number, compound_id, contact_address
    '                 contact_email, contact_name, created_at, delivery_time, description
    '                 elemental_formula, experimental_logp, experimental_solubility, export, food_of_origin
    '                 id, maximum_quantity, melting_point, minimum_quantity, origin
    '                 production_method_reference_content_type, production_method_reference_file_name, production_method_reference_file_size, production_method_reference_text, production_method_reference_updated_at
    '                 public_id, purity, quantity_units, spectra_details, stability
    '                 storage_condition, storage_conditions, storage_form, taxonomy_class, taxonomy_family
    '                 updated_at
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
''' DROP TABLE IF EXISTS `foodcomex_compounds`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `foodcomex_compounds` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `compound_id` int(11) NOT NULL,
'''   `origin` varchar(255) DEFAULT NULL,
'''   `storage_form` varchar(255) DEFAULT NULL,
'''   `maximum_quantity` varchar(255) DEFAULT NULL,
'''   `storage_condition` varchar(255) DEFAULT NULL,
'''   `contact_name` varchar(255) DEFAULT NULL,
'''   `contact_address` text,
'''   `contact_email` varchar(255) DEFAULT NULL,
'''   `created_at` datetime NOT NULL,
'''   `updated_at` datetime NOT NULL,
'''   `export` tinyint(1) DEFAULT NULL,
'''   `purity` text,
'''   `description` text,
'''   `spectra_details` varchar(255) DEFAULT NULL,
'''   `delivery_time` varchar(255) DEFAULT NULL,
'''   `stability` varchar(255) DEFAULT NULL,
'''   `admin_user_id` int(11) DEFAULT NULL,
'''   `public_id` varchar(255) NOT NULL,
'''   `cas_number` varchar(255) DEFAULT '',
'''   `taxonomy_class` varchar(255) DEFAULT '',
'''   `taxonomy_family` varchar(255) DEFAULT '',
'''   `experimental_logp` varchar(255) DEFAULT '',
'''   `experimental_solubility` varchar(255) DEFAULT '',
'''   `melting_point` varchar(255) DEFAULT '',
'''   `food_of_origin` varchar(255) DEFAULT '',
'''   `production_method_reference_text` text,
'''   `production_method_reference_file_name` varchar(255) DEFAULT NULL,
'''   `production_method_reference_content_type` varchar(255) DEFAULT NULL,
'''   `production_method_reference_file_size` int(11) DEFAULT NULL,
'''   `production_method_reference_updated_at` datetime DEFAULT NULL,
'''   `elemental_formula` varchar(255) DEFAULT '',
'''   `minimum_quantity` varchar(255) DEFAULT '',
'''   `quantity_units` varchar(255) DEFAULT '',
'''   `available_spectra` text,
'''   `storage_conditions` text,
'''   PRIMARY KEY (`id`),
'''   KEY `index_foodcomex_compounds_on_compound_id` (`compound_id`),
'''   KEY `index_foodcomex_compounds_on_admin_user_id` (`admin_user_id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=1048 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("foodcomex_compounds", Database:="foodb", SchemaSQL:="
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
) ENGINE=InnoDB AUTO_INCREMENT=1048 DEFAULT CHARSET=utf8;")>
Public Class foodcomex_compounds: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("compound_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("origin"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="origin")> Public Property origin As String
    <DatabaseField("storage_form"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="storage_form")> Public Property storage_form As String
    <DatabaseField("maximum_quantity"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="maximum_quantity")> Public Property maximum_quantity As String
    <DatabaseField("storage_condition"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="storage_condition")> Public Property storage_condition As String
    <DatabaseField("contact_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="contact_name")> Public Property contact_name As String
    <DatabaseField("contact_address"), DataType(MySqlDbType.Text), Column(Name:="contact_address")> Public Property contact_address As String
    <DatabaseField("contact_email"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="contact_email")> Public Property contact_email As String
    <DatabaseField("created_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("export"), DataType(MySqlDbType.Boolean, "1"), Column(Name:="export")> Public Property export As Boolean
    <DatabaseField("purity"), DataType(MySqlDbType.Text), Column(Name:="purity")> Public Property purity As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
    <DatabaseField("spectra_details"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="spectra_details")> Public Property spectra_details As String
    <DatabaseField("delivery_time"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="delivery_time")> Public Property delivery_time As String
    <DatabaseField("stability"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="stability")> Public Property stability As String
    <DatabaseField("admin_user_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="admin_user_id")> Public Property admin_user_id As Long
    <DatabaseField("public_id"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="public_id")> Public Property public_id As String
    <DatabaseField("cas_number"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="cas_number")> Public Property cas_number As String
    <DatabaseField("taxonomy_class"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="taxonomy_class")> Public Property taxonomy_class As String
    <DatabaseField("taxonomy_family"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="taxonomy_family")> Public Property taxonomy_family As String
    <DatabaseField("experimental_logp"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_logp")> Public Property experimental_logp As String
    <DatabaseField("experimental_solubility"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="experimental_solubility")> Public Property experimental_solubility As String
    <DatabaseField("melting_point"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="melting_point")> Public Property melting_point As String
    <DatabaseField("food_of_origin"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="food_of_origin")> Public Property food_of_origin As String
    <DatabaseField("production_method_reference_text"), DataType(MySqlDbType.Text), Column(Name:="production_method_reference_text")> Public Property production_method_reference_text As String
    <DatabaseField("production_method_reference_file_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="production_method_reference_file_name")> Public Property production_method_reference_file_name As String
    <DatabaseField("production_method_reference_content_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="production_method_reference_content_type")> Public Property production_method_reference_content_type As String
    <DatabaseField("production_method_reference_file_size"), DataType(MySqlDbType.Int64, "11"), Column(Name:="production_method_reference_file_size")> Public Property production_method_reference_file_size As Long
    <DatabaseField("production_method_reference_updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="production_method_reference_updated_at")> Public Property production_method_reference_updated_at As Date
    <DatabaseField("elemental_formula"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="elemental_formula")> Public Property elemental_formula As String
    <DatabaseField("minimum_quantity"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="minimum_quantity")> Public Property minimum_quantity As String
    <DatabaseField("quantity_units"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="quantity_units")> Public Property quantity_units As String
    <DatabaseField("available_spectra"), DataType(MySqlDbType.Text), Column(Name:="available_spectra")> Public Property available_spectra As String
    <DatabaseField("storage_conditions"), DataType(MySqlDbType.Text), Column(Name:="storage_conditions")> Public Property storage_conditions As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `foodcomex_compounds` (`compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `foodcomex_compounds` (`id`, `compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `foodcomex_compounds` (`compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `foodcomex_compounds` (`id`, `compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `foodcomex_compounds` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `foodcomex_compounds` SET `id`='{0}', `compound_id`='{1}', `origin`='{2}', `storage_form`='{3}', `maximum_quantity`='{4}', `storage_condition`='{5}', `contact_name`='{6}', `contact_address`='{7}', `contact_email`='{8}', `created_at`='{9}', `updated_at`='{10}', `export`='{11}', `purity`='{12}', `description`='{13}', `spectra_details`='{14}', `delivery_time`='{15}', `stability`='{16}', `admin_user_id`='{17}', `public_id`='{18}', `cas_number`='{19}', `taxonomy_class`='{20}', `taxonomy_family`='{21}', `experimental_logp`='{22}', `experimental_solubility`='{23}', `melting_point`='{24}', `food_of_origin`='{25}', `production_method_reference_text`='{26}', `production_method_reference_file_name`='{27}', `production_method_reference_content_type`='{28}', `production_method_reference_file_size`='{29}', `production_method_reference_updated_at`='{30}', `elemental_formula`='{31}', `minimum_quantity`='{32}', `quantity_units`='{33}', `available_spectra`='{34}', `storage_conditions`='{35}' WHERE `id` = '{36}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `foodcomex_compounds` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `foodcomex_compounds` (`id`, `compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, compound_id, origin, storage_form, maximum_quantity, storage_condition, contact_name, contact_address, contact_email, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), export, purity, description, spectra_details, delivery_time, stability, admin_user_id, public_id, cas_number, taxonomy_class, taxonomy_family, experimental_logp, experimental_solubility, melting_point, food_of_origin, production_method_reference_text, production_method_reference_file_name, production_method_reference_content_type, production_method_reference_file_size, MySqlScript.ToMySqlDateTimeString(production_method_reference_updated_at), elemental_formula, minimum_quantity, quantity_units, available_spectra, storage_conditions)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `foodcomex_compounds` (`id`, `compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, compound_id, origin, storage_form, maximum_quantity, storage_condition, contact_name, contact_address, contact_email, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), export, purity, description, spectra_details, delivery_time, stability, admin_user_id, public_id, cas_number, taxonomy_class, taxonomy_family, experimental_logp, experimental_solubility, melting_point, food_of_origin, production_method_reference_text, production_method_reference_file_name, production_method_reference_content_type, production_method_reference_file_size, MySqlScript.ToMySqlDateTimeString(production_method_reference_updated_at), elemental_formula, minimum_quantity, quantity_units, available_spectra, storage_conditions)
        Else
        Return String.Format(INSERT_SQL, compound_id, origin, storage_form, maximum_quantity, storage_condition, contact_name, contact_address, contact_email, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), export, purity, description, spectra_details, delivery_time, stability, admin_user_id, public_id, cas_number, taxonomy_class, taxonomy_family, experimental_logp, experimental_solubility, melting_point, food_of_origin, production_method_reference_text, production_method_reference_file_name, production_method_reference_content_type, production_method_reference_file_size, MySqlScript.ToMySqlDateTimeString(production_method_reference_updated_at), elemental_formula, minimum_quantity, quantity_units, available_spectra, storage_conditions)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{compound_id}', '{origin}', '{storage_form}', '{maximum_quantity}', '{storage_condition}', '{contact_name}', '{contact_address}', '{contact_email}', '{created_at}', '{updated_at}', '{export}', '{purity}', '{description}', '{spectra_details}', '{delivery_time}', '{stability}', '{admin_user_id}', '{public_id}', '{cas_number}', '{taxonomy_class}', '{taxonomy_family}', '{experimental_logp}', '{experimental_solubility}', '{melting_point}', '{food_of_origin}', '{production_method_reference_text}', '{production_method_reference_file_name}', '{production_method_reference_content_type}', '{production_method_reference_file_size}', '{production_method_reference_updated_at}', '{elemental_formula}', '{minimum_quantity}', '{quantity_units}', '{available_spectra}', '{storage_conditions}')"
        Else
            Return $"('{compound_id}', '{origin}', '{storage_form}', '{maximum_quantity}', '{storage_condition}', '{contact_name}', '{contact_address}', '{contact_email}', '{created_at}', '{updated_at}', '{export}', '{purity}', '{description}', '{spectra_details}', '{delivery_time}', '{stability}', '{admin_user_id}', '{public_id}', '{cas_number}', '{taxonomy_class}', '{taxonomy_family}', '{experimental_logp}', '{experimental_solubility}', '{melting_point}', '{food_of_origin}', '{production_method_reference_text}', '{production_method_reference_file_name}', '{production_method_reference_content_type}', '{production_method_reference_file_size}', '{production_method_reference_updated_at}', '{elemental_formula}', '{minimum_quantity}', '{quantity_units}', '{available_spectra}', '{storage_conditions}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `foodcomex_compounds` (`id`, `compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, compound_id, origin, storage_form, maximum_quantity, storage_condition, contact_name, contact_address, contact_email, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), export, purity, description, spectra_details, delivery_time, stability, admin_user_id, public_id, cas_number, taxonomy_class, taxonomy_family, experimental_logp, experimental_solubility, melting_point, food_of_origin, production_method_reference_text, production_method_reference_file_name, production_method_reference_content_type, production_method_reference_file_size, MySqlScript.ToMySqlDateTimeString(production_method_reference_updated_at), elemental_formula, minimum_quantity, quantity_units, available_spectra, storage_conditions)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `foodcomex_compounds` (`id`, `compound_id`, `origin`, `storage_form`, `maximum_quantity`, `storage_condition`, `contact_name`, `contact_address`, `contact_email`, `created_at`, `updated_at`, `export`, `purity`, `description`, `spectra_details`, `delivery_time`, `stability`, `admin_user_id`, `public_id`, `cas_number`, `taxonomy_class`, `taxonomy_family`, `experimental_logp`, `experimental_solubility`, `melting_point`, `food_of_origin`, `production_method_reference_text`, `production_method_reference_file_name`, `production_method_reference_content_type`, `production_method_reference_file_size`, `production_method_reference_updated_at`, `elemental_formula`, `minimum_quantity`, `quantity_units`, `available_spectra`, `storage_conditions`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, compound_id, origin, storage_form, maximum_quantity, storage_condition, contact_name, contact_address, contact_email, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), export, purity, description, spectra_details, delivery_time, stability, admin_user_id, public_id, cas_number, taxonomy_class, taxonomy_family, experimental_logp, experimental_solubility, melting_point, food_of_origin, production_method_reference_text, production_method_reference_file_name, production_method_reference_content_type, production_method_reference_file_size, MySqlScript.ToMySqlDateTimeString(production_method_reference_updated_at), elemental_formula, minimum_quantity, quantity_units, available_spectra, storage_conditions)
        Else
        Return String.Format(REPLACE_SQL, compound_id, origin, storage_form, maximum_quantity, storage_condition, contact_name, contact_address, contact_email, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), export, purity, description, spectra_details, delivery_time, stability, admin_user_id, public_id, cas_number, taxonomy_class, taxonomy_family, experimental_logp, experimental_solubility, melting_point, food_of_origin, production_method_reference_text, production_method_reference_file_name, production_method_reference_content_type, production_method_reference_file_size, MySqlScript.ToMySqlDateTimeString(production_method_reference_updated_at), elemental_formula, minimum_quantity, quantity_units, available_spectra, storage_conditions)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `foodcomex_compounds` SET `id`='{0}', `compound_id`='{1}', `origin`='{2}', `storage_form`='{3}', `maximum_quantity`='{4}', `storage_condition`='{5}', `contact_name`='{6}', `contact_address`='{7}', `contact_email`='{8}', `created_at`='{9}', `updated_at`='{10}', `export`='{11}', `purity`='{12}', `description`='{13}', `spectra_details`='{14}', `delivery_time`='{15}', `stability`='{16}', `admin_user_id`='{17}', `public_id`='{18}', `cas_number`='{19}', `taxonomy_class`='{20}', `taxonomy_family`='{21}', `experimental_logp`='{22}', `experimental_solubility`='{23}', `melting_point`='{24}', `food_of_origin`='{25}', `production_method_reference_text`='{26}', `production_method_reference_file_name`='{27}', `production_method_reference_content_type`='{28}', `production_method_reference_file_size`='{29}', `production_method_reference_updated_at`='{30}', `elemental_formula`='{31}', `minimum_quantity`='{32}', `quantity_units`='{33}', `available_spectra`='{34}', `storage_conditions`='{35}' WHERE `id` = '{36}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, origin, storage_form, maximum_quantity, storage_condition, contact_name, contact_address, contact_email, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), export, purity, description, spectra_details, delivery_time, stability, admin_user_id, public_id, cas_number, taxonomy_class, taxonomy_family, experimental_logp, experimental_solubility, melting_point, food_of_origin, production_method_reference_text, production_method_reference_file_name, production_method_reference_content_type, production_method_reference_file_size, MySqlScript.ToMySqlDateTimeString(production_method_reference_updated_at), elemental_formula, minimum_quantity, quantity_units, available_spectra, storage_conditions, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As foodcomex_compounds
                         Return DirectCast(MyClass.MemberwiseClone, foodcomex_compounds)
                     End Function
End Class


End Namespace
