Imports System.Runtime.CompilerServices

Module Utils

    <Extension>
    Friend Sub StoreParallelStrings(targetList As ICollection(Of KeyValuePair(Of String, String)),
                                    names As IList(Of String),
                                    values As IList(Of String),
                                    Optional skipEmptyNames As Boolean = False,
                                    Optional replaceTabsInValues As Boolean = False)
        Call targetList.Clear()

        For i As Integer = 0 To names.Count - 1
            If skipEmptyNames AndAlso (String.IsNullOrWhiteSpace(names(i)) OrElse Equals(names(i), CStr(ChrW(1)))) Then
                ' Name is empty or null
                Continue For
            End If

            If replaceTabsInValues AndAlso values(CInt(i)).Contains(vbTab) Then
                targetList.Add(New KeyValuePair(Of String, String)(names(i), values(CInt(i)).Replace(CStr(vbTab), CStr(" ")).TrimEnd(" "c)))
            Else
                targetList.Add(New KeyValuePair(Of String, String)(names(i), values(i).TrimEnd(" "c)))
            End If
        Next
    End Sub
End Module
