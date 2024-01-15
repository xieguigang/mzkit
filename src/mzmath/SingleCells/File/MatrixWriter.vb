Imports System.IO
Imports System.Text
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

        Call bin.Write(magic.Select(Function(c) CByte(Asc(c))).ToArray, Scan0, magic.Length)
        Call bin.Write(m.tolerance)
        Call bin.Write(m.featureSize)
        Call bin.Write(m.matrixType)  ' int32

        For Each mzi As Double In m.mz.SafeQuery
            Call bin.Write(mzi)
        Next

        ' save count of the spots
        Call bin.Write(m.matrix.TryCount)
        Call bin.Flush()

        Dim offset As Long = s.Position

        ' write index placeholder
        Call bin.Write(0&)
        Call bin.Write(0&)

        Dim spot_index As New List(Of (Integer, Integer, Integer, Long))
        Dim label_index As New List(Of (String, Long))

        For Each spot As PixelData In m.matrix.SafeQuery
            Dim label As String = If(spot.label, "")

            Call spot_index.Add((spot.X, spot.Y, spot.Z, s.Position))
            Call label_index.Add((label, s.Position))

            Call bin.Write(spot.X)
            Call bin.Write(spot.Y)
            Call bin.Write(spot.Z)
            Call bin.Write(label)

            ' intensity vector size equals to the mz features
            For Each i As Double In spot.intensity
                Call bin.Write(i)
            Next
        Next

        ' write spatial spot index
        Dim offset1 As Long = s.Position

        For Each index In spot_index
            Call bin.Write(index.Item1)
            Call bin.Write(index.Item2)
            Call bin.Write(index.Item3)
            Call bin.Write(index.Item4)
        Next

        ' write singlecell label index
        Dim offset2 As Long = s.Position

        For Each index In label_index
            Call bin.Write(index.Item1)
            Call bin.Write(index.Item2)
        Next

        Call s.Seek(offset, SeekOrigin.Begin)
        Call bin.Write(offset1)
        Call bin.Write(offset2)
        Call bin.Flush()
    End Sub
End Class
