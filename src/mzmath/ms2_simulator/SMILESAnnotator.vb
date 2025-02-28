Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Public Class SMILESAnnotator

    ReadOnly smiles As ChemicalFormula
    ReadOnly formula As Formula

    Sub New(smiles As ChemicalFormula, formula As Formula)
        Me.smiles = smiles
        Me.formula = formula
    End Sub

    Public Sub Annotation(ByRef spec As ISpectrum)
        For Each frag As ms2 In spec.GetIons
            frag.Annotation = Annotation(frag.mz)
        Next
    End Sub

    Public Function Annotation(mz As Double) As String

    End Function

End Class
