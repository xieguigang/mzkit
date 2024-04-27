#Region "Microsoft.VisualBasic::8975116fc6a6cc880b595f2b1eae91e3, G:/mzkit/src/metadb/Massbank//Public/TMIC/FooDB/Content.vb"

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

    '   Total Lines: 27
    '    Code Lines: 13
    ' Comment Lines: 11
    '   Blank Lines: 3
    '     File Size: 1.08 KB


    '     Class Content
    ' 
    '         Properties: food_id, orig_content, orig_food_part, orig_max, orig_min
    '                     orig_unit, preparation_type, source_id, source_type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace TMIC.FooDB

    Public Class Content

        Public Property source_id As String
        Public Property source_type As String
        Public Property food_id As String
        '  Public Property orig_food_common_name As String
        ' Public Property orig_food_scientific_name As String
        Public Property orig_food_part As String
        '  Public Property orig_source_id As String
        ' Public Property orig_source_name As String
        Public Property orig_content As String
        Public Property orig_min As String
        Public Property orig_max As String
        Public Property orig_unit As String
        ' Public Property orig_citation As String
        ' Public Property citation As String
        ' Public Property citation_type As String
        ' Public Property orig_method As String
        ' Public Property orig_unit_expression As String
        ' Public Property standard_content As String
        Public Property preparation_type As String
        ' Public Property export As String

    End Class
End Namespace
