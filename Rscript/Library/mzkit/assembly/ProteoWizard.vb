#Region "Microsoft.VisualBasic::d0a4086654a46b55e7f3230258c61d9a, Rscript\Library\mzkit\ProteoWizard.vb"

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

    ' Module ProteoWizard
    ' 
    '     Function: ConvertThermoRawFile, GetServices, msLevelFilter, Ready, scanTimeFilter
    '               wiffMRM
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports ProteoWizard.Interop
Imports ProteoWizard.Interop.filters
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.System.Configuration

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
    Public Function msLevelFilter(level As String) As filter
        Return New msLevel(level)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="start">Start time in time unit of seconds</param>
    ''' <param name="stop">Stop time in time unit of seconds</param>
    ''' <returns></returns>
    <ExportAPI("filter.scanTime")>
    Public Function scanTimeFilter(start#, stop#) As filter
        Return New scanTime(start, [stop])
    End Function

    ''' <summary>
    ''' Convert thermo raw files to mzXML files
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="output">The output directory</param>
    ''' <param name="filetype"></param>
    ''' <param name="filters"></param>
    ''' <returns></returns>
    <ExportAPI("convert.thermo.raw")>
    Public Function ConvertThermoRawFile(raw As String(), output$,
                                         Optional filetype As OutFileTypes = OutFileTypes.mzXML,
                                         Optional filters As filter() = Nothing,
                                         Optional env As Environment = Nothing) As Object

        Dim result As New List(Of Object)
        Dim outputfile$
        Dim bin As ProteoWizardCLI = GetServices(env)

        If Not bin.IsAvailable Then
            Return Internal.debug.stop(ErrMsg, env)
        End If

        For Each file As String In raw
            outputfile = $"{output}/{file.FileName}"
            bin.Convert2mzML(file, output, filetype, filters)

            If outputfile.FileExists(ZERO_Nonexists:=True) Then
                result.Add(True)
            Else
                result.Add(file)
            End If
        Next

        Return result.ToArray
    End Function
End Module
