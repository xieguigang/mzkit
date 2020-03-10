#Region "Microsoft.VisualBasic::2c3c7f942a5d4d0dd194433a741a36d7, src\mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\WiffRaw.vb"

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

'     Module WiffRaw
' 
'         Function: Scan, ScanPeakTable
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace MRM

    Public Module WiffRaw

        Public Function ScanPeakTable(mzML$, ions As IonPair(), tolerance As Tolerance, timeWindowSize#, angleThreshold#, baselineQuantile#, rtshifts As Dictionary(Of String, Double),
                                      Optional peakAreaMethod As PeakArea.Methods = PeakArea.Methods.NetPeakSum,
                                      Optional TPAFactors As Dictionary(Of String, Double) = Nothing) As DataSet()

            ' 得到当前的这个原始文件之中的峰面积数据
            Dim TPA() = mzML.ScanTPA(
                ionpairs:=ions,
                peakAreaMethod:=peakAreaMethod,
                TPAFactors:=TPAFactors,
                tolerance:=tolerance,
                timeWindowSize:=timeWindowSize,
                angleThreshold:=angleThreshold,
                baselineQuantile:=baselineQuantile,
                rtshifts:=rtshifts
            )
            Dim peaktable As DataSet() = TPA _
                .Select(Function(ion)
                            Return New DataSet With {
                                .ID = ion.name,
                                .Properties = New Dictionary(Of String, Double) From {
                                    {"rtmin", ion.peakROI.Min},
                                    {"rtmax", ion.peakROI.Max},
                                    {"area", ion.area},
                                    {"baseline", ion.baseline},
                                    {"maxinto", ion.maxPeakHeight}
                                }
                            }
                        End Function) _
                .ToArray

            Return peaktable
        End Function

        <Extension>
        Public Function CreateRtShiftMatrix(rtshifts As RTAlignment()) As Dictionary(Of String, Dictionary(Of String, Double))
            Return rtshifts _
               .Select(Function(i)
                           Return i.CalcRtShifts.Select(Function(shift) (i.ion.target.accession, shift))
                       End Function) _
               .IteratesALL _
               .GroupBy(Function(shift) shift.shift.Name) _
               .ToDictionary(Function(sample) sample.Key,
                             Function(sample)
                                 Return sample _
                                     .ToDictionary(Function(ion) ion.accession,
                                                   Function(ion)
                                                       Return ion.shift.Value
                                                   End Function)
                             End Function)
        End Function

        ''' <summary>
        ''' 从原始数据之中扫描峰面积数据，返回来的数据集之中的<see cref="DataSet.ID"/>是HMDB代谢物编号
        ''' </summary>
        ''' <param name="mzMLRawFiles">``*.wiff``，转换之后的结果文件夹，其中标准曲线的数据都是默认使用``L数字``标记的。</param>
        ''' <param name="ions">包括离子对的定义数据以及浓度区间</param>
        ''' <param name="TPAFactors">
        ''' ``{<see cref="Standards.ID"/>, <see cref="Standards.Factor"/>}``，这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
        ''' </param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function Scan(mzMLRawFiles$(),
                             ions As IonPair(),
                             peakAreaMethod As PeakArea.Methods,
                             TPAFactors As Dictionary(Of String, Double),
                             tolerance As Tolerance,
                             timeWindowSize#,
                             angleThreshold#,
                             baselineQuantile#,
                             rtshifts As RTAlignment(),
                             Optional ByRef refName$() = Nothing,
                             Optional removesWiffName As Boolean = False) As DataSet()

            Dim ionTPAs As New Dictionary(Of String, Dictionary(Of String, Double))
            Dim refNames As New List(Of String)
            Dim level$
            Dim wiffName$ = mzMLRawFiles _
                .Select(Function(path) path.ParentDirName) _
                .GroupBy(Function(name) name) _
                .OrderByDescending(Function(name) name.Count) _
                .First _
                .Key

            Call $"The wiff raw file name is: {wiffName}".__DEBUG_ECHO

            For Each ion As IonPair In ions
                ionTPAs(ion.accession) = New Dictionary(Of String, Double)
            Next

            Dim shiftMatrix = rtshifts.CreateRtShiftMatrix

            For Each file As String In mzMLRawFiles
                ' 得到当前的这个原始文件之中的峰面积数据
                Dim ionShifts = shiftMatrix.TryGetValue(file.BaseName)
                Dim TPA() = file.ScanTPA(
                    ionpairs:=ions,
                    peakAreaMethod:=peakAreaMethod,
                    TPAFactors:=TPAFactors,
                    tolerance:=tolerance,
                    timeWindowSize:=timeWindowSize,
                    angleThreshold:=angleThreshold,
                    baselineQuantile:=baselineQuantile,
                    rtshifts:=ionShifts
                )

                refNames += file.BaseName
                level$ = file.BaseName

                If removesWiffName Then
                    level = level.Replace(wiffName, "").Trim("-"c, " "c)
                    ' level = r.Replace(level, "^\d+[-]", "", RegexICMul).Trim("-"c, " "c)
                End If

                For Each ion In TPA
                    ionTPAs(ion.name).Add(level, ion.area)
                Next
            Next

            refName = refNames.ToArray

            Return ionTPAs _
                .Select(Function(ion)
                            Return New DataSet With {
                                .ID = ion.Key,
                                .Properties = ion.Value
                            }
                        End Function) _
                .ToArray
        End Function
    End Module

End Namespace
