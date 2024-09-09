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
            .matrix = task.spots,
            .matrixType = FileApplicationClass.MSImaging,
            .mz = mzIndex.ionSet,
            .mzmax = .mz,
            .mzmin = .mz,
            .tolerance = mzdiff.GetScript
        }
    End Function

    Private Class DeconvoluteTask : Inherits VectorTask

        ReadOnly pixels As PixelScan()

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
                Dim v As Double() = Deconvolute.Math.DeconvoluteScan(mz, into, len, mzIndex)

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
