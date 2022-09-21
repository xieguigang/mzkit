#Region "Microsoft.VisualBasic::b7c49eddc0be6a41a66a15a8525ff1ec, mzkit\src\visualize\MsImaging\PixelScan\InMemoryPixel.vb"

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

    '   Total Lines: 53
    '    Code Lines: 41
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 1.65 KB


    '     Class InMemoryPixel
    ' 
    '         Properties: scanId, X, Y
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetMs, GetMsPipe, GetMzIonIntensity, HasAnyMzIon
    ' 
    '         Sub: release
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Pixel

    Public Class InMemoryPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
        Public Overrides ReadOnly Property Y As Integer

        Dim data As ms2()

        Public Overrides ReadOnly Property scanId As String
            Get
                Return $"({X},{Y})"
            End Get
        End Property

        Public Overrides ReadOnly Property sampleTag As String
            Get
                Return "in-memory cache"
            End Get
        End Property

        Sub New(x As Integer, y As Integer, data As ms2())
            Me.X = x
            Me.Y = y
            Me.data = data
        End Sub

        Public Overrides Function HasAnyMzIon() As Boolean
            Return Not data.IsNullOrEmpty
        End Function

        Protected Friend Overrides Sub release()
            Erase data
        End Sub

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Return data.Any(Function(x) mz.Any(Function(mzi) tolerance(mzi, x.mz)))
        End Function

        Public Overrides Function GetMs() As ms2()
            Return data
        End Function

        Protected Friend Overrides Function GetMsPipe() As IEnumerable(Of ms2)
            Return data
        End Function

        Public Overrides Function GetMzIonIntensity(mz As Double, mzdiff As Tolerance) As Double
            Dim allMatched = data.Where(Function(mzi) mzdiff(mz, mzi.mz)).ToArray

            If allMatched.Length = 0 Then
                Return 0
            Else
                Return Aggregate mzi As ms2
                       In allMatched
                       Into Max(mzi.intensity)
            End If
        End Function

        Public Overrides Function GetMzIonIntensity() As Double()
            Return data.Select(Function(i) i.intensity).ToArray
        End Function
    End Class
End Namespace
