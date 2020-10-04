Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task

Module FeatureSearchHandler

    Public Sub SearchByMz(text As String, raw As IEnumerable(Of Raw))
        If text.StringEmpty Then
            Return
        ElseIf text.IsNumeric Then
            Call searchInFileByMz(mz:=Val(text), raw:=raw)
        Else


            MyApplication.host.ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Private Sub searchInFileByMz(mz As Double, raw As IEnumerable(Of Raw))
        Dim ppm As Double = MyApplication.host.GetPPMError()
        Dim tolerance As Tolerance = Tolerance.PPM(ppm)
        Dim display As New frmFeatureSearch

        display.Show(MyApplication.host.dockPanel)

        For Each file In raw
            Dim result = file.GetMs2Scans.Where(Function(a) tolerance(a.mz, mz)).ToArray

            display.AddFileMatch(file.source, mz, result)
        Next
    End Sub
End Module
