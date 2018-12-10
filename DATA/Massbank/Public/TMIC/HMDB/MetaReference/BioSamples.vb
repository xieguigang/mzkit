Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace TMIC.HMDB.Repository

    Public Enum BioSamples
        All = 0
        Bloods = 2
        MetabolicSystems = 4
        OtherFluids = 8
        Biomass = 16
    End Enum

    Public Module BioSampleExtensions

        ReadOnly samples As New Dictionary(Of BioSamples, String()) From {
            {BioSamples.Biomass, {"Feces"}},
            {BioSamples.Bloods, {"Blood", "serum", "plasma", "blood plasma"}},
            {BioSamples.MetabolicSystems, {"Saliva", "Urine", "Sweat"}},
            {BioSamples.OtherFluids, {"Cerebrospinal Fluid (CSF)", "Breast Milk"}}
        }

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ParseSampleType(type As String) As BioSamples
            Return Enums(Of BioSamples).Where(Function(key) key.Description.TextEquals(type)).FirstOrDefault
        End Function

        <Extension>
        Public Function GetSampleLocations(type As BioSamples) As String()
            If type = BioSamples.All Then
                Return Enums(Of BioSamples) _
                    .Where(Function(key)
                               Return samples.ContainsKey(key)
                           End Function) _
                    .Select(Function(key) samples(key)) _
                    .IteratesALL _
                    .ToArray
            Else
                Dim list As New List(Of String)

                If type.HasFlag(BioSamples.Bloods) Then list += samples(BioSamples.Bloods)
                If type.HasFlag(BioSamples.Biomass) Then list += samples(BioSamples.Biomass)
                If type.HasFlag(BioSamples.MetabolicSystems) Then list += samples(BioSamples.MetabolicSystems)
                If type.HasFlag(BioSamples.OtherFluids) Then list += samples(BioSamples.OtherFluids)

                Return list
            End If
        End Function
    End Module
End Namespace