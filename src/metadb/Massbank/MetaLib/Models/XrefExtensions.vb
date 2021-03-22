#Region "Microsoft.VisualBasic::7ffd1418ecfbd0ef715753b35b9415a4, src\metadb\Massbank\MetaLib\Models\XrefExtensions.vb"

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

    '     Module XrefExtensions
    ' 
    '         Function: FormatChEbiID, FormatHMDBId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace MetaLib.Models

    <HideModuleName>
    Public Module XrefExtensions

        <Extension>
        Public Function FormatChEbiID(id As String) As String
            id = id.Match("\d+")

            If Val(id) <= 0 Then
                Return ""
            Else
                Return $"CHEBI:{id}"
            End If
        End Function

        <Extension>
        Public Function FormatHMDBId(id As String) As String
            If Not xref.IsHMDB(id) Then
                Return ""
            Else
                id = id.Match("\d+").ParseInteger.ToString

                If Val(id) <= 0 Then
                    Return ""
                Else
                    Return $"HMDB{id.FormatZero("0000000")}"
                End If
            End If
        End Function
    End Module
End Namespace
