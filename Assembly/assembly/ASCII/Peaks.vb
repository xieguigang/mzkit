#Region "Microsoft.VisualBasic::10ac7af36e3f1155687780ab2962def0, assembly\ASCII\Peaks.vb"

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

    ' Structure MSMSPeak
    ' 
    '     Properties: comment, intensity, mz
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Public Structure MSMSPeak

    <XmlAttribute> Public Property mz As Double
    <XmlAttribute> Public Property intensity As Double
    <XmlAttribute> Public Property comment As String

    Sub New(mz$, intensity$, Optional comment$ = Nothing)
        Me.mz = Val(mz)
        Me.intensity = Val(intensity)
        Me.comment = comment
    End Sub

    Sub New(mz#, intensity#)
        Me.mz = mz
        Me.intensity = intensity
    End Sub

    Public Overrides Function ToString() As String
        If comment.StringEmpty Then
            Return $"{mz} ({intensity})"
        Else
            Return $"{mz} ({intensity})  #{comment}"
        End If
    End Function
End Structure

