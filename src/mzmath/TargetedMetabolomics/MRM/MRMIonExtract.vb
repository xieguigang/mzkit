#Region "Microsoft.VisualBasic::cc2658650d5482a56e02234c812b814b, TargetedMetabolomics\MRM\MRMIonExtract.vb"

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
    '         Function: GetSamplePeaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

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

        Public Overrides Iterator Function GetSamplePeaks(sample As indexedmzML) As IEnumerable(Of TargetPeakPoint)
            Dim sampleName As String = DirectCast(sample, IFileReference).FilePath.BaseName

            For Each ionData In sample.mzML.run.chromatogramList.list.MRMSelector(ionpairs, ms1ppm)
                Yield New TargetPeakPoint With {
                    .Name = ionData.ion.target.accession,
                    .SampleName = sampleName
                }
            Next
        End Function
    End Class
End Namespace
