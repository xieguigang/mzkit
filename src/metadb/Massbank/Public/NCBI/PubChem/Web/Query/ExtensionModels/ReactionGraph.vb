
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Serialization.JSON
Imports any = Microsoft.VisualBasic.Scripting

Namespace NCBI.PubChem.ExtensionModels

    ''' <summary>
    ''' reaction.json data which downloaded from pubchem
    ''' </summary>
    Public Class ReactionGraph

        Public Property name As Object
        Public Property source As String
        Public Property externalid As String
        Public Property url As String
        Public Property definition As String
        Public Property reaction As String
        Public Property control As String
        Public Property cids As Object
        Public Property protacxns As Object
        Public Property geneids As Object
        Public Property taxid As UInteger
        Public Property taxname As String
        Public Property ecs As Object
        Public Property cidsreactant As Object
        Public Property cidsproduct As Object

        Public Function GetReactants() As String()
            Return getArray(cidsreactant)
        End Function

        Public Function GetName() As String
            Return getArray(name).JoinBy("/")
        End Function

        Public Function GetProducts() As String()
            Return getArray(cidsproduct)
        End Function

        Private Shared Function getArray(val As Object) As String()
            If val Is Nothing Then
                Return {}
            ElseIf val.GetType.IsArray Then
                Return DirectCast(val, Array) _
                    .AsObjectEnumerator _
                    .Select(Function(o) any.ToString(o)) _
                    .ToArray
            Else
                Return New String() {any.ToString(val)}
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{source}:{externalid}] {definition}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(str As String) As ReactionGraph()
            Return JsonParser _
                .Parse(str, strictVectorSyntax:=False) _
                .CreateObject(GetType(ReactionGraph()), decodeMetachar:=True)
        End Function

    End Class
End Namespace