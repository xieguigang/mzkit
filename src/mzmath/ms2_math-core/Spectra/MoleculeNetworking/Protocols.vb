Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math

''' <summary>
''' 1. 按照母离子m/z二叉树聚类
''' 2. 每一个聚类结果作为一个node进行碎片合并
''' 3. 基于node进行molecular networking
''' 4. 导出网络数据
''' </summary>
Public Class Protocols

    ReadOnly ms1_tolerance As Tolerance
    ReadOnly ms2_tolerance As Tolerance

    ReadOnly treeIdentical As Double
    ReadOnly treeSimilar As Double
    ReadOnly intoCutoff As Double

    ''' <summary>
    ''' 步骤1
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    Public Iterator Function BinaryTree(raw As IEnumerable(Of PeakMs2)) As IEnumerable(Of SpectrumCluster)
        Dim tree As New SpectrumTreeCluster(SpectrumTreeCluster.SSMCompares(ms2_tolerance, treeIdentical, treeSimilar))

        Call tree.doCluster(raw.ToArray)

        For Each cluster In tree.PopulateClusters
            Yield cluster
        Next
    End Function

    ''' <summary>
    ''' 为了减少内存占用，在这里进行离散化
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    Private Function centroid(raw As PeakMs2) As PeakMs2
        Return New PeakMs2 With {
            .activation = raw.activation,
            .file = raw.file,
            .collisionEnergy = raw.collisionEnergy,
            .lib_guid = raw.lib_guid,
            .meta = New Dictionary(Of String, String)(raw.meta),
            .mz = raw.mz,
            .mzInto = raw.mzInto.Centroid(ms2_tolerance, intoCutoff).ToArray,
            .precursor_type = raw.precursor_type,
            .rt = raw.rt,
            .scan = raw.scan
        }
    End Function

    Public Iterator Function ProduceNodes(raw As IEnumerable(Of PeakMs2)) As IEnumerable(Of NetworkingNode)
        Dim groupByMz As NamedCollection(Of PeakMs2)() = raw _
            .AsParallel _
            .Select(AddressOf centroid) _
            .GroupBy(Function(peak) peak.mz, ms1_tolerance) _
            .ToArray

        For Each mz As NamedCollection(Of PeakMs2) In groupByMz
            For Each cluster In BinaryTree(mz)
                Yield NetworkingNode.Create(Val(mz.name), cluster, ms2_tolerance)
            Next
        Next
    End Function
End Class

Public Class NetworkingNode

    Public Property representation As PeakMs2

    Public Property members As PeakMs2()
    Public Property mz As Double

    Public Function GetXIC() As ChromatogramTick()
        Return members _
            .Select(Function(a)
                        Return New ChromatogramTick With {
                            .Time = a.rt,
                            .Intensity = a.Ms2Intensity
                        }
                    End Function) _
            .OrderBy(Function(a) a.Time) _
            .ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="tolerance">ms2 tolerance</param>
    ''' <returns></returns>
    Public Shared Function Create(parentIon As Double, raw As SpectrumCluster, tolerance As Tolerance) As NetworkingNode
        Dim ions As PeakMs2() = raw.cluster _
            .Select(Function(a)
                        Dim maxInto = a.mzInto.Select(Function(x) x.intensity).Max

                        For i As Integer = 0 To a.mzInto.Length - 1
                            a.mzInto(i).quantity = a.mzInto(i).intensity / maxInto
                        Next

                        Return a
                    End Function) _
            .ToArray

    End Function

End Class