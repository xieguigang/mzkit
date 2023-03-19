Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace NCBI.PubChem

    Public Class PubMed
        Public Property pmid As Long
        Public Property articlepubdate As Integer
        Public Property articletype As String
        <Collection("pmidsrcs", "|")>
        Public Property pmidsrcs As String()
        <Collection("meshheadings", "|")>
        Public Property meshheadings As String()
        <Collection("meshsubheadings", "|")>
        Public Property meshsubheadings As String()
        Public Property meshcodes As String
        <Collection("cids", "|")>
        Public Property cids As String()
        Public Property sids As String
        Public Property articletitle As String
        Public Property articleabstract As String
        Public Property articlejourname As String
        Public Property articleauth As String
        Public Property articleaffil As String
        Public Property citation As String
        Public Property doi As String
        Public Property annotation As String
    End Class
End Namespace