#Region "Microsoft.VisualBasic::3c8fefbe2ac9540ec8946974783d319c, metadb\Massbank\Public\HERB\HERB_target_info.vb"

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
    '     File Size: 639 B


    '     Class HERB_target_info
    ' 
    '         Properties: Chromosome, Db_xrefs, Description, Gene_alias, Gene_id
    '                     Gene_name, Map_location, Target_id, Tax_id, TTD_target_id
    '                     TTD_target_type, Type_of_gene
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace HERB

    Public Class HERB_target_info

        Public Property Target_id As String
        Public Property Tax_id As String
        Public Property Gene_id As String
        Public Property Gene_name As String
        Public Property Gene_alias As String
        Public Property Db_xrefs As String
        Public Property Chromosome As String
        Public Property Map_location As String
        Public Property Description As String
        Public Property Type_of_gene As String
        Public Property TTD_target_id As String
        Public Property TTD_target_type As String

    End Class
End Namespace
