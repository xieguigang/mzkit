#Region "Microsoft.VisualBasic::1b69f6c1a8028c87c530f0eadc3110bf, mzmath\mz_deco\Save\SaveRtShifts.vb"

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

    '   Total Lines: 55
    '    Code Lines: 46 (83.64%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (16.36%)
    '     File Size: 1.92 KB


    ' Module SaveRtShifts
    ' 
    '     Function: ParseRtShifts
    ' 
    '     Sub: DumpShiftsData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module SaveRtShifts

    <Extension>
    Public Sub DumpShiftsData(rt_shifts As IEnumerable(Of RtShift), file As Stream)
        Dim pool As RtShift() = rt_shifts.ToArray
        Dim all_samples As String() = pool.Select(Function(o) o.sample).Distinct.ToArray
        Dim buf As Byte() = Encoding.UTF8.GetBytes(all_samples.GetJson)
        Dim bin As New BinaryWriter(file)

        bin.Write(pool.Length)
        bin.Write(all_samples.Length)
        bin.Write(buf.Length)
        bin.Write(buf)

        Dim sample_index As Index(Of String) = all_samples.Indexing

        For Each tick As RtShift In pool
            bin.Write(tick.refer_rt)
            bin.Write(tick.sample_rt)
            bin.Write(tick.RI)
            bin.Write(sample_index(tick.sample))
            bin.Write(tick.xcms_id)
        Next

        Call bin.Flush()
    End Sub

    Public Iterator Function ParseRtShifts(file As Stream) As IEnumerable(Of RtShift)
        Dim bin As New BinaryReader(file)
        Dim size As Integer = bin.ReadInt32
        Dim nsamples As Integer = bin.ReadInt32
        Dim json_bytes As Integer = bin.ReadInt32
        Dim buf As Byte() = New Byte(json_bytes - 1) {}
        Dim sample_names As String()

        bin.Read(buf, 0, json_bytes)
        sample_names = Encoding.UTF8.GetString(buf).LoadJSON(Of String())

        For i As Integer = 1 To size
            Yield New RtShift With {
                .refer_rt = bin.ReadDouble,
                .sample_rt = bin.ReadDouble,
                .RI = bin.ReadDouble,
                .sample = sample_names(bin.ReadInt32),
                .xcms_id = bin.ReadString
            }
        Next
    End Function
End Module
