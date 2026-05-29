#Region "Microsoft.VisualBasic::60a55bde7d54e3a4946f408bca5cc4fa, mzkit\services\MZWork\GCMSReader.vb"

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

'   Total Lines: 53
'    Code Lines: 50 (94.34%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 3 (5.66%)
'     File Size: 2.73 KB


' Module GCMSReader
' 
'     Function: LoadAllMemory
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Module GCMSReader

    Public Function LoadAllMemory(file As String) As GCMSnetCDF
        Dim nc As New CDF.PInvoke.DataReader(file)
        Dim error_log As String = DirectCast(nc.GetData("error_log"), chars).ToArray.CharString
        Dim a_d_sampling_rate As doubles = nc.GetData("a_d_sampling_rate")
        Dim a_d_coaddition_factor As shorts = nc.GetData("a_d_coaddition_factor")
        Dim scan_acquisition_time As doubles = nc.GetData("scan_acquisition_time")
        Dim scan_duration As doubles = nc.GetData("scan_duration")
        Dim inter_scan_time As doubles = nc.GetData("inter_scan_time")
        Dim resolution As doubles = nc.GetData("resolution")
        Dim actual_scan_number As integers = nc.GetData("actual_scan_number")
        Dim total_intensity As doubles = nc.GetData("total_intensity")
        Dim mass_range_min As doubles = nc.GetData("mass_range_min")
        Dim mass_range_max As doubles = nc.GetData("mass_range_max")
        Dim time_range_min As doubles = nc.GetData("time_range_min")
        Dim time_range_max As doubles = nc.GetData("time_range_max")
        Dim scan_index As integers = nc.GetData("scan_index")
        Dim point_count As integers = nc.GetData("point_count")
        Dim flag_count As integers = nc.GetData("flag_count")
        Dim mass_values As ICDFDataVector = nc.GetData("mass_values")
        Dim intensity_values As floats = nc.GetData("intensity_values")
        Dim mz As Single() = DirectCast(CObj(mass_values), ICTypeVector).ToFloat

        Return New GCMSnetCDF With {
            .actual_scan_number = actual_scan_number,
            .a_d_coaddition_factor = a_d_coaddition_factor,
            .a_d_sampling_rate = a_d_sampling_rate,
            .error_log = error_log,
            .flag_count = flag_count,
            .intensity_values = intensity_values,
            .inter_scan_time = inter_scan_time,
            .mass_range_max = mass_range_max,
            .mass_range_min = mass_range_min,
            .mass_values = mz,
            .point_count = point_count,
            .resolution = resolution,
            .scan_acquisition_time = scan_acquisition_time,
            .scan_duration = scan_duration,
            .scan_index = scan_index,
            .time_range_max = time_range_max,
            .time_range_min = time_range_min,
            .total_intensity = total_intensity,
            .metadata = nc.attributes
        }
    End Function
End Module
