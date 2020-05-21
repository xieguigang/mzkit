#Region "Microsoft.VisualBasic::842831d35401ad5d361bb8f01c370612, src\mzmath\TargetedMetabolomics\GCMS\QuantifyAnalysis\ReferenceLibrary.vb"

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

'     Module ReferenceLibrary
' 
'         Function: Create, ExportReferenceROITable
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace GCMS.QuantifyAnalysis

    ''' <summary>
    ''' 构建GCMS的靶向定量的参考标准品库
    ''' </summary>
    Public Module ReferenceLibrary

        ''' <summary>
        ''' 创建靶向GCMS的标准品库
        ''' </summary>
        ''' <param name="cdf$"></param>
        ''' <param name="names$"></param>
        ''' <returns></returns>
        Public Function Create(cdf$, names$(), peakwidth As DoubleRange, Optional angle# = 5, Optional sn_threads# = 2) As (ROIlist As ROI(), ROITable As ROITable())
            Dim gcms As Raw = netCDFReader.Open(cdf).ReadData()
            Dim ROIlist As ROI() = gcms.GetTIC _
                .Shadows _
                .PopulateROI(angleThreshold:=angle, peakwidth:=peakwidth) _
                .ToArray
            Dim result As ROITable() = ROIlist.ExportReferenceROITable(
                raw:=gcms,
                names:=names,
                sn:=sn_threads
            )

            Return (ROIlist, result)
        End Function

        ''' <summary>
        ''' 导出标准品参考的ROI区间列表，用于``GC/MS``自动化定性分析
        ''' </summary>
        ''' <param name="regions"></param>
        ''' <param name="sn">
        ''' 信噪比筛选阈值
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 保留指数的计算：在标准化流程之中，GCMS的出峰顺序保持不变，但是保留时间可能会在不同批次实验间有变化
        ''' 这个时候如果定量用的标准品混合物和样本之中的所检测物质的出峰顺序一致，则可以将标准品混合物之中的
        ''' 第一个出峰的物质和最后一个出峰的物质作为保留指数的参考，在这里假设第一个出峰的物质的保留指数为零，
        ''' 最后一个出峰的物质的保留指数为1000，则可以根据这个区间和rt之间的线性关系计算出保留指数
        ''' </remarks>
        <Extension> Public Function ExportReferenceROITable(regions As ROI(), raw As Raw,
                                                            Optional sn# = 5,
                                                            Optional names$() = Nothing,
                                                            Optional RImax# = 1000) As ROITable()

            With regions.Where(Function(ROI) ROI.snRatio >= sn).ToArray
                Dim refA = .First, refB = .Last
                Dim A = (refA.rt, 0)
                Dim B = (refB.rt, RImax)
                Dim getTitle As Func(Of ROI, Integer, String)

                If names.IsNullOrEmpty Then
                    getTitle = Function(ROI, i) $"#{i + 1}={Fix(ROI.rt)}s"
                Else
                    getTitle = Function(ROI, i)
                                   Return names.ElementAtOrDefault(i, $"#{i + 1}={Fix(ROI.rt)}s")
                               End Function
                End If

                Return .Select(Function(ROI, i)
                                   Return ROI.ConvertAsTabular(raw, ROI.RetentionIndex(A, B), getTitle(ROI, i))
                               End Function) _
                       .ToArray
            End With
        End Function
    End Module
End Namespace
