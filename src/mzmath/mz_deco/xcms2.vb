Imports Microsoft.VisualBasic.Data.csv.IO

''' <summary>
''' the peak table format table file model of xcms version 2
''' </summary>
Public Class xcms2 : Inherits DataSet

    Public Property mz As Double
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property rt As Double
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property npeaks As Integer

    Public Shared Function Load(file As String) As xcms2()
        Return DataSet _
            .LoadDataSet(Of xcms2)(file, uidMap:=NameOf(ID)) _
            .ToArray
    End Function

End Class
