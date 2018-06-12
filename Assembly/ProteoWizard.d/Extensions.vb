Imports Microsoft.VisualBasic.ApplicationServices

Module Extensions

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
    Friend Function ensureZipExtract(path As String) As String
        If path.ExtensionSuffix.TextEquals("zip") Then
            ' 对zip文件进行解压缩
            Dim zipFolder$ = path.ParentPath & "/" & path.BaseName

            GZip.ImprovedExtractToDirectory(path, zipFolder, Overwrite.Always, extractToFlat:=True)
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
