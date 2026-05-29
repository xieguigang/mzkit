#Region "Microsoft.VisualBasic::70b11fb8cf28e24511cf7e791ae14798, metadb\Chemoinformatics\test\fingerprintTest.vb"

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

    '   Total Lines: 26
    '    Code Lines: 22 (84.62%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (15.38%)
    '     File Size: 712 B


    ' Module fingerprintTest
    ' 
    '     Sub: Main333
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.MorganFingerprint
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models

Module fingerprintTest

    Sub Main333()
        Dim struct As New [Structure] With {.Atoms = {
            New Atom("C"),
            New Atom("O"),
            New Atom("O"),
            New Atom("H"),
            New Atom("H")
        },
        .Bounds = {
            New Bound(0, 1, BoundTypes.Double),
            New Bound(0, 2),
            New Bound(0, 3),
            New Bound(2, 4)
        }}

        Dim hash = struct.CalculateFingerprintCheckSum

        Call Console.WriteLine(BitConverter.ToString(hash))
        Call Pause()
    End Sub
End Module
