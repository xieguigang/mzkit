Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.StatsMath
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Point = System.Drawing.Point
Imports std = System.Math

Public Class SpatialIonStats

    ''' <summary>
    ''' extract the ion features from the pixel data collection
    ''' </summary>
    ''' <param name="allPixels"></param>
    ''' <param name="nsize"></param>
    ''' <param name="da"></param>
    ''' <param name="mz"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    Public Shared Function DoStat(allPixels As PixelScan(),
                                  Optional nsize As Integer = 5,
                                  Optional da As Double = 0.05,
                                  Optional mz As Double() = Nothing,
                                  Optional parallel As Boolean = False) As IEnumerable(Of IonStat)
        Dim ionList = allPixels _
            .Select(Function(i)
                        Dim pt As New Point(i.X, i.Y)
                        Dim ions = i.GetMsPipe.Select(Function(ms) New PixelData(pt, ms))
                        Return ions.ToArray
                    End Function) _
            .ToArray
        Dim total_spots As Integer = allPixels.Length

        If Not mz.IsNullOrEmpty Then
            Dim allHits = ionList _
                .AsParallel _
                .Select(Function(sp)
                            Return sp _
                                .Where(Function(i)
                                           Return mz.Any(Function(mzi) std.Abs(mzi - i.mz) <= da)
                                       End Function) _
                                .ToArray
                        End Function) _
                .DoCall(Function(allIons) allIons.DoStatInternal(nsize, da,
                                                                 total_spots:=total_spots,
                                                                 parallel:=parallel)) _
                .ToList

            For Each mzi As Double In mz
                If allHits.Count = 0 OrElse Not allHits.All(Function(m) std.Abs(m.mz - mzi) <= da) Then
                    ' missing current ion
                    ' fill empty
                    Call allHits.Add(New IonStat With {.mz = mzi})
                End If
            Next

            Return allHits
        Else
            Return ionList.DoStatInternal(nsize, da,
                                          total_spots:=total_spots,
                                          parallel:=parallel)
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
            .AsParallel _
            .Select(Iterator Function(scan) As IEnumerable(Of PixelData)
                        Dim xy As Point = scan.GetMSIPixel

                        For Each ms1 As ms2 In scan.GetMs
                            Yield New PixelData(xy, ms1)
                        Next
                    End Function) _
            .Select(Function(p) p.ToArray) _
            .DoCall(Function(allIons)
                        Call VBDebugger.EchoLine("start to pull all pixel data from the raw data pack...")
                        Return allIons.DoStatInternal(nsize, da,
                                                      total_spots:=raw.MS.Length,
                                                      parallel:=parallel)
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
        Dim stats As IonStat = ion.DoStatSingleIon(nsize,
                                                   total_spots:=layer.DimensionSize.Area,
                                                   parallel:=True)
        Return stats
    End Function
End Class
