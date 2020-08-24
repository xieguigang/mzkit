#Region "Microsoft.VisualBasic::8bccc566bd6a469582db6e03e5ee300a, src\metadb\KEGG\Origin.vb"

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

    ' Class Origin
    ' 
    '     Properties: kegg_id, ncbi_taxonomy
    ' 
    '     Function: CreateOrigin
    ' 
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' the metabolite origin
''' </summary>
Public Class Origin

    ''' <summary>
    ''' kegg id of the given compounds
    ''' </summary>
    ''' <returns></returns>
    Public Property kegg_id As String
    ''' <summary>
    ''' ncbi taxonomy id
    ''' </summary>
    ''' <returns></returns>
    Public Property ncbi_taxonomy As String()


    Public Shared Function CreateOrigin() As Origin

    End Function

End Class


