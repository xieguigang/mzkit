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


Namespace uk.ac.ebi.nmr.fid.tools.phasing

    ''' <summary>
    ''' Method to perform phase correction
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/05/2013
    ''' Time: 14:29
    ''' To change this template use File | Settings | File Templates.
    ''' </summary>
    Public Interface PhaseCorrector

        Function phaseCorrection(ByVal spectrum As Spectrum, ByVal zeroPhase As Double, ByVal firstOrderPhase As Double, ByVal pivot As Integer) As Spectrum

        Function phaseCorrection(ByVal spectrum As Spectrum, ByVal zeroPhase As Double, ByVal firstOrderPhase As Double) As Spectrum

    End Interface

End Namespace
