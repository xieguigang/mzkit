Imports System.Collections.Generic

Imports InchiLibrary = io.github.dan2097.jnainchi.inchi.InchiLibrary

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

    Public NotInheritable Class InchiKeyStatus

        ''' <summary>
        ''' Success; no errors or warnings </summary>
        Public Shared ReadOnly OK As InchiKeyStatus = New InchiKeyStatus("OK", InnerEnum.OK, InchiLibrary.INCHIKEY_OK)

        ''' <summary>
        ''' Unknown program error </summary>
        Public Shared ReadOnly UNKNOWN_ERROR As InchiKeyStatus = New InchiKeyStatus("UNKNOWN_ERROR", InnerEnum.UNKNOWN_ERROR, InchiLibrary.INCHIKEY_UNKNOWN_ERROR)

        ''' <summary>
        ''' Source string is empty </summary>
        Public Shared ReadOnly EMPTY_INPUT As InchiKeyStatus = New InchiKeyStatus("EMPTY_INPUT", InnerEnum.EMPTY_INPUT, InchiLibrary.INCHIKEY_EMPTY_INPUT)

        ''' <summary>
        ''' Invalid InChI prefix or invalid version (not 1) </summary>
        Public Shared ReadOnly INVALID_INCHI_PREFIX As InchiKeyStatus = New InchiKeyStatus("INVALID_INCHI_PREFIX", InnerEnum.INVALID_INCHI_PREFIX, InchiLibrary.INCHIKEY_INVALID_INCHI_PREFIX)

        ''' <summary>
        ''' Not enough memory </summary>
        Public Shared ReadOnly NOT_ENOUGH_MEMORY As InchiKeyStatus = New InchiKeyStatus("NOT_ENOUGH_MEMORY", InnerEnum.NOT_ENOUGH_MEMORY, InchiLibrary.INCHIKEY_NOT_ENOUGH_MEMORY)

        ''' <summary>
        ''' Source InChI has invalid layout </summary>
        Public Shared ReadOnly INVALID_INCHI As InchiKeyStatus = New InchiKeyStatus("INVALID_INCHI", InnerEnum.INVALID_INCHI, InchiLibrary.INCHIKEY_INVALID_INCHI)

        ''' <summary>
        ''' Source standard InChI has invalid layout </summary>
        Public Shared ReadOnly INVALID_STD_INCHI As InchiKeyStatus = New InchiKeyStatus("INVALID_STD_INCHI", InnerEnum.INVALID_STD_INCHI, InchiLibrary.INCHIKEY_INVALID_STD_INCHI)

        Private Shared ReadOnly valueList As IList(Of InchiKeyStatus) = New List(Of InchiKeyStatus)()

        Public Enum InnerEnum
            OK
            UNKNOWN_ERROR
            EMPTY_INPUT
            INVALID_INCHI_PREFIX
            NOT_ENOUGH_MEMORY
            INVALID_INCHI
            INVALID_STD_INCHI
        End Enum

        Public ReadOnly innerEnumValue As InnerEnum
        Private ReadOnly nameValue As String
        Private ReadOnly ordinalValue As Integer
        Private Shared nextOrdinal As Integer = 0

        Private ReadOnly code As Integer

        Private Sub New(name As String, innerEnum As InnerEnum, code As Integer)
            Me.code = code

            nameValue = name
            ordinalValue = Math.Min(Threading.Interlocked.Increment(nextOrdinal), nextOrdinal - 1)
            innerEnumValue = innerEnum
        End Sub

        Private Shared ReadOnly map As IDictionary(Of Integer, InchiKeyStatus) = New Dictionary(Of Integer, InchiKeyStatus)()

        Shared Sub New()
            For Each val In values()
                map.Add(val.code, val)
            Next

            valueList.Add(OK)
            valueList.Add(UNKNOWN_ERROR)
            valueList.Add(EMPTY_INPUT)
            valueList.Add(INVALID_INCHI_PREFIX)
            valueList.Add(NOT_ENOUGH_MEMORY)
            valueList.Add(INVALID_INCHI)
            valueList.Add(INVALID_STD_INCHI)
        End Sub

        Friend Shared Function [of](code As Integer) As InchiKeyStatus
            Return map(code)
        End Function


        Public Shared Function values() As IList(Of InchiKeyStatus)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiKeyStatus
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace
