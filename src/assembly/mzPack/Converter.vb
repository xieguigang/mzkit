Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Module Converter

    ''' <summary>
    ''' A unify method for load mzpack data from mzXML/mzML raw data file
    ''' </summary>
    ''' <param name="xml">the file path of the raw mzXML/mzML data file.</param>
    ''' <returns></returns>
    Public Function LoadRawFileAuto(xml As String) As mzPack
        If xml.ExtensionSuffix("mzXML") Then
            Return New mzPack With {
                .MS = New mzXMLScans().Load(xml).ToArray
            }
        ElseIf xml.ExtensionSuffix("mzML") Then
            Dim UVdetecor As String = ExtractUVData.GetPhotodiodeArrayDetectorInstrumentConfigurationId(xml)
            Dim scanLoader As New mzMLScans
            Dim MS As ScanMS1() = scanLoader.Load(xml)
            Dim UV As New ChromatogramOverlap
            Dim PDA As New List(Of ChromatogramTick)

            For Each time_scan As GeneralSignal In scanLoader.GetUVScans(UVdetecor)
                Dim scanId As String = time_scan.meta!scan
                Dim scan_time As Double = time_scan.meta!scan_time
                Dim TIC As Double = time_scan.meta!total_ion_current

                PDA.Add(New ChromatogramTick With {.Time = scan_time, .Intensity = TIC})
                UV(scanId) = New DataReader.Chromatogram With {
                    .TIC = time_scan.Strength,
                    .scan_time = time_scan.Measures,
                    .BPC = .TIC
                }
            Next

            Dim PDAPlot As New ChromatogramOverlap

            PDAPlot("PDA") = New DataReader.Chromatogram With {
                .scan_time = PDA.Select(Function(t) t.Time).ToArray,
                .TIC = PDA.Select(Function(t) t.Intensity).ToArray,
                .BPC = .TIC
            }

            Return New mzPack With {
                .MS = MS,
                .Scanners = New Dictionary(Of String, ChromatogramOverlap) From {
                    {ExtractUVData.UVScanType, UV},
                    {"PDA", PDAPlot}
                }
            }
        Else
            Throw New NotImplementedException(xml.ExtensionSuffix)
        End If
    End Function
End Module
