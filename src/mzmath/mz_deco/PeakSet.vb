Imports Microsoft.VisualBasic.Linq

Public Class PeakSet

    Public Property peaks As xcms2()

    Public ReadOnly Property sampleNames As String()
        Get
            Return peaks.Select(Function(pk) pk.Properties.Keys).IteratesALL.Distinct.ToArray
        End Get
    End Property

    Public Function Norm() As PeakSet
        Return New PeakSet With {
            .peaks = peaks _
                .Select(Function(pk) pk.totalPeakSum) _
                .ToArray
        }
    End Function

    Public Function Subset(sampleNames As String()) As PeakSet
        Dim subpeaks = peaks _
            .Select(Function(pk)
                        Return New xcms2 With {
                            .ID = pk.ID,
                            .mz = pk.mz,
                            .mzmax = pk.mzmax,
                            .mzmin = pk.mzmin,
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