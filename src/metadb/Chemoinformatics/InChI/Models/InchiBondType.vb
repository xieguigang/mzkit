#Region "Microsoft.VisualBasic::803e6401cdf4d3e21936ffb500624f80, metadb\Chemoinformatics\InChI\Models\InchiBondType.vb"

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

    '   Total Lines: 110
    '    Code Lines: 69 (62.73%)
    ' Comment Lines: 17 (15.45%)
    '    - Xml Docs: 11.76%
    ' 
    '   Blank Lines: 24 (21.82%)
    '     File Size: 4.07 KB


    '     Class InchiBondType
    ' 
    ' 
    '         Enum InnerEnum
    ' 
    '             [DOUBLE], [SINGLE], ALTERN, NONE, TRIPLE
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

    Public NotInheritable Class InchiBondType

        Public Const INCHI_BOND_TYPE_NONE As Integer = 0
        Public Const INCHI_BOND_TYPE_SINGLE As Integer = 1
        Public Const INCHI_BOND_TYPE_DOUBLE As Integer = 2
        Public Const INCHI_BOND_TYPE_TRIPLE As Integer = 3
        Public Const INCHI_BOND_TYPE_ALTERN As Integer = 4

        Public Shared ReadOnly NONE As InchiBondType = New InchiBondType("NONE", InnerEnum.NONE, INCHI_BOND_TYPE_NONE)

        Public Shared ReadOnly [SINGLE] As InchiBondType = New InchiBondType("SINGLE", InnerEnum.SINGLE, INCHI_BOND_TYPE_SINGLE)

        Public Shared ReadOnly [DOUBLE] As InchiBondType = New InchiBondType("DOUBLE", InnerEnum.DOUBLE, INCHI_BOND_TYPE_DOUBLE)

        Public Shared ReadOnly TRIPLE As InchiBondType = New InchiBondType("TRIPLE", InnerEnum.TRIPLE, INCHI_BOND_TYPE_TRIPLE)

        ''' <summary>
        ''' avoid by all means </summary>
        Public Shared ReadOnly ALTERN As InchiBondType = New InchiBondType("ALTERN", InnerEnum.ALTERN, INCHI_BOND_TYPE_ALTERN)

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
            For Each val As InchiBondType In values()
                map.Add(val.codeField, val)
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
