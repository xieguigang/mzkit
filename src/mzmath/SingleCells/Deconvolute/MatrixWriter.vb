﻿Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Linq

Public Class MatrixWriter

    ReadOnly m As MzMatrix

    Public Const magic As String = "single_cell"

    Sub New(m As MzMatrix)
        Me.m = m
    End Sub

    Public Sub Write(s As Stream)
        Dim bin As New BinaryWriter(s, encoding:=Encoding.ASCII)

        Call bin.Write(magic.Select(Function(c) CByte(Asc(c))).ToArray, Scan0, magic.Length)
        Call bin.Write(m.tolerance)
        Call bin.Write(m.featureSize)

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

        Dim spot_index As New List(Of (Integer, Integer, Long))
        Dim label_index As New List(Of (String, Long))

        For Each spot As PixelData In m.matrix.SafeQuery
            Dim label As String = If(spot.label, "")

            Call spot_index.Add((spot.X, spot.Y, s.Position))
            Call label_index.Add((label, s.Position))

            Call bin.Write(spot.X)
            Call bin.Write(spot.Y)
            Call bin.Write(label)

            ' intensity vector size equals to the mz features
            For Each i As Double In spot.intensity
                Call bin.Write(i)
            Next
        Next

        ' write spot index
        Dim offset1 As Long = s.Position

        For Each index In spot_index
            Call bin.Write(index.Item1)
            Call bin.Write(index.Item2)
            Call bin.Write(index.Item3)
        Next

        ' write label index
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