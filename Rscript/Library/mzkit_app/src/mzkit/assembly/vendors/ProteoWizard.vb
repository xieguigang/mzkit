#Region "Microsoft.VisualBasic::25679da18b46104dd28711820b4d2ebb, Rscript\Library\mzkit_app\src\mzkit\assembly\vendors\ProteoWizard.vb"

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

    '   Total Lines: 143
    '    Code Lines: 92 (64.34%)
    ' Comment Lines: 31 (21.68%)
    '    - Xml Docs: 93.55%
    ' 
    '   Blank Lines: 20 (13.99%)
    '     File Size: 5.11 KB


    ' Module ProteoWizard
    ' 
    '     Function: ConvertThermoRawFile, GetServices, msLevelFilter, Ready, scanTimeFilter
    '               wiffMRM
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports ProteoWizard.Interop
Imports ProteoWizard.Interop.filters
Imports SMRUCC.Rsharp.Development.Configuration
Imports SMRUCC.Rsharp.Runtime

''' <summary>
''' ProteoWizard helper
''' 
''' You should config the bin path of ProteoWizard at first 
''' by using ``options`` function
''' </summary>
<Package("ProteoWizard", Category:=APICategories.UtilityTools)>
Module ProteoWizard

    ''' <summary>
    ''' Is the ``ProteoWizard`` program ready to use?
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("msconvert.ready")>
    Public Function Ready(Optional env As Environment = Nothing) As Boolean
        VBDebugger.Mute = True

        If Not ProteoWizardCLI.IsAvaiable OrElse Not GetServices(env).IsAvailable Then
            VBDebugger.Mute = False
            Call env.AddMessage(ErrMsg, MSG_TYPES.WRN)
            Return False
        Else
            VBDebugger.Mute = False
            Return True
        End If
    End Function

    Private Function GetServices(env As Environment) As ProteoWizardCLI
        If Not ProteoWizardCLI.IsAvaiable Then
            Dim opts As Options = env.globalEnvironment.options
            Dim config As String = opts.getOption("ProteoWizard")

            Call ProteoWizardCLI.ConfigProgram(config)
        End If

        Return New ProteoWizardCLI
    End Function

    Const ErrMsg$ = "ProteoWizard un-available, you can config the program location by ``options(ProteoWizard='filepath')``!"

    ''' <summary>
    ''' Convert MRM wiff file to mzMl files
    ''' </summary>
    ''' <param name="wiff">The file path of the wiff file</param>
    ''' <returns>
    ''' File path collection of the converted mzML files.
    ''' </returns>
    <ExportAPI("MRM.mzML")>
    Public Function wiffMRM(wiff As String, Optional output$ = Nothing, Optional env As Environment = Nothing) As Object
        Dim bin As ProteoWizardCLI = GetServices(env)

        If Not bin.IsAvailable Then
            Return Internal.debug.stop(ErrMsg, env)
        Else
            If output.StringEmpty Then
                output = wiff.TrimSuffix
            End If

            bin.Convert2mzML(wiff, output, OutFileTypes.mzML)
        End If

        Return output _
            .EnumerateFiles("*.mzML") _
            .ToArray
    End Function

    <ExportAPI("filter.msLevel")>
    Public Function msLevelFilter(level As String) As Filter
        Return New msLevel(level)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="start">Start time in time unit of seconds</param>
    ''' <param name="stop">Stop time in time unit of seconds</param>
    ''' <returns></returns>
    <ExportAPI("filter.scanTime")>
    Public Function scanTimeFilter(start#, stop#) As Filter
        Return New scanTime(start, [stop])
    End Function

    ''' <summary>
    ''' Convert thermo raw files to mzXML files
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="output">The output directory</param>
    ''' <param name="filetype">the file type of the raw data output.</param>
    ''' <param name="filters">data filters</param>
    ''' <returns></returns>
    <ExportAPI("convert.thermo.raw")>
    Public Function ConvertThermoRawFile(raw As String(), output$,
                                         Optional filetype As OutFileTypes = OutFileTypes.mzXML,
                                         Optional filters As Filter() = Nothing,
                                         Optional parallel As Object = False,
                                         Optional env As Environment = Nothing) As Object

        Dim bin As ProteoWizardCLI = GetServices(env)
        Dim process As New PWConvertProcessor With {
            .filetype = filetype,
            .bin = bin,
            .filters = filters,
            .output = output
        }

        If Not bin.IsAvailable Then
            Return Internal.debug.stop(ErrMsg, env)
        ElseIf parallel > App.CPUCoreNumbers Then
            Call env.AddMessage($"the given parallelism degree is greater than the processor counts!", MSG_TYPES.WRN)
        End If

        Dim rawPipeline As IEnumerable(Of SeqValue(Of Boolean))

        If parallel > 1 Then
            rawPipeline = raw _
                .SeqIterator _
                .AsParallel _
                .Select(AddressOf process.runConvert)
        Else
            rawPipeline = raw _
                .SeqIterator _
                .Select(AddressOf process.runConvert)
        End If

        Dim result As Boolean() = rawPipeline _
            .OrderBy(Function(file) file.i) _
            .Select(Function(file)
                        Return file.value
                    End Function) _
            .ToArray

        Return result
    End Function
End Module
