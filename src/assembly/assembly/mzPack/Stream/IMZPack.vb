Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq

Namespace mzData.mzWebCache

    ''' <summary>
    ''' An abstract mzpack data model for the ms spectrum data 
    ''' </summary>
    Public Interface IMZPack

        Property MS As ScanMS1()
        Property Application As FileApplicationClass
        Property source As String
        Property metadata As Dictionary(Of String, String)

    End Interface

    Public Module ReaderHelper

        ''' <summary>
        ''' get XIC data points 
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mz"></param>
        ''' <param name="mzErr"></param>
        ''' <returns>data points has already been re-order by time</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetXIC(raw As IMZPack, mz As Double, mzErr As Tolerance) As ChromatogramTick()
            Return raw.MS _
                .SafeQuery _
                .Select(Function(i)
                            Return New ChromatogramTick With {
                                .Time = i.rt,
                                .Intensity = i.GetIntensity(mz, mzErr)
                            }
                        End Function) _
                .OrderBy(Function(ti) ti.Time) _
                .ToArray
        End Function

    End Module
End Namespace