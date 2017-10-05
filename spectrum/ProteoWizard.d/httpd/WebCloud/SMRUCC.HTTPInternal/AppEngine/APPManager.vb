#Region "Microsoft.VisualBasic::6b9632a90619184e31cbc1a0f272c9f1, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\APPManager.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace AppEngine

    ''' <summary>
    ''' Help document developer user manual page
    ''' </summary>
    <[Namespace]("dashboard")> Public Class APPManager : Inherits WebApp
        Implements IEnumerable(Of APPEngine)

        ''' <summary>
        ''' 键名要求是小写的
        ''' </summary>
        Dim RunningAPP As New Dictionary(Of String, APPEngine)
        ''' <summary>
        ''' 动态添加的API，这些API的url不是标准的url命名空间
        ''' </summary>
        Dim dynamics As New Dictionary(Of String, (App As Object, API As APIInvoker))

        ''' <summary>
        ''' 生成帮助文档所需要的
        ''' </summary>
        ''' <returns></returns>
        Public Property baseUrl As String

        Sub New(API As PlatformEngine)
            Call MyBase.New(API)
            Call Register(Me)
        End Sub

        Default Public ReadOnly Property App(name As String) As APPEngine
            Get
                name = name.ToLower

                If RunningAPP.ContainsKey(name) Then
                    Return RunningAPP(name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url$"></param>
        ''' <param name="method"></param>
        ''' <param name="API"></param>
        ''' <param name="APP">假若是模块Module，则使用这个默认的空值，假若是Class中的实例方法，则还需要把那个Class对象传递进来</param>
        ''' <param name="help$"></param>
        Public Sub Join(url$, method As APIMethod, API As MethodInfo, Optional APP As Object = Nothing, Optional help$ = "No help info...")
            dynamics(url.ToLower) = (APP, New APIInvoker With {
                .EntryPoint = API,
                .Help = help,
                .Method = method,
                .Name = url
            })
        End Sub

        ''' <summary>
        ''' Get running app by type.
        ''' </summary>
        ''' <typeparam name="App"></typeparam>
        ''' <returns></returns>
        Public Function GetApp(Of App As Class)() As App
            Dim appType As Type = GetType(App)
            Dim LQuery As App = LinqAPI.DefaultFirst(Of App) <=
 _
                From x As APPEngine
                In RunningAPP.Values
                Where appType.Equals(x.Application.GetType)
                Let AppInstant As App =
                    DirectCast(x.Application, App)
                Select AppInstant

            Return LQuery
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of APPEngine) Implements IEnumerable(Of APPEngine).GetEnumerator
            For Each obj In RunningAPP
                Yield obj.Value
            Next
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="response">HTML输出页面或者json数据</param>
        ''' <returns></returns>
        Public Function InvokePOST(request As HttpPOSTRequest, response As HttpResponse) As Boolean
            Return APPEngine.InvokePOST(request, RunningAPP, response, dynamics)
        End Function

        ''' <summary>
        ''' 当WebApp查找失败的时候所执行的默认的API函数
        ''' </summary>
        ''' <returns></returns>
        Public Property DefaultAPI As APIAbstract

        ''' <summary>
        ''' 默认是API执行失败
        ''' </summary>
        ''' <param name="api"></param>
        ''' <returns></returns>
        Private Shared Function __defaultFailure(api As String, request As HttpRequest, response As HttpResponse) As Boolean
            Return False
        End Function

        Public Sub ResetAPIDefault()
            DefaultAPI = AddressOf __defaultFailure
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="response">HTML输出页面或者json数据</param>
        ''' <returns></returns>
        Public Function Invoke(request As HttpRequest, response As HttpResponse) As Boolean
            Return APPEngine.Invoke(request, RunningAPP, response, dynamics, DefaultAPI)
        End Function

        Public Function PrintHelp() As String
            Dim LQuery$() = LinqAPI.Exec(Of String) <=
 _
                From app As KeyValuePair(Of String, APPEngine)
                In Me.RunningAPP
                Let nsDescrib As String = If(
                    String.IsNullOrEmpty(app.Value.Namespace.Description),
                    "",
                    $"                <p>{app.Value.Namespace.Description}</p><br /><br />")
                Let head = $"<br /><div><h3>Application/Namespace                --- <strong>{baseUrl}/{app.Value.Namespace.Namespace}/</strong> ---</h3>" & nsDescrib
                Select head & vbCrLf & app.Value.GetHelp & vbCrLf & "</div>"

            Dim HelpPage As String = String.Join($"<br /><br />", LQuery)

            Return HelpPage
        End Function

        <ExportAPI("/dashboard/help_doc.html",
                   Info:="Get the help documents about how to using the mipaimai platform WebAPI.",
                   Usage:="/dashboard/help_doc.html",
                   Example:="<a href=""/dashboard/help_doc.html"">/dashboard/help_doc.html</a>")>
        <[GET](GetType(String))>
        Public Function Help(request As HttpRequest, response As HttpResponse) As Boolean
            Call response.WriteHTML(PrintHelp)
            Return True
        End Function

        <ExportAPI("/dashboard/server_status.vbs")>
        <[GET](GetType(String))>
        Public Function ServerStatus(request As HttpRequest, response As HttpResponse) As Boolean
            Call response.WriteLine(PlatformEngine._threadPool.GetStatus.GetJson(indent:=True))
            Return True
        End Function

        <ExportAPI("/dashboard/404_test.vbs")>
        <[GET](GetType(String))>
        Public Function Test404(request As HttpRequest, response As HttpResponse) As Boolean
            Throw New StackOverflowException(PlatformEngine._threadPool.GetStatus.GetJson(indent:=True))
        End Function

        ''' <summary>
        ''' 向开放平台之中注册API接口
        ''' </summary>
        ''' <typeparam name="APP"></typeparam>
        ''' <param name="application"></param>
        ''' <returns></returns>
        Public Function Register(Of APP As WebApp)(application As APP) As Boolean
            Dim registerApp = APPEngine.Imports(application)

            If registerApp Is Nothing Then
                Return False
            End If

            Dim hash As String = registerApp.Namespace.Namespace.ToLower
            If Me.RunningAPP.ContainsKey(hash) Then
                Return False
            Else
                Call RunningAPP.Add(hash, registerApp)
            End If

            Return True
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
