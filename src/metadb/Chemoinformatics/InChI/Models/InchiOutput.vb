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
    Public Class InchiOutput

        Private ReadOnly inchiField As String
        Private ReadOnly auxInfoField As String
        Private ReadOnly messageField As String
        Private ReadOnly logField As String
        Private ReadOnly statusField As InchiStatus

        Friend Sub New(inchi As String, auxInfo As String, message As String, log As String, status As InchiStatus)
            inchiField = inchi
            auxInfoField = auxInfo
            messageField = message
            logField = log
            statusField = status
        End Sub

        Public Overridable ReadOnly Property Inchi As String
            Get
                Return inchiField
            End Get
        End Property

        Public Overridable ReadOnly Property AuxInfo As String
            Get
                Return auxInfoField
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

        Public Overrides Function ToString() As String
            Return inchiField
        End Function
    End Class

End Namespace
