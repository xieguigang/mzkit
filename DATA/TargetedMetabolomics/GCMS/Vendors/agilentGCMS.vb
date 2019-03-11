#Region "Microsoft.VisualBasic::7767068ded6ad69cdd3d982ca2b915a1, TargetedMetabolomics\GCMS\Vendors\agilentGCMS.vb"

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

    '     Module agilentGCMS
    ' 
    '         Function: Read
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.netCDF
Imports Microsoft.VisualBasic.MIME.application.netCDF.Components

Namespace GCMS.Vendors

    Public Module agilentGCMS

        Public Function Read(cdf As netCDFReader) As Raw
            Dim time As CDFData = cdf.getDataVariable("scan_acquisition_time")
            Dim tic = cdf.getDataVariable("total_intensity")
            Dim pointCount = cdf.getDataVariable("point_count")
            Dim massValues = cdf.getDataVariable("mass_values").tiny_num
            Dim intensityValues = cdf.getDataVariable("intensity_values").tiny_num
            Dim scan_times = cdf.getDataVariable("time_values").tiny_num

            Dim ms As ms1_scan()() = New ms1_scan(pointCount.Length - 1)() {}
            Dim index As VBInteger = Scan0
            Dim size%

            For i As Integer = 0 To ms.Length - 1
                size = pointCount.integers(i)
                ms(i) = New ms1_scan(size - 1) {}

                For j As Integer = 0 To size - 1
                    ms(i)(j) = New ms1_scan With {
                        .mz = massValues(index),
                        .intensity = intensityValues(index),
                        .scan_time = scan_times(++index)
                    }
                Next
            Next

            Return New Raw With {
                .times = time.numerics,
                .tic = tic.numerics,
                .ms = ms,
                .title = cdf.getAttribute("experiment_title")
            }
        End Function
    End Module
End Namespace
