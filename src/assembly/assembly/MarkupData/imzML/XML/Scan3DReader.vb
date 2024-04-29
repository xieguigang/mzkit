#Region "Microsoft.VisualBasic::d982089d92969629dd1c68ec29742fd4, E:/mzkit/src/assembly/assembly//MarkupData/imzML/XML/Scan3DReader.vb"

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

    '   Total Lines: 63
    '    Code Lines: 48
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 1.77 KB


    '     Class Scan3DReader
    ' 
    '         Properties: x, y, z
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: LoadMsData, ToString
    ' 
    '     Class PointF3D
    ' 
    '         Properties: X, Y, Z
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace MarkupData.imzML

    Public Class Scan3DReader

        ReadOnly ibd As ibdReader
        ReadOnly xyz As ScanData3D

        Public ReadOnly Property x As Double
            Get
                Return xyz.x
            End Get
        End Property

        Public ReadOnly Property y As Double
            Get
                Return xyz.y
            End Get
        End Property

        Public ReadOnly Property z As Double
            Get
                Return xyz.z
            End Get
        End Property

        Sub New(scan As ScanData3D, ibd As ibdReader)
            Me.xyz = scan
            Me.ibd = ibd
        End Sub

        Public Function LoadMsData() As ms2()
            Dim mz As Double() = ibd.ReadArray(xyz.MzPtr)
            Dim intensity As Double() = ibd.ReadArray(xyz.IntPtr)
            Dim scanData As New List(Of ms2)

            For i As Integer = 0 To mz.Length - 1
                If intensity(i) > 0 Then
                    Call scanData.Add(New ms2 With {
                        .mz = mz(i),
                        .intensity = intensity(i)
                    })
                End If
            Next

            Return scanData.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return $"{ibd.UUID} {MyBase.ToString}"
        End Function

    End Class

    Public Class PointF3D : Implements Imaging.PointF3D

        Public Property X As Double Implements Imaging.PointF3D.X
        Public Property Y As Double Implements Imaging.PointF3D.Y
        Public Property Z As Double Implements Imaging.PointF3D.Z

    End Class
End Namespace
