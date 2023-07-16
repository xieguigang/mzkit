Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports Microsoft.VisualBasic.ComponentModel.Algorithm

Namespace PackLib

    Friend Class MzIonSearch

        ReadOnly mzIndex As BlockSearchFunction(Of IonIndex)
        ReadOnly da As Tolerance

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return mzIndex.size = 0
            End Get
        End Property

        Sub New(mz As IEnumerable(Of IonIndex), da As Tolerance)
            Me.da = da

            ' see dev notes about the mass tolerance in 
            ' MSSearch module
            mzIndex = New BlockSearchFunction(Of IonIndex)(
                data:=mz,
                eval:=Function(m) m.mz,
                tolerance:=1,
                factor:=3
            )
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{mzIndex.Keys.Length} ions m/z index key information, mzdiff: {da.ToString}"
        End Function

        ''' <summary>
        ''' query the spectrum reference tree nodes via parent m/z matched
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <returns></returns>
        Public Function QueryByMz(mz As Double) As IEnumerable(Of IonIndex)
            Dim query As New IonIndex With {.mz = mz}
            Dim result As IEnumerable(Of IonIndex) = mzIndex _
                .Search(query) _
                .Where(Function(d) da(d.mz, mz))

            Return result
        End Function
    End Class
End Namespace