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

Namespace uk.ac.ebi.nmr.fid.tools

    ''' <summary>
    ''' @name    FastFourierTransform
    ''' @date    2013.01.31
    ''' @version $Rev$ : Last Changed $Date$
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' @author  pmoreno
    ''' @brief   interface for the FFT calss
    ''' 
    ''' </summary>
    Public Interface FastFourierTransform

        Function computeFFT() As Spectrum

        Function computeFFT(ByVal offset As Integer) As Spectrum

    End Interface

End Namespace
