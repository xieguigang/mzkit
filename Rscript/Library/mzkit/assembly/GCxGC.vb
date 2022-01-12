Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Processing GCxGC comprehensive chromatogram data
''' </summary>
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
    ''' <param name="mz">
    ''' target mz value for extract XIC data. NA means extract 
    ''' TIC data by default.
    ''' </param>
    ''' <param name="mzdiff">
    ''' the mz tolerance error for match the intensity data for
    ''' extract XIC data if the <paramref name="mz"/> is not 
    ''' ``NA`` value.
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function will extract the TIC data by default.
    ''' </remarks>
    <ExportAPI("extract_2D_peaks")>
    <RApiReturn(GetType(D2Chromatogram))>
    Public Function create2DPeaks(raw As mzPack,
                                  Optional mz As Double = Double.NaN,
                                  Optional mzdiff As Object = "ppm:30",
                                  Optional env As Environment = Nothing) As Object

        Dim extract_XIC As Boolean = Not mz.IsNaNImaginary AndAlso mz > 0
        Dim mzErr = Math.getTolerance(mzdiff, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim test As Tolerance = mzErr.TryCast(Of Tolerance)
        Dim extract As Func(Of ScanMS1, D2Chromatogram)

        If extract_XIC Then
            extract = extractXIC(mz, mzdiff:=test)
        Else
            extract = AddressOf extractTIC
        End If

        Return raw.MS _
            .Select(Function(d)
                        Return extract(d)
                    End Function) _
            .ToArray
    End Function

    Private Function extractXIC(mz As Double, mzdiff As Tolerance) As Func(Of ScanMS1, D2Chromatogram)
        Return Function(d)
                   Return New D2Chromatogram With {
                       .scan_time = d.rt,
                       .intensity = d.GetIntensity(mz, mzdiff),
                       .d2chromatogram = d.products _
                            .Select(Function(t)
                                        Return New ChromatogramTick With {
                                            .Time = t.rt,
                                            .Intensity = t.GetIntensity(mz, mzdiff)
                                        }
                                    End Function) _
                            .ToArray
                   }
               End Function
    End Function

    Private Function extractTIC(d As ScanMS1) As D2Chromatogram
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
    End Function
End Module
