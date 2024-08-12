Namespace Formula.MS

    Public NotInheritable Class MemorySheet

        Public Shared Iterator Function GetDefault() As IEnumerable(Of ProductIon)
            Yield New ProductIon("[COH]+", "Aldehyde", "")
        End Function
    End Class
End Namespace