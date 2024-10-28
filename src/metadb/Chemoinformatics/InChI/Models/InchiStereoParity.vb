#Region "Microsoft.VisualBasic::95c76c3f115956f01dd71e01a9a6272c, metadb\Chemoinformatics\InChI\Models\InchiStereoParity.vb"

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

    '   Total Lines: 111
    '    Code Lines: 70 (63.06%)
    ' Comment Lines: 15 (13.51%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 26 (23.42%)
    '     File Size: 4.05 KB


    '     Class InchiStereoParity
    ' 
    ' 
    '         Enum InnerEnum
    ' 
    '             EVEN, NONE, ODD, UNDEFINED, UNKNOWN
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

    Public NotInheritable Class InchiStereoParity

        Public Const INCHI_PARITY_NONE = 0
        Public Const INCHI_PARITY_ODD = 1
        Public Const INCHI_PARITY_EVEN = 2
        Public Const INCHI_PARITY_UNKNOWN = 3
        Public Const INCHI_PARITY_UNDEFINED = 4


        Public Shared ReadOnly NONE As InchiStereoParity = New InchiStereoParity("NONE", InnerEnum.NONE, INCHI_PARITY_NONE)

        Public Shared ReadOnly ODD As InchiStereoParity = New InchiStereoParity("ODD", InnerEnum.ODD, INCHI_PARITY_ODD)

        Public Shared ReadOnly EVEN As InchiStereoParity = New InchiStereoParity("EVEN", InnerEnum.EVEN, INCHI_PARITY_EVEN)

        Public Shared ReadOnly UNKNOWN As InchiStereoParity = New InchiStereoParity("UNKNOWN", InnerEnum.UNKNOWN, INCHI_PARITY_UNKNOWN)

        Public Shared ReadOnly UNDEFINED As InchiStereoParity = New InchiStereoParity("UNDEFINED", InnerEnum.UNDEFINED, INCHI_PARITY_UNDEFINED)

        Private Shared ReadOnly valueList As IList(Of InchiStereoParity) = New List(Of InchiStereoParity)()

        Public Enum InnerEnum
            NONE
            ODD
            EVEN
            UNKNOWN
            UNDEFINED
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

        Private Shared ReadOnly map As IDictionary(Of SByte, InchiStereoParity) = New Dictionary(Of SByte, InchiStereoParity)()

        Shared Sub New()
            For Each val As InchiStereoParity In values()
                map.Add(val.codeField, val)
            Next

            valueList.Add(NONE)
            valueList.Add(ODD)
            valueList.Add(EVEN)
            valueList.Add(UNKNOWN)
            valueList.Add(UNDEFINED)
        End Sub

        Friend Shared Function [of](code As SByte) As InchiStereoParity
            Return map(code)
        End Function

        Public Shared Function values() As IList(Of InchiStereoParity)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As InchiStereoParity
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace

