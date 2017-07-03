Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class metabolite

    Public Property version As String
    Public Property creation_date As String
    Public Property update_date As String
    Public Property accession As String
    Public Property secondary_accessions As secondary_accessions
    Public Property name As String
    Public Property description As String
    Public Property synonyms As synonyms
    Public Property chemical_formula As String
    Public Property average_molecular_weight As Double
    Public Property monisotopic_molecular_weight As Double
    Public Property iupac_name As String
    Public Property traditional_iupac As String
    Public Property cas_registry_number As String
    Public Property smiles As String
    Public Property inchi As String
    Public Property inchikey As String
    Public Property taxonomy As taxonomy
End Class

Public Structure secondary_accessions
    <XmlElement> Public Property accession As String()

    Public Overrides Function ToString() As String
        Return Me.accession.GetJson
    End Function
End Structure

Public Structure synonyms
    <XmlElement> Public Property synonym As String()

    Public Overrides Function ToString() As String
        Return Me.synonym.GetJson
    End Function
End Structure