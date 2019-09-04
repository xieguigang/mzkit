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

                Return .ToString
            End With
        End Function
    End Module
End Namespace