Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language.Default

Namespace MarkupData.mzML

    Public Class spectrumList : Inherits DataList

        <XmlElement("spectrum")>
        Public Property spectrums As spectrum()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllMs1() As IEnumerable(Of spectrum)
            Return spectrums.GetAllMs1
        End Function
    End Class

    Public Class spectrum : Inherits BinaryData

        <XmlAttribute> Public Property controllerType As String
        <XmlAttribute> Public Property controllerNumber As String
        <XmlAttribute> Public Property scan As String

        Public Property scanList As scanList

        Public ReadOnly Property ms_level As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return cvParams.KeyItem("ms level")?.value
            End Get
        End Property

        ''' <summary>
        ''' 返回来的时间结果值是以秒为单位的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property scan_time As Double
            Get
                With scanList.scans(0).cvParams.KeyItem("scan start time")
                    Dim time# = Val(.value)

                    If .unitName = "second" Then
                        Return time
                    ElseIf .unitName = "minute" Then
                        Return time * 60
                    Else
                        Throw New NotImplementedException(.unitName)
                    End If
                End With
            End Get
        End Property

        Public Overrides Function ToString() As String
            Static noTitle As DefaultValue(Of String) = "Unknown title"
            Return cvParams.KeyItem("spectrum title")?.value Or noTitle
        End Function
    End Class

    Public Class scanList : Inherits List

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()
        <XmlElement("scan")>
        Public Property scans As scan()

    End Class

    Public Class scan : Inherits Params

        <XmlAttribute>
        Public Property instrumentConfigurationRef As String
    End Class

    Public Class scanWindowList : Inherits List
        <XmlElement(NameOf(scanWindow))>
        Public Property scanWindows As scanWindow()
    End Class

    Public Class scanWindow : Inherits Params
    End Class
End Namespace