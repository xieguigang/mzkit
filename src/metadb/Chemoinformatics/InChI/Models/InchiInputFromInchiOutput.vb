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
Namespace IUPAC.InChI
    Public Class InchiInputFromInchiOutput

        Private ReadOnly inchiInputField As InchiInput
        Private ReadOnly messageField As String
        Private ReadOnly logField As String
        Private ReadOnly statusField As InchiStatus
        Private ReadOnly warningFlagsField As Long()()

        Friend Sub New(inchiInput As InchiInput, message As String, log As String, status As InchiStatus, warningFlags As Long()())
            inchiInputField = inchiInput
            messageField = message
            logField = log
            statusField = status
            warningFlagsField = warningFlags
        End Sub

        Public Overridable ReadOnly Property InchiInput As InchiInput
            Get
                Return inchiInputField
            End Get
        End Property

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
