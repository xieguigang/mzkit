

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

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
    Public Class InchiOutput

        Public Overridable ReadOnly Property Inchi As String
        Public Overridable ReadOnly Property AuxInfo As String
        Public Overridable ReadOnly Property Message As String
        Public Overridable ReadOnly Property Log As String
        Public Overridable ReadOnly Property Status As InchiStatus

        Sub New(inchi As String, auxInfo As String, message As String, log As String, status As InchiStatus)
            _Inchi = inchi
            _AuxInfo = auxInfo
            _Message = message
            _Log = log
            _Status = status
        End Sub

        Public Overrides Function ToString() As String
            Return Inchi
        End Function
    End Class

End Namespace
