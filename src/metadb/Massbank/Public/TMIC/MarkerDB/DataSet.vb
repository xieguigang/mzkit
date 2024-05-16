#Region "Microsoft.VisualBasic::41a0fdf0c9d9877a4595893129b6a4f8, metadb\Massbank\Public\TMIC\MarkerDB\DataSet.vb"

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


    ' Code Statistics:

    '   Total Lines: 124
    '    Code Lines: 73
    ' Comment Lines: 33
    '   Blank Lines: 18
    '     File Size: 4.08 KB


    '     Class DataSet
    ' 
    '         Properties: biomarker_type, biomarkers, chemicals, karyotypes, proteins
    '                     sequence_variants, version
    ' 
    '     Class biomarkers
    ' 
    '         Properties: chemical, gene, karyotype, protein
    ' 
    '     Class gene
    ' 
    '         Properties: biomarker_type, conditions, entrez_gene_id, external_link, gene_symbol
    '                     id, indication_types, position, variation
    ' 
    '     Class protein
    ' 
    '         Properties: conditions, creation_date, gene_name, id, name
    '                     uniprot_id, update_date
    ' 
    '     Class sequence_variant
    ' 
    '         Properties: external_link, id, position, reference, sequence_variant_measurements
    '                     variation
    ' 
    '     Class sequence_variant_measurement
    ' 
    '         Properties: reference
    ' 
    '     Class karyotype
    ' 
    '         Properties: biomarker_type, description, id, karyotype
    ' 
    '     Class chemical
    ' 
    '         Properties: biomarker_type, conditions, creation_date, hmdb_id, id
    '                     name, update_date
    ' 
    '     Class condition
    ' 
    '         Properties: age, biofluid, citation, concentration, condition
    '                     indication_type, sex
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace TMIC.MarkerDB.XML

    Public Class DataSet

        Public Property version As String
        Public Property biomarker_type As String
        ''' <summary>
        ''' all_chemicals.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property chemicals As chemical()
        Public Property biomarkers As biomarkers
        ''' <summary>
        ''' all_karyotypes.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property karyotypes As karyotype()
        ''' <summary>
        ''' all_sequence_variants.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property sequence_variants As sequence_variant()
        ''' <summary>
        ''' all_proteins.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property proteins As protein()
    End Class

    Public Class biomarkers
        ''' <summary>
        ''' + all_exposure_chemicals.xml
        ''' + all_diagnostic_chemicals.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property chemical As chemical()
        ''' <summary>
        ''' all_predictive_genetics.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property gene As gene()
        ''' <summary>
        ''' all_diagnostic_proteins.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property protein As protein()
        ''' <summary>
        ''' all_diagnostic_karyotypes.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property karyotype As karyotype()
    End Class

    Public Class gene
        Public Property biomarker_type As String
        Public Property id As String
        Public Property variation As String
        Public Property position As String
        Public Property external_link As String
        Public Property gene_symbol As String
        Public Property entrez_gene_id As String
        Public Property conditions As String
        Public Property indication_types As String
    End Class

    Public Class protein
        Public Property id As String
        Public Property creation_date As String
        Public Property update_date As String
        Public Property name As String
        Public Property gene_name As String
        Public Property uniprot_id As String
        Public Property conditions As condition()
    End Class

    Public Class sequence_variant : Inherits condition
        Public Property id As String
        Public Property variation As String
        Public Property position As String
        Public Property external_link As String
        Public Property reference As String
        Public Property sequence_variant_measurements As sequence_variant_measurement()
    End Class

    Public Class sequence_variant_measurement : Inherits condition
        Public Property reference As String
    End Class

    Public Class karyotype : Inherits condition

        Public Property biomarker_type As String
        Public Property id As String
        Public Property karyotype As String
        Public Property description As String

    End Class

    Public Class chemical : Inherits condition

        Public Property biomarker_type As String
        Public Property id As String
        Public Property creation_date As String
        Public Property update_date As String
        Public Property name As String
        Public Property hmdb_id As String
        Public Property conditions As condition()

    End Class

    Public Class condition

        Public Property concentration As String
        Public Property age As String
        Public Property sex As String
        Public Property biofluid As String
        Public Property condition As String
        Public Property indication_type As String
        Public Property citation As String

    End Class

End Namespace
