Imports System.IO
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

        Call bin.Write(m.matrix.TryCount)

        For Each spot As PixelData In m.matrix.SafeQuery
            Call bin.Write(spot.X)
            Call bin.Write(spot.Y)
            Call bin.Write(If(spot.label, ""))

            ' intensity vector size equals to the mz features
            For Each i As Double In spot.intensity
                Call bin.Write(i)
            Next
        Next

        Call bin.Flush()
    End Sub
End Class
