Public Class ClassyfireAnnotation

    Public Property CompoundID As String
    Public Property Smiles As String
    Public Property ChemOntID As String
    Public Property ParentName As String

    Public Overrides Function ToString() As String
        Return $"Dim {CompoundID} As {ChemOntID} = ""{ParentName}"""
    End Function
End Class
