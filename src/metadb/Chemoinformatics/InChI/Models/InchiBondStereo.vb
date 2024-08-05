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

    Public NotInheritable Class InchiBondStereo

        ''' <summary>
        ''' No stereo information recorded for this bond </summary>
        Public Shared ReadOnly NONE As InchiBondStereo = New InchiBondStereo("NONE", InnerEnum.NONE, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_NONE)

        ''' <summary>
        ''' sharp end points to this atom i.e. reference atom is <seealsocref="InchiBond.getStart()"/> </summary>
        Public Shared ReadOnly SINGLE_1UP As InchiBondStereo = New InchiBondStereo("SINGLE_1UP", InnerEnum.SINGLE_1UP, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_SINGLE_1UP)

        ''' <summary>
        ''' sharp end points to this atom i.e. reference atom is <seealsocref="InchiBond.getStart()"/> </summary>
        Public Shared ReadOnly SINGLE_1EITHER As InchiBondStereo = New InchiBondStereo("SINGLE_1EITHER", InnerEnum.SINGLE_1EITHER, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_SINGLE_1EITHER)

        ''' <summary>
        ''' sharp end points to this atom i.e. reference atom is <seealsocref="InchiBond.getStart()"/> </summary>
        Public Shared ReadOnly SINGLE_1DOWN As InchiBondStereo = New InchiBondStereo("SINGLE_1DOWN", InnerEnum.SINGLE_1DOWN, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_SINGLE_1DOWN)

        ''' <summary>
        ''' sharp end points to the opposite atom i.e. reference atom is <seealsocref="InchiBond.getEnd()"/> </summary>
        Public Shared ReadOnly SINGLE_2UP As InchiBondStereo = New InchiBondStereo("SINGLE_2UP", InnerEnum.SINGLE_2UP, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_SINGLE_2UP)

        ''' <summary>
        ''' sharp end points to the opposite atom i.e. reference atom is <seealsocref="InchiBond.getEnd()"/> </summary>
        Public Shared ReadOnly SINGLE_2EITHER As InchiBondStereo = New InchiBondStereo("SINGLE_2EITHER", InnerEnum.SINGLE_2EITHER, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_SINGLE_2EITHER)

        ''' <summary>
        ''' sharp end points to the opposite atom i.e. reference atom is <seealsocref="InchiBond.getEnd()"/> </summary>
        Public Shared ReadOnly SINGLE_2DOWN As InchiBondStereo = New InchiBondStereo("SINGLE_2DOWN", InnerEnum.SINGLE_2DOWN, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_SINGLE_2DOWN)

        ''' <summary>
        ''' unknown stereobond geometry </summary>
        Public Shared ReadOnly DOUBLE_EITHER As InchiBondStereo = New InchiBondStereo("DOUBLE_EITHER", InnerEnum.DOUBLE_EITHER, inchi.InchiLibrary.tagINCHIBondStereo2D_Fields.INCHI_BOND_STEREO_DOUBLE_EITHER)

        Private Shared ReadOnly valueList As IList(Of InchiBondStereo) = New List(Of InchiBondStereo)()

        Public Enum InnerEnum
            NONE
            SINGLE_1UP
            SINGLE_1EITHER
            SINGLE_1DOWN
            SINGLE_2UP
            SINGLE_2EITHER
            SINGLE_2DOWN
            DOUBLE_EITHER
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

        Private Shared ReadOnly map As IDictionary(Of SByte, InchiBondStereo) = New Dictionary(Of SByte, InchiBondStereo)()

        Shared Sub New()
            For Each val In values()
                map.Add(val.codeField, val)
            Next

            valueList.Add(NONE)
            valueList.Add(SINGLE_1UP)
            valueList.Add(SINGLE_1EITHER)
            valueList.Add(SINGLE_1DOWN)
            valueList.Add(SINGLE_2UP)
            valueList.Add(SINGLE_2EITHER)
            valueList.Add(SINGLE_2DOWN)
            valueList.Add(DOUBLE_EITHER)
        End Sub

        Friend Shared Function [of](code As SByte) As InchiBondStereo
            Return map(code)
        End Function


        Public Shared Function values() As IList(Of InchiBondStereo)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiBondStereo
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace
