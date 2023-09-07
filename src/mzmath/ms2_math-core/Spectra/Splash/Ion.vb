'
'  Ion.cs
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
    Public NotInheritable Class Ion
        Implements IComparable
        Private mzField As Double = 0.0
        Private intensityField As Double = 0.0
        Private Const PRECISION As Integer = 6

        Private mzFormat As String = String.Format("{{0,5:F{0}}}", PRECISION)
        Private intFormat As String = String.Format("{{0,5:F{0}}}", PRECISION)

        Public Property MZ As Double
            Get
                Return mzField
            End Get
            Set(value As Double)
                mzField = value
            End Set
        End Property

        Public Property Intensity As Double
            Get
                Return intensityField
            End Get
            Set(value As Double)
                intensityField = value
            End Set
        End Property

        Public Sub New(mz As Double, intensity As Double)
            mzField = mz
            intensityField = intensity
        End Sub

        'returning ion in mz:intensity format with 6 decimals
        Public Overrides Function ToString() As String
            Return String.Format("{0}:{1}", String.Format(mzFormat, mzField), String.Format(intFormat, intensityField))
        End Function

        'returning ion in mz:intensity format with 6 decimals in JSON format
        Public Function ToJSON() As String
            Dim json As StringBuilder = New StringBuilder()
            json.Append("{""mass"": ").AppendFormat(mzFormat, mzField).Append(", ""intensity"":").AppendFormat(intFormat, intensityField).Append("}")
            Return json.ToString()
        End Function

        'compares by mz value
        Public Function CompareTo(other As Object) As Integer Implements IComparable.CompareTo
            If [GetType]() IsNot other.GetType() Then
                Throw New ArgumentException(String.Format("Can't compare {0} with {1}.", [GetType](), other.GetType()))
            End If

            Dim otherCpy = CType(other, Ion)

            If intensityField < otherCpy.intensityField Then
                Return -1
            ElseIf intensityField > otherCpy.intensityField Then
                Return 1
            Else
                Return otherCpy.mzField.CompareTo(mzField)
            End If
        End Function
    End Class
End Namespace
