#Region "Microsoft.VisualBasic::86f01581a9b6bb15ce0762e2888b225a, G:/mzkit/src/mzmath/mz_deco//Save/SaveXcms.vb"

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

    '   Total Lines: 161
    '    Code Lines: 118
    ' Comment Lines: 19
    '   Blank Lines: 24
    '     File Size: 5.70 KB


    ' Module SaveXcms
    ' 
    '     Function: GetPeaks, ReadSample, ReadTextTable
    ' 
    '     Sub: DumpSample
    ' 
    ' /********************************************************************************/

#End Region

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

        Dim offsets = headers.ToArray
        Dim peaks As xcms2() = s _
            .GetPeaks(deli, ID, mz, mzmin, mzmax, rt, rtmin, rtmax, peaks:=offsets) _
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

            For Each sample As SeqValue(Of String) In peaks
                pk(sample.value) = Val(t(sample))
            Next

            Yield pk
        Loop
    End Function

    ''' <summary>
    ''' save as binary file
    ''' </summary>
    ''' <param name="sample"></param>
    ''' <param name="file"></param>
    <Extension>
    Public Sub DumpSample(sample As PeakSet, file As Stream)
        Dim bin As New BinaryWriter(file)
        Dim sampleNames As String() = sample.sampleNames

        Call bin.Write(sample.ROIs)
        Call bin.Write(sampleNames.Length)

        For Each name As String In sampleNames
            Call bin.Write(name)
        Next

        For Each pk As xcms2 In sample.peaks
            Call bin.Write(pk.ID)
            Call bin.Write(pk.mz)
            Call bin.Write(pk.rt)
            Call bin.Write(pk.mzmin)
            Call bin.Write(pk.mzmax)
            Call bin.Write(pk.rtmin)
            Call bin.Write(pk.rtmax)

            For Each name As String In sampleNames
                Call bin.Write(pk(name))
            Next
        Next

        Call bin.Flush()
    End Sub

    ''' <summary>
    ''' load binary file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Function ReadSample(file As Stream) As PeakSet
        Dim rd As New BinaryReader(file)
        Dim ROIs As Integer = rd.ReadInt32
        Dim samples As Integer = rd.ReadInt32
        Dim names As String() = New String(samples - 1) {}
        Dim peaks As xcms2() = New xcms2(ROIs - 1) {}
        Dim pk As xcms2

        For i As Integer = 0 To samples - 1
            names(i) = rd.ReadString
        Next

        For i As Integer = 0 To ROIs - 1
            pk = New xcms2 With {
                .ID = rd.ReadString,
                .mz = rd.ReadDouble,
                .rt = rd.ReadDouble,
                .mzmin = rd.ReadDouble,
                .mzmax = rd.ReadDouble,
                .rtmin = rd.ReadDouble,
                .rtmax = rd.ReadDouble
            }

            For offset As Integer = 0 To samples - 1
                pk(names(offset)) = rd.ReadDouble
            Next

            peaks(i) = pk
        Next

        Return New PeakSet With {.peaks = peaks}
    End Function
End Module
