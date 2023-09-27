#Region "Microsoft.VisualBasic::9ef6f690224e209c6ce5ebaeb7e665bb, mzkit\src\visualize\MsImaging\Analysis\IonStat.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 149
'    Code Lines: 124
' Comment Lines: 10
'   Blank Lines: 15
'     File Size: 6.05 KB


' Class IonStat
' 
'     Properties: basePixelX, basePixelY, density, maxIntensity, mz
'                 pixels, Q1Intensity, Q2Intensity, Q3Intensity, RSD
' 
'     Function: (+2 Overloads) DoStat, DoStatInternal, DoStatSingleIon
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Point = System.Drawing.Point
Imports stdNum = System.Math

Public Class IonStat

    Public Property mz As Double
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property mzwidth As String
    Public Property pixels As Integer
    Public Property density As Double
    Public Property maxIntensity As Double
    Public Property basePixelX As Integer
    Public Property basePixelY As Integer
    Public Property Q1Intensity As Double
    Public Property Q2Intensity As Double
    Public Property Q3Intensity As Double
    Public Property moran As Double
    Public Property pvalue As Double

    Public Shared Function DoStat(allPixels As PixelScan(),
                                  Optional nsize As Integer = 5,
                                  Optional da As Double = 0.05,
                                  Optional mz As Double() = Nothing,
                                  Optional parallel As Boolean = False) As IEnumerable(Of IonStat)
        Dim ionList = allPixels _
            .Select(Function(i)
                        Dim pt As New Point(i.X, i.Y)
                        Dim ions = i.GetMsPipe.Select(Function(ms) (pt, ms))

                        Return ions
                    End Function) _
            .IteratesALL _
            .ToArray

        If Not mz.IsNullOrEmpty Then
            Dim allHits = ionList _
                .AsParallel _
                .Where(Function(i)
                           Return mz.Any(Function(mzi) stdNum.Abs(mzi - i.ms.mz) <= da)
                       End Function) _
                .DoCall(Function(allIons) DoStatInternal(allIons, nsize, da, parallel)) _
                .ToList

            For Each mzi As Double In mz
                If allHits.Count = 0 OrElse Not allHits.All(Function(m) stdNum.Abs(m.mz - mzi) <= da) Then
                    ' missing current ion
                    ' fill empty
                    allHits.Add(New IonStat With {.mz = mzi})
                End If
            Next

            Return allHits
        Else
            Return DoStatInternal(ionList, nsize, da, parallel)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="nsize">
    ''' the grid size for evaluate the density value
    ''' </param>
    ''' <returns></returns>
    Public Shared Function DoStat(raw As mzPack,
                                  Optional nsize As Integer = 5,
                                  Optional da As Double = 0.05,
                                  Optional parallel As Boolean = False) As IEnumerable(Of IonStat)
        Return raw.MS _
            .Select(Function(scan)
                        Return scan.GetMs.Select(Function(ms1) (scan.GetMSIPixel, ms1))
                    End Function) _
            .IteratesALL _
            .DoCall(Function(allIons)
                        Return DoStatInternal(allIons, nsize, da, parallel)
                    End Function)
    End Function

    Private Shared Function DoStatSingleIon(ion As NamedCollection(Of (pixel As Point, ms As ms2)), nsize As Integer) As IonStat
        Dim pixels = Grid(Of (Point, ms2)).Create(ion, Function(x) x.Item1)
        Dim basePixel = ion.OrderByDescending(Function(i) i.ms.intensity).First
        Dim intensity As Double() = ion _
            .Select(Function(i) i.ms.intensity) _
            .ToArray
        Dim moran As MoranTest = MoranTest.moran_test(intensity, ion.Select(Function(p) CDbl(p.pixel.X)).ToArray, ion.Select(Function(p) CDbl(p.pixel.Y)).ToArray)
        Dim Q As DataQuartile = intensity.Quartile
        Dim counts As New List(Of Double)
        Dim A As Double = nsize ^ 2
        Dim mzlist As Double() = ion.Select(Function(p) p.ms.mz).ToArray

        For Each top As (pixel As Point, ms As ms2) In From i As (pixel As Point, ms As ms2)
                                                       In ion
                                                       Order By i.ms.intensity Descending
                                                       Take 30
            Dim count As Integer = pixels _
                .Query(top.pixel.X, top.pixel.Y, nsize) _
                .Where(Function(i)
                           Return Not i.Item2 Is Nothing AndAlso i.Item2.intensity > 0
                       End Function) _
                .Count
            Dim density As Double = count / A

            Call counts.Add(density)
        Next

        Return New IonStat With {
            .mz = Val(ion.name),
            .basePixelX = basePixel.pixel.X,
            .basePixelY = basePixel.pixel.Y,
            .maxIntensity = intensity.Average,
            .pixels = pixels.size,
            .Q1Intensity = Q.Q1,
            .Q2Intensity = Q.Q2,
            .Q3Intensity = Q.Q3,
            .density = counts.Average,
            .moran = moran.Observed,
            .pvalue = moran.pvalue,
            .mzmin = mzlist.Min,
            .mzmax = mzlist.Max,
            .mzwidth = If(PPMmethod.PPM(.mzmin, .mzmax) > 30, $"da:{ .mzmax - .mzmin}", $"ppm:{PPMmethod.PPM(.mzmin, .mzmax)}")
        }
    End Function

    Private Shared Iterator Function DoStatInternal(allIons As IEnumerable(Of (pixel As Point, ms As ms2)),
                                                    nsize As Integer,
                                                    da As Double,
                                                    parallel As Boolean) As IEnumerable(Of IonStat)
        Dim ions = allIons _
            .ToArray _
            .GroupBy(Function(d) d.ms.mz, Tolerance.DeltaMass(da)) _
            .ToArray

        If parallel Then
            For Each stat In ions _
                .AsParallel _
                .Select(Function(ion)
                            Return DoStatSingleIon(ion, nsize)
                        End Function)

                Yield stat
            Next
        Else
            For Each ion As NamedCollection(Of (pixel As Point, ms As ms2)) In ions
                Yield DoStatSingleIon(ion, nsize)
            Next
        End If
    End Function
End Class
