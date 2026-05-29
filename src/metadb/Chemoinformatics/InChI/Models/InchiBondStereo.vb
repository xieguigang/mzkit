#Region "Microsoft.VisualBasic::4b5e971d174f2726a40af24df26eb1c3, metadb\Chemoinformatics\InChI\Models\InchiBondStereo.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 140
    '    Code Lines: 81 (57.86%)
    ' Comment Lines: 31 (22.14%)
    '    - Xml Docs: 51.61%
    ' 
    '   Blank Lines: 28 (20.00%)
    '     File Size: 6.10 KB


    '     Class InchiBondStereo
    ' 
    ' 
    '         Enum InnerEnum
    ' 
    '             DOUBLE_EITHER, NONE, SINGLE_1DOWN, SINGLE_1EITHER, SINGLE_1UP
    '             SINGLE_2DOWN, SINGLE_2EITHER, SINGLE_2UP
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: Code
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: [of], ordinal, ToString, valueOf, values
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' JNA-InChI - Library for calling InChI from Java
' Copyright © 2018 Daniel Lowe
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public License
' along with this program.  If not, see </>.

Namespace IUPAC.InChI

    Public NotInheritable Class InchiBondStereo

        Public Const INCHI_BOND_STEREO_NONE As Integer = 0
        Public Const INCHI_BOND_STEREO_SINGLE_1UP As Integer = 1
        Public Const INCHI_BOND_STEREO_SINGLE_1EITHER As Integer = 4
        Public Const INCHI_BOND_STEREO_SINGLE_1DOWN As Integer = 6
        Public Const INCHI_BOND_STEREO_SINGLE_2UP As Integer = -1
        Public Const INCHI_BOND_STEREO_SINGLE_2EITHER As Integer = -4
        Public Const INCHI_BOND_STEREO_SINGLE_2DOWN As Integer = -6
        Public Const INCHI_BOND_STEREO_DOUBLE_EITHER As Integer = 3

        ''' <summary>
        ''' No stereo information recorded for this bond </summary>
        Public Shared ReadOnly NONE As InchiBondStereo = New InchiBondStereo("NONE", InnerEnum.NONE, INCHI_BOND_STEREO_NONE)

        ''' <summary>
        ''' sharp end points to this atom i.e. reference atom is <seealso cref="InchiBond.Start"/> </summary>
        Public Shared ReadOnly SINGLE_1UP As InchiBondStereo = New InchiBondStereo("SINGLE_1UP", InnerEnum.SINGLE_1UP, INCHI_BOND_STEREO_SINGLE_1UP)

        ''' <summary>
        ''' sharp end points to this atom i.e. reference atom is <seealso cref="InchiBond.Start"/> </summary>
        Public Shared ReadOnly SINGLE_1EITHER As InchiBondStereo = New InchiBondStereo("SINGLE_1EITHER", InnerEnum.SINGLE_1EITHER, INCHI_BOND_STEREO_SINGLE_1EITHER)

        ''' <summary>
        ''' sharp end points to this atom i.e. reference atom is <seealso cref="InchiBond.Start"/> </summary>
        Public Shared ReadOnly SINGLE_1DOWN As InchiBondStereo = New InchiBondStereo("SINGLE_1DOWN", InnerEnum.SINGLE_1DOWN, INCHI_BOND_STEREO_SINGLE_1DOWN)

        ''' <summary>
        ''' sharp end points to the opposite atom i.e. reference atom is <seealso cref="InchiBond.End"/> </summary>
        Public Shared ReadOnly SINGLE_2UP As InchiBondStereo = New InchiBondStereo("SINGLE_2UP", InnerEnum.SINGLE_2UP, INCHI_BOND_STEREO_SINGLE_2UP)

        ''' <summary>
        ''' sharp end points to the opposite atom i.e. reference atom is <seealso cref="InchiBond.End"/> </summary>
        Public Shared ReadOnly SINGLE_2EITHER As InchiBondStereo = New InchiBondStereo("SINGLE_2EITHER", InnerEnum.SINGLE_2EITHER, INCHI_BOND_STEREO_SINGLE_2EITHER)

        ''' <summary>
        ''' sharp end points to the opposite atom i.e. reference atom is <seealso cref="InchiBond.End"/> </summary>
        Public Shared ReadOnly SINGLE_2DOWN As InchiBondStereo = New InchiBondStereo("SINGLE_2DOWN", InnerEnum.SINGLE_2DOWN, INCHI_BOND_STEREO_SINGLE_2DOWN)

        ''' <summary>
        ''' unknown stereobond geometry </summary>
        Public Shared ReadOnly DOUBLE_EITHER As InchiBondStereo = New InchiBondStereo("DOUBLE_EITHER", InnerEnum.DOUBLE_EITHER, INCHI_BOND_STEREO_DOUBLE_EITHER)

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
            For Each val As InchiBondStereo In values()
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
