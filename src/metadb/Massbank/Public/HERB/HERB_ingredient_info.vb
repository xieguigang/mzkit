#Region "Microsoft.VisualBasic::2925b4f49b6ed1b8a9cbff19ac7c8536, metadb\Massbank\Public\HERB\HERB_ingredient_info.vb"

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

    '   Total Lines: 28
    '    Code Lines: 23 (82.14%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (17.86%)
    '     File Size: 998 B


    '     Class HERB_ingredient_info
    ' 
    '         Properties: [Alias], CAS_id, DrugBank_id, Ingredient_formula, Ingredient_id
    '                     Ingredient_name, Ingredient_Smile, Ingredient_weight, OB_score, PubChem_id
    '                     SymMap_id, TCM_ID_id, TCMID_id, TCMSP_id
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace HERB

    Public Class HERB_ingredient_info

        Public Property Ingredient_id As String
        Public Property Ingredient_name As String
        Public Property [Alias] As String
        Public Property Ingredient_formula As String
        Public Property Ingredient_Smile As String
        Public Property Ingredient_weight As String
        Public Property OB_score As String
        Public Property CAS_id As String
        Public Property SymMap_id As String
        Public Property TCMID_id As String
        Public Property TCMSP_id As String
        <Column("TCM-ID_id")>
        Public Property TCM_ID_id As String
        Public Property PubChem_id As String
        Public Property DrugBank_id As String

        Public Overrides Function ToString() As String
            Return $"{Ingredient_name} ({Ingredient_formula})"
        End Function

    End Class
End Namespace
