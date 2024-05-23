#Region "Microsoft.VisualBasic::a79db58a41dafad9580e8a2677da66d8, metadna\metaDNA\Extensions.vb"

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

    '   Total Lines: 66
    '    Code Lines: 55 (83.33%)
    ' Comment Lines: 2 (3.03%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (13.64%)
    '     File Size: 2.45 KB


    ' Module Extensions
    ' 
    '     Function: ExportInferRaw, GetRaw, MgfSeed, MgfSeeds
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.Linq

<HideModuleName> Public Module Extensions

    <Extension>
    Public Iterator Function MgfSeeds(mgf As IEnumerable(Of PeakMs2)) As IEnumerable(Of AnnotatedSeed)
        For Each ion As PeakMs2 In mgf
            Yield ion.MgfSeed
        Next
    End Function

    <Extension>
    Private Function MgfSeed(ion As PeakMs2) As AnnotatedSeed
        Dim ms1 As New ms1_scan With {
            .mz = ion.mz,
            .scan_time = ion.rt,
            .intensity = ion.intensity
        }
        Dim ms2 As New LibraryMatrix With {
            .name = ion.lib_guid,
            .ms2 = ion.mzInto
        }

        Return New AnnotatedSeed With {
            .inferSize = 1,
            .kegg_id = ion.meta!KEGG,
            .id = ion.lib_guid,
            .parent = ms1,
            .parentTrace = 100,
            .products = New Dictionary(Of String, LibraryMatrix) From {
                {ion.lib_guid, ms2}
            }
        }
    End Function

    <Extension>
    Public Function ExportInferRaw(raw As IEnumerable(Of CandidateInfer), result As IEnumerable(Of MetaDNAResult)) As MetaDNARawSet
        Return New MetaDNARawSet With {
            .Inference = raw.GetRaw(result).ToArray
        }
    End Function

    <Extension>
    Private Iterator Function GetRaw(raw As IEnumerable(Of CandidateInfer), result As IEnumerable(Of MetaDNAResult)) As IEnumerable(Of Candidate)
        Dim resultIndex As New Dictionary(Of String, Candidate)

        ' create unique index of the plot result raw data
        For Each infer As CandidateInfer In raw.SafeQuery
            For Each candidate As Candidate In infer.infers
                resultIndex($"{infer.kegg_id}|{candidate.precursorType}|{candidate.ROI}|{candidate.infer.rawFile}|{candidate.infer.reference.id}") = candidate
            Next
        Next

        ' populate raw in result table row orders
        For Each row As MetaDNAResult In result.SafeQuery
            Dim keyId As String = $"{row.KEGGId}|{row.precursorType}|{row.ROI_id}|{row.fileName}|{row.seed}"
            Dim align As Candidate = resultIndex(keyId)

            Yield align
        Next
    End Function
End Module
