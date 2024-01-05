Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO
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

End Module
