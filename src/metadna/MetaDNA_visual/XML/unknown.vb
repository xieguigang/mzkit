#Region "Microsoft.VisualBasic::5f02f7be9f12d6da88262757f19723db, MetaDNA_visual\XML\unknown.vb"

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

    ' Class unknown
    ' 
    '     Properties: edges, intensity, length, Msn, name
    '                 scores
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

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

