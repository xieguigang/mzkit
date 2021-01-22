Imports System.Reflection
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports stdnum = System.Math

Public Class AtomGroupHandler

    Shared ReadOnly alkyl As Dictionary(Of String, Formula)
    Shared ReadOnly ketones As Dictionary(Of String, Formula)

    Shared Sub New()
        alkyl = loadGroup(Of Alkyl)()
        ketones = loadGroup(Of Ketones)()
    End Sub

    Private Shared Function loadGroup(Of T As Class)() As Dictionary(Of String, Formula)
        Return DataFramework.Schema(Of T)(
            flag:=PropertyAccess.Readable,
            nonIndex:=True,
            binds:=BindingFlags.Static Or BindingFlags.Public
        ) _
        .ToDictionary(Function(p) p.Key,
                        Function(p)
                            Return DirectCast(p.Value.GetValue(Nothing, Nothing), Formula)
                        End Function)
    End Function

    Public Shared Function GetByMass(mass As Double) As NamedValue(Of Formula)
        Static all_groups As List(Of KeyValuePair(Of String, Formula)) = New List(Of KeyValuePair(Of String, Formula)) + alkyl + ketones

        For Each group In all_groups
            If stdnum.Abs(group.Value.ExactMass - mass) <= 0.00001 Then
                Return New NamedValue(Of Formula)(group.Key, group.Value)
            End If
        Next

        Return Nothing
    End Function
End Class
