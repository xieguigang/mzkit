#Region "Microsoft.VisualBasic::12d0d775e65f76af46ff020d74e7cd81, mzkit\src\assembly\assembly\mzPack\Stream\MSScan.vb"

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

'   Total Lines: 57
'    Code Lines: 38
' Comment Lines: 10
'   Blank Lines: 9
'     File Size: 1.93 KB


'     Class MSScan
' 
'         Properties: rt, scan_id
' 
'         Function: GetIntensity, GetMs, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace mzData.mzWebCache

    Public Class MSScan : Inherits PeakList
        Implements IRetentionTime
        Implements INamedValue
        Implements IMsScan
        Implements ISpectrum

        Public Overridable Property rt As Double Implements IRetentionTime.rt
        Public Property scan_id As String Implements INamedValue.Key

        Private Sub SetIons(ions As IEnumerable(Of ms2)) Implements ISpectrum.SetIons
            With ions.ToArray
                mz = .Select(Function(i) i.mz).ToArray
                into = .Select(Function(i) i.intensity).ToArray
            End With
        End Sub

        Public Overrides Function ToString() As String
            Return scan_id
        End Function

        ''' <summary>
        ''' get XIC
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        Public Function GetIntensity(mz As Double, tolerance As Tolerance) As Double Implements IMsScan.GetMzIonIntensity
            Dim max As Double = 0

            For i As Integer = 0 To Me.mz.Length - 1
                If tolerance(mz, Me.mz(i)) Then
                    If max < into(i) Then
                        max = into(i)
                    End If
                End If
            Next

            Return max
        End Function

        Public Iterator Function GetMs(mz As Double, tolerance As Tolerance) As IEnumerable(Of ms1_scan)
            For i As Integer = 0 To Me.mz.Length - 1
                If tolerance(mz, Me.mz(i)) Then
                    Yield New ms1_scan(Me.mz(i), rt, into(i))
                End If
            Next
        End Function

        ''' <summary>
        ''' create matrix from [<see cref="mz"/>...] and [<see cref="into"/>...] these two vector
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GetMs() As IEnumerable(Of ms2) Implements IMsScan.GetMs, ISpectrum.GetIons
            If mz Is Nothing Then
                Return
            End If

            For i As Integer = 0 To mz.Length - 1
                Yield New ms2 With {
                    .mz = mz(i),
                    .intensity = into(i)
                }
            Next
        End Function
    End Class
End Namespace
