Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math

''' <summary>
''' a unify raw data reader for MSI render
''' </summary>
Public MustInherit Class PixelReader : Implements IDisposable

    Private disposedValue As Boolean

    Public MustOverride ReadOnly Property dimension As Size

    Protected MustOverride Sub release()

    Protected MustOverride Function AllPixels() As IEnumerable(Of PixelScan)

    Private Iterator Function FindMatchedPixels(mz As Double(), tolerance As Tolerance) As IEnumerable(Of PixelScan)
        For Each pixel As PixelScan In AllPixels()
            If pixel.HasAnyMzIon(mz, tolerance) Then
                Yield pixel
            End If
        Next
    End Function

    ''' <summary>
    ''' load pixels data for match a given list of m/z ions with tolerance
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="skipZero"></param>
    ''' <returns></returns>
    Public Iterator Function LoadPixels(mz As Double(), tolerance As Tolerance, Optional skipZero As Boolean = True) As IEnumerable(Of PixelData)
        Dim pixel As PixelData

        For Each point As PixelScan In FindMatchedPixels(mz, tolerance)
            Dim msScan As ms2() = point.GetMs
            Dim into As NamedCollection(Of ms2)() = msScan _
                .Where(Function(mzi)
                           Return mz.Any(Function(dmz) tolerance(mzi.mz, dmz))
                       End Function) _
                .GroupBy(Function(a) a.mz, tolerance) _
                .ToArray

            Call Application.DoEvents()

            If skipZero AndAlso into.Length = 0 Then
                Continue For
            Else
                For Each mzi As NamedCollection(Of ms2) In into
                    pixel = New PixelData With {
                        .x = point.X,
                        .y = point.Y,
                        .mz = Val(mzi.name),
                        .intensity = Aggregate x In mzi Into Max(x.intensity)
                    }

                    Yield pixel
                Next
            End If
        Next
    End Function

    ''' <summary>
    ''' load all ions m/z in the raw data file
    ''' </summary>
    ''' <param name="ppm"></param>
    ''' <returns></returns>
    Public MustOverride Function LoadMzArray(ppm As Double) As Double()

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call release()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
