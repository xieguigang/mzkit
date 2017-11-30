Namespace TMIC.HMDB

    Public Structure NameValue : Implements IEquatable(Of NameValue)

        Public Property name As String
        Public Property match As String
        Public Property type As String
        Public Property metabolite As String
        Public Property ID As String

        Public Overrides Function ToString() As String
            Return $"name={name}, match={match}, metabolite={metabolite}, type={type}"
        End Function

        Public Overloads Function Equals(other As NameValue) As Boolean Implements IEquatable(Of NameValue).Equals
            Return other.ToString = Me.ToString
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If obj Is Nothing Then
                Return False
            ElseIf Not obj.GetType Is GetType(NameValue) Then
                Return False
            End If

            Return Equals(other:=DirectCast(obj, NameValue))
        End Function
    End Structure
End Namespace