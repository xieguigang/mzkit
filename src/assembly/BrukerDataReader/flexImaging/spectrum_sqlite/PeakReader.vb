#Region "Microsoft.VisualBasic::1c535a8c60fe85976da5b144f1db29d8, E:/mzkit/src/assembly/BrukerDataReader//flexImaging/spectrum_sqlite/PeakReader.vb"

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

    '   Total Lines: 105
    '    Code Lines: 77
    ' Comment Lines: 15
    '   Blank Lines: 13
    '     File Size: 4.15 KB


    ' Class PeakReader
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetProperties, GetSpectra
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.SQLSchema
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.Tables

''' <summary>
''' peaks.sqlite
''' </summary>
Public Class PeakReader : Implements IDisposable

    ReadOnly peaks As Sqlite3Database
    Private disposedValue As Boolean

    Sub New(file As String)
        peaks = Sqlite3Database.OpenFile(file)
    End Sub

    Public Iterator Function GetSpectra() As IEnumerable(Of Spectra)
        Dim table As Sqlite3Table = peaks.GetTable("Spectra")
        Dim schema As SQLSchema.Schema = table.SchemaDefinition.ParseSchema
        Dim id As Integer = schema.GetOrdinal(NameOf(Spectra.Id))
        Dim chip As Integer = schema.GetOrdinal(NameOf(Spectra.Chip))
        Dim spot As Integer = schema.GetOrdinal(NameOf(Spectra.SpotName))
        Dim region As Integer = schema.GetOrdinal(NameOf(Spectra.RegionNumber))
        Dim x As Integer = schema.GetOrdinal(NameOf(Spectra.XIndexPos))
        Dim y As Integer = schema.GetOrdinal(NameOf(Spectra.YIndexPos))
        Dim numPeaks As Integer = schema.GetOrdinal(NameOf(Spectra.NumPeaks))
        Dim mz As Integer = schema.GetOrdinal(NameOf(Spectra.PeakMzValues))
        Dim into As Integer = schema.GetOrdinal(NameOf(Spectra.PeakIntensityValues))
        Dim intensity As Double()
        Dim mzi As Double()
        Dim intensityBuf As Byte()
        Dim mzBuf As Byte()
        Dim npeaks As Integer

        ' 20220729
        ' ms spectra data is not network byte order 

        For Each row As Sqlite3Row In table.EnumerateRows
            npeaks = row(numPeaks)
            intensityBuf = row(into)
            intensity = intensityBuf _
                .Split(4) _
                .Select(Function(b) CDbl(BitConverter.ToSingle(b, Scan0))) _
                .ToArray
            mzBuf = row(mz)
            mzi = mzBuf _
                .Split(8) _
                .Select(Function(b) BitConverter.ToDouble(b, Scan0)) _
                .ToArray

            Yield New Spectra With {
                .Chip = row(chip),
                .SpotName = row(spot),
                .RegionNumber = row(region),
                .Id = row(id),
                .NumPeaks = npeaks,
                .XIndexPos = row(x),
                .YIndexPos = row(y),
                .PeakIntensityValues = intensity,
                .PeakMzValues = mzi
            }
        Next
    End Function

    Public Iterator Function GetProperties() As IEnumerable(Of Properties)
        Dim table As Sqlite3Table = peaks.GetTable("Properties")
        Dim schema As SQLSchema.Schema = table.SchemaDefinition.ParseSchema
        Dim key As Integer = schema.GetOrdinal("Key")
        Dim value As Integer = schema.GetOrdinal("Value")

        For Each row As Sqlite3Row In table.EnumerateRows
            Yield New Properties With {
                .Key = row(key),
                .Value = row(value)
            }
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call peaks.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
