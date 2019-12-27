#Region "Microsoft.VisualBasic::12becd26b3d929a40f71af2ceaa2d0d5, Assembly\ProteoWizard.d\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: EnsureZipExtract, normalizePath
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip

Public Module Extensions

    ''' <summary>
    ''' OSS存储的文件系统位置
    ''' </summary>
    ReadOnly OSS_ROOT$

    Sub New()
        OSS_ROOT = App.GetVariable("oss")

        Call $"OSS_ROOT={OSS_ROOT}".__INFO_ECHO

        If Not OSS_ROOT.DirectoryExists Then
            Throw New Exception("OSS file system should be mounted at first!")
        End If
    End Sub

    ''' <summary>
    ''' 确保输入的源文件不是zip文件压缩包，如果目标文件是zip压缩包，则进行解压缩
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Function EnsureZipExtract(path As String) As String
        If path.ExtensionSuffix.TextEquals("zip") Then
            ' 对zip文件进行解压缩
            Dim zipFolder$ = path.ParentPath & "/" & path.BaseName

            UnZip.ImprovedExtractToDirectory(path, zipFolder, Overwrite.Always, extractToFlat:=True)
            path.SetValue(zipFolder)
        End If

        Return path
    End Function

    Friend Function normalizePath(path As String) As String
        ' Add OSS drive location if the given path is a relative path
        If InStr(path, ":\") = 0 AndAlso InStr(path, ":/") = 0 Then
            path = OSS_ROOT & "/" & path
        End If

        Return path
    End Function
End Module
