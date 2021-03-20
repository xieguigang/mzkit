#Region "Microsoft.VisualBasic::57015c606b43fc535d8df35b8bac7720, src\mzkit\Task\ImportsRawData.vb"

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

' Class ImportsRawData
' 
'     Properties: raw
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: importsMzML, importsMzXML, RunImports
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly

Public Class ImportsRawData

    ReadOnly source As String
    ReadOnly cache As String
    ReadOnly showProgress As Action(Of String)
    ReadOnly success As Action

    Public ReadOnly Property raw As Raw

    Sub New(file As String, progress As Action(Of String), finished As Action)
        Dim cacheKey As String = file.GetFullPath.MD5

        source = file
        cache = App.AppSystemTemp & $"/mzkit_win32/.cache/{cacheKey.Substring(0, 2)}/" & cacheKey & ".mzPack"
        showProgress = progress
        success = finished
        raw = New Raw With {
            .cache = cache.GetFullPath,
            .source = source.GetFullPath
        }
    End Sub

    Public Sub RunImports()
        Dim mzpack As mzPack = Converter.LoadRawFileAuto(source, showProgress)

        showProgress("Create snapshot...")
        mzpack.Thumbnail = mzpack.DrawScatter

        Using file As Stream = cache.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call showProgress("Write mzPack cache data...")
            Call mzpack.Write(file)
        End Using

        Call showProgress("Job Done!")
        Call Thread.Sleep(1500)
        Call success()
    End Sub
End Class
