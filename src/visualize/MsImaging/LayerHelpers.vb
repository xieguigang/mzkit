Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.DataMining.DensityQuery
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Linq

Public Module LayerHelpers

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns>
    ''' a collection of m/z tagged density value
    ''' </returns>
    <Extension>
    Public Iterator Function GetMSIIons(raw As mzPack,
                                        Optional mzdiff As Tolerance = Nothing,
                                        Optional gridSize As Integer = 5,
                                        Optional qcut As Double = 0.3) As IEnumerable(Of DoubleTagged(Of SingleIonLayer))

        Dim cellSize As New Size(gridSize, gridSize)
        Dim graph As Grid(Of ScanMS1) = Grid(Of ScanMS1).Create(raw.MS, Function(scan) scan.GetMSIPixel)
        Dim mzErr As Tolerance = mzdiff Or Tolerance.DefaultTolerance
        Dim reader As PixelReader = New ReadRawPack(raw)
        Dim ncut As Integer = graph.size * qcut
        Dim allMz = raw.MS _
            .AsParallel _
            .Select(Function(scan)
                        Dim pt As Point = scan.GetMSIPixel

                        Return scan _
                            .GetMs _
                            .ToArray _
                            .Centroid(mzErr, New RelativeIntensityCutoff(0.01)) _
                            .Select(Function(mzi) (mzi, pt))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(d) d.mzi.mz, mzErr) _
            .Where(Function(d) d.Length > ncut) _
            .ToArray

        For i As Integer = 0 To allMz.Length - 1
            Dim mz As Double = allMz(i) _
                .OrderByDescending(Function(d) d.mzi.intensity) _
                .First _
                .mzi.mz
            Dim layer As New SingleIonLayer With {
                .IonMz = mz,
                .DimensionSize = New Size(graph.width, graph.height),
                .MSILayer = Grid(Of (mzi As ms2, pt As Point)) _
                    .Create(allMz(i), Function(d) d.Item2) _
                    .EnumerateData _
                    .Select(Function(d)
                                Return New PixelData With {
                                    .intensity = d.mzi.intensity,
                                    .level = 0,
                                    .mz = d.mzi.mz,
                                    .x = d.pt.X,
                                    .y = d.pt.Y
                                }
                            End Function) _
                    .ToArray
            }
            Dim density As NamedValue(Of Double)() = layer.MSILayer _
                .Density(
                    getName:=Function(pt) $"{pt.x},{pt.y}",
                    getX:=Function(p) p.x,
                    getY:=Function(p) p.y,
                    gridSize:=cellSize,
                    parallel:=True
                ) _
                .ToArray
            Dim q As Double = density.Select(Function(d) d.Value).Median

            Yield New DoubleTagged(Of SingleIonLayer) With {
               .Tag = mz,
               .Value = layer,
               .TagStr = q
            }
        Next
    End Function

End Module
