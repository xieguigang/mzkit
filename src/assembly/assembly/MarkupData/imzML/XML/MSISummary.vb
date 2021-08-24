#Region "Microsoft.VisualBasic::1528bf312888b3eac321e9cbc62c463b, src\assembly\assembly\MarkupData\imzML\XML\MSISummary.vb"

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

    '     Class MSISummary
    ' 
    '         Properties: rowScans, size
    ' 
    '         Function: GetBasePeakMz, GetLayer, GetRowScan, ToString
    ' 
    '     Enum IntensitySummary
    ' 
    '         Average, BasePeak, Total
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace MarkupData.imzML

    Public Class MSISummary

        Public Property rowScans As iPixelIntensity()()
        Public Property size As Size

        Public Function GetBasePeakMz() As LibraryMatrix
            Return New LibraryMatrix With {
                .centroid = False,
                .name = "basePeak M/z",
                .ms2 = rowScans _
                    .IteratesALL _
                    .Select(Function(p)
                                Return New ms2 With {
                                    .mz = p.basePeakMz,
                                    .intensity = p.basePeakIntensity,
                                    .Annotation = $"{p.x},{p.y}"
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        Private Iterator Function GetRowScan(row As iPixelIntensity(), summary As IntensitySummary) As IEnumerable(Of PixelScanIntensity)
            For Each p As iPixelIntensity In row
                Dim into As Double

                Select Case summary
                    Case IntensitySummary.Average : into = p.average
                    Case IntensitySummary.BasePeak : into = p.basePeakIntensity
                    Case IntensitySummary.Total : into = p.totalIon
                    Case Else
                        Throw New NotImplementedException
                End Select

                Yield New PixelScanIntensity With {
                    .totalIon = into,
                    .x = p.x,
                    .y = p.y
                }
            Next
        End Function

        Public Function GetLayer(summary As IntensitySummary) As IEnumerable(Of PixelScanIntensity)
            Return rowScans _
                .Select(Function(row)
                            Return GetRowScan(row, summary)
                        End Function) _
                .IteratesALL
        End Function

        Public Overrides Function ToString() As String
            Return $"[width:={size.Width}, height:={size.Height}]"
        End Function

    End Class

    ''' <summary>
    ''' TIC/BPC/Average
    ''' </summary>
    Public Enum IntensitySummary As Integer
        Total
        BasePeak
        Average
    End Enum

End Namespace
