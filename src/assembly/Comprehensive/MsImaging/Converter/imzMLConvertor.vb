Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit

Namespace MsImaging

    Public Module imzMLConvertor

        Public Function ConvertImzMLOntheFly(target As String, output As String,
                                             Optional defaultIon As IonModes = IonModes.Positive,
                                             Optional progress As RunSlavePipeline.SetProgressEventHandler = Nothing) As Boolean
            ' convert file on the fly
            Dim allscans As ScanData() = Nothing
            Dim imzml As imzMLMetadata = Nothing
            Dim metadata = Converter.loadimzMLMetadata(target, allscans, metadata:=imzml)
            Dim ibd As String = target.ChangeSuffix("ibd")
            Dim ibdStream As Stream = ibd.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim ibdreader As New ibdReader(ibdStream, imzml.format)
            Dim filename As String = imzml.sourcefiles.First.FileName
            Dim contentMeta As Dictionary(Of String, String) = imzml.AsList

            Using s As Stream = output.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Return allscans _
                    .LoadScanStream(ibdreader, filename, defaultIon, , progress) _
                    .WriteStream(file:=s,
                                 source:=filename,
                                 meta_size:=64 * ByteSize.MB,
                                 [class]:=FileApplicationClass.MSImaging,
                                 metadata:=contentMeta)
            End Using
        End Function
    End Module
End Namespace