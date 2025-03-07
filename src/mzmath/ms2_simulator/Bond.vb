#Region "Microsoft.VisualBasic::636398d4d89201739d95ced1bb4c454c, mzmath\ms2_simulator\Bond.vb"

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

    '   Total Lines: 76
    '    Code Lines: 55 (72.37%)
    ' Comment Lines: 4 (5.26%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 17 (22.37%)
    '     File Size: 2.96 KB


    ' Class Bond
    ' 
    '     Properties: BondLength, ForceConstant
    ' 
    '     Function: CalculateBondLength
    ' 
    ' Structure Angle
    ' 
    '     Function: CalculateBondAngle
    ' 
    ' Structure ForceFieldParameters
    ' 
    '     Function: CalculateEnergy
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Numerics
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports std = System.Math

Public Class Bond : Inherits Bound

    Public Property BondLength As Double
    Public Property ForceConstant As Double

    Public Shared Function CalculateBondLength(atom1 As Atom, atom2 As Atom) As Double
        Dim p1 As Point3D = atom1.Coordination
        Dim p2 As Point3D = atom2.Coordination
        Dim dx As Double = p1.X - p2.X
        Dim dy As Double = p1.Y - p2.Y
        Dim dz As Double = p1.Z - p2.Z

        Return std.Sqrt(dx * dx + dy * dy + dz * dz)
    End Function
End Class

Structure Angle

    Public Atom1 As Integer
    Public Atom2 As Integer
    Public Atom3 As Integer
    Public AngleValue As Double
    Public ForceConstant As Double

    Public Shared Function CalculateBondAngle(atom1 As Atom, atom2 As Atom, atom3 As Atom) As Double
        Dim p1 = atom1.Coordination
        Dim p2 = atom2.Coordination
        Dim p3 = atom3.Coordination
        Dim v1 As New Vector3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z)
        Dim v2 As New Vector3(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z)

        Dim dotProduct As Double = Vector3.Dot(v1, v2)
        Dim magnitudeV1 As Double = v1.Length()
        Dim magnitudeV2 As Double = v2.Length()

        Dim cosTheta As Double = dotProduct / (magnitudeV1 * magnitudeV2)
        ' Ensure the value is within the valid range for Acos
        cosTheta = std.Max(-1, std.Min(1, cosTheta))

        Return std.Acos(cosTheta) * (180.0 / std.PI) ' Convert to degrees
    End Function
End Structure

Structure ForceFieldParameters

    Public BondLengths As Dictionary(Of (String, String), (Double, Double))
    Public Angles As Dictionary(Of (String, String, String), (Double, Double))

    Shared Function CalculateEnergy(atoms As List(Of Atom), bonds As List(Of Bond), angles As List(Of Angle), forceField As ForceFieldParameters,
                                    temperature As Double,
                                    pressure As Double) As Double
        Dim energy As Double = 0.0

        ' Calculate bond energy
        For Each bond As Bond In bonds
            Dim actualLength As Double = Insilicon.Bond.CalculateBondLength(atoms(bond.i), atoms(bond.j))
            Dim delta As Double = actualLength - bond.BondLength
            energy += 0.5 * bond.ForceConstant * delta * delta
        Next

        ' Calculate angle energy
        For Each angle As Angle In angles
            Dim actualAngle As Double = Insilicon.Angle.CalculateBondAngle(atoms(angle.Atom1), atoms(angle.Atom2), atoms(angle.Atom3))
            Dim delta As Double = actualAngle - angle.AngleValue
            energy += 0.5 * angle.ForceConstant * delta * delta
        Next

        ' Here you would add additional terms for temperature and pressure if necessary

        Return energy
    End Function
End Structure
