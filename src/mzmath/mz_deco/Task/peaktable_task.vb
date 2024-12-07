Imports Microsoft.VisualBasic.Parallel

Namespace Tasks

    Public MustInherit Class peaktable_task : Inherits VectorTask

        Public ReadOnly out As New List(Of xcms2)
        Public ReadOnly rt_shifts As New List(Of RtShift)

        ''' <summary>
        ''' construct a new parallel task executator
        ''' </summary>
        ''' <param name="nsize"></param>
        ''' <remarks>
        ''' the thread count for run the parallel task is configed
        ''' via the <see cref="n_threads"/> by default.
        ''' </remarks>
        Sub New(nsize As Integer,
                Optional verbose As Boolean = False,
                Optional workers As Integer? = Nothing)

            Call MyBase.New(nsize, verbose, workers)
        End Sub
    End Class
End Namespace