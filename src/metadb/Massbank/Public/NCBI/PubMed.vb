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

        Public Function GetPublishDate() As Date
            Try
                Return TryParseInternal(articlepubdate.ToString)
            Catch ex As Exception
                Return New Date(2010, 1, 1)
            End Try
        End Function

        Private Shared Function TryParseInternal(str As String) As Date
            If str.Length <> 8 Then
                str = str.PadEnd(8, "0"c)
            End If

            Dim yyyy = str.Substring(0, 4)
            Dim mm = str.Substring(4, 2)
            Dim dd = str.Substring(6, 2)

            If mm = "00" Then mm = "01"
            If dd = "00" Then dd = "01"

            Return New Date(Integer.Parse(yyyy), Integer.Parse(mm), Integer.Parse(dd))
        End Function

        Public Overrides Function ToString() As String
            Return articletitle
        End Function

    End Class
End Namespace