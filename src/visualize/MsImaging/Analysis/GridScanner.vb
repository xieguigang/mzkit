Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Model
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language.Python
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions

Public Module GridScanner

    <Extension>
    Public Function IonColocalization(raw As mzPack,
                                      Optional grid_width As Integer = 5,
                                      Optional grid_height As Integer = 5,
                                      Optional repeats As Integer = 3,
                                      Optional bag_size As Integer = 32,
                                      Optional mzdiff As Double = 0.01) As IEnumerable

        Dim grid2 = Grid(Of ScanMS1).Create(raw.MS, Function(scan) scan.GetMSIPixel)

        grid_width = grid2.width / grid_width
        grid_height = grid2.height / grid_height

        Dim region As New Size(grid_width / 2, grid_height / 2)
        Dim matrix = grid2 _
            .PopulateIonMatrix(region, repeats, bag_size, mzdiff) _
            .ToArray

    End Function

    <Extension>
    Private Iterator Function PopulateIonMatrix(grid2 As Grid(Of ScanMS1),
                                                region As Size,
                                                repeats As Integer,
                                                bag_size As Integer,
                                                mzdiff As Double) As IEnumerable(Of EntityClusterModel)

        Dim blocks As New Dictionary(Of String, SeqValue(Of ScanMS1())())
        Dim points As ScanMS1()
        Dim grid_width = region.Width * 2
        Dim grid_height = region.Height * 2

        For i As Integer = 0 To grid2.width Step grid_width
            For j As Integer = 0 To grid2.height Step grid_height
                points = grid2 _
                    .Query(i - region.Width, j - region.Height, region) _
                    .ToArray

                If points.Length > 0 Then
                    Call blocks.Add(
                        key:=$"{i},{j}",
                        value:=Bootstraping _
                            .Samples(points, bag_size, bags:=repeats) _
                            .ToArray
                    )
                End If
            Next
        Next

        Dim mzErr As Tolerance = Tolerance.DeltaMass(mzdiff)
        Dim allMz As ms2() = blocks.Values _
            .IteratesALL _
            .Select(Function(i) i.value) _
            .IteratesALL _
            .Select(Function(m) m.GetMs) _
            .IteratesALL _
            .ToArray _
            .Centroid(mzErr, New RelativeIntensityCutoff(0)) _
            .ToArray

        For Each mz As ms2 In allMz
            Dim ion As New EntityClusterModel With {.ID = mz.mz.ToString("F3")}

            For Each scan In blocks
                For Each group In scan.Value
                    ion($"{scan.Key}-{group.i}") = group.value _
                        .Select(Function(p) p.GetIntensity(mz.mz, mzErr)) _
                        .Average
                Next
            Next

            Yield ion
        Next
    End Function
End Module
