Module ServerHelper

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

    Public Function NormalizeOSSPath(path As String) As String
        ' Add OSS drive location if the given path is a relative path
        If InStr(path, ":\") = 0 AndAlso InStr(path, ":/") = 0 Then
            path = OSS_ROOT & "/" & path
        End If

        Return path
    End Function
End Module
