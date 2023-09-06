Imports CompMs.Common.Enum
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Reflection

Namespace CompMs.Common.Lipidomics
    Public Class LipidClassProperty
        Public Sub New(ByVal [class] As LbmClass, ByVal displayName As String, ByVal acylChain As Integer, ByVal alkylChain As Integer, ByVal sphingoChain As Integer)
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
        Private Sub New()
            lbmItemsField = New Dictionary(Of LbmClass, LipidClassProperty)()
            LbmItems = New ReadOnlyDictionary(Of LbmClass, LipidClassProperty)(lbmItemsField)
        End Sub

        Public ReadOnly Property LbmItems As ReadOnlyDictionary(Of LbmClass, LipidClassProperty)

        Private ReadOnly lbmItemsField As Dictionary(Of LbmClass, LipidClassProperty)


        Public Shared ReadOnly Property [Default] As LipidClassDictionary
            Get
                If defaultField Is Nothing Then
                    defaultField = ParseDictinary()
                End If
                Return defaultField
            End Get
        End Property
        Private Shared defaultField As LipidClassDictionary

        Private Shared Function ParseDictinary() As LipidClassDictionary
            Dim resourceName = "CompMs.Common.Resources.LipidClassProperties.csv"
            Dim assembly = Reflection.Assembly.GetExecutingAssembly()
            Dim result = New LipidClassDictionary()
            Dim acyl As Integer = Nothing, alkyl As Integer = Nothing, sphingo As Integer = Nothing
            Using stream = assembly.GetManifestResourceStream(resourceName)
                Using reader = New StreamReader(stream)
                    reader.ReadLine() ' skip header
                    While reader.Peek() >= 0
                        Dim cols = reader.ReadLine().Split(","c)
                        Dim item = New LipidClassProperty(System.Enum.Parse(GetType(LbmClass), cols(0)), cols(1), If(Integer.TryParse(cols(2), acyl), acyl, 0), If(Integer.TryParse(cols(3), alkyl), alkyl, 0), If(Integer.TryParse(cols(4), sphingo), sphingo, 0))
                        result.lbmItemsField(item.Class) = item
                    End While
                End Using
            End Using
            Return result
        End Function
    End Class
End Namespace
