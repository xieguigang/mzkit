#Region "Microsoft.VisualBasic::1f350ba88fe3d6d0d25ab332418e9f41, assembly\MarkupData\mzML\XML\XML.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Class Xml
    ' 
    '         Properties: fileChecksum, indexList, indexListOffset, mzML
    ' 
    '     Class indexList
    ' 
    '         Properties: index
    ' 
    '     Class index
    ' 
    '         Properties: name, offsets
    ' 
    '     Class offset
    ' 
    '         Properties: idRef, value
    ' 
    '         Function: ToString
    ' 
    '     Class mzML
    ' 
    '         Properties: cvList, run
    ' 
    '     Class cvList
    ' 
    '         Properties: list
    ' 
    '     Class List
    ' 
    '         Properties: count
    ' 
    '     Class DataList
    ' 
    '         Properties: defaultDataProcessingRef
    ' 
    '     Class BinaryData
    ' 
    '         Properties: binaryDataArrayList, defaultArrayLength, id, index
    ' 
    '     Structure cv
    ' 
    '         Properties: fullName, id, URI, version
    ' 
    '     Class precursor
    ' 
    '         Properties: activation, isolationWindow
    ' 
    '     Class product
    ' 
    '         Properties: activation, isolationWindow
    ' 
    '     Class Params
    ' 
    '         Properties: cvParams, userParams
    ' 
    '     Class userParam
    ' 
    '         Properties: name, type, value
    ' 
    '         Function: ToString
    ' 
    '     Class binaryDataArrayList
    ' 
    '         Properties: list
    ' 
    '     Class binaryDataArray
    ' 
    '         Properties: binary, cvParams, encodedLength
    ' 
    '         Function: GetCompressionType, GetPrecision, ToString
    ' 
    '     Class cvParam
    ' 
    '         Properties: accession, cvRef, name, unitAccession, unitCvRef
    '                     unitName, value
    ' 
    '         Function: ToString
    ' 
    '     Class run
    ' 
    '         Properties: chromatogramList, defaultInstrumentConfigurationRef, defaultSourceFileRef, id, spectrumList
    '                     startTimeStamp
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.Default

Namespace MarkupData.mzML

    <XmlType("indexedmzML", [Namespace]:="http://psi.hupo.org/ms/mzml")>
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

        Public Overrides Function ToString() As String
            Return $"{idRef}: {value}"
        End Function
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

    Public Class DataList : Inherits List
        <XmlAttribute>
        Public Property defaultDataProcessingRef As String
    End Class

    Public Class BinaryData : Inherits Params

        <XmlAttribute> Public Property index As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property defaultArrayLength As String

        Public Property binaryDataArrayList As binaryDataArrayList

    End Class

    Public Structure cv
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property fullName As String
        <XmlAttribute> Public Property version As String
        <XmlAttribute> Public Property URI As String
    End Structure

    Public Class precursor : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params

    End Class

    Public Class product : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params

    End Class

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
        <XmlAttribute> Public Property unitCvRef As String
        <XmlAttribute> Public Property unitAccession As String

        Shared ReadOnly Unknown As DefaultValue(Of String) = NameOf(Unknown)

        Public Overrides Function ToString() As String
            Return $"[{accession}] Dim {name} As <{unitName Or Unknown}> = {value}"
        End Function
    End Class

    Public Class run
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property defaultInstrumentConfigurationRef As String
        <XmlAttribute> Public Property startTimeStamp As String
        <XmlAttribute> Public Property defaultSourceFileRef As String

        Public Property chromatogramList As chromatogramList
        Public Property spectrumList As spectrumList
    End Class
End Namespace
