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

        ''' <summary>
        ''' 这个拓展函数接受使用``|``分隔的token来进行or运算
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ParseSampleType(type As String) As BioSamples
            Dim list = type.Split("|"c)
            Dim biosample As BioSamples = BioSamples.All
            Dim types As BioSamples() = Enums(Of BioSamples)()

            For Each typeName As String In list
                For Each key As BioSamples In types
                    If key.Description.TextEquals(typeName) Then
                        biosample = biosample Or key
                    End If
                Next
            Next

            Return biosample
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