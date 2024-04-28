#Region "Microsoft.VisualBasic::4599997c21495ae0c6237e013e48bccc, E:/mzkit/src/assembly/BrukerDataReader//XMass/FidReader.vb"

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

    '   Total Lines: 310
    '    Code Lines: 72
    ' Comment Lines: 223
    '   Blank Lines: 15
    '     File Size: 10.29 KB


    '     Class FidReader
    ' 
    '         Function: readAcquFile, readFidFile
    ' 
    '     Class AcquMetadata
    ' 
    '         Properties: [Date], acquisitionDate, acquisitionMethod, acquisitionMode, acquisitionOperatorMode
    '                     byteOrder, calibrationConstants, comments, dataSystem, deflection
    '                     deflectionPulserCal1, deflectionPulserMass, digitizerType, fidFile, flexControlVersion
    '                     hpcCoefficients, hpcLimits, hpcOrder, hpcUse, id
    '                     inlet, instrument, instrumentId, instrumentType, ionizationMode
    '                     laserAttenuation, laserRepetition, laserShots, massError, number
    '                     patch, path, sampleName, spectrometerType, spectrumType
    '                     spot, targetCount, targetIdString, targetSerialNumber, targetTypeNumber
    '                     timeDelay, timeDelta, tofMode
    ' 
    '     Class hpcLimits
    ' 
    '         Properties: maxMass, minMass
    ' 
    '     Class calibrationConstants
    ' 
    '         Properties: ML1, ML2, ML3
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO

Namespace XMass

    '## Copyright 2010-2012 Sebastian Gibb
    '## <mail@sebastiangibb.de>
    '##
    '## This file Is part Of readBrukerFlexData For R And related languages.
    '##
    '## readBrukerFlexData Is free software: you can redistribute it And/Or modify
    '## it under the terms Of the GNU General Public License As published by
    '## the Free Software Foundation, either version 3 Of the License, Or
    '## (at your Option) any later version.
    '##
    '## readBrukerFlexData Is distributed In the hope that it will be useful,
    '## but WITHOUT ANY WARRANTY; without even the implied warranty Of
    '## MERCHANTABILITY Or FITNESS For A PARTICULAR PURPOSE.  See the
    '## GNU General Public License For more details.
    '##
    '## You should have received a copy Of the GNU General Public License
    '## along With readBrukerFlexData. If Not, see <https://www.gnu.org/licenses/>

    Public Class FidReader

        ''' <summary>
        ''' Reads binary fid file.
        ''' 
        ''' This function reads a binary fid file. A fid file contains intensities for
        ''' all measured time points.
        ''' </summary>
        ''' <param name="fidFile">path to fid file e.g.
        '''  Pankreas_HB_L_061019_A10/0_a19/1/1SLin/fid</param>
        ''' <param name="nIntensities">number of data entries
        '''  (total count; get from acqu file)</param>
        ''' <param name="endian">endianness of the fid file
        '''  (\sQuote{little} or \sQuote{big}; default: \sQuote{little})</param>
        ''' <returns>A vector of intensity values.</returns>
        Public Function readFidFile(fidFile As String, nIntensities As Integer, Optional endian As ByteOrder = ByteOrder.LittleEndian) As Double()
            Using con As Stream = fidFile.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Dim bin As New BinaryDataReader(con, endian)
                Dim ints As UInteger() = bin.ReadUInt32s(nIntensities)
                Dim into As Double() = ints _
                    .Select(Function(i) CDbl(i)) _
                    .ToArray

                Return into
            End Using
        End Function

        ''' <summary>
        ''' Reads an acqu file.
        ''' 
        ''' This function reads constants, calibrations values and a lot of more from
        ''' \emph{acqu} files.
        ''' </summary>
        ''' <param name="proj">a xmass project object</param>
        ''' <param name="verbose">print verbose messages?</param>
        ''' <returns>acqu metadata</returns>
        Public Function readAcquFile(proj As Project, Optional verbose As Boolean = False) As AcquMetadata

        End Function

    End Class

    Public Class AcquMetadata
        ''' <summary>
        ''' We have To import the following data To calculating mass:
        '''  $BYTORDA: endianness, 0==little, 1==big
        ''' </summary>
        ''' <returns></returns>
        Public Property byteOrder As ByteOrder
        ''' <summary>
        ''' TD: total number Of measured time periods
        ''' </summary>
        ''' <returns></returns>
        Public Property number As Integer
        ''' <summary>
        ''' DELAY: first measured intensity after $DELAY ns
        ''' </summary>
        ''' <returns></returns>
        Public Property timeDelay As Double
        ''' <summary>
        ''' DW: ns between measured time periods
        ''' </summary>
        ''' <returns></returns>
        Public Property timeDelta As Double

        Public Property calibrationConstants As calibrationConstants

        ''' <summary>
        ''' If we want To use High Precision Calibration (HPC), we need:
        ''' </summary>
        ''' <returns></returns>
        Public Property hpcLimits As hpcLimits
        ''' <summary>
        ''' HPClOrd: polynomial order
        ''' </summary>
        ''' <returns></returns>
        Public Property hpcOrder
        ''' <summary>
        ''' HPClUse: maybe Using Of HPC? (seems always be "yes" In our test data)
        ''' </summary>
        ''' <returns></returns>
        Public Property hpcUse
        ''' <summary>
        ''' HPCStr: polynomial coefficients In a String
        ''' </summary>
        ''' <returns></returns>
        Public Property hpcCoefficients
        ''' <summary>
        ''' we Try To import [optional]:
        '''  DATATYPE
        '''  SPECTROMETER/DATASYSTEM
        ''' </summary>
        ''' <returns></returns>
        Public Property dataSystem
        ''' <summary>
        ''' SPECTROMETER TYPE
        ''' </summary>
        ''' <returns></returns>
        Public Property spectrometerType
        ''' <summary>
        ''' INLET
        ''' </summary>
        ''' <returns></returns>
        Public Property inlet
        ''' <summary>
        ''' IONIZATION MODE
        ''' </summary>
        ''' <returns></returns>
        Public Property ionizationMode
        ''' <summary>
        ''' DATE
        ''' </summary>
        ''' <returns></returns>
        Public Property [Date] As Date
        ''' <summary>
        ''' ACQMETH
        ''' </summary>
        ''' <returns></returns>
        Public Property acquisitionMethod
        ''' <summary>
        ''' AQ_DATE
        ''' </summary>
        ''' <returns></returns>
        Public Property acquisitionDate
        ''' <summary>
        ''' AQ_mod
        ''' </summary>
        ''' <returns></returns>
        Public Property acquisitionMode
        ''' <summary>
        ''' AQOP_mod
        ''' </summary>
        ''' <returns></returns>
        Public Property acquisitionOperatorMode
        ''' <summary>
        ''' (replaces file path based method)
        ''' </summary>
        ''' <returns></returns>
        Public Property tofMode As String
        ''' <summary>
        ''' ATTEN
        ''' </summary>
        ''' <returns></returns>
        Public Property laserAttenuation
        ''' <summary>
        ''' COM[1:4]
        ''' </summary>
        ''' <returns></returns>
        Public Property comments
        ''' <summary>
        ''' DEFLON
        ''' </summary>
        ''' <returns></returns>
        Public Property deflection
        ''' <summary>
        ''' DIGTYP
        ''' </summary>
        ''' <returns></returns>
        Public Property digitizerType
        ''' <summary>
        ''' DPCAL1
        ''' </summary>
        ''' <returns></returns>
        Public Property deflectionPulserCal1
        ''' <summary>
        ''' DPMASS
        ''' </summary>
        ''' <returns></returns>
        Public Property deflectionPulserMass
        ''' <summary>
        ''' FCVer
        ''' </summary>
        ''' <returns></returns>
        Public Property flexControlVersion
        ''' <summary>
        ''' ID_raw
        ''' </summary>
        ''' <returns></returns>
        Public Property id
        ''' <summary>
        ''' INSTRUM
        ''' </summary>
        ''' <returns></returns>
        Public Property instrument
        ''' <summary>
        ''' InstrID
        ''' </summary>
        ''' <returns></returns>
        Public Property instrumentId
        ''' <summary>
        ''' InstrTyp
        ''' </summary>
        ''' <returns></returns>
        Public Property instrumentType
        ''' <summary>
        ''' Masserr
        ''' </summary>
        ''' <returns></returns>
        Public Property massError
        ''' <summary>
        ''' NoSHOTS: number Of laser shots
        ''' </summary>
        ''' <returns></returns>
        Public Property laserShots
        ''' <summary>
        ''' SPType
        ''' </summary>
        ''' <returns></returns>
        Public Property spectrumType
        ''' <summary>
        ''' PATCHNO: sample position On target
        ''' </summary>
        ''' <returns></returns>
        Public Property patch
        ''' <summary>
        ''' PATH: original file path (On Bruker flex series controller PC)
        ''' </summary>
        ''' <returns></returns>
        Public Property path
        ''' <summary>
        ''' REPHZ
        ''' </summary>
        ''' <returns></returns>
        Public Property laserRepetition
        ''' <summary>
        ''' SPOTNO: same As $PATCHNO (In older files often empty, that's why we use
        '''      $PATCHNO instead)
        ''' </summary>
        ''' <returns></returns>
        Public Property spot
        ''' <summary>
        ''' TgIDS: target ids
        ''' </summary>
        ''' <returns></returns>
        Public Property targetIdString
        ''' <summary>
        ''' TgCount: number Of measurement With this target
        ''' </summary>
        ''' <returns></returns>
        Public Property targetCount
        ''' <summary>
        ''' TgSer: target serial number
        ''' </summary>
        ''' <returns></returns>
        Public Property targetSerialNumber
        ''' <summary>
        ''' TgTyp: target type number
        ''' </summary>
        ''' <returns></returns>
        Public Property targetTypeNumber
        ''' <summary>
        ''' import from file path:
        ''' full current path To fid file:
        ''' </summary>
        ''' <returns></returns>
        Public Property fidFile As String

        ''' <summary>
        ''' sample name
        ''' </summary>
        ''' <returns></returns>
        Public Property sampleName As String

    End Class

    Public Class hpcLimits
        ''' <summary>
        ''' HPClBHi: upper mass threshold
        ''' </summary>
        ''' <returns></returns>
        Public Property maxMass As Double
        ''' <summary>
        ''' HPClBLo: lower mass threshold
        ''' </summary>
        ''' <returns></returns>
        Public Property minMass As Double
    End Class

    ''' <summary>
    ''' calibration constant
    ''' </summary>
    Public Class calibrationConstants
        Public Property ML1
        Public Property ML2
        Public Property ML3
    End Class
End Namespace
