Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData

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
End Module
