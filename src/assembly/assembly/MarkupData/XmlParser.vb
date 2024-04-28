#Region "Microsoft.VisualBasic::06b5c361e2ffe3da16c70a13d0b14b8d, G:/mzkit/src/assembly/assembly//MarkupData/XmlParser.vb"

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

    '   Total Lines: 81
    '    Code Lines: 52
    ' Comment Lines: 16
    '   Blank Lines: 13
    '     File Size: 2.82 KB


    '     Class XmlParser
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getTagName, (+2 Overloads) GotoReadText, (+2 Overloads) ParseDataNode, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData

    ''' <summary>
    ''' A unify xml file data parser
    ''' </summary>
    Public Class XmlParser

        ReadOnly bin As StreamReader
        ReadOnly tag As String
        ReadOnly type As XmlFileTypes

        Sub New(file As Stream, type As XmlFileTypes)
            Me.type = type
            Me.bin = New StreamReader(file)
            Me.tag = getTagName(type)
        End Sub

        Private Shared Function getTagName(type As XmlFileTypes) As String
            Select Case type
                Case XmlFileTypes.mzML, XmlFileTypes.imzML
                    Return "spectrum"
                Case XmlFileTypes.mzXML
                    Return "scan"
                Case Else
                    Throw New NotImplementedException(type.Description)
            End Select
        End Function

        ''' <summary>
        ''' jump to target position and then start to populate
        ''' all text lines data to file end
        ''' </summary>
        ''' <param name="offset"></param>
        ''' <returns></returns>
        Friend Function GotoReadText(offset As Long) As IEnumerable(Of String)
            Return GotoReadText(bin, offset)
        End Function

        ''' <summary>
        ''' jump to target position and then start to populate
        ''' all text lines data to file end
        ''' </summary>
        ''' <param name="bin"></param>
        ''' <param name="offset"></param>
        ''' <returns></returns>
        Friend Shared Iterator Function GotoReadText(bin As StreamReader, offset As Long) As IEnumerable(Of String)
            Call bin.DiscardBufferedData()
            Call bin.BaseStream.Seek(offset, SeekOrigin.Begin)

            Do While Not bin.EndOfStream
                Yield bin.ReadLine
            Loop
        End Function

        Public Function ParseDataNode(Of T As Class)(index As Long) As T
            Return ParseDataNode(Of T)(bin, index, tag)
        End Function

        Public Shared Function ParseDataNode(Of T As Class)(bin As StreamReader, index As Long, tag As String) As T
            If index < 0 Then
                Return Nothing
            End If

            Dim stream As IEnumerable(Of String) = GotoReadText(bin, offset:=index)
            Dim blockText As String = NodeIterator.CreateBlockReader(tag)(stream).FirstOrDefault

            If blockText.StringEmpty Then
                Return Nothing
            Else
                Return Data.CreateNodeObject(Of T)(blockText)
            End If
        End Function

        Public Overrides Function ToString() As String
            Return type.Description & "::" & tag
        End Function
    End Class
End Namespace
