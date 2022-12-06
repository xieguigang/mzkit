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
''' Spectrum data structure.
''' 
''' @author  Luis F. de Figueiredo
''' 
''' User: ldpf
''' Date: 29/04/2013
''' Time: 12:22
''' 
''' </summary>
Public Class Spectrum


    Private acquField As Acqu
    Private procField As Proc
    Private fidField As Fid
    Private realChannelDataField As Double()
    Private imaginaryChannelDataField As Double()
    Private baselineModelField As Double()
    Private baselineField As Boolean()

    Public Sub New(fid As Double(), acqu As Acqu, proc As Proc)
        fidField = New Fid(fid)
        acquField = acqu
        procField = proc
    End Sub
    Public Sub New(fid As Double(), acqu As Acqu)
        Me.New(fid, acqu, New Proc(acqu))
    End Sub

    Sub New(fid As Fid)
        fidField = fid
        acquField = New Acqu(Acqu.Spectrometer.BRUKER)
        procField = New Proc(acquField)
    End Sub

    Public Overridable ReadOnly Property Proc As Proc
        Get
            Return procField
        End Get
    End Property

    Public Overridable ReadOnly Property Acqu As Acqu
        Get
            Return acquField
        End Get
    End Property

    Public Overridable ReadOnly Property Fid As Double()
        Get
            Return fidField.Data
        End Get
    End Property

    Public Overridable Sub setFid(i As Integer, value As Double)
        fidField.Data(i) = value
    End Sub

    Public Overridable Property RealChannelData As Double()
        Get
            Return realChannelDataField
        End Get
        Set(value As Double())
            realChannelDataField = value
        End Set
    End Property


    Public Overridable Sub setRealChannelData(i As Integer, value As Double)
        realChannelDataField(i) = value
    End Sub

    Public Overridable Property ImaginaryChannelData As Double()
        Get
            Return imaginaryChannelDataField
        End Get
        Set(value As Double())
            imaginaryChannelDataField = value
        End Set
    End Property

    Public Overridable Sub setImaginaryChannelData(i As Integer, value As Double)
        imaginaryChannelDataField(i) = value
    End Sub

    Public Overridable Property BaselineModel As Double()
        Get
            Return baselineModelField
        End Get
        Set(value As Double())
            baselineModelField = value
        End Set
    End Property


    Public Overridable Property Baseline As Boolean()
        Get
            Return baselineField
        End Get
        Set(value As Boolean())
            baselineField = value
        End Set
    End Property

End Class
