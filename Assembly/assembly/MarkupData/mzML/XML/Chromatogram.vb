Imports System.Xml.Serialization

Namespace MarkupData.mzML

    Public Class chromatogramList : Inherits DataList
        <XmlElement(NameOf(chromatogram))>
        Public Property list As chromatogram()
    End Class

    Public Class chromatogram : Inherits BinaryData

        Public Property precursor As precursor
        Public Property product As product

    End Class

    Public Interface IMRMSelector
        Property isolationWindow As Params
    End Interface
End Namespace