Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class SpatialMapping : Inherits ListOf(Of SpotMap)

    <XmlElement> Public Property label As String
    <XmlElement> Public Property spots As SpotMap()

    Protected Overrides Function getSize() As Integer
        Return spots.TryCount
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of SpotMap)
        Return spots
    End Function
End Class
