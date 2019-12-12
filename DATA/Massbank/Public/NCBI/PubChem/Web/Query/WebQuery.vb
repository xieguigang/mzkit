Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

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

    Public Class CIDExport : Inherits WebQuery(Of String)

        Const queryCAS_Api As String = "https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&outfmt=jsonp"

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1)
            MyBase.New(AddressOf queryApi, AddressOf normalizeFileName, AddressOf parseExportTable, , cache, interval)
        End Sub

        Private Shared Function parseExportTable(text As String, schema As Type) As Object
            Return text _
                .LineTokens _
                .AsDataSource(Of QueryTableExport) _
                .ToArray
        End Function

        Private Shared Function queryApi(CAS As String) As String
            Dim query As New JsonQuery With {
                .where = New QueryWhere With {
                    .ands = {New Dictionary(Of String, String) From {
                        {"*", CAS}
                    }}
                }
            }
            Dim json$ = query.GetJson.UrlEncode
            Dim url$ = $"{queryCAS_Api}&query={json}"

            Return url
        End Function

        Private Shared Function normalizeFileName(text As String) As String
            Return text.NormalizePathString(False)
        End Function
    End Class

    Public Class CIDQuery : Inherits WebQuery(Of String)

        ''' <summary>
        ''' Search pubchem by CAS
        ''' </summary>
        Const queryCAS_URL As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/%s/cids/JSON"

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)
            MyBase.New(AddressOf queryApi, AddressOf normalizeFileName, AddressOf loadQueryJson, AddressOf prefix, cache, interval, offline)
        End Sub

        ''' <summary>
        ''' Path prefix of the compound name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Private Shared Function prefix(name As String) As String
            Return name.NormalizePathString.Trim("_"c, " "c).First
        End Function

        Private Shared Function loadQueryJson(jsonText As String, type As Type) As IdentifierList
            If jsonText.StringEmpty Then
                ' 404 代码之下得到的content text是空字符串
                Return Nothing
            Else
                Dim list As IdentifierList = jsonText _
                    .LoadJSON(Of QueryResponse) _
                   ?.IdentifierList
                Return list
            End If
        End Function

        Private Shared Function normalizeFileName(text As String) As String
            Return text.NormalizePathString(False)
        End Function

        Private Shared Function queryApi(name As String) As String
            Return CLangStringFormatProvider.sprintf(queryCAS_URL, name.UrlEncode(jswhitespace:=True))
        End Function

    End Class

    Public Class WebQuery : Inherits WebQuery(Of String)

        Const fetchPugView As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/%s/XML/"

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)
            MyBase.New(AddressOf pugViewApi, Function(cid) cid, AddressOf loadPugView, AddressOf prefix, cache, interval, offline)
        End Sub

        ''' <summary>
        ''' Path prefix of the CID number
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Private Shared Function prefix(name As String) As String
            Return Mid(name, 1, 3)
        End Function

        Private Shared Function loadPugView(xml As String, type As Type) As PugViewRecord
            If type Is GetType(PugViewRecord) Then
                Return xml.LoadFromXml(Of PugViewRecord)(throwEx:=False)
            Else
                Throw New NotImplementedException
            End If
        End Function

        Private Shared Function pugViewApi(cid As String) As String
            Return CLangStringFormatProvider.sprintf(fetchPugView, cid)
        End Function
    End Class

    Public Class QueryResponse

        Public Property IdentifierList As IdentifierList
        ''' <summary>
        ''' 当这个属性为空值的时候说明请求成功,反之不为空的时候说明出现了错误
        ''' </summary>
        ''' <returns></returns>
        Public Property Fault As Fault

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class Fault
        Public Property Code As String
        Public Property Message As String
        Public Property Details As String()
    End Class

    Public Class IdentifierList

        Public Property CID As String()

        Public Overrides Function ToString() As String
            Return CID.GetJson
        End Function
    End Class
End Namespace