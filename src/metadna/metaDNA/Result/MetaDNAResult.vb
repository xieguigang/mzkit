Imports BioNovoGene.BioDeep.MetaDNA.Infer

Public Class MetaDNAResult

    ''' <summary>
    ''' the unique id of ms1 parent ion
    ''' </summary>
    ''' <returns></returns>
    Public Property ROI_id As String
    ''' <summary>
    ''' the unique id of ms2 spectrum peaks
    ''' </summary>
    ''' <returns></returns>
    Public Property query_id As String
    Public Property mz As Double
    Public Property rt As Double
    Public Property intensity As Double
    Public Property KEGGId As String
    Public Property exactMass As Double
    Public Property formula As String
    Public Property name As String
    Public Property precursorType As String
    ''' <summary>
    ''' calculated m/z value based on <see cref="mz"/> and <see cref="precursorType"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property mzCalc As Double
    Public Property ppm As Double
    ''' <summary>
    ''' the score match of ms1 <see cref="rt"/> and rt value of the KEGG compound reference
    ''' </summary>
    ''' <returns></returns>
    Public Property rt_adjust As Double
    Public Property inferLevel As String
    Public Property forward As Double
    Public Property reverse As Double
    Public Property jaccard As Double
    Public Property parentTrace As Double
    Public Property inferSize As Integer
    Public Property score1 As Double
    Public Property score2 As Double
    Public Property pvalue As Double
    Public Property seed As String
    Public Property partnerKEGGId As String
    Public Property KEGG_reaction As String
    Public Property reaction As String
    Public Property fileName As String

End Class
