Imports System.Runtime.CompilerServices
Imports SMRUCC.MassSpectrum.DATA.File

Namespace NCBI.PubChem

    <HideModuleName> Public Module SDFExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CID(sdf As SDF) As Long
            Return Long.Parse(sdf.ID)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ChemicalProperties(sdf As SDF) As ChemicalDescriptor
            Return New ChemicalDescriptor(sdf.MetaData)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Name(sdf As SDF) As String
            Return sdf.MetaData.TryGetValue("PUBCHEM_IUPAC_NAME", [default]:={}).FirstOrDefault
        End Function
    End Module
End Namespace