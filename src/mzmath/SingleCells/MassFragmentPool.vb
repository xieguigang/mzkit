Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Public Class MassFragmentPool : Implements INamedValue, IMassFragmentPool

    Public Property sample As String Implements INamedValue.Key, IMassFragmentPool.sample
    Public Property fragments As ms2() Implements IMassFragmentPool.fragments

    Sub New()
    End Sub

    Sub New(copy As IMassFragmentPool)
        sample = copy.sample
        fragments = copy.fragments
    End Sub

    Public Overrides Function ToString() As String
        Return sample
    End Function

    Public Shared Function CreateMatrix(Of T As IMassFragmentPool)(rawdata As IEnumerable(Of T), Optional mzdiff As Double = 0.01, Optional q As Double = 0.01) As MzMatrix
        Dim pool As MassFragmentPool() = rawdata.Select(Function(a) New MassFragmentPool(a)).ToArray
        Dim features As MassWindow() = pool.Select(Function(a) a.fragments.Select(Function(m) m.mz).ToArray).AsList.GetMzIndexFastBin(mzdiff, q)
        Dim mzSet As Double() = features.Mass
        Dim mzIndex As New MzPool(mzSet)
        Dim matrix As PixelData() = New PixelData(pool.Length - 1) {}
        Dim len As Integer = features.Length

        For i As Integer = 0 To pool.Length - 1
            Dim scan As MassFragmentPool = pool(i)
            Dim scan_mz = scan.fragments.Select(Function(a) a.mz).ToArray
            Dim scan_into = scan.fragments.Select(Function(a) a.intensity).ToArray
            Dim v As Double() = SpectraEncoder.DeconvoluteScan(scan_mz, scan_into, len, mzIndex)

            matrix(i) = New PixelData With {
                .intensity = v,
                .label = scan.sample,
                .X = 0,
                .Y = 0,
                .Z = 0
            }
        Next

        Return New MzMatrix With {
            .matrix = matrix,
            .mz = mzSet,
            .tolerance = "da:" & mzdiff,
            .matrixType = FileApplicationClass.LCMS,
            .mzmin = features.Select(Function(mzi) mzi.mzmin).ToArray,
            .mzmax = features.Select(Function(mzi) mzi.mzmax).ToArray
        }
    End Function

End Class

Public Interface IMassFragmentPool

    ReadOnly Property sample As String
    ReadOnly Property fragments As ms2()

End Interface

Public Class MSnFragmentProvider
    Implements IMassFragmentPool

    Public ReadOnly Property sample As String Implements IMassFragmentPool.sample
        Get
            Return raw.source
        End Get
    End Property

    Public ReadOnly Property fragments As ms2() Implements IMassFragmentPool.fragments
        Get
            Return raw.MS _
                .Select(Function(m1) m1.products) _
                .IteratesALL _
                .Select(Function(m2) m2.GetMs) _
                .IteratesALL _
                .ToArray
        End Get
    End Property

    ReadOnly raw As IMZPack

    Sub New(raw As IMZPack)
        Me.raw = raw
    End Sub

    Public Overrides Function ToString() As String
        Return sample
    End Function

End Class