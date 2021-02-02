#Region "Microsoft.VisualBasic::b08ab440f6205908ca96adc24270c810, Library\mzkit.insilicons\ms2_simulator.vb"

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

    ' Module ms2_simulator
    ' 
    '     Function: energyModel_custom, energyModel_normalDist, energyRange, loadKCF, MolecularFragmentation
    '               MolecularGraph, writeMgf
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Runtime

<Package("mzkit.simulator")>
Module ms2_simulator

    <ExportAPI("read.kcf")>
    Public Function loadKCF(file As String) As KCF
        Return file.LoadKCF(False)
    End Function

    <ExportAPI("molecular.graph")>
    Public Function MolecularGraph(mol As KCF, Optional verbose As Boolean = False) As NetworkGraph
        Dim g As NetworkGraph = mol _
           .CreateGraph _
           .FillBoundEnergy(New BoundEnergyFinder)

        If verbose Then
            Call Console.WriteLine($"energy range: {energyRange(g).ToString}")
        End If

        Return g
    End Function

    <ExportAPI("energy.range")>
    Public Function energyRange(mol As NetworkGraph) As DoubleRange
        Dim energies As Double() = mol.graphEdges _
            .Select(Function(e)
                        Return e.weight
                    End Function) _
            .ToArray

        Return energies
    End Function

    <ExportAPI("fragmentation")>
    Public Function MolecularFragmentation(mol As NetworkGraph, energy As EnergyModel,
                                           Optional nIntervals% = 1000,
                                           Optional precision% = 4) As LibraryMatrix
        Return mol.MolecularFragment(energy, nIntervals, precision, verbose:=False)
    End Function

    <ExportAPI("energy.normal")>
    Public Function energyModel_normalDist(mu#, delta#, Optional max# = 1000) As EnergyModel
        Return New EnergyModel(Function(x)
                                   Return pnorm.ProbabilityDensity(x, mu, delta)
                               End Function, 0, max)
    End Function

    <ExportAPI("energy.custom")>
    Public Function energyModel_custom(fun As DeclareLambdaFunction, max#, Optional env As Environment = Nothing) As EnergyModel
        Dim math As Func(Of Double, Double) = fun.CreateLambda(Of Double, Double)(env)
        Dim energy As New EnergyModel(math, 0, max)

        Return energy
    End Function

    <ExportAPI("write.mgf")>
    Public Function writeMgf(fragments As LibraryMatrix, file$) As Boolean
        Using mgf As StreamWriter = file.OpenWriter
            Call fragments.MgfIon.WriteAsciiMgf(mgf)
        End Using

        Return True
    End Function
End Module
