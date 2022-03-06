Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary

Public Class InputDialog

    Public Shared Sub Input(Of Form As {New, InputDialog})(setConfig As Action(Of Form),
                                                           Optional cancel As Action = Nothing,
                                                           Optional config As Form = Nothing)
        Dim getConfig As Form = If(config, New Form)
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getConfig) = DialogResult.OK Then
            Call setConfig(getConfig)
        ElseIf Not cancel Is Nothing Then
            Call cancel()
        End If
    End Sub

End Class