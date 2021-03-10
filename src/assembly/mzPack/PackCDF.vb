Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Module PackCDF


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="overlaps"></param>
    ''' <param name="file"></param>
    ''' <remarks>
    ''' |TIC|BPC|
    ''' </remarks>
    <Extension>
    Public Sub SavePackData(overlaps As ChromatogramOverlap, file As Stream)
        Dim scan_time As Double() = overlaps.overlaps _
            .Values _
            .Select(Function(sig) sig.scan_time) _
            .IteratesALL _
            .OrderBy(Function(x) x) _
            .ToArray
        Dim line As New CDFData With {.numerics = scan_time}
        Dim length As New Dimension With {.name = "scan_length", .size = scan_time.Length}
        Dim dataLen As New Dimension With {.name = "data_length", .size = scan_time.Length * 2}

        Using cdf As New CDFWriter(file)
            ' add X axis
            Call cdf _
                .Dimensions(length, dataLen) _
                .AddVariable("scan_time", line, length.name)

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

    <Extension>
    Public Function ReadPackData(file As Stream) As ChromatogramOverlap

    End Function
End Module
