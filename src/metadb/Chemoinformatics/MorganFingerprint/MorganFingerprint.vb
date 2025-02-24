Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports Microsoft.VisualBasic.Math.HashMaps

Namespace MorganFingerprint

    Public Module MorganFingerprint

        <Extension>
        Public Function CalculateFingerprint(struct As [Structure],
                                             Optional FingerprintLength As Integer = 1024,
                                             Optional Iterations As Integer = 3) As BitArray

            Dim atoms As MorganAtom() = New MorganAtom(struct.Atoms.Length - 1) {}

            ' Initialize atom codes based on atom type
            For i As Integer = 0 To struct.Atoms.Length - 1
                atoms(i) = New MorganAtom(struct.Atoms(i))
                atoms(i).Code = CULng(GetAtomCode(struct.Atoms(i).Atom))
                atoms(i).Index = i
            Next

            ' Perform iterations to expand the atom codes
            For iteration As Integer = 0 To Iterations - 1
                Dim newCodes As ULong() = New ULong(struct.Atoms.Length - 1) {}

                For Each bound As Bound In struct.Bounds
                    Dim code1 As ULong = atoms(bound.i).Code
                    Dim code2 As ULong = atoms(bound.j).Code

                    newCodes(bound.i) = HashCodes(code1, code2, bound.Type, bound.Stereo)
                    newCodes(bound.j) = HashCodes(code2, code1, bound.Type, bound.Stereo)
                Next

                For i As Integer = 0 To struct.Atoms.Length - 1
                    atoms(i).Code = newCodes(i)
                Next
            Next

            ' Generate the final fingerprint
            Dim fingerprint As New BitArray(FingerprintLength)

            For Each atom As MorganAtom In atoms
                Call fingerprint.Xor(position:=atom.Code Mod FingerprintLength)
            Next

            Return fingerprint
        End Function

        <Extension>
        Private Sub [Xor](fingerprint As BitArray, position As Integer)
            If fingerprint.Get(position) Then
                Call fingerprint.Set(position, False)
            Else
                Call fingerprint.Set(position, True)
            End If
        End Sub

        ''' <summary>
        ''' Simple hash function for atom type
        ''' </summary>
        ''' <param name="element"></param>
        ''' <returns></returns>
        Private Function GetAtomCode(element As String) As Integer
            Return element.GetHashCode()
        End Function

        ''' <summary>
        ''' Combine the codes and return a new hash code
        ''' </summary>
        ''' <param name="code1"></param>
        ''' <param name="code2"></param>
        ''' <param name="bondType"></param>
        ''' <param name="stereo"></param>
        ''' <returns></returns>
        Private Function HashCodes(code1 As ULong, code2 As ULong, bondType As BoundTypes, stereo As BoundStereos) As ULong
            Return HashMap.HashCodePair(code1, code2) Xor CULng(bondType) Xor CULng(stereo)
        End Function

    End Module
End Namespace