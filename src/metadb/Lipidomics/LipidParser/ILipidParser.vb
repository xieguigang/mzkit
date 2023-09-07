
Public Interface ILipidParser
        ReadOnly Property Target As String
        Function Parse(lipidStr As String) As ILipid
    End Interface

