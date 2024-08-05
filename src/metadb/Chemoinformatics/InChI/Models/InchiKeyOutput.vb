
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
