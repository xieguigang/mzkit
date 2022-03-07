Imports BioNovoGene.mzkit_win32.My

Module Actions

    ReadOnly actions As New Dictionary(Of String, Action(Of Array))

    Public ReadOnly Property allActions As IEnumerable(Of String)
        Get
            Return actions.Keys
        End Get
    End Property

    Public Sub Register(name As String, action As Action(Of Array))
        actions(name) = action
    End Sub

    Public Sub RunAction(name As String, data As Array)
        If actions.ContainsKey(name) Then
            Call actions(name)(data)
        Else
            Call MyApplication.host.warning($"missing action '{name}'!")
        End If
    End Sub

    Sub New()
        Call registerMs1Search()
    End Sub

    Private Sub registerMs1Search()
        Call Register("Peak List Annotation",
             Sub(data)

             End Sub)
    End Sub

End Module
