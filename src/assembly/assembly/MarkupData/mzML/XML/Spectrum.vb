#Region "Microsoft.VisualBasic::4a988678a96566eb80ca0e178e8c080f, src\assembly\assembly\MarkupData\mzML\XML\Spectrum.vb"

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

    '     Class spectrumList
    ' 
    '         Properties: spectrums
    ' 
    '         Function: GetAllMs1
    ' 
    '     Class spectrum
    ' 
    '         Properties: controllerNumber, controllerType, ms_level, scan, scan_time
    '                     scanList
    ' 
    '         Function: ToString
    ' 
    '     Class scanList
    ' 
    '         Properties: cvParams, scans
    ' 
    '     Class scan
    ' 
    '         Properties: instrumentConfigurationRef
    ' 
    '     Class scanWindowList
    ' 
    '         Properties: scanWindows
    ' 
    '     Class scanWindow
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
            Static noTitle As [Default](Of String) = "Unknown title"
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
