#Region "Microsoft.VisualBasic::8ac6e236f4fa54eb43aabac466e808b4, metadb\Chemoinformatics\InChI\Models\InchiInputFromAuxinfoOutput.vb"

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

'   Total Lines: 68
'    Code Lines: 34 (50.00%)
' Comment Lines: 21 (30.88%)
'    - Xml Docs: 85.71%
' 
'   Blank Lines: 13 (19.12%)
'     File Size: 2.25 KB


'     Class InchiInputFromAuxinfoOutput
' 
'         Properties: ChiralFlag, InchiInput, Message, Status
' 
'         Constructor: (+1 Overloads) Sub New
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

    Public Class InchiInputFromAuxinfoOutput


        Private ReadOnly chiralFlagField As Boolean?
        Private ReadOnly messageField As String
        Private ReadOnly statusField As InchiStatus


        Friend Sub New(inchiInput As InchiInput, chiralFlag As Boolean?, message As String, status As InchiStatus)
            _InchiInput = inchiInput
            chiralFlagField = chiralFlag
            messageField = message
            statusField = status
        End Sub

        Public Overridable ReadOnly Property InchiInput As InchiInput

        ''' <summary>
        ''' True if the structure was marked as chiral
        ''' False if marked as not chiral
        ''' null if not marked
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property ChiralFlag As Boolean?
            Get
                Return chiralFlagField
            End Get
        End Property


        Public Overridable ReadOnly Property Message As String
            Get
                Return messageField
            End Get
        End Property


        Public Overridable ReadOnly Property Status As InchiStatus
            Get
                Return statusField
            End Get
        End Property

    End Class

End Namespace

