#Region "Microsoft.VisualBasic::870ac24ced65fb2363573dcc292f8afc, metadb\SMILES\Embedding\AtomLink.vb"

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

    '   Total Lines: 21
    '    Code Lines: 17 (80.95%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (19.05%)
    '     File Size: 587 B


    '     Class AtomLink
    ' 
    '         Properties: atom1, atom2, score, v0, vertex
    '                     vk
    ' 
    '         Function: GetSortUniqueId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Embedding

    Public Class AtomLink

        Public Property atom1 As String
        Public Property atom2 As String
        Public Property score As Double
        Public Property vk As Double
        Public Property v0 As Double
        Public Property vertex As Dictionary(Of String, String())

        Friend Function GetSortUniqueId() As String
            If atom1 >= atom2 Then
                Return atom1 & "|" & atom2
            Else
                Return atom2 & "|" & atom1
            End If
        End Function

    End Class
End Namespace
