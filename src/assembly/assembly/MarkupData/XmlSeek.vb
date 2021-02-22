Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace MarkupData

    ''' <summary>
    ''' binary data seeking of the xml data file.
    ''' </summary>
    Public Class XmlSeek : Implements IDisposable

        ReadOnly bin As BinaryDataReader
        ReadOnly type As XmlFileTypes = XmlFileTypes.mzXML
        ReadOnly indexOffset As Long
        ReadOnly sha1 As String

        Dim disposedValue As Boolean

        Sub New(file As String)
            bin = file.OpenBinaryReader(Encodings.UTF8)
            type = ParseFileType(file)

            With parseIndex()
                sha1 = .sha1
                indexOffset = .indexOffset
            End With

            bin.Seek(indexOffset, SeekOrigin.Begin)
        End Sub

        Private Function parseIndex() As (indexOffset As Long, sha1 As String)
            Dim tails As String = bin.BaseStream.Tails(128, encoding:=bin.Encoding)

            Select Case type
                Case XmlFileTypes.mzXML
                    Return parseIndexOfmzXML(tails)

                Case XmlFileTypes.mzML, XmlFileTypes.imzML
                    Return parseIndexOfmzML(tails)

                Case Else
                    Throw New NotImplementedException(type.Description)
            End Select
        End Function

        Private Shared Function parseIndexOfmzXML(tails As String) As (indexOffset As Long, sha1 As String)
            Dim offset As Long = tails.Match("[<]indexOffset.+\d+[<]/indexOffset>").GetValue.ParseLong
            Dim sha1 As String = tails.Match("[<]sha1.+[<]/sha1>").GetValue

            Return (offset, sha1)
        End Function

        Private Shared Function parseIndexOfmzML(tails As String) As (indexOffset As Long, sha1 As String)
            Dim offset As Long = tails.Match("[<]indexListOffset.+\d+[<]/indexListOffset>").GetValue.ParseLong
            Dim sha1 As String = tails.Match("[<]fileChecksum.+[<]/fileChecksum>").GetValue

            Return (offset, sha1)
        End Function

        Public Shared Function ParseFileType(file As String) As XmlFileTypes
            Select Case file.ExtensionSuffix.ToLower
                Case "mzxml" : Return XmlFileTypes.mzXML
                Case "mzml" : Return XmlFileTypes.mzML
                Case "imzml" : Return XmlFileTypes.imzML
                Case "mzdata" : Return XmlFileTypes.mzData
                Case Else
                    Throw New NotSupportedException($"unknown file data type: {file.ExtensionSuffix}!")
            End Select
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call bin.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace