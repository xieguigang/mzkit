#Region "Microsoft.VisualBasic::8df98d85659269d403e443ca420630f6, mzmath\ms2_math-core\Spectra\ISpectrumScanData.vb"

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

    '   Total Lines: 18
    '    Code Lines: 10 (55.56%)
    ' Comment Lines: 4 (22.22%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (22.22%)
    '     File Size: 665 B


    '     Interface ISpectrumScanData
    ' 
    '         Properties: ActivationMethod, Charge, CollisionEnergy, Polarity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Spectra

    ''' <summary>
    ''' an abstract spectrum source data model which is describ the source data model 
    ''' from the assembly rawdata file.
    ''' </summary>
    Public Interface ISpectrumScanData : Inherits IMs1Scan, IReadOnlyId, ISpectrumVector

        ReadOnly Property ActivationMethod As ActivationMethods
        ReadOnly Property CollisionEnergy As Double
        ReadOnly Property Charge As Integer
        ReadOnly Property Polarity As IonModes

    End Interface
End Namespace
