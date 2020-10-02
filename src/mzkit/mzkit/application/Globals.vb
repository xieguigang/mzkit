#Region "Microsoft.VisualBasic::3cebafd485838873edbf7af88a066741, src\mzkit\mzkit\application\Globals.vb"

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

    ' Module Globals
    ' 
    '     Properties: Settings
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CheckFormOpened, CurrentRawFile, GetColors, GetTotalCacheSize, LoadRawFileCache
    ' 
    '     Sub: (+2 Overloads) addRawFile, AddRecentFileHistory, SaveRawFileCache
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Task

Module Globals

    Dim defaultWorkspace As String = App.LocalData & "/cacheList.dat"

    Public ReadOnly Property Settings As Settings

    Sub New()
        Settings = Settings.GetConfiguration()
    End Sub

    Public Sub AddRecentFileHistory(file As String)
        Settings.recentFiles = {file}.JoinIterates(Settings.recentFiles).Distinct.ToArray
        Settings.Save()
    End Sub

    Public Function GetColors() As String
        Return Globals.Settings.viewer.colorSet.JoinBy(",")
    End Function

    <Extension>
    Public Sub SaveRawFileCache(explorer As TreeView, progress As Action(Of String))
        Dim files As New List(Of Task.Raw)

        For Each node As TreeNode In explorer.Nodes
            files.Add(node.Tag)
            progress(files.Last.source)
            Application.DoEvents()
        Next

        ' fix for duplicated file name
        Dim obj As Dictionary(Of String, Raw()) = files _
            .GroupBy(Function(raw) raw.source.FileName) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)

        Dim schema As Type = obj.GetType
        Dim model As JsonElement = schema.GetJsonElement(obj, New JSONSerializerOptions)

        progress("write workspace file...")

        Using buffer = defaultWorkspace.Open(doClear:=True)
            Call DirectCast(model, JsonObject).WriteBuffer(buffer)
        End Using

        Application.DoEvents()
    End Sub

    <Extension>
    Public Iterator Function FindRaws(explorer As TreeView, sourceName As String) As IEnumerable(Of Raw)
        For Each node As TreeNode In explorer.Nodes
            Dim raw As Raw = DirectCast(node.Tag, Raw)

            If raw.source.FileName = sourceName Then
                Yield raw
            End If
        Next
    End Function

    <Extension>
    Public Function GetTotalCacheSize(explorer As TreeNode) As String
        Dim size As Double

        For Each node As TreeNode In explorer.Nodes
            size += DirectCast(node.Tag, Raw).GetCacheFileSize
        Next

        If size = 0.0 Then
            Return "0 KB"
        Else
            Return Lanudry(size)
        End If
    End Function

    Friend SplashScreenUpdater As Action(Of String)

    ''' <summary>
    ''' two root nodes:
    ''' 
    ''' 1. Raw Data Files
    ''' 2. R# Automation
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <param name="defaultWorkspace"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadRawFileCache(explorer As TreeView, Optional defaultWorkspace As String = Nothing) As Integer
        If defaultWorkspace.StringEmpty Then
            defaultWorkspace = Globals.defaultWorkspace
        End If

        If Not defaultWorkspace.FileExists Then
            Return 0
        Else
            Call SplashScreenUpdater("Load raw file list...")
        End If

        Dim files As ViewerProject = ViewerProject.LoadWorkspace(defaultWorkspace, SplashScreenUpdater)
        Dim i As Integer
        Dim rawFiles As New TreeNode("Raw Data Files")

        For Each raw As Raw In files.GetRawDataFiles
            Call SplashScreenUpdater($"[Raw File Viewer] Loading {raw.source.FileName}...")

            Dim rawFileNode As New TreeNode($"{raw.source.FileName} [{raw.numOfScans} Scans]") With {
                .Checked = True,
                .Tag = raw
            }

            rawFiles.Nodes.Add(rawFileNode)
            'rawFileNode.addRawFile(raw, True, True)
            rawFileNode.Checked = False

            i += 1
        Next

        explorer.Nodes.Add(rawFiles)

        If files.GetAutomationScripts.SafeQuery.Count > 0 Then
            Dim scripts As New TreeNode("R# Automation")

            For Each script As String In files.GetAutomationScripts
                Dim fileNode As New TreeNode(script.FileName) With {
                    .Checked = False,
                    .Tag = script
                }

                scripts.Nodes.Add(fileNode)
            Next

            explorer.Nodes.Add(scripts)
        End If

        Return i
    End Function

    <Extension>
    Public Sub addRawFile(rawFileNode As TreeNode, raw As Raw)
        For Each scan As Ms1ScanEntry In raw.scans
            Dim scanNode As New TreeNode(scan.id) With {
                .Tag = scan
            }

            rawFileNode.Nodes.Add(scanNode)

            For Each ms2 As ScanEntry In scan.products.SafeQuery
                Dim productNode As New TreeNode(ms2.id) With {.Tag = ms2}

                scanNode.Nodes.Add(productNode)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 这个函数总是返回当前选中的节点的文件根节点相关的数据
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CurrentRawFile(explorer As TreeView) As (raw As Raw, tree As TreeNode)
        Dim node = explorer.SelectedNode

        If node Is Nothing Then
            Return Nothing
        ElseIf TypeOf node.Tag Is Raw Then
            Return (DirectCast(node.Tag, Raw), node)
        Else
            Return (DirectCast(node.Parent.Tag, Raw), node.Parent)
        End If
    End Function

    Public Function CheckFormOpened(form As Form) As Boolean
        For i As Integer = 0 To Application.OpenForms.Count - 1
            If Application.OpenForms.Item(i) Is form Then
                Return True
            End If
        Next

        Return False
    End Function

    <Extension>
    Public Function GetXICMaxYAxis(raw As Raw) As Double
        Dim XIC As Double() = raw _
            .GetMs2Scans _
            .Select(Function(a) a.XIC) _
            .ToArray

        If XIC.Length = 0 Then
            Return 0
        Else
            Return XIC.Max
        End If
    End Function
End Module
