Imports System.IO
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Namespace NCBI.PubChem.DataSources

    Public Class AnnotationJSON

        Public Shared Iterator Function GetAnnotations(s As Stream) As IEnumerable(Of Annotation)
            Dim json As JsonObject = New JsonParser(New StreamReader(s), False).OpenJSON
            Dim list As JsonArray = DirectCast(json!Annotations, JsonObject)!Annotation

            For Each obj As JsonObject In list
                Yield DirectCast(obj.CreateObject(GetType(Annotation), True), Annotation)
            Next
        End Function
    End Class
End Namespace