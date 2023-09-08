Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public NotInheritable Class PeptideCalc
    Private Sub New()
    End Sub

    Private Shared OH As Double = 17.002739652
    Private Shared H As Double = 1.00782503207
    Private Shared H2O As Double = 18.010564684

    ' N -> C, just return exactmass using default setting
    Public Shared Function Sequence2Mass(sequence As String) As Double
        Dim mass = 0.0
        Dim char2mass = OneChar2Mass
        Dim offsetMass = OH + H2O * (sequence.Length - 2) + H ' N-terminal, internal amino acids, C-terminal
        For i = 0 To sequence.Length - 1
            Dim aaChar = sequence(i)
            If char2mass.ContainsKey(aaChar) Then
                mass += char2mass(aaChar)
            End If
        Next
        Return mass - offsetMass
    End Function

    Public Shared Function Sequence2Mass(sequence As List(Of AminoAcid)) As Double
        Dim mass = 0.0
        Dim offsetMass = OH + H2O * (sequence.Count - 2) + H ' N-terminal, internal amino acids, C-terminal
        For i = 0 To sequence.Count - 1
            Dim aaChar = sequence(i)
            mass += aaChar.ExactMass()
        Next
        Return mass - offsetMass
    End Function

    Public Shared Function Sequence2AminoAcids(sequence As String, char2AA As Dictionary(Of Char, AminoAcid)) As List(Of AminoAcid)
        Dim aalist = New List(Of AminoAcid)()
        For Each oneletter In sequence
            If char2AA.ContainsKey(oneletter) Then aalist.Add(char2AA(oneletter))
        Next
        Return aalist
    End Function

    Public Shared Function Sequence2Peptide(peptide As Peptide) As Peptide
        Dim sequence = peptide.SequenceObj
        peptide.ExactMass = Sequence2Mass(sequence)
        Return peptide
    End Function

    Public Shared Function Sequence2Peptides(peptide As Peptide, container As ModificationContainer, Optional maxNumberOfModificationsPerPeptide As Integer = 2, Optional minPeptideMass As Double = 300, Optional maxPeptideMass As Double = 4600) As List(Of Peptide)

        Dim fmPeptide = Sequence2PeptideByFixedModifications(peptide, container, maxPeptideMass)
        If fmPeptide Is Nothing Then Return Nothing
        If fmPeptide.CountModifiedAminoAcids() > maxNumberOfModificationsPerPeptide Then Return Nothing
        Return Sequence2PeptidesByVariableModifications(peptide, container, maxNumberOfModificationsPerPeptide, minPeptideMass, maxPeptideMass)
    End Function

    Public Shared Function Sequence2FastPeptides(peptide As Peptide, container As ModificationContainer, Optional maxNumberOfModificationsPerPeptide As Integer = 2, Optional minPeptideMass As Double = 300, Optional maxPeptideMass As Double = 4600) As List(Of Peptide)
        Dim fmPeptide = Sequence2PeptideByFixedModifications(peptide, container, maxPeptideMass)
        If fmPeptide Is Nothing Then Return Nothing
        If fmPeptide.CountModifiedAminoAcids() > maxNumberOfModificationsPerPeptide Then Return Nothing
        Return Sequence2FastPeptidesByVariableModifications(peptide, container, fmPeptide.CountModifiedAminoAcids(), maxNumberOfModificationsPerPeptide, minPeptideMass, maxPeptideMass)
    End Function

    Public Shared Function Sequence2PeptideByFixedModifications(peptide As Peptide, container As ModificationContainer, Optional maxPeptideMass As Double = 4600) As Peptide
        Dim sequence = peptide.SequenceObj
        If container.IsEmptyOrNull Then Return Sequence2Peptide(peptide)

        Dim isProteinNTerminal = peptide.IsProteinNterminal
        Dim isProteinCTerminal = peptide.IsProteinCterminal
        Dim aaSequence = New List(Of AminoAcid)()
        For i = 0 To sequence.Count - 1
            Dim modseq = New List(Of Modification)()
            Dim aa = GetAminoAcidByFixedModifications(peptide, modseq, container, i)
            aaSequence.Add(aa)
        Next

        peptide.SequenceObj = aaSequence
        Dim formula = CalculatePeptideFormula(aaSequence)
        If formula.ExactMass > maxPeptideMass Then Return Nothing
        peptide.ExactMass = formula.ExactMass

        Return peptide
    End Function

    Public Shared Function GetAminoAcidByFixedModifications(peptide As Peptide, modseq As List(Of Modification), container As ModificationContainer, index As Integer) As AminoAcid

        Dim isProteinNTerminal = peptide.IsProteinNterminal
        Dim isProteinCTerminal = peptide.IsProteinCterminal

        Dim sequence = peptide.SequenceObj
        Dim aaChar = sequence(index).OneLetter

        Dim isPeptideNTerminal = If(index = 0, True, False)
        Dim isPeptideCTerminal = If(index = sequence.Count - 1, True, False)

        Return GetAminoAcidByFixedModifications(modseq, container, aaChar, isPeptideNTerminal, isPeptideCTerminal, isProteinNTerminal, isProteinCTerminal)
    End Function

    Public Shared Function GetAminoAcidByFixedModifications(modseq As List(Of Modification), container As ModificationContainer, aaChar As Char, isPeptideNTerminal As Boolean, isPeptideCTerminal As Boolean, isProteinNTerminal As Boolean, isProteinCTerminal As Boolean) As AminoAcid
        If isPeptideNTerminal AndAlso isProteinNTerminal AndAlso container.ProteinNterm2FixedMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.ProteinNterm2FixedMod(aaChar))
        ElseIf isPeptideNTerminal AndAlso container.AnyNtermSite2FixedMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.AnyNtermSite2FixedMod(aaChar))
        End If

        If Not isPeptideNTerminal AndAlso Not isPeptideCTerminal AndAlso container.NotCtermSite2FixedMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.NotCtermSite2FixedMod(aaChar))
        End If

        If isPeptideCTerminal AndAlso isProteinCTerminal AndAlso container.ProteinCtermSite2FixedMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.ProteinCtermSite2FixedMod(aaChar))
        ElseIf isPeptideCTerminal AndAlso container.AnyCtermSite2FixedMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.AnyCtermSite2FixedMod(aaChar))
        End If

        If container.AnywehereSite2FixedMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.AnywehereSite2FixedMod(aaChar))
        End If

        Dim compositions = ModificationUtility.GetModifiedCompositions(aaChar.ToString(), modseq)
        Dim code = compositions.Item1
        If Not container.Code2AminoAcidObj.IsNullOrEmpty AndAlso container.Code2AminoAcidObj.ContainsKey(code) Then
            Return container.Code2AminoAcidObj(code)
        ElseIf Not container.Code2AminoAcidObj.IsNullOrEmpty AndAlso Equals(code, String.Empty) Then
            Return container.Code2AminoAcidObj(aaChar.ToString())
        Else
            Dim aa = New AminoAcid(container.AnywehereSite2FixedMod(aaChar).OriginalAA, compositions.Item1, compositions.Item2)
            Return aa
        End If
        'var aa = new AminoAcid(container.AnywehereSite2FixedMod[aaChar].OriginalAA, compositions.Item1, compositions.Item2);
        'return aa;
    End Function

    Public Shared Function GetSimpleChar2AminoAcidDictionary() As Dictionary(Of Char, AminoAcid)
        Dim aaletters = AminoAcidLetters
        Dim dict = New Dictionary(Of Char, AminoAcid)()
        For Each oneletter In aaletters ' initialize normal amino acids
            Dim aa = New AminoAcid(oneletter)
            dict(oneletter) = aa
        Next

        Return dict
    End Function


    ''' <summary>
    ''' peptide should be processed by Sequence2PeptideByFixedModifications before using this method
    ''' </summary>
    ''' <param name="peptide"></param>
    ''' <param name="container"></param>
    ''' <param name="maxNumberOfModificationsPerPeptide"></param>
    ''' <returns></returns>
    Public Shared Function Sequence2PeptidesByVariableModifications(peptide As Peptide, container As ModificationContainer, Optional maxNumberOfModificationsPerPeptide As Integer = 2, Optional minPeptideMass As Double = 300, Optional maxPeptideMass As Double = 4600) As List(Of Peptide)
        'var sequence = peptide.Sequence;
        If container.IsEmptyOrNull Then Return New List(Of Peptide)() From {
            Sequence2Peptide(peptide)
        }

        Dim currentModCount = peptide.CountModifiedAminoAcids()
        Dim results = New List(Of List(Of AminoAcid))()
        Call EnumerateModifications(peptide, container, 0, currentModCount, maxNumberOfModificationsPerPeptide, New List(Of AminoAcid)(), results)

        Dim peptides = New List(Of Peptide)()
        For Each result In results
            Dim nPep = New Peptide() With {
                .DatabaseOrigin = peptide.DatabaseOrigin,
                .DatabaseOriginID = peptide.DatabaseOriginID,
                .SequenceObj = result,
                .Position = New IntRange(peptide.Position.Min, peptide.Position.Max),
                .IsProteinCterminal = peptide.IsProteinCterminal,
                .IsProteinNterminal = peptide.IsProteinNterminal
            }
            'nPep.SequenceObj = result;
            Dim formula = PeptideCalc.CalculatePeptideFormula(result)
            If formula.ExactMass > maxPeptideMass Then Continue For
            If formula.ExactMass < minPeptideMass Then Continue For
            nPep.ExactMass = formula.ExactMass
            nPep.ResidueCodeIndexToModificationIndex = GetResidueCodeIndexToModificationIndexDictionary(nPep, container)
            peptides.Add(nPep)
        Next

        Return peptides
    End Function

    Public Shared Function Sequence2FastPeptidesByVariableModifications(peptide As Peptide, container As ModificationContainer, fixedModCount As Integer, Optional maxNumberOfModificationsPerPeptide As Integer = 2, Optional minPeptideMass As Double = 300, Optional maxPeptideMass As Double = 4600) As List(Of Peptide)

        If container.IsEmptyOrNull Then Return New List(Of Peptide)() From {
            Sequence2Peptide(peptide)
        }
        Dim seq = peptide.SequenceObj
        Dim dict = New Dictionary(Of Integer, AminoAcid)()
        Dim diff2positions = New Dictionary(Of Integer, List(Of Integer))()
        For i = 0 To seq.Count - 1
            If seq(i).IsModified() Then
                dict(i) = seq(i)
            Else
                Dim originAA = peptide.SequenceObj(i)
                Dim [mod] = If(originAA.Modifications.IsNullOrEmpty, New List(Of Modification)(), originAA.Modifications.ToList())
                Dim modifiedAA = GetAminoAcidByVariableModifications(peptide, [mod], container, i)

                Dim diff = modifiedAA.ExactMass() - originAA.ExactMass()
                If diff > 0 Then
                    Dim key = CInt(diff) * 1000000
                    If diff2positions.ContainsKey(key) Then
                        diff2positions(key).Add(i)
                    Else
                        diff2positions(key) = New List(Of Integer)() From {
                            i
                        }

                    End If
                End If
                dict(i) = modifiedAA
            End If
        Next

        Dim combinations = New List(Of List(Of List(Of Integer)))()
        For i = 0 To maxNumberOfModificationsPerPeptide - fixedModCount
            combinations.Add(New List(Of List(Of Integer))())
        Next

        combinations(0).Add(New List(Of Integer)())

        For Each item In diff2positions
            Dim key = item.Key
            Dim values = item.Value
            For i = combinations.Count - 1 To 0 Step -1
                Dim combination = combinations(i)
                For j = 0 To combination.Count - 1
                    For k = 0 To values.Count - 1
                        If i + k + 1 > combinations.Count - 1 Then Continue For
                        combinations(i + k + 1).Add(combination(j).Concat(values.Take(k + 1)).ToList())
                    Next
                Next
            Next
        Next
        Dim peptides = New List(Of Peptide)()
        If peptide.ExactMass >= minPeptideMass AndAlso peptide.ExactMass <= maxPeptideMass Then peptides.Add(peptide)

        For Each item In combinations
            For Each pair In item
                Dim result = New List(Of AminoAcid)(seq.Count)
                For Each aa In seq
                    result.Add(aa)
                Next
                For i = 0 To pair.Count - 1
                    result(pair(i)) = dict(pair(i))
                Next

                Dim nPep = New Peptide() With {
                    .DatabaseOrigin = peptide.DatabaseOrigin,
                    .DatabaseOriginID = peptide.DatabaseOriginID,
                    .SequenceObj = result,
                    .Position = New IntRange(peptide.Position.Min, peptide.Position.Max),
                    .IsProteinCterminal = peptide.IsProteinCterminal,
                    .IsProteinNterminal = peptide.IsProteinNterminal
                }

                Dim formula = CalculatePeptideFormula(result)
                If formula.ExactMass > maxPeptideMass OrElse formula.ExactMass < minPeptideMass Then Continue For
                nPep.ExactMass = formula.ExactMass
                nPep.ResidueCodeIndexToModificationIndex = GetResidueCodeIndexToModificationIndexDictionary(nPep, container)
                peptides.Add(nPep)
            Next
        Next
        Return peptides
    End Function

    Private Shared Function GetResidueCodeIndexToModificationIndexDictionary(peptide As Peptide, container As ModificationContainer) As Dictionary(Of Integer, Integer)
        Dim dict = New Dictionary(Of Integer, Integer)()
        For i = 0 To peptide.SequenceObj.Count() - 1
            Dim aa = peptide.SequenceObj(i)
            If aa.IsModified() Then
                dict(i) = container.Code2ID(aa.Code())
            End If
        Next
        Return dict
    End Function



    Private Shared Sub EnumerateModifications(pep As Peptide, container As ModificationContainer, index As Integer, numModifications As Integer, maxModifications As Integer, aminoacids As List(Of AminoAcid), result As List(Of List(Of AminoAcid)))

        'Console.WriteLine(index);
        If index >= pep.SequenceObj.Count Then
            result.Add(aminoacids.ToList())
            Return
        End If
        Dim originAA = pep.SequenceObj(index)
        aminoacids.Add(originAA)
        EnumerateModifications(pep, container, index + 1, numModifications, maxModifications, aminoacids, result)
        aminoacids.RemoveAt(index)

        If maxModifications > numModifications Then
            Dim [mod] = If(originAA.Modifications.IsNullOrEmpty, New List(Of Modification)(), originAA.Modifications.ToList())
            Dim modifiedAA = GetAminoAcidByVariableModifications(pep, [mod], container, index)

            If modifiedAA.IsModified() AndAlso Not Equals(originAA.Code(), modifiedAA.Code()) Then
                aminoacids.Add(modifiedAA)
                EnumerateModifications(pep, container, index + 1, numModifications + 1, maxModifications, aminoacids, result)
                aminoacids.RemoveAt(index)
            End If
        End If
    End Sub

    Public Shared Function GetAminoAcidByVariableModifications(peptide As Peptide, modseq As List(Of Modification), container As ModificationContainer, index As Integer) As AminoAcid

        Dim isProteinNTerminal = peptide.IsProteinNterminal
        Dim isProteinCTerminal = peptide.IsProteinCterminal

        Dim sequence = peptide.SequenceObj
        Dim aaChar = sequence(index).OneLetter

        Dim isPeptideNTerminal = If(index = 0, True, False)
        Dim isPeptideCTerminal = If(index = sequence.Count - 1, True, False)

        Return GetAminoAcidByVariableModifications(modseq, container, aaChar, isPeptideNTerminal, isPeptideCTerminal, isProteinNTerminal, isProteinCTerminal)
    End Function

    Public Shared Function GetAminoAcidByVariableModifications(modseq As List(Of Modification), container As ModificationContainer, aaChar As Char, isPeptideNTerminal As Boolean, isPeptideCTerminal As Boolean, isProteinNTerminal As Boolean, isProteinCTerminal As Boolean) As AminoAcid
        If isPeptideNTerminal AndAlso isProteinNTerminal AndAlso container.ProteinNterm2VariableMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.ProteinNterm2VariableMod(aaChar))
        ElseIf isPeptideNTerminal AndAlso container.AnyNtermSite2VariableMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.AnyNtermSite2VariableMod(aaChar))
        End If

        If Not isPeptideNTerminal AndAlso Not isPeptideCTerminal AndAlso container.NotCtermSite2VariableMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.NotCtermSite2VariableMod(aaChar))
        End If

        If isPeptideCTerminal AndAlso isProteinCTerminal AndAlso container.ProteinCtermSite2VariableMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.ProteinCtermSite2VariableMod(aaChar))
        ElseIf isPeptideCTerminal AndAlso container.AnyCtermSite2VariableMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.AnyCtermSite2VariableMod(aaChar))
        End If

        If container.AnywehereSite2VariableMod(aaChar).IsModified() Then
            SetModificationSequence(modseq, container.AnywehereSite2VariableMod(aaChar))
        End If

        Dim compositions = ModificationUtility.GetModifiedCompositions(aaChar.ToString(), modseq)
        Dim code = compositions.Item1
        If Not container.Code2AminoAcidObj.IsNullOrEmpty AndAlso container.Code2AminoAcidObj.ContainsKey(code) Then
            Return container.Code2AminoAcidObj(code)
        ElseIf Not container.Code2AminoAcidObj.IsNullOrEmpty AndAlso Equals(code, String.Empty) Then
            Return container.Code2AminoAcidObj(aaChar.ToString())
        Else
            Dim aa = New AminoAcid(container.AnywehereSite2VariableMod(aaChar).OriginalAA, compositions.Item1, compositions.Item2)
            Return aa
        End If

    End Function

    Public Shared Function CalculatePeptideFormula(aaSequence As List(Of AminoAcid)) As Formula
        Dim dict = New Dictionary(Of String, Integer)()
        For Each aa In aaSequence
            Dim formula = aa.GetFormula()
            For Each pair In formula.CountsByElement
                If dict.ContainsKey(pair.Key) Then
                    dict(pair.Key) += pair.Value
                Else
                    dict(pair.Key) = pair.Value
                End If
            Next
        Next

        Dim offsetHydrogen = (aaSequence.Count - 1) * 2
        Dim offsetOxygen = aaSequence.Count - 1

        dict("H") -= offsetHydrogen
        dict("O") -= offsetOxygen

        Return New Formula(dict)
    End Function

    Public Shared Sub SetModificationSequence(modseq As List(Of Modification), protocol As ModificationProtocol)
        Dim mods = protocol.ModSequence
        For Each [mod] In mods
            modseq.Add([mod])
        Next
    End Sub

    Public Shared Function Sequence2Formula(sequence As String) As Formula
        Dim carbon = 0
        Dim hydrogen = 0
        Dim nitrogen = 0
        Dim oxygen = 0
        Dim sulfur = 0

        Dim char2formula = AminoAcid.OneChar2Formula
        Dim offsetHydrogen = (sequence.Length - 1) * 2
        Dim offsetOxygen = sequence.Length - 1

        For i = 0 To sequence.Length - 1
            Dim aaChar = sequence(i)
            If char2formula.ContainsKey(aaChar) Then
                carbon += char2formula(aaChar)!C
                hydrogen += char2formula(aaChar)!H
                nitrogen += char2formula(aaChar)!N
                oxygen += char2formula(aaChar)!O
                sulfur += char2formula(aaChar)!S
            End If
        Next

        Return New DerivatizationFormula(carbon, hydrogen - offsetHydrogen, nitrogen, oxygen - offsetOxygen, 0, sulfur, 0, 0, 0, 0, 0)
    End Function
End Class
