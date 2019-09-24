Imports System.Runtime.CompilerServices
Imports System.Text

Namespace NCBI.PubChem.PCCompound

    <HideModuleName>
    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadFromXml(xml As String) As Compound
            Return Compound.LoadFromXml(xml)
        End Function

        Friend Function TrimXml(xml As String) As String
            With New StringBuilder(xml)
                Call .Replace("PC-Compound_", "")
                Call .Replace("PC-Count_", "")
                Call .Replace("PC-CompoundType_", "")
                Call .Replace("PC-Atoms_", "")
                Call .Replace("PC-AtomInt_", "")
                Call .Replace("PC-Bonds_", "")
                Call .Replace("PC-StereoTetrahedral_", "")
                Call .Replace("PC-Coordinates_", "")
                Call .Replace("PC-Conformer_", "")
                Call .Replace("PC-DrawAnnotations_", "")

                Return .ToString
            End With
        End Function
    End Module
End Namespace