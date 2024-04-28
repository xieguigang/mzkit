#Region "Microsoft.VisualBasic::6e0f28ad0b1c72a173510b336a8f1ac9, E:/mzkit/src/metadb/Chemoinformatics/test//nameTest.vb"

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

    '   Total Lines: 53
    '    Code Lines: 36
    ' Comment Lines: 3
    '   Blank Lines: 14
    '     File Size: 2.27 KB


    ' Module nameTest
    ' 
    '     Sub: echo, Main, parseLipids
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.NaturalProduct
Imports Microsoft.VisualBasic.Serialization.JSON

Module nameTest

    Dim parser As New GlycosylNameSolver

    Sub Main()
        Call parseLipids()

        Call echo(" 3-o-[6-o-(malonyl)-beta-d-glucopyranoside]-7-o-[6-o-(trans-p-coumaryl)-beta-d-glucopyranoside]-3'-o-[6-o-(trans-4-o-(6-o-(trans-4-o-(beta-d-glucopyranosyl)-p-coumaryl)-beta-d-glucopyranoside]")

        Call echo("  3-(6-p-coumaroylglucoside)-5-[6-(malonyl)-4-(rhamnosyl)glucoside)]")

        Call echo(" 3'-o-(2''-o-galloyl-6''-o-acetyl-beta-galactopyranoside)")
        Call echo(" ( 3-gentiobios-6'''-yl)(apigenin 7-glucos-6''-yl)malonate")

        Call echo("  3-o-(6-o-(4-o-alpha-rhamnopyranosyl-beta-glucopyranoside)-5-o-(6-o-malonyl-beta-glucopyranoside)")
        Call echo("  3-o-(6-o-(4-o-malonyl-alpha-rhamnopyranosyl)-beta-glucopyranoside)-5-o-beta-glucopyranoside")

        Call echo("Malvidin 3,7-di-(6-malonylglucoside)")
        Call echo("Delphinidin 3,5-di(6-acetylglucoside)")
        Call echo("Delphinidin 3,7,3'-triglucoside")
        Call echo("Delphinidin 3,7-diglucoside-3',5'-di(6-O-p-coumaroyl-beta-glucoside)")
        Call echo("Cyanidin 3-(6'',6'''-di-p-coumarylsophoroside)-5-(6-malonylglucoside)")
        Call echo("Cyanidin 3-O-[2''-O-(xylosyl)-6''-O-(p-coumaroyl) glucoside] 5-O-malonylglucoside")
        Call echo("Petunidin 3,5-di-O-beta-D-glucoside")

        Pause()
    End Sub

    Sub parseLipids()
        Dim print = Sub(str As String)
                        Dim name As LipidName = LipidName.ParseLipidName(str)

                        Call Console.WriteLine(name.ToString)
                        Call Console.WriteLine(name.GetJson)
                    End Sub

        ' Call print("LysoPC(20:5)")
        ' Call print("SM(d19:1/16:0)")
        ' Call print("LysoPC(20:5(5Z,8Z,11Z,14Z,17Z))")
        Call print("PC(22:6(4Z,7Z,10Z,13Z,16Z,19Z)/20:5(5Z,8Z,11Z,14Z,16E)-OH(18R))")

        Pause()
    End Sub

    Private Sub echo(name As String)
        Call Console.WriteLine(name)
        Call Console.WriteLine(parser.GlycosylNameParser(name).GetJson)
        Call Console.WriteLine(New String("-"c, 32))
    End Sub
End Module
