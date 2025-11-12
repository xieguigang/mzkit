#Region "Microsoft.VisualBasic::81acf754eaf0d6da11d124a87926634a, mzkit\services\MZWork\WorkspaceFile.vb"

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

    '   Total Lines: 35
    '    Code Lines: 15 (42.86%)
    ' Comment Lines: 14 (40.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (17.14%)
    '     File Size: 1.12 KB


    ' Class WorkspaceFile
    ' 
    '     Properties: cacheFiles, openedScripts, scriptFiles, unsavedScripts
    ' 
    '     Function: PopulateMzPacks
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class WorkspaceFile

    ''' <summary>
    ''' 原始数据文件的缓存对象列表
    ''' </summary>
    Public Property cacheFiles As New Dictionary(Of String, Raw())

    ''' <summary>
    ''' 自动化脚本的文件路径列表
    ''' </summary>
    Public Property scriptFiles As String() = {}

    ''' <summary>
    ''' 打开的脚本
    ''' </summary>
    ''' <returns></returns>
    Public Property openedScripts As String() = {}
    ''' <summary>
    ''' 编辑之后尚未保存的脚本
    ''' </summary>
    ''' <returns></returns>
    Public Property unsavedScripts As NamedValue() = {}

    Public Iterator Function PopulateMzPacks(println As Action(Of String, String)) As IEnumerable(Of mzPack)
        For Each raw As Raw In cacheFiles.Values.IteratesALL
            Yield raw.LoadMzpack(println).GetLoadedMzpack
        Next
    End Function

End Class
