#Region "Microsoft.VisualBasic::5c7020fca5b93170beacac0ae8cb3da4, mzkit\Rscript\Library\mzkit.plot\NMR.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 55
    '    Code Lines: 44
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 2.37 KB


    ' Module plotNMR
    ' 
    '     Function: plotFidData, plotFrequencyData, plotNMRSpectrum
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.Analytical.NMRFidTool
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
        Call Internal.generic.add("plot", GetType(Fid), AddressOf plotFidData)
        Call Internal.generic.add("plot", GetType(Spectrum), AddressOf plotFrequencyData)
    End Sub

    Public Function plotFrequencyData(freq As Spectrum, args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(args!padding, [default]:="padding: 200px 400px 300px 100px")
        }
        Dim app As New nmrSpectrumPlot(freq, theme)
        Dim sizeVal = InteropArgumentHelper.getSize(args!size, env, "3600,2400")

        Return app.Plot(sizeVal)
    End Function

    Public Function plotFidData(fidData As Fid, args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(args!padding, [default]:="padding: 200px 400px 300px 100px")
        }
        Dim app As New fidDataPlot(fidData, theme)
        Dim sizeVal = InteropArgumentHelper.getSize(args!size, env, "3600,2400")

        Return app.Plot(sizeVal)
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
