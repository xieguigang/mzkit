#Region "Microsoft.VisualBasic::a540ca1e3fdbff298a12b553810a5627, mzmath\ms2_math-core\Spectra\Models\Xml\AlignmentOutput.vb"

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

    '   Total Lines: 232
    '    Code Lines: 132 (56.90%)
    ' Comment Lines: 73 (31.47%)
    '    - Xml Docs: 95.89%
    ' 
    '   Blank Lines: 27 (11.64%)
    '     File Size: 8.93 KB


    '     Class AlignmentOutput
    ' 
    '         Properties: alignment_str, alignments, cosine, entropy, forward
    '                     jaccard, mean, mirror, nhits, query
    '                     reference, reverse
    ' 
    '         Function: (+2 Overloads) CreateLinearMatrix, GetAlignmentMirror, GetHitsMzPeaks, ParseAlignment, ParseAlignmentLinearMatrix
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace Spectra.Xml

    ''' <summary>
    ''' the spectrum similarity comparision result object
    ''' </summary>
    ''' <remarks>
    ''' contains the score and spectrum fragment alignment details result data.
    ''' </remarks>
    Public Class AlignmentOutput

        ''' <summary>
        ''' the forward cosine score
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property forward As Double
        ''' <summary>
        ''' the reverse cosine score
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property reverse As Double
        ''' <summary>
        ''' the jaccard index between the fragment set of query vs subject
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property jaccard As Double
        ''' <summary>
        ''' the spectrum entropy similarity
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property entropy As Double

        Public ReadOnly Property mirror As Double
            Get
                If _alignments.IsNullOrEmpty Then
                    Return 0.0
                Else
                    Dim nq As Integer = alignments.Where(Function(x) x.query > 0).Count
                    Dim nr As Integer = alignments.Where(Function(x) x.ref > 0).Count

                    Return jaccard * (std.Min(nq, nr) / std.Max(nq, nr))
                End If
            End Get
        End Property

        ''' <summary>
        ''' the min value of <see cref="forward"/> and <see cref="reverse"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property cosine As Double
            Get
                Return std.Min(forward, reverse)
            End Get
        End Property

        ''' <summary>
        ''' get score mean result
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property mean As Double
            Get
                Return {forward, reverse, jaccard, entropy}.Average
            End Get
        End Property

        ''' <summary>
        ''' the reference to the sample query
        ''' </summary>
        ''' <returns></returns>
        Public Property query As Meta

        ''' <summary>
        ''' the reference to the reference library
        ''' </summary>
        ''' <returns></returns>
        Public Property reference As Meta

        ''' <summary>
        ''' the spectrum alignment fragments details
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("alignments")>
        Public Property alignments As SSM2MatrixFragment()

        ''' <summary>
        ''' get basepeak of the reference ion as Q3 product ion
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Q3 As Double
            Get
                If alignments.IsNullOrEmpty Then
                    Return 0
                Else
                    Return alignments.OrderByDescending(Function(i) i.ref).First.mz
                End If
            End Get
        End Property

        Public ReadOnly Property Q3_ratio As Double
            Get
                If alignments.IsNullOrEmpty Then
                    Return 0
                Else
                    Dim basepeak = alignments.OrderByDescending(Function(i) i.ref).First
                    Dim min = std.Min(basepeak.query, basepeak.ref)
                    Dim max = std.Max(basepeak.query, basepeak.ref)
                    Return min / max
                End If
            End Get
        End Property

        Public ReadOnly Property nhits As Integer
            Get
                If alignments Is Nothing OrElse alignments.Length = 0 Then
                    Return 0
                End If

                Return alignments _
                    .Where(Function(a)
                               If a.da = "NA" Then
                                   Return False
                               Else
                                   Return Not Double.Parse(a.da).IsNaNImaginary
                               End If
                           End Function) _
                    .Count
            End Get
        End Property

        Public ReadOnly Property alignment_str As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CreateLinearMatrix(alignments).JoinBy(" ")
            End Get
        End Property

        Public Iterator Function GetHitsMzPeaks() As IEnumerable(Of NamedValue(Of Double))
            For Each hit As SSM2MatrixFragment In alignments
                If hit.query > 0 AndAlso hit.ref > 0 Then
                    Yield New NamedValue(Of Double)(hit.annotation, hit.mz)
                End If
            Next
        End Function

        Public Function GetNormalized() As AlignmentOutput
            Dim qmax As Double = Aggregate i As SSM2MatrixFragment In alignments Into Max(i.query)
            Dim rmax As Double = Aggregate i As SSM2MatrixFragment In alignments Into Max(i.ref)

            Return New AlignmentOutput With {
                .entropy = entropy,
                .forward = forward,
                .jaccard = jaccard,
                .query = query,
                .reference = reference,
                .reverse = reverse,
                .alignments = alignments _
                    .SafeQuery _
                    .Select(Function(i)
                                Return New SSM2MatrixFragment With {
                                    .ref = i.ref / rmax,
                                    .query = i.query / qmax,
                                    .da = i.da,
                                    .annotation = i.annotation,
                                    .IsNeutralLossMatched = i.IsNeutralLossMatched,
                                    .IsProductIonMatched = i.IsProductIonMatched,
                                    .mz = i.mz
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' construct a tuple of the spectrum data as the mirror alignment
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' the name of the generated <see cref="LibraryMatrix"/> object is 
        ''' generates from the metadata of query and reference.
        ''' </remarks>
        Public Function GetAlignmentMirror() As (query As LibraryMatrix, ref As LibraryMatrix)
            With New Ms2AlignMatrix(alignments)
                Dim q = .GetQueryMatrix.With(Sub(a) a.name = query?.id)
                Dim r = .GetReferenceMatrix.With(Sub(a) a.name = reference?.id)

                If q.name.StringEmpty(, True) Then
                    q.name = "Query"
                End If
                If r.name.StringEmpty(, True) Then
                    r.name = "Reference"
                End If

                Return (q, r)
            End With
        End Function

        Public Overrides Function ToString() As String
            Return $"{query} vs {reference}"
        End Function

        Public Shared Iterator Function CreateLinearMatrix(matrix As IEnumerable(Of SSM2MatrixFragment)) As IEnumerable(Of String)
            For Each line As SSM2MatrixFragment In matrix.SafeQuery
                If Not line Is Nothing Then
                    Dim mz = line.mz.ToString("F4")
                    Dim q = line.query.ToString("G4")
                    Dim r = line.ref.ToString("G4")
                    Dim str = Strings.Trim(line.annotation).Replace(" "c, "-"c).Replace("_"c, "-"c).Replace("'"c, "-"c).Replace(""""c, "-"c)

                    If str = "" Then
                        Yield New String() {mz, q, r}.JoinBy("_")
                    Else
                        Yield New String() {mz, q, r, str}.JoinBy("_")
                    End If
                End If
            Next
        End Function

        ''' <summary>
        ''' Export function for the ``R#`` language
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <param name="query"></param>
        ''' <param name="ref"></param>
        ''' <param name="annotation_str"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateLinearMatrix(mz As Double(), query As Double(), ref As Double(), annotation_str As String()) As IEnumerable(Of String)
            Return CreateLinearMatrix(mz.Select(Function(mzi, i) New SSM2MatrixFragment(mzi, query(i), ref(i), annotation_str(i))))
        End Function

        ''' <summary>
        ''' parse the string data as the peak fragment alignment details matrix.
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns>
        ''' an alingment details matrix between the sample query and reference spectrum.
        ''' </returns>
        Public Shared Iterator Function ParseAlignmentLinearMatrix(str As String) As IEnumerable(Of SSM2MatrixFragment)
            If Not str Is Nothing Then
                Dim peaks As String() = str.Split

                For Each ti As String In peaks
                    Dim tokens = ti.Split("_"c)
                    Dim mz = Val(tokens(0))
                    Dim query = Val(tokens(1))
                    Dim refer = Val(tokens(2))

                    ' optional annotation string of current
                    ' spectrum peak fragment alingment.
                    str = tokens.ElementAtOrNull(3)

                    Yield New SSM2MatrixFragment With {
                        .mz = mz,
                        .query = query,
                        .ref = refer,
                        .annotation = str
                    }
                Next
            End If
        End Function

        ''' <summary>
        ''' Parse the alignment string and then returns the alignment details and the score values
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseAlignment(str As String) As AlignmentOutput
            Dim result As New AlignmentOutput With {
                .alignments = ParseAlignmentLinearMatrix(str).ToArray,
                .entropy = AlignmentProvider.GetEntropyScore(.alignments),
                .jaccard = AlignmentProvider.GetJaccardIndex(.alignments)
            }
            Dim cos = AlignmentProvider.GetCosineScore(result.alignments)
            result.forward = cos.forward
            result.reverse = cos.reverse
            Return result
        End Function

    End Class
End Namespace
