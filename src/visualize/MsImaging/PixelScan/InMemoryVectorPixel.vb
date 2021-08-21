#Region "Microsoft.VisualBasic::95ea729605b68a1912c93e56bf154e91, src\visualize\MsImaging\PixelScan\InMemoryVectorPixel.vb"

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

'     Class InMemoryVectorPixel
' 
'         Properties: intensity, mz, X, Y
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: GetMsPipe, HasAnyMzIon
' 
'         Sub: release
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO

Namespace Pixel

    Public Class InMemoryVectorPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
        Public Overrides ReadOnly Property Y As Integer

        Public ReadOnly Property mz As Double()
        Public ReadOnly Property intensity As Double()

        Sub New(x As Integer, y As Integer, mz As Double(), into As Double())
            Me.X = x
            Me.Y = y
            Me.mz = mz
            Me.intensity = into
        End Sub

        ''' <summary>
        ''' create from mzpack scan data
        ''' </summary>
        ''' <param name="scan"></param>
        Sub New(scan As ScanMS1)
            Call Me.New(scan.meta!x, scan.meta!y, scan.mz, scan.into)
        End Sub

        Sub New(pixel As PixelScan)
            X = pixel.X
            Y = pixel.Y

            If TypeOf pixel Is mzPackPixel Then
                Dim raw As mzPackPixel = DirectCast(pixel, mzPackPixel)

                mz = raw.scan.mz
                intensity = raw.scan.into
            ElseIf TypeOf pixel Is ibdPixel Then
                Dim ibd As ibdPixel = DirectCast(pixel, ibdPixel)
                Dim data As ms2() = ibd.GetMs

                mz = data.Select(Function(i) i.mz).ToArray
                intensity = data.Select(Function(i) i.intensity).ToArray
            Else
                Throw New NotImplementedException(pixel.GetType.FullName)
            End If
        End Sub

        Sub New()
        End Sub

        Public Function GetBuffer() As Byte()
            Using buf As New MemoryStream, file As New BinaryDataWriter(buf)
                file.Write(X)
                file.Write(Y)
                file.Write(mz.Length)
                file.Write(mz)
                file.Write(intensity)

                Return buf.ToArray
            End Using
        End Function

        Public Shared Function Parse(buffer As Byte()) As InMemoryVectorPixel
            Using file As New BinaryDataReader(New MemoryStream(buffer))
                Dim x As Integer = file.ReadInt32
                Dim y As Integer = file.ReadInt32
                Dim size As Integer = file.ReadInt32
                Dim mz As Double() = file.ReadDoubles(size)
                Dim into As Double() = file.ReadDoubles(size)

                Return New InMemoryVectorPixel(x, y, mz, into)
            End Using
        End Function

        Protected Friend Overrides Sub release()
            Erase _mz
            Erase _intensity
        End Sub

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Return mz.Any(Function(mzi) Me.mz.Any(Function(mz2) tolerance(mzi, mz2)))
        End Function

        Protected Friend Overrides Iterator Function GetMsPipe() As IEnumerable(Of ms2)
            For i As Integer = 0 To mz.Length - 1
                Yield New ms2 With {
                    .mz = mz(i),
                    .intensity = intensity(i)
                }
            Next
        End Function
    End Class

End Namespace
