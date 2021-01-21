Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class WorkspaceFile

    ''' <summary>
    ''' 原始数据文件的缓存对象列表
    ''' </summary>
    Public Property cacheFiles As Dictionary(Of String, Raw())

    ''' <summary>
    ''' 自动化脚本的文件路径列表
    ''' </summary>
    Public Property scriptFiles As String()

    ''' <summary>
    ''' 打开的脚本
    ''' </summary>
    ''' <returns></returns>
    Public Property openedScripts As String()
    ''' <summary>
    ''' 编辑之后尚未保存的脚本
    ''' </summary>
    ''' <returns></returns>
    Public Property unsavedScripts As NamedValue()

End Class
