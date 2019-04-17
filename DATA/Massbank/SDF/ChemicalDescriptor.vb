Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace File

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

        Shared ReadOnly schema As PropertyInfo() = DataFramework _
            .Schema(Of ChemicalDescriptor)(PropertyAccess.Readable, True, True) _
            .Values _
            .OrderBy(Function(p) p.Name) _
            .ToArray

        Sub New(data As Dictionary(Of String, String()))
            Dim read = getOne(data)

            On Error Resume Next

            ExactMass = Double.Parse(read("PUBCHEM_EXACT_MASS"))
            XLogP3 = Double.Parse(read("PUBCHEM_XLOGP3"))
            XLogP3_AA = Double.Parse(read("PUBCHEM_XLOGP3_AA"))
            FormalCharge = Integer.Parse(read("PUBCHEM_TOTAL_CHARGE"))
            TopologicalPolarSurfaceArea = Double.Parse(read("PUBCHEM_CACTVS_TPSA"))
            HydrogenAcceptor = Integer.Parse(read("PUBCHEM_CACTVS_HBOND_ACCEPTOR"))
            HydrogenDonors = Integer.Parse(read("PUBCHEM_CACTVS_HBOND_DONOR"))
            RotatableBonds = Integer.Parse(read("PUBCHEM_CACTVS_ROTATABLE_BOND"))
            HeavyAtoms = Integer.Parse(read("PUBCHEM_HEAVY_ATOM_COUNT"))
            Complexity = Integer.Parse(read("PUBCHEM_CACTVS_COMPLEXITY"))
        End Sub

        Sub New()
        End Sub

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
            Next

            Return descript
        End Function

        Private Shared Function getOne(data As Dictionary(Of String, String())) As Func(Of String, String)
            Return Function(key)
                       Return data.TryGetValue(key, [default]:={CStr(-10000000)}).FirstOrDefault
                   End Function
        End Function
    End Class
End Namespace