#Region "Microsoft.VisualBasic::d0651cd8a19fe8cc5354f989f0c8f7ea, src\assembly\ProteoWizard.Interop\ProteoWizardCLI.vb"

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

' Class ProteoWizardCLI
' 
'     Properties: BIN
'     Enum OutFileTypes
' 
' 
' 
' 
'  
' 
'     Constructor: (+2 Overloads) Sub New
' 
'     Function: Convert2mzML, convertThermoRawFile, convertWatersRawFile, IsAvaiable
' 
'     Sub: ConfigProgram
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports ProteoWizard.Interop.filters

''' <summary>
''' 
''' </summary>
''' <remarks>
''' Usage: msconvert [options] [filemasks]
''' Convert mass spec data file formats.
'''
''' Return value :  # of failed files.
'''
''' Options:
'''  -f [ --filelist ] arg            : specify text file containing filenames
'''  -o [ --outdir ] arg (=.)         : Set output directory ('-' for stdout) [.]
'''  -c [ --config ] arg              : configuration file(optionName = value)
'''  --outfile arg                    : Override the name Of output file.
'''  -e [ --ext ] arg                 : Set extension For output files [mzML|mzXML|mgf|txt|mz5]
'''  --mzML                           : write mzML format [default]
'''  --mzXML                          : write mzXML format
'''  --mz5                            : write mz5 format
'''  --mgf                            : write Mascot generic format
'''  --text                           : write ProteoWizard internal text format
'''  --ms1                            : write MS1 format
'''  --cms1                           : write CMS1 format
'''  --ms2                            : write MS2 format
'''  --cms2                           : write CMS2 format
'''  -v [ --verbose ]                 : display detailed progress information
'''  --64                             : Set Default binary encoding To 64-bit precision [default]
'''  --32                             : Set Default binary encoding To 32-bit precision
'''  --mz64                           : encode m/z values In 64-bit precision [default]
'''  --mz32                           : encode m/z values In 32-bit precision
'''  --inten64                        : encode intensity values In 64-bit precision
'''  --inten32                        : encode intensity values In 32-bit precision [default]
'''  --noindex                        : Do Not write index
'''  -i [ --contactInfo ] arg         : filename for contact info
'''  -z [ --zlib ]                    : use zlib compression For binary data
'''  --numpressLinear [=arg(=2e-009)] : use numpress linear prediction compression For binary mz And rt data (relative accuracy loss will Not exceed given tolerance arg, unless Set To 0)
'''  --numpressPic                    : use numpress positive Integer compression For binary intensities (absolute accuracy loss will Not exceed 0.5)
'''  --numpressSlof [=arg(=0.0002)]   : use numpress Short logged float compression For binary intensities (relative accuracy loss will Not exceed given tolerance arg, unless Set To 0)
'''  -n [ --numpressAll ]             : same as --numpressLinear --numpressSlof (see https://github.com/fickludd/ms-numpress for more info)
'''  -g [ --gzip ]                    : gzip entire output file (adds .gz To filename)
'''  --filter arg                     : add a spectrum list filter
'''  --merge                          : create a Single output file from multiple input files by merging file-level metadata And concatenating spectrum lists
'''  --simAsSpectra                   : write selected ion monitoring As spectra, Not chromatograms
'''  --srmAsSpectra                   : write selected reaction monitoring As spectra, Not chromatograms
'''  --combineIonMobilitySpectra      : write all drift bins/scans In a frame/block As one spectrum instead Of individual spectra
'''  --acceptZeroLengthSpectra        : some vendor readers have an efficient way Of filtering out empty spectra, but it takes more time To open the file
'''  --ignoreUnknownInstrumentError   : If True, If an instrument cannot be determined from a vendor file, it will Not be an Error
'''  --help                           : show this message, With extra detail On filter options
'''
''' FILTER OPTIONS
''' run this command With --help To see more detail
''' index &lt;index_value_set>
''' msLevel &lt;mslevels>
''' chargeState &lt;charge_states>
''' precursorRecalculation
''' mzRefiner input1.pepXML input2.mzid [msLevels=&lt;1->] [thresholdScore=&lt;CV_Score_Name>] [thresholdValue=&lt;floatset>] [thresholdStep=&lt;float>] [maxSteps=&lt;count>]
''' lockmassRefiner mz=&lt;real> mzNegIons=&lt;real (mz)> tol=&lt;real (1.0 Daltons)>
''' precursorRefine
''' peakPicking [&lt;PickerType> [snr=&lt;minimum signal-to-noise ratio>] [peakSpace=&lt;minimum peak spacing>] [msLevel=&lt;ms_levels>]]
''' scanNumber &lt;scan_numbers>
''' scanEvent &lt;scan_event_set>
''' scanTime &lt;scan_time_range>
''' sortByScanTime
''' stripIT
''' metadataFixer
''' titleMaker &lt;format_string>
''' threshold &lt;type>&lt;threshold>&lt;orientation> [&lt;mslevels>]
''' mzWindow &lt;mzrange>
''' mzPrecursors &lt;precursor_mz_list> [mzTol=&lt;mzTol (10 ppm)>] [mode=&lt;include|exclude (include)>]
''' defaultArrayLength &lt;peak_count_range>
''' zeroSamples &lt;mode> [&lt;MS_levels>]
''' mzPresent &lt;mz_list> [mzTol=&lt;tolerance> (0.5 mz)] [type=&lt;type> (count)] [threshold=&lt;threshold> (10000)] [orientation=&lt;orientation> (most-intense)] [mode=&lt;include|exclude (include)>]
''' scanSumming [precursorTol=&lt;precursor tolerance>] [scanTimeTol=&lt;scan time tolerance>]
''' thermoScanFilter &lt;exact|contains>&lt;include|exclude>&lt;match string>
''' MS2Denoise [&lt;peaks_in_window> [&lt;window_width_Da> [multicharge_fragment_relaxation]]]
''' MS2Deisotope [hi_res [mzTol=&lt;mzTol>]] [Poisson [minCharge=&lt;minCharge>] [maxCharge=&lt;maxCharge>]]
''' ETDFilter [&lt;removePrecursor> [&lt;removeChargeReduced> [&lt;removeNeutralLoss> [&lt;blanketRemoval> [&lt;matchingTolerance> ]]]]]
''' demultiplex massError=&lt;tolerance and units, eg 0.5Da (default 10ppm)> nnlsMaxIter=&lt;int (50)> nnlsEps=&lt;real (1e-10)> noWeighting=&lt;bool (false)> demuxBlockExtra=&lt;real (0)> variableFill=&lt;bool (false)> noSumNormalize=&lt;bool (false)> optimization=&lt;(none)|overlap_only
''' chargeStatePredictor [overrideExistingCharge=&lt;true|false (false)>] [maxMultipleCharge=&lt;int (3)>] [minMultipleCharge=&lt;int (2)>] [singleChargeFractionTIC=&lt;real (0.9)>] [maxKnownCharge=&lt;int (0)>] [makeMS2=&lt;true|false (false)>]
''' turbocharger [minCharge=&lt;minCharge>] [maxCharge=&lt;maxCharge>] [precursorsBefore=&lt;before>] [precursorsAfter=&lt;after>] [halfIsoWidth=&lt;half-width of isolation window>] [defaultMinCharge=&lt;defaultMinCharge>] [defaultMaxCharge=&lt;defaultMaxCharge>] [useVendorPeaks=&lt;useVe
'''                                                                                                                                                                                                                                                                   activation &lt;precursor_activation_type>
''' analyzer &lt;analyzer>
''' analyzerType &lt;analyzer>
''' polarity &lt;polarity>
'''
'''
''' Examples:
'''
''' # convert data.RAW to data.mzML
''' msconvert data.RAW
'''
''' # convert data.RAW to data.mzXML
''' msconvert data.RAW --mzXML
'''
''' # put output file in my_output_dir
''' msconvert data.RAW -o my_output_dir
'''
''' # combining options to create a smaller mzML file, much like the old ReAdW converter program
''' msconvert data.RAW --32 --zlib --filter "peakPicking true 1-" --filter "zeroSamples removeExtra"
'''
''' # extract scan indices 5...10 and 20...25
''' msconvert data.RAW --filter "index [5,10] [20,25]"
'''
''' # extract MS1 scans only
''' msconvert data.RAW --filter "msLevel 1"
'''
''' # extract MS2 and MS3 scans only
''' msconvert data.RAW --filter "msLevel 2-3"
'''
''' # extract MSn scans for n>1
''' msconvert data.RAW --filter "msLevel 2-"
'''
''' # apply ETD precursor mass filter
''' msconvert data.RAW --filter ETDFilter
'''
''' # remove non-flanking zero value samples
''' msconvert data.RAW --filter "zeroSamples removeExtra"
'''
''' # remove non-flanking zero value samples in MS2 and MS3 only
''' msconvert data.RAW --filter "zeroSamples removeExtra 2 3"
'''
''' # add missing zero value samples (with 5 flanking zeros) in MS2 and MS3 only
''' msconvert data.RAW --filter "zeroSamples addMissing=5 2 3"
'''
''' # keep only HCD spectra from a decision tree data file
''' msconvert data.RAW --filter "activation HCD"
'''
''' # keep the top 42 peaks or samples (depending on whether spectra are centroid or profile):
''' msconvert data.RAW --filter "threshold count 42 most-intense"
'''
''' # multiple filters: select scan numbers and recalculate precursors
''' msconvert data.RAW --filter "scanNumber [500,1000]" --filter "precursorRecalculation"
'''
''' # multiple filters: apply peak picking and then keep the bottom 100 peaks:
''' msconvert data.RAW --filter "peakPicking true 1-" --filter "threshold count 100 least-intense"
'''
''' # multiple filters: apply peak picking and then keep all peaks that are at least 50% of the intensity of the base peak:
''' msconvert data.RAW --filter "peakPicking true 1-" --filter "threshold bpi-relative .5 most-intense"
'''
''' # use a configuration file
''' msconvert data.RAW -c config.txt
'''
''' # example configuration file
''' mzXML=true
''' zlib=true
''' filter="index [3,7]"
''' filter="precursorRecalculation"
'''
'''
''' Questions, comments, and bug reports:
''' http://proteowizard.sourceforge.net
''' support@proteowizard.org
'''
''' ProteoWizard release: 3.0.10650 (2017-3-27)
''' ProteoWizard MSData: 3.0.10577 (2017-3-6)
''' ProteoWizard Analysis: 3.0.10650 (2017-3-27)
''' Build date: Mar 28 2017 00:21:42
''' </remarks>
Public Class ProteoWizardCLI : Inherits InteropService

    ''' <summary>
    ''' + msconvert
    ''' 
    ''' ProteoWizard命令行程序的位置
    ''' </summary>
    Public Shared ReadOnly Property BIN As String

    Shared Sub New()
        Call ProteoWizardCLI.ConfigProgram(bin:=App.GetVariable("bin"))
    End Sub

    Sub New(Optional bin$ = Nothing)
        If bin.StringEmpty Then
            Me._executableAssembly = ProteoWizardCLI.BIN
        Else
            Me._executableAssembly = bin
        End If
    End Sub

    Public Shared Function IsAvaiable() As Boolean
        Return BIN.FileExists(True)
    End Function

    Const WARN$ = "ProteoWizard is missing or invalid program file format, this web app will not working unless you put ProteoWizard to the location '{0}'!"

    Public Shared Sub ConfigProgram(bin As String)
        ProteoWizardCLI._BIN = bin

        ' debug echo
        If Not IsAvaiable() Then
            Call WARN.FormatString(bin).Warning
        Else
            Call $"msconvert={bin}".__INFO_ECHO
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="output"></param>
    ''' <param name="type"></param>
    ''' <param name="filters">
    ''' ``--filter arg``: add a spectrum list filter
    ''' </param>
    ''' <param name="verbose">
    ''' ``--verbose``: display detailed progress information
    ''' </param>
    ''' <returns></returns>
    Public Function Convert2mzML(input$, output$,
                                 Optional type As OutFileTypes = OutFileTypes.mzXML,
                                 Optional filters As IEnumerable(Of filter) = Nothing,
                                 Optional verbose As Boolean = True) As String

        Call output.ParentPath.MkDIR

        If Strings.LCase(input).EndsWith(".raw.zip") Then
            Return convertWatersRawFile(input, output, type, filters.SafeQuery.ToArray, verbose)
        Else
            Return convertRawFile(input, output, type, filters, verbose)
        End If
    End Function

    Private Function convertRawFile(input$, output$, type As OutFileTypes, filters As IEnumerable(Of filter), verbose As Boolean) As String
        Dim std$ = ""
        Dim args$ = cli(input, output, type, filters.SafeQuery.ToArray, verbose)

        Call input.__INFO_ECHO
        Call args.SetValue(args.TrimNewLine(" "))

        Dim proc = Me.RunProgram(args, )
        proc.Run()
        std = proc.StandardOutput

        Return std
    End Function

    Private Function cli(input$, output$,
                         type As OutFileTypes,
                         filters As filter(),
                         verbose As Boolean) As String

        Dim args As New ScriptBuilder(input.GetFullPath.CLIPath)

        args += " "
        args += "--mz64"
        args += type.Description
        args += "--zlib"

        If Not filters Is Nothing Then
            args += filter.GetFilters(filters)
        End If

        If verbose Then
            args += "--verbose"
        End If

        args += $"--ignoreUnknownInstrumentError"
        args += $"-o {output.GetDirectoryFullPath.CLIPath}"

        Return args.ToString
    End Function

    Private Function convertWatersRawFile(input$, output$, type As OutFileTypes, filters As filter(), verbose As Boolean) As String
        Dim std$ = ""

        For Each part In SplitDirectory(waters:=input)
            std = std & vbCrLf & convertRawFile(part.In, output, type, filters, verbose)

            ' cleanup filesystem for avoid file system crash
            Try
                Call FileSystem.DeleteDirectory(part.In.GetFullPath, DeleteDirectoryOption.DeleteAllContents)
            Catch ex As Exception

            End Try
        Next

        Return std
    End Function
End Class
