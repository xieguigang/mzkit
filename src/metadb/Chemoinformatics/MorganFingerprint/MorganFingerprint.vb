Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports Microsoft.VisualBasic.Math.HashMaps

Namespace MorganFingerprint

    ''' <summary>
    ''' Morgan fingerprints, also known as circular fingerprints, are a type of molecular fingerprint 
    ''' used in cheminformatics to represent the structure of chemical compounds. The algorithm steps 
    ''' for generating Morgan fingerprints are as follows:
    ''' 
    ''' 1. **Initialization**:
    '''  - Start with the initial set of atoms in the molecule.
    '''  - Assign a unique identifier (e.g., integer) to each atom.
    '''  
    ''' 2. **Atom Environment Encoding**:
    '''  - For each atom, encode its immediate environment, which includes the atom type and the types of its directly connected neighbors.
    '''  - This information can be represented as a string or a hash.
    '''  
    ''' 3. **Iterative Expansion**:
    '''  - Expand the environment encoding iteratively to include atoms further away from the starting atom.
    '''  - In each iteration, update the encoding to include the types of atoms that are two, three, etc., bonds away from the starting atom.
    '''  
    ''' 4. **Hashing**:
    '''  - Convert the environment encoding into a fixed-size integer using a hashing function. This integer represents the fingerprint of the atom's environment.
    '''  - Different atoms in the molecule will have different fingerprints based on their environments.
    '''  
    ''' 5. **Circular Fingerprint Generation**:
    '''  - For each atom, generate a series of fingerprints that represent its environment at different radii (number of bonds away).
    '''  - The final fingerprint for an atom is a combination of these series of fingerprints.
    '''  
    ''' 6. **Molecular Fingerprint**:
    '''  - Combine the fingerprints of all atoms in the molecule to create the final molecular fingerprint.
    '''  - This can be done by taking the bitwise OR of all atom fingerprints, resulting in a single fingerprint that represents the entire molecule.
    '''  
    ''' 7. **Optional Folding**:
    '''  - To reduce the size of the fingerprint, an optional folding step can be applied. This involves 
    '''    dividing the fingerprint into chunks and performing a bitwise XOR operation within each chunk.
    '''    
    ''' 8. **Result**:
    '''  - The final result is a binary vector (or a list of integers) that represents the Morgan fingerprint 
    '''    of the molecule. This fingerprint can be used for similarity searching, clustering, and other 
    '''    cheminformatics tasks.
    '''    
    ''' Morgan fingerprints are particularly useful because they capture the circular nature of molecular
    ''' environments, meaning that the path taken to reach an atom is not as important as the environment 
    ''' around it. This makes them effective for comparing the similarity of molecules based on their 
    ''' structural features.
    ''' </summary>
    Public Module MorganFingerprint

        Public ReadOnly Property FingerprintLength As Integer = 1024

        Public Sub SetLength(len As Integer)
            If len Mod 8 <> 0 Then
                Throw New InvalidProgramException("the length of the Morgan Fingerprint vector should be mod by 8!")
            Else
                _FingerprintLength = len
            End If
        End Sub

        <Extension>
        Public Function CalculateFingerprintCheckSum(struct As [Structure], Optional radius As Integer = 3) As Byte()
            Dim bits As BitArray = struct.CalculateFingerprint(radius)
            Dim bytes = New Byte(FingerprintLength / 8 - 1) {}
            bits.CopyTo(bytes, 0)
            Return bytes
        End Function

        <Extension>
        Public Function CalculateFingerprint(struct As [Structure], Optional radius As Integer = 3) As BitArray
            Dim atoms As MorganAtom() = New MorganAtom(struct.Atoms.Length - 1) {}

            ' Initialize atom codes based on atom type
            For i As Integer = 0 To struct.Atoms.Length - 1
                atoms(i) = New MorganAtom(struct.Atoms(i))
                atoms(i).Code = CULng(GetAtomCode(struct.Atoms(i).Atom))
                atoms(i).Index = i
            Next

            ' Perform iterations to expand the atom codes
            For r As Integer = 0 To radius - 1
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