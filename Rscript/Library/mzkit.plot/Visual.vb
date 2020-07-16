#Region "Microsoft.VisualBasic::91a2d71faccab8672004bbb4e4fbda40, Rscript\Library\mzkit.plot\Visual.vb"

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

' Module Visual
' 
'     Function: getSpectrum, SpectrumPlot
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("mzkit.visual")>
Module Visual

    ''' <summary>
    ''' Plot of the mass spectrum
    ''' </summary>
    ''' <param name="spectrum"></param>
    ''' <param name="alignment"></param>
    ''' <param name="title$"></param>
    ''' <returns></returns>
    <ExportAPI("mass_spectrum.plot")>
    Public Function SpectrumPlot(spectrum As Object, Optional alignment As Object = Nothing, Optional title$ = "Mass Spectrum Plot") As GraphicsData
        If alignment Is Nothing Then
            Return MassSpectra.MirrorPlot(getSpectrum(spectrum), plotTitle:=title)
        Else
            Return MassSpectra.AlignMirrorPlot(getSpectrum(spectrum), getSpectrum(alignment), title:=title)
        End If
    End Function

    Private Function getSpectrum(data As Object) As LibraryMatrix
        Dim type As Type = data.GetType

        Select Case type
            Case GetType(ms2())
                Return New LibraryMatrix With {.ms2 = data, .name = "Mass Spectrum"}
            Case GetType(LibraryMatrix)
                Return data
            Case GetType(MGF.Ions)
                Return DirectCast(data, MGF.Ions).GetLibrary
            Case GetType(dataframe)
                Dim matrix As dataframe = DirectCast(data, dataframe)
                Dim mz As Double() = REnv.asVector(Of Double)(matrix.getColumnVector("mz"))
                Dim into As Double() = REnv.asVector(Of Double)(matrix.getColumnVector("into"))
                Dim ms2 As ms2() = mz _
                    .Select(Function(m, i)
                                Return New ms2 With {
                                    .mz = m,
                                    .intensity = into(i),
                                    .quantity = into(i)
                                }
                            End Function) _
                    .ToArray

                Return New LibraryMatrix With {.ms2 = ms2, .name = "Mass Spectrum"}
            Case GetType(PeakMs2)
                Return DirectCast(data, PeakMs2).mzInto
            Case Else
                Throw New NotImplementedException(type.FullName)
        End Select
    End Function

    <ExportAPI("plot.UV_signals")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function PlotUVSignals(<RRawVectorArgument>
                                  timeSignals As Object,
                                  Optional size As Object = "1600,1200",
                                  Optional padding As Object = g.DefaultPadding,
                                  Optional colorSet As String = "Set1:c8",
                                  Optional pt_size As Single = 8,
                                  Optional line_width As Single = 5,
                                  Optional env As Environment = Nothing) As Object

        Dim signals As pipeline = pipeline.TryCreatePipeline(Of GeneralSignal)(timeSignals, env)

        If signals.isError Then
            Return signals.getError
        End If

        Return UVsignalPlot.Plot(
            signals:=signals.populates(Of GeneralSignal)(env),
            size:=InteropArgumentHelper.getSize(size),
            padding:=InteropArgumentHelper.getPadding(padding),
            colorSet:=colorSet,
            pt_size:=pt_size,
            line_width:=line_width
        )
    End Function
End Module
