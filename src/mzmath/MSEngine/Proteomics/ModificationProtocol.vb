#Region "Microsoft.VisualBasic::aff3c042b0232123de6487d6593e1a99, mzmath\MSEngine\Proteomics\ModificationProtocol.vb"

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

    '   Total Lines: 78
    '    Code Lines: 69 (88.46%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (11.54%)
    '     File Size: 3.08 KB


    ' Class ModificationProtocol
    ' 
    '     Properties: ModifiedAA, ModifiedAACode, ModifiedComposition, ModSequence, OriginalAA
    ' 
    '     Function: IsModified
    ' 
    '     Sub: UpdateObjects, UpdateProtocol
    ' 
    ' /********************************************************************************/

#End Region

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
