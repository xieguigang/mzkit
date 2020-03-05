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

            Me.TPAFactors = TPAFactors
            Me.tolerance = tolerance
            Me.timeWindowSize = timeWindowSize
            Me.angleThreshold = angleThreshold
            Me.baselineQuantile = baselineQuantile
            Me.integratorTicks = integratorTicks
            Me.peakAreaMethod = peakAreaMethod
        End Sub

        Public Shared Function GetDefaultArguments() As MRMArguments
            Return New MRMArguments(
                TPAFactors:=Nothing,
                tolerance:=Tolerance.DeltaMass(0.3),
                timeWindowSize:=5,
                angleThreshold:=5,
                baselineQuantile:=0.65,
                integratorTicks:=5000,
                peakAreaMethod:=Methods.NetPeakSum
            )
        End Function
    End Class
End Namespace