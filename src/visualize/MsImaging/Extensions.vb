Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel

Public Module Extensions

    ''' <summary>
    ''' parse pixel mapping from 
    ''' </summary>
    ''' <returns>
    ''' [xy => index]
    ''' </returns>
    <Extension>
    Public Function GetPixelKeys(raw As BinaryStreamReader) As Dictionary(Of String, String())
        Return raw.EnumerateIndex _
            .Select(Function(id)
                        Dim meta = raw.GetMetadata(id)
                        Dim pxy = $"{meta!x},{meta!y}"

                        Return (id, pxy)
                    End Function) _
            .GroupBy(Function(t) t.pxy) _
            .ToDictionary(Function(t) t.Key,
                            Function(t)
                                Return t.Select(Function(i) i.id).ToArray
                            End Function)
    End Function
End Module
