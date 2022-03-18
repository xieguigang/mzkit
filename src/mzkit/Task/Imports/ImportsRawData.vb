#Region "Microsoft.VisualBasic::9f97a5ce6224a263f4df28570df577c7, mzkit\src\mzkit\Task\Imports\ImportsRawData.vb"

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


    ' Code Statistics:

    '   Total Lines: 40
    '    Code Lines: 31
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.43 KB


    ' Class ImportsRawData
    ' 
    '     Properties: raw
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetCachePath
    ' 
    '     Sub: RunImports
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline

Public Class ImportsRawData

    ReadOnly source As String
    ReadOnly cache As String
    ReadOnly showProgress As Action(Of String)
    ReadOnly success As Action

    Public ReadOnly Property raw As Raw

    Sub New(file As String, progress As Action(Of String), finished As Action, Optional cachePath As String = Nothing)
        source = file
        cache = If(cachePath, GetCachePath(file))
        showProgress = progress
        success = finished
        raw = New Raw With {
            .cache = cache.GetFullPath,
            .source = source.GetFullPath
        }
    End Sub

    Public Shared Function GetCachePath(file As String) As String
        Dim cacheKey As String = file.GetFullPath.MD5
        Dim path As String = App.AppSystemTemp & $"/.cache/{cacheKey.Substring(0, 2)}/" & cacheKey & ".mzPack"

        Return path
    End Function

    Public Sub RunImports()
        Dim Rscript As String = RscriptPipelineTask.GetRScript("mzpack.R")
        Dim cli As String = $"""{Rscript}"" --mzXML ""{raw.source}"" --cache ""{raw.cache}"""
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)

        AddHandler pipeline.Finish, AddressOf success.Invoke
        AddHandler pipeline.SetMessage, AddressOf showProgress.Invoke

        Call pipeline.Run()
    End Sub
End Class
