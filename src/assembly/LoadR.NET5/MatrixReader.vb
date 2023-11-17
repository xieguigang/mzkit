Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]

Public Class MatrixReader : Implements IdataframeReader

    ReadOnly m As MzMatrix
    ReadOnly index As BlockSearchFunction(Of (mz As Double, Integer))
    ReadOnly mzdiff As Tolerance

    Sub New(m As MzMatrix)
        Me.m = m
        Me.mzdiff = Tolerance.ParseScript(m.tolerance)
        Me.index = m.mz.CreateMzIndex
    End Sub

    Public Function getColumn(index As Object, env As Environment) As Object Implements IdataframeReader.getColumn
        Throw New NotImplementedException()
    End Function

    Public Function getRow(index As Object, env As Environment) As Object Implements IdataframeReader.getRow
        Throw New NotImplementedException()
    End Function
End Class
