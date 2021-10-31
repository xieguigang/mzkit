Public Class Compound
    Public Property id As String
    Public Property public_id As String
    Public Property name As String
    Public Property state As String
    Public Property annotation_quality As String
    Public Property description As String
    Public Property cas_number As String
    Public Property moldb_smiles As String
    Public Property moldb_inchi As String
    Public Property moldb_mono_mass As String
    Public Property moldb_inchikey As String
    Public Property moldb_iupac As String
    Public Property kingdom As String
    Public Property superklass As String
    Public Property klass As String
    Public Property subklass As String

    Public Overrides Function ToString() As String
        Return $"[{public_id}] {name}"
    End Function
End Class
