Namespace Formula.MS

    Public NotInheritable Class MemorySheet

        Public Shared Iterator Function GetDefault() As IEnumerable(Of ProductIon)
            Yield New ProductIon("[COH]+", "Aldehyde", "")
            Yield New ProductIon("[NO]+", "Nitro", "")
            Yield New ProductIon("[CH2OH]+", "Alcohol", "Aliphatic")
            Yield New ProductIon("[C3H3]+", "Aromatic", "")
            Yield New ProductIon("[CH2CNH]+", "Nitrile", "")
            Yield New ProductIon("[C2H4O]+", "Aldehyde", "McLafferty rearrangement")
            Yield New ProductIon("[OCOH]+", "Carboxylic acid or ester", "")
            Yield New ProductIon("[C4H3]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C3H3O]+", "Ketone", "Cyclic, saturated")
            Yield New ProductIon("[COOCH3]+", "Methyl ester", "")
            Yield New ProductIon("[C5H5]+", "Aromatic", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
            Yield New ProductIon("", "", "")
        End Function
    End Class
End Namespace