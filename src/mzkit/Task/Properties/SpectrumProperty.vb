#Region "Microsoft.VisualBasic::9e5885f6472a268442804fe24c9582c9, src\mzkit\Task\Properties\SpectrumProperty.vb"

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

    ' Class SpectrumProperty
    ' 
    '     Properties: activationMethod, centroided, collisionEnergy, msLevel, polarity
    '                 precursorCharge, precursorMz, rawfile, retentionTime, rtmin
    '                 scanId
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Serialization.JSON
Imports stdNum = System.Math

Public Class SpectrumProperty

    <Description("The ms level of current scan data.")>
    <Category("Precursor Ion")> Public Property msLevel As Integer
    <Description("M/z of current ion scan.")>
    <Category("Precursor Ion")> Public Property precursorMz As Double
    <Description("The retension time in seconds.")>
    <Category("Precursor Ion")> Public Property retentionTime As Double
    <Description("The retension time in minute.")>
    <Category("Precursor Ion")> Public Property rtmin As Double

    <Description("The charge value of current ion.")>
    <Category("Precursor Ion")> Public Property precursorCharge As Double
    <Description("The charge polarity of current ion.")>
    <Category("Precursor Ion")> Public Property polarity As String

    <Description("Activation method for produce the product fragments of current ion's scan.")>
    <Category("MS/MS")> Public Property activationMethod As String
    <Description("Energy level that used for produce the product fragments of current ion's scan.")>
    <Category("MS/MS")> Public Property collisionEnergy As String
    <Description("Current ion scan is in centroid mode? False means in profile mode.")>
    <Category("MS/MS")> Public Property centroided As String

    Public Property rawfile As String
    Public Property scanId As String

    Sub New(scanId As String, rawfile As String, msLevel As Integer, attrs As ScanMS2)
        With attrs
            msLevel = msLevel
            collisionEnergy = .collisionEnergy
            centroided = .centroided
            precursorMz = .parentMz.ToString("F4")
            retentionTime = .rt.ToString("F2")
            precursorCharge = .charge
            polarity = .polarity
            activationMethod = .activationMethod.Description
            rtmin = stdNum.Round(retentionTime / 60, 2)
        End With

        Me.rawfile = rawfile
        Me.scanId = scanId
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
