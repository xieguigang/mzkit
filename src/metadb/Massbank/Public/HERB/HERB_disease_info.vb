#Region "Microsoft.VisualBasic::b1b394b0f89b2d543416bea4ceb87f68, metadb\Massbank\Public\HERB\HERB_disease_info.vb"

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

    '   Total Lines: 19
    '    Code Lines: 16 (84.21%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (15.79%)
    '     File Size: 695 B


    '     Class HERB_disease_info
    ' 
    '         Properties: Disease_id, Disease_name, Disease_type, DiseaseClass_MeSH, DiseaseClassName_MeSH
    '                     DisGENet_disease_id, DO_ClassId, DO_ClassName, HPO_ClassId, HPO_ClassName
    '                     UMLS_SemanticTypeId, UMLS_SemanticTypeName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace HERB

    Public Class HERB_disease_info

        Public Property Disease_id As String
        Public Property DisGENet_disease_id As String
        Public Property Disease_name As String
        Public Property Disease_type As String
        Public Property DiseaseClass_MeSH As String
        Public Property DiseaseClassName_MeSH As String
        Public Property HPO_ClassId As String
        Public Property HPO_ClassName As String
        Public Property DO_ClassId As String
        Public Property DO_ClassName As String
        Public Property UMLS_SemanticTypeId As String
        Public Property UMLS_SemanticTypeName As String

    End Class
End Namespace
