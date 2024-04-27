#Region "Microsoft.VisualBasic::cadc4166d8edc8942f1feabaa3910233, G:/mzkit/src/mzmath/MSEngine//ModificationContainer.vb"

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

    '   Total Lines: 491
    '    Code Lines: 370
    ' Comment Lines: 11
    '   Blank Lines: 110
    '     File Size: 22.48 KB


    ' Class ModificationContainer
    ' 
    '     Properties: AnyCtermFixedMods, AnyCtermSite2FixedMod, AnyCtermSite2VariableMod, AnyCtermVariableMods, AnyNtermFixedMods
    '                 AnyNtermSite2FixedMod, AnyNtermSite2VariableMod, AnyNtermVariableMods, AnywehereSite2FixedMod, AnywehereSite2VariableMod
    '                 AnywhereFixedMods, AnywhereVariableMods, Code2AminoAcidObj, Code2ID, ID2Code
    '                 NotCtermFixedMods, NotCtermSite2FixedMod, NotCtermSite2VariableMod, NotCtermVariableMods, ProteinCtermFixedMods
    '                 ProteinCtermSite2FixedMod, ProteinCtermSite2VariableMod, ProteinCtermVariableMods, ProteinNterm2FixedMod, ProteinNterm2VariableMod
    '                 ProteinNtermFixedMods, ProteinNtermVariableMods
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: GetAminoAcidDictionaryUsedInModificationProtocol, GetCode2ID, GetID2Code, GetInitializeObject, GetModificationProtocolDict
    '               IsEmptyOrNull
    ' 
    ' Class ModificationUtility
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetFastModifiedPeptides, (+2 Overloads) GetModificationContainer, GetModifiedAminoacidCode, GetModifiedComposition, GetModifiedCompositions
    '               GetModifiedPeptides
    ' 
    ' Class ModificationProtocol
    ' 
    '     Properties: ModifiedAA, ModifiedAACode, ModifiedComposition, ModSequence, OriginalAA
    ' 
    '     Function: IsModified
    ' 
    '     Sub: UpdateObjects, UpdateProtocol
    ' 
    ' Class Modification
    ' 
    '     Properties: Composition, CreateDate, Description, IsSelected, IsVariable
    '                 LastModifiedDate, ModificationSites, Position, ReporterCorrectionM1, ReporterCorrectionM2
    '                 ReporterCorrectionP1, ReporterCorrectionP2, ReporterCorrectionType, TerminusType, Title
    '                 Type, User
    ' 
    ' Class ModificationSite
    ' 
    '     Properties: DiagnosticIons, DiagnosticNLs, Site
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Class ModificationContainer
    ' fixed modifications

    Public Property ProteinNtermFixedMods As List(Of Modification)

    Public Property ProteinCtermFixedMods As List(Of Modification)

    Public Property AnyNtermFixedMods As List(Of Modification)

    Public Property AnyCtermFixedMods As List(Of Modification)

    Public Property AnywhereFixedMods As List(Of Modification)

    Public Property NotCtermFixedMods As List(Of Modification)


    Public Property AnywehereSite2FixedMod As Dictionary(Of Char, ModificationProtocol)

    Public Property NotCtermSite2FixedMod As Dictionary(Of Char, ModificationProtocol)

    Public Property AnyCtermSite2FixedMod As Dictionary(Of Char, ModificationProtocol)

    Public Property AnyNtermSite2FixedMod As Dictionary(Of Char, ModificationProtocol)

    Public Property ProteinCtermSite2FixedMod As Dictionary(Of Char, ModificationProtocol)

    Public Property ProteinNterm2FixedMod As Dictionary(Of Char, ModificationProtocol)

    ' variable modifications

    Public Property ProteinNtermVariableMods As List(Of Modification)

    Public Property ProteinCtermVariableMods As List(Of Modification)

    Public Property AnyNtermVariableMods As List(Of Modification)

    Public Property AnyCtermVariableMods As List(Of Modification)

    Public Property AnywhereVariableMods As List(Of Modification)

    Public Property NotCtermVariableMods As List(Of Modification)


    Public Property AnywehereSite2VariableMod As Dictionary(Of Char, ModificationProtocol)

    Public Property NotCtermSite2VariableMod As Dictionary(Of Char, ModificationProtocol)

    Public Property AnyCtermSite2VariableMod As Dictionary(Of Char, ModificationProtocol)

    Public Property AnyNtermSite2VariableMod As Dictionary(Of Char, ModificationProtocol)

    Public Property ProteinCtermSite2VariableMod As Dictionary(Of Char, ModificationProtocol)

    Public Property ProteinNterm2VariableMod As Dictionary(Of Char, ModificationProtocol)


    Public Property Code2AminoAcidObj As Dictionary(Of String, AminoAcid)

    Public Property ID2Code As Dictionary(Of Integer, String)

    Public Property Code2ID As Dictionary(Of String, Integer)

    Public Function IsEmptyOrNull() As Boolean
        Return ProteinNtermFixedMods.IsNullOrEmpty AndAlso ProteinCtermFixedMods.IsNullOrEmpty AndAlso AnyNtermFixedMods.IsNullOrEmpty AndAlso AnyCtermFixedMods.IsNullOrEmpty AndAlso AnywhereFixedMods.IsNullOrEmpty AndAlso NotCtermFixedMods.IsNullOrEmpty AndAlso NotCtermSite2FixedMod.IsNullOrEmpty AndAlso AnyCtermSite2FixedMod.IsNullOrEmpty AndAlso AnyNtermSite2FixedMod.IsNullOrEmpty AndAlso ProteinCtermSite2FixedMod.IsNullOrEmpty AndAlso ProteinNterm2FixedMod.IsNullOrEmpty AndAlso ProteinNtermVariableMods.IsNullOrEmpty AndAlso ProteinCtermVariableMods.IsNullOrEmpty AndAlso AnyNtermVariableMods.IsNullOrEmpty AndAlso AnyCtermVariableMods.IsNullOrEmpty AndAlso AnywhereVariableMods.IsNullOrEmpty AndAlso NotCtermVariableMods.IsNullOrEmpty AndAlso NotCtermSite2VariableMod.IsNullOrEmpty AndAlso AnyCtermSite2VariableMod.IsNullOrEmpty AndAlso AnyNtermSite2VariableMod.IsNullOrEmpty AndAlso ProteinCtermSite2VariableMod.IsNullOrEmpty AndAlso ProteinNterm2VariableMod.IsNullOrEmpty
    End Function

    Public Sub New()

    End Sub

    Public Sub New(modifications As List(Of Modification))
        ProteinNtermFixedMods = modifications.Where(Function(n) Not n.IsVariable AndAlso Equals(n.Position, "proteinNterm")).ToList()
        ProteinCtermFixedMods = modifications.Where(Function(n) Not n.IsVariable AndAlso Equals(n.Position, "proteinCterm")).ToList()
        AnyNtermFixedMods = modifications.Where(Function(n) Not n.IsVariable AndAlso Equals(n.Position, "anyNterm")).ToList()
        AnyCtermFixedMods = modifications.Where(Function(n) Not n.IsVariable AndAlso Equals(n.Position, "anyCterm")).ToList()
        AnywhereFixedMods = modifications.Where(Function(n) Not n.IsVariable AndAlso Equals(n.Position, "anywhere")).ToList()
        NotCtermFixedMods = modifications.Where(Function(n) Not n.IsVariable AndAlso Equals(n.Position, "notCterm")).ToList()

        AnywehereSite2FixedMod = GetModificationProtocolDict(AnywhereFixedMods)
        NotCtermSite2FixedMod = GetModificationProtocolDict(NotCtermFixedMods)
        AnyCtermSite2FixedMod = GetModificationProtocolDict(AnyCtermFixedMods)
        AnyNtermSite2FixedMod = GetModificationProtocolDict(AnyNtermFixedMods)
        ProteinCtermSite2FixedMod = GetModificationProtocolDict(ProteinCtermFixedMods)
        ProteinNterm2FixedMod = GetModificationProtocolDict(ProteinNtermFixedMods)

        ProteinNtermVariableMods = modifications.Where(Function(n) n.IsVariable AndAlso Equals(n.Position, "proteinNterm")).ToList()
        ProteinCtermVariableMods = modifications.Where(Function(n) n.IsVariable AndAlso Equals(n.Position, "proteinCterm")).ToList()
        AnyNtermVariableMods = modifications.Where(Function(n) n.IsVariable AndAlso Equals(n.Position, "anyNterm")).ToList()
        AnyCtermVariableMods = modifications.Where(Function(n) n.IsVariable AndAlso Equals(n.Position, "anyCterm")).ToList()
        AnywhereVariableMods = modifications.Where(Function(n) n.IsVariable AndAlso Equals(n.Position, "anywhere")).ToList()
        NotCtermVariableMods = modifications.Where(Function(n) n.IsVariable AndAlso Equals(n.Position, "notCterm")).ToList()

        AnywehereSite2VariableMod = GetModificationProtocolDict(AnywhereVariableMods)
        NotCtermSite2VariableMod = GetModificationProtocolDict(NotCtermVariableMods)
        AnyCtermSite2VariableMod = GetModificationProtocolDict(AnyCtermVariableMods)
        AnyNtermSite2VariableMod = GetModificationProtocolDict(AnyNtermVariableMods)
        ProteinCtermSite2VariableMod = GetModificationProtocolDict(ProteinCtermVariableMods)
        ProteinNterm2VariableMod = GetModificationProtocolDict(ProteinNtermVariableMods)

        Code2AminoAcidObj = GetAminoAcidDictionaryUsedInModificationProtocol()
        ID2Code = GetID2Code()
        Code2ID = GetCode2ID()
    End Sub

    Private Function GetCode2ID() As Dictionary(Of String, Integer)
        Dim dict = New Dictionary(Of String, Integer)()
        For Each item In ID2Code
            dict(item.Value) = item.Key
        Next
        Return dict
    End Function

    Private Function GetID2Code() As Dictionary(Of Integer, String)
        Dim counter = 0
        Dim dict = New Dictionary(Of Integer, String)()
        For Each item In Code2AminoAcidObj
            dict(counter) = item.Key
            counter += 1
        Next
        Return dict
    End Function

    Public Function GetAminoAcidDictionaryUsedInModificationProtocol() As Dictionary(Of String, AminoAcid)
        Dim aaletters = AminoAcidObjUtility.AminoAcidLetters
        Dim dict = New Dictionary(Of String, AminoAcid)()
        Dim aminoacids = New List(Of AminoAcid)()
        For Each oneletter In aaletters ' initialize normal amino acids
            Dim aa = New AminoAcid(oneletter)
            aminoacids.Add(aa)
        Next

        ' fixed modification's amino acids
        Dim fixedModificationAAs = New List(Of AminoAcid)()
        For Each aa In aminoacids
            Dim modseq1 = New List(Of Modification)()
            'isPeptideNTerminal, isPeptideCTerminal, isProteinNTerminal, isProteinCTerminal
            Dim mAA1 = PeptideCalc.GetAminoAcidByFixedModifications(modseq1, Me, aa.OneLetter, False, False, False, False)
            fixedModificationAAs.Add(mAA1)
            Dim modseq2 = New List(Of Modification)()
            Dim mAA2 = PeptideCalc.GetAminoAcidByFixedModifications(modseq2, Me, aa.OneLetter, True, False, False, False)
            fixedModificationAAs.Add(mAA2)

            Dim modseq3 = New List(Of Modification)()
            Dim mAA3 = PeptideCalc.GetAminoAcidByFixedModifications(modseq3, Me, aa.OneLetter, False, True, False, False)
            fixedModificationAAs.Add(mAA3)

            Dim modseq4 = New List(Of Modification)()
            Dim mAA4 = PeptideCalc.GetAminoAcidByFixedModifications(modseq4, Me, aa.OneLetter, True, False, True, False)
            fixedModificationAAs.Add(mAA4)

            Dim modseq5 = New List(Of Modification)()
            Dim mAA5 = PeptideCalc.GetAminoAcidByFixedModifications(modseq5, Me, aa.OneLetter, False, True, False, True)
            fixedModificationAAs.Add(mAA5)
        Next

        ' fixed modification's amino acids
        Dim variableModificationAAs = New List(Of AminoAcid)()
        For Each aa In fixedModificationAAs
            Dim modseq1 = If(aa.Modifications.IsNullOrEmpty, New List(Of Modification)(), aa.Modifications.ToList())

            'isPeptideNTerminal, isPeptideCTerminal, isProteinNTerminal, isProteinCTerminal
            Dim mAA1 = PeptideCalc.GetAminoAcidByVariableModifications(modseq1, Me, aa.OneLetter, False, False, False, False)
            variableModificationAAs.Add(mAA1)
            Dim modseq2 = If(aa.Modifications.IsNullOrEmpty, New List(Of Modification)(), aa.Modifications.ToList())
            Dim mAA2 = PeptideCalc.GetAminoAcidByVariableModifications(modseq2, Me, aa.OneLetter, True, False, False, False)
            variableModificationAAs.Add(mAA2)

            Dim modseq3 = If(aa.Modifications.IsNullOrEmpty, New List(Of Modification)(), aa.Modifications.ToList())
            Dim mAA3 = PeptideCalc.GetAminoAcidByVariableModifications(modseq3, Me, aa.OneLetter, False, True, False, False)
            variableModificationAAs.Add(mAA3)

            Dim modseq4 = If(aa.Modifications.IsNullOrEmpty, New List(Of Modification)(), aa.Modifications.ToList())
            Dim mAA4 = PeptideCalc.GetAminoAcidByVariableModifications(modseq4, Me, aa.OneLetter, True, False, True, False)
            variableModificationAAs.Add(mAA4)

            Dim modseq5 = If(aa.Modifications.IsNullOrEmpty, New List(Of Modification)(), aa.Modifications.ToList())
            Dim mAA5 = PeptideCalc.GetAminoAcidByVariableModifications(modseq5, Me, aa.OneLetter, False, True, False, True)
            variableModificationAAs.Add(mAA5)
        Next

        ' finalization
        For Each aa In aminoacids
            dict(aa.OneLetter.ToString()) = aa
        Next

        For Each aa In fixedModificationAAs
            Dim code = aa.Code().ToString()
            If Not dict.ContainsKey(code) Then dict(code) = aa
        Next

        For Each aa In variableModificationAAs
            Dim code = aa.Code().ToString()
            If Not dict.ContainsKey(code) Then dict(code) = aa
        Next

        Return dict
    End Function


    Public Function GetModificationProtocolDict(modifications As List(Of Modification)) As Dictionary(Of Char, ModificationProtocol)
        Dim dict = GetInitializeObject()
        For Each [mod] In modifications
            For Each site In [mod].ModificationSites
                If Equals(site.Site.Trim(), "-") Then ' meaning that the modification is executed in any of amion acid species
                    For Each pair In dict
                        pair.Value.UpdateProtocol(pair.Key, [mod])
                    Next
                ElseIf dict.ContainsKey(site.Site(0)) Then
                    dict(site.Site(0)).UpdateProtocol(site.Site(0), [mod])
                End If
            Next
        Next
        Return dict
    End Function

    Public Function GetInitializeObject() As Dictionary(Of Char, ModificationProtocol)
        Dim aaletters = AminoAcidLetters
        Dim dict = New Dictionary(Of Char, ModificationProtocol)()

        For Each oneletter In aaletters
            dict(oneletter) = New ModificationProtocol() With {
                .OriginalAA = New AminoAcid(oneletter)
            }
        Next
        Return dict
    End Function
End Class

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

    Public Shared Function GetModificationContainer(fixedModifications As List(Of Modification), variableModifications As List(Of Modification), selectedFixedModifications As List(Of String), selectedVariableModifications As List(Of String)) As ModificationContainer

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


    Public Shared Function GetModifiedPeptides(peptides As List(Of Peptide), modContainer As ModificationContainer, Optional maxNumberOfModificationsPerPeptide As Integer = 2, Optional minPeptideMass As Double = 300, Optional maxPeptideMass As Double = 4600) As List(Of Peptide)
        Dim mPeptides = New List(Of Peptide)()
        For Each peptide In peptides
            Dim results = PeptideCalc.Sequence2Peptides(peptide, modContainer, maxNumberOfModificationsPerPeptide, minPeptideMass, maxPeptideMass)
            For Each result In results.SafeQuery
                mPeptides.Add(result)
            Next
        Next
        Return mPeptides
    End Function

    Public Shared Function GetFastModifiedPeptides(peptides As List(Of Peptide), modContainer As ModificationContainer, Optional maxNumberOfModificationsPerPeptide As Integer = 2, Optional minPeptideMass As Double = 300, Optional maxPeptideMass As Double = 4600) As List(Of Peptide)
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

'proteinNterm modification is allowed only once.
'proteinCterm modification is allowed only once.
'anyCterm modification is allowed only once.
'anyNterm modification is allowed only once.

Public Class Modification

    Public Property Title As String

    Public Property Description As String

    Public Property CreateDate As String

    Public Property LastModifiedDate As String

    Public Property User As String

    Public Property ReporterCorrectionM2 As Integer

    Public Property ReporterCorrectionM1 As Integer

    Public Property ReporterCorrectionP1 As Integer

    Public Property ReporterCorrectionP2 As Integer

    Public Property ReporterCorrectionType As Boolean

    Public Property Composition As Formula ' only derivative moiety 

    Public Property ModificationSites As List(Of ModificationSite) = New List(Of ModificationSite)()

    Public Property Position As String ' anyCterm, anyNterm, anywhere, notCterm, proteinCterm, proteinNterm

    Public Property Type As String ' Standard, Label, IsobaricLabel, Glycan, AaSubstitution, CleavedCrosslink, NeuCodeLabel

    Public Property TerminusType As String

    Public Property IsSelected As Boolean

    Public Property IsVariable As Boolean
End Class


Public Class ModificationSite

    Public Property Site As String

    Public Property DiagnosticIons As List(Of ProductIon) = New List(Of ProductIon)()

    Public Property DiagnosticNLs As List(Of NeutralLoss) = New List(Of NeutralLoss)()
End Class
