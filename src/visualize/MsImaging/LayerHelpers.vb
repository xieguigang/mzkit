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
                                        Optional gridSize As Integer = 5) As IEnumerable(Of DoubleTagged(Of SingleIonLayer))

        Dim cellSize As New Size(gridSize, gridSize)
        Dim graph As Grid(Of ScanMS1) = Grid(Of ScanMS1).Create(raw.MS, Function(scan) scan.GetMSIPixel)
        Dim mzErr As Tolerance = mzdiff Or Tolerance.DefaultTolerance
        Dim reader As PixelReader = New ReadRawPack(raw)
        Dim allMz = raw.MS _
            .AsParallel _
            .Select(Function(scan)
                        Return scan _
                            .GetMs _
                            .ToArray _
                            .Centroid(mzErr, New RelativeIntensityCutoff(0.05))
                    End Function) _
            .IteratesALL _
            .ToArray _
            .Centroid(mzErr, New RelativeIntensityCutoff(0)) _
            .ToArray

        For Each mz As Double In allMz.Select(Function(mzi) mzi.mz)
            Dim layer As SingleIonLayer = reader.LoadLayer(mz, mzErr)
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
