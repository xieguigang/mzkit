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

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ApplicationServices.Plugin
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Parallel
Imports Point = System.Drawing.Point
Imports stdNum = System.Math

''' <summary>
''' Stats the ion features inside a MSI raw data slide
''' </summary>
Public Class IonStat

    <Category("MSdata")> <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F4")> Public Property mz As Double
    <Category("MSdata")> <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F4")> Public Property mzmin As Double
    <Category("MSdata")> <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F4")> Public Property mzmax As Double

    <Category("MSdata")> <DisplayName("mz.diff")>
    Public Property mzwidth As String

    <Category("Spatial")> Public Property pixels As Integer
    <Category("Spatial")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F2")>
    Public Property density As Double

    <Category("MSdata")> <DisplayName("max.into")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property maxIntensity As Double

    <DisplayName("basepeak.x")>
    <Category("Spatial")> Public Property basePixelX As Integer
    <DisplayName("basepeak.y")>
    <Category("Spatial")> Public Property basePixelY As Integer

    <Category("MSdata")> <DisplayName("intensity(Q1)")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property Q1Intensity As Double
    <Category("MSdata")> <DisplayName("intensity(Q2)")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property Q2Intensity As Double
    <Category("MSdata")> <DisplayName("intensity(Q3)")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property Q3Intensity As Double

    <Category("Spatial")> <DisplayName("moran I")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F3")>
    Public Property moran As Double

    <Category("Spatial")> <DisplayName("moran p-value")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G4")>
    Public Property pvalue As Double

    Public Shared Function DoStat(allPixels As PixelScan(),
                                  Optional nsize As Integer = 5,
                                  Optional da As Double = 0.05,
                                  Optional mz As Double() = Nothing,
                                  Optional parallel As Boolean = False) As IEnumerable(Of IonStat)
        Dim ionList = allPixels _
            .Select(Function(i)
                        Dim pt As New Point(i.X, i.Y)
                        Dim ions = i.GetMsPipe.Select(Function(ms) New PixelData(pt, ms))
                        Return ions
                    End Function) _
            .IteratesALL _
            .ToArray

        If Not mz.IsNullOrEmpty Then
            Dim allHits = ionList _
                .AsParallel _
                .Where(Function(i)
                           Return mz.Any(Function(mzi) stdNum.Abs(mzi - i.mz) <= da)
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
            .Select(Iterator Function(scan) As IEnumerable(Of PixelData)
                        Dim xy As Point = scan.GetMSIPixel

                        For Each ms1 As ms2 In scan.GetMs
                            Yield New PixelData(xy, ms1)
                        Next
                    End Function) _
            .IteratesALL _
            .DoCall(Function(allIons)
                        Return DoStatInternal(allIons, nsize, da, parallel)
                    End Function)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="nsize">
    ''' the grid cell size for evaluate the pixel density
    ''' </param>
    ''' <returns></returns>
    Public Shared Function DoStat(layer As SingleIonLayer, Optional nsize As Integer = 5) As IonStat
        Dim ion As New NamedCollection(Of PixelData)(layer.IonMz, layer.MSILayer)
        Dim stats As IonStat = DoStatSingleIon(ion, nsize, parallel:=True)

        Return stats
    End Function

    ''' <summary>
    ''' Run analysis for a single ion layer data
    ''' </summary>
    ''' <param name="ion">An ion layer data, consist with a collection of the spatial spot data</param>
    ''' <param name="nsize">
    ''' the grid cell size for evaluate the pixel density
    ''' </param>
    ''' <returns></returns>
    Private Shared Function DoStatSingleIon(ion As NamedCollection(Of PixelData), nsize As Integer, parallel As Boolean) As IonStat
        Dim pixels = Grid(Of PixelData).Create(ion, Function(x) New Point(x.x, x.y))
        Dim basePixel = ion.OrderByDescending(Function(i) i.intensity).First
        Dim intensity As Double() = ion _
            .Select(Function(i) i.intensity) _
            .ToArray
        Dim sampling = ion.Where(Function(p) p.x Mod 5 = 0 AndAlso p.y Mod 5 = 0).ToArray
        Dim moran As MoranTest

        If sampling.Length < 3 Then
            moran = New MoranTest With {
                .df = 0, .Expected = 0, .Observed = 0,
                .prob2 = 1, .pvalue = 1, .SD = 0,
                .t = 0, .z = 0
            }
        Else
            moran = MoranTest.moran_test(
                x:=sampling.Select(Function(i) i.intensity).ToArray,
                c1:=sampling.Select(Function(p) CDbl(p.x)).ToArray,
                c2:=sampling.Select(Function(p) CDbl(p.y)).ToArray,
                parallel:=parallel,
                throwMaxIterError:=False
            )
        End If

        Dim Q As DataQuartile = intensity.Quartile
        Dim counts As New List(Of Double)
        Dim A As Double = nsize ^ 2
        Dim mzlist As Double() = ion.Select(Function(p) p.mz).ToArray

        For Each top As PixelData In From i As PixelData
                                     In ion
                                     Order By i.intensity Descending
                                     Take 30

            Dim count As Integer = pixels _
                .Query(top.x, top.y, nsize) _
                .Where(Function(i)
                           Return Not i Is Nothing AndAlso i.intensity > 0
                       End Function) _
                .Count
            Dim density As Double = count / A

            Call counts.Add(density)
        Next

        Return New IonStat With {
            .mz = Val(ion.name),
            .basePixelX = basePixel.x,
            .basePixelY = basePixel.y,
            .maxIntensity = intensity.Average,
            .pixels = pixels.size,
            .Q1Intensity = Q.Q1,
            .Q2Intensity = Q.Q2,
            .Q3Intensity = Q.Q3,
            .density = counts.Average,
            .moran = If(ion.Length <= 3, -1, moran.Observed),
            .pvalue = If(ion.Length <= 3, 1, moran.pvalue),
            .mzmin = mzlist.Min,
            .mzmax = mzlist.Max,
            .mzwidth = If(PPMmethod.PPM(.mzmin, .mzmax) > 30, $"da:{ (.mzmax - .mzmin).ToString("F3")}", $"ppm:{PPMmethod.PPM(.mzmin, .mzmax).ToString("F1")}")
        }
    End Function

    Private Shared Function DoStatInternal(allIons As IEnumerable(Of PixelData),
                                           nsize As Integer,
                                           da As Double,
                                           parallel As Boolean) As IEnumerable(Of IonStat)

        ' convert the spatial spot pack as multiple imaging layers
        ' based on the ion feature tag data
        Dim ions = allIons _
            .ToArray _
            .GroupBy(Function(d) d.mz, Tolerance.DeltaMass(da)) _
            .ToArray
        Dim par As New StatTask(ions, nsize)

        If parallel Then
            Call par.Run()
        Else
            Call par.Solve()
        End If

        Return par.result
    End Function

    Private Class StatTask : Inherits VectorTask

        Public result As IonStat()

        Dim layers As NamedCollection(Of PixelData)()
        Dim nsize As Integer

        Sub New(layers As NamedCollection(Of PixelData)(), nsize As Integer)
            Call MyBase.New(layers.Length)

            Me.layers = layers
            Me.result = New IonStat(layers.Length - 1) {}
            Me.nsize = nsize
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer)
            For i As Integer = start To ends
                ' moran parallel if in sequenceMode
                ' moran sequence if not in sequenceMode
                result(i) = DoStatSingleIon(layers(i), nsize, parallel:=sequenceMode)
            Next
        End Sub
    End Class
End Class
