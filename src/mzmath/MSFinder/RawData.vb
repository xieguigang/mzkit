#Region "Microsoft.VisualBasic::808e349d5b2b8b93a8210c1fab536f66, mzmath\MSFinder\RawData.vb"

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

    '   Total Lines: 22
    '    Code Lines: 15 (68.18%)
    ' Comment Lines: 3 (13.64%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (18.18%)
    '     File Size: 926 B


    ' Class RawData
    ' 
    '     Properties: CarbonNitrogenNumberFromLabeledExperiment, CarbonNitrogenSulfurNumberFromLabeledExperiment, CarbonNumberFromLabeledExperiment, CarbonSulfurNumberFromLabeledExperiment, NitrogenNumberFromLabeledExperiment
    '                 NitrogenSulfurNumberFromLabeledExperiment, OxygenNumberFromLabeledExperiment, SulfurNumberFromLabeledExperiment
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Serialization.JSON

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

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class
