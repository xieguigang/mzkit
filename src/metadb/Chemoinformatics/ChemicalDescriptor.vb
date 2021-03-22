#Region "Microsoft.VisualBasic::a65e27d3ce31fd3ad893ff18865450bc, src\metadb\Chemoinformatics\ChemicalDescriptor.vb"

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

    ' Class ChemicalDescriptor
    ' 
    '     Properties: AtomDefStereoCount, AtomUdefStereoCount, BondDefStereoCount, BondUdefStereoCount, Complexity
    '                 ComponentCount, ExactMass, FormalCharge, HeavyAtoms, HydrogenAcceptor
    '                 HydrogenDonors, IsotopicAtomCount, RotatableBonds, schema, TautoCount
    '                 TopologicalPolarSurfaceArea, XLogP3, XLogP3_AA
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: FromBytes, GetBytesBuffer, GetEnumerator, getOne, TryParseDouble
    '               TryParseInteger
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

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

    ''' <summary>
    ''' All of the property reflection info of <see cref="ChemicalDescriptor"/> object.
    ''' </summary>
    Public Shared ReadOnly Property schema As PropertyInfo() = DataFramework _
        .Schema(Of ChemicalDescriptor)(PropertyAccess.Readable, True, True) _
        .Values _
        .OrderBy(Function(p) p.Name) _
        .ToArray

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
