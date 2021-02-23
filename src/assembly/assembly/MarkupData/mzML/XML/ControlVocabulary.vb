Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.Default

Namespace MarkupData.mzML.ControlVocabulary

    Public Class cvList : Inherits List

        <XmlElement(NameOf(cv))>
        Public Property list As cv()

    End Class

    Public Structure cv

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property fullName As String
        <XmlAttribute> Public Property version As String
        <XmlAttribute> Public Property URI As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return fullName
        End Function
    End Structure

    Public Class Params

        <XmlElement(NameOf(cvParam))>
        Public Property cvParams As cvParam()

        <XmlElement(NameOf(userParam))>
        Public Property userParams As userParam()

    End Class

    Public Class userParam : Implements INamedValue

        <XmlAttribute> Public Property name As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property value As String
        <XmlAttribute> Public Property type As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"Dim {name} As {type} = {value}"
        End Function
    End Class

    ''' <summary>
    ''' [<see cref="cvParam.name"/> => <see cref="cvParam"/>]
    ''' </summary>
    Public Class cvParam : Implements INamedValue

        <XmlAttribute> Public Property cvRef As String
        <XmlAttribute> Public Property accession As String
        <XmlAttribute> Public Property name As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property value As String
        <XmlAttribute> Public Property unitName As String
        <XmlAttribute> Public Property unitCvRef As String
        <XmlAttribute> Public Property unitAccession As String

        Shared ReadOnly Unknown As [Default](Of String) = NameOf(Unknown)

        ''' <summary>
        ''' returns <see cref="value"/> as <see cref="Double"/>
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetDouble() As Double
            Return Val(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{accession}] Dim {name} As <{unitName Or Unknown}> = {value}"
        End Function
    End Class
End Namespace
