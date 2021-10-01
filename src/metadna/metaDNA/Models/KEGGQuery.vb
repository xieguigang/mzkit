#Region "Microsoft.VisualBasic::d1eb34d8d84e907b08596691624e6380, src\metadna\metaDNA\Models\KEGGQuery.vb"

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

    ' Structure KEGGQuery
    ' 
    '     Properties: kegg_id, mz, ppm, precursorType, score
    ' 
    '     Function: Clone, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Public Structure KEGGQuery

    <XmlAttribute> Public Property mz As Double
    <XmlAttribute> Public Property ppm As Double
    <XmlAttribute> Public Property precursorType As String
    <XmlText> Public Property kegg_id As String

    ''' <summary>
    ''' used in <see cref="MSJointConnection"/>
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property score As Double

    Public Function Clone() As KEGGQuery
        Return New KEGGQuery With {
            .kegg_id = kegg_id,
            .mz = mz,
            .ppm = ppm,
            .precursorType = precursorType,
            .score = score
        }
    End Function

    Public Overrides Function ToString() As String
        Dim prefix As String = $"{kegg_id} {precursorType}, m/z {mz.ToString("F4")}"

        If score > 0 Then
            Return $"{prefix}; score={score}"
        Else
            Return prefix
        End If
    End Function

End Structure
