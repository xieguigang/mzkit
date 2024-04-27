#Region "Microsoft.VisualBasic::31aba5aea8723a7b521cf04d2e365afa, G:/mzkit/src/assembly/Comprehensive//MsImaging/Udp/UDPPrj.vb"

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

    '   Total Lines: 111
    '    Code Lines: 87
    ' Comment Lines: 4
    '   Blank Lines: 20
    '     File Size: 3.52 KB


    '     Class UDPPrj
    ' 
    '         Properties: [Global], Scan, Spectra, UDPVersion
    ' 
    '         Function: GetDimension, GetMassRange, GetMSIMetadata, GetRawdataFile, GetResolution
    '                   ReadUdpFile
    ' 
    '     Class UdpSpectraInfo
    ' 
    '         Properties: SpecAnz
    ' 
    '         Function: ToString
    ' 
    '     Class UdpScanMeta
    ' 
    '         Properties: MassMax, MassMin, MaxN, MaxX, MaxY
    '                     MaxZ, OriginX, OriginY, OriginZ, Pattern
    '                     ResolutionX, ResolutionY, ResolutionZ, SpectraPerPixel
    ' 
    '     Class UdpGlobal
    ' 
    '         Properties: [Operator], DataPath, EndTime, LastModifier, LastUser
    '                     Machine, SpecOrigin, StartTime
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace MsImaging.APSMALDI

    Public Class UDPPrj

        <XmlAttribute>
        Public Property UDPVersion As String
        Public Property [Global] As UdpGlobal
        Public Property Scan As UdpScanMeta
        Public Property Spectra As UdpSpectraInfo

        Public Function GetMassRange() As DoubleRange
            If Scan Is Nothing Then
                Return Nothing
            End If
            If Scan.MassMin = 0 AndAlso Scan.MassMax = 0 Then
                Return Nothing
            End If

            Return New DoubleRange(Scan.MassMin, Scan.MassMax)
        End Function

        Public Function GetDimension() As Size
            If Scan Is Nothing Then
                Return Nothing
            Else
                Return New Size(Scan.MaxX, Scan.MaxY)
            End If
        End Function

        Public Function GetResolution() As Double
            If Scan Is Nothing Then
                Return 13
            Else
                Return (Scan.ResolutionX + Scan.ResolutionY) / 2
            End If
        End Function

        ''' <summary>
        ''' Get metadata for MS-imaging
        ''' </summary>
        ''' <returns></returns>
        Public Function GetMSIMetadata() As Metadata
            Return New Metadata(GetDimension) With {
                .[class] = FileApplicationClass.MSImaging.ToString,
                .mass_range = GetMassRange(),
                .resolution = GetResolution()
            }
        End Function

        Public Function GetRawdataFile(folder As String) As String
            If [Global] Is Nothing Then
                Return Nothing
            End If
            If [Global].DataPath.StringEmpty Then
                Return Nothing
            End If

            Return $"{folder}/{[Global].DataPath}"
        End Function

        Public Shared Function ReadUdpFile(file As String) As UDPPrj
            Return file.LoadXml(Of UDPPrj)
        End Function

    End Class

    Public Class UdpSpectraInfo

        <XmlAttribute> Public Property SpecAnz As String

        Public Overrides Function ToString() As String
            Return SpecAnz
        End Function

    End Class

    Public Class UdpScanMeta
        Public Property MassMin As Double
        Public Property MassMax As Double
        Public Property OriginX As Double
        Public Property OriginY As Double
        Public Property OriginZ As Double
        Public Property ResolutionX As Double
        Public Property ResolutionY As Double
        Public Property ResolutionZ As Double
        Public Property MaxX As Double
        Public Property MaxY As Double
        Public Property MaxZ As Double
        Public Property MaxN As Double
        Public Property Pattern As Double
        Public Property SpectraPerPixel As Double
    End Class

    Public Class UdpGlobal

        Public Property StartTime As String
        Public Property EndTime As String
        Public Property SpecOrigin As String
        Public Property DataPath As String
        Public Property [Operator] As String
        Public Property LastUser As String
        Public Property LastModifier As String
        Public Property Machine As String

    End Class
End Namespace
