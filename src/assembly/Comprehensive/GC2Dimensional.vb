#Region "Microsoft.VisualBasic::e4e1f9b9a5fdf6b9c7b6a622d4f741c4, src\assembly\Comprehensive\GC2Dimensional.vb"

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

    ' Module GC2Dimensional
    ' 
    '     Function: CreateMSScans, readIntoMatrix, readMzMatrix, ToMzPack
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

''' <summary>
''' GCxGC assembly data
''' </summary>
Public Module GC2Dimensional

    <Extension>
    Public Function ToMzPack(agilentGC As netCDFReader) As mzPack
        Dim scan_time As doubles = agilentGC.getDataVariable("scan_acquisition_time")
        Dim totalIons As doubles = agilentGC.getDataVariable("total_intensity")
        Dim point_count As integers = agilentGC.getDataVariable("point_count")
        Dim mz As Double()() = agilentGC.readMzMatrix(point_count).ToArray
        Dim into As Double()() = agilentGC.readIntoMatrix(point_count).ToArray

        Return New mzPack With {
            .MS = scan_time.Array.CreateMSScans(totalIons, mz, into).ToArray
        }
    End Function

    <Extension>
    Private Iterator Function readMzMatrix(agilentGC As netCDFReader, point_count As integers) As IEnumerable(Of Double())
        Dim offset As Integer = Scan0
        Dim mz As shorts = Nothing

        Call Console.WriteLine("read m/z matrix...")
        Call agilentGC.getDataVariable("mass_values", mz)

        For Each width As Integer In point_count
            Yield mz _
                .Copy(offset, width) _
                .Select(Function(i) CDbl(i)) _
                .ToArray

            offset += width
        Next
    End Function

    <Extension>
    Private Iterator Function readIntoMatrix(agilentGC As netCDFReader, point_count As integers) As IEnumerable(Of Double())
        Dim into As integers
        Dim offset As Integer = Scan0

        Call Console.WriteLine("read intensity matrix...")

        into = agilentGC.getDataVariable("intensity_values")

        For Each width As Integer In point_count
            Yield into _
                .Copy(offset, width) _
                .Select(Function(i) CDbl(i)) _
                .ToArray

            offset += width
        Next
    End Function

    <Extension>
    Private Iterator Function CreateMSScans(scan_time As Double(), totalIons As Double(), mz As Double()(), into As Double()()) As IEnumerable(Of ScanMS1)
        For i As Integer = 0 To scan_time.Length - 1
            Dim mzi As Double() = mz(i)
            Dim inti As Double() = into(i)
            Dim BPC As Double = inti.Max
            ' 20210328
            ' fix bugs fix mzkit_win32: required [MS1] prefix for indicate MS1
            Dim scan_id As String = $"[MS1] {i + 1}.scan_time={stdNum.Round(scan_time(i))}, m/z={mzi(which.Max(inti))}({BPC.ToString("G3")})"

            Yield New ScanMS1 With {
                .TIC = totalIons(i),
                .BPC = BPC,
                .rt = scan_time(i),
                .mz = mzi,
                .into = inti,
                .scan_id = scan_id
            }
        Next
    End Function
End Module
