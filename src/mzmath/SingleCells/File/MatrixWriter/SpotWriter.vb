Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute

Public Class SpotWriter

    ReadOnly bin As BinaryWriter
    ReadOnly spot_index As New List(Of (Integer, Integer, Integer, Long))
    ReadOnly label_index As New List(Of (String, Long))

    ''' <summary>
    ''' get current stream position
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property sPos As Long
        Get
            Return bin.BaseStream.Position
        End Get
    End Property

    Sub New(bin As BinaryWriter)
        Me.bin = bin
    End Sub

    Public Sub AddSpot(spot As PixelData)
        Dim label As String = If(spot.label, "")
        Dim pos As Long = sPos

        Call spot_index.Add((spot.X, spot.Y, spot.Z, pos))
        Call label_index.Add((label, pos))

        Call bin.Write(spot.X)
        Call bin.Write(spot.Y)
        Call bin.Write(spot.Z)
        Call bin.Write(label)

        ' intensity vector size equals to the mz features
        For Each i As Double In spot.intensity
            Call bin.Write(i)
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSpotOffSets() As IEnumerable(Of (Integer, Integer, Integer, Long))
        Return spot_index
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetLabelOffsetIndex() As IEnumerable(Of (String, Long))
        Return label_index
    End Function

End Class