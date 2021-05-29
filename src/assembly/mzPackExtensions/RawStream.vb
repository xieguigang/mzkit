Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.Language

Public Module RawStream

    <Extension>
    Public Function LoadFromXRaw(raw As MSFileReader) As mzPack
        Return New XRawStream(raw).StreamTo
    End Function

    <Extension>
    Public Function GetChromatogram(mzpack As mzPack) As Chromatogram
        Dim rt As New List(Of Double)
        Dim BPC As New List(Of Double)
        Dim TIC As New List(Of Double)

        For Each scan As ScanMS1 In mzpack.MS
            rt += scan.rt
            BPC += scan.BPC
            TIC += scan.TIC
        Next

        Return New Chromatogram With {
            .TIC = TIC.ToArray,
            .BPC = BPC.ToArray,
            .scan_time = rt.ToArray
        }
    End Function
End Module
