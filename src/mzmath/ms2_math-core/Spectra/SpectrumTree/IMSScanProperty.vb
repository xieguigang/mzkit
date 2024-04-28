#Region "Microsoft.VisualBasic::6600b12fc5ff5d820e82660198b13a94, G:/mzkit/src/mzmath/ms2_math-core//Spectra/SpectrumTree/IMSScanProperty.vb"

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

    '   Total Lines: 21
    '    Code Lines: 15
    ' Comment Lines: 3
    '   Blank Lines: 3
    '     File Size: 682 B


    '     Interface IMSScanProperty
    ' 
    '         Properties: ScanID, Spectrum
    ' 
    '         Sub: AddPeak
    ' 
    '     Interface IMSProperty
    ' 
    '         Properties: ChromXs, IonMode, PrecursorMz
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Namespace Spectra

    ''' <summary>
    ''' A spectrum object
    ''' </summary>
    Public Interface IMSScanProperty
        Inherits IMSProperty
        Property ScanID As Integer
        Property Spectrum As List(Of SpectrumPeak)
        Sub AddPeak(mass As Double, intensity As Double, Optional comment As String = Nothing)
    End Interface

    Public Interface IMSProperty
        Property ChromXs As ChromXs
        Property IonMode As IonModes
        Property PrecursorMz As Double
    End Interface
End Namespace
