Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Parallel.Threads
Imports SMRUCC.WebCloud.HTTPInternal
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

''' <summary>
''' VB server script
''' </summary>
<[Namespace]("ProteoWizard.d")>
Public Class VBServerScript : Inherits WebApp

    ''' <summary>
    ''' + msconvert
    ''' 
    ''' ProteoWizard命令行程序的位置
    ''' </summary>
    ReadOnly BIN$
    ''' <summary>
    ''' OSS存储的文件系统位置
    ''' </summary>
    ReadOnly OSS_ROOT$

    Dim taskPool As New ThreadPool

    Sub New(main As PlatformEngine)
        Call MyBase.New(main)

        BIN = App.GetVariable("bin")
        OSS_ROOT = App.GetVariable("oss")

        If Not OSS_ROOT.DirectoryExists Then
            Throw New Exception("OSS file system should be mounted at first!")
        End If
    End Sub

    <ExportAPI("/ProteoWizard.d/mzXML.vbs")>
    <Usage("/ProteoWizard.d/mzXML.vbs?path=<path>")>
    <[GET](GetType(String))>
    Public Function ConvertTomzXML(request As HttpRequest, response As HttpResponse) As Boolean
        Dim path$ = OSS_ROOT & "/" & request.URLParameters("path")
        Dim args$ = $"{path.CLIPath} --mz64 --mzXML --filter ""msLevel 1-2"" --ignoreUnknownInstrumentError"

        Call New IORedirectFile(BIN, args).Run()

        If Not response Is Nothing Then
            Call response.SuccessMsg("Task complete!")
        End If

        Return True
    End Function

    <ExportAPI("/ProteoWizard.d/mzXML.task.vbs")>
    <Usage("/ProteoWizard.d/mzXML.task.vbs?path=<path>")>
    <[GET](GetType(String))>
    Public Function ConvertTomzXMLTask(request As HttpRequest, response As HttpResponse) As Boolean
        Dim task = Sub() ConvertTomzXML(request, Nothing)

        Call taskPool.RunTask(task)
        Call response.SuccessMsg("Task pending...")

        Return True
    End Function
End Class
