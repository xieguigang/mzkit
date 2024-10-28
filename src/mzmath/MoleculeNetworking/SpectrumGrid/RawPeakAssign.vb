Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class RawPeakAssign : Implements IReadOnlyId

    Public Property peak As xcms2
    Public Property ms2 As PeakMs2()
    Public Property cor As Double
    Public Property score As Double
    Public Property pval As Double

    Public Property v1 As Double()
    Public Property v2 As Double()

    Public ReadOnly Property Id As String Implements IReadOnlyId.Identity
        Get
            Return peak.ID
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return peak.ToString & $" correlated with {ms2.Length} spectrum, pearson={cor}"
    End Function

End Class