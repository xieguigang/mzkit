Namespace filters

    Public MustInherit Class filter

        Protected MustOverride Function getFilterName() As String
        Protected MustOverride Function getFilterArgs() As String

        Public Overrides Function ToString() As String
            Return $"{getFilterName()} {getFilterArgs()}"
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

        Protected Overrides Function getFilterName() As String
            Return NameOf(scanTime)
        End Function

        Protected Overrides Function getFilterArgs() As String
            Return timeRange
        End Function
    End Class
End Namespace