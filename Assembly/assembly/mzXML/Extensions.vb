Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports sciBASIC.ZIP
Imports SMRUCC.MassSpectrum.Assembly.mzXML

Public Module Extensions

    ''' <summary>
    ''' 解析出色谱峰数据
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <returns></returns>
    <Extension> Public Function ExtractMzI(peaks As peaks) As List(Of MSMSPeak)
        Dim floats#() = peaks.Base64Decode
        Dim data As New List(Of MSMSPeak)

        For Each signal As Double() In floats.Split(2)
            data += New MSMSPeak With {
                .mz = signal(Scan0),   'm/z质核比数据
                .intensity = signal(1)        ' 信号强度, 归一化为 0-100 之间的数值
            }
        Next

        Return data
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="scan">
    ''' 可以依据扫描信号结果数据<see cref="scan.msLevel"/>来获取相应的质谱信号数据
    ''' 
    ''' + 1, MS1   一级质谱信号数据
    ''' + 2, MS/MS 二级质谱信号数据
    ''' </param>
    ''' <returns></returns>
    <Extension> Public Function ExtractMzI(scan As scan) As (name$, peaks As MSMSPeak())
        Dim name$ = scan.__getName
        Dim peaks As MSMSPeak() = scan.peaks.ExtractMzI(name)
        Return (name, peaks)
    End Function

    <Extension> Private Function __getName(scan As scan) As String
        Dim level$ = If(scan.msLevel = 1, "MS1", "MS/MS")
        Return $"[{level}] {scan.scanType} Scan, ({scan.polarity}) mz={scan.precursorMz} / retentionTime={scan.retentionTime}"
    End Function

    ''' <summary>
    ''' 对质谱扫描信号结果进行解码操作
    ''' </summary>
    ''' <param name="peaks$"></param>
    ''' <returns></returns>
    <Extension> Public Function Base64Decode(stream As IBase64Container) As Double()
        Dim bytes As Byte() = Convert.FromBase64String(stream.BinaryArray)
        Dim ms As New MemoryStream
        Dim gz As New ZLibStream(New MemoryStream(bytes), CompressionMode.Decompress)

        gz.CopyTo(ms)
        bytes = ms.ToArray

        Dim floats#() = bytes _
            .Split(8) _
            .Select(Function(b) BitConverter.ToDouble(b, Scan0)) _
            .ToArray

        Return floats
    End Function
End Module
