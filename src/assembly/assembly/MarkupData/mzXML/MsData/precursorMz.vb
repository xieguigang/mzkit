Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MarkupData.mzXML

    ''' <summary>
    ''' 这个类型模型的隐式转换的数据来源为<see cref="precursorMz.value"/>属性值
    ''' </summary>
    Public Structure precursorMz : Implements IComparable(Of precursorMz)

        <XmlAttribute> Public Property windowWideness As String
        <XmlAttribute> Public Property precursorCharge As Double
        ''' <summary>
        ''' 母离子可以从这个属性指向的ms1 scan获取，这个属性对应着<see cref="scan.num"/>属性
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property precursorScanNum As String
        <XmlAttribute> Public Property precursorIntensity As Double
        <XmlAttribute> Public Property activationMethod As String
        <XmlText>
        Public Property value As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CompareTo(other As precursorMz) As Integer Implements IComparable(Of precursorMz).CompareTo
            Return Me.value.CompareTo(other.value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(mz As precursorMz) As Double
            Return mz.value
        End Operator
    End Structure
End Namespace