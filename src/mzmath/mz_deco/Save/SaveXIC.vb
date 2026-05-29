#Region "Microsoft.VisualBasic::0a1dfd89cd48c24217cce2048346b1a3, mzmath\mz_deco\Save\SaveXIC.vb"

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

'   Total Lines: 48
'    Code Lines: 37 (77.08%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 11 (22.92%)
'     File Size: 1.45 KB


' Module SaveXIC
' 
'     Function: ReadSample
' 
'     Sub: DumpSample
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Public Module SaveXIC

    ''' <summary>
    ''' Save multiple ROI XIC data into a binary file
    ''' </summary>
    ''' <param name="sample"></param>
    ''' <param name="file"></param>
    <Extension>
    Public Sub DumpSample(sample As IEnumerable(Of MzGroup), file As Stream)
        Dim bin As New BinaryWriter(file)
        Dim n As Integer = 0

        For Each ion As MzGroup In sample
            Call bin.Write(If(ion.tag, ""))
            Call bin.Write(ion.mz)
            Call bin.Write(ion.size)

            If ion.RI.IsNullOrEmpty Then
                Call bin.Write(CByte(0))
            Else
                Call bin.Write(CByte(1))

                For Each ri As Double In ion.RI
                    Call bin.Write(ri)
                Next
            End If

            For Each tick As ChromatogramTick In ion.XIC
                Call bin.Write(tick.Time)
                Call bin.Write(tick.Intensity)
            Next

            n += 1
        Next

        Call bin.Write(n)
        Call bin.Flush()
    End Sub

    ''' <summary>
    ''' Read multiple ROI XIC data from a binary data file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Iterator Function ReadSample(file As Stream) As IEnumerable(Of MzGroup)
        Dim bin As New BinaryReader(file)
        Dim n As Integer

        file.Seek(file.Length - 4, SeekOrigin.Begin)
        n = bin.ReadInt32
        file.Seek(0, SeekOrigin.Begin)

        For i As Integer = 1 To n
            Dim name As String = bin.ReadString
            Dim mz As Double = bin.ReadDouble
            Dim size As Integer = bin.ReadInt32
            Dim ticks As ChromatogramTick() = New ChromatogramTick(size - 1) {}
            Dim ri As New List(Of Double)

            If bin.ReadByte = 1 Then
                For offset As Integer = 0 To size - 1
                    Call ri.Add(bin.ReadDouble)
                Next
            End If

            For offset As Integer = 0 To size - 1
                ticks(offset) = New ChromatogramTick(bin.ReadDouble, bin.ReadDouble)
            Next

            Yield New MzGroup With {
                .mz = mz,
                .XIC = ticks,
                .RI = ri.ToArray,
                .tag = name
            }
        Next
    End Function
End Module
