Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' the <see cref="xcms2"/> read/write helper
''' </summary>
Public Module SaveXcms

    ''' <summary>
    ''' read table in ascii text file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="tsv"></param>
    ''' <returns></returns>
    Public Function ReadTextTable(file As String, Optional tsv As Boolean = False) As PeakSet
        Dim deli As Char = If(tsv, vbTab, ","c)
        Dim buf As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
        Dim s As New StreamReader(buf)
        Dim headers As Index(Of String) = s.ReadLine.Split(deli)
        Dim ID As Integer = headers.GetSynonymOrdinal("xcms_id", "id", "ID", "")
        Dim mz As Integer = headers.GetSynonymOrdinal("mz", "m/z")
        Dim mzmin As Integer = headers("mzmin")
        Dim mzmax As Integer = headers("mzmax")
        Dim rt As Integer = headers("rt")
        Dim rtmin As Integer = headers("rtmin")
        Dim rtmax As Integer = headers("rtmax")
        Dim npeaks As Integer = headers.GetSynonymOrdinal("npeaks", ".")

        Call headers.Delete("xcms_id", "id", "ID", "")
        Call headers.Delete("mz", "m/z")
        Call headers.Delete("mzmin", "mzmax")
        Call headers.Delete("rt", "rtmin", "rtmax")
        Call headers.Delete("npeaks", ".")
        Call headers.Delete("maxinto")

        Dim peaks As xcms2() = s _
            .GetPeaks(deli, ID, mz, mzmin, mzmax, rt, rtmin, rtmax, peaks:=headers.ToArray) _
            .ToArray

        Call buf.Dispose()

        Return New PeakSet With {.peaks = peaks.ToArray}
    End Function

    <Extension>
    Private Iterator Function GetPeaks(s As StreamReader, deli As Char,
                                       ID As Integer,
                                       mz As Integer, mzmin As Integer, mzmax As Integer,
                                       rt As Integer, rtmin As Integer, rtmax As Integer,
                                       peaks As SeqValue(Of String)()) As IEnumerable(Of xcms2)

        Dim str As Value(Of String) = ""
        Dim t As String()
        Dim pk As xcms2

        If ID < 0 Then
            Throw New InvalidDataException($"the required of the unique id in peaktable could not be found!")
        End If
        If mz < 0 Then
            Throw New InvalidDataException($"the required of the ion m/z field in peaktable could not be found!")
        End If
        If rt < 0 Then
            Throw New InvalidDataException($"the required of the ion peak rt field in peaktable could not be found!")
        End If

        Do While (str = s.ReadLine) IsNot Nothing
            t = str.Split(deli)
            pk = New xcms2 With {
                .ID = t(ID),
                .mz = Double.Parse(t(mz)),
                .mzmax = If(mzmax > -1, Val(t(mzmax)), .mz),
                .mzmin = If(mzmin > -1, Val(t(mzmin)), .mz),
                .rt = Double.Parse(t(rt)),
                .rtmax = If(rtmax > -1, Val(t(rtmax)), .rt),
                .rtmin = If(rtmin > -1, Val(t(rtmin)), .rt)
            }


        Loop
    End Function

End Module
