
#If netcore5 = 0 Then
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

''' <summary>
''' [mz, rt, intensity]
''' </summary>
Public Class ms1_scan : Implements IMs1, IMs1Scan

    <XmlAttribute> Public Property mz As Double Implements IMs1.mz
    <XmlAttribute> Public Property scan_time As Double Implements IMs1.rt
    <XmlAttribute> Public Property intensity As Double Implements IMs1Scan.intensity

    Public Overrides Function ToString() As String
        Return $"{mz.ToString("F4")}@{stdNum.Round(scan_time)} ({intensity})"
    End Function

    ''' <summary>
    ''' 按照``m/z``分组合并取出<see cref="intensity"/>最大值作为合并之后的结果
    ''' 这个函数是忽略掉<see cref="scan_time"/>的，即认为这些数据都是获取自同一个
    ''' <see cref="scan_time"/>下的结果
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="tolerance">默认是误差在0.3个道尔顿以内</param>
    ''' <returns></returns>
    Public Shared Function GroupByMz(data As IEnumerable(Of ms1_scan), Optional tolerance As Tolerance = Nothing) As ms1_scan()
        Dim mzGroups As NamedCollection(Of ms1_scan)() = data _
            .GroupBy(Function(scan) scan.mz, tolerance Or tolerance.DefaultTolerance) _
            .ToArray
        Dim scans As ms1_scan() = mzGroups _
            .Select(Function(group)
                        Dim mz = group.Select(Function(d) d.mz).Average
                        Dim into = group.Select(Function(d) d.intensity).Max
                        Dim time = group.Select(Function(d) d.scan_time).Average

                        Return New ms1_scan With {
                            .mz = mz,
                            .intensity = into,
                            .scan_time = time
                        }
                    End Function) _
            .ToArray

        Return scans
    End Function
End Class