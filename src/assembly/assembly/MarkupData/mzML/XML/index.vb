Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.Default

Namespace MarkupData.mzML

    Public Class indexList : Inherits List

        <XmlElement(NameOf(index))>
        Public Property index As index()

    End Class

    Public Class index

        <XmlAttribute>
        Public Property name As String

        <XmlElement(NameOf(offset))>
        Public Property offsets As offset()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class

    Public Class offset

        <XmlAttribute>
        Public Property idRef As String

        <XmlText> Public Property value As Long

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{idRef}: {value}"
        End Function
    End Class
End Namespace