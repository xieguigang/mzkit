Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace NCBI.PubChem

    Public Module Extensions

        <Extension>
        Public Function GetInformationString(section As Section, key$) As String
            If section Is Nothing Then
                Return ""
            Else
                Return section _
                    .Information _
                    .SafeQuery _
                    .FirstOrDefault(Function(i) i.Name = key) _
                   ?.StringValue
            End If
        End Function
    End Module
End Namespace