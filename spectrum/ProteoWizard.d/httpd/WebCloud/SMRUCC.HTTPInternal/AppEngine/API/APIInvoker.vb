#Region "Microsoft.VisualBasic::f86510328addd0a3495c92ebc3e928c5, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\API\APIInvoker.vb"

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
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments

Namespace AppEngine.APIMethods

    ''' <summary>
    ''' WebApp API的抽象接口
    ''' </summary>
    ''' <param name="api">URL</param>
    ''' <param name="request">URL后面的参数请求</param>
    ''' <param name="response">返回的html页面的文档</param>
    ''' <returns>是否执行成功</returns>
    Public Delegate Function APIAbstract(api As String, request As HttpRequest, response As HttpResponse) As Boolean

    ''' <summary>
    ''' <see cref="[GET]"/> API interface
    ''' </summary>
    ''' <param name="request">url arguments</param>
    ''' <param name="response">output json or html page</param>
    ''' <returns>Execute success or not?</returns>
    Public Delegate Function _GET(request As HttpRequest, response As HttpResponse) As Boolean

    ''' <summary>
    ''' <see cref="POST"/> API interface
    ''' </summary>
    ''' <param name="request">url arguments and Form data</param>
    ''' <param name="response"></param>
    ''' <returns>Execute success or not?</returns>
    Public Delegate Function _POST(request As HttpPOSTRequest, response As HttpResponse) As Boolean

    Public Class APIInvoker : Implements INamedValue

        Public Property Name As String Implements INamedValue.Key
        Public Property EntryPoint As MethodInfo
        Public Property Help As String
        Public Property Method As APIMethod

        Public Overrides Function ToString() As String
            Return Name
        End Function

        <POST(GetType(Boolean))>
        Public Function InvokePOST(App As Object, request As HttpPOSTRequest, response As HttpResponse) As Boolean
            Try
                Return __invokePOST(App, request, response)
            Catch ex As Exception
                Return __handleERROR(ex, request.URL, response)
            End Try
        End Function

        ''' <summary>
        ''' 在API的函数调用的位置，就只需要有args这一个参数
        ''' </summary>
        ''' <returns></returns>
        <[GET](GetType(Boolean))>
        Public Function Invoke(App As Object, request As HttpRequest, response As HttpResponse) As Boolean
            Try
                Return __invoke(App, request, response)
            Catch ex As Exception
                Return __handleERROR(ex, request.URL, response)
            End Try
        End Function

        Private Function __handleERROR(ex As Exception, url As String, ByRef response As HttpResponse) As Boolean
            Dim result As String
            ex = New Exception("Request page: " & url, ex)

#If DEBUG Then
            result = ex.ToString
#Else
            result = Fakes(ex.ToString)
#End If
            Call App.LogException(ex)
            Call ex.PrintException
            Call response.Write404(result)

            Return False
        End Function

        Private Function VirtualPath(strData As String(), prefix As String) As Dictionary(Of String, String)
            Dim LQuery = From source As String
                         In strData
                         Let trimPrefix = Regex.Replace(source, "in [A-Z][:]\\", "", RegexOptions.IgnoreCase)
                         Let line = Regex.Match(trimPrefix, "[:]line \d+").Value
                         Let path = trimPrefix.Replace(line, "")
                         Select source,
                             path

            Dim LTokens = (From obj
                           In LQuery
                           Let tokens As String() = obj.path.Split("\"c)
                           Select tokens,
                               obj.source).ToArray
            Dim p As Integer

            If LTokens.Length = 0 Then
                Return New Dictionary(Of String, String)
            End If

            For i As Integer = 0 To (From obj In LTokens Select obj.tokens.Length).Min - 1
                p = i

                If (From n In LTokens Select n.tokens(p) Distinct).Count > 1 Then
                    Exit For
                End If
            Next

            Dim LSkips = (From obj In LTokens Select obj.source, obj.tokens.Skip(p).ToArray).ToArray
            Dim LpreFakes = (From obj In LSkips
                             Select obj.source,
                                 virtual = String.Join("/", obj.ToArray).Replace(".vb", ".vbs")).ToArray
            Dim hash As Dictionary(Of String, String) = LpreFakes.ToDictionary(
                Function(obj) obj.source,
                Function(obj) $"in {prefix}/{obj.virtual}:line {CInt(5000 * Rnd() + 100)}")
            Return hash
        End Function

        Const virtual As String = "/root/ubuntu.d~/->/wwwroot/~azure.microsoft.com/api.vbs?virtual=ms_visualBasic_sh:/"

        Private Function Fakes(ex As String) As String
            Dim line As String() = Regex.Matches(ex, "in .+?[:]line \d+").ToArray
            Dim hash As Dictionary(Of String, String) = VirtualPath(line, virtual)
            Dim sbr As New StringBuilder(ex)

            For Each obj In hash
                Call sbr.Replace(obj.Key, obj.Value)
            Next

            Return sbr.ToString
        End Function

        Private Function __invokePOST(App As Object, request As HttpPOSTRequest, response As HttpResponse) As Boolean
            Dim value As Object = EntryPoint.Invoke(App, {request, response})
            Return DirectCast(value, Boolean)
        End Function

        Private Function __invoke(App As Object, request As HttpRequest, response As HttpResponse) As Boolean
            Dim value As Object = EntryPoint.Invoke(App, {request, response})
            Return DirectCast(value, Boolean)
        End Function
    End Class
End Namespace
