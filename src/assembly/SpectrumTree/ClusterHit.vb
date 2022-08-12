Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Public Class ClusterHit

    Public Property Id As String
    Public Property representive As SSM2MatrixFragment()
    Public Property forward As Double
    Public Property reverse As Double

    Public Property ClusterForward As Double()
    Public Property ClusterReverse As Double()
    Public Property ClusterId As String()

End Class
