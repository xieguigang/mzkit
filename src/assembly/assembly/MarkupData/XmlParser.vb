Imports System.IO
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData

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