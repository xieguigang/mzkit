#Region "Microsoft.VisualBasic::d237b80ba79bad90a00c62ac13613187, Library\mzkit\assembly\data.vb"

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

' Module data
' 
'     Function: getIntensity, getScantime, RtSlice, XIC, XICTable
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' m/z data operator module
''' </summary>
<Package("data")>
Module data

    <RInitialize>
    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(ms1_scan()), AddressOf XICTable)
    End Sub

    Private Function XICTable(XIC As ms1_scan(), args As list, env As Environment) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        table.columns("mz") = XIC.Select(Function(a) a.mz).ToArray
        table.columns("scan_time") = XIC.Select(Function(a) a.scan_time).ToArray
        table.columns("intensity") = XIC.Select(Function(a) a.intensity).ToArray

        Return table
    End Function

    ''' <summary>
    ''' get chromatogram data for a specific metabolite with given m/z from the ms1 scans data.
    ''' </summary>
    ''' <param name="ms1">a sequence data of ms1 scans</param>
    ''' <param name="mz">target mz value</param>
    ''' <param name="ppm">
    ''' tolerance value in unit ppm for extract mz data from the given ms1 ion scans.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("XIC")>
    <RApiReturn(GetType(ms1_scan))>
    Public Function XIC(<RRawVectorArgument> ms1 As Object, mz#,
                        Optional ppm# = 20,
                        Optional env As Environment = Nothing) As Object

        Dim ms1_scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1, env)

        If ms1_scans.isError Then
            Return ms1_scans.getError
        End If

        Dim xicFilter As ms1_scan() = ms1_scans _
            .populates(Of ms1_scan)(env) _
            .Where(Function(pt) PPMmethod.PPM(pt.mz, mz) <= ppm) _
            .OrderBy(Function(a) a.scan_time) _
            .ToArray

        Return xicFilter
    End Function

    ''' <summary>
    ''' slice a region of ms1 scan data by a given rt window.
    ''' </summary>
    ''' <param name="ms1">a sequence of ms1 scan data.</param>
    ''' <param name="rtmin"></param>
    ''' <param name="rtmax"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("rt_slice")>
    <RApiReturn(GetType(ms1_scan))>
    Public Function RtSlice(<RRawVectorArgument>
                            ms1 As Object,
                            rtmin#, rtmax#,
                            Optional env As Environment = Nothing) As Object

        Dim ms1_scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1, env)

        If ms1_scans.isError Then
            Return ms1_scans.getError
        End If

        Dim xicFilter As ms1_scan() = ms1_scans _
            .populates(Of ms1_scan)(env) _
            .Where(Function(pt) pt.scan_time >= rtmin AndAlso pt.scan_time <= rtmax) _
            .OrderBy(Function(a) a.scan_time) _
            .ToArray

        Return xicFilter
    End Function

    ''' <summary>
    ''' get intensity value from the ion scan points
    ''' </summary>
    ''' <param name="ticks"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("intensity")>
    <RApiReturn(GetType(Double))>
    Public Function getIntensity(<RRawVectorArgument>
                                 ticks As Object,
                                 Optional env As Environment = Nothing) As Object

        Dim scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ticks, env)

        If scans.isError Then
            Return scans.getError
        End If

        Return scans.populates(Of ms1_scan)(env) _
            .Select(Function(x) x.intensity) _
            .DoCall(AddressOf vector.asVector)
    End Function

    ''' <summary>
    ''' get scan time value from the ion scan points
    ''' </summary>
    ''' <param name="ticks"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("scan_time")>
    <RApiReturn(GetType(Double))>
    Public Function getScantime(<RRawVectorArgument>
                                ticks As Object,
                                Optional env As Environment = Nothing) As Object

        Dim scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ticks, env)

        If scans.isError Then
            Return scans.getError
        End If

        Return scans.populates(Of ms1_scan)(env) _
            .Select(Function(x) x.scan_time) _
            .DoCall(AddressOf vector.asVector)
    End Function

    <ExportAPI("make.ROI_names")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function makeROInames(<RRawVectorArgument> ROIlist As Object, Optional env As Environment = Nothing) As Object
        Dim dataList As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ROIlist, env)

        If dataList.isError Then
            Return dataList.getError
        End If

        Dim allData As PeakMs2() = dataList.populates(Of PeakMs2)(env).ToArray
        Dim allId As String() = allData.Select(Function(p) $"M{p.mz}T{p.rt}").ToArray
        Dim uniques As String() = base.makeNames(allId, unique:=True, allow_:=True)

        For i As Integer = 0 To allData.Length - 1
            allData(i).lib_guid = uniques(i)
        Next

        Return allData
    End Function
End Module
