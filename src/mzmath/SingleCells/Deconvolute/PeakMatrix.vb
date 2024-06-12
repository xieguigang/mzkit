#Region "Microsoft.VisualBasic::ea7a487c91c8973c1999d2ea6988f342, mzmath\SingleCells\Deconvolute\PeakMatrix.vb"

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

    '   Total Lines: 109
    '    Code Lines: 78 (71.56%)
    ' Comment Lines: 16 (14.68%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (13.76%)
    '     File Size: 4.18 KB


    '     Module PeakMatrix
    ' 
    '         Function: CreateMatrix, deconvoluteMatrix, deconvoluteMatrixParallel, (+2 Overloads) ExportScans
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math

Namespace Deconvolute

    ''' <summary>
    ''' helper function for create matrix from a ms rawdata object
    ''' </summary>
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

            Dim massVals As MassWindow()

            If mzSet.IsNullOrEmpty Then
                massVals = GetMzIndex(
                    raw:=raw,
                    mzdiff:=mzdiff, freq:=freq,
                    fast:=fastBin,
                    verbose:=verbose
                )
            Else
                massVals = mzSet _
                    .Select(Function(mzi) New MassWindow(mzi)) _
                    .ToArray
            End If

            Dim mzIndex As New MzPool(mzSet)
            Dim matrix As PixelData() = raw _
                .deconvoluteMatrixParallel(mzSet.Length, mzIndex) _
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
            Dim mzIndex As New MzPool(mzSet)
            Dim len As Integer = mzSet.Length

            For Each scan As ScanMS1 In Tqdm.Wrap(raw.MS)
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
            Dim mzIndex As New MzPool(mzSet)
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
        Private Function deconvoluteMatrixParallel(raw As IMZPack, len As Integer, mzIndex As MzPool) As IEnumerable(Of PixelData)
            Return raw.MS _
                .AsParallel _
                .Select(Function(scan)
                            Return scan.deconvoluteMatrix(len, mzIndex)
                        End Function)
        End Function

        <Extension>
        Private Function deconvoluteMatrix(scan As ScanMS1, len As Integer, mzIndex As MzPool) As PixelData
            Dim xy As Point = scan.GetMSIPixel
            Dim v As Double() = Math.DeconvoluteScan(scan.mz, scan.into, len, mzIndex)

            Return New PixelData With {
               .X = xy.X,
               .Y = xy.Y,
               .intensity = v
            }
        End Function
    End Module
End Namespace
