Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' 主要是为了减少对原始数据文件的频繁读取操作而创建的色谱数据缓存模块
''' </summary>
Public Module PackCDF

    <Extension>
    Private Function UnionTimeSeq(overlaps As ChromatogramOverlap) As Double()
        Dim union As Double() = overlaps.overlaps _
            .Values _
            .Select(Function(sig) sig.scan_time) _
            .IteratesALL _
            .OrderBy(Function(x) x) _
            .ToArray

        Return seq(union.Min, union.Max, 0.1).ToArray
    End Function

    ''' <summary>
    ''' write cache
    ''' </summary>
    ''' <param name="overlaps"></param>
    ''' <param name="file"></param>
    ''' <remarks>
    ''' |TIC|BPC|
    ''' </remarks>
    <Extension>
    Public Sub SavePackData(overlaps As ChromatogramOverlap, file As Stream)
        Dim scan_time As Double() = overlaps.UnionTimeSeq
        Dim line As New CDFData With {.numerics = scan_time}
        Dim length As New Dimension With {.name = "scan_length", .size = scan_time.Length}
        Dim dataLen As New Dimension With {.name = "data_length", .size = scan_time.Length * 2}

        Using cdf As New CDFWriter(file)
            Dim allNames As New CDFData With {
                .chars = overlaps.overlaps _
                    .Keys _
                    .ToArray _
                    .GetJson
            }
            Dim strSize As New Dimension With {
                .name = "name_data",
                .size = allNames.chars.Length
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

                line = New CDFData With {
                    .numerics = Resampler _
                        .CreateSampler(TIC)(scan_time) _
                        .JoinIterates(Resampler.CreateSampler(BPC)(scan_time)) _
                        .ToArray
                }

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
            Dim nameStr As String = cdf.getDataVariable("signalNames").chars
            Dim names As String() = nameStr.LoadJSON(Of String())
            Dim scan_time As Double() = cdf.getDataVariable("scan_time").numerics
            Dim overlaps As New ChromatogramOverlap With {
                .overlaps = New Dictionary(Of String, Chromatogram)
            }
            Dim joinData As Double()()

            For Each name As String In names
                joinData = cdf _
                    .getDataVariable(name).numerics _
                    .Split(scan_time.Length)
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
