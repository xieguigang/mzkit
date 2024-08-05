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

    Public NotInheritable Class InchiBondType

        Public Shared ReadOnly NONE As InchiBondType = New InchiBondType("NONE", InnerEnum.NONE, InChI.InchiLibrary.tagINCHIBondType_Fields.INCHI_BOND_TYPE_NONE)

        Public Shared ReadOnly [SINGLE] As InchiBondType = New InchiBondType("SINGLE", InnerEnum.SINGLE, InChI.InchiLibrary.tagINCHIBondType_Fields.INCHI_BOND_TYPE_SINGLE)

        Public Shared ReadOnly [DOUBLE] As InchiBondType = New InchiBondType("DOUBLE", InnerEnum.DOUBLE, InChI.InchiLibrary.tagINCHIBondType_Fields.INCHI_BOND_TYPE_DOUBLE)

        Public Shared ReadOnly TRIPLE As InchiBondType = New InchiBondType("TRIPLE", InnerEnum.TRIPLE, InChI.InchiLibrary.tagINCHIBondType_Fields.INCHI_BOND_TYPE_TRIPLE)

        ''' <summary>
        ''' avoid by all means </summary>
        Public Shared ReadOnly ALTERN As InchiBondType = New InchiBondType("ALTERN", InnerEnum.ALTERN, InChI.InchiLibrary.tagINCHIBondType_Fields.INCHI_BOND_TYPE_ALTERN)

        Private Shared ReadOnly valueList As IList(Of InchiBondType) = New List(Of InchiBondType)()

        Public Enum InnerEnum
            NONE
            [SINGLE]
            [DOUBLE]
            TRIPLE
            ALTERN
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

        Private Shared ReadOnly map As IDictionary(Of SByte, InchiBondType) = New Dictionary(Of SByte, InchiBondType)()

        Shared Sub New()
            For Each Val In values()
                map.Add(Val.codeField, Val)
            Next

            valueList.Add(NONE)
            valueList.Add([SINGLE])
            valueList.Add([DOUBLE])
            valueList.Add(TRIPLE)
            valueList.Add(ALTERN)
        End Sub

        Public Shared Function [of](code As SByte) As InchiBondType
            Return map(code)
        End Function

        Public Shared Function values() As IList(Of InchiBondType)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiBondType
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace
