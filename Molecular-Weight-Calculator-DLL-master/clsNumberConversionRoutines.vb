Option Strict On

Class clsNumberConversionRoutines

	Public Shared Function CDblSafe(ByVal strWork As String) As Double
		Dim dblValue As Double = 0
		If Double.TryParse(strWork, dblValue) Then
			Return dblValue
		Else
			Return 0
		End If
	End Function

	Public Shared Function CShortSafe(ByVal dblWork As Double) As Int16
		If dblWork <= 32767 And dblWork >= -32767 Then
			Return CShort(dblWork)
		Else
			If dblWork < 0 Then
				Return -32767
			Else
				Return 32767
			End If
		End If
	End Function

	Public Shared Function CShortSafe(ByVal strWork As String) As Int16

		Dim dblValue As Double = 0

		If Double.TryParse(strWork, dblValue) Then
			Return CShortSafe(dblValue)
		ElseIf strWork.ToLower() = "true" Then
			Return -1
		Else
			Return 0
		End If

	End Function

	Public Shared Function CIntSafe(ByVal dblWork As Double) As Int32
		If dblWork <= Integer.MaxValue And dblWork >= Integer.MinValue Then
			Return CInt(dblWork)
		Else
			If dblWork < 0 Then
				Return Integer.MinValue
			Else
				Return Integer.MaxValue
			End If
		End If
	End Function

	Public Shared Function CIntSafe(ByVal strWork As String) As Int32

		Dim dblValue As Double = 0

		If Double.TryParse(strWork, dblValue) Then
			Return CIntSafe(dblValue)
		ElseIf strWork.ToLower() = "true" Then
			Return -1
		Else
			Return 0
		End If

	End Function

	Public Shared Function CStrSafe(ByVal Item As Object) As String
		Try
			If Item Is Nothing Then
				Return String.Empty
			ElseIf Convert.IsDBNull(Item) Then
				Return String.Empty
			Else
				Return CStr(Item)
			End If
		Catch ex As Exception
			Return String.Empty
		End Try
	End Function

    Public Shared Function IsNumber(ByVal strValue As String) As Boolean        
        Try
			Return Double.TryParse(strValue, 0)
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class
