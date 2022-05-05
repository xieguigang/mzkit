Public Class Chain

    Public Property carbons As Integer
    Public Property doubleBonds As Integer
    Public Property position As BondPosition()
    Public Property groups As Group()

End Class

Public Class Group : Inherits BondPosition

    Public Property groupName As String

End Class

Public Class BondPosition

    Public Property index As Integer
    ''' <summary>
    ''' E/Z
    ''' </summary>
    ''' <returns></returns>
    Public Property [structure] As Char

End Class