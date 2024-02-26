Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

Module MsSpectrumData

    ''' <summary>
    ''' A unify method for extract the <see cref="LibraryMatrix"/> spectrum 
    ''' data from various mzkit data object model.
    ''' </summary>
    ''' <param name="data">
    ''' <see cref="dataframe"/> object should contains 
    ''' ``mz`` and ``into`` these two column data at 
    ''' least.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' supports data models:
    ''' 
    ''' 1. <see cref="ms2"/>[], 
    ''' 2. <see cref="LibraryMatrix"/>, 
    ''' 3. <see cref="MGF.Ions"/>, 
    ''' 4. <see cref="PeakMs2"/>, 
    ''' 5. <see cref="ScanMs1"/>, 
    ''' 6. <see cref="ScanMs2"/> 
    ''' 7. and <see cref="dataframe"/>
    ''' </remarks>
    Public Function getSpectrum(data As Object, env As Environment) As [Variant](Of Message, LibraryMatrix)
        Dim type As Type = data.GetType

        Select Case type
            Case GetType(ms2())
                Return New LibraryMatrix With {.ms2 = data, .name = "Mass Spectrum"}
            Case GetType(LibraryMatrix)
                Return DirectCast(data, LibraryMatrix)
            Case GetType(MGF.Ions)
                Return DirectCast(data, MGF.Ions).GetLibrary
            Case GetType(ScanMS1), GetType(ScanMS2)
                With DirectCast(data, MSScan)
                    Dim peaks = .GetMs.ToArray
                    Dim libname As String = .scan_id
                    Dim libMs As New LibraryMatrix With {
                        .ms2 = peaks,
                        .name = libname
                    }

                    Return libMs
                End With
            Case GetType(dataframe)
                Return DirectCast(data, dataframe).SpectrumParseFromDataframe(env)
            Case GetType(PeakMs2)
                Dim name As String = DirectCast(data, PeakMs2).lib_guid

                If name.StringEmpty Then
                    name = $"M{CInt(DirectCast(data, PeakMs2).mz)}T{CInt(DirectCast(data, PeakMs2).rt) + 1}"
                End If

                Return New LibraryMatrix With {
                    .ms2 = DirectCast(data, PeakMs2).mzInto,
                    .name = name
                }
            Case Else
                Return RInternal.debug.stop(New NotImplementedException(type.FullName), env)
        End Select
    End Function

    <Extension>
    Private Function SpectrumParseFromDataframe(matrix As dataframe, env As Environment) As [Variant](Of Message, LibraryMatrix)
        If Not matrix.hasName("mz") Then
            Return Message.SymbolNotFound(env, "mz", TypeCodes.double)
        ElseIf Not matrix.hasName("into") Then
            Return Message.SymbolNotFound(env, "into", TypeCodes.double)
        End If

        Dim mz As Double() = CLRVector.asNumeric(matrix.getColumnVector("mz"))
        Dim into As Double() = CLRVector.asNumeric(matrix.getColumnVector("into"))
        Dim ms2 As ms2() = mz _
            .Select(Function(m, i)
                        Return New ms2 With {
                            .mz = m,
                            .intensity = into(i)
                        }
                    End Function) _
            .ToArray

        Return New LibraryMatrix With {.ms2 = ms2, .name = "Mass Spectrum"}
    End Function
End Module
