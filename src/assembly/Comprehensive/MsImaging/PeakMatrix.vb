#Region "Microsoft.VisualBasic::9cfc2c9915cb19bd719cb0a28cef9e4e, assembly\Comprehensive\MsImaging\PeakMatrix.vb"

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

    '   Total Lines: 95
    '    Code Lines: 57 (60.00%)
    ' Comment Lines: 30 (31.58%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 8 (8.42%)
    '     File Size: 4.07 KB


    '     Module PeakMatrix2
    ' 
    '         Function: AlignMzPeaks, SelectivePeakMatrix, TopIonsPeakMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

Namespace MsImaging

    Public Module PeakMatrix2

        ''' <summary>
        ''' Create a dataset matrix of spot xy id in rows and ions mz features in columns
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="raw"></param>
        ''' <param name="mzErr"></param>
        ''' <param name="cutoff"></param>
        ''' <param name="getPeaks"></param>
        ''' <param name="getSampleId"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function AlignMzPeaks(Of T)(raw As T(),
                                                    mzErr As Tolerance,
                                                    cutoff As Double,
                                                    getPeaks As Func(Of T, ms2()),
                                                    getSampleId As Func(Of T, String)) As IEnumerable(Of DataSet)
            Dim allMz As Double() = raw _
                .Select(getPeaks) _
                .IteratesALL _
                .ToArray _
                .Centroid(mzErr, New RelativeIntensityCutoff(cutoff)) _
                .Select(Function(i) i.mz) _
                .OrderBy(Function(mz) mz) _
                .ToArray
            Dim vec As Dictionary(Of String, Double)

            For Each pixel As T In raw
                vec = allMz _
                    .ToDictionary(Function(mz) mz.ToString,
                                  Function(mz)
                                      Dim matchIon As ms2 = getPeaks(pixel) _
                                          .Where(Function(i) mzErr(i.mz, mz)) _
                                          .FirstOrDefault

                                      If matchIon Is Nothing Then
                                          Return 0
                                      Else
                                          Return matchIon.intensity
                                      End If
                                  End Function)

                Yield New DataSet With {
                    .ID = getSampleId(pixel),
                    .Properties = vec
                }
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="ions"></param>
        ''' <param name="mzErr"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <see cref="MzMatrix.ExportSpatial(Of T)()"/>
        ''' </remarks>
        <Extension>
        Public Function SelectivePeakMatrix(raw As mzPack, ions As Dictionary(Of String, Double), mzErr As Tolerance) As IEnumerable(Of DataSet)
            Dim m As MzMatrix = PeakMatrix.CreateMatrix(raw, mzErr.DeltaTolerance, 0, mzSet:=ions.Values.ToArray)
            Dim ds As IEnumerable(Of DataSet) = m.ExportSpatial(Of DataSet)
            Return ds
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="topN"></param>
        ''' <param name="mzErr"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <see cref="MzMatrix.ExportSpatial(Of T)()"/>
        ''' </remarks>
        <Extension>
        Public Function TopIonsPeakMatrix(raw As mzPack, topN As Integer, mzErr As Tolerance) As IEnumerable(Of DataSet)
            Dim topIons As Double() = raw.GetMzIndex(mzdiff:=mzErr.DeltaTolerance, topN:=topN)
            Dim m = PeakMatrix.CreateMatrix(raw, mzErr.DeltaTolerance, 0, mzSet:=topIons)
            Dim ds As IEnumerable(Of DataSet) = m.ExportSpatial(Of DataSet)
            Return ds
        End Function
    End Module
End Namespace
