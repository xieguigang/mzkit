Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports Point = System.Drawing.Point

Public Class IonStat

    Public Property mz As Double
    Public Property pixels As Integer
    Public Property density As Double
    Public Property maxIntensity As Double
    Public Property basePixel As Point
    Public Property Q1Intensity As Double
    Public Property Q2Intensity As Double
    Public Property Q3Intensity As Double

    Public Shared Iterator Function DoStat(raw As mzPack) As IEnumerable(Of IonStat)
        Dim allIons = raw.MS _
            .Select(Function(scan)
                        Return scan.GetMs.Select(Function(ms1) (scan.GetMSIPixel, ms1))
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim ions = allIons _
            .GroupBy(Function(d) d.ms1.mz, Tolerance.DeltaMass(0.05)) _
            .ToArray
        Dim A As Double = 5 ^ 2

        For Each ion In ions
            Dim pixels = Grid(Of (Point, ms2)).Create(ion, Function(x) x.Item1)
            Dim basePixel = ion.OrderByDescending(Function(i) i.ms1.intensity).First
            Dim Q = ion.Select(Function(i) i.ms1.intensity).Quartile
            Dim counts As New List(Of Double)

            For Each top In ion.OrderByDescending(Function(i) i.ms1.intensity).Take(30)
                Dim count As Integer = pixels _
                    .Query(top.GetMSIPixel.X, top.GetMSIPixel.Y, 5) _
                    .Where(Function(i)
                               Return Not i.Item2 Is Nothing AndAlso i.Item2.intensity > 0
                           End Function) _
                    .Count
                Dim density As Double = count / A

                counts.Add(density)
            Next

            Yield New IonStat With {
                .mz = Val(ion.name),
                .basePixel = basePixel.GetMSIPixel,
                .maxIntensity = basePixel.ms1.intensity,
                .pixels = pixels.size,
                .Q1Intensity = Q.Q1,
                .Q2Intensity = Q.Q2,
                .Q3Intensity = Q.Q3,
                .density = counts.Average
            }
        Next
    End Function
End Class
