Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML

Public Class frmSRMIonsExplorer

    Public Sub LoadMRM(file As String)
        Call CheckedListBox1.Items.Clear()

        For Each chr As chromatogram In file.LoadChromatogramList
            CheckedListBox1.Items.Add(chr)
        Next
    End Sub
End Class