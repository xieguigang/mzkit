Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Public Class AlignmentProperty

    <Category("Ion Information")>
    Public Property query As String
    <Category("Ion Information")>
    Public Property reference As String

    <Category("Alignment")>
    Public Property forward As Double
    <Category("Alignment")>
    Public Property reverse As Double
    <Category("Alignment")>
    Public Property jaccard As Double
    <Category("Alignment")>
    Public Property shares As Integer

    Sub New(alignment As AlignmentOutput)
        query = alignment.query.id
        reference = alignment.reference.id
        forward = alignment.forward
        reverse = alignment.reverse

        Dim all = alignment.alignments.Length

        shares = alignment.alignments.Where(Function(a) a.da <> "NaN").Count
        jaccard = shares / all
    End Sub
End Class
