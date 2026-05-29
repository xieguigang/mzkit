#Region "Microsoft.VisualBasic::518c412af6ba93b6d7d1a9e1c686300e, metadb\Massbank\MetaLib\Match\ComparesIdXrefInteger.vb"

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

    '   Total Lines: 60
    '    Code Lines: 48 (80.00%)
    ' Comment Lines: 3 (5.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (15.00%)
    '     File Size: 1.81 KB


    '     Class ComparesIdXrefInteger
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ParseInteger
    ' 
    '         Sub: DoCompares
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language


Namespace Metabolite.CrossReference

    Friend Class ComparesIdXrefInteger

        Dim intId As i32 = 0
        Dim check As MetaEquals.SimpleCheck

        Sub New(check As MetaEquals.SimpleCheck)
            Me.check = check
        End Sub

        Public Sub DoCompares(a$, b$)
            a = Strings.Trim(a)
            b = Strings.Trim(b)

            ' 2019-03-25
            ' 都没有该数据库的编号,即改数据库之中还没有登录该物质
            ' 则不应该认为是不一样的
            If a = b AndAlso a = "NA" Then
                check.yes()
                Return
            ElseIf (a.StringEmpty OrElse b.StringEmpty) AndAlso (a = "NA" OrElse b = "NA") Then
                check.yes()
                Return
            End If

            If a = b AndAlso Not a.StringEmpty Then
                check.yes()
                Return
            ElseIf a.StringEmpty OrElse b.StringEmpty Then
                check.no()
                Return
            End If

            If ((intId = ParseInteger(a)) = ParseInteger(b)) Then
                If intId.Equals(0) Then
                    check.no()
                Else
                    check.yes()
                End If
            Else
                check.no()
            End If
        End Sub

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
    End Class
End Namespace
