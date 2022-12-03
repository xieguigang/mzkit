Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.Analytical.NMR
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' plot NMR spectrum data
''' </summary>
<Package("NMR")>
Public Module plotNMR

    Friend Sub Main()
        Call Internal.generic.add("plot", GetType(fidData), AddressOf plotFidData)
        Call Internal.generic.add("plot", GetType(FrequencyData), AddressOf plotFrequencyData)
    End Sub

    Public Function plotFrequencyData(freq As FrequencyData, args As list, env As Environment) As Object
        Dim theme As New Theme
        Dim app As New FrequencyPlot(freq, theme)

        Return app.Plot
    End Function

    Public Function plotFidData(fidData As fidData, args As list, env As Environment) As Object
        Dim theme As New Theme
        Dim app As New fidDataPlot(fidData, theme)

        Return app.Plot
    End Function

    <ExportAPI("plot_nmr")>
    Public Function plotNMRSpectrum(nmr As LibraryMatrix,
                                    <RRawVectorArgument> Optional size As Object = "3600,2400",
                                    <RRawVectorArgument> Optional padding As Object = "padding: 200px 400px 300px 100px",
                                    Optional env As Environment = Nothing) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(padding, [default]:="padding: 200px 400px 300px 100px")
        }
        Dim app As New NMRSpectrum(nmr, theme)
        Dim sizeVal = InteropArgumentHelper.getSize(size, env, "3600,2400")

        Return app.Plot(sizeVal)
    End Function
End Module