''' <summary>
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
''' </summary>
Namespace io.github.dan2097.jnainchi
    Public Class InchiInputFromAuxinfoOutput

        Private ReadOnly inchiInputField As InchiInput
        Private ReadOnly chiralFlagField As Boolean?
        Private ReadOnly messageField As String
        Private ReadOnly statusField As InchiStatus


        Friend Sub New(inchiInput As InchiInput, chiralFlag As Boolean?, message As String, status As InchiStatus)
            inchiInputField = inchiInput
            chiralFlagField = chiralFlag
            messageField = message
            statusField = status
        End Sub

        Public Overridable ReadOnly Property InchiInput As InchiInput
            Get
                Return inchiInputField
            End Get
        End Property

        ''' <summary>
        ''' True if the structure was marked as chiral
        ''' False if marked as not chiral
        ''' null if not marked
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property ChiralFlag As Boolean?
            Get
                Return chiralFlagField
            End Get
        End Property


        Public Overridable ReadOnly Property Message As String
            Get
                Return messageField
            End Get
        End Property


        Public Overridable ReadOnly Property Status As InchiStatus
            Get
                Return statusField
            End Get
        End Property

    End Class

End Namespace
