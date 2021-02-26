Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports any = Microsoft.VisualBasic.Scripting

Public Class KEGGHandler

    ReadOnly precursorTypes As MzCalculator()
    ReadOnly tolerance As Tolerance
    ReadOnly massIndex As AVLTree(Of MassIndexKey, Compound)

    Sub New(tree As AVLTree(Of MassIndexKey, Compound), tolerance As Tolerance, precursorTypes As MzCalculator())
        Me.tolerance = tolerance
        Me.massIndex = tree
        Me.precursorTypes = precursorTypes
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>
    ''' 函数返回符合条件的kegg代谢物编号
    ''' </returns>
    Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of String)

    End Function

    Public Function CreateIndex(compounds As IEnumerable(Of Compound), types As MzCalculator(), tolerance As Tolerance) As KEGGHandler
        Dim tree As New AVLTree(Of MassIndexKey, Compound)(AddressOf ComparesMass, AddressOf any.ToString)

        For Each compound As Compound In compounds
            For Each type As MzCalculator In types
                Dim index As New MassIndexKey With {
                    .precursorType = type.ToString,
                    .mz = type.CalcMZ(compound.exactMass)
                }

                tree.Add(index, compound, valueReplace:=False)
            Next
        Next

        Return New KEGGHandler(tree, tolerance, types)
    End Function

    Private Shared Function ComparesMass(x As MassIndexKey, b As MassIndexKey) As Integer
        If x.mz > b.mz Then
            Return 1
        ElseIf x.mz < b.mz Then
            Return -1
        Else
            Return 0
        End If
    End Function

End Class

''' <summary>
''' Indexed of target compound by m/z
''' </summary>
Public Structure MassIndexKey

    Dim mz As Double
    Dim precursorType As String

    Public Overrides Function ToString() As String
        Return $"{precursorType} {mz}"
    End Function

End Structure