
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
    Public Property pvalue As Double

    ''' <summary>
    ''' sample hits of current library reference
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' null or empty means annotation in ms1 level
    ''' </remarks>
    Public Property samplefiles As Dictionary(Of String, Ms2Score)

    Default Public Property SampleAlignment(sampleName As String) As Ms2Score
        Get
            Return samplefiles.TryGetValue(sampleName)
        End Get
        Set
            samplefiles(sampleName) = Value
        End Set
    End Property

    Sub New()
    End Sub

    Sub New(copy As AlignmentHit)
        xcms_id = copy.xcms_id
        libname = copy.libname
        mz = copy.mz
        rt = copy.rt
        RI = copy.RI
        theoretical_mz = copy.theoretical_mz
        exact_mass = copy.exact_mass
        adducts = copy.adducts
        ppm = copy.ppm
        occurrences = copy.occurrences
        biodeep_id = copy.biodeep_id
        name = copy.name
        formula = copy.formula
        npeaks = copy.npeaks
        pvalue = copy.pvalue
        samplefiles = New Dictionary(Of String, Ms2Score)(copy.samplefiles)
    End Sub

    Public Overrides Function ToString() As String
        Return name & "_" & adducts
    End Function

End Class
