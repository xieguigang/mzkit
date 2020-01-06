#Region "Microsoft.VisualBasic::49a9c7c353e33db711d396c78788b751, src\mzmath\ms2_math-core\Ms1\ms1.vb"

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

    ' Class Ms1Feature
    ' 
    '     Properties: ID, mz, rt
    ' 
    '     Function: ToString
    ' 
    ' Interface IMs1
    ' 
    '     Properties: mz
    ' 
    ' Interface IRetentionTime
    ' 
    '     Properties: rt
    ' 
    ' Class MetaInfo
    ' 
    '     Properties: name, xref
    ' 
    ' Interface IMs1Scan
    ' 
    '     Properties: intensity
    ' 
    ' Class ms1_scan
    ' 
    '     Properties: intensity, mz, scan_time
    ' 
    '     Function: GroupByMz, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Math
Imports sys = System.Math

''' <summary>
''' The ms1 peak
''' </summary>
Public Class Ms1Feature : Implements INamedValue, IMs1, IRetentionTime

    <Column(Name:="#ID")>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property mz As Double Implements IMs1.mz
    Public Property rt As Double Implements IMs1.rt

    Public Overrides Function ToString() As String
        Return $"{sys.Round(mz, 4)}@{rt}"
    End Function
End Class

Public Interface IMs1 : Inherits IRetentionTime

    Property mz As Double

End Interface

Public Interface IRetentionTime

    ''' <summary>
    ''' Rt in seconds
    ''' </summary>
    ''' <returns></returns>
    Property rt As Double

End Interface

''' <summary>
''' 质谱标准品基本注释信息
''' </summary>
Public Class MetaInfo : Inherits Ms1Feature

    Public Property name As String

    ''' <summary>
    ''' 这个ms1信息所对应的物质在数据库之中的编号信息列表
    ''' </summary>
    ''' <returns></returns>
    Public Property xref As Dictionary(Of String, String)

End Class

Public Interface IMs1Scan : Inherits IMs1

    Property intensity As Double

End Interface

Public Class ms1_scan : Implements IMs1, IMs1Scan

    <XmlAttribute> Public Property mz As Double Implements IMs1.mz
    <XmlAttribute> Public Property scan_time As Double Implements IMs1.rt
    <XmlAttribute> Public Property intensity As Double Implements IMs1Scan.intensity

    Public Overrides Function ToString() As String
        Return $"{mz.ToString("F4")}@{sys.Round(scan_time)} ({intensity})"
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
        Dim mzGroups = data _
            .GroupBy(Function(scan) scan.mz, AddressOf (tolerance Or Tolerance.DefaultTolerance).Assert) _
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
