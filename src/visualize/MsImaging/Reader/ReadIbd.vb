Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Application = Microsoft.VisualBasic.Parallel

Namespace Reader

    Public Class ReadIbd : Inherits PixelReader

        Dim pixels As ibdPixel()

        Public ReadOnly Property ibd As ibdReader

        Public ReadOnly Property UUID As String
            Get
                Return ibd.UUID
            End Get
        End Property

        Public Overrides ReadOnly Property dimension As Size

        Sub New(imzML As String, Optional memoryCache As Boolean = False)
            ibd = ibdReader.Open(imzML.ChangeSuffix("ibd"))
            pixels = XML.LoadScans(file:=imzML) _
                .Select(Function(p) New ibdPixel(ibd, p)) _
                .ToArray
            dimension = New Size With {
                .Width = pixels.Select(Function(p) p.X).Max,
                .Height = pixels.Select(Function(p) p.Y).Max
            }

            If memoryCache Then
                Call "loading raw data into memory cache...".__DEBUG_ECHO

                For Each p As ibdPixel In pixels
                    Call p.GetMs()
                Next

                Call ibd.Dispose()
                Call "cache done!".__DEBUG_ECHO
            End If
        End Sub

        ''' <summary>
        ''' load all ions m/z in the raw data file
        ''' </summary>
        ''' <param name="ppm"></param>
        ''' <returns></returns>
        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Dim mzlist = pixels _
                .Select(Function(p)
                            Return Application.DoEvents(Function() p.ReadMz)
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

        Protected Overrides Sub release()
            Call ibd.Dispose()
        End Sub

        Protected Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return pixels.Select(Function(p) DirectCast(p, PixelScan))
        End Function
    End Class
End Namespace