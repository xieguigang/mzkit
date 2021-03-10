#Region "Microsoft.VisualBasic::b41dd2f1bd1a6ac01f4b9707b8213605, assembly\MarkupData\mzML\XML\MsData\spectrum.vb"

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

    '     Class spectrum
    ' 
    '         Properties: controllerNumber, controllerType, ms_level, precursorList, profile
    '                     scan, scan_time, scanList, selectedIon
    ' 
    '         Function: GetRawMatrix, ScanData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace MarkupData.mzML

    Public Class spectrum : Inherits BinaryData

        <XmlAttribute> Public Property controllerType As String
        <XmlAttribute> Public Property controllerNumber As String
        <XmlAttribute> Public Property scan As String

        Public Property scanList As scanList
        Public Property precursorList As precursorList

        Public ReadOnly Property ms_level As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return cvParams.KeyItem("ms level")?.value
            End Get
        End Property

        Public ReadOnly Property profile As Boolean
            Get
                Return cvParams.Any(Function(cv) cv.name = "profile spectrum")
            End Get
        End Property

        ''' <summary>
        ''' selected ion m/z
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property selectedIon As (mz As Double, intensity As Double)
            Get
                If Not precursorList Is Nothing Then
                    Dim mz = precursorList.precursor(Scan0).selectedIonList.GetIonMz
                    Dim into = precursorList.precursor(Scan0).selectedIonList.GetIonIntensity

                    Return (mz(Scan0), into(Scan0))
                Else
                    Return (0, 0)
                End If
            End Get
        End Property

        ''' <summary>
        ''' 返回来的时间结果值是以秒为单位的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property scan_time As Double
            Get
                With scanList.scans(0).cvParams.KeyItem("scan start time")
                    Dim time# = Val(.value)

                    If .unitName = "second" Then
                        Return time
                    ElseIf .unitName = "minute" Then
                        Return time * 60
                    Else
                        Throw New NotImplementedException(.unitName)
                    End If
                End With
            End Get
        End Property

        ''' <summary>
        ''' this function is only works for mass spectrum raw data
        ''' due to the reason of byte array data is fixed with ``m/z``
        ''' and ``intensity`` values.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetRawMatrix() As ms2()
            Dim mz As Double() = Me.ByteArray("m/z array").Base64Decode
            Dim intensity = Me.ByteArray("intensity array").Base64Decode.AsVector

            If mz.Length = 0 Then
                Return {}
            End If

            Dim relInto As Vector = intensity / intensity.Max
            Dim matrix As ms2() = CInt(defaultArrayLength) _
                .Sequence _
                .Select(Function(i)
                            Return New ms2 With {
                                .mz = mz(i),
                                .intensity = intensity(i)
                            }
                        End Function) _
                .ToArray

            Return matrix
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="basename"></param>
        ''' <returns></returns>
        Public Function ScanData(Optional basename$ = Nothing,
                                 Optional centroid As Boolean = False,
                                 Optional raw As Boolean = False,
                                 Optional centroidTolerance As Tolerance = Nothing,
                                 Optional intocutoff As LowAbundanceTrimming = Nothing) As PeakMs2

            Dim ms2 As ms2() = GetRawMatrix()
            Dim mzInto As New LibraryMatrix With {
                .centroid = Not profile,
                .ms2 = ms2,
                .name = ToString()
            }
            Dim precursor As (mz#, into#)
            Dim activationMethod$
            Dim collisionEnergy As Double
            Dim charge As String = "0"

            Static ms1 As [Default](Of String) = "ms1"

            ' 合并碎片只针对2级碎片有效
            If ParseInteger(ms_level) > 1 Then
                If centroid Then
                    If centroidTolerance Is Nothing Then
                        centroidTolerance = Tolerance.DeltaMass(0.1)
                    End If

                    mzInto = mzInto.CentroidMode(centroidTolerance, intocutoff Or LowAbundanceTrimming.Default)
                End If

                activationMethod = precursorList.precursor(Scan0).GetActivationMethod()
                precursor = selectedIon
                collisionEnergy = precursorList.precursor(Scan0).GetCollisionEnergy

                ' unsure for the scans that missing charge values
                charge = precursorList.precursor(Scan0) _
                    .selectedIonList _
                    .selectedIon(Scan0) _
                    .cvParams _
                    .KeyItem("charge state")?.value Or "-1".AsDefault
            Else
                activationMethod = ms1
            End If

            If Not raw Then
                mzInto = mzInto / mzInto.Max
            End If

            Return New PeakMs2 With {
                .mz = precursor.mz,
                .rt = scan_time,
                .intensity = precursor.into,
                .scan = index,
                .file = basename,
                .mzInto = mzInto.Array,
                .activation = activationMethod,
                .collisionEnergy = collisionEnergy,
                .meta = New Dictionary(Of String, String) From {
                    {"charge", charge}
                }
            }
        End Function

        Public Overrides Function ToString() As String
            Static noTitle As [Default](Of String) = "Unknown title"
            Return cvParams.KeyItem("spectrum title")?.value Or noTitle
        End Function
    End Class

End Namespace
