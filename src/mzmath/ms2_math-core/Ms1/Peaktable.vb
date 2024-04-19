#Region "Microsoft.VisualBasic::6f2a31bbec89a3ad7a3379478dab08c1, mzkit\src\mzmath\ms2_math-core\Ms1\Peaktable.vb"

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

'   Total Lines: 71
'    Code Lines: 43
' Comment Lines: 22
'   Blank Lines: 6
'     File Size: 2.07 KB


' Class Peaktable
' 
'     Properties: annotation, energy, index, intb, into
'                 ionization, maxo, mz, mzmax, mzmin
'                 name, rt, rtmax, rtmin, rtMinute
'                 sample, scan, sn
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

''' <summary>
''' 一级信息表
''' </summary>
Public Class Peaktable
    Implements IMs1
    Implements IRetentionTime
    Implements IROI
    Implements IMS1Annotation
    Implements IMassBin

    ''' <summary>
    ''' 可以是差异代谢物的编号
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' xcms_id
    ''' </remarks>
    Public Property name As String Implements IMS1Annotation.Key
    Public Property mz As Double Implements IMs1.mz, IMassBin.mass
    Public Property mzmin As Double Implements IMassBin.min
    Public Property mzmax As Double Implements IMassBin.max
    Public Property rt As Double Implements IMs1.rt

    ''' <summary>
    ''' 以分钟为单位的保留时间
    ''' </summary>
    ''' <returns></returns>
    <Column(Name:="rt(minute)")>
    Public ReadOnly Property rtMinute As Double
        Get
            Return rt / 60
        End Get
    End Property

    Public Property rtmin As Double Implements IROI.rtmin
    Public Property rtmax As Double Implements IROI.rtmax
    Public Property into As Double Implements IMS1Annotation.intensity
    Public Property intb As Double
    Public Property maxo As Double
    Public Property sn As Double
    Public Property sample As String
    Public Property index As Double
    ''' <summary>
    ''' The scan number
    ''' </summary>
    ''' <returns></returns>
    Public Property scan As Integer
    ''' <summary>
    ''' CID/HCD
    ''' </summary>
    ''' <returns></returns>
    Public Property ionization As String
    Public Property energy As String
    Public Property annotation As String Implements IMS1Annotation.precursor_type

    Public Overrides Function ToString() As String
        Dim display As String = $"#{scan} {mz}@{rt}_{ionization}_{energy}V"

        If annotation.StringEmpty Then
            Return display
        Else
            Return $"{display}: {annotation}"
        End If
    End Function
End Class
