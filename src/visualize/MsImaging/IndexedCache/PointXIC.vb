Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Data.IO

Namespace IndexedCache

    ''' <summary>
    ''' 适用于稀疏数据点
    ''' </summary>
    Public Class PointXIC : Inherits MatrixXIC

        ''' <summary>
        ''' 稀疏点的X坐标轴信息
        ''' </summary>
        ''' <returns></returns>
        Public Property x As Integer()
        ''' <summary>
        ''' 稀疏点的Y坐标轴信息
        ''' </summary>
        ''' <returns></returns>
        Public Property y As Integer()

        Public ReadOnly Property pixels As Integer
            Get
                Return intensity.Length
            End Get
        End Property

        Public Overrides Sub Serialize(buffer As Stream)
            Dim bin As New BinaryDataWriter(buffer)

            bin.ByteOrder = ByteOrder.BigEndian
            ' 1 means type is point XIC
            bin.Write(CByte(1))
            bin.Write(mz)
            bin.Write(intensity.Length)
            bin.Write(intensity)
            bin.Write(x)
            bin.Write(y)
            bin.Flush()
        End Sub

        Public Overrides Function GetLayer(dims As Size) As SingleIonLayer
            Return New SingleIonLayer With {
                .DimensionSize = dims,
                .IonMz = mz.ToString("F4"),
                .MSILayer = intensity _
                    .Select(Function(into, i)
                                Return New PixelData(_x(i), _y(i), into)
                            End Function) _
                    .ToArray
            }
        End Function
    End Class
End Namespace