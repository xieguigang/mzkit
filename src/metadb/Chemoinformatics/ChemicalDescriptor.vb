#Region "Microsoft.VisualBasic::3e425b8f9d872744270c07f35e74e2de, metadb\Chemoinformatics\ChemicalDescriptor.vb"

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

    '   Total Lines: 217
    '    Code Lines: 148 (68.20%)
    ' Comment Lines: 32 (14.75%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 37 (17.05%)
    '     File Size: 7.36 KB


    ' Class Value
    ' 
    '     Properties: reference, value
    ' 
    ' Class UnitValue
    ' 
    '     Properties: condition, unit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' Class CCS
    ' 
    '     Properties: ion, reference, value
    ' 
    '     Function: ToString
    ' 
    ' Class ChemicalDescriptor
    ' 
    '     Properties: AtomDefStereoCount, AtomUdefStereoCount, BoilingPoint, BondDefStereoCount, BondUdefStereoCount
    '                 CCS, Color, Complexity, ComponentCount, CovalentlyBonded
    '                 Density, ExactMass, FlashPoint, FormalCharge, HeavyAtoms
    '                 HydrogenAcceptor, HydrogenDonors, IsotopicAtomCount, LogP, MeltingPoint
    '                 Odor, RotatableBonds, schema, Solubility, Taste
    '                 TautoCount, TopologicalPolarSurfaceArea, VaporPressure, XLogP3, XLogP3_AA
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: FromBytes, GetBytesBuffer, GetEnumerator, getOne, TryParseDouble
    '               TryParseInteger
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' value with reference source
''' </summary>
Public Class Value

    Public Property value As Double
    Public Property reference As String

End Class

''' <summary>
''' value with data unit tagged
''' </summary>
Public Class UnitValue : Inherits Value

    ''' <summary>
    ''' the data unit of the <see cref="value"/>.
    ''' </summary>
    ''' <returns></returns>
    Public Property unit As String
    Public Property condition As String

    Sub New()
    End Sub

    Public Overrides Function ToString() As String
        Return $"{value} {unit}"
    End Function

End Class

Public Class CCS

    Public Property value As String
    Public Property ion As String
    Public Property reference As String

    Public Overrides Function ToString() As String
        Return value
    End Function

End Class

''' <summary>
''' Chemical descriptor
''' </summary>
Public Class ChemicalDescriptor

    ''' <summary>
    ''' Computed Octanol/Water Partition Coefficient
    ''' </summary>
    ''' <returns></returns>
    Public Property XLogP3 As Double
    Public Property XLogP3_AA As Double
    ''' <summary>
    ''' Hydrogen Bond Donor Count
    ''' </summary>
    ''' <returns></returns>
    Public Property HydrogenDonors As Integer
    ''' <summary>
    ''' Hydrogen Bond Acceptor count
    ''' </summary>
    ''' <returns></returns>
    Public Property HydrogenAcceptor As Integer
    ''' <summary>
    ''' Rotatable Bond Count
    ''' </summary>
    ''' <returns></returns>
    Public Property RotatableBonds As Integer
    Public Property ExactMass As Double
    Public Property TopologicalPolarSurfaceArea As Double
    Public Property HeavyAtoms As Integer
    Public Property FormalCharge As Integer
    Public Property Complexity As Integer
    Public Property IsotopicAtomCount As Integer
    Public Property AtomDefStereoCount As Integer
    Public Property AtomUdefStereoCount As Integer
    Public Property BondDefStereoCount As Integer
    Public Property BondUdefStereoCount As Integer
    Public Property ComponentCount As Integer
    Public Property TautoCount As Integer
    Public Property CovalentlyBonded As Integer

#Region ""

    Public Property MeltingPoint As UnitValue()
    Public Property Solubility As UnitValue()
    Public Property BoilingPoint As UnitValue()
    Public Property FlashPoint As UnitValue()
    Public Property Density As UnitValue()
    Public Property VaporPressure As UnitValue()

    Public Property LogP As Value()
    Public Property CCS As CCS()
    Public Property Odor As UnitValue()
    Public Property Taste As UnitValue()
    Public Property Color As UnitValue()

#End Region

    ''' <summary>
    ''' All of the property reflection info of <see cref="ChemicalDescriptor"/> object.
    ''' </summary>
    Public Shared ReadOnly Property schema As PropertyInfo() = DataFramework _
        .Schema(Of ChemicalDescriptor)(PropertyAccess.Readable, True, True) _
        .Values _
        .OrderBy(Function(p) p.Name) _
        .ToArray

    Sub New(clone As ChemicalDescriptor)
        MeltingPoint = clone.MeltingPoint.SafeQuery.ToArray
        Solubility = clone.Solubility.SafeQuery.ToArray
        BoilingPoint = clone.BoilingPoint.SafeQuery.ToArray
        FlashPoint = clone.FlashPoint.SafeQuery.ToArray
        Density = clone.Density.SafeQuery.ToArray
        VaporPressure = clone.VaporPressure.SafeQuery.ToArray
        LogP = clone.LogP.SafeQuery.ToArray
        CCS = clone.CCS.SafeQuery.ToArray
        Odor = clone.Odor.SafeQuery.ToArray
        Taste = clone.Taste.SafeQuery.ToArray
        Color = clone.Color.SafeQuery.ToArray

        XLogP3 = clone.XLogP3
        XLogP3_AA = clone.XLogP3_AA
        HydrogenDonors = clone.HydrogenDonors
        HydrogenAcceptor = clone.HydrogenAcceptor
        RotatableBonds = clone.RotatableBonds
        ExactMass = clone.ExactMass
        TopologicalPolarSurfaceArea = clone.TopologicalPolarSurfaceArea
        HeavyAtoms = clone.HeavyAtoms
        FormalCharge = clone.FormalCharge
        Complexity = clone.Complexity
        IsotopicAtomCount = clone.IsotopicAtomCount
        AtomDefStereoCount = clone.AtomDefStereoCount
        AtomUdefStereoCount = clone.AtomUdefStereoCount
        BondDefStereoCount = clone.BondDefStereoCount
        BondUdefStereoCount = clone.BondUdefStereoCount
        ComponentCount = clone.ComponentCount
        TautoCount = clone.TautoCount
        CovalentlyBonded = clone.CovalentlyBonded
    End Sub

    Sub New(data As Dictionary(Of String, String()))
        Dim read = getOne(data)

        On Error Resume Next

        ExactMass = TryParseDouble(read("PUBCHEM_EXACT_MASS"))
        XLogP3 = TryParseDouble(read("PUBCHEM_XLOGP3"))
        XLogP3_AA = TryParseDouble(read("PUBCHEM_XLOGP3_AA"))
        FormalCharge = TryParseInteger(read("PUBCHEM_TOTAL_CHARGE"))
        TopologicalPolarSurfaceArea = TryParseDouble(read("PUBCHEM_CACTVS_TPSA"))
        HydrogenAcceptor = TryParseInteger(read("PUBCHEM_CACTVS_HBOND_ACCEPTOR"))
        HydrogenDonors = TryParseInteger(read("PUBCHEM_CACTVS_HBOND_DONOR"))
        RotatableBonds = TryParseInteger(read("PUBCHEM_CACTVS_ROTATABLE_BOND"))
        HeavyAtoms = TryParseInteger(read("PUBCHEM_HEAVY_ATOM_COUNT"))
        Complexity = TryParseInteger(read("PUBCHEM_CACTVS_COMPLEXITY"))

        IsotopicAtomCount = TryParseInteger(read("PUBCHEM_ISOTOPIC_ATOM_COUNT"))
        AtomDefStereoCount = TryParseInteger(read("PUBCHEM_ATOM_DEF_STEREO_COUNT"))
        AtomUdefStereoCount = TryParseInteger(read("PUBCHEM_ATOM_UDEF_STEREO_COUNT"))
        BondDefStereoCount = TryParseInteger(read("PUBCHEM_BOND_DEF_STEREO_COUNT"))
        BondUdefStereoCount = TryParseInteger(read("PUBCHEM_BOND_UDEF_STEREO_COUNT"))
        ComponentCount = TryParseInteger(read("PUBCHEM_COMPONENT_COUNT"))
        TautoCount = TryParseInteger(read("PUBCHEM_CACTVS_TAUTO_COUNT"))
    End Sub

    Sub New()
    End Sub

    Private Function TryParseDouble(s As String) As Double
        If PrimitiveParser.IsNumeric(s) Then
            Return Double.Parse(s)
        Else
            Return -10000000
        End If
    End Function

    Private Function TryParseInteger(s As String) As Integer
        If PrimitiveParser.IsInteger(s) Then
            Return Integer.Parse(s)
        Else
            Return -10000000
        End If
    End Function

    Public Shared Function GetBytesBuffer(descript As ChemicalDescriptor) As Byte()
        Dim bytes As New List(Of Byte)

        For Each data As PropertyInfo In schema
            Dim value = data.GetValue(descript)

            If TypeOf value Is Double Then
                bytes += BitConverter.GetBytes(DirectCast(value, Double))
            ElseIf TypeOf value Is Integer Then
                bytes += BitConverter.GetBytes(DirectCast(value, Integer))
            Else
                Throw New NotImplementedException
            End If
        Next

        Return bytes
    End Function

    Public Shared Function FromBytes(stream As Byte()) As ChemicalDescriptor
        Dim descript As New ChemicalDescriptor
        Dim i As Integer = 0
        Dim value As Object

        For Each data As PropertyInfo In schema
            Select Case data.PropertyType
                Case GetType(Double)
                    value = BitConverter.ToDouble(stream, i)
                    i += 8
                Case GetType(Integer)
                    value = BitConverter.ToInt32(stream, i)
                    i += 4
                Case Else
                    Throw New NotImplementedException
            End Select

            Call data.SetValue(descript, value)
        Next

        Return descript
    End Function

    Private Shared Function getOne(data As Dictionary(Of String, String())) As Func(Of String, String)
        Return Function(key)
                   Return data.TryGetValue(key, [default]:={CStr(-10000000)}).FirstOrDefault
               End Function
    End Function

    Public Iterator Function GetEnumerator() As IEnumerable(Of Double)
        For Each reader As PropertyInfo In schema
            Yield CDbl(reader.GetValue(Me))
        Next
    End Function

    Public Shared Narrowing Operator CType(descriptor As ChemicalDescriptor) As Dictionary(Of String, Double)
        With New Dictionary(Of String, Double)
            For Each data As PropertyInfo In schema
                Call .Add(data.Name, data.GetValue(descriptor))
            Next

            Return .ByRef
        End With
    End Operator
End Class
