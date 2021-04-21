Namespace filters

    Public Class scanTime : Inherits Filter

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