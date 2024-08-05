Imports System.Collections.Generic

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

    Public NotInheritable Class InchiKeyCheckStatus

        Public Shared ReadOnly VALID_STANDARD As InchiKeyCheckStatus = New InchiKeyCheckStatus("VALID_STANDARD", InnerEnum.VALID_STANDARD, inchi.InchiLibrary.tagRetValGetINCHIKey_Fields.INCHIKEY_VALID_STANDARD)
        Public Shared ReadOnly VALID_NON_STANDARD As InchiKeyCheckStatus = New InchiKeyCheckStatus("VALID_NON_STANDARD", InnerEnum.VALID_NON_STANDARD, inchi.InchiLibrary.tagRetValGetINCHIKey_Fields.INCHIKEY_VALID_NON_STANDARD)
        Public Shared ReadOnly INVALID_LENGTH As InchiKeyCheckStatus = New InchiKeyCheckStatus("INVALID_LENGTH", InnerEnum.INVALID_LENGTH, inchi.InchiLibrary.tagRetValGetINCHIKey_Fields.INCHIKEY_INVALID_LENGTH)
        Public Shared ReadOnly INVALID_LAYOUT As InchiKeyCheckStatus = New InchiKeyCheckStatus("INVALID_LAYOUT", InnerEnum.INVALID_LAYOUT, inchi.InchiLibrary.tagRetValGetINCHIKey_Fields.INCHIKEY_INVALID_LAYOUT)
        Public Shared ReadOnly INVALID_VERSION As InchiKeyCheckStatus = New InchiKeyCheckStatus("INVALID_VERSION", InnerEnum.INVALID_VERSION, inchi.InchiLibrary.tagRetValGetINCHIKey_Fields.INCHIKEY_INVALID_VERSION)

        Private Shared ReadOnly valueList As IList(Of InchiKeyCheckStatus) = New List(Of InchiKeyCheckStatus)()

        Public Enum InnerEnum
            VALID_STANDARD
            VALID_NON_STANDARD
            INVALID_LENGTH
            INVALID_LAYOUT
            INVALID_VERSION
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

        Private Shared ReadOnly map As IDictionary(Of Integer, InchiKeyCheckStatus) = New Dictionary(Of Integer, InchiKeyCheckStatus)()

        Shared Sub New()
            For Each val In values()
                map.Add(val.code, val)
            Next

            valueList.Add(VALID_STANDARD)
            valueList.Add(VALID_NON_STANDARD)
            valueList.Add(INVALID_LENGTH)
            valueList.Add(INVALID_LAYOUT)
            valueList.Add(INVALID_VERSION)
        End Sub

        Friend Shared Function [of](code As Integer) As InchiKeyCheckStatus
            Return map(code)
        End Function


        Public Shared Function values() As IList(Of InchiKeyCheckStatus)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiKeyCheckStatus
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace
