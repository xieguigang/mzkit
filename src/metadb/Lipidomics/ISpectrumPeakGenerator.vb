﻿#Region "Microsoft.VisualBasic::2abe88bf5088c64c946ddbb8754a8c94, metadb\Lipidomics\ISpectrumPeakGenerator.vb"

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

    '   Total Lines: 9
    '    Code Lines: 7 (77.78%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 2 (22.22%)
    '     File Size: 704 B


    ' Interface ISpectrumPeakGenerator
    ' 
    '     Function: GetAcylDoubleBondSpectrum, GetAlkylDoubleBondSpectrum, GetSphingoDoubleBondSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS


Public Interface ISpectrumPeakGenerator
    Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
    Function GetAlkylDoubleBondSpectrum(lipid As ILipid, acylChain As AlkylChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
    Function GetSphingoDoubleBondSpectrum(lipid As ILipid, acylChain As SphingoChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
End Interface
