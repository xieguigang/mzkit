Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models

Namespace MRM.Data

    Public Structure IonChromatogram

        Public Property name As String
        Public Property description As String
        Public Property chromatogram As ChromatogramTick()
        Public Property ion As IsomerismIonPairs

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Structure
End Namespace