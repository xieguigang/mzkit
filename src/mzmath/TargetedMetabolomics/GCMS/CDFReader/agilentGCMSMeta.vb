#Region "Microsoft.VisualBasic::2d24c07a115535c5f96d1f9809877d3b, src\mzmath\TargetedMetabolomics\GCMS\CDFReader\agilentGCMSMeta.vb"

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

    '     Class agilentGCMSMeta
    ' 
    '         Properties: dataset_completeness, dataset_origin, experiment_date_time_stamp, experiment_title, experiment_type
    '                     external_file_ref_0, languages, ms_template_revision, netcdf_file_date_time_stamp, netcdf_revision
    '                     number_of_times_calibrated, number_of_times_processed, operator_name, raw_data_intensity_format, raw_data_mass_format
    '                     raw_data_time_format, sample_state, test_detector_type, test_ionization_mode, test_ionization_polarity
    '                     test_ms_inlet, test_resolution_type, test_scan_direction, test_scan_function, test_scan_law
    '                     test_separation_type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace GCMS

    Public Class agilentGCMSMeta

        Public Property dataset_completeness As String
        Public Property ms_template_revision As String
        Public Property netcdf_revision As String
        Public Property languages As String
        Public Property dataset_origin As String
        Public Property netcdf_file_date_time_stamp As String
        Public Property experiment_title As String
        Public Property experiment_date_time_stamp As String
        Public Property operator_name As String
        Public Property external_file_ref_0 As String
        Public Property experiment_type As String
        Public Property number_of_times_processed As String
        Public Property number_of_times_calibrated As String
        Public Property sample_state As String
        Public Property test_separation_type As String
        Public Property test_ms_inlet As String
        Public Property test_ionization_mode As String
        Public Property test_ionization_polarity As String
        Public Property test_detector_type As String
        Public Property test_resolution_type As String
        Public Property test_scan_function As String
        Public Property test_scan_direction As String
        Public Property test_scan_law As String
        Public Property raw_data_mass_format As String
        Public Property raw_data_time_format As String
        Public Property raw_data_intensity_format As String

        Sub New(attrs As Dictionary(Of String, String))
            Static readers As Dictionary(Of String, PropertyInfo) = DataFramework.Schema(Of agilentGCMSMeta)(
                flag:=PropertyAccess.Writeable,
                nonIndex:=True,
                primitive:=True
            )

            For Each proper As PropertyInfo In readers.Values
                proper.SetValue(Me, attrs.TryGetValue(proper.Name))
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
