Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel.Threads
Imports SMRUCC.WebCloud.HTTPInternal
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<[Namespace]("MassWolf.d")>
Public Class MassWolf : Inherits WebApp

    ''' <summary>
    ''' MassWolf命令行程序的位置
    ''' </summary>
    ReadOnly BIN$

    Dim taskPool As New ThreadPool

    Public Sub New(main As PlatformEngine)
        MyBase.New(main)

        BIN = App.GetVariable("masswolf")

        Call $"MassWolf={BIN}".__INFO_ECHO

        If Not BIN.FileExists Then
            Call $"MassWolf perl script is missing, this web app will not working unless you have put MassWolf perlscript to the location {BIN}".Warning
        End If
    End Sub

    <ExportAPI("/MassWolf.d/mzXML.vbs")>
    <Usage("/MassWolf.d/mzXML.vbs?path=<path>")>
    <[GET](GetType(String))>
    Public Function ConvertWatersRaw(request As HttpRequest, response As HttpResponse) As Boolean
        Dim path$ = ensureZipExtract(normalizePath(request.URLParameters("path")))
        Dim out$ = normalizePath(request.URLParameters("to")) Or $"{path.ParentPath}/msconvert".AsDefault
        Dim args = $"{BIN.CLIPath} {path.CLIPath} {out.CLIPath}"
        Dim shell% = App.Shell("perl", args, CLR:=False).Run

        Call response.SuccessMsg("Run MassWolf task complete!")

        If shell = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Iterator Function SplitDirectory(waters$) As IEnumerable(Of String)

    End Function
End Class
