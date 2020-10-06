Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Public Class AlignmentProperty

    Public Property query As String
    Public Property reference As String
    Public Property forward As Double
    Public Property reverse As Double

    Sub New(alignment As AlignmentOutput)
        query = alignment.query.id
        reference = alignment.reference.id
        forward = alignment.forward
        reverse = alignment.reverse
    End Sub
End Class
