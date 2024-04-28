#Region "Microsoft.VisualBasic::7051ed9b1b2a7fcb3ad2de72770a5ddd, G:/mzkit/src/metadb/Chemoinformatics//Formula/Models/Canonical.vb"

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

    '   Total Lines: 45
    '    Code Lines: 33
    ' Comment Lines: 3
    '   Blank Lines: 9
    '     File Size: 1.83 KB


    '     Module Canonical
    ' 
    '         Function: (+2 Overloads) BuildCanonicalFormula
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Formula

    Public Module Canonical

        ''' <summary>
        ''' C -> H -> N -> O -> P -> S -> Cl -> others
        ''' </summary>
        ReadOnly orders As Index(Of String) = {"C", "H", "N", "O", "P", "S", "Cl"}
        ReadOnly order_string As String() = orders.Objects

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function BuildCanonicalFormula(formula As Formula) As String
            Return BuildCanonicalFormula(formula.CountsByElement)
        End Function

        Public Function BuildCanonicalFormula(countsByElement As Dictionary(Of String, Integer)) As String
            Dim sb As New List(Of String)
            Dim n As Integer

            For Each elementName As String In order_string
                If countsByElement.ContainsKey(elementName) Then
                    n = countsByElement(elementName)

                    If n > 0 Then
                        sb.Add(If(n = 1, elementName, elementName & n.ToString))
                    End If
                End If
            Next

            For Each element As KeyValuePair(Of String, Integer) In From e As KeyValuePair(Of String, Integer)
                                                                    In countsByElement
                                                                    Where orders.IndexOf(e.Key) = -1
                                                                    Order By e.Key
                If element.Value > 0 Then
                    sb.Add(If(element.Value = 1, element.Key, element.Key & element.Value))
                End If
            Next

            Return sb.JoinBy("")
        End Function
    End Module
End Namespace
