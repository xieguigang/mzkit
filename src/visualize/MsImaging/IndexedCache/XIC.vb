Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization

Namespace IndexedCache

    Public Class XIC : Inherits RawStream

        Public Property mz As Double
        Public Property intensity As Double()
        Public Property x As Integer()
        Public Property y As Integer()

        Public ReadOnly Property pixels As Integer
            Get
                Return intensity.Length
            End Get
        End Property

        Public Overrides Sub Serialize(buffer As Stream)
            Dim bin As New BinaryDataWriter(buffer)

            bin.ByteOrder = ByteOrder.BigEndian
            bin.Write(mz)
            bin.Write(intensity.Length)
            bin.Write(intensity)
            bin.Write(x)
            bin.Write(y)
            bin.Flush()
        End Sub
    End Class
End Namespace