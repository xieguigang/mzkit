Module Utils

    ''' <summary>
    ''' 判断是否是键
    ''' </summary>
    Public Function isKey(element As String) As Boolean
        Static allKeys As String() = ChemicalKey.allKeys

        For Each c In allKeys
            If element.Equals(c) Then
                '  System.out.println("是键");
                Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' 判断是否是元素  
    ''' 两位的元素没搞，比如Br Cl
    ''' </summary>
    ''' <param name="element"></param>
    ''' <returns></returns>
    Public Function isElement(element As String) As Boolean
        Static elements As String() = ChemicalElement.allElement

        For Each s In elements
            If element.Equals(s) Then
                '   System.out.println("是元素");
                Return True
            End If

            If element.Equals("[") Then
                Return True
            End If
        Next

        Return False
    End Function
End Module
