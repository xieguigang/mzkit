Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.My
Imports Task

Public Class ConnectToBioDeep

    Private Sub New()
    End Sub

    Public Shared Sub OpenAdvancedFunction(action As Action)
        If Not SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call New frmLogin().ShowDialog()
        End If

        If SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call action()
        End If
    End Sub

    Public Shared Sub RunMetaDNA(raw As Raw)
        If Not SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call New frmLogin().ShowDialog()
        End If

        If SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call MetaDNASearch.RunDIA(raw,)
        End If
    End Sub
End Class
