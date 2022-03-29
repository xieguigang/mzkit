#Region "Microsoft.VisualBasic::8f29783761ba25785e2e473b80281f16, mzkit\src\mzkit\mzkit\forms\Task\RscriptProgressTask.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 103
    '    Code Lines: 73
    ' Comment Lines: 5
    '   Blank Lines: 25
    '     File Size: 5.05 KB


    ' Class RscriptProgressTask
    ' 
    '     Function: CreateMSIIndex
    ' 
    '     Sub: CreateMSIPeakTable, CreateMSIRawFromRowBinds
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Task

Public Class RscriptProgressTask

    ''' <summary>
    ''' convert imzML to mzpack
    ''' </summary>
    ''' <param name="imzML"></param>
    ''' <returns></returns>
    Public Shared Function CreateMSIIndex(imzML As String) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("buildMSIIndex.R")
        Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        Dim uid As String = ibd.UUID
        Dim cachefile As String = App.AppSystemTemp & "/MSI_imzML/" & uid
        Dim cli As String = $"""{Rscript}"" --imzML ""{imzML}"" --cache ""{cachefile}"""
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)

        If cachefile.FileLength > 1024 Then
            Return cachefile
        End If

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Open imzML...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        Return cachefile
    End Function

    Public Shared Sub CreateMSIRawFromRowBinds(files As String(), savefile As String)
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".input_files", sessionID:=App.PID.ToHexString, prefix:="CombineRowScans_")
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSI_rbind.R")
        Dim cli As String = $"""{Rscript}"" --files ""{tempfile}"" --save ""{savefile}"""
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)

        Call files.SaveTo(tempfile, encoding:=Encodings.UTF8.CodePage)

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Convert MSI Raw...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If MessageBox.Show("MSI Raw Convert Job Done!" & vbCrLf & "Open MSI raw data file in MSI Viewer?", "MSI Viewer", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
            Call RibbonEvents.showMsImaging()
            Call WindowModules.viewer.loadimzML(savefile)
        End If
    End Sub

    Public Shared Sub ExportSingleIonPlot(mz As Double, tolerance As String, saveAs As String)
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSImaging/singleIon.R")
        Dim cli As String = $"""{Rscript}"" --app {ServiceHub.appPort} --mzlist ""{mz}"" --save ""{saveAs}"" --mzdiff ""{tolerance}"""
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)
        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Single Ion MSImaging", directAccess:=True)
        progress.ShowProgressDetails("Do plot of target ion m/z...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If saveAs.FileExists Then
            If MessageBox.Show("Single Ion MSImaging Job Done!" & vbCrLf & "Open MSImaging result plot file?", "Open Image", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                Call Process.Start(saveAs.GetFullPath)
            End If
        Else
            Call MessageBox.Show("Single Ion MSImaging Task Error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Public Shared Sub CreateMSIPeakTable(regions As Rectangle(), mzpack As String, saveAs As String)
        Dim data As Dictionary(Of String, Integer()) = regions.ToDictionary(Function(r) $"{r.Left},{r.Top}", Function(r) {r.Left, r.Top, r.Width, r.Height})
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".json", App.PID.ToHexString, prefix:="MSI_regions__")
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSI_peaktable.R")
        Dim cli As String = $"""{Rscript}"" --raw ""{mzpack}"" --save ""{saveAs}"" --regions ""{tempfile}"""
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)

        Call data.GetJson.SaveTo(tempfile, encoding:=Encodings.UTF8.CodePage)

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Create MSI sampletable...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If MessageBox.Show("Export MSI sampletable Job Done!" & vbCrLf & "Open MSI sample table data file?", "Open Excel", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
            Call Process.Start(saveAs.GetFullPath)
        End If
    End Sub
End Class
