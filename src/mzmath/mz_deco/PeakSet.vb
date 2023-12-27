Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' A collection of the <see cref="xcms2"/> peak features data
''' </summary>
Public Class PeakSet

    ''' <summary>
    ''' the ROI peaks data
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As xcms2()

    ''' <summary>
    ''' the samples names in current ROI peak set
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property sampleNames As String()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return peaks _
                .Select(Function(pk) pk.Properties.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return sampleNames.GetJson
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Norm() As PeakSet
        Return New PeakSet With {
            .peaks = peaks _
                .Select(Function(pk) pk.totalPeakSum) _
                .ToArray
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function Subset(pk As xcms2, sampleNames As String()) As xcms2
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
    End Function

    Public Function Subset(sampleNames As String()) As PeakSet
        Dim subpeaks As xcms2() = peaks _
            .Select(Function(pk)
                        Return Subset(pk, sampleNames)
                    End Function) _
            .ToArray

        Return New PeakSet With {
            .peaks = subpeaks
        }
    End Function

End Class