Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace NCBI.PubChem

    ' {"download":"*","collection":"compound","where":{"ands":[{"*":"66-84-2"}]},"order":["relevancescore,desc"],"start":1,"limit":1000000}
    ' {"collection":"compound","download":"*","limit":10,"order":["relevancescore,desc"],"start":1,"where":{"ands":{"*":"650818-62-1"}}}
    Public Class JsonQuery

        Public Property download As String = "*"
        Public Property collection As String = "compound"
        Public Property [where] As QueryWhere
        Public Property order As String() = {"relevancescore,desc"}
        Public Property start As Integer = 1
        Public Property limit As Integer = 10

    End Class

    Public Class [QueryWhere]
        Public Property ands As Dictionary(Of String, String)()
    End Class

    ''' <summary>
    ''' Table export result of <see cref="JsonQuery"/>
    ''' </summary>
    Public Class QueryTableExport
        Public Property cid As String
        Public Property cmpdname As String
        <Collection("cmpdsynonym", "|")>
        Public Property cmpdsynonym As String()
        Public Property mw As Double
        Public Property mf As String
        Public Property polararea As Double
        Public Property complexity As Double
        Public Property xlogp As String
        Public Property heavycnt As Double
        Public Property hbonddonor As Double
        Public Property hbondacc As Double
        Public Property rotbonds As Double
        Public Property inchikey As String
        Public Property iupacname As String
        Public Property meshheadings As String
        Public Property annothits As Double
        Public Property annothitcnt As Double
        <Collection("aids", ",")>
        Public Property aids As String()
        Public Property cidcdate As String
        <Collection("dois", "|")>
        Public Property dois As String()
    End Class

End Namespace