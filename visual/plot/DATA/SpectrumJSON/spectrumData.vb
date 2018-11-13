#Region "Microsoft.VisualBasic::eaeb1284cf47110f61a97fb42ded23f5, plot\DATA\SpectrumJSON\spectrumData.vb"

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

    '     Class MetlinData
    ' 
    '         Properties: Data, url
    ' 
    '         Function: Load
    ' 
    '     Class SpectrumData
    ' 
    '         Properties: data, name
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
