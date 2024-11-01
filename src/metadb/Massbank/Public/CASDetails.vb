Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Metabolite = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

''' <summary>
''' The metabolite data model from the cas registry details
''' </summary>
Public Class CASDetails

    Public Property uri As String
    Public Property rn As String
    Public Property name As String
    Public Property images As String()
    Public Property inchi As String
    Public Property inchikey As String
    Public Property smile As String
    Public Property canonicalSmile As String
    Public Property molecularFormula As String
    Public Property molecularMass As String
    Public Property experimentalProperties As experimentalProperty()
    Public Property propertyCitations As propertyCitation()
    Public Property synonyms As String()
    Public Property replacedRns As String()
    Public Property hasMolfile As Boolean

    Public Overrides Function ToString() As String
        Return $"{name} ({molecularFormula.StripHTMLTags})"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function GetDetails(cas_id As String) As CASDetails
        Return $"https://commonchemistry.cas.org/api/detail?cas_rn={cas_id}".GET.LoadJSON(Of CASDetails)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetModel() As Metabolite
        Return New Metabolite With {
            .ID = rn,
            .formula = molecularFormula.StripHTMLTags,
            .exact_mass = FormulaScanner.EvaluateExactMass(.formula),
            .name = name,
            .IUPACName = name,
            .synonym = synonyms,
            .xref = New xref With {
                .CAS = {rn},
                .InChI = inchi,
                .InChIkey = inchikey,
                .SMILES = smile
            }
        }
    End Function

End Class

Public Class propertyCitation

    Public Property docUri As String
    Public Property sourceNumber As Integer
    Public Property source As String

End Class

Public Class experimentalProperty

    Public Property name As String
    Public Property [property] As String
    Public Property sourceNumber As Integer

End Class