#Region "Microsoft.VisualBasic::3a0e1f94fe692c60856b68d5f5586e43, src\visualize\MsImaging\PixelScan\mzPackPixel.vb"

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
    '         Properties: mz, X, Y
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetMs, HasAnyMzIon
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

        ReadOnly scan As ScanMS1
        ReadOnly pixel As Point

        Public ReadOnly Property mz As Double()
            Get
                Return scan.mz
            End Get
        End Property

        Sub New(scan As ScanMS1)
            Me.scan = scan

            If scan.hasMetaKeys("x", "y") Then
                Me.pixel = New Point With {
                    .X = CInt(Val(scan.meta!x)),
                    .Y = CInt(Val(scan.meta!y))
                }
            Else
                Me.pixel = scan.scan_id _
                    .Match("\[\d+,\d+\]") _
                    .GetStackValue("[", "]") _
                    .DoCall(AddressOf Casting.PointParser)
            End If
        End Sub

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
    End Class
End Namespace
