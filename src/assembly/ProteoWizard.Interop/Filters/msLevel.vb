Namespace filters

    Public Class msLevel : Inherits Filter

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
End Namespace