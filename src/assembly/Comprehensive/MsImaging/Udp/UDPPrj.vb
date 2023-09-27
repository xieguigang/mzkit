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

        Public Function GetRawdataFile(path As String) As String
            If [Global] Is Nothing Then
                Return Nothing
            End If
            If [Global].DataPath.StringEmpty Then
                Return Nothing
            End If

            Return $"{path}/{[Global].DataPath}"
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

        Public Property StartTime As Date
        Public Property EndTime As Date
        Public Property SpecOrigin As String
        Public Property DataPath As String
        Public Property [Operator] As String
        Public Property LastUser As String
        Public Property LastModifier As String
        Public Property Machine As String

    End Class
End Namespace