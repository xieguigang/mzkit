#Region "Microsoft.VisualBasic::a6e811aac97d9eaf46295c406281e99d, visualize\MsImaging\Analysis\MatrixExports.vb"

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

    '   Total Lines: 81
    '    Code Lines: 54 (66.67%)
    ' Comment Lines: 15 (18.52%)
    '    - Xml Docs: 86.67%
    ' 
    '   Blank Lines: 12 (14.81%)
    '     File Size: 2.89 KB


    ' Module MatrixExports
    ' 
    '     Function: CreateMatrix
    '     Class DeconvoluteTask
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Solve
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Parallel

Public Module MatrixExports

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <param name="ions"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' data matrix in parallel
    ''' </remarks>
    Public Function CreateMatrix(pixels As IEnumerable(Of PixelScan), ions As IEnumerable(Of Double), mzdiff As Tolerance) As MzMatrix
        Dim mzIndex As New MzPool(ions)
        Dim len = mzIndex.size
        Dim task As New DeconvoluteTask(pixels.ToArray, App.CPUCoreNumbers - 1) With {
            .len = len,
            .mzIndex = mzIndex
        }

        Call task.Run()

        Return New MzMatrix With {
            .matrix = task.spots _
                .Where(Function(i) Not i Is Nothing) _
                .ToArray,
            .matrixType = FileApplicationClass.MSImaging,
            .mz = mzIndex.ionSet,
            .mzmax = .mz,
            .mzmin = .mz,
            .tolerance = mzdiff.GetScript
        }
    End Function

    Private Class DeconvoluteTask : Inherits VectorTask

        ReadOnly pixels As PixelScan()

        ''' <summary>
        ''' 20240909
        ''' 
        ''' there is a bug about the paralle task, may produce the null value?
        ''' </summary>
        Public spots As Deconvolute.PixelData()
        Public len As Integer
        Public mzIndex As MzPool

        Public Sub New(pixels As PixelScan(), n_threads As Integer)
            MyBase.New(pixels.Length, workers:=n_threads)

            Me.pixels = pixels
            Me.spots = Allocate(Of Deconvolute.PixelData)(all:=True)
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            For offset As Integer = start To ends
                Dim xy = pixels(offset)
                Dim ms = xy.GetMs
                Dim mz As Double() = ms.Select(Function(i) i.mz).ToArray
                Dim into As Double() = ms.Select(Function(i) i.intensity).ToArray
                Dim v As Double() = SpectraEncoder.DeconvoluteScan(mz, into, len, mzIndex)

                spots(offset) = New Deconvolute.PixelData With {
                    .X = xy.X,
                    .Y = xy.Y,
                    .intensity = v,
                    .label = xy.scanId
                }
            Next
        End Sub
    End Class

End Module
