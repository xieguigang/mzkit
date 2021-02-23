#Region "Microsoft.VisualBasic::5f6271966ba9c642477c10ecb7c3dd67, assembly\MarkupData\mzXML\MsData\precursorMz.vb"

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

    '     Structure precursorMz
    ' 
    '         Properties: activationMethod, precursorCharge, precursorIntensity, precursorScanNum, value
    '                     windowWideness
    ' 
    '         Function: CompareTo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
