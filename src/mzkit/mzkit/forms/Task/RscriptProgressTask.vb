Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline

Public Class RscriptProgressTask

    Public Shared Function CreateMSIIndex(imzML As String) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("buildMSIIndex.R")
        Dim progress As New frmTaskProgress
        Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        Dim uid As String = ibd.UUID
        Dim cachefile As String = App.AppSystemTemp & "/MSI_imzML/" & uid
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, $"""{Rscript}"" --imzML ""{imzML}"" --cache ""{cachefile}""")

        progress.ShowProgressTitle("Open imzML...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        Return cachefile
    End Function

End Class
