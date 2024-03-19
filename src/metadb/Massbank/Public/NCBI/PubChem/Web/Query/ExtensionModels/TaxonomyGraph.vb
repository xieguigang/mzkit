Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.json

Namespace NCBI.PubChem.ExtensionModels

    Public Class TaxonomyGraph

        Public Property cid As String
        Public Property taxid As String
        Public Property taxname As String
        Public Property dois As String()
        Public Property wikidatacmpd As String
        Public Property wikidatataxon As String
        Public Property wikidataref As String()
        Public Property cmpdname As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(str As String) As TaxonomyGraph()
            Return JsonParser _
                .Parse(str, strictVectorSyntax:=False) _
                .CreateObject(GetType(TaxonomyGraph()), decodeMetachar:=True)
        End Function

    End Class
End Namespace