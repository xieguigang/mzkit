﻿#Region "Microsoft.VisualBasic::ec27a7af66fe6b6e5af6f271e09ed126, assembly\assembly\mzPack\Stream\MSScan.vb"

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

    '   Total Lines: 108
    '    Code Lines: 64 (59.26%)
    ' Comment Lines: 32 (29.63%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (11.11%)
    '     File Size: 3.89 KB


    '     Class MSScan
    ' 
    '         Properties: rt, scan_id
    ' 
    '         Function: GetIntensity, (+2 Overloads) GetMs, ToString
    ' 
    '         Sub: SetIons
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

    ''' <summary>
    ''' an abstract spectrum scan data model with unique reference id,
    ''' scan time and its spectrum data.
    ''' </summary>
    Public Class MSScan : Inherits PeakList
        Implements IRetentionTime
        Implements INamedValue
        Implements IMsScan
        Implements ISpectrum
        Implements IReadOnlyId

        ''' <summary>
        ''' the scan time of current spectrum scan data object.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property rt As Double Implements IRetentionTime.rt

        ''' <summary>
        ''' the unique reference id of current spectrum scan data object
        ''' </summary>
        ''' <returns></returns>
        Public Property scan_id As String Implements INamedValue.Key, IReadOnlyId.Identity

        Private Sub SetIons(ions As IEnumerable(Of ms2)) Implements ISpectrum.SetIons
            With ions.ToArray
                mz = .Select(Function(i) i.mz).ToArray
                into = .Select(Function(i) i.intensity).ToArray
            End With
        End Sub

        ''' <summary>
        ''' view the <see cref="scan_id"/>
        ''' </summary>
        ''' <returns></returns>
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

        ''' <summary>
        ''' get selected ion data
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
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
        Public Overridable Iterator Function GetMs() As IEnumerable(Of ms2) Implements IMsScan.GetMs, ISpectrum.GetIons
            If mz Is Nothing Then
                Return
            End If

            If metadata.IsNullOrEmpty Then
                For i As Integer = 0 To mz.Length - 1
                    Yield New ms2 With {
                        .mz = mz(i),
                        .intensity = into(i)
                    }
                Next
            Else
                For i As Integer = 0 To mz.Length - 1
                    Yield New ms2 With {
                        .mz = mz(i),
                        .intensity = into(i),
                        .Annotation = metadata(i)
                    }
                Next
            End If
        End Function
    End Class
End Namespace
