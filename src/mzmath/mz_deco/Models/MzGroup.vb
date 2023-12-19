#Region "Microsoft.VisualBasic::7755755ecd70ff9dd096754324b7fc87, mzkit\src\mzmath\mz_deco\Models\MzGroup.vb"

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


' Code Statistics:

'   Total Lines: 8
'    Code Lines: 5
' Comment Lines: 0
'   Blank Lines: 3
'     File Size: 185 B


' Class MzGroup
' 
'     Properties: mz, XIC
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

''' <summary>
''' XIC dataset that used for deconv
''' </summary>
''' <remarks>
''' A collection of the <see cref="ChromatogramTick"/> data that 
''' tagged with a numeric m/z value.
''' </remarks>
Public Class MzGroup

    ''' <summary>
    ''' target ion m/z
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property mz As Double
    ''' <summary>
    ''' the chromatogram data of current target ion
    ''' </summary>
    ''' <returns></returns>
    <XmlElement>
    Public Property XIC As ChromatogramTick()

    Public ReadOnly Property size As Integer
        Get
            Return XIC.TryCount
        End Get
    End Property

    Public ReadOnly Property rt As DoubleRange
        Get
            Return New DoubleRange(From t As ChromatogramTick In XIC Select t.Time)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return mz
    End Function

End Class
