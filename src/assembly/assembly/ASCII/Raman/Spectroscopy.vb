Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ASCII.Raman

    Public Class Spectroscopy

        <Field("TITLE")> Public Property Title As String
        <Field("DATA TYPE")> Public Property DataType As String
        <Field("ORIGIN")> Public Property Origin As String
        <Field("OWNER")> Public Property Owner As String
        <Field("DATE")> Public Property [Date] As String
        <Field("TIME")> Public Property Time As String
        <Field("SPECTROMETER/DATA SYSTEM")> Public Property Spectrometer As String
        <Field("LOCALE")> Public Property Locale As String
        <Field("RESOLUTION")> Public Property Resolution As String
        <Field("DELTAX")> Public Property Reltax As String
        <Field("XUNITS")> Public Property Xunits As String
        <Field("YUNITS")> Public Property Yunits As String
        <Field("FIRSTX")> Public Property FirstX As String
        <Field("LASTX")> Public Property LastX As String
        <Field("NPOINTS")> Public Property nPoints As Integer
        <Field("FIRSTY")> Public Property FirstY As String
        <Field("MAXY")> Public Property MaxY As String
        <Field("MINY")> Public Property MinY As String
        Public Property xyData As PointF()
        Public Property Comments As Dictionary(Of String, String)
        Public Property DetailedInformation As Dictionary(Of String, String)
        Public Property MeasurementInformation As Dictionary(Of String, String)

    End Class
End Namespace