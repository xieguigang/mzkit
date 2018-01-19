Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.MassSpectrum.Assembly.mzXML

Namespace mzML

    Public Class Xml
        Public Property mzML As mzML
        Public Property indexList As indexList
        Public Property indexListOffset As Long
        Public Property fileChecksum As String
    End Class

    Public Class indexList : Inherits List
        <XmlElement(NameOf(index))>
        Public Property index As index()
    End Class

    Public Class index
        <XmlAttribute>
        Public Property name As String
        <XmlElement(NameOf(offset))>
        Public Property offsets As offset()
    End Class

    Public Class offset
        <XmlAttribute>
        Public Property idRef As String
        <XmlText>
        Public Property value As Long
    End Class

    <XmlType(NameOf(mzML), [Namespace]:=mzML.Xmlns)>
    Public Class mzML

        Public Const Xmlns$ = Extensions.Xmlns

        Public Property cvList As cvList
        Public Property run As run

    End Class

    Public Class cvList : Inherits List

        <XmlElement(NameOf(cv))>
        Public Property list As cv()
    End Class

    Public Class List
        <XmlAttribute> Public Property count As Integer
    End Class

    Public Structure cv
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property fullName As String
        <XmlAttribute> Public Property version As String
        <XmlAttribute> Public Property URI As String
    End Structure

    Public Class chromatogramList : Inherits List
        <XmlAttribute>
        Public Property defaultDataProcessingRef As String
        <XmlElement(NameOf(chromatogram))>
        Public Property list As chromatogram()
    End Class

    Public Class chromatogram : Inherits Params

        <XmlAttribute> Public Property index As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property defaultArrayLength As String

        Public Property binaryDataArrayList As binaryDataArrayList
        Public Property precursor As precursor
        Public Property product As product

    End Class

    Public Class precursor : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params

    End Class

    Public Class product : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params

    End Class

    Public Interface IMRMSelector
        Property isolationWindow As Params
    End Interface

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

        Public Overrides Function ToString() As String
            Return $"Dim {name} As {type} = {value}"
        End Function
    End Class

    Public Class binaryDataArrayList : Inherits List
        <XmlElement(NameOf(binaryDataArray))>
        Public Property list As binaryDataArray()
    End Class

    Public Class binaryDataArray : Implements IBase64Container

        Public Property encodedLength As Integer
        <XmlElement(NameOf(cvParam))>
        Public Property cvParams As cvParam()
        Public Property binary As String Implements IBase64Container.BinaryArray

        Public Overrides Function ToString() As String
            Return binary
        End Function

        Public Function GetPrecision() As Integer Implements IBase64Container.GetPrecision
            If Not cvParams.KeyItem("64-bit float") Is Nothing Then
                Return 64
            ElseIf Not cvParams.KeyItem("32-bit float") Is Nothing Then
                Return 32
            Else
                Throw New NotImplementedException
            End If
        End Function

        Public Function GetCompressionType() As String Implements IBase64Container.GetCompressionType
            If Not cvParams.KeyItem("zlib compression") Is Nothing Then
                Return "zlib"
            Else
                Throw New NotImplementedException
            End If
        End Function
    End Class

    Public Class cvParam : Implements INamedValue

        <XmlAttribute> Public Property cvRef As String
        <XmlAttribute> Public Property accession As String
        <XmlAttribute> Public Property name As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property value As String
        <XmlAttribute> Public Property unitName As String

        Public Overrides Function ToString() As String
            Return $"[{accession}] Dim {name} = {value}"
        End Function
    End Class

    Public Class run
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property defaultInstrumentConfigurationRef As String
        <XmlAttribute> Public Property startTimeStamp As String
        <XmlAttribute> Public Property defaultSourceFileRef As String

        Public Property chromatogramList As chromatogramList
    End Class
End Namespace