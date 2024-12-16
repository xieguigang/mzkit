
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq

Public NotInheritable Class ModificationUtility
    Private Sub New()
    End Sub

    Public Shared Function GetModifiedComposition(modseqence As List(Of Modification)) As Formula
        If modseqence.IsNullOrEmpty Then Return Nothing
        Dim dict = New Dictionary(Of String, Integer)()
        For Each [mod] In modseqence
            Dim formula = [mod].Composition
            For Each pair In formula.CountsByElement
                If dict.ContainsKey(pair.Key) Then
                    dict(pair.Key) += pair.Value
                Else
                    dict(pair.Key) = pair.Value
                End If
            Next
        Next
        Return New Formula(dict)
    End Function

    Public Shared Function GetModifiedAminoacidCode(originalcode As String, modseqence As List(Of Modification)) As String
        If modseqence.IsNullOrEmpty Then Return String.Empty
        Dim code = originalcode
        For Each [mod] In modseqence
            code += "[" & [mod].Title.Split("("c)(0).Trim() & "]"
        Next
        Return code
    End Function

    Public Shared Function GetModifiedCompositions(originalcode As String, modseqence As List(Of Modification)) As (String, Formula)
        If modseqence.IsNullOrEmpty Then Return (String.Empty, Nothing)
        Dim dict = New Dictionary(Of String, Integer)()
        Dim code = originalcode
        Dim modCodes = New List(Of String)()
        For Each [mod] In modseqence
            modCodes.Add([mod].Title.Split("("c)(0).Trim())

            Dim formula = [mod].Composition
            For Each pair In formula.CountsByElement
                If dict.ContainsKey(pair.Key) Then
                    dict(pair.Key) += pair.Value
                Else
                    dict(pair.Key) = pair.Value
                End If
            Next
        Next

        If Not modCodes.IsNullOrEmpty Then
            If modCodes.Count = 1 Then
                code = code & "[" & modCodes(0) & "]"
            Else
                code = code & "[" & String.Join("][", modCodes) & "]"
            End If
        End If

        Return (code, New Formula(dict))
    End Function

    Public Shared Function GetModificationContainer(fixedModifications As List(Of Modification),
                                                    variableModifications As List(Of Modification),
                                                    selectedFixedModifications As List(Of String),
                                                    selectedVariableModifications As List(Of String)) As ModificationContainer

        Dim sModifications = New List(Of Modification)()
        For Each modString In selectedFixedModifications
            For Each modObj In fixedModifications
                If Equals(modString, modObj.Title) Then
                    modObj.IsSelected = True
                    modObj.IsVariable = False
                    sModifications.Add(modObj)
                    Exit For
                End If
            Next
        Next

        For Each modString In selectedVariableModifications
            For Each modObj In variableModifications
                If Equals(modString, modObj.Title) Then
                    modObj.IsSelected = True
                    modObj.IsVariable = True
                    sModifications.Add(modObj)
                    Exit For
                End If
            Next
        Next

        Return New ModificationContainer(sModifications)
    End Function

    Public Shared Function GetModificationContainer(fixedModifications As List(Of Modification), variableModifications As List(Of Modification)) As ModificationContainer
        Dim modifications = New List(Of Modification)()
        Dim fixedModStrings = New List(Of String)()
        For Each [mod] In fixedModifications.Where(Function(n) n.IsSelected)
            [mod].IsVariable = False
            modifications.Add([mod])
        Next

        fixedModStrings = modifications.[Select](Function(n) n.Title).ToList()
        For Each [mod] In variableModifications.Where(Function(n) n.IsSelected)
            If Not fixedModStrings.Contains([mod].Title) Then
                [mod].IsVariable = True
                modifications.Add([mod])
            End If
        Next

        Return New ModificationContainer(modifications)
    End Function


    Public Shared Function GetModifiedPeptides(peptides As List(Of Peptide), modContainer As ModificationContainer,
                                               Optional maxNumberOfModificationsPerPeptide As Integer = 2,
                                               Optional minPeptideMass As Double = 300,
                                               Optional maxPeptideMass As Double = 4600) As List(Of Peptide)

        Dim mPeptides = New List(Of Peptide)()
        For Each peptide In peptides
            Dim results = PeptideCalc.Sequence2Peptides(peptide, modContainer, maxNumberOfModificationsPerPeptide, minPeptideMass, maxPeptideMass)
            For Each result In results.SafeQuery
                mPeptides.Add(result)
            Next
        Next
        Return mPeptides
    End Function

    Public Shared Function GetFastModifiedPeptides(peptides As List(Of Peptide), modContainer As ModificationContainer,
                                                   Optional maxNumberOfModificationsPerPeptide As Integer = 2,
                                                   Optional minPeptideMass As Double = 300,
                                                   Optional maxPeptideMass As Double = 4600) As List(Of Peptide)
        Dim mPeptides = New List(Of Peptide)()
        For Each peptide In peptides
            If peptide.ExactMass > maxPeptideMass Then Continue For
            Dim results = PeptideCalc.Sequence2FastPeptides(peptide, modContainer, maxNumberOfModificationsPerPeptide, minPeptideMass, maxPeptideMass)
            For Each result In results.SafeQuery
                mPeptides.Add(result)
            Next
        Next
        Return mPeptides
    End Function
End Class
