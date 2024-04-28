#Region "Microsoft.VisualBasic::78b350bcd176dfefee43c55f2a5b41dd, E:/mzkit/src/assembly/NMRFidTool//Math/apodization/LorentzGausApodizatior.vb"

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

    '   Total Lines: 98
    '    Code Lines: 25
    ' Comment Lines: 63
    '   Blank Lines: 10
    '     File Size: 4.57 KB


    '     Class LorentzGausApodizatior
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
    ''' Applies a Lorentz to Gauss window function to the fid.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/04/2013
    ''' Time: 12:25
    ''' 
    ''' </summary>

    Public Class LorentzGausApodizatior
        Inherits AbstractApodizator
        Protected Friend Sub New(spectrum As Spectrum)
            MyBase.New(spectrum)
        End Sub


        ''' <summary>
        ''' calculates the weigth for the Lorentz-Guassian apodization: W(i)= (lb*(i*dw)*pi(1-t/(2*tmax)))
        ''' where t=i*dw gives the time coordinate in (s), lb is the line broadening conversion from Lorents to Gaussian
        ''' in Hz, the gb is the GB-factor ??? from in the Proc file, and N*dw is the acquisition duration in s.
        ''' The cuteNMR implementation is W(i) = exp(-lb * pi * t (1-t/(gb*2*(N*dw)))
        ''' 
        '''  I think there is an error in the cuteNMR implementation because the exponential term has to be multiplied by
        '''  -1 or or at least have one of the constants negative (gb or lb)... otherwise the weighting function has higher
        '''  values in the region with more withe noise...
        ''' 
        ''' This weighting function seems to be called Gaussian resolution enhancement (GRE) function in Pearson (1987)
        ''' J. Mag. Res. 74: 541-545. (or perhpas this is the actual Gaussian weigthing function)
        ''' 
        ''' In Pearson 1987: W(i)=exp(lb*(i*dw)*pi(1-t/(2*tmax)))
        ''' where tmax = (2ln (2))/(pi*lb*ro^2) = GB / AQ. The ro is the relative linewidth [(lb_0+ lb_LB)/lb_0 -- lb_0 is
        ''' the linebroadening and lb_LB is the Lorentzian linebroadening], the AQ is the acquisition time N*dw, and in
        ''' Bruker spectrometers GB corresponds to the GB factor in the processing files.
        ''' </summary>
        ''' <param name="i"> </param>
        ''' <param name="lbLorentzToGauss">
        ''' @return </param>

        Protected Friend Overrides Function calculateFactor(i As Integer, lbLorentzToGauss As Double) As Double
            'TODO check if the equation to calculate the factor is correct
            If lbLorentzToGauss >= 0 Then
                Throw New Exception("the Lorentz to Gaussian linebroadening factor has to be negative")
            End If

            If spectrum.Proc.GbFactor <= 0 OrElse spectrum.Proc.GbFactor > 1 Then
                Throw New Exception("the GB-factor has to be in the interval ]0,1[")
            End If
            ' 
            ' 			cuteNMR implementation
            ' 			 
            Dim acquisitionTime = spectrum.Proc.DwellTime * spectrum.Proc.TdEffective
            Dim a = Math.PI * lbLorentzToGauss
            Dim b = -a / (2.0 * spectrum.Proc.GbFactor * acquisitionTime)
            Dim time = i * spectrum.Proc.DwellTime
            Return Math.Exp(-a * time - b * time * time)
            ' 
            ' 			Pearson GRE function
            ' 			 
            '        double a = Math.PI*lbLorentzToGauss;
            '        double tmax = processing.getTdEffective()*processing.getDwellTime()/processing.getGbFactor();
            '        return Math.exp(a*i*processing.getDwellTime()*
            '                (1-tmax*i*processing.getDwellTime()/2));
        End Function

        ''' <summary>
        ''' calculates the Lourentz-Gaussian weight with the default Lorentz to Gauss line broadening conversion.
        ''' </summary>
        ''' <param name="i">
        ''' @return </param>
        Protected Friend Overrides Function calculateFactor(i As Integer) As Double
            Return calculateFactor(i, -1)
        End Function
    End Class

End Namespace
