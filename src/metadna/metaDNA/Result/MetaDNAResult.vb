#Region "Microsoft.VisualBasic::09135ac7400efa7eb9a14be40f24f851, metadna\metaDNA\Result\MetaDNAResult.vb"

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

    '   Total Lines: 122
    '    Code Lines: 61 (50.00%)
    ' Comment Lines: 51 (41.80%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (8.20%)
    '     File Size: 3.98 KB


    ' Class MetaDNAResult
    ' 
    '     Properties: alignment, entropy, exactMass, fileName, formula
    '                 forward, inferLevel, inferSize, intensity, jaccard
    '                 KEGG_reaction, KEGGId, mirror, mz, mzCalc
    '                 name, parentTrace, partnerKEGGId, ppm, precursorType
    '                 pvalue, query_id, reaction, reverse, ROI_id
    '                 rt, rt_adjust, score1, score2, seed
    ' 
    '     Function: FilterInferenceHits, GetAlignment, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' table output of the metaDNA infer annotation result
''' </summary>
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
    ''' <summary>
    ''' precursor adducts type
    ''' </summary>
    ''' <returns></returns>
    Public Property precursorType As String
    ''' <summary>
    ''' Theoretical calculated m/z value based on <see cref="mz"/> And <see cref="precursorType"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property mzCalc As Double
    ''' <summary>
    ''' ppm error between the Theoretical <see cref="mzCalc"/> and <see cref="mz"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property ppm As Double
    ''' <summary>
    ''' the score match of ms1 <see cref="rt"/> and rt value of the KEGG compound reference
    ''' </summary>
    ''' <returns></returns>
    Public Property rt_adjust As Double
    ''' <summary>
    ''' MS1 or MS2
    ''' </summary>
    ''' <returns></returns>
    Public Property inferLevel As String
    ''' <summary>
    ''' forward cosine score
    ''' </summary>
    ''' <returns></returns>
    Public Property forward As Double
    ''' <summary>
    ''' reverse cosine score
    ''' </summary>
    ''' <returns></returns>
    Public Property reverse As Double
    ''' <summary>
    ''' ranked jaccard score
    ''' </summary>
    ''' <returns></returns>
    Public Property jaccard As Double
    Public Property mirror As Double
    ''' <summary>
    ''' entropy score
    ''' </summary>
    ''' <returns></returns>
    Public Property entropy As Double
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

    ''' <summary>
    ''' mz_into(query)_into(reference)[]
    ''' </summary>
    ''' <returns></returns>
    Public Property alignment As String

    Public Overrides Function ToString() As String
        Return $"{KEGGId}: {name}"
    End Function

    Public Shared Iterator Function GetAlignment(infer As InferLink) As IEnumerable(Of String)
        If infer Is Nothing Then
            Return
        ElseIf infer.level = MetaDNA.Infer.InferLevel.Ms1 Then
            Return
        End If

        For Each match As SSM2MatrixFragment In infer.alignments.SafeQuery
            If Not match Is Nothing Then
                Yield $"{match.mz.ToString("F4")}_{match.query}_{match.ref}"
            End If
        Next
    End Function

    Public Shared Iterator Function FilterInferenceHits(result As IEnumerable(Of MetaDNAResult), cutoff As Double) As IEnumerable(Of MetaDNAResult)
        For Each hit As MetaDNAResult In result.SafeQuery
            Dim score As Double = std.Min(hit.forward, hit.reverse)

            score = std.Max(score, hit.jaccard)
            score = std.Max(score, hit.entropy)

            If score > cutoff Then
                Yield hit
            End If
        Next
    End Function

End Class
