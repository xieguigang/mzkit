Imports System.ComponentModel

Namespace SmileWei.EmbeddedApp
    Partial Class AppContainer
        ''' <summary>
        ''' 必需的设计器变量。
        ''' </summary>
        Private components As IContainer = Nothing

        ''' <summary> 
        ''' 清理所有正在使用的资源。
        ''' </summary>
        ''' <paramname="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

#Region "组件设计器生成的代码"

        ''' <summary>
        ''' 设计器支持所需的方法 - 不要
        ''' 使用代码编辑器修改此方法的内容。
        ''' </summary>
        Private Sub InitializeComponent()
            components = New Container()
        End Sub

#End Region
    End Class
End Namespace
