#Region "Microsoft.VisualBasic::537690ee5d6b812ae15882651358df53, G:/mzkit/Rscript/Library/mzkit_app/src/mzDIA//Oligonucleotide_MS.vb"

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

    '   Total Lines: 45
    '    Code Lines: 34
    ' Comment Lines: 5
    '   Blank Lines: 6
    '     File Size: 1.72 KB


    ' Module Oligonucleotide_MS
    ' 
    '     Function: exact_mass
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("Oligonucleotide_MS")>
Public Module Oligonucleotide_MS

    ''' <summary>
    ''' evaluate the exact mass for the gene sequence
    ''' </summary>
    ''' <param name="fa">should be a collection of the gene sequence.</param>
    ''' <returns></returns>
    <ExportAPI("exact_mass")>
    Public Function exact_mass(<RRawVectorArgument> fa As Object, Optional env As Environment = Nothing) As Object
        If fa Is Nothing Then
            Return Nothing
        End If

        If TypeOf fa Is vector Then
            fa = DirectCast(fa, vector).data
        End If

        If fa.GetType.IsArray Then
            Dim vec As Array = REnv.TryCastGenericArray(fa, env)

            If TypeOf vec Is String() Then
                Return DirectCast(vec, String()).Select(Function(si) New FastaSeq({}, si).CalcMW_Nucleotides).ToArray
            ElseIf TypeOf vec Is FastaSeq() Then
                Return DirectCast(vec, FastaSeq()).Select(Function(si) si.CalcMW_Nucleotides).ToArray
            Else
                Return Message.InCompatibleType(GetType(FastaSeq), fa.GetType, env)
            End If
        Else
            Return env.EvaluateFramework(Of FastaSeq, Double)(fa, Function(seq) seq.CalcMW_Nucleotides)
        End If
    End Function

End Module
