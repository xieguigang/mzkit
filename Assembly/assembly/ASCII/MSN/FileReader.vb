Namespace ASCII.MSN

    Public Module FileReader

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data">File path of the ``*.msn-list.txt``</param>
        ''' <returns></returns>
        Public Iterator Function GetIons(data As String) As IEnumerable(Of MGF.Ions)
            Dim metaPeaksFile As String = $"{data.ParentPath}/{data.BaseName.BaseName}.peak-table.txt"
            Dim peaks As PeakTable() = PeakTable.ParseTable(metaPeaksFile)
            Dim ions As Dictionary(Of String, MsnList) = MsnList.GetMsnList(data).ToDictionary(Function(p) p.id)
        End Function
    End Module
End Namespace