#Region "Microsoft.VisualBasic::7b00deb3e02f7396f9931b509e84a68a, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\APIName.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace AppEngine

    Public Module APIName

        <Extension> Public Function GetAPIName(api As MethodInfo) As String
            Dim entry As ExportAPIAttribute =
                api.GetAttribute(Of ExportAPIAttribute)

            If entry Is Nothing Then
                Return ""
            Else
                Return entry.Name
            End If
        End Function

        Public Function GetAPIName(api As WebApp.IGET) As String
            Return api.Method.GetAPIName
        End Function

        Public Function GetAPIName(api As WebApp.IPOST) As String
            Return api.Method.GetAPIName
        End Function
    End Module
End Namespace
