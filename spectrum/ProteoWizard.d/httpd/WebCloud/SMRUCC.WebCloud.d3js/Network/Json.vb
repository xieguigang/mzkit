Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Network.JSON

    Public Class Regulation

        <Column("ORF ID")> Public Property ORF_ID As String
        Public Property MotifFamily As String
        Public Property Regulator As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' helper class for json text generation 
    ''' </summary>
    Public Structure out

        ''' <summary>
        ''' 网络之中的节点对象
        ''' </summary>
        ''' <returns></returns>
        Public Property nodes As node()
        ''' <summary>
        ''' 节点之间的边链接
        ''' </summary>
        ''' <returns></returns>
        Public Property links As link()       

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Class node : Implements IAddressOf, INamedValue

        Public Property name As String Implements INamedValue.Key
        Public Property group As Integer
        Public Property size As Integer
        Public Property type As String
        Public Property Address As Integer Implements IAddressOf.Address
        Public Property color As String 

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class link

        Public Property source As Integer
        Public Property target As Integer
        Public Property value As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace