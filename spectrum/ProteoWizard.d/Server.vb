Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<[Namespace]("ProteoWizard.d")>
Public Class Server : Inherits WebApp

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

    Sub New(main As PlatformEngine)
        Call MyBase.New(main)

        BIN = App.GetVariable("bin")
        OSS_ROOT = App.GetVariable("oss")
    End Sub

    <ExportAPI("/ProteoWizard.d/mzXML.vbs")>
    <Usage("/ProteoWizard.d/mzXML.vbs?path=<path>")>
    <[GET](GetType(String))>
    Public Function ConvertTomzXML(request As HttpRequest, response As HttpResponse) As Boolean
        Dim path$ = OSS_ROOT & "/" & request.URLParameters("path")
        Dim args$ = $"{path.CLIPath} --mz64 --mzXML --filter ""msLevel 1-2"" --ignoreUnknownInstrumentError"

        Call New IORedirectFile(BIN, args).Run()

        Return True
    End Function
End Class
