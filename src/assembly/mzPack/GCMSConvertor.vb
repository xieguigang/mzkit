#Region "Microsoft.VisualBasic::939cf287b770cf913a2f692d6d9ad2c7, mzkit\src\assembly\mzPack\GCMSConvertor.vb"

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

'   Total Lines: 125
'    Code Lines: 101
' Comment Lines: 2
'   Blank Lines: 22
'     File Size: 5.06 KB


' Module GCMSConvertor
' 
'     Function: ConvertGCMS, CreateMSScans, LoadMs1Scans, readIntoMatrix, readMzMatrix
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' the netCDF in-memory data model of GCMS data 
''' </summary>
Public Class GCMSnetCDF

    Public Property error_log As String
    Public Property a_d_sampling_rate As Double()
    Public Property a_d_coaddition_factor As Short()
    Public Property scan_acquisition_time As Double()
    Public Property scan_duration As Double()
    Public Property inter_scan_time As Double()
    Public Property resolution As Double()
    Public Property actual_scan_number As Integer()
    Public Property total_intensity As Double()
    Public Property mass_range_min As Double()
    Public Property mass_range_max As Double()
    Public Property time_range_min As Double()
    Public Property time_range_max As Double()
    Public Property scan_index As Integer()
    Public Property point_count As Integer()
    Public Property flag_count As Integer()
    Public Property mass_values As Short()
    Public Property intensity_values As Single()
    Public Property metadata As New Dictionary(Of String, String)

End Class

Public Module GCMSConvertor

    Public Function ConvertGCMS(agilentGC As netCDFReader, Optional println As Action(Of String) = Nothing) As mzPack
        Dim metadata As New Dictionary(Of String, String)

        If println Is Nothing Then
            println = AddressOf VBDebugger.EchoLine
        End If

        Call println("load metadata attributes of current GC-MS file:")
        Call println("")

        For Each attr As attribute In agilentGC.globalAttributes
            println(attr.ToString)
            metadata(attr.name) = attr.value
        Next

        Call println("")
        Call println("get TIC data...")

        Dim scan_time As doubles = agilentGC.getDataVariable("scan_acquisition_time")
        Dim totalIons As doubles = agilentGC.getDataVariable("total_intensity")

        Return New mzPack With {
            .Application = FileApplicationClass.GCMS,
            .MS = LoadMs1Scans(agilentGC, println).ToArray,
            .Chromatogram = New Chromatogram With {
                .scan_time = scan_time,
                .BPC = totalIons,
                .TIC = totalIons
            },
            .metadata = metadata
        }
    End Function

    Public Function LoadMs1Scans(agilentGC As netCDFReader, println As Action(Of String)) As IEnumerable(Of ScanMS1)
        Dim scan_time As doubles = agilentGC.getDataVariable("scan_acquisition_time")
        Dim totalIons As doubles = agilentGC.getDataVariable("total_intensity")
        Dim point_count As integers = agilentGC.getDataVariable("point_count")
        Dim mz As Double()() = agilentGC.readMzMatrix(point_count, println).ToArray
        Dim into As Double()() = agilentGC.readIntoMatrix(point_count, println).ToArray

        Call println("read scan matrix!")

        Return scan_time.Array _
            .CreateMSScans(totalIons, mz, into) _
            .OrderBy(Function(t) t.rt)
    End Function

    <Extension>
    Private Iterator Function CreateMSScans(scan_time As Double(),
                                            totalIons As Double(),
                                            mz As Double()(),
                                            into As Double()()) As IEnumerable(Of ScanMS1)

        For i As Integer = 0 To scan_time.Length - 1
            Dim mzi As Double() = mz(i)
            Dim inti As Double() = into(i)
            Dim BPC As Double = inti.Max
            ' 20210328
            ' fix bugs fix mzkit_win32: required [MS1] prefix for indicate MS1
            Dim scan_id As String = $"[MS1] {i + 1}.scan_time={std.Round(scan_time(i))}, m/z={mzi(which.Max(inti))}({BPC.ToString("G3")})"

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

    Const intensity_values As String = "intensity_values"

    ''' <summary>
    ''' read "intensity_values"
    ''' </summary>
    ''' <param name="agilentGC"></param>
    ''' <param name="point_count"></param>
    ''' <param name="println"></param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function readIntoMatrix(agilentGC As netCDFReader, point_count As integers, println As Action(Of String)) As IEnumerable(Of Double())
        Dim into As ICDFDataVector = Nothing
        Dim offset As Integer = Scan0
        Dim type As CDFDataTypes = agilentGC.getDataVariableEntry(intensity_values).type

        Call println("read intensity matrix, may takes a long time to run...")
        Call agilentGC.getDataVariable("intensity_values", into)

        If type = CDFDataTypes.NC_INT Then
            Dim ints As integers = DirectCast(into, integers)

            For Each width As Integer In point_count
                Yield ints _
                    .Copy(offset, width) _
                    .Select(Function(i) CDbl(i)) _
                    .ToArray

                offset += width
            Next
        ElseIf type = CDFDataTypes.NC_FLOAT Then
            Dim singles As floats = DirectCast(into, floats)

            For Each width As Integer In point_count
                Yield singles _
                    .Copy(offset, width) _
                    .Select(Function(i) CDbl(i)) _
                    .ToArray

                offset += width
            Next
        Else
            Throw New NotImplementedException(type.Description)
        End If
    End Function

    ''' <summary>
    ''' read ``mass_values``
    ''' </summary>
    ''' <param name="agilentGC"></param>
    ''' <param name="point_count"></param>
    ''' <param name="println"></param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function readMzMatrix(agilentGC As netCDFReader, point_count As integers, println As Action(Of String)) As IEnumerable(Of Double())
        Dim offset As Integer = Scan0
        Dim mz As shorts = Nothing

        Call println("read m/z matrix, may takes a long time to run...")
        Call agilentGC.getDataVariable("mass_values", mz)

        For Each width As Integer In point_count
            Yield mz _
                .Copy(offset, width) _
                .Select(Function(i) CDbl(i)) _
                .ToArray

            offset += width
        Next
    End Function
End Module
