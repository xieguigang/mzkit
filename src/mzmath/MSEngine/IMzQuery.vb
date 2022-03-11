Public Interface IMzQuery

    Function QueryByMz(mz As Double) As IEnumerable(Of MzQuery)
    Function GetAnnotation(uniqueId As String) As (name As String, formula As String)

    ''' <summary>
    ''' query a set of m/z peak list
    ''' </summary>
    ''' <param name="mzlist"></param>
    ''' <returns></returns>
    Function MSetAnnotation(mzlist As IEnumerable(Of Double), Optional topN As Integer = 3) As IEnumerable(Of MzQuery)

End Interface