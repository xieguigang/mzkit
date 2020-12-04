Imports System.IO
Imports Microsoft.VisualBasic.Data.IO

Namespace MarkupData.imzML

    Public Class ibdReader : Implements IDisposable

        ReadOnly stream As BinaryDataReader

        Dim disposedValue As Boolean

        ''' <summary>
        ''' The first 16 bytes of the binary file are reserved for an Universally Unique Identifier. 
        ''' It is also saved in the imzML file so that a correct assignment of ibd and imzML file 
        ''' is possible even if the names of both files are different.
        ''' </summary>
        Dim magic As String
        Dim format As Format

        ''' <summary>
        ''' Universal Unique Identifier
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UUID As String
            Get
                Return Me.ToString
            End Get
        End Property

        Sub New(file As Stream, layout As Format)
            stream = New BinaryDataReader(file)
            stream.ByteOrder = ByteOrder.LittleEndian
            format = layout
            magic = stream.ReadBytes(16).Select(Function(b) b.ToString("X2")).JoinBy("")
        End Sub

        Public Overrides Function ToString() As String
            Return magic.Substring(0, 8) & "-" &
                   magic.Substring(8, 4) & "-" &
                   magic.Substring(12, 4) & "-" &
                   magic.Substring(16, 4) & "-" &
                   magic.Substring(20)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call stream.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace