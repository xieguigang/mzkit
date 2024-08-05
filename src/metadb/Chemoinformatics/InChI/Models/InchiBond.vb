
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
    Public Class InchiBond

        Private ReadOnly startField As InchiAtom
        Private ReadOnly endField As InchiAtom
        Private ReadOnly typeField As InchiBondType
        Private ReadOnly stereoField As InchiBondStereo

        Public Sub New(start As InchiAtom, [end] As InchiAtom, type As InchiBondType)
            Me.New(start, [end], type, InchiBondStereo.NONE)
        End Sub

        Public Sub New(start As InchiAtom, [end] As InchiAtom, type As InchiBondType, stereo As InchiBondStereo)
            If start.Equals([end]) Then
                Throw New ArgumentException("start and end must be different atoms")
            End If
            If type Is Nothing Then
                Throw New ArgumentException("type must not be null")
            End If
            If stereo Is Nothing Then
                Throw New ArgumentException("stereo must not be null, use InchiBondStereo.NONE")
            End If
            startField = start
            endField = [end]
            typeField = type
            stereoField = stereo
        End Sub

        Public Overridable ReadOnly Property Start As InchiAtom
            Get
                Return startField
            End Get
        End Property

        Public Overridable ReadOnly Property [End] As InchiAtom
            Get
                Return endField
            End Get
        End Property

        Public Overridable ReadOnly Property Type As InchiBondType
            Get
                Return typeField
            End Get
        End Property

        Public Overridable ReadOnly Property Stereo As InchiBondStereo
            Get
                Return stereoField
            End Get
        End Property

        Public Overridable Function getOther(atom As InchiAtom) As InchiAtom
            If startField Is atom Then
                Return endField
            End If
            If endField Is atom Then
                Return startField
            End If
            Return Nothing
        End Function


    End Class

End Namespace
