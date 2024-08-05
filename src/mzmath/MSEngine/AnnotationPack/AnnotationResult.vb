
''' <summary>
''' the annotation result of a specific peak
''' </summary>
Public Class AlignmentHit

    Public Property xcms_id As String
    Public Property libname As String
    Public Property mz As Double
    Public Property rt As Double
    Public Property RI As Double
    Public Property theoretical_mz As Double
    Public Property exact_mass As Double
    Public Property adducts As String
    Public Property ppm As Double
    Public Property occurrences As Integer
    Public Property biodeep_id As String
    Public Property name As String
    Public Property formula As String
    Public Property npeaks As Integer

    ''' <summary>
    ''' sample hits of current library reference
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' null or empty means annotation in ms1 level
    ''' </remarks>
    Public Property samplefiles As Dictionary(Of String, Ms2Score)

End Class
