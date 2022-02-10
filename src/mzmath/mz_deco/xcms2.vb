Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

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

Public Class PeakSet

    Public Property peaks As xcms2()

    Public ReadOnly Property sampleNames As String()
        Get
            Return peaks.Select(Function(pk) pk.Properties.Keys).IteratesALL.Distinct.ToArray
        End Get
    End Property

    Public Function Subset(sampleNames As String()) As PeakSet
        Dim subpeaks = peaks _
            .Select(Function(pk)
                        Return New xcms2 With {
                            .ID = pk.ID,
                            .mz = pk.mz,
                            .mzmax = pk.mzmax,
                            .mzmin = pk.mzmin,
                            .npeaks = pk.npeaks,
                            .rt = pk.rt,
                            .rtmax = pk.rtmax,
                            .rtmin = pk.rtmin,
                            .Properties = sampleNames _
                                .ToDictionary(Function(name) name,
                                              Function(name)
                                                  Return pk(name)
                                              End Function)
                        }
                    End Function) _
            .ToArray

        Return New PeakSet With {
            .peaks = subpeaks
        }
    End Function

End Class