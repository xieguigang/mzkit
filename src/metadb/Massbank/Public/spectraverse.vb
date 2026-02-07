Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class spectraverse

    Public Property TITLE As String
    Public Property CHARGE As Double
    Public Property SOURCE As String
    Public Property SMILES As String
    Public Property IONMODE As String
    Public Property ADDUCT As String
    Public Property COMPOUND_NAME As String
    Public Property NUM_PEAKS As Integer
    Public Property PRECURSOR_MZ As Double
    Public Property PARENT_MASS As Double
    Public Property MS_LEVEL As String
    Public Property INCHIKEY As String
    Public Property INCHI As String
    Public Property FORMULA As String
    Public Property INSTRUMENT_TYPE As String
    Public Property COLLISION_ENERGY_1 As Double
    Public Property COLLISION_ENERGY_2 As Double
    Public Property COLLISION_ENERGY_3 As Double
    Public Property NORMALIZED_COLLISION_ENERGY_1 As Double
    Public Property NORMALIZED_COLLISION_ENERGY_2 As Double
    Public Property NORMALIZED_COLLISION_ENERGY_3 As Double

    Public Function CreateMeta() As MetaInfo
        Dim uniqueId As String = $"{COMPOUND_NAME}-{FORMULA}".ToLower.MD5
        Dim metadata As New MetaInfo With {
            .ID = uniqueId,
            .formula = FORMULA,
            .exact_mass = FormulaScanner.EvaluateExactMass(FORMULA),
            .name = COMPOUND_NAME,
            .xref = New MetaLib.CrossReference.xref With {
                .InChIkey = INCHIKEY,
                .InChI = INCHI,
                .SMILES = SMILES,
                .extras = New Dictionary(Of String, String()) From {
                    {"spectraverse", {TITLE}}
                }
            },
            .description = SOURCE
        }

        Return metadata
    End Function

End Class
