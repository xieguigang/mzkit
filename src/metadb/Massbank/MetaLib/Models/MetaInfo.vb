Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace MetaLib.Models

    Public Class MetaInfo : Implements INamedValue
        Implements IEquatable(Of MetaInfo)

        ''' <summary>
        ''' 该物质在整合库之中的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property formula As String
        <XmlAttribute> Public Property exact_mass As Double

        Public Property name As String

        <XmlElement>
        Public Property synonym As String()

        Public Property xref As xref

        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Overloads Function Equals(other As MetaInfo) As Boolean Implements IEquatable(Of MetaInfo).Equals
            Static metaEquals As MetaEquals

            If metaEquals Is Nothing Then
                metaEquals = New MetaEquals
            End If

            If other Is Nothing Then
                Return False
            Else
                Return metaEquals.Equals(Me, other)
            End If
        End Function
    End Class
End Namespace