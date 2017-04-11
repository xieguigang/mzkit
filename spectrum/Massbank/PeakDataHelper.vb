Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports SMRUCC.proteomics.MS_Spectrum.DATA.Massbank.DATA

Public Module PeakDataHelper

    ''' <summary>
    ''' ``x,y x,y x,y``
    ''' </summary>
    ''' <param name="peakData"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Join(peakData As IEnumerable(Of DoubleTagged(Of Double))) As String
        Return peakData _
            .Select(Function(pk) $"{pk.Tag},{pk.value}") _
            .JoinBy(" ")
    End Function

    ''' <summary>
    ''' 对峰的信号量进行归一化处理
    ''' </summary>
    ''' <param name="record">``MS/MS``信号峰数据</param>
    ''' <returns></returns>
    <Extension>
    Public Function Normalize(record As Record) As DoubleTagged(Of Double)()

    End Function
End Module
