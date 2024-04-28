#Region "Microsoft.VisualBasic::9befff8e3fd91d093138e5dc77178bb9, E:/mzkit/src/metadb/Chemoinformatics//Formula/FormulaCalculateUtility.vb"

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

    '   Total Lines: 305
    '    Code Lines: 272
    ' Comment Lines: 0
    '   Blank Lines: 33
    '     File Size: 17.57 KB


    '     Module AtomMass
    ' 
    ' 
    ' 
    '     Module FormulaCalculateUtility
    ' 
    '         Function: ConvertFormulaAdductPairToPrecursorAdduct, ConvertTmsMeoxSubtractedFormula, (+5 Overloads) GetExactMass, (+4 Overloads) GetFormulaString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Namespace Formula


    Public Module AtomMass
        Public cMass As Double = 12.0
        Public hMass As Double = 1.007825032
        Public nMass As Double = 14.003074
        Public oMass As Double = 15.99491462
        Public pMass As Double = 30.97376163
        Public sMass As Double = 31.972071
        Public fMass As Double = 18.99840322
        Public clMass As Double = 34.96885268
        Public brMass As Double = 78.9183371
        Public iMass As Double = 126.904473
        Public siMass As Double = 27.97692653
    End Module

    Public Module FormulaCalculateUtility


        Public Function ConvertFormulaAdductPairToPrecursorAdduct(formula As Formula, adduct As AdductIon) As Formula

            If adduct.IonMode = IonModes.Positive Then
                Select Case adduct.AdductIonName
                    Case "[M+H]+"
                        Return formula + "H"
                    Case "[M]+"
                        Return formula
                    Case "[M+NH4]+"
                        Return formula + "NH4"
                    Case "[M+Na]+"
                        Return formula
                    Case "[M+H-H2O]+"
                        Return formula - "OH"
                    Case Else
                        Return formula
                End Select
            Else
                Select Case adduct.AdductIonName
                    Case "[M-H]-"
                        Return formula - "H"
                    Case "[M-H2O-H]-"
                        Return formula - "H3O"
                    Case "[M+FA-H]-"
                        Return formula + "CHO2"
                    Case "[M+Hac-H]-"
                        Return formula + "C2H3O2"
                    Case Else
                        Return formula
                End Select
            End If
        End Function

        Public Function ConvertTmsMeoxSubtractedFormula(formula As Formula) As Formula
            If formula!Tms = 0 AndAlso formula!Meox = 0 Then Return formula
            Dim tmsCount = formula!Tms
            Dim meoxCount = formula!Meox

            Dim cNum = formula!C - tmsCount * 3 - meoxCount
            Dim hNum = formula!H - tmsCount * 8 - meoxCount * 3
            Dim nNum = formula!N - meoxCount
            Dim oNum = formula!O
            Dim pNum = formula!P
            Dim sNum = formula!S
            Dim fNum = formula!F
            Dim clNum = formula!Cl
            Dim brNum = formula!Br
            Dim iNum = formula!I
            Dim siNum = formula!Si - tmsCount
            Dim counts As New Dictionary(Of String, Integer) From {
                {"C", cNum}, {"H", hNum}, {"N", nNum}, {"O", oNum}, {"P", pNum},
                {"S", sNum}, {"F", fNum}, {"Cl", clNum}, {"Br", brNum}, {"I", iNum},
                {"Si", siNum}
            }

            Return New Formula(counts)
        End Function

        Public Function GetExactMass(element2count As Dictionary(Of String, Integer)) As Double
            Dim mass = 0.0
            Dim elem2mass = ElementDictionary.MassDict
            For Each pair In element2count
                If elem2mass.ContainsKey(pair.Key) Then
                    mass += elem2mass(pair.Key) * pair.Value
                End If
            Next
            Return mass
        End Function

        Public Function GetExactMass(cnum As Integer, hnum As Integer, nnum As Integer, onum As Integer, pnum As Integer, snum As Integer, fnum As Integer, clnum As Integer, brnum As Integer, inum As Integer, sinum As Integer) As Double
            Dim mass = 0.0

            mass = hMass * hnum + cMass * cnum + nMass * nnum + oMass * onum + pMass * pnum + sMass * snum + fMass * fnum + clMass * clnum + brMass * brnum + siMass * sinum + iMass * inum

            Return mass
        End Function

        Public Function GetExactMass(cnum As Integer, hnum As Integer, nnum As Integer, onum As Integer, pnum As Integer, snum As Integer, fnum As Integer, clnum As Integer, brnum As Integer, inum As Integer, sinum As Integer, cLabelMass As Double, hLabelMass As Double, nLabelMass As Double, oLabelMass As Double, pLabelMass As Double, sLabelMass As Double, fLabelMass As Double, clLabelMass As Double, brLabelMass As Double, iLabelMass As Double, siLabelMass As Double) As Double
            Dim mass = 0.0
            Dim cMass = AtomMass.cMass
            If cLabelMass > 0 Then cMass = cLabelMass
            Dim hMass = AtomMass.hMass
            If hLabelMass > 0 Then hMass = hLabelMass
            Dim nMass = AtomMass.nMass
            If nLabelMass > 0 Then nMass = nLabelMass
            Dim oMass = AtomMass.oMass
            If oLabelMass > 0 Then oMass = oLabelMass
            Dim pMass = AtomMass.pMass
            If pLabelMass > 0 Then pMass = pLabelMass
            Dim sMass = AtomMass.sMass
            If sLabelMass > 0 Then sMass = sLabelMass
            Dim fMass = AtomMass.fMass
            If fLabelMass > 0 Then fMass = fLabelMass
            Dim clMass = AtomMass.clMass
            If clLabelMass > 0 Then clMass = clLabelMass
            Dim brMass = AtomMass.brMass
            If brLabelMass > 0 Then brMass = brLabelMass
            Dim iMass = AtomMass.iMass
            If iLabelMass > 0 Then iMass = iLabelMass
            Dim siMass = AtomMass.siMass
            If siLabelMass > 0 Then siMass = siLabelMass

            mass = hMass * hnum + cMass * cnum + nMass * nnum + oMass * onum + pMass * pnum + sMass * snum + fMass * fnum + clMass * clnum + brMass * brnum + siMass * sinum + iMass * inum

            Return mass
        End Function
        Public Function GetExactMass(cnum As Integer, hnum As Integer, nnum As Integer, onum As Integer, pnum As Integer, snum As Integer, fnum As Integer, clnum As Integer, brnum As Integer, inum As Integer, sinum As Integer, c13num As Integer, h2num As Integer, n15num As Integer, o18num As Integer, s34num As Integer, cl37num As Integer, br81num As Integer, senum As Integer) As Double
            Dim mass = HydrogenMass * hnum + CarbonMass * cnum + NitrogenMass * nnum + OxygenMass * onum + PhosphorusMass * pnum + SulfurMass * snum + FluorideMass * fnum + ChlorideMass * clnum + BromineMass * brnum + SiliconMass * sinum + IodineMass * inum + Hydrogen2Mass * h2num + Carbon13Mass * c13num + Nitrogen15Mass * n15num + Oxygen18Mass * o18num + Sulfur34Mass * s34num + Chloride37Mass * cl37num + Bromine81Mass * br81num + SeleniumMass * senum




            Return mass
        End Function

        Public Function GetExactMass(cnum As Integer, hnum As Integer, nnum As Integer, onum As Integer, pnum As Integer, snum As Integer, fnum As Integer, clnum As Integer, brnum As Integer, inum As Integer, sinum As Integer, c13num As Integer, h2num As Integer) As Double
            Dim mass = HydrogenMass * hnum + CarbonMass * cnum + NitrogenMass * nnum + OxygenMass * onum + PhosphorusMass * pnum + SulfurMass * snum + FluorideMass * fnum + ChlorideMass * clnum + BromineMass * brnum + SiliconMass * sinum + IodineMass * inum + Hydrogen2Mass * h2num + Carbon13Mass * c13num
            Return mass
        End Function

        Public Function GetFormulaString(cnum As Integer, hnum As Integer, nnum As Integer, onum As Integer, pnum As Integer, snum As Integer, fnum As Integer, clnum As Integer, brnum As Integer, inum As Integer, sinum As Integer, Optional tmsCount As Integer = 0, Optional meoxCount As Integer = 0) As String
            Dim formulaString = String.Empty
            Dim lCNum = cnum - tmsCount * 3 - meoxCount
            Dim lHNum = hnum - tmsCount * 8 - meoxCount * 3
            Dim lBrNum = brnum
            Dim lClNum = clnum
            Dim lFNum = fnum
            Dim lINum = inum
            Dim lNNum = nnum - meoxCount
            Dim lONum = onum
            Dim lPNum = pnum
            Dim lSNum = snum
            Dim lSiNum = sinum - tmsCount

            If lCNum > 0 Then
                If lCNum = 1 Then
                    formulaString += "C"
                Else
                    formulaString += "C" & lCNum.ToString()
                End If
            End If
            If lHNum > 0 Then
                If lHNum = 1 Then
                    formulaString += "H"
                Else
                    formulaString += "H" & lHNum.ToString()
                End If
            End If
            If lBrNum > 0 Then
                If lBrNum = 1 Then
                    formulaString += "Br"
                Else
                    formulaString += "Br" & lBrNum.ToString()
                End If
            End If
            If lClNum > 0 Then
                If lClNum = 1 Then
                    formulaString += "Cl"
                Else
                    formulaString += "Cl" & lClNum.ToString()
                End If
            End If
            If lFNum > 0 Then
                If lFNum = 1 Then
                    formulaString += "F"
                Else
                    formulaString += "F" & lFNum.ToString()
                End If
            End If
            If lINum > 0 Then
                If lINum = 1 Then
                    formulaString += "I"
                Else
                    formulaString += "I" & lINum.ToString()
                End If
            End If
            If lNNum > 0 Then
                If lNNum = 1 Then
                    formulaString += "N"
                Else
                    formulaString += "N" & lNNum.ToString()
                End If
            End If
            If lONum > 0 Then
                If lONum = 1 Then
                    formulaString += "O"
                Else
                    formulaString += "O" & lONum.ToString()
                End If
            End If
            If lPNum > 0 Then
                If lPNum = 1 Then
                    formulaString += "P"
                Else
                    formulaString += "P" & lPNum.ToString()
                End If
            End If
            If lSNum > 0 Then
                If lSNum = 1 Then
                    formulaString += "S"
                Else
                    formulaString += "S" & lSNum.ToString()
                End If
            End If
            If lSiNum > 0 Then
                If lSiNum = 1 Then
                    formulaString += "Si"
                Else
                    formulaString += "Si" & lSiNum.ToString()
                End If
            End If

            If tmsCount > 0 Then formulaString += "_" & tmsCount.ToString() & "TMS"
            If meoxCount > 0 Then formulaString += "_" & meoxCount.ToString() & "MEOX"

            Return formulaString
        End Function

        Public Function GetFormulaString(cnum As Integer, hnum As Integer, nnum As Integer, onum As Integer, pnum As Integer, snum As Integer, fnum As Integer, clnum As Integer, brnum As Integer, inum As Integer, sinum As Integer, c13num As Integer, h2num As Integer, n15num As Integer, o18num As Integer, s34num As Integer, cl37num As Integer, br81num As Integer, senum As Integer) As String
            Dim formula = String.Empty

            formula += If(cnum = 1, "C", If(cnum > 1, "C" & cnum.ToString(), If(cnum < 0, "C(" & cnum.ToString() & ")", String.Empty)))
            formula += If(c13num = 1, "[13C]", If(c13num > 1, "[13C]" & c13num.ToString(), If(c13num < 0, "C(" & c13num.ToString() & ")", String.Empty)))
            formula += If(hnum = 1, "H", If(hnum > 1, "H" & hnum.ToString(), If(hnum < 0, "C(" & hnum.ToString() & ")", String.Empty)))
            formula += If(h2num = 1, "[2H]", If(h2num > 1, "[2H]" & h2num.ToString(), If(h2num < 0, "C(" & h2num.ToString() & ")", String.Empty)))
            formula += If(brnum = 1, "Br", If(brnum > 1, "Br" & brnum.ToString(), If(brnum < 0, "C(" & brnum.ToString() & ")", String.Empty)))
            formula += If(br81num = 1, "[81Br]", If(br81num > 1, "[81Br]" & br81num.ToString(), If(br81num < 0, "C(" & br81num.ToString() & ")", String.Empty)))
            formula += If(clnum = 1, "Cl", If(clnum > 1, "Cl" & clnum.ToString(), If(clnum < 0, "C(" & clnum.ToString() & ")", String.Empty)))
            formula += If(cl37num = 1, "[37Cl]", If(cl37num > 1, "[37Cl]" & cl37num.ToString(), If(cl37num < 0, "C(" & cl37num.ToString() & ")", String.Empty)))
            formula += If(fnum = 1, "F", If(fnum > 1, "F" & fnum.ToString(), If(fnum < 0, "C(" & fnum.ToString() & ")", String.Empty)))
            formula += If(inum = 1, "I", If(inum > 1, "I" & inum.ToString(), If(inum < 0, "C(" & inum.ToString() & ")", String.Empty)))
            formula += If(nnum = 1, "N", If(nnum > 1, "N" & nnum.ToString(), If(nnum < 0, "C(" & nnum.ToString() & ")", String.Empty)))
            formula += If(n15num = 1, "[15N]", If(n15num > 1, "[15N]" & n15num.ToString(), If(n15num < 0, "C(" & n15num.ToString() & ")", String.Empty)))
            formula += If(onum = 1, "O", If(onum > 1, "O" & onum.ToString(), If(onum < 0, "C(" & onum.ToString() & ")", String.Empty)))
            formula += If(o18num = 1, "[18O]", If(o18num > 1, "[18O]" & o18num.ToString(), If(o18num < 0, "C(" & o18num.ToString() & ")", String.Empty)))
            formula += If(pnum = 1, "P", If(pnum > 1, "P" & pnum.ToString(), If(pnum < 0, "C(" & pnum.ToString() & ")", String.Empty)))
            formula += If(snum = 1, "S", If(snum > 1, "S" & snum.ToString(), If(snum < 0, "C(" & snum.ToString() & ")", String.Empty)))
            formula += If(s34num = 1, "[34S]", If(s34num > 1, "[34S]" & s34num.ToString(), If(s34num < 0, "C(" & s34num.ToString() & ")", String.Empty)))
            formula += If(sinum = 1, "Si", If(sinum > 1, "Si" & sinum.ToString(), If(sinum < 0, "C(" & sinum.ToString() & ")", String.Empty)))
            formula += If(senum = 1, "Se", If(senum > 1, "Se" & senum.ToString(), If(senum < 0, "C(" & senum.ToString() & ")", String.Empty)))

            Return formula
        End Function
        Public Function GetFormulaString(cnum As Integer, hnum As Integer, nnum As Integer, onum As Integer, pnum As Integer, snum As Integer, fnum As Integer, clnum As Integer, brnum As Integer, inum As Integer, sinum As Integer, c13num As Integer, h2num As Integer, Optional tmsCount As Integer = 0, Optional meoxCount As Integer = 0) As String
            Dim formula = String.Empty

            formula += If(cnum = 1, "C", If(cnum > 1, "C" & cnum.ToString(), If(cnum < 0, "C(" & cnum.ToString() & ")", String.Empty)))
            formula += If(c13num = 1, "[13C]", If(c13num > 1, "[13C]" & c13num.ToString(), If(c13num < 0, "C(" & c13num.ToString() & ")", String.Empty)))
            formula += If(hnum = 1, "H", If(hnum > 1, "H" & hnum.ToString(), If(hnum < 0, "C(" & hnum.ToString() & ")", String.Empty)))
            formula += If(h2num = 1, "[2H]", If(h2num > 1, "[2H]" & h2num.ToString(), If(h2num < 0, "C(" & h2num.ToString() & ")", String.Empty)))
            formula += If(brnum = 1, "Br", If(brnum > 1, "Br" & brnum.ToString(), If(brnum < 0, "C(" & brnum.ToString() & ")", String.Empty)))
            formula += If(clnum = 1, "Cl", If(clnum > 1, "Cl" & clnum.ToString(), If(clnum < 0, "C(" & clnum.ToString() & ")", String.Empty)))
            formula += If(fnum = 1, "F", If(fnum > 1, "F" & fnum.ToString(), If(fnum < 0, "C(" & fnum.ToString() & ")", String.Empty)))
            formula += If(inum = 1, "I", If(inum > 1, "I" & inum.ToString(), If(inum < 0, "C(" & inum.ToString() & ")", String.Empty)))
            formula += If(nnum = 1, "N", If(nnum > 1, "N" & nnum.ToString(), If(nnum < 0, "C(" & nnum.ToString() & ")", String.Empty)))
            formula += If(onum = 1, "O", If(onum > 1, "O" & onum.ToString(), If(onum < 0, "C(" & onum.ToString() & ")", String.Empty)))
            formula += If(pnum = 1, "P", If(pnum > 1, "P" & pnum.ToString(), If(pnum < 0, "C(" & pnum.ToString() & ")", String.Empty)))
            formula += If(snum = 1, "S", If(snum > 1, "S" & snum.ToString(), If(snum < 0, "C(" & snum.ToString() & ")", String.Empty)))
            formula += If(sinum = 1, "Si", If(sinum > 1, "Si" & sinum.ToString(), If(sinum < 0, "C(" & sinum.ToString() & ")", String.Empty)))

            If tmsCount > 0 Then formula += "_" & tmsCount.ToString() & "TMS"
            If meoxCount > 0 Then formula += "_" & meoxCount.ToString() & "MEOX"

            Return formula
        End Function

        Public Function GetFormulaString(element2count As Dictionary(Of String, Integer)) As String
            Dim formulastring = String.Empty
            Dim elem2order = ElementDictionary.HillOrder
            Dim atoms = New List(Of (Element As String, count As Integer, order As Integer))()
            For Each pair In element2count
                If elem2order.ContainsKey(pair.Key) Then
                    Call atoms.Add((Element: =pair.Key, Count: =pair.Value, Order:=elem2order(pair.Key)))
                End If
            Next
            For Each atom In atoms.OrderBy(Function(n) n.order)
                formulastring += If(atom.count > 1, atom.Element & atom.count.ToString(), If(atom.count < 0, atom.Element & "(" & atom.count.ToString() & ")", atom.Element))
            Next
            Return formulastring
        End Function
    End Module
End Namespace
