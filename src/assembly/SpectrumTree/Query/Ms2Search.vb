Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

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