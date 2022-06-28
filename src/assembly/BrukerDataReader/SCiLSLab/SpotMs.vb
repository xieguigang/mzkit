Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace SCiLSLab

    Public Class MsPack : Inherits PackFile

        Public Property matrix As SpotMs()
        Public Property mz As Double()

        Public Shared Function ParseFile(file As Stream, Optional println As Action(Of String) = Nothing) As MsPack
            Dim pull As New MsPack

            If println Is Nothing Then
                println = Sub()
                              ' do nothing
                          End Sub
            End If

            Dim spots As SpotMs() = ParseTable(file, pull, AddressOf SpotMs.Parse, println).ToArray
            Dim headerLine As String = pull.metadata(".header")
            Dim mz As Double() = headerLine _
                .Split(";"c) _
                .Skip(1) _
                .Select(AddressOf Conversion.Val) _
                .ToArray

            pull.mz = mz
            pull.matrix = spots

            Return pull
        End Function
    End Class

    Public Class SpotMs

        Public Property spot_id As String
        Public Property intensity As Double()

        Friend Shared Function Parse(row As String(), headers As Index(Of String), println As Action(Of String)) As SpotMs
            Call println($"read spot vector: {row(Scan0)}")

            Return New SpotMs With {
                .spot_id = row(Scan0),
                .intensity = row _
                    .Skip(1) _
                    .Select(AddressOf Conversion.Val) _
                    .ToArray
            }
        End Function

    End Class
End Namespace