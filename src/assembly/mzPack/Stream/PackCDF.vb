#Region "Microsoft.VisualBasic::a85c593ca15aac34d048b269a5681095, mzkit\src\assembly\mzPack\Stream\PackCDF.vb"

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

    '   Total Lines: 120
    '    Code Lines: 90
    ' Comment Lines: 17
    '   Blank Lines: 13
    '     File Size: 4.52 KB


    ' Module PackCDF
    ' 
    '     Function: ReadPackData, UnionTimeSeq
    ' 
    '     Sub: SavePackData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader.ChromatogramReader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' 主要是为了减少对原始数据文件的频繁读取操作而创建的色谱数据缓存模块
''' </summary>
Public Module PackCDF

    <Extension>
    Public Function UnionTimeSeq(overlaps As ChromatogramOverlap, Optional dt As Double = -1) As Double()
        Dim union As Double() = overlaps.overlaps _
            .Values _
            .Select(Function(sig) sig.scan_time) _
            .IteratesALL _
            .OrderBy(Function(x) x) _
            .ToArray
        Dim avgLen As Integer = overlaps.overlaps _
            .Select(Function(c)
                        Return c.Value.scan_time.Length
                    End Function) _
            .Average
        Dim rtmin As Double = union.Min
        Dim rtmax As Double = union.Max

        If dt <= 0 Then
            dt = (rtmax - rtmin) / avgLen
        End If

        Return seq(union.Min, union.Max, by:=dt).ToArray
    End Function

    ''' <summary>
    ''' write cache
    ''' </summary>
    ''' <param name="overlaps">多个文件或者多个离子通道的色谱图数据集合</param>
    ''' <param name="file"></param>
    ''' <remarks>
    ''' |TIC|BPC|
    ''' </remarks>
    <Extension>
    Public Sub SavePackData(overlaps As ChromatogramOverlap, file As Stream)
        Dim scan_time As Double() = overlaps.UnionTimeSeq
        Dim line As doubles = scan_time
        Dim length As New Dimension With {.name = "scan_length", .size = scan_time.Length}
        Dim dataLen As New Dimension With {.name = "data_length", .size = scan_time.Length * 2}

        Using cdf As New CDFWriter(file)
            Dim allNames As chars = overlaps.overlaps _
                .Keys _
                .ToArray _
                .GetJson
            Dim strSize As New Dimension With {
                .name = "name_data",
                .size = allNames.Length
            }
            Dim formatAttr As New attribute With {
                .name = "format",
                .type = CDFDataTypes.CHAR,
                .value = "JSON"
            }

            ' add X axis
            Call cdf _
                .Dimensions(length, dataLen) _
                .AddVariable("scan_time", line, length.name)
            Call cdf.AddVariable("signalNames", allNames, strSize, {formatAttr})

            For Each chr As NamedValue(Of Chromatogram) In overlaps.EnumerateSignals
                Dim TIC As GeneralSignal = chr.Value.GetSignal(isbpc:=False)
                Dim BPC As GeneralSignal = chr.Value.GetSignal(isbpc:=True)

                line = Resampler _
                    .CreateSampler(TIC)(scan_time) _
                    .JoinIterates(Resampler.CreateSampler(BPC)(scan_time)) _
                    .ToArray

                cdf.AddVariable(chr.Name, line, dataLen.name)
            Next
        End Using
    End Sub

    ''' <summary>
    ''' read cache
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ReadPackData(file As Stream) As ChromatogramOverlap
        Using cdf As New netCDFReader(file)
            Dim nameStr As String = DirectCast(cdf.getDataVariable("signalNames"), chars)
            Dim names As String() = nameStr.LoadJSON(Of String())
            Dim scan_time As Double() = CType(cdf.getDataVariable("scan_time"), doubles)
            Dim overlaps As New ChromatogramOverlap With {
                .overlaps = New Dictionary(Of String, Chromatogram)
            }
            Dim joinData As Double()()

            For Each name As String In names
                joinData = DirectCast(cdf.getDataVariable(name), doubles).Split(scan_time.Length)
                overlaps(name) = New Chromatogram With {
                    .scan_time = scan_time,
                    .TIC = joinData(Scan0),
                    .BPC = joinData(1)
                }
            Next

            Return overlaps
        End Using
    End Function
End Module
