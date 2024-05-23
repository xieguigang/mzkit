#Region "Microsoft.VisualBasic::db2e6fe1c09ac85d192fc92ba2c0ece9, metadb\SMILES\Graph\ChemicalKey.vb"

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

    '   Total Lines: 18
    '    Code Lines: 7 (38.89%)
    ' Comment Lines: 7 (38.89%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (22.22%)
    '     File Size: 510 B


    ' Class ChemicalKey
    ' 
    '     Properties: bond
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network

''' <summary>
''' the edge connection between the atoms
''' </summary>
Public Class ChemicalKey : Inherits Edge(Of ChemicalElement)

    ''' <summary>
    ''' the charge of current chemical key
    ''' </summary>
    ''' <returns></returns>
    Public Property bond As Bonds

    Public Overrides Function ToString() As String
        Return $"{U.elementName}{bond.Description}{V.elementName} (+{CInt(bond)})"
    End Function

End Class
