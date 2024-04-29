#Region "Microsoft.VisualBasic::97f281f8fbd37bf97b81e7b3b8154d72, E:/mzkit/src/assembly/NMRFidTool//Fid.vb"

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

    '   Total Lines: 103
    '    Code Lines: 60
    ' Comment Lines: 30
    '   Blank Lines: 13
    '     File Size: 3.09 KB


    ' Class Fid
    ' 
    '     Properties: Data, Imaginary, Real
    ' 
    '     Constructor: (+4 Overloads) Sub New
    ' 
    '     Function: Create
    ' 
    '     Sub: splitData
    ' 
    ' /********************************************************************************/

#End Region

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

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.nmrML
Imports Microsoft.VisualBasic.Linq
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

    Public Shared Function Create(acquisition As acquisition) As Fid
        Dim cplx128 = acquisition.ParseMatrix()
        Dim fid As Double() = cplx128 _
            .Select(Function(c) {c.real, c.imaging}) _
            .IteratesALL _
            .ToArray
        Dim fidData As New Fid(fid)

        Return fidData
    End Function
End Class
