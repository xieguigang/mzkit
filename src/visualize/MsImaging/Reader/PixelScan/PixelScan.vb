#Region "Microsoft.VisualBasic::d73ce77c249438a8a161d1836ef9ccef, mzkit\src\visualize\MsImaging\Reader\PixelScan\PixelScan.vb"

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

'   Total Lines: 76
'    Code Lines: 45
' Comment Lines: 17
'   Blank Lines: 14
'     File Size: 3.09 KB


'     Class PixelScan
' 
'         Function: GetMs, GetMzIonIntensity, ToString
' 
'         Sub: (+2 Overloads) Dispose
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph

Namespace Pixel

    ''' <summary>
    ''' the abstract model of pixel point [x,y] and method for get ms data vector
    ''' </summary>
    Public MustInherit Class PixelScan : Implements IDisposable, IPoint2D, IMsScan, ISpectrum

        Dim disposedValue As Boolean

        Public MustOverride ReadOnly Property X As Integer Implements IPoint2D.X
        Public MustOverride ReadOnly Property Y As Integer Implements IPoint2D.Y
        Public MustOverride ReadOnly Property scanId As String
        Public MustOverride ReadOnly Property sampleTag As String

        ''' <summary>
        ''' used for evaluate the <see cref="Splash"/>
        ''' </summary>
        Sub New()
        End Sub

        Public Overridable Function GetMs() As ms2()
            Return GetMsPipe.ToArray
        End Function

        ''' <summary>
        ''' current pixel scan is empty or not?
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function HasAnyMzIon() As Boolean
        Public MustOverride Function HasAnyMzIon(mz As Double(), tolerance As Tolerance) As Boolean
        Public MustOverride Function GetMzIonIntensity() As Double()
        Public MustOverride Function SetXY(x As Integer, y As Integer) As mzPackPixel
        Protected MustOverride Sub SetIons(ions As IEnumerable(Of ms2)) Implements ISpectrum.SetIons

        Public Overridable Function GetMzIonIntensity(mz As Double, mzdiff As Tolerance) As Double Implements IMsScan.GetMzIonIntensity
            Dim allMatched = GetMsPipe.Where(Function(mzi) mzdiff(mz, mzi.mz)).ToArray

            If allMatched.Length = 0 Then
                Return 0
            Else
                Return Aggregate mzi As ms2
                       In allMatched
                       Into Max(mzi.intensity)
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{X},{Y}]"
        End Function

        Protected Friend MustOverride Function GetMsPipe() As IEnumerable(Of ms2) Implements IMsScan.GetMs, ISpectrum.GetIons
        Protected Friend MustOverride Sub release()

        Private Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call release()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class
End Namespace
