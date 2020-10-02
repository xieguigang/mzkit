Public Class WorkspaceFile

    ''' <summary>
    ''' 原始数据文件的缓存对象列表
    ''' </summary>
    Public Property cacheFiles As Dictionary(Of String, Raw())

    ''' <summary>
    ''' 自动化脚本的文件路径列表
    ''' </summary>
    Public Property scriptFiles As String()

End Class
