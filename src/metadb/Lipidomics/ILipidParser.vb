Namespace CompMs.Common.Lipidomics
    Public Interface ILipidParser
        ReadOnly Property Target As String
        Function Parse(ByVal lipidStr As String) As ILipid
    End Interface
End Namespace
