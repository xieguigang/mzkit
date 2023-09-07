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
Imports System
Imports System.Text
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Linq
Imports std = System.Math
Imports System.Diagnostics

Namespace Spectra.SplashID
    Public Class Splash
        Implements ISplash
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



        Public Function splashIt(spectrum As ISpectrum) As String Implements ISplash.splashIt

            ' check spectrum var
            If spectrum Is Nothing Then
                Throw New ArgumentNullException("The spectrum can't be null")
            End If

            Dim hash As StringBuilder = New StringBuilder()

            'creating first block 'splash<type><version>'
            hash.Append(getFirstBlock(spectrum.getSpectrumType()))
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
            Call Debug.WriteLine(String.Format("version block: {0}", PREFIX & CInt(specType).ToString() & VERSION.ToString()))
            Return (PREFIX & CInt(specType).ToString() & VERSION.ToString())
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
            For Each ion In CType(spec, MSSpectrum).Ions
                Dim bin = CInt(ion.MZ / binSize) Mod length
                binnedIons(bin) += ion.Intensity

                If binnedIons(bin) > maxIntensity Then
                    maxIntensity = binnedIons(bin)
                End If
            Next

            ' Normalize the histogram
            For i = 0 To length - 1
                binnedIons(i) = (nbase - 1) * binnedIons(i) / maxIntensity
            Next

            ' build histogram
            Dim histogram As StringBuilder = New StringBuilder()

            For Each bin In Enumerable.ToList(binnedIons).GetRange(0, length)
                histogram.Append(INTENSITY_MAP.ElementAt(bin + EPSILON))
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
            Dim ions As List(Of Ion) = spec.getSortedIonsByMZ()

            Dim strIons As StringBuilder = New StringBuilder()
            For Each i In ions
                strIons.Append(String.Format("{0}:{1}", formatMZ(i.MZ), formatIntensity(i.Intensity)))
                strIons.Append(ION_SEPERATOR)
            Next

            'string to hash
            strIons.Remove(strIons.Length - 1, 1)
            Dim message As Byte() = Encoding.UTF8.GetBytes(strIons.ToString())

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

            Dim result As StringBuilder = New StringBuilder()

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

        Private Function formatMZ(number As Double) As String
            Return String.Format("{0}", CLng((number + EPSILON) * MZ_PRECISION_FACTOR))
        End Function

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

        Protected Function filterSpectrum(s As ISpectrum, topIons As Integer, basePeakPercentage As Double) As ISpectrum
            Dim ions As List(Of Ion) = s.GetIons()

            ' Find base peak intensity
            Dim basePeakIntensity = 0.0

            For Each ion In ions
                If ion.Intensity > basePeakIntensity Then basePeakIntensity = ion.Intensity
            Next

            ' Filter by base peak percentage if needed
            If basePeakPercentage >= 0 Then
                Dim filteredIons As List(Of Ion) = New List(Of Ion)()

                For Each ion In ions
                    If ion.Intensity + EPSILON >= basePeakPercentage * basePeakIntensity Then filteredIons.Add(New Ion(ion.MZ, ion.Intensity))
                Next

                ions = filteredIons
            End If

            ' Filter by top ions if necessary
            If topIons > 0 AndAlso ions.Count > topIons Then
                ions = ions.OrderByDescending(Function(i) i.Intensity).ThenBy(Function(m) m.MZ).ToList()

                ions = ions.GetRange(0, topIons)
            End If

            Return New MSSpectrum(ions)
        End Function

        ' *
        ' 		 * Filters spectrum by number of highest abundance ions
        ' 		 * @param s spectrum
        ' 		 * @param topIons number of top ions to retain
        ' 		 * @return filtered spectrum
        ' 

        Protected Function filterSpectrum(s As ISpectrum, topIons As Integer) As ISpectrum
            Return filterSpectrum(s, topIons, -1)
        End Function

        ' *
        ' 		 * Filters spectrum by base peak percentage
        ' 		 * @param s spectrum
        ' 		 * @param basePeakPercentage percentage of base peak above which to retain
        ' 		 * @return filtered spectrum
        ' 

        Protected Function filterSpectrum(s As ISpectrum, basePeakPercentage As Double) As ISpectrum
            Return filterSpectrum(s, -1, basePeakPercentage)
        End Function
    End Class
End Namespace
