Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
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
        Public Function CreateMatrix(raw As mzPack,
                                     Optional mzdiff As Double = 0.001,
                                     Optional freq As Double = 0.001) As MzMatrix

            Dim mzSet As Double() = GetMzIndex(raw:=raw, mzdiff:=mzdiff, freq:=freq)
            Dim mzIndex As New BlockSearchFunction(Of (mz As Double, Integer))(
                data:=mzSet.Select(Function(mzi, i) (mzi, i)),
                eval:=Function(i) i.mz,
                tolerance:=1,
                fuzzy:=True
            )
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
        Public Iterator Function ExportScans(Of T As {New, INamedValue, IVector})(raw As mzPack, mzSet As Double()) As IEnumerable(Of T)
            Dim mzIndex As New BlockSearchFunction(Of (mz As Double, Integer))(
                data:=mzSet.Select(Function(mzi, i) (mzi, i)),
                eval:=Function(i) i.mz,
                tolerance:=1,
                fuzzy:=True
            )
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

        <Extension>
        Private Iterator Function deconvoluteMatrix(raw As mzPack,
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