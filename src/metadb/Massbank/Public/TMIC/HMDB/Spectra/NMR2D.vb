#Region "Microsoft.VisualBasic::64fb9e3a0a9cf3b4827060440b654077, Massbank\Public\TMIC\HMDB\Spectra\NMR2D.vb"

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

    '     Class NMR2D
    ' 
    '         Properties: peakList
    ' 
    '     Class Nmr2DPeak
    ' 
    '         Properties: chemicalShiftX, chemicalShiftY, nmr2Did
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    Public Class NMR2D : Inherits SpectraFile
        Implements IPeakList(Of Nmr2DPeak)

        Public Property peakList As Nmr2DPeak() Implements IPeakList(Of Nmr2DPeak).peakList
    End Class

    Public Class Nmr2DPeak : Inherits Peak

        <XmlElement("nmr-two-d-id")> Public Property nmr2Did As String
        <XmlElement("chemical-shift-x")> Public Property chemicalShiftX As Double
        <XmlElement("chemical-shift-y")> Public Property chemicalShiftY As Double

    End Class
End Namespace
