#Region "Microsoft.VisualBasic::5693c8e799b7dcf836e4627b879dfcfc, metadb\Chemoinformatics\InChI\Models\InchiInputFromInchiOutput.vb"

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

'   Total Lines: 77
'    Code Lines: 41 (53.25%)
' Comment Lines: 25 (32.47%)
'    - Xml Docs: 84.00%
' 
'   Blank Lines: 11 (14.29%)
'     File Size: 2.69 KB


'     Class InchiInputFromInchiOutput
' 
'         Properties: InchiInput, Log, Message, Status, WarningFlags
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
    Public Class InchiInputFromInchiOutput

        Private ReadOnly messageField As String
        Private ReadOnly logField As String
        Private ReadOnly statusField As InchiStatus
        Private ReadOnly warningFlagsField As Long()()

        Friend Sub New(inchiInput As InchiInput, message As String, log As String, status As InchiStatus, warningFlags As Long()())
            _InchiInput = inchiInput
            messageField = message
            logField = log
            statusField = status
            warningFlagsField = warningFlags
        End Sub

        Public Overridable ReadOnly Property InchiInput As InchiInput

        Public Overridable ReadOnly Property Message As String
            Get
                Return messageField
            End Get
        End Property

        Public Overridable ReadOnly Property Log As String
            Get
                Return logField
            End Get
        End Property

        Public Overridable ReadOnly Property Status As InchiStatus
            Get
                Return statusField
            End Get
        End Property

        ''' <summary>
        ''' see INCHIDIFF in inchicmp.h.
        ''' 
        ''' [x][y]:
        ''' x=0 =&gt; Reconnected if present in InChI otherwise Disconnected/Normal
        ''' x=1 =&gt; Disconnected layer if Reconnected layer is present
        ''' y=1 =&gt; Main layer or Mobile-H
        ''' y=0 =&gt; Fixed-H layer
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property WarningFlags As Long()()
            Get
                Return warningFlagsField
            End Get
        End Property

    End Class

End Namespace

