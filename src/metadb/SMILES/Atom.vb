#Region "Microsoft.VisualBasic::c00ccb7c395432c2e8d6d9ae4ddbf318, mzkit\src\metadb\SMILES\Atom.vb"

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

'   Total Lines: 21
'    Code Lines: 16
' Comment Lines: 0
'   Blank Lines: 5
'     File Size: 529.00 B


' Class Atom
' 
'     Properties: label, maxKeys
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: DefaultElements, ToString
' 
' /********************************************************************************/

#End Region

Public Class Atom

    Public Property label As String
    ''' <summary>
    ''' the max number of the chemical keys
    ''' (max charge number)
    ''' </summary>
    ''' <returns></returns>
    Public Property maxKeys As Integer

    Sub New(label As String, max As Integer)
        Me.label = label
        Me.maxKeys = max
    End Sub

    Public Overrides Function ToString() As String
        Return $"{label} ~ H{maxKeys}"
    End Function

    Public Shared Iterator Function DefaultElements() As IEnumerable(Of Atom)
        Yield New Atom("Li", 1)
        Yield New Atom("Na", 1)
        Yield New Atom("K", 1)
        Yield New Atom("Rb", 1)
        Yield New Atom("Cs", 1)
        Yield New Atom("Fr", 1)

        Yield New Atom("Be", 2)
        Yield New Atom("Mg", 2)
        Yield New Atom("Ca", 2)
        Yield New Atom("Sr", 2)
        Yield New Atom("Ba", 2)
        Yield New Atom("Ra", 2)

        Yield New Atom("Sc", 3)
        Yield New Atom("Y", 3)

        Yield New Atom("Cu", 2)
        Yield New Atom("Ag", 1)
        Yield New Atom("Au", 3)

        Yield New Atom("H", 1)

        Yield New Atom("C", 4)
        Yield New Atom("Si", 4)
        Yield New Atom("Pb", 4)
        Yield New Atom("F", 1)
        Yield New Atom("Cl", 7)
        Yield New Atom("Al", 3)
        Yield New Atom("O", 2)
        Yield New Atom("S", 6)
        Yield New Atom("N", 5)
        Yield New Atom("P", 5)

        Yield New Atom("Mn", 7)
        Yield New Atom("Fe", 6)
        Yield New Atom("Br", 7)
        Yield New Atom("Se", 6)
    End Function

End Class
