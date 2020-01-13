#Region "Microsoft.VisualBasic::56d77b1dc7b4d51b209a4075684da8ea, Rscript\Library\mzkit.quantify\Visual.vb"

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
'     Function: chromatogramPlot, DrawStandardCurve, MRMchromatogramPeakPlot
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("mzkit.quantify.visual")>
Module Visual

    ''' <summary>
    ''' Draw standard curve
    ''' </summary>
    ''' <param name="model">The linear model of the targeted metabolism model data.</param>
    ''' <param name="title">The plot title</param>
    ''' <param name="samples">The point data of samples</param>
    ''' <returns></returns>
    <ExportAPI("standard_curve")>
    Public Function DrawStandardCurve(model As StandardCurve,
                                      Optional title$ = "",
                                      Optional samples As NamedValue(Of Double)() = Nothing,
                                      Optional size$ = "1600,1200",
                                      Optional margin$ = "padding: 200px 100px 150px 150px",
                                      Optional factorFormat$ = "G4") As GraphicsData

        Return StandardCurvesPlot.StandardCurves(
            model:=model,
            samples:=samples,
            name:=title,
            size:=size,
            margin:=margin,
            factorFormat:=factorFormat
        )
    End Function

    <ExportAPI("chromatogram.plot")>
    Public Function chromatogramPlot(mzML$, ions As IonPair()) As GraphicsData
        Return ions.MRMChromatogramPlot(mzML)
    End Function

    <ExportAPI("MRM.chromatogramPeaks.plot")>
    Public Function MRMchromatogramPeakPlot(chromatogram As ChromatogramTick(), Optional title$ = "MRM Chromatogram Peak Plot") As GraphicsData
        Return chromatogram.Plot(
            title:=title,
            showMRMRegion:=True,
            showAccumulateLine:=True
        )
    End Function

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
                Return New LibraryMatrix With {.ms2 = data, .Name = "Mass Spectrum"}
            Case GetType(LibraryMatrix)
                Return data
            Case GetType(MGF.Ions)
                Return DirectCast(data, MGF.Ions).GetLibrary
            Case GetType(dataframe)
                Dim matrix As dataframe = DirectCast(data, dataframe)
                Dim mz As Double() = REnv.asVector(Of Double)(matrix.GetColumnVector("mz"))
                Dim into As Double() = REnv.asVector(Of Double)(matrix.GetColumnVector("into"))
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
End Module
