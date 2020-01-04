#Region "Microsoft.VisualBasic::95400e8c35a4346ba06b0c6d57eafeba, src\metadb\Massbank\test\Module1.vb"

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
'     Sub: convertorTest, dumpPubChem, Main
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry
Imports Microsoft.VisualBasic.ComponentModel.Ranges

Module Module1

    Sub Main()

        Call convertorTest()
        '  Call dumpPubChem()

        Pause()
    End Sub

    Sub convertorTest()

        Dim a As New UnitValue(Of Units)(100, Units.cuin_cuft)
        Dim b = a.ConvertTo(Units.milligrams_kg)
        Dim c = a.ConvertTo(Units.cuin_cuft)
        Dim d = a.ConvertTo(Units.drops_gallon_US)
        Dim e = a.ConvertTo(Units.ppm)

        Call $"{a} = {b}".__DEBUG_ECHO
        Call $"{a} = {c}".__DEBUG_ECHO
        Call $"{a} = {d}".__DEBUG_ECHO
        Call $"{a} = {e}".__DEBUG_ECHO

        Pause()
    End Sub

    Sub dumpPubChem()

        Call DumpingPubChemAnnotations("D:\Database\pubchem", "D:\Database\pubchem.sdk_keys.csv")


        Pause()
    End Sub
End Module
