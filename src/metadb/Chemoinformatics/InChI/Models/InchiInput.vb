Imports System.Collections.Generic

''' JNA-InChI - Library for calling InChI from Java
''' Copyright © 2018 Daniel Lowe
''' 
''' This library is free software; you can redistribute it and/or
''' modify it under the terms of the GNU Lesser General Public
''' License as published by the Free Software Foundation; either
''' version 2.1 of the License, or (at your option) any later version.
''' 
''' This program is distributed in the hope that it will be useful,
''' but WITHOUT ANY WARRANTY; without even the implied warranty of
''' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
''' GNU Lesser General Public License for more details.
''' 
''' You should have received a copy of the GNU Lesser General Public License
''' along with this program.  If not, see </>.

Namespace IUPAC.InChI

    Public Class InchiInput

        Private atomsField As IList(Of InchiAtom) = New List(Of InchiAtom)()
        Private bondsField As IList(Of InchiBond) = New List(Of InchiBond)()
        Private stereosField As IList(Of InchiStereo) = New List(Of InchiStereo)()

        Public Overridable Sub addAtom(atom As InchiAtom)
            atomsField.Add(atom)
        End Sub

        Public Overridable Sub addBond(bond As InchiBond)
            bondsField.Add(bond)
        End Sub

        Public Overridable Sub addStereo(stereo As InchiStereo)
            stereosField.Add(stereo)
        End Sub

        Public Overridable Function getAtom(i As Integer) As InchiAtom
            Return atomsField(i)
        End Function

        Public Overridable Function getBond(i As Integer) As InchiBond
            Return bondsField(i)
        End Function

        Public Overridable ReadOnly Property Atoms As IList(Of InchiAtom)
            Get
                Return New List(Of InchiAtom)(atomsField)
            End Get
        End Property

        Public Overridable ReadOnly Property Bonds As IList(Of InchiBond)
            Get
                Return New List(Of InchiBond)(bondsField)
            End Get
        End Property

        Public Overridable ReadOnly Property Stereos As IList(Of InchiStereo)
            Get
                Return New List(Of InchiStereo)(stereosField)
            End Get
        End Property

    End Class

End Namespace
