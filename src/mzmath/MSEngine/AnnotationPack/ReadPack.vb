#Region "Microsoft.VisualBasic::0cd1b66ab6261846351d6c9f38ad55ea, mzmath\MSEngine\AnnotationPack\ReadPack.vb"

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

    '   Total Lines: 76
    '    Code Lines: 66 (86.84%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (13.16%)
    '     File Size: 2.62 KB


    ' Module ReadPack
    ' 
    '     Function: ReadMs2Alignment, ReadMs2Annotation, ReadScore
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ApplicationServices

Public Module ReadPack

    Public Function ReadMs2Annotation(file As Stream) As AlignmentHit
        Dim bin As New BinaryReader(file)
        Dim result As New AlignmentHit With {
            .xcms_id = bin.ReadString,
            .libname = bin.ReadString,
            .mz = bin.ReadDouble,
            .rt = bin.ReadDouble,
            .RI = bin.ReadDouble,
            .theoretical_mz = bin.ReadDouble,
            .exact_mass = bin.ReadDouble,
            .adducts = bin.ReadString,
            .ppm = bin.ReadDouble,
            .occurrences = bin.ReadInt32,
            .biodeep_id = bin.ReadString,
            .name = bin.ReadString,
            .formula = bin.ReadString,
            .npeaks = bin.ReadInt32,
            .samplefiles = New Dictionary(Of String, Ms2Score),
            .pvalue = bin.ReadDouble
        }
        Dim n As Integer = bin.ReadInt32

        For i As Integer = 0 To n - 1
            result(bin.ReadString) = ReadScore(bin)
        Next

        Return result
    End Function

    Private Function ReadScore(bin As BinaryReader) As Ms2Score
        Dim size As Long = bin.ReadInt64
        Dim s As Stream = bin.BaseStream
        Dim buf As Stream = New SubStream(s, s.Position, size)
        Dim result As Ms2Score = ReadMs2Alignment(file:=buf)
        Return result
    End Function

    Public Function ReadMs2Alignment(file As Stream) As Ms2Score
        Dim bin As New BinaryReader(file)
        Dim details As New Ms2Score With {
            .precursor = bin.ReadDouble,
            .rt = bin.ReadDouble,
            .intensity = bin.ReadDouble,
            .score = bin.ReadDouble,
            .forward = bin.ReadDouble,
            .reverse = bin.ReadDouble,
            .jaccard = bin.ReadDouble,
            .entropy = bin.ReadDouble,
            .libname = bin.ReadString,
            .source = bin.ReadString
        }
        Dim n As Integer = bin.ReadInt32
        Dim peaks As SSM2MatrixFragment() = New SSM2MatrixFragment(n - 1) {}

        For i As Integer = 0 To n - 1
            peaks(i) = New SSM2MatrixFragment With {
                .mz = bin.ReadDouble,
                .query = bin.ReadDouble,
                .ref = bin.ReadDouble,
                .da = bin.ReadString
            }
        Next

        details.ms2 = peaks

        Return details
    End Function

End Module

