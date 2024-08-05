
''' JNA-InChI - Library for calling InChI from Java
''' Copyright © 2018 Daniel Lowe
''' 
''' This library is free software; you can redistribute it and/or
''' modify it under the terms of the GNU Lesser General Public
''' License as published by the Free Software Foundation; either
''' version 2.1 of the License, or (at your option) any later version.
''' 
''' This program is distributed in the hope that it will be useful,
''' but WITHOUT ANY WARRANTY; without even the implied warranty of
''' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
''' GNU Lesser General Public License for more details.
''' 
''' You should have received a copy of the GNU Lesser General Public License
''' along with this program.  If not, see </>.

Namespace IUPAC.InChI
    Public Class InchiAtom

        Private elNameField As String
        Private xField As Double = 0
        Private yField As Double = 0
        Private zField As Double = 0

        'array positions for hydrogen (i.e. isotope not specified), protium, deuterium, tritium
        Private implicitHydrogenField As Integer() = New Integer(3) {}
        Private isotopicMassField As Integer = 0
        Private radicalField As InchiRadical = InchiRadical.NONE
        Private chargeField As Integer = 0

        Public Sub New(elName As String)
            elNameField = elName
        End Sub

        Public Sub New(elName As String, x As Double, y As Double, z As Double)
            elNameField = elName
            xField = x
            yField = y
            zField = z
        End Sub

        Public Overridable Property ElName As String
            Get
                Return elNameField
            End Get
            Set(value As String)
                elNameField = value
            End Set
        End Property


        Public Overridable Property X As Double
            Get
                Return xField
            End Get
            Set(value As Double)
                xField = value
            End Set
        End Property


        Public Overridable Property Y As Double
            Get
                Return yField
            End Get
            Set(value As Double)
                yField = value
            End Set
        End Property


        Public Overridable Property Z As Double
            Get
                Return zField
            End Get
            Set(value As Double)
                zField = value
            End Set
        End Property


        Public Overridable Property ImplicitHydrogen As Integer
            Get
                Return implicitHydrogenField(0)
            End Get
            Set(value As Integer)
                If value > SByte.MaxValue OrElse value < -1 Then
                    Throw New ArgumentException("Unacceptable implicitHydrogen:" & value.ToString())
                End If
                implicitHydrogenField(0) = value
            End Set
        End Property


        Public Overridable Property ImplicitProtium As Integer
            Set(value As Integer)
                If value > SByte.MaxValue OrElse value < 0 Then
                    Throw New ArgumentException("Unacceptable implicitProtium:" & value.ToString())
                End If
                implicitHydrogenField(1) = value
            End Set
            Get
                Return implicitHydrogenField(1)
            End Get
        End Property


        Public Overridable Property ImplicitDeuterium As Integer
            Set(value As Integer)
                If value > SByte.MaxValue OrElse value < 0 Then
                    Throw New ArgumentException("Unacceptable implicitDeuterium:" & value.ToString())
                End If
                implicitHydrogenField(2) = value
            End Set
            Get
                Return implicitHydrogenField(2)
            End Get
        End Property


        Public Overridable Property ImplicitTritium As Integer
            Set(value As Integer)
                If value > SByte.MaxValue OrElse value < 0 Then
                    Throw New ArgumentException("Unacceptable implicitTritium:" & value.ToString())
                End If
                implicitHydrogenField(3) = value
            End Set
            Get
                Return implicitHydrogenField(3)
            End Get
        End Property


        Public Overridable Property IsotopicMass As Integer
            Get
                Return isotopicMassField
            End Get
            Set(value As Integer)
                If value > Short.MaxValue OrElse value < 0 Then
                    Throw New ArgumentException("Unacceptable isotopicMass:" & value.ToString())
                End If
                isotopicMassField = value
            End Set
        End Property


        Public Overridable Property Radical As InchiRadical
            Get
                Return radicalField
            End Get
            Set(value As InchiRadical)
                radicalField = value
            End Set
        End Property


        Public Overridable Property Charge As Integer
            Get
                Return chargeField
            End Get
            Set(value As Integer)
                If value > SByte.MaxValue OrElse value < SByte.MinValue Then
                    Throw New ArgumentException("Unacceptable charge:" & value.ToString())
                End If
                chargeField = value
            End Set
        End Property

    End Class

End Namespace
