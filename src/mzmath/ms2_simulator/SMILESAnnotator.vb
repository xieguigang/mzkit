Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES.Embedding
Imports BioNovoGene.BioDeep.MSFinder

Public Class SMILESAnnotator

    ReadOnly formula As Formula
    ReadOnly da As Tolerance = Tolerance.DeltaMass(0.5)

    Sub New(smiles As ChemicalFormula, formula As Formula)
        Me.formula = formula
    End Sub

    Public Sub Annotation(ByRef spec As ISpectrum, ionMode As IonModes)
        For Each frag As ms2 In spec.GetIons
            frag.Annotation = Annotation(frag.mz, ionMode)
        Next
    End Sub

    Public Function Annotation(mz As Double, ionMode As IonModes) As String
        Dim candiates = FragmentAssigner.getValenceCheckedFragmentFormulaList(formula, ionMode, mz, da.DeltaTolerance)
        Dim topRank As Formula = Nothing

        For Each candiate As Formula In candiates
            If candiate.ExactMass > formula.ExactMass Then
                ' [M+xxx]
                ' precursor ion?
                Dim adducts As Formula = candiate - formula

                Return $"[M+{adducts.CanonicalFormula}]{If(ionMode = IonModes.Positive, "+", "-")}"
            ElseIf candiate = formula Then
                Return $"[M]{If(ionMode = IonModes.Positive, "+", "-")}"
            Else
                Dim loss As Formula = formula - candiate

                Throw New NotImplementedException
            End If
        Next

        If topRank Is Nothing Then
            Return Nothing
        End If

        Return topRank.EmpiricalFormula
    End Function

End Class
