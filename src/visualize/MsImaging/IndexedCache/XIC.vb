#Region "Microsoft.VisualBasic::b51bf44b0e1550efe58ee391929ff937, visualize\MsImaging\IndexedCache\XIC.vb"

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

    '   Total Lines: 133
    '    Code Lines: 92
    ' Comment Lines: 22
    '   Blank Lines: 19
    '     File Size: 4.43 KB


    '     Class MatrixXIC
    ' 
    '         Properties: intensity, mz
    ' 
    '         Function: Decode, GetLayer
    ' 
    '         Sub: Serialize
    ' 
    '     Class PointXIC
    ' 
    '         Properties: pixels, x, y
    ' 
    '         Function: GetLayer
    ' 
    '         Sub: Serialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
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

        Public Shared Function Decode(data As Stream, dims As Size) As MatrixXIC
            Using bin As New BinaryDataReader(data) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Dim type As Integer = bin.ReadByte()

                If type = 0 Then
                    Dim mz As Double = bin.ReadDouble
                    Dim intensity As Double() = bin.ReadDoubles(dims.Area)

                    Return New MatrixXIC With {
                        .mz = mz,
                        .intensity = intensity
                    }
                Else
                    Dim mz As Double = bin.ReadDouble
                    Dim nsize As Integer = bin.ReadInt32
                    Dim intensity As Double() = bin.ReadDoubles(nsize)
                    Dim x As Integer() = bin.ReadInt32s(nsize)
                    Dim y As Integer() = bin.ReadInt32s(nsize)

                    Return New PointXIC With {
                        .intensity = intensity,
                        .mz = mz,
                        .x = x,
                        .y = y
                    }
                End If
            End Using
        End Function

        Public Overridable Function GetLayer(dims As Size) As SingleIonLayer
            Dim pixels As New List(Of PixelData)
            Dim idx As i32 = Scan0

            For i As Integer = 1 To dims.Width
                For j As Integer = 1 To dims.Height
                    pixels.Add(New PixelData(i, j, _intensity(++idx)))
                Next
            Next

            Return New SingleIonLayer With {
                .DimensionSize = dims,
                .IonMz = mz.ToString("F4"),
                .MSILayer = pixels.ToArray
            }
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
