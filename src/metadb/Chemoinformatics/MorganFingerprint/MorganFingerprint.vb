Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models

Namespace MorganFingerprint

    Public Class MorganFingerprint

        ReadOnly FingerprintLength As Integer = 1024
        ReadOnly Iterations As Integer = 2

        Public Function CalculateFingerprint(struct As [Structure]) As BitArray
            Dim atoms As MorganAtom() = New MorganAtom(struct.Atoms.Length - 1) {}

            ' Initialize atom codes based on atom type
            For i As Integer = 0 To struct.Atoms.Length - 1
                atoms(i) = New MorganAtom(struct.Atoms(i))
                atoms(i).Code = GetAtomCode(struct.Atoms(i).Atom)
                atoms(i).Index = i
            Next

            ' Perform iterations to expand the atom codes
            For iteration As Integer = 0 To Iterations - 1
                Dim newCodes As Integer() = New Integer(struct.Atoms.Length - 1) {}

                For Each bound As Bound In struct.Bounds
                    Dim code1 As Integer = atoms(bound.i).Code
                    Dim code2 As Integer = atoms(bound.j).Code
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
                Dim position As Integer = atom.Code Mod FingerprintLength
                fingerprint.Set(position, True)
            Next

            Return fingerprint
        End Function

        Private Shared Function GetAtomCode(element As String) As Integer
            ' Simple hash function for atom type
            Return element.GetHashCode()
        End Function

        Private Shared Function HashCodes(code1 As Integer, code2 As Integer, bondType As BoundTypes, stereo As BoundStereos) As Integer
            ' Combine the codes and return a new hash code
            Return (code1 Xor code2 Xor CInt(bondType) Xor CInt(stereo)).GetHashCode()
        End Function

    End Class
End Namespace