#Region "Microsoft.VisualBasic::16e867032a97480ab9cf20db9534b070, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\ExternalCall.vb"

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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace AppEngine

    ''' <summary>
    ''' 调用和注册外部模块为rest服务的插件，从这里拓展核心服务层
    ''' </summary>
    Public Module ExternalCall

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="platform"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 只要是.NET assembly都会进行扫描，否则使用<see cref="DebuggerAPI"/>进行调试的时候
        ''' 就不能够正确的加载对象了，所以在这里取消掉exe的限制
        ''' </remarks>
        Public Iterator Function Scan(platform As PlatformEngine) As IEnumerable(Of String)
            Dim dlls As IEnumerable(Of String) = ls - l - {"*.dll", "*.exe"} <= App.HOME

            For Each dllFile As String In dlls
                Try
                    If ParseDll(dllFile, platform) > 0 Then
                        Yield dllFile
                    End If
                Catch ex As Exception
                    ex = New Exception(dllFile, ex)
                    Call ex.PrintException
                    Call App.LogException(ex)
                End Try
            Next
        End Function

        ''' <summary>
        ''' Register external WebApp as services.
        ''' </summary>
        ''' <param name="dll"></param>
        ''' <param name="platform"></param>
        ''' <returns></returns>
        Public Function ParseDll(dll As String, platform As PlatformEngine) As Integer
            Dim assm As Reflection.Assembly = Reflection.Assembly.LoadFile(dll)
            Dim types As Type() =
                LinqAPI.Exec(Of Type) <= From typeDef As Type
                                         In assm.GetTypes
                                         Where typeDef.IsInheritsFrom(GetType(WebApp)) _
                                             AndAlso
                                             Not typeDef.IsAbstract
                                         Select typeDef
            If types.Length = 0 Then
                Return -1
            End If

            Dim Apps As WebApp() =
                types.ToArray(Of WebApp)(
                Function(typeDef As Type) DirectCast(Activator.CreateInstance(typeDef, {platform}), WebApp))

            For Each app As WebApp In Apps
                Call platform.AppManager.Register(app)
            Next

            Return Apps.Length
        End Function
    End Module
End Namespace
