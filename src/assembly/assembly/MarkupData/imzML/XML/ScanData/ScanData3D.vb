#Region "Microsoft.VisualBasic::b880ac0496e6c580787808c91a74eb95, G:/mzkit/src/assembly/assembly//MarkupData/imzML/XML/ScanData/ScanData3D.vb"

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

    '   Total Lines: 35
    '    Code Lines: 23
    ' Comment Lines: 4
    '   Blank Lines: 8
    '     File Size: 1.19 KB


    '     Class ScanData3D
    ' 
    '         Properties: IntPtr, MzPtr, totalIon, x, y
    '                     z
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    Public Class ScanData3D : Implements ImageScan

        ''' <summary>
        ''' TIC
        ''' </summary>
        ''' <returns></returns>
        Public Property totalIon As Double
        Public Property x As Double
        Public Property y As Double
        Public Property z As Double
        Public Property MzPtr As ibdPtr Implements ImageScan.MzPtr
        Public Property IntPtr As ibdPtr Implements ImageScan.IntPtr

        Sub New(scan As spectrum)
            MzPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(Scan0))
            IntPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(1))

            If Not scan.cvParams Is Nothing Then
                totalIon = Double.Parse(scan.cvParams.KeyItem("total ion current")?.value)
            End If

            Call XML.Get3DPositionXYZ(scan, x, y, z)
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{x}, {y}, {z}] {totalIon.ToString("F3")}"
        End Function
    End Class

End Namespace
