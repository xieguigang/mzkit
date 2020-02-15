Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Math
Imports scan = BioNovoGene.Analytical.MassSpectrometry.Math.IMs1Scan

''' <summary>
''' #### 解卷积计算步骤
''' 
''' 1. 首先对每一个原始数据点按照mz进行数据分组
''' 2. 对每一个mz数据分组按照rt进行升序排序
''' 3. 对每一个mz数据分组进行解卷积，得到峰列表
''' 4. 输出peaktable结果，完成解卷积操作
''' </summary>
Public Module Deconvolution

    ''' <summary>
    ''' All of the mz value in <paramref name="mzpoints"/> should be equals
    ''' </summary>
    ''' <param name="mzpoints"></param>
    ''' <returns></returns>
    ''' <remarks>应用于处理复杂的样本数据</remarks>
    <Extension>
    Public Function GetPeakGroups(mzpoints As IEnumerable(Of TICPoint)) As IEnumerable(Of PeakFeature)
        Dim timepoints = mzpoints.OrderBy(Function(p) p.time).ToArray

        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 进行原始数据的mz分组操作，然后进行rt的升序排序
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <returns></returns>
    Public Function GetMzGroup(scans As IEnumerable(Of scan), Optional tolerance As Tolerance = Nothing) As IEnumerable(Of scan())
        Dim mz_groups = scans.GroupBy(Function(t) t.mz, AddressOf tolerance.Assert)

    End Function

End Module
