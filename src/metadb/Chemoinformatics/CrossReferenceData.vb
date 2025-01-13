Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math

Public Module CrossReferenceData

    Public Iterator Function UniqueGroups(Of C As ICrossReference, T As IMetabolite(Of C))(list As IEnumerable(Of T)) As IEnumerable(Of NamedCollection(Of T))
        Dim masses = list.GroupBy(Function(a) a.ExactMass, offsets:=0.1).ToArray

        For Each unique In MakeGroups(Of C, T)(masses)
            Yield unique
        Next
    End Function

    Private Iterator Function MakeGroups(Of C As ICrossReference, T As IMetabolite(Of C))(massGroup As IEnumerable(Of T)) As IEnumerable(Of NamedCollection(Of T))

    End Function

End Module
