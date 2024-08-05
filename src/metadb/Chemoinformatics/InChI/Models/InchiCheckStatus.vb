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

    Public NotInheritable Class InchiCheckStatus

        Public Shared ReadOnly VALID_STANDARD As InchiCheckStatus = New InchiCheckStatus("VALID_STANDARD", InnerEnum.VALID_STANDARD, InChI.InchiLibrary.tagRetValCheckINCHI_Fields.INCHI_VALID_STANDARD)
        Public Shared ReadOnly VALID_NON_STANDARD As InchiCheckStatus = New InchiCheckStatus("VALID_NON_STANDARD", InnerEnum.VALID_NON_STANDARD, InChI.InchiLibrary.tagRetValCheckINCHI_Fields.INCHI_VALID_NON_STANDARD)
        Public Shared ReadOnly VALID_BETA As InchiCheckStatus = New InchiCheckStatus("VALID_BETA", InnerEnum.VALID_BETA, InChI.InchiLibrary.tagRetValCheckINCHI_Fields.INCHI_VALID_BETA)
        Public Shared ReadOnly INVALID_PREFIX As InchiCheckStatus = New InchiCheckStatus("INVALID_PREFIX", InnerEnum.INVALID_PREFIX, InChI.InchiLibrary.tagRetValCheckINCHI_Fields.INCHI_INVALID_PREFIX)
        Public Shared ReadOnly INVALID_VERSION As InchiCheckStatus = New InchiCheckStatus("INVALID_VERSION", InnerEnum.INVALID_VERSION, InChI.InchiLibrary.tagRetValCheckINCHI_Fields.INCHI_INVALID_VERSION)
        Public Shared ReadOnly INVALID_LAYOUT As InchiCheckStatus = New InchiCheckStatus("INVALID_LAYOUT", InnerEnum.INVALID_LAYOUT, InChI.InchiLibrary.tagRetValCheckINCHI_Fields.INCHI_INVALID_LAYOUT)
        Public Shared ReadOnly FAIL_I2I As InchiCheckStatus = New InchiCheckStatus("FAIL_I2I", InnerEnum.FAIL_I2I, InChI.InchiLibrary.tagRetValCheckINCHI_Fields.INCHI_FAIL_I2I)

        Private Shared ReadOnly valueList As IList(Of InchiCheckStatus) = New List(Of InchiCheckStatus)()

        Public Enum InnerEnum
            VALID_STANDARD
            VALID_NON_STANDARD
            VALID_BETA
            INVALID_PREFIX
            INVALID_VERSION
            INVALID_LAYOUT
            FAIL_I2I
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

        Private Shared ReadOnly map As IDictionary(Of Integer, InchiCheckStatus) = New Dictionary(Of Integer, InchiCheckStatus)()

        Shared Sub New()
            For Each Val In values()
                map.Add(Val.code, Val)
            Next

            valueList.Add(VALID_STANDARD)
            valueList.Add(VALID_NON_STANDARD)
            valueList.Add(VALID_BETA)
            valueList.Add(INVALID_PREFIX)
            valueList.Add(INVALID_VERSION)
            valueList.Add(INVALID_LAYOUT)
            valueList.Add(FAIL_I2I)
        End Sub

        Friend Shared Function [of](code As Integer) As InchiCheckStatus
            Return map(code)
        End Function


        Public Shared Function values() As IList(Of InchiCheckStatus)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiCheckStatus
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace
