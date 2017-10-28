#Region "Microsoft.VisualBasic::634fa633656048de96199055d7c3fec1, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\APPEngine.vb"

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
Imports Microsoft.VisualBasic.Win32
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments

Namespace AppEngine

    ''' <summary>
    ''' Engine for executes the API that defined in the <see cref="WebApp"/>.
    ''' (执行<see cref="WebApp"/>的工作引擎)
    ''' </summary>
    Public Class APPEngine

        ''' <summary>
        ''' 必须按照从长到短来排序
        ''' </summary>
        Dim API As Dictionary(Of String, APIInvoker)

        Public ReadOnly Property [Namespace] As [Namespace]
        Public ReadOnly Property Application As Object

        ''' <summary>
        ''' Gets help page.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetHelp() As String
            Dim LQuery = (From api In Me.API Select "<li>" & api.Value.Help & "</li>").ToArray
            Dim result As String = String.Join("<br />" & vbCrLf, LQuery)
            Return result
        End Function

        Protected Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="api">已经变小写了的</param>
        ''' <param name="request"></param>
        ''' <returns></returns>
        Public Function Invoke(api As String, request As HttpRequest, response As HttpResponse) As Boolean
            If Not Me.API.ContainsKey(api) Then
                Return False
            End If

            Dim script As APIInvoker = Me.API(api)
            Dim success As Boolean =
                script.Invoke(Application, request, response)

            Return success
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="api">已经变小写了的</param>
        ''' <param name="request"></param>
        ''' <returns></returns>
        Public Function Invoke(api As String, request As HttpPOSTRequest, response As HttpResponse) As Boolean
            If Not Me.API.ContainsKey(api) Then
                Return False
            End If

            Dim script As APIInvoker = Me.API(api)
            Dim success As Boolean =
                script.InvokePOST(Application, request, response)

            Return success
        End Function

        Public Shared Function InvokePOST(request As HttpPOSTRequest,
                                          applications As Dictionary(Of String, APPEngine),
                                          response As HttpResponse,
                                          dynamics As Dictionary(Of String, (App As Object, API As APIInvoker))) As Boolean

            Dim application As String = "", api As String = "", parameters As String = ""
            Dim url As String = request.URL

            If Not APPEngine.GetParameter(url, application, api, parameters) Then
                Return False
            Else
                request.URLParameters = parameters.RequestParser
            End If

            If dynamics.ContainsKey(api) Then
                Dim run As (App As Object, api As APIInvoker) =
                    dynamics(api)
                Return run.api _
                    .InvokePOST(run.App, request, response)
            End If

            If Not applications.ContainsKey(application) Then
                Return False
            End If

            Return applications(application).Invoke(api, request, response)
        End Function

        ''' <summary>
        ''' 分析url，然后查找相对应的WebApp，并进行数据请求的执行
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Invoke(request As HttpRequest,
                                      applications As Dictionary(Of String, APPEngine),
                                      response As HttpResponse,
                                      dynamics As Dictionary(Of String, (App As Object, API As APIInvoker)),
                                      [default] As APIAbstract) As Boolean

            Dim application As String = "", api As String = "", parameters As String = ""
            Dim url As String = request.URL

            If Not APPEngine.GetParameter(url, application, api, parameters) Then
                Return False
            Else
                request.URLParameters = parameters.RequestParser
            End If

            If Not applications.ContainsKey(application) Then ' 找不到相对应的WebApp，则默认返回失败 
                If dynamics.ContainsKey(api) Then
                    Dim run As (App As Object, api As APIInvoker) = dynamics(api)
                    Return run.api.Invoke(run.App, request, response)
                End If

                If Not [default] Is Nothing Then
                    Return [default](api, request, response)
                End If

                Return False
            End If

            Return applications(application).Invoke(api, request, response)
        End Function

        Public Shared Function [Imports](Of T As WebApp)(obj As T) As APPEngine
            Dim type As Type = obj.GetType
            Dim ns As [Namespace] = type.NamespaceEntry

            If ns Is Nothing OrElse ns.AutoExtract = True Then
                Dim msg As String = $"Could not found application entry point from {type.FullName}"

                Call msg.__DEBUG_ECHO
                Call ServicesLogs.WriteEntry(msg, MethodBase.GetCurrentMethod, EventLogEntryType.FailureAudit)
                Return Nothing
            End If

            Dim Methods As MethodInfo() = type.GetMethods(BindingFlags.Public Or BindingFlags.Instance)
            Dim API_DEFINE As Type = ExportAPIAttribute.Type
            Dim listAPI = From entryPoint As MethodInfo
                          In Methods
                          Let attrs As Object() =
                              entryPoint.GetCustomAttributes(attributeType:=API_DEFINE, inherit:=True)
                          Where Not attrs.IsNullOrEmpty
                          Let API As ExportAPIAttribute =
                              DirectCast(attrs(Scan0), ExportAPIAttribute)
                          Select (entryPoint:=entryPoint, API:=API)

            Dim LQuery As APIInvoker() = LinqAPI.Exec(Of APIInvoker) <=
 _
                From m As (entryPoint As MethodInfo, API As ExportAPIAttribute)
                In listAPI
                Let entryPoint As MethodInfo = m.entryPoint
                Let api As ExportAPIAttribute = m.API
                Let attr As Object =
                    entryPoint.GetCustomAttributes(GetType(APIMethod), True).ElementAtOrDefault(Scan0)
                Let warning = VBDebugger.Assert(
                    Not attr Is Nothing,
                    $"Not found {GetType(APIMethod).FullName} definition on the method: {api.Name}, you should specific one of this http method custom attribute: GET/POST/DELETE.",
                    $"API: {api.Name}")                                    ' 由于rest服务需要返回json、所以在API的申明的时候还需要同时申明GET、POST里面所返回的json对象的类型，
                Where Not attr Is Nothing
                Let httpMethod As APIMethod = DirectCast(attr, APIMethod)  ' 假若程序是在这里出错的话，则说明有API函数没有进行GET、POST的json类型申明，找到该函数补全即可
                Let invoke = New APIInvoker With {
                    .Name = api.Name.ToLower,
                    .EntryPoint = entryPoint,
                    .Help = api.PrintView(HTML:=True) & $"<br /><div>{httpMethod.GetMethodHelp(entryPoint)}</div>"
                }
                Select invoke
                Order By Len(invoke.Name) Descending

            Return New APPEngine With {
                .API = LQuery _
                    .ToDictionary(Function(api) api.Name.ToLower),
                ._Application = obj,
                ._Namespace = ns
            }
        End Function

        ''' <summary>
        ''' If returns false, which means this function can not parsing any arguments parameter from the input url.
        ''' (返回False标识无法正确的解析出调用数据)
        ''' </summary>
        ''' <param name="url">Url inputs from the user browser.</param>
        ''' <param name="application"></param>
        ''' <param name="API">小写的url</param>
        ''' <param name="parameters">URL参数</param>
        ''' <returns></returns>
        Public Shared Function GetParameter(url As String, ByRef application As String, ByRef API As String, ByRef parameters As String) As Boolean
            Dim p As Integer = InStr(url, "?")
            Dim tokens$() = url.Split("/"c).Skip(1).ToArray

            If tokens.IsNullOrEmpty Then
                Return False
            End If

            application = tokens(Scan0)

            If p > 0 Then '带有参数
                API = Mid(url, 1, p - 1) '/application/function
                parameters = Mid(url, p + 1)

                If tokens.Count = 1 Then
                    application = application.Split("?"c).First
                End If
            Else
                API = url
            End If

            application = application.ToLower
            If Not String.IsNullOrEmpty(API) Then
                API = API.ToLower
            End If

            Return True
        End Function
    End Class
End Namespace
