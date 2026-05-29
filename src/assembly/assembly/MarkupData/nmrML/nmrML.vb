#Region "Microsoft.VisualBasic::5c6251a0e8f93283454a8b6178f4d03a, assembly\assembly\MarkupData\nmrML\nmrML.vb"

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


    ' Code Statistics:

    '   Total Lines: 87
    '    Code Lines: 61 (70.11%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 26 (29.89%)
    '     File Size: 2.71 KB


    '     Class XML
    ' 
    '         Properties: accession, accession_url, acquisition, contactList, cvList
    '                     fileDescription, id, sourceFileList, spectrumAnnotationList, spectrumList
    '                     version
    ' 
    '     Class spectrumAnnotationList
    ' 
    '         Properties: atomAssignment
    ' 
    '     Class atomAssignment
    ' 
    '         Properties: chemicalCompound, spectrumRef
    ' 
    '     Class chemicalCompound
    ' 
    '         Properties: identifierList
    ' 
    '     Class identifierList
    ' 
    '         Properties: identifier
    ' 
    '     Class identifier
    ' 
    '         Properties: accession, cvRef, name
    ' 
    '     Class contactList
    ' 
    '         Properties: contacts
    ' 
    '     Class contact
    ' 
    '         Properties: email, fullname, id, organization
    ' 
    '     Class sourceFileList
    ' 
    '         Properties: sourceFiles
    ' 
    '     Class sourceFile
    ' 
    '         Properties: id, location, name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.nmrML

    <XmlRoot("nmrML", [Namespace]:=nmrML.XML.namespace)>
    Public Class XML

        Public Const [namespace] As String = "http://nmrml.org/schema"

        <XmlAttribute> Public Property accession As String
        <XmlAttribute> Public Property accession_url As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property version As String

        Public Property cvList As cvList
        Public Property fileDescription As fileDescription
        Public Property contactList As contactList
        Public Property sourceFileList As sourceFileList

        <XmlElement(NameOf(acquisition))>
        Public Property acquisition As acquisition()
        <XmlElement(NameOf(spectrumList))>
        Public Property spectrumList As spectrumList()
        Public Property spectrumAnnotationList As spectrumAnnotationList

    End Class

    Public Class spectrumAnnotationList

        <XmlElement>
        Public Property atomAssignment As atomAssignment()
    End Class

    Public Class atomAssignment
        <XmlAttribute> Public Property spectrumRef As String
        Public Property chemicalCompound As chemicalCompound
    End Class

    Public Class chemicalCompound
        Public Property identifierList As identifierList
    End Class

    Public Class identifierList

        <XmlElement>
        Public Property identifier As identifier()
    End Class

    Public Class identifier
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property cvRef As String
        <XmlAttribute> Public Property accession As String
    End Class

    Public Class contactList

        <XmlElement(NameOf(contact))>
        Public Property contacts As contact()

    End Class

    Public Class contact

        <XmlAttribute> Public Property email As String
        <XmlAttribute> Public Property fullname As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property organization As String

    End Class

    Public Class sourceFileList

        <XmlElement(NameOf(sourceFile))>
        Public Property sourceFiles As sourceFile()

    End Class

    Public Class sourceFile : Inherits Params

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property location As String
        <XmlAttribute> Public Property name As String

    End Class
End Namespace
