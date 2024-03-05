
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem.ExtensionModels

    ''' <summary>
    ''' reaction.json data which downloaded from pubchem
    ''' </summary>
    Public Class ReactionGraph

        Public Property name As String
        Public Property source As String
        Public Property externalid As String
        Public Property url As String
        Public Property definition As String
        Public Property reaction As String
        Public Property control As String
        Public Property cids As UInteger()
        Public Property protacxns As String()
        Public Property geneids As UInteger()
        Public Property taxid As UInteger
        Public Property taxname As String
        Public Property ecs As String()
        Public Property cidsreactant As UInteger()
        Public Property cidsproduct As UInteger()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(str As String) As ReactionGraph()
            Return str.LoadJSON(Of ReactionGraph())
        End Function

    End Class
End Namespace