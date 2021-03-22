#Region "Microsoft.VisualBasic::c36a1eb45dfa0ca64311b9f5d4e8c719, src\metadna\metaDNA\Result\MetaDNAResult.vb"

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

    ' Class MetaDNAResult
    ' 
    '     Properties: exactMass, fileName, formula, forward, inferLevel
    '                 inferSize, intensity, jaccard, KEGG_reaction, KEGGId
    '                 mz, mzCalc, name, parentTrace, partnerKEGGId
    '                 ppm, precursorType, pvalue, query_id, reaction
    '                 reverse, ROI_id, rt, rt_adjust, score1
    '                 score2, seed
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.MetaDNA.Infer

Public Class MetaDNAResult

    ''' <summary>
    ''' the unique id of ms1 parent ion
    ''' </summary>
    ''' <returns></returns>
    Public Property ROI_id As String
    ''' <summary>
    ''' the unique id of ms2 spectrum peaks
    ''' </summary>
    ''' <returns></returns>
    Public Property query_id As String
    Public Property mz As Double
    Public Property rt As Double
    Public Property intensity As Double
    Public Property KEGGId As String
    Public Property exactMass As Double
    Public Property formula As String
    Public Property name As String
    Public Property precursorType As String
    ''' <summary>
    ''' calculated m/z value based on <see cref="mz"/> and <see cref="precursorType"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property mzCalc As Double
    Public Property ppm As Double
    ''' <summary>
    ''' the score match of ms1 <see cref="rt"/> and rt value of the KEGG compound reference
    ''' </summary>
    ''' <returns></returns>
    Public Property rt_adjust As Double
    Public Property inferLevel As String
    Public Property forward As Double
    Public Property reverse As Double
    Public Property jaccard As Double
    Public Property parentTrace As Double
    Public Property inferSize As Integer
    Public Property score1 As Double
    Public Property score2 As Double
    Public Property pvalue As Double
    Public Property seed As String
    Public Property partnerKEGGId As String
    Public Property KEGG_reaction As String
    Public Property reaction As String
    Public Property fileName As String

End Class

