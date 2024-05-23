#Region "Microsoft.VisualBasic::5b14d0fe7b504e4b26fe8f1d86f2f419, mzmath\ms2_math-core\Spectra\Splash\Splash.vb"

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

    '   Total Lines: 325
    '    Code Lines: 152 (46.77%)
    ' Comment Lines: 110 (33.85%)
    '    - Xml Docs: 50.00%
    ' 
    '   Blank Lines: 63 (19.38%)
    '     File Size: 13.22 KB


    '     Class Splash
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CalcSplashID, (+3 Overloads) filterSpectrum, formatIntensity, formatMZ, getFirstBlock
    '                   getHistoBlock, getSpectrumBlock, ToBase10, translateBase
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'
'  Splash.cs
'
'  Author:
'       Diego Pedrosa <dpedrosa@ucdavis.edu>
'
'  Copyright (c) 2015 Diego Pedrosa
'
'  This library is free software; you can redistribute it and/or modify
'  it under the terms of the GNU Lesser General Public License as
'  published by the Free Software Foundation; either version 2.1 of the
'  License, or (at your option) any later version.
'
'  This library is distributed in the hope that it will be useful, but
'  WITHOUT ANY WARRANTY; without even the implied warranty of
'  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
'  Lesser General Public License for more details.
'
'  You should have received a copy of the GNU Lesser General Public
'  License along with this library; if not, write to the Free Software
'  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Text
Imports std = System.Math

Namespace Spectra.SplashID

    Public Class Splash

        Private Const PREFIX As String = "splash"
        Private Const VERSION As Integer = 0

        Private Const PREFILTER_BASE As Integer = 3
        Private Const PREFILTER_LENGTH As Integer = 10
        Private Const PREFILTER_BIN_SIZE As Integer = 5

        Private Const SIMILARITY_BASE As Integer = 10
        Private Const SIMILARITY_LENGTH As Integer = 10
        Private Const SIMILARITY_BIN_SIZE As Integer = 100

        ''' <summary>
        ''' how to scale the spectrum
        ''' </summary>
        Public Shared ReadOnly scalingOfRelativeIntensity As Integer = 100

        'private static readonly char[] INTENSITY_MAP = new char[] {
        '	'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c',
        '	'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
        '	'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        '};

        Private Shared ReadOnly INTENSITY_MAP As String = "0123456789abcdefghijklmnopqrstuvwxyz"

        ''' <summary>
        ''' how should ions in the string representation be separeted
        ''' </summary>
        Private Shared ReadOnly ION_SEPERATOR As String = " "

        ''' <summary>
        ''' how many character should be in the spectrum block. Basically this reduces the SHA256 code down
        ''' to a fixed length of N characater
        ''' </summary>
        Private Shared ReadOnly maxCharactersForSpectrumBlockTruncation As Integer = 20

        ''' <summary>
        ''' Fixed precission of masses
        ''' </summary>
        Private Shared ReadOnly fixedPrecissionOfMasses As Integer = 6

        ''' <summary>
        ''' factor to scale m/z floating point values
        ''' </summary>
        Private Shared ReadOnly MZ_PRECISION_FACTOR As Long = std.Pow(10, fixedPrecissionOfMasses)

        ''' <summary>
        ''' Fixed precission of intensites
        ''' </summary>
        Private Shared ReadOnly fixedPrecissionOfIntensities As Integer = 0

        ''' <summary>
        ''' factor to scale m/z floating point values
        ''' </summary>
        Private Shared ReadOnly INTENSITY_PRECISION_FACTOR As Long = std.Pow(10, fixedPrecissionOfIntensities)

        ''' <summary>
        ''' Correction factor to avoid floating point issues between implementations
        ''' and processor architectures
        ''' </summary>
        Private Shared ReadOnly EPSILON As Double = 0.0000001

        Public Shared ReadOnly MSSplash As New Splash(SpectrumType.MS)

        ReadOnly spectrumType As SpectrumType

        Sub New(Optional type As SpectrumType = SpectrumType.MS)
            Me.spectrumType = type
        End Sub

        ''' <summary>
        ''' calculate the splash id of a given spectrum object from this function
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="spectrum"></param>
        ''' <returns></returns>
        Public Function CalcSplashID(Of T As {New, ISpectrum})(spectrum As T) As String
            Dim hash As New StringBuilder()

            'creating first block 'splash<type><version>'
            hash.Append(getFirstBlock(spectrumType))
            hash.Append("-"c)

            'create prefilter block
            Dim filteredSpec = filterSpectrum(spectrum, 10, 0.1)
            Call Debug.WriteLine("filtered spectrum: " & filteredSpec.ToString())
            Dim preFilterHistogram = getHistoBlock(filteredSpec, PREFILTER_BASE, PREFILTER_LENGTH, PREFILTER_BIN_SIZE)
            Debug.WriteLine("prefilter block: " & preFilterHistogram)
            Dim translated = translateBase(preFilterHistogram, PREFILTER_BASE, 36, 4)

            hash.Append(translated)
            hash.Append("-"c)

            'create similarity block
            hash.Append(getHistoBlock(spectrum, SIMILARITY_BASE, SIMILARITY_LENGTH, SIMILARITY_BIN_SIZE))
            hash.Append("-"c)

            'create the spetrum hash block
            hash.Append(getSpectrumBlock(spectrum))

            Return hash.ToString()
        End Function

        ''' <summary>
        ''' Generates the version block
        ''' </summary>
        ''' <param name="specType">type of spectrum beign splashed</param>
        ''' <returns>the version block as a string</returns>
        Private Function getFirstBlock(specType As SpectrumType) As String
            Dim s As String = (PREFIX & CInt(specType).ToString() & VERSION.ToString())
            Call Debug.WriteLine(String.Format("version block: {0}", s))
            Return s
        End Function

        ''' <summary>
        ''' calculates a histogram of the spectrum. If weighted, it sums the mz * intensities for the peaks in each bin
        ''' </summary>
        ''' <param name="spec">the spectrum data (in mz:int pairs)</param>
        ''' <returns>the histogram block for the given spectrum</returns>
        Private Function getHistoBlock(spec As ISpectrum, nbase As Integer, length As Integer, binSize As Integer) As String
            Dim binnedIons = New Double(length - 1) {}
            Dim maxIntensity As Double = 0

            ' initialize and populate bins
            For Each ion As ms2 In spec.GetIons
                Dim bin = CInt(ion.mz / binSize) Mod length
                binnedIons(bin) += ion.intensity

                If binnedIons(bin) > maxIntensity Then
                    maxIntensity = binnedIons(bin)
                End If
            Next

            If maxIntensity > 0 Then
                ' Normalize the histogram
                For i As Integer = 0 To length - 1
                    binnedIons(i) = (nbase - 1) * binnedIons(i) / maxIntensity
                Next
            End If

            ' build histogram
            Dim histogram As New StringBuilder()

            For Each bin As Double In Enumerable.ToList(binnedIons).GetRange(0, length)
                histogram.Append(INTENSITY_MAP.ElementAt(CInt(bin + EPSILON)))
            Next

            Call Debug.WriteLine(String.Format("{1} block: {0}", histogram.ToString(), If(length = 10, "histogram", "similarity")))
            Return histogram.ToString()
        End Function

        ''' <summary>
        ''' calculate the hash for the whole spectrum
        ''' </summary>
        ''' <param name="spec">the spectrum data (in mz:int pairs)</param>
        ''' <returns>the Hash of the spectrum data</returns>
        Private Function getSpectrumBlock(spec As ISpectrum) As String
            Dim ions As List(Of ms2) = spec.getSortedIonsByMZ()
            Dim s_ions As New StringBuilder()

            For Each i As ms2 In ions
                s_ions.Append(String.Format("{0}:{1}", formatMZ(i.mz), formatIntensity(i.intensity)))
                s_ions.Append(ION_SEPERATOR)
            Next

            If s_ions.Length > 0 Then
                'string to hash
                s_ions.Remove(s_ions.Length - 1, 1)
            End If

            Dim message As Byte() = Encoding.UTF8.GetBytes(s_ions.ToString())
            Dim hashString As New SHA256Managed()

            hashString.ComputeHash(message)

            Dim hash = BitConverter.ToString(hashString.Hash)
            hash = hash.Replace("-", "").Substring(0, maxCharactersForSpectrumBlockTruncation).ToLower()

            Debug.WriteLine(String.Format("hash block: {0}", hash))

            Return hash
        End Function


        ''' <summary>
        ''' Translate a number in string format from one numerical base to another
        ''' </summary>
        ''' <param name="number">number in string format</param>
        ''' <param name="initialBase">base in which the given number is represented</param>
        ''' <param name="finalBase">base to translate the number to, up to 36</param>
        ''' <param name="fill">minimum length of string</param>
        ''' <returns></returns>
        Private Function translateBase(number As String, initialBase As Integer, finalBase As Integer, fill As Integer) As String
            Dim n = ToBase10(number, initialBase)
            Dim result As New StringBuilder()

            While n > 0
                result.Insert(0, INTENSITY_MAP(n Mod finalBase))
                n /= finalBase
            End While

            While result.Length < fill
                result.Insert(0, "0"c)
            End While

            Call Debug.WriteLine("prefilter: " & result.ToString())
            Return result.ToString()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function formatMZ(number As Double) As String
            Return String.Format("{0}", CLng((number + EPSILON) * MZ_PRECISION_FACTOR))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function formatIntensity(number As Double) As String
            Return String.Format("{0}", CLng((number + EPSILON) * INTENSITY_PRECISION_FACTOR))
        End Function


        Public Shared Function ToBase10(number As String, start_base As Integer) As Integer
            Dim sum = 0
            Dim power = 0
            Debug.WriteLine("before base10: " & number)
            For Each c In number.Reverse()
                sum += CInt(INTENSITY_MAP.IndexOf(c) * std.Pow(start_base, power))
                power += 1
            Next
            Call Debug.WriteLine("after base10: " & sum.ToString())

            Return sum
        End Function

        ' *
        ' 		 * Filters spectrum by number of highest abundance ions and by base peak percentage
        ' 		 * @param s spectrum
        ' 		 * @param topIons number of top ions to retain
        ' 		 * @param basePeakPercentage percentage of base peak above which to retain
        ' 		 * @return filtered spectrum
        ' 
        Protected Function filterSpectrum(Of T As {New, ISpectrum})(s As T, topIons As Integer, basePeakPercentage As Double) As T
            Dim ions As List(Of ms2) = s.GetIons().AsList

            ' Find base peak intensity
            Dim basePeakIntensity = 0.0

            For Each ion In ions
                If ion.intensity > basePeakIntensity Then basePeakIntensity = ion.intensity
            Next

            ' Filter by base peak percentage if needed
            If basePeakPercentage >= 0 Then
                Dim filteredIons As New List(Of ms2)()

                For Each ion In ions
                    If ion.intensity + EPSILON >= basePeakPercentage * basePeakIntensity Then filteredIons.Add(New ms2(ion.mz, ion.intensity))
                Next

                ions = filteredIons
            End If

            ' Filter by top ions if necessary
            If topIons > 0 AndAlso ions.Count > topIons Then
                ions = ions.OrderByDescending(Function(i) i.intensity).ThenBy(Function(m) m.mz).ToList()

                ions = ions.GetRange(0, topIons)
            End If

            Dim spec As New T
            spec.SetIons(ions)
            Return spec
        End Function

        ' *
        ' 		 * Filters spectrum by number of highest abundance ions
        ' 		 * @param s spectrum
        ' 		 * @param topIons number of top ions to retain
        ' 		 * @return filtered spectrum
        ' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Function filterSpectrum(Of T As {New, ISpectrum})(s As T, topIons As Integer) As T
            Return filterSpectrum(s, topIons, -1)
        End Function

        ' *
        ' 		 * Filters spectrum by base peak percentage
        ' 		 * @param s spectrum
        ' 		 * @param basePeakPercentage percentage of base peak above which to retain
        ' 		 * @return filtered spectrum

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Function filterSpectrum(Of T As {New, ISpectrum})(s As T, basePeakPercentage As Double) As T
            Return filterSpectrum(s, -1, basePeakPercentage)
        End Function
    End Class
End Namespace
