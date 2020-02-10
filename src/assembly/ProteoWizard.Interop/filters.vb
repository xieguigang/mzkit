Namespace filters

    Public MustInherit Class filter

        Protected MustOverride Function getFilterName() As String
        Protected MustOverride Function getFilterArgs() As String

        Public Overrides Function ToString() As String
            Return $"--filter ""{getFilterName()} {getFilterArgs()}"""
        End Function

        Public Shared Function GetFilters(filters As IEnumerable(Of filter)) As String
            ' multiple filters: Select scan numbers And recalculate precursors
            ' msconvert data.RAW --filter "scanNumber [500,1000]" --filter "precursorRecalculation"
            Return filters.JoinBy(" ")
        End Function

    End Class

    Public Class msLevel : Inherits filter

        Public ReadOnly Property levels As String

        Sub New(levels As String)
            Me.levels = levels
        End Sub

        Protected Overrides Function getFilterName() As String
            Return NameOf(msLevel)
        End Function

        Protected Overrides Function getFilterArgs() As String
            Return levels
        End Function
    End Class

    Public Class scanTime : Inherits filter

        Public ReadOnly Property timeRange As String

        Sub New(timeRange As String)
            Me.timeRange = timeRange
        End Sub

        Sub New(start#, stop#)
            Me.timeRange = $"[{start}, {[stop]}]"
        End Sub

        Protected Overrides Function getFilterName() As String
            Return NameOf(scanTime)
        End Function

        Protected Overrides Function getFilterArgs() As String
            Return timeRange
        End Function
    End Class
End Namespace