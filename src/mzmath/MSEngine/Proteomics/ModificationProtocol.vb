Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Class ModificationProtocol

    Public Property OriginalAA As AminoAcid
    Public Function IsModified() As Boolean
        Return Not ModSequence.IsNullOrEmpty
    End Function

    Public Property ModifiedAACode As String 'Tp, K(Acethyl)

    Public Property ModifiedAA As AminoAcid

    Public Property ModifiedComposition As Formula

    Public Property ModSequence As List(Of Modification) = New List(Of Modification)() ' A -> K -> K(Acethyl)

    Public Sub UpdateProtocol(oneletter As Char, [mod] As Modification)
        Dim threeletter2onechar = AminoAcidObjUtility.ThreeLetter2OneChar
        If IsModified() Then
            If Equals(ModSequence(ModSequence.Count - 1).Type, "AaSubstitution") Then
                If IsAAEqual(oneletter, ModifiedAACode) Then
                    ModifiedAACode += "[" & [mod].Title.Split("("c)(0).Trim() & "]"
                    ModSequence.Add([mod])
                End If
            Else
                If IsAAEqual(oneletter, OriginalAA.OneLetter) Then
                    ModifiedAACode += "[" & [mod].Title.Split("("c)(0).Trim() & "]"
                    ModSequence.Add([mod])
                End If
            End If
        Else
            If IsAAEqual(OriginalAA.OneLetter, oneletter) Then
                If Equals([mod].Type, "AaSubstitution") Then
                    Dim convertedAA = [mod].Title.Replace("->", "_").Split("_"c)(1)
                    If Equals(convertedAA, "CamCys") Then
                        ModifiedAACode = "CamCys"
                    Else
                        If threeletter2onechar.ContainsKey(convertedAA) Then
                            ModifiedAACode = threeletter2onechar(convertedAA).ToString()
                        End If
                    End If
                Else
                    ModifiedAACode = oneletter.ToString() & "[" & [mod].Title.Split("("c)(0).Trim() & "]"
                End If
                ModSequence.Add([mod])
            End If
        End If
        If IsModified() Then
            UpdateObjects()
        End If
    End Sub

    Private Sub UpdateObjects()
        Dim dict = New Dictionary(Of String, Integer)()
        For Each [mod] In ModSequence
            Dim formula = [mod].Composition
            For Each pair In formula.CountsByElement
                If dict.ContainsKey(pair.Key) Then
                    dict(pair.Key) += pair.Value
                Else
                    dict(pair.Key) = pair.Value
                End If
            Next
        Next
        ModifiedComposition = New Formula(dict)

        For Each pair In OriginalAA.Formula.CountsByElement
            If dict.ContainsKey(pair.Key) Then
                dict(pair.Key) += pair.Value
            Else
                dict(pair.Key) = pair.Value
            End If
        Next
        ModifiedAA = New AminoAcid("0"c, ModifiedAACode, New Formula(dict))
    End Sub
End Class
