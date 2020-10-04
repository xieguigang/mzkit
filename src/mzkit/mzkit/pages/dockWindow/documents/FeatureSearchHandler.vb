Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Linq
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
            Call runFormulaMatch(text, raw)

            MyApplication.host.ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Private Sub runFormulaMatch(formula As String, files As IEnumerable(Of Raw))
        Dim ppm As Double = MyApplication.host.GetPPMError()
        Dim display As New frmFeatureSearch

        display.Show(MyApplication.host.dockPanel)

        For Each file As Raw In files
            Dim result = MatchByFormula(formula, file).ToArray

            display.AddFileMatch(file.source, result)
        Next
    End Sub

    Public Iterator Function MatchByFormula(formula As String, raw As Raw) As IEnumerable(Of ParentMatch)
        ' formula
        Dim exact_mass As Double = Math.EvaluateFormula(formula)
        Dim ppm As Double = MyApplication.host.GetPPMError()

        ' MyApplication.host.showStatusMessage($"Search MS ions for [{Text}] exact_mass={exact_mass} with tolerance error {ppm} ppm")

        ' C25H40N4O5
        Dim pos = MzCalculator.EvaluateAll(exact_mass, "+", False).ToArray
        Dim neg = MzCalculator.EvaluateAll(exact_mass, "-", False).ToArray
        Dim info As PrecursorInfo()

        For Each scan As ScanEntry In raw.GetMs2Scans
            If scan.polarity > 0 Then
                info = pos
            Else
                info = neg
            End If

            For Each mode As PrecursorInfo In info
                If PPMmethod.PPM(scan.mz, Val(mode.mz)) <= ppm Then
                    Yield New ParentMatch With {
                        .id = scan.id,
                        .mz = scan.mz,
                        .rt = CInt(scan.rt),
                        .BPC = scan.BPC,
                        .TIC = scan.TIC,
                        .M = mode.M,
                        .adducts = mode.adduct,
                        .charge = mode.charge,
                        .precursor_type = mode.precursor_type,
                        .ppm = PPMmethod.PPM(scan.mz, Val(mode.mz)).ToString("F2"),
                        .polarity = scan.polarity,
                        .XIC = 0
                    }
                End If
            Next
        Next
    End Function

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
