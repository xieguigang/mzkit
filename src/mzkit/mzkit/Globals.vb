#Region "Microsoft.VisualBasic::751e644f0cbfa144734671ec1299cc77, src\mzkit\mzkit\Globals.vb"

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
    '     Function: CurrentRawFile, LoadRawFileCache
    ' 
    '     Sub: (+2 Overloads) addRawFile, SaveRawFileCache
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Task

Module Globals

    ReadOnly cacheList As String = App.LocalData & "/cacheList.dat"

    Public ReadOnly Property Settings As Settings

    Sub New()
        Settings = Settings.GetConfiguration()
    End Sub

    <Extension>
    Public Sub SaveRawFileCache(explorer As TreeView)
        Dim files As New List(Of Task.Raw)

        For Each node As TreeNode In explorer.Nodes
            files.Add(node.Tag)
        Next

        Dim obj As Dictionary(Of String, Raw) = files.ToDictionary(Function(raw) raw.source.FileName)
        Dim schema = obj.GetType
        Dim model As JsonElement = schema.GetJsonElement(obj, New JSONSerializerOptions)

        Using buffer = cacheList.Open(doClear:=True)
            Call DirectCast(model, JsonObject).WriteBuffer(buffer)
        End Using
    End Sub

    <Extension>
    Public Function GetTotalCacheSize(explorer As TreeView) As String
        Dim size As Double

        For Each node As TreeNode In explorer.Nodes
            size += DirectCast(node.Tag, Raw).cache.FileLength
        Next

        If size = 0.0 Then
            Return "0 KB"
        Else
            Return Lanudry(size)
        End If
    End Function

    <Extension>
    Public Function LoadRawFileCache(explorer As TreeView) As Integer
        Dim rawBuffer As Byte() = cacheList.ReadBinary

        If rawBuffer.IsNullOrEmpty Then
            Return 0
        End If

        Dim files As Dictionary(Of String, Raw) = rawBuffer _
            .DoCall(AddressOf BSONFormat.Load) _
            .CreateObject(GetType(Dictionary(Of String, Raw)))
        Dim i As Integer

        For Each raw As Raw In files.SafeQuery.Values
            Call explorer.addRawFile(raw)
            i += 1
        Next

        Return i
    End Function

    <Extension>
    Public Sub addRawFile(explorer As TreeView, raw As Raw)
        Dim rawFileNode As New TreeNode($"{raw.source.FileName} [{raw.numOfScans} Scans]") With {
                .Checked = True,
                .Tag = raw
            }

        explorer.Nodes.Add(rawFileNode)
        rawFileNode.addRawFile(raw, True, True)
    End Sub

    <Extension>
    Public Sub addRawFile(rawFileNode As TreeNode, raw As Raw, ms1 As Boolean, ms2 As Boolean)
        For Each scan As ScanEntry In raw.scans
            If scan.mz = 0 AndAlso Not ms1 Then
                Continue For
            End If
            If scan.mz > 0 AndAlso Not ms2 Then
                Continue For
            End If

            Dim scanNode As New TreeNode(scan.id) With {
                .Tag = scan,
                .ToolTipText = "m/z: " & scan.mz
            }

            rawFileNode.Nodes.Add(scanNode)
        Next
    End Sub

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
End Module
