
Imports BioNovoGene.Analytical.MassSpectrometry.Math

''' <summary>
''' Result data pack for save the annotation result data
''' </summary>
''' <remarks>
''' data export for internal annotation workflow, handling to customer report and view on mzkit workbench.
''' </remarks>
Public Class AnnotationPack : Implements IWorkspaceReader, IDisposable

    Private disposedValue As Boolean

    ''' <summary>
    ''' the ms2 spectrum alignment search hits
    ''' </summary>
    ''' <returns></returns>
    Public Property libraries As Dictionary(Of String, AlignmentHit())

    ''' <summary>
    ''' the ms1 peaktable
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As xcms2()

    Public Property file As String

    Public Function CreatePeakSet() As PeakSet
        Return New PeakSet(peaks)
    End Function

    Public Function GetLibraryResult(libraryName As String) As AlignmentHit()
        If libraries.ContainsKey(libraryName) Then
            Return _libraries(libraryName)
        Else
            Return {}
        End If
    End Function

    ''' <summary>
    ''' Make a copy of current in-memory data pack
    ''' </summary>
    ''' <returns></returns>
    Private Function LoadMemory() As AnnotationPack Implements IWorkspaceReader.LoadMemory
        Return New AnnotationPack With {
            .libraries = libraries _
                .ToDictionary(Function(li) li.Key,
                              Function(li)
                                  Return li.Value.ToArray
                              End Function),
            .peaks = peaks.ToArray
        }
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Erase peaks

                ' clear all annotation data
                Call libraries.Clear()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    ''' <summary>
    ''' just release the memory content data at here
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
