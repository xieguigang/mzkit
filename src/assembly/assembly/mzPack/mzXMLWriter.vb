Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Namespace MarkupData.mzXML

    Public Class mzXMLWriter : Implements IDisposable

        ReadOnly mzXML As BinaryDataWriter

        Dim disposedValue As Boolean

        Sub New(mzXML As Stream)
            Me.mzXML = New BinaryDataWriter(mzXML) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encodings.UTF8WithoutBOM.CodePage
            }

            Call Me.mzXML.Write("
<?xml version=""1.0"" encoding=""ISO-8859-1""?>
<mzXML xmlns=""http://sashimi.sourceforge.net/schema_revision/mzXML_3.2""
       xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
       xsi:schemaLocation=""http://sashimi.sourceforge.net/schema_revision/mzXML_3.2 http://sashimi.sourceforge.net/schema_revision/mzXML_3.2/mzXML_idx_3.2.xsd"">
")
        End Sub

        Public Sub WriteHeader()

        End Sub

        Public Sub WriteData(mzData As IEnumerable(Of ScanMS1))
            For Each scan As ScanMS1 In mzData

            Next
        End Sub

        Private Sub WriteSha1()

        End Sub

        Private Sub WriteIndex()

        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call WriteIndex()
                    Call WriteSha1()

                    Call mzXML.Flush()
                    Call mzXML.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
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