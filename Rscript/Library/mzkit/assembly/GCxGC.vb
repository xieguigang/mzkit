Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("GCxGC")>
Module GCxGC

    <ExportAPI("TIC2D")>
    Public Function TIC2D(TIC As ChromatogramTick(), modtime As Double) As D2Chromatogram()
        Return TIC.Demodulate2D(modtime)
    End Function

    <ExportAPI("TIC1D")>
    Public Function TIC1D(matrix As D2Chromatogram()) As ChromatogramTick()
        Return matrix _
            .Select(Function(i)
                        Return New ChromatogramTick With {
                            .Time = i.scan_time,
                            .Intensity = i.intensity
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' extract GCxGC 2d peaks from the mzpack raw data file
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    <ExportAPI("extract_2D_peaks")>
    Public Function create2DPeaks(raw As mzPack) As D2Chromatogram()
        Return raw.MS _
            .Select(Function(d)
                        Return New D2Chromatogram With {
                            .intensity = d.TIC,
                            .scan_time = d.rt,
                            .d2chromatogram = d.products _
                                .Select(Function(t)
                                            Return New ChromatogramTick With {
                                                .Intensity = t.into.Sum,
                                                .Time = t.rt
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
    End Function
End Module
