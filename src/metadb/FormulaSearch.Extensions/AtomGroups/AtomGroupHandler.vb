Imports System.Reflection
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports stdnum = System.Math

Public Class AtomGroupHandler

    Shared ReadOnly alkyl As Dictionary(Of String, Formula) = loadGroup(Of Alkyl)()
    Shared ReadOnly ketones As Dictionary(Of String, Formula) = loadGroup(Of Ketones)()
    Shared ReadOnly amines As Dictionary(Of String, Formula) = loadGroup(Of Amines)()
    Shared ReadOnly alkenyl As Dictionary(Of String, Formula) = loadGroup(Of Alkenyl)()
    Shared ReadOnly others As Dictionary(Of String, Formula) = loadGroup(Of Others)()

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
        Static all_groups As List(Of KeyValuePair(Of String, Formula)) = New List(Of KeyValuePair(Of String, Formula)) + alkyl + ketones + amines + alkenyl + others

        For Each group In all_groups
            If stdnum.Abs(group.Value.ExactMass - mass) <= 0.00001 Then
                Return New NamedValue(Of Formula)(group.Key, group.Value)
            End If
        Next

        Return Nothing
    End Function

    Public Shared Function FindDelta(mz1 As Double, mz2 As Double, Optional ByRef delta As Integer = 0) As NamedValue(Of Formula)
        Dim d As Double = mz1 - mz2
        Dim dmass As Double = stdnum.Abs(d)

        If dmass <= 0.01 Then
            Return Nothing
        End If

        Dim group = GetByMass(dmass)

        delta = stdnum.Sign(d)

        Return group
    End Function
End Class
