Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Namespace mzData.mzWebCache

    Public Class BinaryStreamReader : Implements IMagicBlock
        Implements IDisposable

        Dim disposedValue As Boolean
        Dim file As BinaryDataReader
        Dim index As Dictionary(Of String, Long)

        Public ReadOnly Property magic As String Implements IMagicBlock.magic
            Get
                Return BinaryStreamWriter.Magic
            End Get
        End Property

        Sub New(file As String)
            Me.file = New BinaryDataReader(file.Open(IO.FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True), encoding:=Encodings.ASCII)
            Me.file.ByteOrder = ByteOrder.LittleEndian

            If Not Me.VerifyMagicSignature(Me.file) Then
                Throw New InvalidProgramException("invalid magic header!")
            Else
                Call loadIndex()
            End If
        End Sub

        Private Sub loadIndex()

        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call file.Dispose()
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