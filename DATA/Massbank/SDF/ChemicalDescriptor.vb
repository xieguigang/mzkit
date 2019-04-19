#Region "Microsoft.VisualBasic::10bdddb78c39b6d11c2d33b370071329, Massbank\SDF\ChemicalDescriptor.vb"

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

    '     Class ChemicalDescriptor
    ' 
    '         Properties: Complexity, ExactMass, FormalCharge, HeavyAtoms, HydrogenAcceptor
    '                     HydrogenDonors, RotatableBonds, TopologicalPolarSurfaceArea, XLogP3, XLogP3_AA
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: FromBytes, GetBytesBuffer, GetEnumerator, getOne, IEnumerable_GetEnumerator
    '                   TryParseDouble, TryParseInteger
    ' 
    '     Class DescriptorDatabase
    ' 
    '         Properties: Length
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetDescriptor, GetEnumerator, IEnumerable_GetEnumerator
    ' 
    '         Sub: (+2 Overloads) Dispose, Flush, Write
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace File

    ''' <summary>
    ''' Chemical descriptor
    ''' </summary>
    Public Class ChemicalDescriptor : Implements IEnumerable(Of Double)

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

        ''' <summary>
        ''' All of the property reflection info of <see cref="ChemicalDescriptor"/> object.
        ''' </summary>
        Shared ReadOnly schema As PropertyInfo() = DataFramework _
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

        Public Iterator Function GetEnumerator() As IEnumerator(Of Double) Implements IEnumerable(Of Double).GetEnumerator
            For Each reader As PropertyInfo In schema
                Yield CDbl(reader.GetValue(Me))
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
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

    Public Class DescriptorDatabase : Implements IEnumerable(Of ChemicalDescriptor)
        Implements IDisposable

        ReadOnly repository As Stream
        ReadOnly blockSize As Integer

        Public ReadOnly Property Length As Long

        Sub New(stream As Stream)
            repository = stream

            If repository.Length > 8 Then
                repository.Seek(stream.Length - 8, SeekOrigin.Begin)
                Length = New BinaryReader(repository).ReadInt64
            End If

            blockSize = ChemicalDescriptor.GetBytesBuffer(New ChemicalDescriptor).Length
        End Sub

        Public Sub Flush()
            Call repository.Flush()
        End Sub

        Public Function GetDescriptor(cid As Long) As ChemicalDescriptor
            Dim offset = (cid - 1) * blockSize
            Dim buffer As Byte() = New Byte(blockSize - 1) {}

            Call repository.Seek(offset, SeekOrigin.Begin)
            Call repository.Read(buffer, 0, blockSize)

            Return ChemicalDescriptor.FromBytes(buffer)
        End Function

        Public Sub Write(cid&, descriptor As ChemicalDescriptor)
            Dim offset = (cid - 1) * blockSize
            Dim buffer = ChemicalDescriptor.GetBytesBuffer(descriptor)

            Call repository.Seek(offset, SeekOrigin.Begin)
            Call repository.Write(buffer, 0, blockSize)
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of ChemicalDescriptor) Implements IEnumerable(Of ChemicalDescriptor).GetEnumerator
            For i As Long = 1 To Length
                Yield GetDescriptor(cid:=i)
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call repository.Flush()
                    Call repository.Close()
                    Call repository.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
