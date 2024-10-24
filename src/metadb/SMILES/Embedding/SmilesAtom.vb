﻿#Region "Microsoft.VisualBasic::d1fd1cbc67153d8c61383c29b5c8253a, metadb\SMILES\Embedding\SmilesAtom.vb"

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

    '   Total Lines: 14
    '    Code Lines: 10 (71.43%)
    ' Comment Lines: 3 (21.43%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 1 (7.14%)
    '     File Size: 430 B


    '     Class SmilesAtom
    ' 
    '         Properties: atom, connected, group, id, ion_charge
    '                     links
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Embedding

    ''' <summary>
    ''' A csv table represents of the smiles atom data
    ''' </summary>
    Public Class SmilesAtom

        Public Property id As String
        Public Property atom As String
        Public Property group As String
        Public Property ion_charge As Integer
        Public Property links As Integer
        Public Property connected As String()
        Public Property graph_id As Integer
        Public Property aromatic As Boolean

        ''' <summary>
        ''' get the group label string, this function will tagged the ``(aromatic)`` automatically
        ''' if current atom is on a ring of <see cref="aromatic"/>.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            If aromatic Then
                Return group & "(aromatic)"
            Else
                Return group
            End If
        End Function

    End Class
End Namespace
