#Region "Microsoft.VisualBasic::fcd567f496055e840cb47f387d80be9c, mzmath\MSEngine\Proteomics\ModificationUtility.vb"

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

    '   Total Lines: 143
    '    Code Lines: 124 (86.71%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 19 (13.29%)
    '     File Size: 6.28 KB


    ' Class ModificationUtility
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetFastModifiedPeptides, (+2 Overloads) GetModificationContainer, GetModifiedAminoacidCode, GetModifiedComposition, GetModifiedCompositions
    '               GetModifiedPeptides
    ' 
    ' /********************************************************************************/

#End Region


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

