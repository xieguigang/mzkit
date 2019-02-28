Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace NCBI.PubChem

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationString(section As Section, key$) As String
            Return section.getInformation(key)?.StringValue
        End Function

        <Extension>
        Private Function getInformation(section As Section, key$) As Information
            If section Is Nothing Then
                Return Nothing
            Else
                Return section _
                    .Information _
                    .SafeQuery _
                    .FirstOrDefault(Function(i) i.Name = key)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationTable(section As Section, key$) As Table
            Return section.getInformation(key)?.Table
        End Function
    End Module
End Namespace