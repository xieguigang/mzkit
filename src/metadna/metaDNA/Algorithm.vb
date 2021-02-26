#Region "Microsoft.VisualBasic::4664998e1b599f8590c005186575182c, metaDNA\Algorithm.vb"

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

    ' Class Algorithm
    ' 
    '     Function: alignKeggCompound, GetBestQuery, RunInfer, RunIteration
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports stdnum = System.Math

''' <summary>
''' implements of the metadna algorithm in VisualBasic language
''' </summary>
Public Class Algorithm

    ReadOnly tolerance As Tolerance
    ReadOnly unknowns As UnknownSet
    ReadOnly kegg As KEGGHandler
    ReadOnly network As KEGGNetwork
    ReadOnly precursorTypes As MzCalculator()
    ReadOnly dotcutoff As Double
    ReadOnly MSalignment As AlignmentProvider

    ''' <summary>
    ''' Create infer network
    ''' </summary>
    ''' <param name="seeds"></param>
    ''' <returns></returns>
    Public Function RunIteration(seeds As IEnumerable(Of AnnotatedSeed)) As IEnumerable(Of InferLink)
        Return seeds _
            .AsParallel _
            .Select(AddressOf RunInfer) _
            .IteratesALL
    End Function

    Private Iterator Function RunInfer(seed As AnnotatedSeed) As IEnumerable(Of InferLink)
        Dim hits As KEGGQuery() = kegg.QueryByMz(seed.parent.mz).ToArray

        For Each query As KEGGQuery In hits
            Dim partners As String() = network.FindPartners(query.kegg_id).ToArray

            For Each kegg_id As String In partners
                Dim compound As Compound = kegg.GetCompound(kegg_id)

                If compound Is Nothing Then
                    Continue For
                End If

                For Each hit As InferLink In alignKeggCompound(seed, compound)
                    Yield hit
                Next
            Next
        Next
    End Function

    Private Iterator Function alignKeggCompound(seed As AnnotatedSeed, compound As Compound) As IEnumerable(Of InferLink)
        For Each type As MzCalculator In precursorTypes
            Dim mz As Double = type.CalcMZ(compound.exactMass)
            Dim candidates As PeakMs2() = unknowns.QueryByParentMz(mz)

            If candidates.IsNullOrEmpty Then
                Continue For
            End If

            For Each hit As PeakMs2 In candidates
                Dim alignment As InferLink = GetBestQuery(hit, seed)

                If stdnum.Min(alignment.forward, alignment.reverse) < dotcutoff Then
                    alignment.alignments = Nothing
                    alignment.level = InferLevel.Ms1
                    alignment.forward = 0
                    alignment.reverse = 0
                Else
                    alignment.level = InferLevel.Ms2
                End If

                Yield alignment
            Next
        Next
    End Function

    Private Function GetBestQuery(hit As PeakMs2, seed As AnnotatedSeed) As InferLink
        Dim max As InferLink = Nothing
        Dim align As AlignmentOutput
        Dim score As (forward#, reverse#)

        For Each ref In seed.products
            align = MSalignment.CreateAlignment(hit.mzInto, ref.Value.ms2)
            score = MSalignment.GetScore(align.alignments)

            If max Is Nothing OrElse stdnum.Min(score.forward, score.reverse) > stdnum.Min(max.forward, max.reverse) Then
                max = New InferLink With {
                    .reverse = score.reverse,
                    .forward = score.forward,
                    .alignments = align.alignments
                }
            End If
        Next

        Return max
    End Function

End Class
