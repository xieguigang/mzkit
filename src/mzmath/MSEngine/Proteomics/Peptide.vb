Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports std = System.Math


Public Class Peptide

    Public Property DatabaseOrigin As String

    Public Property DatabaseOriginID As Integer

    Public ReadOnly Property Sequence As String

        Get
            If cacheSequence Is Nothing Then
                cacheSequence = If(SequenceObj.IsNullOrEmpty, String.Empty, String.Join("", SequenceObj.[Select](Function(n) n.OneLetter.ToString())))
            End If
            Return cacheSequence
        End Get
    End Property ' original amino acid sequence

    Private cacheSequence As String = Nothing


    Public ReadOnly Property ModifiedSequence As String
        Get
            Return If(SequenceObj.IsNullOrEmpty, String.Empty, String.Join("", SequenceObj.[Select](Function(n) n.Code())))
        End Get
    End Property

    Public Property Position As intRange

    Public Property ExactMass As Double

    Public ReadOnly Property Formula As Formula
        Get
            Return If(SequenceObj.IsNullOrEmpty, Nothing, PeptideCalc.CalculatePeptideFormula(SequenceObj))
        End Get
    End Property


    Public Property IsProteinNterminal As Boolean

    Public Property IsProteinCterminal As Boolean

    Public Property SequenceObj As List(Of AminoAcid) ' N -> C, including modified amino acid information


    Public Property IsDecoy As Boolean = False

    Public Property MissedCleavages As Integer = 0

    Public Property SamePeptideNumberInSearchedProteins As Integer = 0

    Public Property ResidueCodeIndexToModificationIndex As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()

    Public Function CountModifiedAminoAcids() As Integer
        If SequenceObj Is Nothing Then Return 0
        Return SequenceObj.Where(Function(n) n.IsModified()).Count
    End Function

    Public Sub GenerateSequenceObj(proteinSeq As String, start As Integer, [end] As Integer, ResidueCodeIndexToModificationIndex As Dictionary(Of Integer, Integer), ID2Code As Dictionary(Of Integer, String), Code2AminoAcidObj As Dictionary(Of String, AminoAcid))
        SequenceObj = GetSequenceObj(proteinSeq, start, [end], ResidueCodeIndexToModificationIndex, ID2Code, Code2AminoAcidObj)
    End Sub

    Private Function GetSequenceObj(proteinSeq As String, start As Integer, [end] As Integer, ResidueCodeIndexToModificationIndex As Dictionary(Of Integer, Integer), iD2Code As Dictionary(Of Integer, String), code2AminoAcidObj As Dictionary(Of String, AminoAcid)) As List(Of AminoAcid)
        Dim sequence = New List(Of AminoAcid)()
        If std.Max(start, [end]) > proteinSeq.Length - 1 Then Return Nothing
        For i = start To [end]
            Dim oneleter = proteinSeq(i)
            If ResidueCodeIndexToModificationIndex.ContainsKey(i) Then
                Dim residueID = ResidueCodeIndexToModificationIndex(i)
                Dim residueCode = iD2Code(residueID)
                Dim aa = code2AminoAcidObj(residueCode)
                sequence.Add(aa)
            Else
                Dim residueCode = oneleter
                Dim aa = code2AminoAcidObj(residueCode.ToString())
                sequence.Add(aa)
            End If
        Next
        Return sequence
    End Function
End Class

