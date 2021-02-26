#Region "Microsoft.VisualBasic::49764638572636ad023db14e395a4914, TargetedMetabolomics\MRM\MRMIonExtract.vb"

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

    '     Class MRMIonExtract
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetSamplePeaks, GetTargetPeak, LoadSamples
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math
Imports chromatogramTicks = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Namespace MRM

    Public Class MRMIonExtract : Inherits FeatureExtract(Of indexedmzML)

        ReadOnly ionpairs As IsomerismIonPairs()
        ReadOnly ms1ppm As Tolerance

        Public Sub New(ionpairs As IEnumerable(Of IonPair), ms1ppm As Tolerance, peakwidth As DoubleRange)
            MyBase.New(peakwidth)

            Me.ms1ppm = ms1ppm
            Me.ionpairs = IonPair _
                .GetIsomerism(ionpairs.ToArray, ms1ppm) _
                .ToArray
        End Sub

        Public Shared Function GetTargetPeak(ion As IonPair,
                                             chr As chromatogramTicks,
                                             Optional preferName As Boolean = False) As TargetPeakPoint

            Dim ticks As ChromatogramTick() = chr.Ticks
            Dim peakWin As DoubleRange = ticks.Shadows.MRMPeak(baselineQuantile:=0.65)

            ticks = ticks.Shadows.PickArea(peakWin).ToArray

            Dim peak As New ROIPeak With {
                .window = peakWin,
                .base = ticks.Baseline(0.65),
                .peakHeight = ticks _
                    .Select(Function(t) t.Intensity) _
                    .Max,
                .ticks = ticks
            }

            Return New TargetPeakPoint With {
                .Name = If(preferName, ion.name, ion.accession),
                .Peak = peak,
                .ChromatogramSummary = peak.ticks _
                    .Summary _
                    .ToArray
            }
        End Function

        Public Overrides Iterator Function GetSamplePeaks(sample As indexedmzML) As IEnumerable(Of TargetPeakPoint)
            Dim sampleName As String = DirectCast(sample, IFileReference).FilePath.BaseName
            Dim point As TargetPeakPoint
            Dim rawList As chromatogramTicks() = sample.mzML.run.chromatogramList.list

            For Each ionData In rawList.MRMSelector(ionpairs, ms1ppm)
                point = GetTargetPeak(ionData.ion.target, ionData.chromatogram)
                point.SampleName = sampleName

                Yield point
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="files"></param>
        ''' <param name="qIon">the MRM quantify ion pair</param>
        ''' <returns></returns>
        Public Shared Iterator Function LoadSamples(files As IEnumerable(Of NamedValue(Of String)), qIon As IonPair) As IEnumerable(Of TargetPeakPoint)
            Dim da3 As Tolerance = Tolerance.DeltaMass(0.3)
            Dim raw As indexedmzML
            Dim rawList As chromatogramTicks()
            Dim ionLine As chromatogramTicks
            Dim peakTicks As TargetPeakPoint

            For Each file As NamedValue(Of String) In files
                raw = indexedmzML.LoadFile(file)
                rawList = raw.mzML.run.chromatogramList.list
                ionLine = rawList _
                    .Where(Function(c) qIon.Assert(c, da3)) _
                    .FirstOrDefault
                peakTicks = MRMIonExtract.GetTargetPeak(qIon, ionLine, preferName:=True)
                peakTicks.SampleName = file.Name

                Yield peakTicks
            Next
        End Function
    End Class
End Namespace
