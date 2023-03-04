Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Module RamanSpectroscopy

    <Extension>
    Public Function ToSignal(raman As Raman.Spectroscopy) As GeneralSignal
        Return New GeneralSignal With {
            .description = "Raman Spectroscopy",
            .measureUnit = "Raman Shift [cm-1]",
            .reference = raman.Title,
            .Measures = raman.xyData.Select(Function(p) CDbl(p.X)).ToArray,
            .Strength = raman.xyData.Select(Function(p) CDbl(p.Y)).ToArray,
            .meta = raman.Comments _
                .JoinIterates(raman.DetailedInformation) _
                .JoinIterates(raman.MeasurementInformation) _
                .ToDictionary
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToChromatogram(raman As Raman.Spectroscopy) As ChromatogramTick()
        Return raman.xyData _
            .Select(Function(c) New ChromatogramTick With {.Time = c.X, .Intensity = c.Y}) _
            .ToArray
    End Function
End Module
