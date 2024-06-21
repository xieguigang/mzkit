#Region "Microsoft.VisualBasic::700eb052c44b0f944465ff25c03e12e1, visualize\MsImaging\Analysis\StatsMath\DoStatMatrix.vb"

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

'   Total Lines: 150
'    Code Lines: 122 (81.33%)
' Comment Lines: 3 (2.00%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 25 (16.67%)
'     File Size: 6.41 KB


'     Module DoStatMatrix
' 
'         Function: DoStat
'         Class IonFeatureTask
' 
'             Constructor: (+1 Overloads) Sub New
' 
'             Function: Result
' 
'             Sub: Solve
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Point = System.Drawing.Point

Namespace StatsMath

    Module DoStatMatrix

        <Extension>
        Public Function DoStat(rawdata As MzMatrix, Optional grid_size As Integer = 5, Optional parallel As Boolean = True) As IEnumerable(Of IonStat)
            Dim par As New IonFeatureTask(rawdata, grid_size, parallel)

            If parallel Then
                Call par.Run()
            Else
                Call par.Solve()
            End If

            Return par.Result
        End Function

        ''' <summary>
        ''' analysis of the ion features inside a ms-imaging data matrix
        ''' </summary>
        Private Class IonFeatureTask : Inherits MatrixMath.DoStatMatrix

            ReadOnly grid_size As Integer
            ReadOnly A As Double

            ReadOnly density As Double()
            ReadOnly average As Double()
            ReadOnly moran As Double()
            ReadOnly pvalue As Double()

            ReadOnly x, y As Integer()
            ReadOnly parallel As Boolean

            ReadOnly mz As Double()
            ReadOnly mzmin As Double()
            ReadOnly mzmax As Double()

            Public Sub New(m As MzMatrix, grid_size As Integer, parallel As Boolean)
                MyBase.New(m)

                Me.A = grid_size ^ 2
                Me.grid_size = grid_size
                Me.parallel = parallel
                Me.mz = m.mz
                Me.mzmin = m.mzmin
                Me.mzmax = m.mzmax

                Me.density = Allocate(Of Double)(all:=True)
                Me.average = Allocate(Of Double)(all:=True)
                Me.moran = Allocate(Of Double)(all:=True)
                Me.pvalue = Allocate(Of Double)(all:=True)

                Me.x = Allocate(Of Integer)(all:=True)
                Me.y = Allocate(Of Integer)(all:=True)
            End Sub

            Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
                Call MyBase.Solve(start, ends, cpu_id)

                For i As Integer = start To ends
                    Dim offset As Integer = i
                    Dim intensity_vec As Double() = (From cell As Deconvolute.PixelData In matrix Select cell(offset)).ToArray
                    Dim mean As Double = intensity_vec.Average
                    Dim max_spot As Deconvolute.PixelData = matrix(MyBase.max_spot(offset))

                    x(offset) = max_spot.X
                    y(offset) = max_spot.Y
                    average(offset) = mean

                    Dim points = (From cell As Deconvolute.PixelData In matrix Where cell(offset) > 0 Select cell).ToArray
                    Dim pixels = points.Select(Function(a) New Point(a.X, a.Y)).CreateReadOnlyGrid
                    Dim sampling = points.Where(Function(p) p.X Mod 5 = 0 AndAlso p.Y Mod 5 = 0).ToArray
                    Dim moran_test As MoranTest

                    If sampling.Length < 3 Then
                        moran_test = New MoranTest With {
                            .df = 0, .Expected = 0, .Observed = 0,
                            .prob2 = 1, .pvalue = 1, .SD = 0,
                            .t = 0, .z = 0
                        }
                    Else
                        moran_test = MoranTest.moran_test(
                            x:=sampling.Select(Function(a) a(offset)).ToArray,
                            c1:=sampling.Select(Function(p) CDbl(p.X)).ToArray,
                            c2:=sampling.Select(Function(p) CDbl(p.Y)).ToArray,
                            parallel:=Not parallel,
                            throwMaxIterError:=False
                        )
                    End If

                    Dim counts As New List(Of Double)

                    For Each top As Deconvolute.PixelData In (From cell As Deconvolute.PixelData
                                                              In points
                                                              Order By cell(offset) Descending
                                                              Take 30)

                        Call counts.Add(Aggregate cell As Point
                                        In pixels.Query(top.X, top.Y, grid_size)
                                        Where Not cell.IsEmpty
                                        Into Count)
                    Next

                    If counts.Count > 0 Then
                        density(offset) = SIMD.Divide.f64_op_divide_f64_scalar(counts.ToArray, A).Average
                    End If

                    moran(offset) = moran_test.Observed
                    pvalue(offset) = moran_test.pvalue
                Next
            End Sub

            Public Iterator Function Result() As IEnumerable(Of IonStat)
                For i As Integer = 0 To feature_size - 1
                    Yield New IonStat With {
                        .averageIntensity = average(i),
                        .basePixelX = x(i),
                        .basePixelY = y(i),
                        .density = density(i),
                        .entropy = entropy(i),
                        .maxIntensity = max_into(i),
                        .moran = moran(i),
                        .mz = mz(i),
                        .mzmax = mzmax(i),
                        .mzmin = mzmin(i),
                        .mzwidth = MassWindow.ToString(.mzmax, .mzmin),
                        .pixels = cells(i),
                        .pvalue = pvalue(i),
                        .Q1Intensity = q1(i),
                        .Q2Intensity = q2(i),
                        .Q3Intensity = q3(i),
                        .rsd = rsd(i),
                        .sparsity = sparsity(i)
                    }
                Next
            End Function
        End Class
    End Module
End Namespace
