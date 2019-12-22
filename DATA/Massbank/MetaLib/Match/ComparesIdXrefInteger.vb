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