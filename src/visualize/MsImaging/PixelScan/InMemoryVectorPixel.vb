#Region "Microsoft.VisualBasic::b0b6488c49177cd439a4400d94285c1c, src\visualize\MsImaging\PixelScan\InMemoryVectorPixel.vb"

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
    '         Constructor: (+4 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) GetBuffer, GetMsPipe, GetMzIonIntensity, HasAnyMzIon, Parse
    '                   ParseVector
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
                file.Flush()

                Return buf.ToArray
            End Using
        End Function

        Public Shared Function Parse(buffer As Byte()) As InMemoryVectorPixel
            If buffer.Length = 0 Then
                Return Nothing
            End If

            Using file As New BinaryDataReader(New MemoryStream(buffer))
                Dim x As Integer = file.ReadInt32
                Dim y As Integer = file.ReadInt32
                Dim size As Integer = file.ReadInt32
                Dim mz As Double() = file.ReadDoubles(size)
                Dim into As Double() = file.ReadDoubles(size)

                Return New InMemoryVectorPixel(x, y, mz, into)
            End Using
        End Function

        Public Shared Iterator Function ParseVector(buffer As Byte()) As IEnumerable(Of InMemoryVectorPixel)
            Using file As New BinaryDataReader(New MemoryStream(buffer))
                Dim n As Integer = file.ReadInt32

                For i As Integer = 0 To n - 1
                    Dim byts As Byte() = file.ReadBytes(file.ReadInt32)

                    Yield Parse(byts)
                Next
            End Using
        End Function

        Public Shared Function GetBuffer(data As InMemoryVectorPixel()) As Byte()
            Using buf As New MemoryStream, file As New BinaryDataWriter(buf)
                Call file.Write(data.Length)

                For Each item In data
                    Dim byts As Byte() = item.GetBuffer

                    Call file.Write(byts.Length)
                    Call file.Write(byts)
                Next

                Call file.Flush()

                Return buf.ToArray
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

        Public Overrides Function GetMzIonIntensity(mz As Double, mzdiff As Tolerance) As Double
            Dim allMatched = GetMsPipe.Where(Function(mzi) mzdiff(mz, mzi.mz)).ToArray

            If allMatched.Length = 0 Then
                Return 0
            Else
                Return Aggregate mzi As ms2
                       In allMatched
                       Into Max(mzi.intensity)
            End If
        End Function
    End Class

End Namespace
