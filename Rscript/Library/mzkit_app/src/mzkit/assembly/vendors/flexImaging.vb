#Region "Microsoft.VisualBasic::984c21cad6d18159178822af2d537c7d, Rscript\Library\mzkit_app\src\mzkit\assembly\vendors\flexImaging.vb"

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

    '   Total Lines: 62
    '    Code Lines: 50
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 2.42 KB


    ' Module flexImaging
    ' 
    '     Function: ImportsExperiment, ImportSpots, ReadMetadata
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader.SCiLSLab
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("flexImaging")>
Module flexImaging

    <ExportAPI("read.metadata")>
    Public Function ReadMetadata(mcf As String) As Object
        Dim data = Storage.GetMetaData(mcf).ToArray
        Dim meta As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        meta.add("metadata", data.Select(Function(a) a.Name))
        meta.add("data", data.Select(Function(a) a.Value))
        meta.add("type", data.Select(Function(a) a.Description.Split("|"c).First))
        meta.add("information", data.Select(Function(a) a.Description.GetTagValue("|").Value))

        Return meta
    End Function

    <ExportAPI("importSpotList")>
    Public Function ImportSpots(spots As String,
                                spectrum As String,
                                Optional scale As Boolean = True,
                                Optional env As Environment = Nothing) As mzPack

        Dim println = env.WriteLineHandler

        Using spotFile As Stream = spots.Open(FileMode.Open, doClear:=False, [readOnly]:=True),
            spectraFile As Stream = spectrum.Open(FileMode.Open, doClear:=False, [readOnly]:=True)

            Dim data As mzPack = LoadMSIFromSCiLSLab(spotFile, spectraFile, Sub(txt) println(txt))

            If scale Then
                data = data.ScalePixels
            End If

            Return data
        End Using
    End Function

    <ExportAPI("importsExperiment")>
    Public Function ImportsExperiment(scans As IEnumerable(Of String), Optional env As Environment = Nothing) As mzPack
        Dim tuplefiles = scans.CheckSpotFiles.ToArray
        Dim println = env.WriteLineHandler
        Dim verbose As Boolean = env.globalEnvironment.options.verbose
        Dim data As mzPack = MSIRawPack.LoadMSIFromSCiLSLab(
            files:=tuplefiles,
            println:=Sub(txt) println(txt),
            verbose:=verbose
        )

        Return data
    End Function
End Module
