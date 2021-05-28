Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Application = Microsoft.VisualBasic.Parallel

Public Class ReadIbd : Inherits PixelReader

    Dim pixels As ScanData()

    Public ReadOnly Property ibd As ibdReader

    Public ReadOnly Property UUID As String
        Get
            Return ibd.UUID
        End Get
    End Property

    Public Overrides ReadOnly Property dimension As Size

    Sub New(imzML As String)
        ibd = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        pixels = XML.LoadScans(file:=imzML).ToArray
        dimension = New Size With {
            .Width = pixels.Select(Function(p) p.x).Max,
            .Height = pixels.Select(Function(p) p.y).Max
        }
    End Sub

    ''' <summary>
    ''' load all ions m/z in the raw data file
    ''' </summary>
    ''' <param name="ppm"></param>
    ''' <returns></returns>
    Public Overrides Function LoadMzArray(ppm As Double) As Double()
        Dim mzlist = pixels _
            .Select(Function(p)
                        Return Application.DoEvents(Function() ibd.ReadArray(p.MzPtr))
                    End Function) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim groups = mzlist _
            .GroupBy(Function(mz) mz, Tolerance.PPM(ppm)) _
            .Select(Function(mz) Val(mz.name)) _
            .OrderBy(Function(mzi) mzi) _
            .ToArray

        Return groups
    End Function

    Public Overrides Iterator Function LoadPixels(mz As Double(), tolerance As Tolerance, Optional skipZero As Boolean = True) As IEnumerable(Of PixelData)
        Dim pixel As PixelData

        For Each point As ScanData In Me.pixels
            Dim msScan As ms2() = ibd.GetMSMS(point)
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
                        .x = point.x,
                        .y = point.y,
                        .mz = Val(mzi.name),
                        .intensity = Aggregate x In mzi Into Max(x.intensity)
                    }

                    Yield pixel
                Next
            End If
        Next
    End Function

    Protected Overrides Sub release()
        Call ibd.Dispose()
    End Sub
End Class
