#Region "Microsoft.VisualBasic::8d3b23400f3485b3a333f660066debc6, E:/mzkit/src/assembly/NMRFidTool//Math/apodization/TrafApodizator.vb"

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

    '   Total Lines: 57
    '    Code Lines: 20
    ' Comment Lines: 28
    '   Blank Lines: 9
    '     File Size: 2.00 KB


    '     Class TrafApodizator
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) calculateFactor
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
