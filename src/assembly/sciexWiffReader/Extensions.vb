Imports System.Runtime.CompilerServices

Module Extensions

    <Extension>
    Public Function SubArray(Of T)(data As T(), length As Integer) As T()
        If data.Length <= length Then
            Return CType(data.Clone(), T())
        End If
        Dim array As T() = New T(length - 1) {}
        Global.System.Array.Copy(data, 0, array, 0, length)
        Return array
    End Function
End Module
