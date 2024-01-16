Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' helper module for write the matrix as binary data
''' </summary>
Public Class MatrixWriter

    ''' <summary>
    ''' the rawdata matrix object to save to file
    ''' </summary>
    ReadOnly m As MzMatrix

    ''' <summary>
    ''' the binary file magic header string
    ''' </summary>
    Public Const magic As String = "single_cell"

    Sub New(m As MzMatrix)
        Me.m = m
    End Sub

    ''' <summary>
    ''' save the matrix to a file
    ''' </summary>
    ''' <param name="s">target file to save the matrix data</param>
    Public Sub Write(s As Stream)
        Dim bin As New BinaryWriter(s, encoding:=Encoding.ASCII)
        Dim offset As Long = WriteHeader(bin, m.GetHeader)

        ' write index placeholder
        Call bin.Write(0&)
        Call bin.Write(0&)

        Dim writeSpots As New SpotWriter(bin)
        Dim offset1, offset2 As Long

        For Each spot As PixelData In m.matrix.SafeQuery
            Call writeSpots.AddSpot(spot)
        Next

        Call WriteIndex(bin, writeSpots, offset1, offset2)

        Call s.Seek(offset, SeekOrigin.Begin)
        Call bin.Write(offset1)
        Call bin.Write(offset2)
        Call bin.Flush()
    End Sub

    Public Shared Function WriteHeader(bin As BinaryWriter, header As MatrixHeader) As Long
        Dim s As Stream = bin.BaseStream

        Call bin.Write(magic.Select(Function(c) CByte(Asc(c))).ToArray, Scan0, magic.Length)
        Call bin.Write(header.tolerance)
        Call bin.Write(header.featureSize)
        Call bin.Write(header.matrixType)  ' int32

        For Each mzi As Double In header.mz.SafeQuery
            Call bin.Write(mzi)
        Next

        ' save count of the spots
        Call bin.Write(header.numSpots)
        Call bin.Flush()

        Return s.Position
    End Function

    Public Shared Sub WriteIndex(bin As BinaryWriter, tmp As SpotWriter, ByRef offset1 As Long, ByRef offset2 As Long)
        Dim s As Stream = bin.BaseStream

        ' write spatial spot index
        offset1 = s.Position

        For Each index In tmp.GetSpotOffSets
            Call bin.Write(index.Item1)
            Call bin.Write(index.Item2)
            Call bin.Write(index.Item3)
            Call bin.Write(index.Item4)
        Next

        ' write singlecell label index
        offset2 = s.Position

        For Each index In tmp.GetLabelOffsetIndex
            Call bin.Write(index.Item1)
            Call bin.Write(index.Item2)
        Next
    End Sub
End Class

Public Class MatrixHeader

    ''' <summary>
    ''' m/z vector in numeric format of round to digit 4, this ion m/z 
    ''' feature list is generated under the current mass 
    ''' <see cref="tolerance"/>.
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double()

    ''' <summary>
    ''' the script string of the mz diff tolerance for <see cref="mz"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property tolerance As String

    ''' <summary>
    ''' get count of the ion feature size under current mass <see cref="tolerance"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property featureSize As Integer
        Get
            Return mz.TryCount
        End Get
    End Property

    ''' <summary>
    ''' number of the spots
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' number of the rows in <see cref="MzMatrix.matrix"/>
    ''' </remarks>
    Public Property numSpots As Integer

    ''' <summary>
    ''' the matrix data type of current object, value of this property could be one of the flag value:
    ''' 
    ''' 1. <see cref="FileApplicationClass.MSImaging"/> 2d spatial data
    ''' 2. <see cref="FileApplicationClass.MSImaging3D"/> 3d spatial data
    ''' 3. <see cref="FileApplicationClass.SingleCellsMetabolomics"/> single cell matrix data
    ''' </summary>
    ''' <returns></returns>
    Public Property matrixType As FileApplicationClass

End Class

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