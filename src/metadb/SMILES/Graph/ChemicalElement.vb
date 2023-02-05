#Region "Microsoft.VisualBasic::90e0d87ce3e8d7fea0419c68a19d7e06, mzkit\src\metadb\SMILES\Graph\ChemicalElement.vb"

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

    '   Total Lines: 31
    '    Code Lines: 16
    ' Comment Lines: 8
    '   Blank Lines: 7
    '     File Size: 743.00 B


    ' Class ChemicalElement
    ' 
    '     Properties: coordinate, elementName, Keys
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network

''' <summary>
''' the chemical atom element
''' </summary>
Public Class ChemicalElement : Inherits Node

    Public Property elementName As String

    ''' <summary>
    ''' 与当前的这个元素连接的化学键的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Keys As Integer
        Get
            Return degree.In + degree.Out
        End Get
    End Property

    ''' <summary>
    ''' 2D coordinate ``[x,y]``
    ''' </summary>
    ''' <returns></returns>
    Public Property coordinate As Double()
    ''' <summary>
    ''' the atom group name
    ''' </summary>
    ''' <returns></returns>
    Public Property group As String

    Sub New()
    End Sub

    Sub New(element As String)
        Me.label = App.GetNextUniqueName($"{element}_")
        Me.elementName = element
    End Sub

    Public Shared Sub SetAtomGroups(formula As ChemicalFormula)
        Dim connected As New List(Of ChemicalElement)

        ' build connection edges
        For Each atom In formula.vertex
            For Each partner In formula.vertex.Where(Function(v) v IsNot atom)
                If formula.QueryEdge(atom.label, partner.label) IsNot Nothing Then
                    Call connected.Add(partner)
                End If
            Next

            Select Case atom.elementName
                Case "C"
                    Select Case connected.Count
                        Case 1 : atom.group = "-CH3"
                        Case 2 : atom.group = "-CH2-"
                        Case 3 : atom.group = "-CH="
                        Case Else
                            atom.group = "C???"
                    End Select
                Case "O"
                    Select Case connected.Count
                        Case 1 : atom.group = "-OH"
                        Case Else
                            atom.group = "O"
                    End Select
                Case "N"
                    Select Case connected.Count
                        Case 1 : atom.group = "-NH3"
                        Case 2 : atom.group = "-NH2-"
                        Case 3 : atom.group = "-NH--"
                        Case Else
                            atom.group = "N???"
                    End Select
                Case Else
                    atom.group = atom.elementName
            End Select

            Call connected.Clear()
        Next
    End Sub
End Class
