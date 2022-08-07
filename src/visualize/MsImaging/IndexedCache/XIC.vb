#Region "Microsoft.VisualBasic::6982ff7bb640fb02cbd46e53b8aa5416, mzkit\src\visualize\MsImaging\IndexedCache\XIC.vb"

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

'   Total Lines: 32
'    Code Lines: 26
' Comment Lines: 0
'   Blank Lines: 6
'     File Size: 884.00 B


'     Class XIC
' 
'         Properties: intensity, mz, pixels, x, y
' 
'         Sub: Serialize
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization

Namespace IndexedCache

    ''' <summary>
    ''' 适用于稠密的全矩阵数据
    ''' </summary>
    Public Class MatrixXIC : Inherits RawStream

        Public Property mz As Double

        ''' <summary>
        ''' 在稠密全矩阵数据中，按照行扫描的格式进行排布
        ''' </summary>
        ''' <returns></returns>
        Public Property intensity As Double()

        Public Overrides Sub Serialize(buffer As Stream)
            Dim bin As New BinaryDataWriter(buffer)

            ' the size of the intensity array is already known 
            ' width * height
            bin.ByteOrder = ByteOrder.BigEndian
            ' 0 means type is matrix XIC
            bin.Write(CByte(0))
            bin.Write(mz)
            bin.Write(intensity)
            bin.Flush()
        End Sub

        Public Overridable Function GetLayer(dims As Size) As SingleIonLayer

        End Function
    End Class

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
