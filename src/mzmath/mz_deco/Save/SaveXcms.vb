﻿#Region "Microsoft.VisualBasic::fab8fa719e1f6feb2c0c3b19f296a45c, mzmath\mz_deco\Save\SaveXcms.vb"

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

    '   Total Lines: 255
    '    Code Lines: 180 (70.59%)
    ' Comment Lines: 37 (14.51%)
    '    - Xml Docs: 81.08%
    ' 
    '   Blank Lines: 38 (14.90%)
    '     File Size: 9.79 KB


    ' Module SaveXcms
    ' 
    '     Function: DumpSample, GetPeaks, ReadSample, ReadSamplePeaks, ReadTextTable
    ' 
    '     Sub: DumpSample
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Trinity
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
    Public Function ReadTextTable(file As String,
                                  Optional tsv As Boolean = False,
                                  Optional make_unique As Boolean = False,
                                  Optional delete_fields As String() = Nothing) As PeakSet

        Dim deli As Char = If(tsv, vbTab, ","c)
        Dim buf As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
        Dim s As New StreamReader(buf)
        ' 20240530 all of the data header title maybe wrapped with quote
        ' so we needs to trims the data at first!
        Dim headers As Index(Of String) = Strings.Trim(s.ReadLine) _
            .Split(deli) _
            .Select(Function(si) si.Trim(""""c, " "c)) _
            .Indexing

        If headers.Count = 1 Then
            Throw New InvalidDataException("Invalid table file header parse result, please check of the table file format or check of the csv and tsv parameter?")
        Else
            For Each col As String In delete_fields.SafeQuery
                Call headers.Delete(col)
            Next
        End If

        Static required_id As String() = {"xcms_id", "id", "ID", "name"}
        Static required_mz As String() = {"mz", "m/z", "MZ", "M/Z", "mass to charge"}
        Static required_rt As String() = {"rt", "RT", "retention_time"}

        Dim ID As Integer = headers.GetSynonymOrdinal(required_id)
        Dim mz As Integer = headers.GetSynonymOrdinal(required_mz)
        Dim rt As Integer = headers.GetSynonymOrdinal(required_rt)
        Dim mzmin As Integer = headers("mzmin")
        Dim mzmax As Integer = headers("mzmax")
        Dim rtmin As Integer = headers("rtmin")
        Dim rtmax As Integer = headers("rtmax")
        Dim npeaks As Integer = headers.GetSynonymOrdinal("npeaks", ".")
        Dim ri As Integer = headers.GetSynonymOrdinal("ri", "RI", "retention_index")
        Dim maxinto As Integer = headers.GetSynonymOrdinal("maxinto", "into")
        Dim RImin As Integer = headers("RImin")
        Dim RImax As Integer = headers("RImax")

        If ID < 0 AndAlso headers.Objects(0) = "" Then
            ' row.names = 1
            ' first column is xcms_id
            ID = 0

            Call headers.Delete("")
            Call VBDebugger.EchoLine("use the first column as the xcms_id(row.names = 1).")
        End If

        Call headers.Delete(required_id)
        Call headers.Delete(required_mz)
        Call headers.Delete(required_rt)
        Call headers.Delete("mzmin", "mzmax")
        Call headers.Delete("rtmin", "rtmax")
        Call headers.Delete("npeaks", ".", "")
        Call headers.Delete("maxinto", "into")
        Call headers.Delete("ri", "RI")
        Call headers.Delete("RImin", "RImax")
        Call headers.Delete("groups")

        If ID < 0 Then
            Throw New InvalidDataException($"the required of the unique id in peaktable could not be found! You should check is there any fields named {required_id.Concatenate()} existed in your data table?")
        End If
        If mz < 0 Then
            Throw New InvalidDataException($"the required of the ion m/z field in peaktable could not be found! You should check is there any fields named {required_mz.Concatenate()} existed in your data table?")
        End If
        If rt < 0 Then
            Throw New InvalidDataException($"the required of the ion peak rt field in peaktable could not be found! You should check is there any fields named {required_rt.Concatenate()} existed in your data table?")
        End If

        Dim offsets = headers.ToArray
        Dim peaks As xcms2() = s _
            .GetPeaks(deli, ID,
                      mz, mzmin, mzmax,
                      rt, rtmin, rtmax,
                      ri, RImin, RImax,
                      npeaks:=npeaks, maxinto:=maxinto,
                      peaks:=offsets) _
            .ToArray

        If make_unique Then
            Dim unique_id = peaks.Select(Function(i) i.ID).UniqueNames

            For i As Integer = 0 To unique_id.Length - 1
                peaks(i).ID = unique_id(i)
            Next
        End If

        Call buf.Dispose()

        Return New PeakSet With {.peaks = peaks.ToArray}
    End Function

    <Extension>
    Private Iterator Function GetPeaks(s As StreamReader, deli As Char,
                                       ID As Integer,
                                       mz As Integer, mzmin As Integer, mzmax As Integer,
                                       rt As Integer, rtmin As Integer, rtmax As Integer,
                                       ri As Integer, rimin As Integer, rimax As Integer,
                                       npeaks As Integer, maxinto As Integer,
                                       peaks As SeqValue(Of String)()) As IEnumerable(Of xcms2)

        Dim str As Value(Of String) = ""
        Dim t As String()
        Dim pk As xcms2

        Do While (str = s.ReadLine) IsNot Nothing
            t = str.Split(deli)
            pk = New xcms2 With {
                .ID = t(ID).Trim(""""c, " "c),
                .mz = Double.Parse(t(mz)),
                .mzmax = If(mzmax > -1, Val(t(mzmax)), .mz),
                .mzmin = If(mzmin > -1, Val(t(mzmin)), .mz),
                .rt = Double.Parse(t(rt)),
                .rtmax = If(rtmax > -1, Val(t(rtmax)), .rt),
                .rtmin = If(rtmin > -1, Val(t(rtmin)), .rt),
                .RI = If(ri > -1, Val(t(ri)), 0),
                .RImax = If(rimax > -1, Val(t(rimax)), 0),
                .RImin = If(rimin > -1, Val(t(rimin)), 0)
            }

            If npeaks > -1 Then
                pk.SetPeaks(Val(t(npeaks)))
            End If
            If maxinto > -1 Then
                pk.into = Val(t(maxinto))
            End If

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
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function DumpSample(sample As PeakSet, file As Stream) As Boolean
        Call sample.peaks.DumpSample(sample.ROIs, sample.sampleNames, file)
        Return True
    End Function

    ReadOnly magic As Byte() = Encoding.ASCII.GetBytes("xcms/mzkit")

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sample"></param>
    ''' <param name="npeaks">number of ROI peaks in current table, not the number of sample files.</param>
    ''' <param name="sampleNames"></param>
    ''' <param name="file"></param>
    <Extension>
    Public Sub DumpSample(sample As IEnumerable(Of xcms2), npeaks As Integer, sampleNames As String(), file As Stream)
        Dim bin As New BinaryWriter(file)

        Call bin.Write(magic)
        Call bin.Write(npeaks)
        Call bin.Write(sampleNames.Length)

        For Each name As String In sampleNames
            Call bin.Write(name)
        Next

        For Each pk As xcms2 In sample
            Call bin.Write(pk.ID)
            Call bin.Write(pk.mz)
            Call bin.Write(pk.rt)
            Call bin.Write(pk.mzmin)
            Call bin.Write(pk.mzmax)
            Call bin.Write(pk.rtmin)
            Call bin.Write(pk.rtmax)
            Call bin.Write(pk.RI)
            Call bin.Write(pk.RImin)
            Call bin.Write(pk.RImax)
            Call bin.Write(pk.groups)

            For Each name As String In sampleNames
                Call bin.Write(pk(name))
            Next
        Next

        Call bin.Flush()
    End Sub

    ''' <summary>
    ''' read table data from rawdata file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Function ReadSamplePeaks(file As Stream) As xcms2()
        Dim rd As New BinaryReader(file)
        Dim header As Byte() = rd.ReadBytes(magic.Length)

        If Not header.SequenceEqual(magic) Then
            Throw New InvalidDataException($"invalid magic header: {Encoding.ASCII.GetString(header)}({header.Select(Function(b) b.ToString("x2")).JoinBy("-")})!")
        End If

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
                .rtmax = rd.ReadDouble,
                .RI = rd.ReadDouble,
                .RImin = rd.ReadDouble,
                .RImax = rd.ReadDouble,
                .groups = rd.ReadDouble
            }

            For offset As Integer = 0 To samples - 1
                pk(names(offset)) = rd.ReadDouble
            Next

            peaks(i) = pk
        Next

        Return peaks
    End Function

    ''' <summary>
    ''' load binary file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ReadSample(file As Stream) As PeakSet
        Return New PeakSet With {.peaks = ReadSamplePeaks(file)}
    End Function
End Module
