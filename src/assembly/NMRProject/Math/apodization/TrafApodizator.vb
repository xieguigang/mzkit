Imports System

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

Namespace fidMath.Apodization

    ''' <summary>
    ''' Applies a TRAF window function to the fid.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/04/2013
    ''' Time: 12:29
    ''' 
    ''' </summary>

    Public Class TrafApodizator
        Inherits AbstractApodizator

        Protected Friend Sub New(spectrum As Spectrum)
            MyBase.New(spectrum)
        End Sub

        Protected Friend Overloads Overrides Function calculateFactor(i As Integer, lbTraf As Double) As Double
            Dim acquisitionTime = spectrum.Proc.DwellTime * spectrum.Proc.TdEffective
            Dim time, e, eps As Double
            time = i * spectrum.Proc.DwellTime
            e = Math.Exp(-time * lbTraf * Math.PI)
            eps = Math.Exp((time - acquisitionTime) * lbTraf * Math.PI)
            Return e / (e * e + eps * eps)
        End Function

        Protected Friend Overloads Overrides Function calculateFactor(i As Integer) As Double
            ' this creates imutability issues
            '        spectrum.getProc().setLineBroadening(0.2);
            Return calculateFactor(i, 0.2)
        End Function

    End Class

End Namespace
