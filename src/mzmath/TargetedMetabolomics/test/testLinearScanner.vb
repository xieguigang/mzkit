#Region "Microsoft.VisualBasic::ef4ed80d69fe8ea9029a54146d29946b, mzmath\TargetedMetabolomics\test\testLinearScanner.vb"

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

    '   Total Lines: 37
    '    Code Lines: 31 (83.78%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (16.22%)
    '     File Size: 1.52 KB


    ' Module testLinearScanner
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Serialization.JSON

Module testLinearScanner

    Sub Main()
        Dim raw = mzML.LoadChromatogramList("D:\test\111\cal\cal9.mzML").ToArray
        Dim ion As New IonPair With {.name = "3-Indoleacetic acid", .precursor = 173.95, .product = 129.9, .accession = "3-Indoleacetic acid"}

        Call raw.MRMSelector(ion, Tolerance.DeltaMass(0.1)).Plot.Save("D:\test\111\cal\test_IND.png")

        Dim target = raw.MRMSelector(ion, Tolerance.DeltaMass(0.1))
        Dim TIC = target.GetChromatogram
        Dim vector As IVector(Of ChromatogramTick) = TIC.Chromatogram.Shadows
        Dim ROIData As ROI() = vector _
            .PopulateROI(
                rt:={530, 580},
                baselineQuantile:=0.65,
                angleThreshold:=3,
                peakwidth:={8, 30},
                snThreshold:=-1
            ) _
            .ToArray

        Call Console.WriteLine(ROIData.GetJson)

        Pause()
    End Sub
End Module
