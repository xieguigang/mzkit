Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

''' <summary>
''' binary file r/w for <see cref="SingleIonLayer"/>
''' </summary>
Public Module LayerFile

    <Extension>
    Public Sub SaveLayer(layer As SingleIonLayer, file As Stream)
        Dim wr As New BinaryDataWriter(file, Encodings.UTF8)

        Call wr.Write(layer.IonMz)
        Call wr.Write(layer.DimensionSize.Width)
        Call wr.Write(layer.DimensionSize.Height)
        Call wr.Write(0&)
        Call PixelData.GetBuffer(layer.MSILayer, file:=wr)
        Call wr.Flush()
    End Sub

    Public Function ParseLayer(bin As Stream) As SingleIonLayer
        Dim rd As New BinaryDataReader(bin, Encodings.UTF8)
        Dim label As String = rd.ReadString
        Dim size As Integer() = rd.ReadInt32s(count:=2)
        Dim pixels As PixelData()

        rd.ReadInt64()
        pixels = PixelData.Parse(rd).ToArray

        Return New SingleIonLayer With {
            .DimensionSize = New Size(size(0), size(1)),
            .IonMz = label,
            .MSILayer = pixels
        }
    End Function

    <Extension>
    Public Sub SaveMSISummary(layer As MSISummary, file As Stream)
        Dim wr As New BinaryDataWriter(file)
        Dim vec As iPixelIntensity() = layer.rowScans.IteratesALL.ToArray

        wr.Write(layer.size.Width)
        wr.Write(layer.size.Height)
        wr.Write(vec.Length)

        For Each pixel As iPixelIntensity In vec
            Call wr.Write(pixel.x)
            Call wr.Write(pixel.y)
            Call wr.Write(pixel.totalIon)
            Call wr.Write(pixel.basePeakIntensity)
            Call wr.Write(pixel.average)
            Call wr.Write(pixel.basePeakMz)
            Call wr.Write(pixel.min)
            Call wr.Write(pixel.median)
            Call wr.Write(pixel.numIons)
        Next

        Call wr.Flush()
    End Sub

    Public Function LoadSummaryLayer(bin As Stream) As MSISummary
        Dim rd As New BinaryDataReader(bin)
        Dim size As Integer() = rd.ReadInt32s(count:=2)
        Dim n As Integer = rd.ReadInt32

        Return MSISummary.FromPixels(rd.readPixels(n), dims:=New Size(size(0), size(1)))
    End Function

    <Extension>
    Private Iterator Function readPixels(rd As BinaryDataReader, nsize As Integer) As IEnumerable(Of iPixelIntensity)
        For i As Integer = 0 To nsize - 1
            Yield New iPixelIntensity With {
                .x = rd.ReadInt32,
                .y = rd.ReadInt32,
                .totalIon = rd.ReadDouble,
                .basePeakIntensity = rd.ReadDouble,
                .average = rd.ReadDouble,
                .basePeakMz = rd.ReadDouble,
                .min = rd.ReadDouble,
                .median = rd.ReadDouble,
                .numIons = rd.ReadInt32
            }
        Next
    End Function

End Module
