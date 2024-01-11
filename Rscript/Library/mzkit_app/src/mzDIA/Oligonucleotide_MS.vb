
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
