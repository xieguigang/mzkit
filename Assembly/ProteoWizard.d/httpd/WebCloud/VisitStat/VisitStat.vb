#Region "Microsoft.VisualBasic::ff580cb6d6750e1bfc932c1b2d591674, ..\httpd\WebCloud\VisitStat\VisitStat.vb"

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

Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Public Class VisitStat : Inherits Plugins.PluginBase

    ReadOnly _transactions As New List(Of visitor_stat)
    ReadOnly _commitThread As UpdateThread
    ReadOnly _mySQL As New MySQL

    ''' <summary>
    ''' ```bash
    ''' /@set "host=localhost;mysql_port=3306;user=root;password=1234;database=test"
    ''' ```
    ''' </summary>
    ''' <param name="platform"></param>
    Sub New(platform As PlatformEngine)
        Call MyBase.New(platform)
        _commitThread = New UpdateThread(60 * 1000, AddressOf __commits)

#Region "通过环境变量来初始化mysql连接"

        If _mySQL <= New ConnectionUri With {
            .Database = App.GetVariable("database"),
            .IPAddress = App.GetVariable("host"),
            .Password = App.GetVariable("password"),
            .ServicesPort = App.GetVariable("mysql_port"),
            .User = App.GetVariable("user")
        } = -1.0R Then

#Disable Warning
            Dim ex As New Exception("Unable establish mysql connection!")
            Dim environment$ = App _
                .GetAppVariables _
                .ToDictionary(Function(k) k.Name,
                              Function(k) k.Value) _
                .GetJson

            Throw New ExecutionEngineException(environment, ex)
#Enable Warning

        End If
#End Region
    End Sub

    Private Sub __commits()
        If _transactions.IsNullOrEmpty Then
            Return
        End If

        Call _mySQL.CommitInserts(_transactions)
        Call _transactions.Clear()
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        _commitThread.Stop()
        __commits()
        MyBase.Dispose(disposing)
    End Sub

    Public Overrides Sub handleVisit(p As HttpRequest, success As Boolean)
        Dim ip As String = p.Remote
        Dim visit As New visitor_stat With {
            .ip = ip,
            .method = p.HTTPMethod,
            .success = success,
            .time = Now,
            .ua = p.HttpHeaders(""),
            .url = p.URL
        }
        Call _transactions.Add(visit)
    End Sub
End Class
