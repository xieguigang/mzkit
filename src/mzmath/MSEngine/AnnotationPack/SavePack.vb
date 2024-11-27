#Region "Microsoft.VisualBasic::234b50e099e5a5aaeb2c203edbe5de25, mzmath\MSEngine\AnnotationPack\SavePack.vb"

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
    '    Code Lines: 60 (84.51%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (15.49%)
    '     File Size: 2.19 KB


    ' Module SavePack
    ' 
    '     Function: (+2 Overloads) PackAlignment
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Linq

Public Module SavePack

    <Extension>
    Public Function PackAlignment(align As AlignmentHit) As MemoryStream
        Dim ms As New MemoryStream
        Dim bin As New BinaryWriter(ms)

        bin.Write(If(align.xcms_id, ""))
        bin.Write(If(align.libname, ""))
        bin.Write(align.mz)
        bin.Write(align.rt)
        bin.Write(align.RI)
        bin.Write(align.theoretical_mz)
        bin.Write(align.exact_mass)
        bin.Write(If(align.adducts, ""))
        bin.Write(align.ppm)
        bin.Write(align.occurrences)
        bin.Write(If(align.biodeep_id, ""))
        bin.Write(If(align.name, ""))
        bin.Write(If(align.formula, ""))
        bin.Write(align.npeaks)
        bin.Write(align.pvalue)
        bin.Write(align.samplefiles.TryCount)

        For Each sample In align.samplefiles.SafeQuery
            Using buf As MemoryStream = sample.Value.PackAlignment
                Call bin.Write(sample.Key)
                Call bin.Write(buf.Length)
                Call bin.Write(buf.ToArray)
            End Using
        Next

        Return ms
    End Function

    <Extension>
    Public Function PackAlignment(align As Ms2Score) As MemoryStream
        Dim ms As New MemoryStream
        Dim bin As New BinaryWriter(ms)

        bin.Write(align.precursor)
        bin.Write(align.rt)
        bin.Write(align.intensity)
        bin.Write(align.score)
        bin.Write(align.forward)
        bin.Write(align.reverse)
        bin.Write(align.jaccard)
        bin.Write(align.entropy)
        bin.Write(If(align.libname, ""))
        bin.Write(If(align.source, ""))
        bin.Write(align.ms2.TryCount)

        For Each peak As SSM2MatrixFragment In align.ms2.SafeQuery
            bin.Write(peak.mz)
            bin.Write(peak.query)
            bin.Write(peak.ref)
            bin.Write(If(peak.da, ""))
        Next

        bin.Flush()
        ms.Seek(Scan0, SeekOrigin.Begin)

        Return ms
    End Function

End Module
