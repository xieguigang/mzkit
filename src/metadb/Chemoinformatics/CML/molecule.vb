Imports System.Xml.Serialization

Namespace ChemicalMarkupLanguage

    Public Class bond

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property atomRefs2 As String()
        <XmlAttribute> Public Property order As Integer

    End Class

    Public Class atom

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property elementType As String
        <XmlAttribute> Public Property hydrogenCount As Integer
        <XmlAttribute> Public Property x2 As Double
        <XmlAttribute> Public Property y2 As Double

    End Class

    Public Class molecule

        <XmlAttribute>
        Public Property id As String

        Public Property atomArray As atomArray
        Public Property bondArray As bondArray

    End Class

    Public Class ArrayList

        <XmlAttribute> Public Property id As String

    End Class

    Public Class atomArray : Inherits ArrayList

        <XmlElement("atom")> Public Property atoms As atom()

        Sub New()
        End Sub

        Sub New(id As String, atoms As IEnumerable(Of atom))
            Me.id = id
            Me.atoms = atoms.ToArray
        End Sub

    End Class

    Public Class bondArray : Inherits ArrayList

        <XmlElement("bond")> Public Property bonds As bond()

        Sub New()
        End Sub

        Sub New(id As String, bonds As IEnumerable(Of bond))
            Me.id = id
            Me.bonds = bonds.ToArray
        End Sub

    End Class
End Namespace