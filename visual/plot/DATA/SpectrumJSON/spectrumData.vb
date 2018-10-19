Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DATA.SpectrumJSON

    Public Class MetlinData

        Public Property url As String
        Public Property Data As SpectrumData()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load(json As String) As MetlinData
            Return New MetlinData With {
                .url = json,
                .Data = json.LoadJSON(Of SpectrumData())
            }
        End Function
    End Class

    ''' <summary>
    ''' 某一个标准品的一个MS/MS二级质谱数据
    ''' </summary>
    Public Class SpectrumData

        ''' <summary>
        ''' 库的名称
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' 二级碎片
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("data")>
        Public Property data As IntensityCoordinate()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace