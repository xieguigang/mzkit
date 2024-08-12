Namespace Formula.MS

    Public NotInheritable Class MemorySheet

        ''' <summary>
        ''' https://www.acdlabs.com/blog/common-adduct-and-fragment-ions-in-mass-spectrometry/
        ''' </summary>
        ''' <returns></returns>
        Public Shared Iterator Function GetDefault(Optional nmax As Integer = 3) As IEnumerable(Of ProductIon)
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
            Yield New ProductIon("[C6H5]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C6H6]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C6H7]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C7H7]+", "Aromatic", "[C7H7]+ (Tropylium ion)")
            Yield New ProductIon("[C7H8]+", "Aromatic", "McLafferty rearrangement")
            Yield New ProductIon("[C6H5O]+", "Ether", "Aromatic")
            Yield New ProductIon("[C7H5O]+", "Aldehyde", "Aromatic")
            Yield New ProductIon("[C7H5O]+", "Carboxylic acid or ester", "Aromatic")
            Yield New ProductIon("[C7H5O]+", "Ketone", "Aromatic")

            For n As Integer = 1 To nmax
                Yield New ProductIon(Formula(n, 2 * n + 1), "Alkane", "14n+1")
                Yield New ProductIon(Formula(n, 2 * n - 1, 2), "Carboxylic acid or ester", "14n+31")
                Yield New ProductIon(Formula(n, 2 * n - 1), "Alkane", "14n-1")
                Yield New ProductIon(Formula(n, 2 * n), "Alkane", "16n")
            Next
        End Function

        Private Shared Function Formula(C As Integer, H As Integer) As Dictionary(Of String, Integer)
            Return New Dictionary(Of String, Integer) From {
                {NameOf(C), C},
                {NameOf(H), H}
            }
        End Function

        Private Shared Function Formula(C As Integer, H As Integer, O As Integer) As Dictionary(Of String, Integer)
            Return New Dictionary(Of String, Integer) From {
                {NameOf(C), C},
                {NameOf(H), H},
                {NameOf(O), O}
            }
        End Function

        Public Shared Iterator Function GetDefaultNeutralLoss() As IEnumerable(Of NeutralLoss)
            Yield New NeutralLoss("[M-OH]+", "Carboxylic acid or ester", "")
            Yield New NeutralLoss("[M-H2O]+", "Alcohol/Aldehyde/Carboxylic acid or ester", "Straight chain/Aromatic")
            Yield New NeutralLoss("[M-CO]+", "Alcohol", "Phenol")
            Yield New NeutralLoss("[M-C2H4]+", "Cycloalkane/Aldehyde", "")
            Yield New NeutralLoss("[M-COH]+", "Alcohol", "Phenol")
            Yield New NeutralLoss("[M-CH2CH3]+", "Cycloalkane", "")
            Yield New NeutralLoss("[M-CH2CHO]+", "Aldehyde", "Straight chain")
            Yield New NeutralLoss("[M-CH2CHOH]+", "Aldehyde", "Straight chain")
            Yield New NeutralLoss("[M-CO2H]+", "Carboxylic acid or ester", "")
        End Function
    End Class
End Namespace