#Region "Microsoft.VisualBasic::2300e92fe01ae678a094450dde21ade2, ..\httpd\WebCloud\SMRUCC.WebCloud.d3js\Scatter.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class serialData

    Public Property name As String
    Public Property data As date_count()
    Public Property total As Long

End Class

Public Structure date_count
    Implements INamedValue

    Public Property [date] As String Implements INamedValue.Key
    Public Property count As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
