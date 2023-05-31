#Region "Microsoft.VisualBasic::070951941072d44d921539945862df2c, mzkit\src\assembly\Comprehensive\MsImaging\PeakMatrix.vb"

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

    '   Total Lines: 68
    '    Code Lines: 59
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 2.98 KB


    '     Module PeakMatrix
    ' 
    '         Function: AlignMzPeaks, TopIonsPeakMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

Namespace MsImaging

    Public Module PeakMatrix

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

        <Extension>
        Public Function TopIonsPeakMatrix(raw As mzPack,
                                          Optional topN As Integer = 3,
                                          Optional tolerance As String = "da:0.05") As IEnumerable(Of DataSet)

            Dim mzErr As Tolerance = Ms1.Tolerance.ParseScript(tolerance)
            Dim topPeaks = raw.MS _
                .AsParallel _
                .Select(Function(scan)
                            Dim pid As String = $"{scan.meta!x},{scan.meta!y}"
                            Dim topIons As ms2() = scan _
                                .GetMs _
                                .ToArray _
                                .Centroid(mzErr, New RelativeIntensityCutoff(0)) _
                                .OrderByDescending(Function(i) i.intensity) _
                                .Take(topN) _
                                .ToArray

                            Return (pid, topIons)
                        End Function) _
                .ToArray

            Return topPeaks.AlignMzPeaks(mzErr, 0, Function(i) i.topIons, Function(pixel) pixel.pid)
        End Function
    End Module
End Namespace
