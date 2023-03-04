Imports System.Drawing

Namespace ASCII.Raman

    Public Class Spectroscopy

        Public Property Title As String
        Public Property DataType As String
        Public Property Origin As String
        Public Property Owner As String
        Public Property [Date] As String
        Public Property Time As String
        Public Property Spectrometer As String
        Public Property Locale As String
        Public Property Resolution As String
        Public Property Reltax As String
        Public Property Xunits As String
        Public Property Yunits As String
        Public Property FirstX As String
        Public Property LastX As String
        Public Property nPoints As Integer
        Public Property FirstY As String
        Public Property MaxY As String
        Public Property MinY As String
        Public Property xyData As PointF()
        Public Property Comments As Dictionary(Of String, String)
        Public Property DetailedInformation As Dictionary(Of String, String)
        Public Property MeasurementInformation As Dictionary(Of String, String)

    End Class
End Namespace