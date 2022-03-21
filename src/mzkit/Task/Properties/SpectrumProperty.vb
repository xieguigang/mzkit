#Region "Microsoft.VisualBasic::9bfdb4a3b03181d85a66e7dc10a57910, mzkit\src\mzkit\Task\Properties\SpectrumProperty.vb"

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

    '   Total Lines: 111
    '    Code Lines: 95
    ' Comment Lines: 0
    '   Blank Lines: 16
    '     File Size: 4.94 KB


    ' Class SpectrumProperty
    ' 
    '     Properties: activationMethod, basePeakMz, centroided, collisionEnergy, highMass
    '                 lowMass, maxIntensity, msLevel, n_fragments, polarity
    '                 precursorCharge, precursorMz, rawfile, retentionTime, rtmin
    '                 scanId, totalIons
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ToString
    ' 
    '     Sub: Copy
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Serialization.JSON
Imports stdNum = System.Math

Public Class SpectrumProperty : Implements ICopyProperties

    <Description("The ms level of current scan data.")>
    <Category("Precursor Ion")> Public ReadOnly Property msLevel As Integer
    <Description("M/z of current ion scan.")>
    <Category("Precursor Ion")> Public ReadOnly Property precursorMz As Double
    <Description("The retension time in seconds.")>
    <Category("Precursor Ion")> Public ReadOnly Property retentionTime As Double
    <Description("The retension time in minute.")>
    <Category("Precursor Ion")> Public ReadOnly Property rtmin As Double

    <Description("The charge value of current ion.")>
    <Category("Precursor Ion")> Public ReadOnly Property precursorCharge As Double
    <Description("The charge polarity of current ion.")>
    <Category("Precursor Ion")> Public ReadOnly Property polarity As String

    <Description("Activation method for produce the product fragments of current ion's scan.")>
    <Category("MSn")> Public ReadOnly Property activationMethod As String
    <Description("Energy level that used for produce the product fragments of current ion's scan.")>
    <Category("MSn")> Public ReadOnly Property collisionEnergy As String
    <Description("Current ion scan is in centroid mode? False means in profile mode.")>
    <Category("MSn")> Public ReadOnly Property centroided As String

    <Category("Product Ions")>
    Public ReadOnly Property basePeakMz As Double
    <Category("Product Ions")>
    Public ReadOnly Property maxIntensity As String
    <Category("Product Ions")>
    Public ReadOnly Property totalIons As String
    <Category("Product Ions")>
    Public ReadOnly Property n_fragments As Integer
    <Category("Product Ions")>
    Public ReadOnly Property lowMass As Double
    <Category("Product Ions")>
    Public ReadOnly Property highMass As Double

    Public ReadOnly Property rawfile As String
    Public ReadOnly Property scanId As String

    Sub New(scanId As String, rawfile As String, msLevel As Integer, attrs As ScanMS2)
        With attrs
            Me.msLevel = msLevel
            collisionEnergy = .collisionEnergy
            centroided = .centroided
            precursorMz = .parentMz.ToString("F4")
            retentionTime = .rt.ToString("F2")
            precursorCharge = .charge
            polarity = .polarity
            activationMethod = .activationMethod.Description
            rtmin = stdNum.Round(retentionTime / 60, 2)
        End With

        Dim ms2 As ms2() = attrs.GetMs.ToArray

        If ms2.Length > 0 Then
            With ms2.OrderByDescending(Function(i) i.intensity).First
                basePeakMz = stdNum.Round(.mz, 4)
                maxIntensity = .intensity.ToString("G4")
            End With

            totalIons = (Aggregate i In ms2 Into Sum(i.intensity)).ToString("G4")
            n_fragments = ms2.Length

            With ms2.Select(Function(i) i.mz).ToArray
                lowMass = stdNum.Round(.Min, 4)
                highMass = stdNum.Round(.Max, 4)
            End With
        End If

        Me.rawfile = rawfile
        Me.scanId = scanId
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Sub Copy() Implements ICopyProperties.Copy
        Dim text As New StringBuilder($"spectrum property{vbTab}value")

        Call text.AppendLine()
        Call text.AppendLine($"rawfile{vbTab}{rawfile}")
        Call text.AppendLine($"scanId{vbTab}{scanId}")
        Call text.AppendLine($"msLevel{vbTab}{msLevel}")
        Call text.AppendLine($"precursorMz{vbTab}{precursorMz}")
        Call text.AppendLine($"retentionTime{vbTab}{retentionTime}")
        Call text.AppendLine($"rt(min){vbTab}{rtmin}")
        Call text.AppendLine($"precursorCharge{vbTab}{precursorCharge}")
        Call text.AppendLine($"polarity{vbTab}{polarity}")
        Call text.AppendLine($"activationMethod{vbTab}{activationMethod}")
        Call text.AppendLine($"collisionEnergy{vbTab}{collisionEnergy}")
        Call text.AppendLine($"centroided{vbTab}{centroided}")
        Call text.AppendLine($"basePeakMz{vbTab}{basePeakMz}")
        Call text.AppendLine($"maxIntensity{vbTab}{maxIntensity}")
        Call text.AppendLine($"totalIons{vbTab}{totalIons}")
        Call text.AppendLine($"n_fragments{vbTab}{n_fragments}")
        Call text.AppendLine($"lowMass{vbTab}{lowMass}")
        Call text.AppendLine($"highMass{vbTab}{highMass}")

        Call Clipboard.Clear()
        Call Clipboard.SetText(text.ToString)
    End Sub
End Class
