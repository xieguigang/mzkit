#Region "Microsoft.VisualBasic::b7877be2ef0acbbe575bba8f779e2c7d, DATA\PubChem.MySql\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: Main, parserTest
    ' 
    ' /********************************************************************************/

#End Region

Module Module1

    Sub Main()
        ' Call parserTest()

        Call PubChem.MySql.CreateMySqlDatabase("N:\pubchem\raw\SDF\uncompress", "N:\pubchem\raw\mysql", "N:\pubchem\raw\extras\metalib.Xml")
    End Sub

    Sub parserTest()

        Dim line$ = "2.0000-9999.9999    0.0000 I   0  0  0  0  0  0  0  0  0  0  0  0"
        Dim line2 = "4.9966   -1.2250    0.0000 H   0  0  0  0  0  0  0  0  0  0  0  0"
        Dim line3 = "-96589-9999.999949999.1992 H   0  0  0  0  0  0  0  0  0  0  0  0"
        Dim atom3 = SMRUCC.MassSpectrum.DATA.File.Atom.Parse(line3)
        Dim atom2 = SMRUCC.MassSpectrum.DATA.File.Atom.Parse(line2)
        Dim atom1 = SMRUCC.MassSpectrum.DATA.File.Atom.Parse(line)

        Call atom1.__DEBUG_ECHO
        Call atom2.__DEBUG_ECHO
        Call atom3.__DEBUG_ECHO

        Pause()
    End Sub

End Module

