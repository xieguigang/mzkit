#Region "Microsoft.VisualBasic::1b7fc10ad2bda670ab1e43f8243b2729, mzmath\MSEngine\Proteomics\ModificationContainer.vb"

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

    '   Total Lines: 229
    '    Code Lines: 162 (70.74%)
    ' Comment Lines: 7 (3.06%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 60 (26.20%)
    '     File Size: 12.01 KB


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
    ' /********************************************************************************/

#End Region

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
