
Public Interface ILipidParser
        ReadOnly Property Target As String
        Function Parse(ByVal lipidStr As String) As ILipid
    End Interface

