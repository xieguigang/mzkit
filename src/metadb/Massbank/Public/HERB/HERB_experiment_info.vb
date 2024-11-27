#Region "Microsoft.VisualBasic::1025b7d99cd0a268d5d25cb725d2ee1d, metadb\Massbank\Public\HERB\HERB_experiment_info.vb"

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

    '   Total Lines: 30
    '    Code Lines: 26 (86.67%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (13.33%)
    '     File Size: 1.31 KB


    '     Class HERB_experiment_info
    ' 
    '         Properties: Cell_line, Cell_Type, Control_condition, Control_samples, Drug_delivery
    '                     EXP_id, Experiment_special_pretreatment, Experiment_subject, Experiment_subject_detail, Experiment_type
    '                     GSE_id, Herb_ingredient, Herb_ingredient_id, Herb_ingredient_name, Organism
    '                     Plat_info, Sequence_type, Strain, Tissue, Treatment_condition
    '                     Treatment_samples
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace HERB

    Public Class HERB_experiment_info

        Public Property EXP_id As String
        <Column("Herb/ingredient_id")> Public Property Herb_ingredient_id As String
        <Column("Herb/ingredient_name")> Public Property Herb_ingredient_name As String
        <Column("Herb/ingredient")> Public Property Herb_ingredient As String
        Public Property GSE_id As String
        Public Property Organism As String
        Public Property Strain As String
        Public Property Tissue As String
        Public Property Cell_Type As String
        Public Property Cell_line As String
        Public Property Experiment_type As String
        Public Property Sequence_type As String
        Public Property Experiment_subject As String
        Public Property Experiment_subject_detail As String
        Public Property Experiment_special_pretreatment As String
        Public Property Control_condition As String
        Public Property Control_samples As String
        Public Property Treatment_condition As String
        Public Property Treatment_samples As String
        Public Property Drug_delivery As String
        Public Property Plat_info As String

    End Class
End Namespace
