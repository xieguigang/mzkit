Public Class LipidName

    Public Property className As String
    Public Property chains As Chain()

    Public Overrides Function ToString() As String
        Return ToSystematicName()
    End Function

    Public Function ToSystematicName() As String

    End Function

    Public Function ToOverviewName() As String

    End Function

    Public Shared Function ParseName(name As String) As LipidName

    End Function

End Class
