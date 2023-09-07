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
        Protected typeField As SpectrumType
        Public Property Type As SpectrumType
            Get
                Return typeField
            End Get
            Friend Set(value As SpectrumType)
                typeField = value
            End Set
        End Property

        Protected ionsField As List(Of Ion) = New List(Of Ion)()
        Public Property Ions As List(Of Ion)
            Get
                Return ionsField
            End Get
            Friend Set(value As List(Of Ion))
                ionsField = value
            End Set
        End Property
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

            ionsField = New List(Of Ion)()
            Dim splitData As Array = data.Split(" "c)

            For Each ion As String In splitData
                'get m/z
                Dim mz = Double.Parse(ion.Split(":"c)(0))
                'get intensity
                Dim intensity = Double.Parse(ion.Split(":"c)(1))

                Dim newIon As Ion = New Ion(mz, intensity)
                Ions.Add(newIon)
            Next

            Ions = toRelative(MAX_RELATIVE_INTENSITY)
        End Sub


        ''' <summary>
        ''' Creates a spectrum based on a list of ions
        ''' </summary>
        ''' <param name="ions"></param>
        Public Sub New(ions As List(Of Ion))
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

        'returns a JSON representation of the spectrum
        Public Function toJSON() As String
            Dim ionList As StringBuilder = New StringBuilder()
            For Each ion In Ions
                ionList.Append(ion.ToJSON())
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
        Public Function getSortedIonsByIntensity(Optional desc As Boolean = True) As List(Of Ion) Implements ISpectrum.getSortedIonsByIntensity
            Dim sorted As List(Of Ion) = Ions.OrderByDescending(Function(i) i.Intensity).ThenBy(Function(m) m.MZ).ToList()

            If Not desc Then
                sorted.Reverse()
            End If

            Return sorted
        End Function

        ' returns the current spectrum's list of ions sorted by ascending m/z ratio values
        Public Function getSortedIonsByMZ(Optional desc As Boolean = False) As List(Of Ion) Implements ISpectrum.getSortedIonsByMZ
            Dim sorted As List(Of Ion) = Ions.OrderBy(Function(i) i.MZ).ThenByDescending(Function(i) i.Intensity).ToList()

            If desc Then
                sorted.Reverse()
            End If

            Return sorted
        End Function

        ' calculate the relative spectrum in the range [0 .. 100]
        Private Function toRelative(scale As Integer) As List(Of Ion)
            Dim relativeIons As List(Of Ion) = New List(Of Ion)()
            relativeIons.AddRange(Ions)

            Dim maxInt = relativeIons.Max(Function(ion) ion.Intensity)
            relativeIons.ForEach(Sub(i) i.Intensity = i.Intensity / maxInt * scale)

            Return relativeIons
        End Function

        ' returns list of ions
        Public MustOverride Function GetIons() As List(Of Ion) Implements ISpectrum.GetIons
    End Class
End Namespace
