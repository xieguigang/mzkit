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

    Public NotInheritable Class InchiStereoType

        Public Const INCHI_StereoType_None = 0
        Public Const INCHI_StereoType_DoubleBond = 1
        Public Const INCHI_StereoType_Tetrahedral = 2
        Public Const INCHI_StereoType_Allene = 3


        Public Shared ReadOnly None As InchiStereoType = New InchiStereoType("None", InnerEnum.None, INCHI_StereoType_None)

        Public Shared ReadOnly DoubleBond As InchiStereoType = New InchiStereoType("DoubleBond", InnerEnum.DoubleBond, INCHI_StereoType_DoubleBond)

        Public Shared ReadOnly Tetrahedral As InchiStereoType = New InchiStereoType("Tetrahedral", InnerEnum.Tetrahedral, INCHI_StereoType_Tetrahedral)

        Public Shared ReadOnly Allene As InchiStereoType = New InchiStereoType("Allene", InnerEnum.Allene, INCHI_StereoType_Allene)

        Private Shared ReadOnly valueList As IList(Of InchiStereoType) = New List(Of InchiStereoType)()

        Public Enum InnerEnum
            None
            DoubleBond
            Tetrahedral
            Allene
        End Enum

        Public ReadOnly innerEnumValue As InnerEnum
        Private ReadOnly nameValue As String
        Private ReadOnly ordinalValue As Integer
        Private Shared nextOrdinal As Integer = 0

        Private Sub New(name As String, innerEnum As InnerEnum, code As Integer)
            _Code = CSByte(code)

            nameValue = name
            ordinalValue = Math.Min(Threading.Interlocked.Increment(nextOrdinal), nextOrdinal - 1)
            innerEnumValue = innerEnum
        End Sub

        Public ReadOnly Property Code As SByte

        Private Shared ReadOnly map As IDictionary(Of SByte, InchiStereoType) = New Dictionary(Of SByte, InchiStereoType)()

        Shared Sub New()
            For Each val As InchiStereoType In values()
                map.Add(val.Code, val)
            Next

            valueList.Add(None)
            valueList.Add(DoubleBond)
            valueList.Add(Tetrahedral)
            valueList.Add(Allene)
        End Sub

        Friend Shared Function [of](code As SByte) As InchiStereoType
            Return map(code)
        End Function

        Public Shared Function values() As IList(Of InchiStereoType)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiStereoType
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace
