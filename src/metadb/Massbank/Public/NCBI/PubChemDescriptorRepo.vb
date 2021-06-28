#Region "Microsoft.VisualBasic::298a037ae8b397b8e6f556f229ec7f6b, src\metadb\Massbank\Public\NCBI\PubChemDescriptorRepo.vb"

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

    ' Class PubChemDescriptorRepo
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetDescriptor
    ' 
    '     Sub: (+2 Overloads) Dispose, Write
    ' 
    ' Module BinaryHelper
    ' 
    '     Sub: (+2 Overloads) Write
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics

Public Class PubChemDescriptorRepo : Implements IDisposable

    ReadOnly stream As New Dictionary(Of String, FileStream)
    ReadOnly base$

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="dir">
    ''' The database file is consist with multiple db files:
    ''' 
    ''' 1. cid index database
    ''' 2. chemical descriptor property values database files.
    ''' 
    ''' </param>
    Sub New(dir$)
        base = dir
        dir.MakeDir

        For Each descriptor As PropertyInfo In ChemicalDescriptor.schema
            stream(descriptor.Name) = File.Open(
                path:=$"{dir}/{descriptor.Name}.db",
                mode:=FileMode.OpenOrCreate,
                access:=FileAccess.ReadWrite,
                share:=FileShare.Read
            )
        Next
    End Sub

    Public Function GetDescriptor(cid As Long) As ChemicalDescriptor
        Dim offset32Bits As Long = (cid - 1) * 4
        Dim offset64Bits As Long = (cid - 1) * 8
        Dim descriptor As New ChemicalDescriptor
        Dim buffer As Byte() = New Byte(8 - 1) {}

        stream(NameOf(descriptor.Complexity)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.Complexity)).Read(buffer, Scan0, 4)
        descriptor.Complexity = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.ExactMass)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.ExactMass)).Read(buffer, Scan0, 8)
        descriptor.ExactMass = BitConverter.ToDouble(buffer, Scan0)

        stream(NameOf(descriptor.FormalCharge)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.FormalCharge)).Read(buffer, Scan0, 4)
        descriptor.FormalCharge = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.HeavyAtoms)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.HeavyAtoms)).Read(buffer, Scan0, 4)
        descriptor.HeavyAtoms = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.HydrogenAcceptor)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.HydrogenAcceptor)).Read(buffer, Scan0, 4)
        descriptor.HydrogenAcceptor = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.HydrogenDonors)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.HydrogenDonors)).Read(buffer, Scan0, 4)
        descriptor.HydrogenDonors = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.RotatableBonds)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.RotatableBonds)).Read(buffer, Scan0, 4)
        descriptor.RotatableBonds = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.TopologicalPolarSurfaceArea)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.TopologicalPolarSurfaceArea)).Read(buffer, Scan0, 8)
        descriptor.TopologicalPolarSurfaceArea = BitConverter.ToDouble(buffer, Scan0)

        stream(NameOf(descriptor.XLogP3)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.XLogP3)).Read(buffer, Scan0, 8)
        descriptor.XLogP3 = BitConverter.ToDouble(buffer, Scan0)

        stream(NameOf(descriptor.XLogP3_AA)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.XLogP3_AA)).Read(buffer, Scan0, 8)
        descriptor.XLogP3_AA = BitConverter.ToDouble(buffer, Scan0)

        stream(NameOf(descriptor.AtomDefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.AtomDefStereoCount)).Read(buffer, Scan0, 4)
        descriptor.AtomDefStereoCount = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.AtomUdefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.AtomUdefStereoCount)).Read(buffer, Scan0, 4)
        descriptor.AtomUdefStereoCount = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.BondDefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.BondDefStereoCount)).Read(buffer, Scan0, 4)
        descriptor.BondDefStereoCount = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.BondUdefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.BondUdefStereoCount)).Read(buffer, Scan0, 4)
        descriptor.BondUdefStereoCount = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.ComponentCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.ComponentCount)).Read(buffer, Scan0, 4)
        descriptor.ComponentCount = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.IsotopicAtomCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.IsotopicAtomCount)).Read(buffer, Scan0, 4)
        descriptor.IsotopicAtomCount = BitConverter.ToInt32(buffer, Scan0)

        stream(NameOf(descriptor.TautoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.TautoCount)).Read(buffer, Scan0, 4)
        descriptor.TautoCount = BitConverter.ToInt32(buffer, Scan0)

        Return descriptor
    End Function

    Public Sub Write(cid&, descriptor As ChemicalDescriptor)
        Dim offset32Bits As Long = (cid - 1) * 4
        Dim offset64Bits As Long = (cid - 1) * 8

        stream(NameOf(descriptor.Complexity)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.Complexity)).Write(descriptor.Complexity)
        ' stream(NameOf(descriptor.Complexity)).Flush()

        stream(NameOf(descriptor.ExactMass)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.ExactMass)).Write(descriptor.ExactMass)
        ' stream(NameOf(descriptor.ExactMass)).Flush()

        stream(NameOf(descriptor.FormalCharge)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.FormalCharge)).Write(descriptor.FormalCharge)
        ' stream(NameOf(descriptor.FormalCharge)).Flush()

        stream(NameOf(descriptor.HeavyAtoms)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.HeavyAtoms)).Write(descriptor.HeavyAtoms)
        ' stream(NameOf(descriptor.HeavyAtoms)).Flush()

        stream(NameOf(descriptor.HydrogenAcceptor)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.HydrogenAcceptor)).Write(descriptor.HydrogenAcceptor)
        ' stream(NameOf(descriptor.HydrogenAcceptor)).Flush()

        stream(NameOf(descriptor.HydrogenDonors)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.HydrogenDonors)).Write(descriptor.HydrogenDonors)
        ' stream(NameOf(descriptor.HydrogenDonors)).Flush()

        stream(NameOf(descriptor.RotatableBonds)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.RotatableBonds)).Write(descriptor.RotatableBonds)
        ' stream(NameOf(descriptor.RotatableBonds)).Flush()

        stream(NameOf(descriptor.TopologicalPolarSurfaceArea)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.TopologicalPolarSurfaceArea)).Write(descriptor.TopologicalPolarSurfaceArea)
        ' stream(NameOf(descriptor.TopologicalPolarSurfaceArea)).Flush()

        stream(NameOf(descriptor.XLogP3)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.XLogP3)).Write(descriptor.XLogP3)
        ' stream(NameOf(descriptor.XLogP3)).Flush()

        stream(NameOf(descriptor.XLogP3_AA)).Seek(offset64Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.XLogP3_AA)).Write(descriptor.XLogP3_AA)
        ' stream(NameOf(descriptor.XLogP3_AA)).Flush()

        stream(NameOf(descriptor.AtomDefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.AtomDefStereoCount)).Write(descriptor.AtomDefStereoCount)

        stream(NameOf(descriptor.AtomUdefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.AtomUdefStereoCount)).Write(descriptor.AtomUdefStereoCount)

        stream(NameOf(descriptor.BondDefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.BondDefStereoCount)).Write(descriptor.BondDefStereoCount)

        stream(NameOf(descriptor.BondUdefStereoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.BondUdefStereoCount)).Write(descriptor.BondUdefStereoCount)

        stream(NameOf(descriptor.ComponentCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.ComponentCount)).Write(descriptor.ComponentCount)

        stream(NameOf(descriptor.IsotopicAtomCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.IsotopicAtomCount)).Write(descriptor.IsotopicAtomCount)

        stream(NameOf(descriptor.TautoCount)).Seek(offset32Bits, SeekOrigin.Begin)
        stream(NameOf(descriptor.TautoCount)).Write(descriptor.TautoCount)
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                For Each file As FileStream In stream.Values
                    Call file.Flush()
                    Call file.Close()
                    Call file.Dispose()
                Next
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

Module BinaryHelper

    <Extension>
    Public Sub Write(fs As FileStream, int As Integer)
        fs.Write(BitConverter.GetBytes(int), Scan0, 4)
    End Sub

    <Extension>
    Public Sub Write(fs As FileStream, dbl As Double)
        fs.Write(BitConverter.GetBytes(dbl), Scan0, 8)
    End Sub
End Module
