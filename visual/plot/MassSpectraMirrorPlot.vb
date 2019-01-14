#Region "Microsoft.VisualBasic::602152f44bc8c777f04fcc4086045b55, plot\MassSpectraMirrorPlot.vb"

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

    ' Module MassSpectra
    ' 
    '     Function: AlignMirrorPlot, MirrorPlot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Spectra

Public Module MassSpectra

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function MirrorPlot(library As LibraryMatrix,
                               Optional size$ = "1200,800",
                               Optional margin$ = "padding: 100px 30px 50px 100px;",
                               Optional intoCutoff# = 0.05,
                               Optional titles$() = Nothing) As GraphicsData

        Dim a As New LibraryMatrix With {.ms2 = library.ms2, .Name = titles.ElementAtOrDefault(0, library.Name)}
        Dim b As New LibraryMatrix With {.ms2 = library.ms2, .Name = titles.ElementAtOrDefault(1, library.Name)}

        Return AlignMirrorPlot(
            a, b,
            size:=size,
            intoCutoff:=intoCutoff,
            margin:=margin
        )
    End Function

    Public Function AlignMirrorPlot(query As LibraryMatrix, ref As LibraryMatrix,
                                    Optional size$ = "1200,800",
                                    Optional margin$ = "padding: 100px 30px 50px 100px;",
                                    Optional intoCutoff# = 0.05) As GraphicsData

        Dim mzRange As DoubleRange = query _
            .Trim(intoCutoff) _
            .Join(ref.Trim(intoCutoff)) _
            .Select(Function(mass) mass.mz) _
            .Range
        Dim qMatrix As (x#, into#)() = query.Select(Function(q) (q.mz, q.intensity)).ToArray
        Dim sMatrix As (x#, into#)() = ref.Select(Function(s) (s.mz, s.intensity)).ToArray

        mzRange = {
            mzRange.Min - (mzRange.Min * 0.125),
            mzRange.Max + (mzRange.Max * 0.125)
        }

        Return AlignmentPlot.PlotAlignment(
            qMatrix, sMatrix,
            queryName:=query.Name,
            subjectName:=ref.Name,
            xrange:=$"{mzRange.Min},{mzRange.Max}",
            yrange:="0,100",
            size:=size, padding:=margin,
            xlab:="M/Z ratio",
            ylab:="Relative Intensity(%)",
            title:="BioDeep™ MS/MS alignment Viewer",
            titleCSS:=CSSFont.Win7Large,
            format:="F0",
            yAxislabelPosition:=YlabelPosition.LeftCenter,
            labelPlotStrength:=0.3
        )
    End Function
End Module

