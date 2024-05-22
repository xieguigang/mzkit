#Region "Microsoft.VisualBasic::30b83d3bacdd2e403e4d76bd726464dc, mzmath\MSFinder\RawData.vb"

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

    '   Total Lines: 17
    '    Code Lines: 11 (64.71%)
    ' Comment Lines: 3 (17.65%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (17.65%)
    '     File Size: 777 B


    ' Class RawData
    ' 
    '     Properties: CarbonNitrogenNumberFromLabeledExperiment, CarbonNitrogenSulfurNumberFromLabeledExperiment, CarbonNumberFromLabeledExperiment, CarbonSulfurNumberFromLabeledExperiment, NitrogenNumberFromLabeledExperiment
    '                 NitrogenSulfurNumberFromLabeledExperiment, OxygenNumberFromLabeledExperiment, SulfurNumberFromLabeledExperiment
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

''' <summary>
''' a spectrum data which is extract from the raw data file.
''' </summary>
Public Class RawData : Inherits PeakMs2

    Public Property CarbonNumberFromLabeledExperiment As Integer
    Public Property SulfurNumberFromLabeledExperiment As Integer
    Public Property NitrogenNumberFromLabeledExperiment As Integer
    Public Property OxygenNumberFromLabeledExperiment As Integer
    Public Property CarbonNitrogenNumberFromLabeledExperiment As Integer
    Public Property CarbonSulfurNumberFromLabeledExperiment As Integer
    Public Property NitrogenSulfurNumberFromLabeledExperiment As Integer
    Public Property CarbonNitrogenSulfurNumberFromLabeledExperiment As Integer

End Class
