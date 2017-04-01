Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.proteomics.MS_Spectrum.mzXML
Imports ZLibNet

Public Module Extensions

    ''' <summary>
    ''' 从mzXML文件之中解析出色谱峰数据
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <returns></returns>
    <Extension> Public Function ExtractMzI(peaks As peaks, Optional name$ = "") As spectrumData
        Dim floats#() = peaks.value.__decode
        Dim data As New List(Of MSSignal)

        For Each signal As Double() In floats.Split(2)
            data += New MSSignal With {
                .x = signal(Scan0),
                .y = signal(1)
            }
        Next

        Return New spectrumData With {
            .data = data,
            .name = name
        }
    End Function

    <Extension> Public Function ExtractMzI(scan As scan) As spectrumData
        Dim name$ = scan.__getName
        Dim signals As spectrumData = scan.peaks.ExtractMzI(name)
        Return signals
    End Function

    <Extension> Private Function __getName(scan As scan) As String
        'Dim table As New Dictionary(Of String, String)
        'Return table.GetJson
        Dim level$ = If(scan.msLevel = 1, "MS1", "MS/MS")
        Return $"[{level}] {scan.scanType} Scan, ({scan.polarity}) mz={scan.precursorMz} / retentionTime={scan.retentionTime}"
    End Function

    ''' <summary>
    ''' 对质谱扫描信号结果进行解码操作
    ''' </summary>
    ''' <param name="peaks$"></param>
    ''' <returns></returns>
    <Extension> Private Function __decode(peaks$) As Double()
        Dim bytes As Byte() = Convert.FromBase64String(peaks)
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
