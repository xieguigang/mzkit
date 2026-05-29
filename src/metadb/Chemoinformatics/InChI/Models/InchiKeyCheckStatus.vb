#Region "Microsoft.VisualBasic::7f6b56dd892f39a91ec2e5aa94422cbf, metadb\Chemoinformatics\InChI\Models\InchiKeyCheckStatus.vb"

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

    '   Total Lines: 101
    '    Code Lines: 65 (64.36%)
    ' Comment Lines: 15 (14.85%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (20.79%)
    '     File Size: 4.24 KB


    '     Class InchiKeyCheckStatus
    ' 
    ' 
    '         Enum InnerEnum
    ' 
    '             INVALID_LAYOUT, INVALID_LENGTH, INVALID_VERSION, VALID_NON_STANDARD, VALID_STANDARD
    ' 
    ' 
    ' 
    '  
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: [of], ordinal, ToString, valueOf, values
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

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

    Public NotInheritable Class InchiKeyCheckStatus

        Public Const INCHIKEY_VALID_STANDARD As Integer = 0
        Public Const INCHIKEY_VALID_NON_STANDARD As Integer = -1
        Public Const INCHIKEY_INVALID_LENGTH As Integer = 1
        Public Const INCHIKEY_INVALID_LAYOUT As Integer = 2
        Public Const INCHIKEY_INVALID_VERSION As Integer = 3

        Public Shared ReadOnly VALID_STANDARD As InchiKeyCheckStatus = New InchiKeyCheckStatus("VALID_STANDARD", InnerEnum.VALID_STANDARD, INCHIKEY_VALID_STANDARD)
        Public Shared ReadOnly VALID_NON_STANDARD As InchiKeyCheckStatus = New InchiKeyCheckStatus("VALID_NON_STANDARD", InnerEnum.VALID_NON_STANDARD, INCHIKEY_VALID_NON_STANDARD)
        Public Shared ReadOnly INVALID_LENGTH As InchiKeyCheckStatus = New InchiKeyCheckStatus("INVALID_LENGTH", InnerEnum.INVALID_LENGTH, INCHIKEY_INVALID_LENGTH)
        Public Shared ReadOnly INVALID_LAYOUT As InchiKeyCheckStatus = New InchiKeyCheckStatus("INVALID_LAYOUT", InnerEnum.INVALID_LAYOUT, INCHIKEY_INVALID_LAYOUT)
        Public Shared ReadOnly INVALID_VERSION As InchiKeyCheckStatus = New InchiKeyCheckStatus("INVALID_VERSION", InnerEnum.INVALID_VERSION, INCHIKEY_INVALID_VERSION)

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
            For Each val As InchiKeyCheckStatus In values()
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
