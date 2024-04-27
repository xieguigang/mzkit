#Region "Microsoft.VisualBasic::35de912963eca3f04214e34f51c8eb2b, G:/mzkit/src/assembly/assembly//MarkupData/imzML/XML/ScanData/ScanData.vb"

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

    '   Total Lines: 62
    '    Code Lines: 43
    ' Comment Lines: 7
    '   Blank Lines: 12
    '     File Size: 2.25 KB


    '     Class ScanData
    ' 
    '         Properties: IntPtr, mass, MzPtr, polarity, spotID
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace MarkupData.imzML

    ''' <summary>
    ''' a 2D single pixel pointer data in the generated image
    ''' </summary>
    Public Class ScanData : Inherits PixelScanIntensity
        Implements ImageScan

        Public Property MzPtr As ibdPtr Implements ImageScan.MzPtr
        Public Property IntPtr As ibdPtr Implements ImageScan.IntPtr

        Public Property spotID As Integer
        Public Property polarity As IonModes

        ''' <summary>
        ''' [lowest observed m/z, highest observed m/z]
        ''' </summary>
        ''' <returns></returns>
        Public Property mass As DoubleRange

        Sub New(scan As spectrum)
            If Not scan.cvParams Is Nothing Then
                Dim lowMass As Double = scan.FindVocabulary("MS:1000528")?.value
                Dim highMass As Double = scan.FindVocabulary("MS:1000527")?.value

                totalIon = Double.Parse(scan.cvParams.KeyItem("total ion current")?.value)
                mass = New DoubleRange(lowMass, highMass)
            End If

            Dim scanInfo As mzML.scan = scan.scanList.scans(0)

            polarity = scan.polarity
            spotID = scan.index
            x = Integer.Parse(scanInfo.cvParams.KeyItem("position x")?.value)
            y = Integer.Parse(scanInfo.cvParams.KeyItem("position y")?.value)
            MzPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(Scan0))
            IntPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(1))
        End Sub

        <DebuggerStepThrough>
        Sub New()
        End Sub

        <DebuggerStepThrough>
        Sub New(copy As ScanData)
            totalIon = copy.totalIon
            x = copy.x
            y = copy.y
            MzPtr = copy.MzPtr
            IntPtr = copy.IntPtr
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{x}, {y}] {totalIon.ToString("F3")}"
        End Function
    End Class
End Namespace
