Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace NCBI.PubChem

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationString(section As Section, key$) As String
            Dim info = section.getInformation(key)

            If info Is Nothing Then
                Return ""
            Else
                Return info.Value.StringWithMarkup.First.String
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationStrings(section As Section, key$) As String()
            Dim info = section.getInformation(key)

            If info Is Nothing Then
                Return {}
            Else
                Return info.InfoValue
            End If
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

        <Extension>
        Friend Function GetHMDBId(refs As Reference()) As String
            Return refs.SafeQuery _
                .FirstOrDefault(Function(ref)
                                    Return ref.SourceName = PugViewRecord.HMDB
                                End Function) _
               ?.SourceID
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationTable(section As Section, key$) As Table
            Return section.getInformation(key)?.Table
        End Function
    End Module
End Namespace