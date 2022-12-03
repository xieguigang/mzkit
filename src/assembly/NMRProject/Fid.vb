' 
'  Copyright (c) 2013. EMBL, European Bioinformatics Institute
' 
'  This program is free software: you can redistribute it and/or modify
'  it under the terms of the GNU Lesser General Public License as published by
'  the Free Software Foundation, either version 3 of the License, or
'  (at your option) any later version.
' 
'  This program is distributed in the hope that it will be useful,
'  but WITHOUT ANY WARRANTY; without even the implied warranty of
'  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'  GNU Lesser General Public License for more details.
' 
'  You should have received a copy of the GNU Lesser General Public License
'  along with this program.  If not, see <http://www.gnu.org/licenses/>.
' 

''' <summary>
''' Data structure for the fid
''' 
''' @author  Luis F. de Figueiredo
''' 
''' User: ldpf
''' Date: 14/01/2013
''' Time: 14:00
''' 
''' </summary>
Public Class Fid

    Private dataField As Double()
    Private realField As Double()
    Private imaginaryField As Double()

    Public Sub New(fid As IList(Of Integer?))
        Me.New(fid.Select(Function(i) CDbl(i)).ToArray())
    End Sub

    Public Sub New(fid As Double())
        dataField = fid
        splitData()

    End Sub

    Private Sub splitData()
        realField = New Double(dataField.Length / 2 - 1) {}
        imaginaryField = New Double(dataField.Length / 2 - 1) {}
        For i = 0 To dataField.Length - 1 Step 2
            realField(i / 2) = dataField(i) ' real are in even positions
            imaginaryField(i / 2) = dataField(i + 1) ' imaginary are in odd positions
        Next
    End Sub

    Public Sub New(fid As Integer())
        dataField = New Double(fid.Length - 1) {}
        For i = 0 To fid.Length - 1
            dataField(i) = fid(i)
        Next
        splitData()
    End Sub

    Public Sub New(fid As Single())
        dataField = New Double(fid.Length - 1) {}
        For i = 0 To fid.Length - 1
            dataField(i) = fid(i)
        Next
        splitData()
    End Sub

    ''' <summary>
    ''' the raw input data
    ''' </summary>
    ''' <returns></returns>
    Public Overridable ReadOnly Property Data As Double()
        Get
            Return dataField
        End Get
    End Property

    Public Overridable ReadOnly Property Real As Double()
        Get
            Return realField
        End Get
    End Property

    Public Overridable ReadOnly Property Imaginary As Double()
        Get
            Return imaginaryField
        End Get
    End Property
End Class
