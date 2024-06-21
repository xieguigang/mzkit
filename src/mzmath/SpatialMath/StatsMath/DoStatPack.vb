#Region "Microsoft.VisualBasic::dbf022850b1f45783a1aa7abb4c15eb6, visualize\MsImaging\Analysis\StatsMath\DoStatPack.vb"

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

    '   Total Lines: 162
    '    Code Lines: 123 (75.93%)
    ' Comment Lines: 16 (9.88%)
    '    - Xml Docs: 68.75%
    ' 
    '   Blank Lines: 23 (14.20%)
    '     File Size: 6.66 KB


    '     Module DoStatPack
    ' 
    '         Function: DoStatInternal, DoStatSingleIon
    '         Class IonFeatureTask
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: Solve
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Parallel
Imports Point = System.Drawing.Point

Namespace StatsMath

    ''' <summary>
    ''' create stats analysis result for ms-imaging
    ''' </summary>
    Public Module DoStatPack

        ''' <summary>
        ''' Run analysis for a single ion layer data
        ''' </summary>
        ''' <param name="ion">An ion layer data, consist with a collection of the spatial spot data</param>
        ''' <param name="nsize">
        ''' the grid cell size for evaluate the pixel density
        ''' </param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function DoStatSingleIon(ion As NamedCollection(Of PixelData), nsize As Integer, total_spots As Integer, parallel As Boolean) As IonStat
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

            Dim mzwidth_desc As String
            Dim mzmin As Double = mzlist.Min
            Dim mzmax As Double = mzlist.Max

            If PPMmethod.PPM(mzmin, mzmax) > 30 Then
                mzwidth_desc = $"da:{ (mzmax - mzmin).ToString("F3")}"
            Else
                mzwidth_desc = $"ppm:{PPMmethod.PPM(mzmin, mzmax).ToString("F1")}"
            End If

            intensity = SIMD.Divide.f64_op_divide_f64_scalar(intensity, intensity.Sum)

            Return New IonStat With {
                .mz = Val(ion.name),
                .basePixelX = basePixel.x,
                .basePixelY = basePixel.y,
                .maxIntensity = intensity.Max,
                .pixels = pixels.size,
                .Q1Intensity = Q.Q1,
                .Q2Intensity = Q.Q2,
                .Q3Intensity = Q.Q3,
                .density = counts.Average,
                .moran = If(ion.Length <= 3, -1, moran.Observed),
                .pvalue = If(ion.Length <= 3, 1, moran.pvalue),
                .mzmin = mzmin,
                .mzmax = mzmax,
                .mzwidth = mzwidth_desc,
                .averageIntensity = intensity.Average,
                .sparsity = 1 - intensity.Length / total_spots,
                .entropy = intensity.ShannonEntropy,
                .rsd = intensity.RSD
            }
        End Function

        <Extension>
        Public Function DoStatInternal(allIons As IEnumerable(Of PixelData()),
                                       nsize As Integer,
                                       da As Double,
                                       total_spots As Integer,
                                       parallel As Boolean) As IEnumerable(Of IonStat)

            ' convert the spatial spot pack as multiple imaging layers
            ' based on the ion feature tag data
            Dim ions = allIons _
                .GroupByTree(Function(d) d.mz, Tolerance.DeltaMass(da)) _
                .ToArray
            Dim par As New IonFeatureTask(ions, nsize, total_spots)

            Call VBDebugger.EchoLine($"get {ions.Length} ion features from the raw data pack!")

            If parallel Then
                Call par.Run()
            Else
                Call par.Solve()
            End If

            Return par.result
        End Function

        Private Class IonFeatureTask : Inherits VectorTask

            Public result As IonStat()

            Dim layers As NamedCollection(Of PixelData)()
            Dim nsize As Integer
            Dim total_spots As Integer

            Sub New(layers As NamedCollection(Of PixelData)(), nsize As Integer, total_spots As Integer)
                Call MyBase.New(layers.Length)

                Me.layers = layers
                Me.result = New IonStat(layers.Length - 1) {}
                Me.nsize = nsize
            End Sub

            Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
                For i As Integer = start To ends
                    ' moran parallel if in sequenceMode
                    ' moran sequence if not in sequenceMode
                    result(i) = DoStatSingleIon(layers(i), nsize, total_spots, parallel:=sequenceMode)
                Next
            End Sub
        End Class
    End Module
End Namespace
