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