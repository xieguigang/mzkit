#Region "Microsoft.VisualBasic::d17dfd0eca8de221d5a6b0eb9bb0bb2a, metadb\SMILES\Graph\Bonds.vb"

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

    '   Total Lines: 19
    '    Code Lines: 9 (47.37%)
    ' Comment Lines: 8 (42.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (10.53%)
    '     File Size: 689 B


    ' Enum Bonds
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models

''' <summary>
''' Single, double, triple, and aromatic bonds are 
''' represented by the symbols ``-``, ``=``, ``#``, and ``:``, 
''' respectively. Adjacent atoms are assumed to be 
''' connected to each other by a single or aromatic 
''' bond (single and aromatic bonds may always be 
''' omitted).
''' </summary>
Public Enum Bonds As Byte
    NA = BoundTypes.Other

    <Description("-")> [single] = BoundTypes.Single
    <Description("=")> [double] = BoundTypes.Double
    <Description("#")> triple = BoundTypes.Triple
    <Description(":")> aromatic = BoundTypes.Aromatic
End Enum
