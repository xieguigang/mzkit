
''' <summary>
''' Copyright (C) 2010  Pascal Fricke
''' Copyright (C) 2000-2010  Kirk Marat, The University of Manitoba
''' 
''' 
''' This file is part of nmr-fid library. 
''' nmr-fid library is free software: you can redistribute it and/or modify
''' it under the terms of the GNU General Public License as published by
''' the Free Software Foundation, either version 3 of the License, or
''' (at your option) any later version.
''' 
''' nmr-fid library is distributed in the hope that it will be useful,
''' but WITHOUT ANY WARRANTY; without even the implied warranty of
''' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
''' GNU General Public License for more details.
''' 
''' You should have received a copy of the GNU General Public License
''' along with cuteNMR.  If not, see </>.
''' 
''' This code is based upon the libraries released by Dr Kirk Marat from the University
''' of Manitoba and cuteNMR:
'''      ftp://davinci.chem.umanitoba.ca/pub/marat/SpinWorks/source_library/
'''      https://sourceforge.net/projects/cutenmr/
''' </summary>
Namespace fidMath.Apodization

    ''' <summary>
    ''' The ApodizationTool allows to apply various methods to improve the signal to noise value of an fid.
    ''' 
    ''' Copyright (C) 2010  Pascal Fricke
    ''' Copyright (C) 2000-2010  Kirk Marat, The University of Manitoba
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 14/01/2013
    ''' Time: 13:54
    ''' </summary>

    <Obsolete>
    Public Class ApodizationTool
        Private spectrum As Double()
        Private acquisitionMode As Acqu.AcquisitionMode = Acqu.AcquisitionMode.SEQUENTIAL
        Private processing As Proc

        ''' 
        ''' <param name="spectrum"> </param>
        ''' <param name="processing"> </param>
        Public Sub New(spectrum As Double(), processing As Proc)
            Me.spectrum = spectrum
            Me.processing = processing
        End Sub
        ' TODO perhaps change this to accept an Acqu object, to make aparent that the acquisition mode has to be defined
        Public Sub New(spectrum As Double(), acquisitionMode As Acqu.AcquisitionMode, processing As Proc)
            Me.spectrum = spectrum
            Me.processing = processing
            Me.acquisitionMode = acquisitionMode
        End Sub

        '    public double[] getSpectrum() {
        '        return spectrum;
        '    }

        ''' <summary>
        ''' performs the exponential apodization of the fid using the function F(x,i)= x.exp(-i*dw*lb)
        ''' where i*dw gives the time coordinate in (s) and the line broadening is in Hz.
        ''' The cuteNMR implementation is W(x,i) = x.exp(-(dw*i) * lb * pi)
        ''' @return
        ''' </summary>
        '    public double [] exponential(){
        '        double data[] = new double[processing.getTdEffective()];
        '        if (acquisitionMode.equals(Acqu.AcquisitionMode.SEQUENTIAL)) {
        '            for (int i =0 ; i< processing.getTdEffective() && i < spectrum.length; i++)
        '                data[i]*= Math.exp(-i*processing.getDwellTime()*Math.PI*processing.getLineBroadning());
        '''                data[i] = spectrum[i]* Math.exp(-i*processing.getDwellTime()*processing.getLineBroadning());
        '        } else { // simultaneous data
        '            for (int i =0 ; i< processing.getTdEffective()-1 && i < spectrum.length; i+=2){
        '                double factor = Math.exp(-i*processing.getDwellTime()*Math.PI*processing.getLineBroadning());
        '''                double factor = Math.exp(-i*processing.getDwellTime()*processing.getLineBroadning());
        '                data[i]= spectrum[i]* factor;
        '                data[i+1]=spectrum[i+1]* factor;
        '''                spectrum[i+1]*= Math.exp(-i*processing.getDwellTime()*Math.PI*processing.getLineBroadning());
        '            }
        '        }
        '        return data;
        '    }













        ''' <summary>
        ''' performs the guassian apodization according to Bramer 2001: F(x,i)=x*exp(-(i*dw*lb)^2)
        ''' where i*dw gives the time coordinate in (s) and the line broadening (lbGauss) in Hz.
        ''' This also correspondes to one of the alternative ways of defining the standard gaussian distribution,
        ''' according to wikipedia. </summary>
        ''' <param name="lbGauss"> </param>
        ''' <exceptioncref="Exception"> </exception>
        Public Overridable Function gaussianBramer2001(lbGauss As Double) As Double()
            Dim data = New Double(processing.TdEffective - 1) {}
            Dim time As Double
            If lbGauss = 0 Then
                Throw New Exception("line broadening cannot be zero")
            End If
            '        double sigmaFactor=0.5*Math.pow(1/lbGauss,2);

            If acquisitionMode.Equals(Acqu.AcquisitionMode.SEQUENTIAL) Then
                Dim i = 0

                While i < processing.TdEffective AndAlso i < spectrum.Length
                    time = i * processing.DwellTime
                    data(i) = spectrum(i) * Math.Exp(-Math.Pow(time * lbGauss, 2))
                    i += 1
                End While
            Else
                Dim i = 0

                While i < processing.TdEffective AndAlso i < spectrum.Length
                    time = i * processing.DwellTime
                    Dim factor = Math.Exp(-Math.Pow(time * lbGauss, 2))
                    data(i) = spectrum(i) * factor
                    data(i + 1) = spectrum(i + 1) * factor
                    i += 2
                End While
            End If
            Return data
        End Function

        Public Overridable Sub firstPoint()
            firstPoint(1)
        End Sub
        Public Overridable Sub firstPoint(fpCorrection As Double)
            spectrum(0) *= fpCorrection
            If Not acquisitionMode.Equals(Acqu.AcquisitionMode.SEQUENTIAL) Then
                spectrum(1) *= fpCorrection
            End If
        End Sub


    End Class

End Namespace
