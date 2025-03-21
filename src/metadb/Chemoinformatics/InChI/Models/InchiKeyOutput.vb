﻿#Region "Microsoft.VisualBasic::3a783b4ec78e64143b7ab4ccbfc15088, metadb\Chemoinformatics\InChI\Models\InchiKeyOutput.vb"

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

    '   Total Lines: 56
    '    Code Lines: 24 (42.86%)
    ' Comment Lines: 23 (41.07%)
    '    - Xml Docs: 34.78%
    ' 
    '   Blank Lines: 9 (16.07%)
    '     File Size: 1.95 KB


    '     Class InchiKeyOutput
    ' 
    '         Properties: Block1HashExtension, Block2HashExtension, InchiKey, Status
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

    Public Class InchiKeyOutput

        Private ReadOnly szXtra1 As String
        Private ReadOnly szXtra2 As String

        Public Overridable ReadOnly Property InchiKey As String
        Public Overridable ReadOnly Property Status As InchiKeyStatus

        ''' <summary>
        ''' Returns the rest of the 256-bit SHA-2 signature for the first block
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property Block1HashExtension As String
            Get
                Return szXtra1
            End Get
        End Property

        ''' <summary>
        ''' Returns the rest of the 256-bit SHA-2 signature for the second block
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property Block2HashExtension As String
            Get
                Return szXtra2
            End Get
        End Property

        Friend Sub New(inchiKey As String, status As InchiKeyStatus, szXtra1 As String, szXtra2 As String)
            _InchiKey = inchiKey
            _Status = status
            Me.szXtra1 = szXtra1
            Me.szXtra2 = szXtra2
        End Sub

    End Class

End Namespace
