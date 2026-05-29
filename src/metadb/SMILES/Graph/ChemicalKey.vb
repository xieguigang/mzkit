#Region "Microsoft.VisualBasic::9b3796ac92762bab4260ab9e22312d76, metadb\SMILES\Graph\ChemicalKey.vb"

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

    '   Total Lines: 51
    '    Code Lines: 29 (56.86%)
    ' Comment Lines: 15 (29.41%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (13.73%)
    '     File Size: 1.45 KB


    ' Class ChemicalKey
    ' 
    '     Properties: bond, source, target
    ' 
    '     Function: AtomGroups, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

''' <summary>
''' the edge connection between the atoms
''' </summary>
Public Class ChemicalKey : Inherits Edge(Of ChemicalElement)
    Implements IInteraction

    ''' <summary>
    ''' the charge of current chemical key
    ''' </summary>
    ''' <returns></returns>
    Public Property bond As Bonds

    ''' <summary>
    ''' atom group of source atom, apply for build linking matrix
    ''' </summary>
    ''' <returns></returns>
    Private Property source As String Implements IInteraction.source
        Get
            Return U.group
        End Get
        Set(value As String)
            U.group = value
        End Set
    End Property

    ''' <summary>
    ''' atom group of target atom, apply for build linking matrix
    ''' </summary>
    ''' <returns></returns>
    Private Property target As String Implements IInteraction.target
        Get
            Return V.group
        End Get
        Set(value As String)
            V.group = value
        End Set
    End Property

    Public Iterator Function AtomGroups() As IEnumerable(Of ChemicalElement)
        Yield U
        Yield V
    End Function

    Public Overrides Function ToString() As String
        Return $"{U.elementName}{bond.Description}{V.elementName} (+{CInt(bond)})"
    End Function

End Class
