#Region "Microsoft.VisualBasic::5335e2356b6c4eebdf10d9cc25264ed6, metadb\Chemoinformatics\InChI\InChIStringReader.vb"

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

    '   Total Lines: 36
    '    Code Lines: 28 (77.78%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (22.22%)
    '     File Size: 1.16 KB


    '     Class InChIStringReader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) GetByPrefix, GetStringData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

Namespace IUPAC.InChI

    Friend Class InChIStringReader

        ReadOnly prefixData As String()

        Shared ReadOnly prefixes As Index(Of Char) = "chpqbtmsihfr"

        Sub New(tokens As String())
            prefixData = tokens
        End Sub

        Public Function GetByPrefix(c As Char) As String
            If c = ASCII.NUL Then
                Return prefixData.First(Function(t) Not t.First Like prefixes)
            Else
                Return GetStringData(prefixData.FirstOrDefault(Function(t) c = t.First))
            End If
        End Function

        Private Shared Function GetStringData(str As String) As String
            If str.StringEmpty Then
                Return ""
            Else
                Return str.Substring(1)
            End If
        End Function

        Public Function GetByPrefix(any As Char()) As String
            Return GetStringData(prefixData.FirstOrDefault(Function(t) any.Any(Function(cc) cc = t.First)))
        End Function
    End Class
End Namespace
