Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel

Namespace NCBI.PubChem

    Public Class CIDQuery : Inherits WebQuery(Of String)

        ''' <summary>
        ''' Search pubchem by CAS
        ''' </summary>
        Const queryCAS_URL As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/%s/cids/JSON"

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1)
            MyBase.New(AddressOf queryApi, AddressOf normalizeFileName, AddressOf loadQueryJson, , cache, interval)
        End Sub

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

        Private Shared Function queryApi(CAS As String) As String
            Return sprintf(queryCAS_URL, CAS)
        End Function

    End Class

    Public Class WebQuery : Inherits WebQuery(Of String)

        Const fetchPugView As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/%s/XML/"

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1)
            MyBase.New(AddressOf pugViewApi, Function(cid) cid, AddressOf loadPugView, , cache, interval)
        End Sub

        Private Shared Function loadPugView(xml As String, type As Type) As PugViewRecord
            If type Is GetType(PugViewRecord) Then
                Return xml.LoadFromXml(Of PugViewRecord)
            Else
                Throw New NotImplementedException
            End If
        End Function

        Private Shared Function pugViewApi(cid As String) As String
            Return sprintf(fetchPugView, cid)
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