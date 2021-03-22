#Region "Microsoft.VisualBasic::8867261c68d4e879706383d2e7339bb2, src\mzmath\ms2_math-core\Spectra\Models\Xml\AlignmentOutput.vb"

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

    '     Class AlignmentOutput
    ' 
    '         Properties: alignments, forward, jaccard, query, reference
    '                     reverse
    ' 
    '         Function: GetAlignmentMirror, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Spectra.Xml

    Public Class AlignmentOutput

        <XmlAttribute> Public Property forward As Double
        <XmlAttribute> Public Property reverse As Double
        <XmlAttribute> Public Property jaccard As Double

        Public Property query As Meta
        Public Property reference As Meta

        <XmlElement>
        Public Property alignments As SSM2MatrixFragment()

        Public Function GetAlignmentMirror() As (query As LibraryMatrix, ref As LibraryMatrix)
            With New Ms2AlignMatrix(alignments)
                Dim q = .GetQueryMatrix.With(Sub(a) a.name = query.id)
                Dim r = .GetReferenceMatrix.With(Sub(a) a.name = reference.id)

                Return (q, r)
            End With
        End Function

        Public Overrides Function ToString() As String
            Return $"{query} vs {reference}"
        End Function

    End Class
End Namespace
