#Region "Microsoft.VisualBasic::005453f1e9410c323232438ac2fc6faa, MetaDNA\MZ.MetaDNA\keggKB.vb"

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

' Class keggKB
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

''' <summary>
''' KEGG knowledge base for MetaDNA algorithm.
''' </summary>
''' 
<XmlRoot("MetaDNA")> Public Class XML

    <XmlElement("compound")> Public Property compounds As compound()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadDocument(file As String) As XML
        Return file.SolveStream.LoadFromXml(Of XML)
    End Function
End Class

Public Class compound

    <XmlAttribute>
    Public Property kegg As String
    <XmlAttribute("candidates")>
    Public Property size As Integer

    <XmlElement("unknown")>
    Public Property candidates As unknown()

End Class

Public Class unknown
    <XmlAttribute> Public Property name As String
    <XmlAttribute> Public Property Msn As String
    <XmlAttribute> Public Property length As Integer

    <XmlElement("node")>
    Public Property edges As node()
End Class

Public Class node
    <XmlAttribute> Public Property kegg As String
    <XmlAttribute> Public Property ms1 As String

    ''' <summary>
    ''' The ms2 index
    ''' </summary>
    ''' <returns></returns>
    <XmlText>
    Public Property ms2 As String
End Class