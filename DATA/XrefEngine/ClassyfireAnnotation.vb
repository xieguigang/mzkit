
''' <summary>
''' Parser for file download from https://jcheminf.biomedcentral.com/articles/10.1186/s13321-016-0174-y
''' </summary>
Public Class ClassyfireAnnotation

    ''' <summary>
    ''' Compound id in given database.
    ''' </summary>
    ''' <returns></returns>
    Public Property CompoundID As String
    Public Property Smiles As String
    ''' <summary>
    ''' 分类的term id
    ''' </summary>
    ''' <returns></returns>
    Public Property ChemOntID As String
    Public Property ParentName As String

    Public Overrides Function ToString() As String
        Return $"Dim {CompoundID} As {ChemOntID} = ""{ParentName}"""
    End Function
End Class
