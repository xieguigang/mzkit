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

        Private Iterator Function GotoReadText(offset As Long) As IEnumerable(Of String)
            Call bin.DiscardBufferedData()
            Call bin.BaseStream.Seek(offset, SeekOrigin.Begin)

            Do While Not bin.EndOfStream
                Yield bin.ReadLine
            Loop
        End Function

        Public Function ParseDataNode(Of T As Class)(index As Long) As T
            Dim stream As IEnumerable(Of String) = GotoReadText(offset:=index)
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