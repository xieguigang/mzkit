Public Interface IMzQuery

    Function QueryByMz(mz As Double) As IEnumerable(Of MzQuery)

End Interface