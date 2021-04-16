Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData

    Public Class XmlParser

        ReadOnly bin As StreamReader
        ReadOnly tag As String
        ReadOnly type As XmlFileTypes

        Sub New(file As Stream, type As XmlFileTypes)
            Me.type = type
            Me.bin = New StreamReader(file)

            Select Case type
                Case XmlFileTypes.mzML, XmlFileTypes.imzML
                    Throw New NotImplementedException(type.Description)
                Case XmlFileTypes.mzXML
                    tag = "scan"
                Case Else
                    Throw New NotImplementedException(type.Description)
            End Select
        End Sub

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
    End Class
End Namespace