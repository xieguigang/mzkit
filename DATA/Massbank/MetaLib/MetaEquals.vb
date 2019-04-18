#Region "Microsoft.VisualBasic::2aa1e8bb2811e77fed42d03af0ea7ad8, Massbank\MetaLib\MetaEquals.vb"

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

    '     Module MetaEquals
    ' 
    '         Function: comparesInteger, Equals, ParseInteger
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace MetaLib

    Module MetaEquals

        Public Function Equals(meta As MetaLib, other As MetaLib) As Boolean
            Dim xref As xref = meta.xref
            Dim agree As Integer
            Dim total As Integer
            Dim yes = Sub()
                          agree += 1
                          total += 1
                      End Sub
            Dim no = Sub() total += 1

            Dim compareInteger = comparesInteger(yes, no)

            If Math.Abs(meta.mass - other.mass) > 0.3 Then
                no()
            End If

            ' 下面的这个几个数据库编号可能都是没有的
            Call compareInteger(xref.chebi, other.xref.chebi)
            Call compareInteger(xref.KEGG, other.xref.KEGG)
            Call compareInteger(xref.pubchem, other.xref.pubchem)
            Call compareInteger(xref.HMDB, other.xref.HMDB)

            If xref.CAS.Any(Function(id) other.xref.CAS.IndexOf(id) > -1) Then
                yes()
            Else
                no()
            End If

            If xref.InChIkey = other.xref.InChIkey Then
                yes()
            Else
                no()
            End If

            ' 因为name在不同的数据库之间差异有些大,所以在这里只作为可选参考
            ' 不调用no函数了
            If Strings.Trim(meta.name).TextEquals(Strings.Trim(other.name)) AndAlso Not Strings.Trim(other.name).StringEmpty Then
                yes()
            End If

            Return (agree / total) >= 0.45
        End Function

        Private Function comparesInteger(yes As Action, no As Action) As Action(Of String, String)
            Dim intId As VBInteger = 0

            Return Sub(a$, b$)
                       a = Strings.Trim(a)
                       b = Strings.Trim(b)

                       ' 2019-03-25
                       ' 都没有该数据库的编号,即改数据库之中还没有登录该物质
                       ' 则不应该认为是不一样的
                       If a = b AndAlso a = "NA" Then
                           yes()
                           Return
                       ElseIf (a.StringEmpty OrElse b.StringEmpty) AndAlso (a = "NA" OrElse b = "NA") Then
                           yes()
                           Return
                       End If

                       If a = b AndAlso Not a.StringEmpty Then
                           yes()
                           Return
                       ElseIf a.StringEmpty OrElse b.StringEmpty Then
                           no()
                           Return
                       End If

                       If ((intId = ParseInteger(a)) = ParseInteger(b)) Then
                           If intId.Equals(0) Then
                               no()
                           Else
                               yes()
                           End If
                       Else
                           no()
                       End If
                   End Sub
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function ParseInteger(xref As String) As Integer
            With xref.Match("\d+")
                If .StringEmpty Then
                    Return 0
                Else
                    Return Integer.Parse(.ByRef)
                End If
            End With
        End Function
    End Module
End Namespace
