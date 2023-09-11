Imports System.IO

Public Class LipidClassProperty
        Public Sub New([class] As LbmClass, displayName As String, acylChain As Integer, alkylChain As Integer, sphingoChain As Integer)
            Me.Class = [class]
            Me.DisplayName = displayName
            Me.AcylChain = acylChain
            Me.AlkylChain = alkylChain
            Me.SphingoChain = sphingoChain
        End Sub

        Public ReadOnly Property [Class] As LbmClass
        Public ReadOnly Property DisplayName As String

        Public ReadOnly Property TotalChain As Integer
            Get
                Return AcylChain + AlkylChain + SphingoChain
            End Get
        End Property
        Public ReadOnly Property AcylChain As Integer
        Public ReadOnly Property AlkylChain As Integer
        Public ReadOnly Property SphingoChain As Integer
        ' public int ExtraAcylChain { get; }
    End Class

Public Class LipidClassDictionary

    Public ReadOnly Property LbmItems As IReadOnlyDictionary(Of LbmClass, LipidClassProperty)
        Get
            Return m_lbmItems
        End Get
    End Property

    ReadOnly m_lbmItems As Dictionary(Of LbmClass, LipidClassProperty)

    Private Sub New()
        m_lbmItems = New Dictionary(Of LbmClass, LipidClassProperty)()
    End Sub

    Public Shared ReadOnly Property [Default] As LipidClassDictionary
        Get
            Static defaultField As LipidClassDictionary

            If defaultField Is Nothing Then
                defaultField = ParseDictinary()
            End If

            Return defaultField
        End Get
    End Property

    Private Shared Function ParseDictinary() As LipidClassDictionary
        Dim result = New LipidClassDictionary()
        Dim acyl As Integer = Nothing, alkyl As Integer = Nothing, sphingo As Integer = Nothing

        Using stream = My.Resources.ResourceManager.GetStream("LipidClassProperties")
            Using reader = New StreamReader(stream)
                Call reader.ReadLine() ' skip header

                While reader.Peek() >= 0
                    Dim cols = reader.ReadLine().Split(","c)
                    Dim item = New LipidClassProperty(
                        System.Enum.Parse(GetType(LbmClass), cols(0)),
                        cols(1),
                        If(Integer.TryParse(cols(2), acyl), acyl, 0),
                        If(Integer.TryParse(cols(3), alkyl), alkyl, 0),
                        If(Integer.TryParse(cols(4), sphingo), sphingo, 0)
                    )

                    result.m_lbmItems(item.Class) = item
                End While
            End Using
        End Using
        Return result
    End Function
End Class
