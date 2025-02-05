#Region "Microsoft.VisualBasic::d1d3f4db34286c41ce9217e0b70f94f6, assembly\assembly\mzPack\Stream\ScanMS2.vb"

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

    '   Total Lines: 64
    '    Code Lines: 49 (76.56%)
    ' Comment Lines: 7 (10.94%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (12.50%)
    '     File Size: 2.43 KB


    '     Class ScanMS2
    ' 
    '         Properties: activationMethod, centroided, charge, collisionEnergy, intensity
    '                     parentMz, polarity, rt
    ' 
    '         Function: GetMatrix, GetScanMeta, GetSpectrum2, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ValueTypes

Namespace mzData.mzWebCache

    ''' <summary>
    ''' MSn product scan
    ''' </summary>
    Public Class ScanMS2 : Inherits MSScan
        Implements IMs1
        Implements IMs1Scan
        Implements ISpectrumScanData

        ''' <summary>
        ''' the parent ion m/z
        ''' </summary>
        ''' <returns></returns>
        Public Property parentMz As Double Implements IMs1.mz
        Public Overrides Property rt As Double Implements IRetentionTime.rt
        Public Property intensity As Double Implements IMs1Scan.intensity
        Public Property polarity As IonModes Implements ISpectrumScanData.Polarity
        Public Property charge As Integer Implements ISpectrumScanData.Charge
        Public Property activationMethod As ActivationMethods Implements ISpectrumScanData.ActivationMethod
        Public Property collisionEnergy As Double Implements ISpectrumScanData.CollisionEnergy
        Public Property centroided As Boolean

        ''' <summary>
        ''' the ms3/ms4/... product scan data
        ''' </summary>
        ''' <returns></returns>
        Public Property product As ScanMS2

        Public Overloads Function GetMs(loadProductTree As Boolean, MSn As Integer) As IEnumerable(Of ms2)
            If Not loadProductTree Then
                Return Me.GetMs
            ElseIf product Is Nothing Then
                If MSn > 2 Then
                    Return Me.GetMs.Normalize.UnifyTag($"MS{MSn},precursor_m/z={parentMz.ToString("F3")}")
                Else
                    Return Me.GetMs.Normalize
                End If
            Else
                Dim products As ms2() = product _
                    .GetMs(True, MSn + 1) _
                    .ToArray

                Return Me.GetMs.Normalize.JoinIterates(products)
            End If
        End Function

        Public Overrides Function ToString() As String
            Return MyBase.ToString() & " - " & DateTimeHelper.ReadableElapsedTime(rt * 1000)
        End Function

        Public Function GetMatrix() As LibraryMatrix
            Return New LibraryMatrix With {
                .centroid = centroided,
                .ms2 = GetMs.ToArray,
                .name = scan_id
            }
        End Function

        Public Function GetScanMeta() As Meta
            Return New Meta With {
                .id = scan_id,
                .mz = parentMz,
                .intensity = intensity,
                .scan_time = rt
            }
        End Function

        Public Function GetSpectrum2() As PeakMs2
            Return New PeakMs2 With {
                .mz = parentMz,
                .intensity = intensity,
                .lib_guid = scan_id.GetTagValue("#").Value,
                .mzInto = GetMs.ToArray,
                .rt = rt,
                .collisionEnergy = collisionEnergy,
                .activation = activationMethod.ToString
            }
        End Function

    End Class
End Namespace
