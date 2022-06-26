Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace SCiLSLab

    Public Class SpotPack : Inherits PackFile

        Public Property index As Dictionary(Of String, SpotSite)

        Public Shared Function ParseFile(file As Stream) As SpotPack
            Dim pull As New SpotPack
            Dim spots As SpotSite() = ParseTable(file, pull, AddressOf SpotSite.Parse).ToArray

            pull.index = spots.ToDictionary(Function(sp) sp.index)

            Return pull
        End Function
    End Class

    Public Class SpotSite

        Public Property index As String
        Public Property x As Double
        Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"spot_{index} [{x},{y}]"
        End Function

        Friend Shared Function Parse(row As String(), headers As Index(Of String)) As SpotSite
            ' Spot index;x;y
            Dim index As String = row(headers("Spot index"))
            Dim x As Double = Val(row(headers("x")))
            Dim y As Double = Val(row(headers("y")))

            Return New SpotSite With {
                .index = index,
                .x = x,
                .y = y
            }
        End Function

    End Class
End Namespace