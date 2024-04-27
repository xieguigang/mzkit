#Region "Microsoft.VisualBasic::cb8d6194800e3a3bd20213fda16a8812, G:/mzkit/src/metadb/SMILES//Graph/ChemicalElement.vb"

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

    '   Total Lines: 172
    '    Code Lines: 108
    ' Comment Lines: 47
    '   Blank Lines: 17
    '     File Size: 5.81 KB


    ' Class ChemicalElement
    ' 
    '     Properties: charge, coordinate, elementName, group, hydrogen
    '                 Keys
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetConnection
    ' 
    '     Sub: (+2 Overloads) SetAtomGroups, setAtomLabel
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network

''' <summary>
''' the chemical atom element
''' </summary>
Public Class ChemicalElement : Inherits Node

    ''' <summary>
    ''' the atom or atom group element label text
    ''' </summary>
    ''' <returns></returns>
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

    ''' <summary>
    ''' the ion charge value
    ''' </summary>
    ''' <returns></returns>
    Public Property charge As Integer
    ''' <summary>
    ''' The number of the hydrogen of current atom group it has
    ''' </summary>
    ''' <returns></returns>
    Public Property hydrogen As Integer = 0

    Sub New()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="element">
    ''' the atom or atom group element label text
    ''' </param>
    ''' <param name="index"></param>
    Sub New(element As String, Optional index As Integer? = Nothing)
        Me.label = If(
            index Is Nothing,
            App.GetNextUniqueName($"{element}_"),
            $"{element}_{CInt(index)}"
        )
        Me.elementName = element
    End Sub

    Public Shared Iterator Function GetConnection(formula As ChemicalFormula, atom As ChemicalElement) As IEnumerable(Of (keys As Bonds, ChemicalElement))
        Dim key1, key2 As ChemicalKey

        For Each partner As ChemicalElement In formula.vertex.Where(Function(v) v IsNot atom)
            key1 = formula.QueryEdge(atom.label, partner.label)

            If key1 IsNot Nothing Then
                Yield (key1.bond, partner)
            End If

            key2 = formula.QueryEdge(partner.label, atom.label)

            If key2 IsNot Nothing AndAlso Not key1 Is key2 Then
                Yield (key2.bond, partner)
            End If
        Next
    End Function

    ''' <summary>
    ''' Set atom group label based on the chemical keys
    ''' </summary>
    ''' <param name="formula"></param>
    Public Shared Sub SetAtomGroups(formula As ChemicalFormula)
        ' build connection edges
        For Each atom As ChemicalElement In formula.vertex
            Call SetAtomGroups(
                atom:=atom,
                keys:=Aggregate link
                      In GetConnection(formula, atom)
                      Into Sum(link.keys)
            )
        Next
    End Sub

    Private Sub setAtomLabel(label As String, nH As Integer)
        Dim atom As ChemicalElement = Me

        atom.group = label
        atom.hydrogen = nH
    End Sub

    ''' <summary>
    ''' Set atom group label based on the chemical keys
    ''' </summary>
    ''' <param name="atom"></param>
    ''' <param name="keys"></param>
    Private Shared Sub SetAtomGroups(atom As ChemicalElement, keys As Integer)
        Select Case atom.elementName
            Case "C"
                Select Case keys
                    Case 1 : atom.setAtomLabel("-CH3", 3)
                    Case 2 : atom.setAtomLabel("-CH2-", 2)
                    Case 3 : atom.setAtomLabel("-CH=", 1)
                    Case Else
                        atom.group = "C"
                End Select
            Case "O"
                Select Case keys
                    Case 1
                        If atom.charge = 0 Then
                            atom.setAtomLabel("-OH", 1)
                        Else
                            ' an ion with negative charge value
                            ' [O-]
                            atom.setAtomLabel("[O-]-", 0)
                        End If
                    Case Else
                        atom.group = "-O-"
                End Select
            Case "N"
                Dim n As Integer

                ' N -3
                Select Case keys
                    Case 1
                        If atom.charge = 0 Then
                            atom.setAtomLabel("-NH2", 2)
                        Else
                            n = 3 - atom.charge
                            atom.setAtomLabel($"[-NH{n}]{atom.charge}+", n)
                        End If
                    Case 2
                        If atom.charge = 0 Then
                            atom.setAtomLabel("-NH-", 1)
                        Else
                            n = 2 - atom.charge
                            atom.setAtomLabel($"[-NH{n}-]{atom.charge}+", n)
                        End If
                    Case 3
                        If atom.charge = 0 Then
                            atom.group = "-N="
                        Else
                            atom.group = $"[-N=]{atom.charge}+"
                        End If
                    Case Else
                        atom.group = "N"
                End Select
            Case Else
                If atom.charge = 0 OrElse SMILES.Atom.AtomGroups.ContainsKey(atom.elementName) Then
                    atom.group = atom.elementName
                ElseIf atom.charge > 0 Then
                    atom.group = $"[{atom.elementName}]{atom.charge}+"
                Else
                    atom.group = $"[{atom.elementName}]{-atom.charge}-"
                End If
        End Select
    End Sub
End Class
