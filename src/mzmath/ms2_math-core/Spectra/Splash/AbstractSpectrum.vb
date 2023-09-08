'
'  AbstractSpectrum.cs
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

Imports System.Text

Namespace Spectra.SplashID

    Public MustInherit Class AbstractSpectrum
        Implements ISpectrum
        Private Const MAX_RELATIVE_INTENSITY As Integer = 100

#Region "fields and Properties"

        Public Property Type As SpectrumType
        Public Property Ions As New List(Of ms2)

#End Region

#Region "Constructors"
        ''' <summary>
        ''' Creates a new spectrum based on a string of ions
        ''' </summary>
        ''' <param name="data"></param>
        Public Sub New(data As String)
            'checke data has data 
            If Equals("", data) Then
                Throw New ArgumentException("The spectrum data can't be null or empty.")
            End If

            Ions = New List(Of ms2)()
            Dim splitData As Array = data.Split(" "c)

            For Each ion As String In splitData
                'get m/z
                Dim mz = Double.Parse(ion.Split(":"c)(0))
                'get intensity
                Dim intensity = Double.Parse(ion.Split(":"c)(1))

                Dim newIon As New ms2(mz, intensity)
                Ions.Add(newIon)
            Next

            Ions = toRelative(MAX_RELATIVE_INTENSITY)
        End Sub


        ''' <summary>
        ''' Creates a spectrum based on a list of ions
        ''' </summary>
        ''' <param name="ions"></param>
        Public Sub New(ions As List(Of ms2))
            If ions.Count <= 0 Then
                Throw New ArgumentException("The spectrum data can't be null or empty.")
            End If

            Me.Ions = ions
            Me.Ions = toRelative(MAX_RELATIVE_INTENSITY)
        End Sub
#End Region

        Public Overrides Function ToString() As String Implements ISpectrum.ToString
            Dim ionList As StringBuilder = New StringBuilder()
            For Each ion In Ions
                ionList.Append(ion)
                ionList.Append(" "c)
            Next

            If ionList.Length > 1 Then
                ionList.Remove(ionList.Length - 1, 1)
            End If

            Return String.Format("[Spectrum: Type={0}, Ions={1}]", Type, ionList.ToString())
        End Function

        'returns the current spectrum type
        Public Function getSpectrumType() As SpectrumType Implements ISpectrum.getSpectrumType
            Return Type
        End Function

        ' returns the current spectrum's list of ions sorted by descending intensities
        Public Function getSortedIonsByIntensity(Optional desc As Boolean = True) As List(Of ms2) Implements ISpectrum.getSortedIonsByIntensity
            Dim sorted As List(Of ms2) = Ions.OrderByDescending(Function(i) i.intensity).ThenBy(Function(m) m.mz).ToList()

            If Not desc Then
                sorted.Reverse()
            End If

            Return sorted
        End Function

        ' returns the current spectrum's list of ions sorted by ascending m/z ratio values
        Public Function getSortedIonsByMZ(Optional desc As Boolean = False) As List(Of ms2) Implements ISpectrum.getSortedIonsByMZ
            Dim sorted As List(Of ms2) = Ions.OrderBy(Function(i) i.mz).ThenByDescending(Function(i) i.intensity).ToList()

            If desc Then
                sorted.Reverse()
            End If

            Return sorted
        End Function

        ' calculate the relative spectrum in the range [0 .. 100]
        Private Function toRelative(scale As Integer) As List(Of ms2)
            Dim relativeIons As List(Of ms2) = New List(Of ms2)()
            relativeIons.AddRange(Ions)

            Dim maxInt = relativeIons.Max(Function(ion) ion.Intensity)
            relativeIons.ForEach(Sub(i) i.Intensity = i.Intensity / maxInt * scale)

            Return relativeIons
        End Function

        ' returns list of ions
        Public MustOverride Function GetIons() As List(Of ms2) Implements ISpectrum.GetIons
    End Class
End Namespace
