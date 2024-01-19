Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute

Public Class SpotWriter

    ReadOnly bin As BinaryWriter
    ReadOnly spot_index As New List(Of SpatialIndex)
    ReadOnly label_index As New List(Of (String, Long))

    ''' <summary>
    ''' get current stream position
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property sPos As Long
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return bin.BaseStream.Position
        End Get
    End Property

    Sub New(bin As BinaryWriter)
        Me.bin = bin
    End Sub

    ''' <summary>
    ''' for a better perfermance of binary data file seek operation
    ''' the scan data is in structrue of:
    ''' 
    ''' ```
    '''   x,  y,  z,intensity,label_string
    ''' i32,i32,i32,  f64 * n,string
    ''' ```
    ''' 
    ''' so, for seek a ion intensity value will be in fast speed
    ''' </summary>
    ''' <remarks>
    ''' <see cref="MatrixReader.LoadCurrentSpot()"/>
    ''' </remarks>
    Public Sub AddSpot(spot As PixelData)
        Dim label As String = If(spot.label, "")
        Dim pos As Long = sPos

        Call spot_index.Add(New SpatialIndex(spot.X, spot.Y, spot.Z, pos))
        Call label_index.Add((label, pos))

        Call bin.Write(spot.X)
        Call bin.Write(spot.Y)
        Call bin.Write(spot.Z)

        ' intensity vector size equals to the mz features
        For Each i As Double In spot.intensity
            Call bin.Write(i)
        Next

        Call bin.Write(label)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSpotOffSets() As IEnumerable(Of SpatialIndex)
        Return spot_index
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetLabelOffsetIndex() As IEnumerable(Of (String, Long))
        Return label_index
    End Function

End Class