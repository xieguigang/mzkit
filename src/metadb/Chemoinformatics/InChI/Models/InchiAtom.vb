#Region "Microsoft.VisualBasic::34ad6050122ac08336ea4f101284e04f, metadb\Chemoinformatics\InChI\Models\InchiAtom.vb"

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

'   Total Lines: 172
'    Code Lines: 127 (73.84%)
' Comment Lines: 16 (9.30%)
'    - Xml Docs: 75.00%
' 
'   Blank Lines: 29 (16.86%)
'     File Size: 5.63 KB


'     Class InchiAtom
' 
'         Properties: Charge, ElName, ImplicitDeuterium, ImplicitHydrogen, ImplicitProtium
'                     ImplicitTritium, IsotopicMass, Radical, X, Y
'                     Z
' 
'         Constructor: (+2 Overloads) Sub New
' 
' 
' /********************************************************************************/

#End Region


' JNA-InChI - Library for calling InChI from Java
' Copyright © 2018 Daniel Lowe
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public License
' along with this program.  If not, see </>.

Namespace IUPAC.InChI

    Public Class InchiAtom

        ' array positions for hydrogen (i.e. isotope not specified), protium, deuterium, tritium
        Private implicitHydrogenField As Integer() = New Integer(3) {}
        Private isotopicMassField As Integer = 0
        Private chargeField As Integer = 0

        Public Overridable Property ElName As String
        Public Overridable Property X As Double
        Public Overridable Property Y As Double
        Public Overridable Property Z As Double

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

        Public Overridable Property Radical As InchiRadical = InchiRadical.NONE

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

        Public Sub New(elName As String)
            _ElName = elName
        End Sub

        Public Sub New(elName As String, x As Double, y As Double, z As Double)
            _ElName = elName
            _X = x
            _Y = y
            _Z = z
        End Sub

        Public Shared Operator =(a As InchiAtom, b As InchiAtom) As Boolean
            If a Is Nothing AndAlso b Is Nothing Then
                Return True
            ElseIf a Is Nothing OrElse b Is Nothing Then
                Return False
            End If

            If a.ElName <> b.ElName Then Return False
            If a.X <> b.X Then Return False
            If a.Y <> b.Y Then Return False
            If a.Z <> b.Z Then Return False

            Return True
        End Operator

        Public Shared Operator <>(a As InchiAtom, b As InchiAtom) As Boolean
            Return Not (a = b)
        End Operator
    End Class

End Namespace

