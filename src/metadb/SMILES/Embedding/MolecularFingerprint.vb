Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.MorganFingerprint

Public Class MolecularFingerprint

    Public Property Length As Integer
        Get
            Return MorganFingerprint.FingerprintLength
        End Get
        Set(value As Integer)
            Call MorganFingerprint.SetLength(value)
        End Set
    End Property

    Public Shared Function ConvertToMorganFingerprint(smiles As String, Optional radius As Integer = 3, Optional strict As Boolean = False) As Byte()
        Dim g As ChemicalFormula = ChemicalFormula.ParseGraph(smiles, strict:=False)
        Dim struct As [Structure] = g.CreateStructureGraph
        Dim checksum As Byte() = struct.CalculateFingerprintCheckSum(radius)

        Return checksum
    End Function
End Class