Public Class Information

    Public Property name As String()
    Public Property formula As String
    Public Property mw As Double
    Public Property CAS As String()
    Public Property CID As String
    Public Property InChIKey As String
    Public Property InChICode As String
    Public Property SMILES As String
    Public Property Biosynthesis As String
    Public Property Organism As Organism()
    ''' <summary>
    ''' data uri
    ''' </summary>
    ''' <returns></returns>
    Public Property img As String

End Class

Public Class Organism
    Public Property Kingdom As String
    Public Property Family As String
    Public Property Species As String
End Class