#Region "Microsoft.VisualBasic::0186df8dfeb82618a709e04b7f55eb56, E:/mzkit/Rscript/Library/mzkit_app/src/mzkit//assembly/RamanSpectroscopy.vb"

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

    '   Total Lines: 68
    '    Code Lines: 20
    ' Comment Lines: 43
    '   Blank Lines: 5
    '     File Size: 3.94 KB


    ' Module RamanSpectroscopy
    ' 
    '     Function: readFile
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.Raman
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Raman spectroscopy (/ˈrɑːmən/) (named after physicist C. V. Raman) is a spectroscopic 
''' technique typically used to determine vibrational modes of molecules, although rotational
''' and other low-frequency modes of systems may also be observed.[1] Raman spectroscopy
''' is commonly used in chemistry to provide a structural fingerprint by which molecules 
''' can be identified.
'''
''' Raman spectroscopy relies upon inelastic scattering Of photons, known As Raman scattering.
''' A source Of monochromatic light, usually from a laser In the visible, near infrared, Or 
''' near ultraviolet range Is used, although X-rays can also be used. The laser light interacts 
''' With molecular vibrations, phonons Or other excitations In the system, resulting In the
''' energy Of the laser photons being shifted up Or down. The shift In energy gives information
''' about the vibrational modes In the system. Infrared spectroscopy typically yields similar 
''' yet complementary information.
''' 
''' Typically, a sample Is illuminated With a laser beam. Electromagnetic radiation from the 
''' illuminated spot Is collected With a lens And sent through a monochromator. Elastic 
''' scattered radiation at the wavelength corresponding To the laser line (Rayleigh scattering)
''' Is filtered out by either a notch filter, edge pass filter, Or a band pass filter, While 
''' the rest Of the collected light Is dispersed onto a detector.
''' 
''' Spontaneous Raman scattering Is typically very weak; As a result, For many years the main 
''' difficulty In collecting Raman spectra was separating the weak inelastically scattered 
''' light from the intense Rayleigh scattered laser light (referred To As "laser rejection"). 
''' Historically, Raman spectrometers used holographic gratings And multiple dispersion stages
''' To achieve a high degree Of laser rejection. In the past, photomultipliers were the detectors
''' Of choice For dispersive Raman setups, which resulted In Long acquisition times. However, 
''' modern instrumentation almost universally employs notch Or edge filters For laser rejection.
''' Dispersive Single-stage spectrographs (axial transmissive (AT) Or Czerny–Turner (CT) 
''' monochromators) paired With CCD detectors are most common although Fourier transform (FT) 
''' spectrometers are also common For use With NIR lasers.
''' 
''' The name "Raman spectroscopy" typically refers To vibrational Raman Using laser wavelengths 
''' which are Not absorbed by the sample. There are many other variations Of Raman spectroscopy 
''' including surface-enhanced Raman, resonance Raman, tip-enhanced Raman, polarized Raman, 
''' stimulated Raman, transmission Raman, spatially-offset Raman, And hyper Raman.
''' </summary>
<Package("RamanSpectroscopy")>
Public Module RamanSpectroscopy

    ''' <summary>
    ''' parse the Raman Spectroscopy data from a text file
    ''' </summary>
    ''' <param name="file">A file path of a ascii text file that contains the Raman Spectroscopy data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("readFile")>
    <RApiReturn(GetType(Spectroscopy))>
    Public Function readFile(<RRawVectorArgument> file As Object,
                             Optional env As Environment = Nothing) As Object

        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Return FileReader.ParseTextFile(New StreamReader(buf))
    End Function
End Module
