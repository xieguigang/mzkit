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
        ''' <summary>
        ''' ML1: calibration constant
        ''' </summary>
        ''' <returns></returns>
        Public Property calibrationConstants As Object
        ''' <summary>
        ''' ML2: calibration constant
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>2 elements</remarks>
        Public Property calibrationConstants As Object()
        ''' <summary>
        ''' ML3: calibration constant
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>3 elements</remarks>
        Public Property calibrationConstants As Object()
        ''' <summary>
        ''' If we want To use High Precision Calibration (HPC), we need:
        '''  HPClBHi: upper mass threshold
        ''' </summary>
        ''' <returns></returns>
        Public Property hpcLimits_maxMass
        ''' <summary>
        ''' HPClBLo: lower mass threshold
        ''' </summary>
        ''' <returns></returns>
        Public Property hpcLimits_minMass
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
End Namespace