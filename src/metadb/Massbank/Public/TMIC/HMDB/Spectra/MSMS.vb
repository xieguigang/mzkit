#Region "Microsoft.VisualBasic::b3c3bdf51c82bed16f6a1cc64f5fa6a5, src\metadb\Massbank\Public\TMIC\HMDB\Spectra\MSMS.vb"

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

    '     Class MSMS
    ' 
    '         Properties: collisionEnergyLevel, collisionEnergyVoltage, energyField, peakList
    ' 
    '     Class MSMSPeak
    ' 
    '         Properties: moleculeID, msmsID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    <XmlType("ms-ms")> Public Class MSMS : Inherits SpectraFile
        Implements IPeakList(Of MSMSPeak)

        <XmlElement("energy-field")> Public Property energyField As NullableValue
        <XmlElement("collision-energy-level")> Public Property collisionEnergyLevel As NullableValue
        <XmlElement("collision-energy-voltage")> Public Property collisionEnergyVoltage As NullableValue

        <XmlArray("ms-ms-peaks")>
        Public Property peakList As MSMSPeak() Implements IPeakList(Of MSMSPeak).peakList

    End Class

    <XmlType("ms-ms-peak")> Public Class MSMSPeak : Inherits MSPeak

        <XmlElement("ms-ms-id")>
        Public Property msmsID As String
        <XmlElement("molecule-id")>
        Public Property moleculeID As NullableValue

    End Class
End Namespace
