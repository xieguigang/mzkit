Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace Query

    Public MustInherit Class Ms2Search

        Protected ReadOnly da As Tolerance
        Protected ReadOnly intocutoff As RelativeIntensityCutoff

        Sub New(Optional da As Double = 0.3, Optional intocutoff As Double = 0.05)
            Me.da = Tolerance.DeltaMass(da)
            Me.intocutoff = intocutoff
        End Sub

        Public Function Centroid(matrix As ms2()) As ms2()
            Return matrix.Centroid(da, intocutoff).ToArray
        End Function

        Public MustOverride Function Search(centroid As ms2(), mz1 As Double) As ClusterHit

    End Class
End Namespace