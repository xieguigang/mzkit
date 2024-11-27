#Region "Microsoft.VisualBasic::7f018c0179d237b084569af67d39a84e, metadb\Massbank\Public\HERB\HERB_reference_info.vb"

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

    '   Total Lines: 24
    '    Code Lines: 20 (83.33%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (16.67%)
    '     File Size: 1022 B


    '     Class HERB_reference_info
    ' 
    '         Properties: Animal_Experiment, Cell_Experiment, Clinical_Experiment, DOI, Experiment_subject
    '                     Experiment_type, Herb_ingredient, Herb_ingredient_id, Herb_ingredient_name, Journal
    '                     Not_Mentioned, Publish_Date, PubMed_id, REF_id, Reference_title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace HERB

    Public Class HERB_reference_info

        Public Property REF_id As String
        <Column("Herb/ingredient_id")> Public Property Herb_ingredient_id As String
        <Column("Herb/ingredient_name")> Public Property Herb_ingredient_name As String
        <Column("Herb/ingredient")> Public Property Herb_ingredient As String
        Public Property Reference_title As String
        Public Property PubMed_id As String
        Public Property Journal As String
        <Column("Publish.Date")> Public Property Publish_Date As String
        Public Property DOI As String
        Public Property Experiment_subject As String
        Public Property Experiment_type As String
        Public Property Animal_Experiment As String
        Public Property Cell_Experiment As String
        Public Property Clinical_Experiment As String
        Public Property Not_Mentioned As String

    End Class
End Namespace
