#Region "Microsoft.VisualBasic::bdb6fbad43e7262f118836c2637c15c1, src\mzmath\ms2_simulator\test\simulatorTest.vb"

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

    ' Module simulatorTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph

Module simulatorTest

    Sub Main()
        Dim boundEnergies = BoundEnergy.GetEnergyTable
        Dim boundTable As New BoundEnergyFinder

        Dim molecule = "D:\MassSpectrum-toolkits\visual\KCF\DATA\NADPH.txt".LoadKCF.CreateGraph.FillBoundEnergy(boundTable)
        Dim energy As New EnergyModel(Function(x, y)
                                          Return x
                                      End Function, 0, 1000)
        Dim result As LibraryMatrix = molecule.MolecularFragment(energy)

        Using mgf As StreamWriter = "D:\MassSpectrum-toolkits\DATA\DATA\NADPH_insilicons.txt".OpenWriter
            Call result.MgfIon.WriteAsciiMgf(mgf)
        End Using

        Pause()
    End Sub
End Module
