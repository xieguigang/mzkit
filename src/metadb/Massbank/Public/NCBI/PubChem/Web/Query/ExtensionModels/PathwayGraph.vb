Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.json

Namespace NCBI.PubChem.ExtensionModels

    Public Class PathwayGraph

        Public Property taxname As String
        Public Property pwacc As String
        Public Property name As String
        Public Property pwtype As String
        Public Property category As String
        Public Property url As String
        Public Property source As String
        Public Property srcid As String
        Public Property externalid As String
        Public Property extid As String
        Public Property taxid As String
        Public Property core As String
        Public Property cids As String()
        Public Property geneids As String()
        Public Property protacxns As String()
        Public Property ecs As String()
        Public Property pmids As String()
        Public Property annotation As String()

        Public Overrides Function ToString() As String
            Return $"{pwacc}: {name}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(str As String) As PathwayGraph()
            Return JsonParser _
                .Parse(str, strictVectorSyntax:=False) _
                .CreateObject(GetType(PathwayGraph()), decodeMetachar:=True)
        End Function
    End Class
End Namespace