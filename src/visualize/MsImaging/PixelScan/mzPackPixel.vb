#Region "Microsoft.VisualBasic::7c16ff090eabf03f5a4601164203249d, src\visualize\MsImaging\PixelScan\mzPackPixel.vb"

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

    '     Class mzPackPixel
    ' 
    '         Properties: mz, scan, X, Y
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetMsPipe, GetMzIonIntensity, GetPixelPoint, HasAnyMzIon
    ' 
    '         Sub: release
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Pixel

    Public Class mzPackPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
            Get
                Return pixel.X
            End Get
        End Property

        Public Overrides ReadOnly Property Y As Integer
            Get
                Return pixel.Y
            End Get
        End Property

        Public ReadOnly Property scan As ScanMS1

        ReadOnly pixel As Point

        Public Overrides ReadOnly Property scanId As String
            Get
                Return scan.scan_id
            End Get
        End Property

        Public ReadOnly Property mz As Double()
            Get
                Return scan.mz
            End Get
        End Property

        Sub New(scan As ScanMS1, Optional x As Integer = Integer.MinValue, Optional y As Integer = Integer.MinValue)
            Me.scan = scan

            If x = Integer.MinValue OrElse y = Integer.MinValue Then
                Me.pixel = GetPixelPoint(scan)
            Else
                Me.pixel = New Point(x, y)
            End If
        End Sub

        Public Shared Function GetPixelPoint(scan As ScanMS1) As Point
            If scan.hasMetaKeys("x", "y") Then
                Return New Point With {
                    .X = CInt(Val(scan.meta!x)),
                    .Y = CInt(Val(scan.meta!y))
                }
            Else
                Return scan.scan_id _
                    .Match("\[\d+,\d+\]") _
                    .GetStackValue("[", "]") _
                    .DoCall(AddressOf Casting.PointParser)
            End If
        End Function

        Protected Friend Overrides Function GetMsPipe() As IEnumerable(Of ms2)
            Return scan.GetMs
        End Function

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Return mz _
                .Any(Function(mzi)
                         Return scan.mz.Any(Function(zzz) tolerance(zzz, mzi))
                     End Function)
        End Function

        Protected Friend Overrides Sub release()
            If Not scan Is Nothing Then
                Erase scan.into
                Erase scan.mz
                Erase scan.products
            End If
        End Sub

        Public Overrides Function GetMzIonIntensity(mz As Double, mzdiff As Tolerance) As Double
            Dim allMatched = scan.GetMs.Where(Function(mzi) mzdiff(mz, mzi.mz)).ToArray

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
