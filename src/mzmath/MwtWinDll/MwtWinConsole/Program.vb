#Region "Microsoft.VisualBasic::68014d175e929bec99b0a068dacebd21, src\mzmath\MwtWinDll\MwtWinConsole\Program.vb"

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

    ' Module Program
    ' 
    '     Sub: Main, test2, testSplit
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports PNNL.OMICS.MwtWinDll.Extensions
Imports PNNL.OMICS.MwtWinDll.FormulaFinder

Module Program

    Sub Main()

        'println("C00047:  C6H14N2O2")

        'Dim profile As New AtomProfiles({"C", "H", "N", "O"})
        'Dim list = profile.SearchByLimitDaMass(146.1055, deltaPPM:=10)

        'For Each f In list
        '    Call f.ToString.__DEBUG_ECHO
        'Next

        'Pause()
        Call testSplit()

        Call test2()

    End Sub

    Sub testSplit()

        Dim formula$ = "H2O"
        Dim composition As FormulaComposition ' = FormulaCompare.SplitFormula(formula)

        '   composition = FormulaCompare.SplitFormula("CO2")
        composition = FormulaCompare.SplitFormula("(CH3)3CH")
        composition = FormulaCompare.SplitFormula("(CH3)4C")
        composition = FormulaCompare.SplitFormula("(NH3)10C22H44N4O14P2Si2")

        Pause()
    End Sub

    Sub test2()
        Dim mz = 477.0631
        Dim profile As New AtomProfiles({"C", "H", "N", "O", "S", "P"})
        Dim list = profile.SearchByMZAndLimitCharges(New IntRange(2, 2), mz, 20).OrderBy(Function(f) f.CountsByElement("C"))

        For Each formual As FormulaFinderResult In list
            Call Console.WriteLine(formual.ToString)
        Next

        Pause()
    End Sub

End Module
