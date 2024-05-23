#Region "Microsoft.VisualBasic::9ee086c8b77adc1ebfd88b89e0bd2e1f, mzmath\TargetedMetabolomics\GCMS\CDFReader\Vendors\agilentGCMS.vb"

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

    '   Total Lines: 65
    '    Code Lines: 56 (86.15%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (13.85%)
    '     File Size: 2.75 KB


    '     Module agilentGCMS
    ' 
    '         Function: Read, readValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Language
Imports any = Microsoft.VisualBasic.Scripting

Namespace GCMS.Vendors

    Public Module agilentGCMS

        Public Function Read(cdf As netCDFReader) As Raw
            Dim time As doubles = cdf.getDataVariable("scan_acquisition_time")
            Dim tic As doubles = cdf.getDataVariable("total_intensity")
            Dim pointCount As integers = cdf.getDataVariable("point_count")
            Dim massValues As ICDFDataVector = cdf.getDataVariable("mass_values")
            Dim intensityValues As ICDFDataVector = cdf.getDataVariable("intensity_values")
            Dim scan_times As ICDFDataVector = cdf.getDataVariable("time_values")
            Dim attrs As New Dictionary(Of String, String)

            For Each attr As attribute In cdf.globalAttributes
                attrs(attr.name) = attr.value
            Next

            Dim ms As ms1_scan()() = New ms1_scan(pointCount.Length - 1)() {}
            Dim index As i32 = Scan0
            Dim size%
            Dim massReader As Func(Of Integer, Double) = readValue(massValues)
            Dim intoReader As Func(Of Integer, Double) = readValue(intensityValues)
            Dim timeReader As Func(Of Integer, Double) = readValue(scan_times)

            For i As Integer = 0 To ms.Length - 1
                size = pointCount(i)
                ms(i) = New ms1_scan(size - 1) {}

                For j As Integer = 0 To size - 1
                    ms(i)(j) = New ms1_scan With {
                        .mz = massReader(index),
                        .intensity = intoReader(index),
                        .scan_time = timeReader(++index)
                    }
                Next
            Next

            Return New Raw With {
                .times = time.Array,
                .tic = tic.Array,
                .ms = ms,
                .title = any.ToString(cdf.getAttribute("experiment_title")),
                .attributes = attrs,
                .mz = ms.MzList
            }
        End Function

        Private Function readValue(data As ICDFDataVector) As Func(Of Integer, Double)
            If TypeOf data Is floats Then
                Return Function(i) DirectCast(data, floats)(i)
            ElseIf TypeOf data Is shorts Then
                Return Function(i) DirectCast(data, shorts)(i)
            Else
                Throw New NotImplementedException
            End If
        End Function
    End Module
End Namespace
