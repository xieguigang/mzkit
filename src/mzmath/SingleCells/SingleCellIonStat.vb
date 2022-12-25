
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly

Public Class SingleCellIonStat

    Public Property mz As Double
    Public Property cells As Integer
    Public Property maxIntensity As Double
    Public Property baseCell As String
    Public Property Q1Intensity As Double
    Public Property Q2Intensity As Double
    Public Property Q3Intensity As Double
    Public Property RSD As Double

    Public Shared Function DoIonStats(data As mzPack) As IEnumerable(Of SingleCellIonStat)

    End Function

End Class