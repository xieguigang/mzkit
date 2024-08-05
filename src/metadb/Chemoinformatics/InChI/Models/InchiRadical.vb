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

    Public NotInheritable Class InchiRadical

        Public Const INCHI_RADICAL_NONE = 0
        Public Const INCHI_RADICAL_SINGLET = 1
        Public Const INCHI_RADICAL_DOUBLET = 2
        Public Const INCHI_RADICAL_TRIPLET = 3


        Public Shared ReadOnly NONE As InchiRadical = New InchiRadical("NONE", InnerEnum.NONE, INCHI_RADICAL_NONE)

        Public Shared ReadOnly SINGLET As InchiRadical = New InchiRadical("SINGLET", InnerEnum.SINGLET, INCHI_RADICAL_SINGLET)

        Public Shared ReadOnly DOUBLET As InchiRadical = New InchiRadical("DOUBLET", InnerEnum.DOUBLET, INCHI_RADICAL_DOUBLET)

        Public Shared ReadOnly TRIPLET As InchiRadical = New InchiRadical("TRIPLET", InnerEnum.TRIPLET, INCHI_RADICAL_TRIPLET)

        Private Shared ReadOnly valueList As IList(Of InchiRadical) = New List(Of InchiRadical)()

        Public Enum InnerEnum
            NONE
            SINGLET
            DOUBLET
            TRIPLET
        End Enum

        Public ReadOnly innerEnumValue As InnerEnum
        Private ReadOnly nameValue As String
        Private ReadOnly ordinalValue As Integer
        Private Shared nextOrdinal As Integer = 0

        Private ReadOnly codeField As SByte

        Private Sub New(name As String, innerEnum As InnerEnum, code As Integer)
            codeField = CSByte(code)

            nameValue = name
            ordinalValue = Math.Min(Threading.Interlocked.Increment(nextOrdinal), nextOrdinal - 1)
            innerEnumValue = innerEnum
        End Sub

        Friend ReadOnly Property Code As SByte
            Get
                Return codeField
            End Get
        End Property

        Private Shared ReadOnly map As IDictionary(Of SByte, InchiRadical) = New Dictionary(Of SByte, InchiRadical)()

        Shared Sub New()
            For Each val As InchiRadical In values()
                map.Add(val.codeField, val)
            Next

            valueList.Add(NONE)
            valueList.Add(SINGLET)
            valueList.Add(DOUBLET)
            valueList.Add(TRIPLET)
        End Sub

        Friend Shared Function [of](code As SByte) As InchiRadical
            Return map(code)
        End Function


        Public Shared Function values() As IList(Of InchiRadical)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiRadical
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class
End Namespace
