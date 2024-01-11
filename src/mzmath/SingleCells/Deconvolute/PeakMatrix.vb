#Region "Microsoft.VisualBasic::ce9fee484b46dfddd8b5affd3d429006, mzkit\src\mzmath\SingleCells\Deconvolute\PeakMatrix.vb"

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

'   Total Lines: 85
'    Code Lines: 62
' Comment Lines: 13
'   Blank Lines: 10
'     File Size: 3.27 KB


'     Module PeakMatrix
' 
'         Function: CreateMatrix, deconvoluteMatrix, ExportScans
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math

Namespace Deconvolute

    Public Module PeakMatrix

        ''' <summary>
        ''' ms-imaging raw data matrix deconvolution
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq"></param>
        ''' <returns></returns>
        Public Function CreateMatrix(raw As IMZPack,
                                     Optional mzdiff As Double = 0.001,
                                     Optional freq As Double = 0.01,
                                     Optional mzSet As Double() = Nothing,
                                     Optional fastBin As Boolean = True,
                                     Optional verbose As Boolean = False) As MzMatrix

            If mzSet.IsNullOrEmpty Then
                mzSet = GetMzIndex(
                    raw:=raw,
                    mzdiff:=mzdiff, freq:=freq,
                    fast:=fastBin,
                    verbose:=verbose
                )
            End If

            Dim mzIndex = mzSet.CreateMzIndex
            Dim matrix As PixelData() = raw _
                .deconvoluteMatrix(mzSet.Length, mzIndex) _
                .ToArray

            Return New MzMatrix With {
                .matrix = matrix,
                .mz = mzSet,
                .tolerance = mzdiff
            }
        End Function

        ''' <summary>
        ''' apply for export single cell metabolism data
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="raw"></param>
        ''' <returns></returns>
        Public Iterator Function ExportScans(Of T As {New, INamedValue, IVector})(raw As IMZPack, mzSet As Double()) As IEnumerable(Of T)
            Dim mzIndex = mzSet.CreateMzIndex
            Dim len As Integer = mzSet.Length

            For Each scan As ScanMS1 In raw.MS
                Dim cellId As String = scan.scan_id
                Dim v As Double() = Math.DeconvoluteScan(scan.mz, scan.into, len, mzIndex)
                Dim cell_scan As New T With {
                    .Data = v,
                    .Key = cellId
                }

                Yield cell_scan
            Next
        End Function

        Public Iterator Function ExportScans(Of T As {New, INamedValue, IVector})(raw As IEnumerable(Of LibraryMatrix), mzSet As Double()) As IEnumerable(Of T)
            Dim mzIndex = mzSet.CreateMzIndex
            Dim len As Integer = mzSet.Length

            For Each scan As LibraryMatrix In raw
                Dim cellId As String = scan.name
                Dim v As Double() = Math.DeconvoluteScan(scan.mz, scan.intensity, len, mzIndex)
                Dim cell_scan As New T With {
                    .Data = v,
                    .Key = cellId
                }

                Yield cell_scan
            Next
        End Function

        <Extension>
        Private Iterator Function deconvoluteMatrix(raw As IMZPack,
                                                    len As Integer,
                                                    mzIndex As BlockSearchFunction(Of (mz As Double, Integer))) As IEnumerable(Of PixelData)
            For Each scan As ScanMS1 In raw.MS
                Dim xy As Point = scan.GetMSIPixel
                Dim v As Double() = Math.DeconvoluteScan(scan.mz, scan.into, len, mzIndex)

                Yield New PixelData With {
                    .X = xy.X,
                    .Y = xy.Y,
                    .intensity = v
                }
            Next
        End Function
    End Module
End Namespace
