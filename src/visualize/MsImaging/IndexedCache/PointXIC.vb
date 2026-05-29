#Region "Microsoft.VisualBasic::cb83a120f47194a5c9d51c43321d9bb5, visualize\MsImaging\IndexedCache\PointXIC.vb"

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

    '   Total Lines: 55
    '    Code Lines: 36 (65.45%)
    ' Comment Lines: 12 (21.82%)
    '    - Xml Docs: 91.67%
    ' 
    '   Blank Lines: 7 (12.73%)
    '     File Size: 1.66 KB


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
