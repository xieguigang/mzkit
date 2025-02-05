﻿#Region "Microsoft.VisualBasic::fd236bbd4660535ad3f02a6dc0f492df, metadb\Chemoinformatics\InChI\Models\InchiInput.vb"

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

    '   Total Lines: 72
    '    Code Lines: 43 (59.72%)
    ' Comment Lines: 15 (20.83%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (19.44%)
    '     File Size: 2.42 KB


    '     Class InchiInput
    ' 
    '         Properties: Atoms, Bonds, Stereos
    ' 
    '         Function: getAtom, getBond
    ' 
    '         Sub: addAtom, (+2 Overloads) addBond, addStereo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

' JNA-InChI - Library for calling InChI from Java
' Copyright © 2018 Daniel Lowe
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public License
' along with this program.  If not, see </>.

Namespace IUPAC.InChI

    Public Class InchiInput

        Dim m_atoms As New Dictionary(Of UInteger, InchiAtom)
        Dim m_bonds As New List(Of InchiBond)()
        Dim m_stereos As New List(Of InchiStereo)()

        Public Overridable ReadOnly Property Atoms As InchiAtom()
            Get
                Return m_atoms.Values.ToArray
            End Get
        End Property

        Public Overridable ReadOnly Property Bonds As InchiBond()
            Get
                Return m_bonds.ToArray
            End Get
        End Property

        Public Overridable ReadOnly Property Stereos As InchiStereo()
            Get
                Return m_stereos.ToArray
            End Get
        End Property

        Public Overridable Sub addAtom(id As UInteger, atom As InchiAtom)
            m_atoms(id) = atom
        End Sub

        Public Overridable Sub addBond(bond As InchiBond)
            m_bonds.Add(bond)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub addBond(i As UInteger, j As UInteger, type As InchiBondType)
            Call addBond(New InchiBond(getAtom(i), getAtom(j), type))
        End Sub

        Public Overridable Sub addStereo(stereo As InchiStereo)
            m_stereos.Add(stereo)
        End Sub

        Public Overridable Function getAtom(i As UInteger) As InchiAtom
            Return m_atoms(i)
        End Function

        Public Overridable Function getBond(i As Integer) As InchiBond
            Return m_bonds(i)
        End Function
    End Class

End Namespace
