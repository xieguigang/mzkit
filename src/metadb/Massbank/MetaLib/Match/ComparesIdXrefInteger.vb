#Region "Microsoft.VisualBasic::014171f8d99980ad7e5d3d4fbdf7433c, Massbank\MetaLib\Match\ComparesIdXrefInteger.vb"

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

Namespace MetaLib

    Friend Class ComparesIdXrefInteger

        Dim intId As i32 = 0
        Dim yes, no As Action

        Sub New(yes As Action, no As Action)
            Me.no = no
            Me.yes = yes
        End Sub

        Public Sub DoCompares(a$, b$)
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
