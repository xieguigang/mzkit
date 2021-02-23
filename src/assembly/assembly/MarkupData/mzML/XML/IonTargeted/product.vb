Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.mzML.IonTargeted

    Public Class product : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params

    End Class
End Namespace