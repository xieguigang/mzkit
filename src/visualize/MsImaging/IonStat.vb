#Region "Microsoft.VisualBasic::9aa0f529fa26bbb17565c9f15f78d452, mzkit\src\visualize\MsImaging\IonStat.vb"

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

'   Total Lines: 85
'    Code Lines: 75
' Comment Lines: 0
'   Blank Lines: 10
'     File Size: 3.51 KB


' Class IonStat
' 
'     Properties: basePixelX, basePixelY, density, maxIntensity, mz
'                 pixels, Q1Intensity, Q2Intensity, Q3Intensity
' 
'     Function: (+3 Overloads) DoStat
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
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
    Public Property basePixelX As Integer
    Public Property basePixelY As Integer
    Public Property Q1Intensity As Double
    Public Property Q2Intensity As Double
    Public Property Q3Intensity As Double
    Public Property RSD As Double

    Public Shared Function DoStat(allPixels As PixelScan(), Optional nsize As Integer = 5) As IEnumerable(Of IonStat)
        Return allPixels _
            .Select(Function(i)
                        Dim pt As New Point(i.X, i.Y)
                        Dim ions = i.GetMsPipe.Select(Function(ms) (pt, ms))

                        Return ions
                    End Function) _
            .IteratesALL _
            .DoCall(Function(allIons)
                        Return DoStat(allIons, nsize)
                    End Function)
    End Function

    Public Shared Function DoStat(raw As mzPack, Optional nsize As Integer = 5) As IEnumerable(Of IonStat)
        Return raw.MS _
            .Select(Function(scan)
                        Return scan.GetMs.Select(Function(ms1) (scan.GetMSIPixel, ms1))
                    End Function) _
            .IteratesALL _
            .DoCall(Function(allIons)
                        Return DoStat(allIons, nsize)
                    End Function)
    End Function

    Private Shared Iterator Function DoStat(allIons As IEnumerable(Of (pixel As Point, ms As ms2)), nsize As Integer) As IEnumerable(Of IonStat)
        Dim ions = allIons _
            .GroupBy(Function(d) d.ms.mz, Tolerance.DeltaMass(0.05)) _
            .ToArray
        Dim A As Double = nsize ^ 2

        For Each ion In ions
            Dim pixels = Grid(Of (Point, ms2)).Create(ion, Function(x) x.Item1)
            Dim basePixel = ion.OrderByDescending(Function(i) i.ms.intensity).First
			Dim intensity As Double() = ion.Select(Function(i) i.ms.intensity).ToArray
            Dim Q = intensity.Quartile
            Dim counts As New List(Of Double)

            For Each top In ion.OrderByDescending(Function(i) i.ms.intensity).Take(30)
                Dim count As Integer = pixels _
                    .Query(top.pixel.X, top.pixel.Y, nsize) _
                    .Where(Function(i)
                               Return Not i.Item2 Is Nothing AndAlso i.Item2.intensity > 0
                           End Function) _
                    .Count
                Dim density As Double = count / A

                counts.Add(density)
            Next

            Yield New IonStat With {
                .mz = Val(ion.name),
                .basePixelX = basePixel.pixel.X,
                .basePixelY = basePixel.pixel.Y,
                .maxIntensity = basePixel.ms.intensity,
                .pixels = pixels.size,
                .Q1Intensity = Q.Q1,
                .Q2Intensity = Q.Q2,
                .Q3Intensity = Q.Q3,
                .density = counts.Average,
                .RSD = intensity.RSD * 100
            }
        Next
    End Function
End Class
