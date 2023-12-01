#Region "Microsoft.VisualBasic::1c74a93cb8b649752f8caf6ca2797e45, mzkit\src\assembly\assembly\MarkupData\imzML\XML\ScanData.vb"

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

'   Total Lines: 157
'    Code Lines: 108
' Comment Lines: 18
'   Blank Lines: 31
'     File Size: 5.32 KB


'     Interface IMSIPixel
' 
'         Properties: intensity, x, y
' 
'     Class PixelScanIntensity
' 
'         Properties: totalIon, x, y
' 
'         Function: GetBuffer, GetPoint, Parse
' 
'     Class iPixelIntensity
' 
'         Properties: average, basePeakIntensity, basePeakMz
' 
'         Function: ToString
' 
'     Class ScanData3D
' 
'         Properties: IntPtr, MzPtr, totalIon, x, y
'                     z
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: ToString
' 
'     Interface ImageScan
' 
'         Properties: IntPtr, MzPtr
' 
'     Class ScanData
' 
'         Properties: IntPtr, MzPtr
' 
'         Constructor: (+3 Overloads) Sub New
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    ''' <summary>
    ''' a 2D single pixel pointer data in the generated image
    ''' </summary>
    Public Class ScanData : Inherits PixelScanIntensity
        Implements ImageScan

        Public Property MzPtr As ibdPtr Implements ImageScan.MzPtr
        Public Property IntPtr As ibdPtr Implements ImageScan.IntPtr

        Sub New(scan As spectrum)
            If Not scan.cvParams Is Nothing Then
                totalIon = Double.Parse(scan.cvParams.KeyItem("total ion current")?.value)
            End If

            x = Integer.Parse(scan.scanList.scans(Scan0).cvParams.KeyItem("position x")?.value)
            y = Integer.Parse(scan.scanList.scans(Scan0).cvParams.KeyItem("position y")?.value)
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
