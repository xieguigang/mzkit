Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class SpatialMapping : Inherits ListOf(Of SpotMap)

    <XmlElement> Public Property label As String
    <XmlElement("spot")>
    Public Property spots As SpotMap()
    ''' <summary>
    ''' the transform operation that generates
    ''' the current spot location in STdata.
    ''' </summary>
    ''' <returns></returns>
    Public Property transform As Transform()

    Protected Overrides Function getSize() As Integer
        Return spots.TryCount
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of SpotMap)
        Return spots
    End Function
End Class

Public Class Transform

    Public Enum Operation
        Rotate
        Mirror
    End Enum

    <XmlAttribute> Public Property op As Operation
    ''' <summary>
    ''' rotate angle in degree if the <see cref="op"/> code is <see cref="Transform.Operation.Rotate"/>
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property argument As Double

    Public Overrides Function ToString() As String
        Return $"{op.Description}({argument})"
    End Function

End Class