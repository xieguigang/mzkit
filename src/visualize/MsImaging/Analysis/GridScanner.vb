#Region "Microsoft.VisualBasic::c74c94e8d299627be80685a811692d52, mzkit\src\visualize\MsImaging\Analysis\GridScanner.vb"

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

'   Total Lines: 262
'    Code Lines: 192
' Comment Lines: 31
'   Blank Lines: 39
'     File Size: 10.68 KB


' Module GridScanner
' 
'     Function: (+2 Overloads) IonColocalization, PCAGroups, PopulateClusters, PopulateIonMatrix
'     Class CorrelationAligner
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetSimilarity
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.DataMining
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Statistics.Linq

Public Module GridScanner

    <Extension>
    Private Iterator Function PCAProject(matrix As EntityClusterModel()) As IEnumerable(Of EntityClusterModel)
        Dim block_tags As String() = matrix(Scan0).Properties.Keys.ToArray
        Dim matrix2 = matrix.Z.Select(Function(r) r(block_tags)).ToArray
        'Dim PCA As New PCA(matrix2, center:=False)
        'Dim pc3 = PCA _
        '    .Project(nPC:=3) _
        '    .Select(Function(r, i)
        '                Return New EntityClusterModel With {
        '                    .ID = matrix(i).ID,
        '                    .Properties = New Dictionary(Of String, Double) From {
        '                        {"PC1", r(0)},
        '                        {"PC2", r(1)},
        '                        {"PC3", r(2)}
        '                    }
        '                }
        '            End Function) _
        '    .ToArray

        Throw New NotImplementedException
    End Function

    <Extension>
    Public Iterator Function PCAGroups(raw As IEnumerable(Of PixelScan),
                                       Optional grid_width As Integer = 5,
                                       Optional grid_height As Integer = 5,
                                       Optional repeats As Integer = 3,
                                       Optional bag_size As Integer = 32,
                                       Optional mzdiff As Double = 0.1,
                                       Optional k As Integer = 16) As IEnumerable(Of EntityClusterModel)

        Dim grid2 = Grid(Of IMsScan).Create(
            data:=raw.Select(Function(i) DirectCast(i, IMsScan)),
            getX:=Function(scan) DirectCast(scan, PixelScan).X,
            getY:=Function(scan) DirectCast(scan, PixelScan).Y
        )

        grid_width = grid2.width / grid_width
        grid_height = grid2.height / grid_height

        Dim region As New Size(grid_width / 2, grid_height / 2)
        Dim matrix As EntityClusterModel() = grid2.PopulateIonMatrix(region, repeats, bag_size, mzdiff).ToArray
        ' run dbscan cluster?
        'Dim pc3_groups = New DbscanAlgorithm(Of EntityClusterModel)(AddressOf metric.DistanceTo) _
        '    .ComputeClusterDBSCAN(pc3, eps, 3) _
        '    .ToArray
        Dim pc3_groups = matrix.PCAProject.Kmeans(expected:=k).ToArray

        'For Each group In pc3_groups
        '    For Each x As EntityClusterModel In group
        '        x.Cluster = group.name
        '    Next
        'Next

        Dim pcMatrix = pc3_groups.ToDictionary(Function(a) a.ID)
        Dim pc As EntityClusterModel

        For Each x As EntityClusterModel In matrix
            pc = pcMatrix(x.ID)
            x.Cluster = pc.Cluster
            x!PC1 = pc!PC1
            x!PC2 = pc!PC2
            x!PC3 = pc!PC3

            Yield x
        Next
    End Function

    ''' <summary>
    ''' populate ion co-localization cluster data
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="grid_width"></param>
    ''' <param name="grid_height"></param>
    ''' <param name="repeats"></param>
    ''' <param name="bag_size"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IonColocalization(raw As IEnumerable(Of PixelScan),
                                      Optional grid_width As Integer = 5,
                                      Optional grid_height As Integer = 5,
                                      Optional repeats As Integer = 3,
                                      Optional bag_size As Integer = 32,
                                      Optional mzdiff As Double = 0.1,
                                      Optional equals As Double = 0.6) As IEnumerable(Of EntityClusterModel)

        Dim grid2 = Grid(Of IMsScan).Create(
            data:=raw.Select(Function(i) DirectCast(i, IMsScan)),
            getX:=Function(scan) DirectCast(scan, PixelScan).X,
            getY:=Function(scan) DirectCast(scan, PixelScan).Y
        )

        grid_width = grid2.width / grid_width
        grid_height = grid2.height / grid_height

        Dim region As New Size(grid_width / 2, grid_height / 2)

        Return grid2.PopulateClusters(region, repeats, bag_size, mzdiff, equals)
    End Function

    ''' <summary>
    ''' populate ion co-localization cluster data
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="grid_width"></param>
    ''' <param name="grid_height"></param>
    ''' <param name="repeats"></param>
    ''' <param name="bag_size"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="equals"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IonColocalization(raw As mzPack,
                                      Optional grid_width As Integer = 5,
                                      Optional grid_height As Integer = 5,
                                      Optional repeats As Integer = 3,
                                      Optional bag_size As Integer = 32,
                                      Optional mzdiff As Double = 0.1,
                                      Optional equals As Double = 0.6) As IEnumerable(Of EntityClusterModel)

        Dim grid2 = Grid(Of IMsScan).Create(
            data:=raw.MS.Select(Function(i) DirectCast(i, IMsScan)),
            getPixel:=Function(scan) DirectCast(scan, ScanMS1).GetMSIPixel
        )

        grid_width = grid2.width / grid_width
        grid_height = grid2.height / grid_height

        Dim region As New Size(grid_width / 2, grid_height / 2)

        Return grid2.PopulateClusters(region, repeats, bag_size, mzdiff, equals)
    End Function

    <Extension>
    Private Iterator Function PopulateClusters(grid2 As Grid(Of IMsScan),
                                               region As Size,
                                               repeats As Integer,
                                               bag_size As Integer,
                                               mzdiff As Double,
                                               equals As Double) As IEnumerable(Of EntityClusterModel)

        Dim matrix As Dictionary(Of String, EntityClusterModel) =
            grid2 _
            .PopulateIonMatrix(region, repeats, bag_size, mzdiff) _
            .ToDictionary(Function(e) e.ID)
        Dim align As New CorrelationAligner(matrix)
        Dim root As New ClusterTree
        Dim i As Integer = 1
        Dim args As New ClusterTree.Argument With {.alignment = align, .threshold = equals}

        For Each key As String In matrix.Keys
            Call ClusterTree.Add(root, args.SetTargetKey(key))
        Next

        For Each cluster As ClusterTree In ClusterTree.GetClusters(root)
            Dim obj = matrix(cluster.Data)

            obj.Cluster = "group_" & i
            Yield obj

            For Each id As String In cluster.Members
                obj = matrix(id)
                obj.Cluster = "group_" & i
                Yield obj
            Next

            i += 1
        Next
    End Function

    Private Class CorrelationAligner : Inherits ComparisonProvider

        ReadOnly matrix As Dictionary(Of String, EntityClusterModel)
        ReadOnly sampling As String()

        Public Sub New(matrix As Dictionary(Of String, EntityClusterModel))
            ' equals/gt parameter is no used
            Call MyBase.New(equals:=-1, gt:=-1)

            Me.matrix = matrix
            Me.sampling = matrix.Values _
                .First _
                .Properties _
                .Keys _
                .ToArray
        End Sub

        Public Overrides Function GetSimilarity(x As String, y As String) As Double
            Return matrix(x)(sampling).Pearson(matrix(y)(sampling))
        End Function

        Public Overrides Function GetObject(id As String) As Object
            Return matrix(id)
        End Function
    End Class

    <Extension>
    Private Iterator Function PopulateIonMatrix(grid2 As Grid(Of IMsScan),
                                                region As Size,
                                                repeats As Integer,
                                                bag_size As Integer,
                                                mzdiff As Double) As IEnumerable(Of EntityClusterModel)

        Dim blocks As New Dictionary(Of String, SeqValue(Of IMsScan())())
        Dim points As IMsScan()
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
                        .Select(Function(p) p.GetMzIonIntensity(mz.mz, mzErr)) _
                        .Average
                Next
            Next

            Yield ion
        Next
    End Function
End Module
