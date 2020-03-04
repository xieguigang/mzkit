Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Namespace MRM

    Public Class MRMArguments

        Public ReadOnly Property TPAFactors As Dictionary(Of String, Double)
        Public ReadOnly Property tolerance As Tolerance
        Public ReadOnly Property timeWindowSize#
        Public ReadOnly Property angleThreshold#
        Public ReadOnly Property baselineQuantile# = 0.65
        Public ReadOnly Property integratorTicks% = 5000
        Public ReadOnly Property peakAreaMethod As PeakArea.Methods = Methods.Integrator

        Sub New(TPAFactors As Dictionary(Of String, Double),
                tolerance As Tolerance,
                timeWindowSize#,
                angleThreshold#,
                baselineQuantile#,
                integratorTicks%,
                peakAreaMethod As PeakArea.Methods)

        End Sub
    End Class
End Namespace