#Region "Microsoft.VisualBasic::c93dab6fcd5118d49206df4744afe59e, src\metadna\MetaDNA_visual\XML.vb"

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

    ' Class XML
    ' 
    '     Properties: compounds
    ' 
    '     Function: LoadDocument, ToString
    ' 
    ' Class compound
    ' 
    '     Properties: candidates, kegg, size
    ' 
    '     Function: ToString
    ' 
    ' Class unknown
    ' 
    '     Properties: edges, intensity, length, Msn, name
    '                 scores
    ' 
    '     Function: ToString
    ' 
    ' Class node
    ' 
    '     Properties: kegg, ms1, ms2
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' KEGG knowledge base for MetaDNA algorithm.
''' </summary>
''' 
<XmlRoot("MetaDNA")> Public Class XML

    <XmlElement("compound")>
    Public Property compounds As compound()

    Public Overrides Function ToString() As String
        Return compounds _
            .SafeQuery _
            .Select(Function(c) c.kegg) _
            .GetJson
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadDocument(file As String) As XML
        Return file.SolveStream.LoadFromXml(Of XML)
    End Function
End Class

Public Class compound : Implements INamedValue

    <XmlAttribute>
    Public Property kegg As String Implements INamedValue.Key
    <XmlAttribute("candidates")>
    Public Property size As Integer

    <XmlElement("unknown")>
    Public Property candidates As unknown()

    Public Overrides Function ToString() As String
        Return $"{kegg} have {candidates.Length} candidates: {candidates.Select(Function(c) c.name).GetJson}"
    End Function

End Class

Public Class unknown

    <XmlAttribute> Public Property name As String
    <XmlAttribute> Public Property Msn As String
    <XmlAttribute> Public Property length As Integer
    <XmlAttribute> Public Property intensity As Double
    <XmlAttribute> Public Property scores As Double()

    ''' <summary>
    ''' 请注意，第一个推断节点肯定是metaDNA的最初的通过标准品库所鉴定出来的seed数据
    ''' 所以这个seed的ms1名称肯定是在unknown之中找不到的
    ''' </summary>
    ''' <returns></returns>
    <XmlElement("node")>
    Public Property edges As node()

    Public Overrides Function ToString() As String
        Return $"{name}: {edges.Select(Function(n) n.kegg).JoinBy(" -> ")}"
    End Function
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

    Public Overrides Function ToString() As String
        Return $"Dim {kegg} = ""{ms1}|{ms2}"""
    End Function

End Class
