Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ASCII.MGF

    Public Class MetaData

        Dim meta As Dictionary(Of String, String)

#Region "MetaData getter/setter"

        Public Property rawfile As String
            Get
                Return meta.TryGetValue("rawfile")
            End Get
            Set(value As String)
                meta("rawfile") = value
            End Set
        End Property

        Public Property collisionEnergy As String
            Get
                Return meta.TryGetValue("collisionEnergy")
            End Get
            Set(value As String)
                meta("collisionEnergy") = value
            End Set
        End Property

        Public Property activation As String
            Get
                Return meta.TryGetValue("activation")
            End Get
            Set(value As String)
                meta("activation") = value
            End Set
        End Property

        Public Property scan As String
            Get
                Return meta.TryGetValue("scan")
            End Get
            Set(value As String)
                meta("scan") = value
            End Set
        End Property

        Public Property name As String
            Get
                Return meta.TryGetValue("name")
            End Get
            Set(value As String)
                meta("name") = value
            End Set
        End Property

        Public Property compound_class As String
            Get
                Return meta.TryGetValue("compound_class")
            End Get
            Set(value As String)
                meta("compound_class") = value
            End Set
        End Property

        Public Property formula As String
            Get
                Return meta.TryGetValue("formula")
            End Get
            Set(value As String)
                meta("formula") = value
            End Set
        End Property

        Public Property mass As Double
            Get
                Return meta.TryGetValue("mass")
            End Get
            Set(value As Double)
                meta("mass") = value
            End Set
        End Property

        Public Property kegg As String
            Get
                Return meta.TryGetValue("kegg")
            End Get
            Set(value As String)
                meta("kegg") = value
            End Set
        End Property
#End Region

        Public ReadOnly Property MetaTable As Dictionary(Of String, String)
            Get
                Return meta
            End Get
        End Property

        Sub New()
            Call Me.New(New Dictionary(Of String, String))
        End Sub

        Sub New(meta As Dictionary(Of String, String))
            Me.meta = meta
        End Sub

        Public Overrides Function ToString() As String
            Return meta.GetJson
        End Function

        Public Shared Narrowing Operator CType(meta As MetaData) As Dictionary(Of String, String)
            Return meta.meta
        End Operator

    End Class
End Namespace