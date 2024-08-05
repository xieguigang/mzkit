
''' JNA-InChI - Library for calling InChI from Java
''' Copyright © 2018 Daniel Lowe
''' 
''' This library is free software; you can redistribute it and/or
''' modify it under the terms of the GNU Lesser General Public
''' License as published by the Free Software Foundation; either
''' version 2.1 of the License, or (at your option) any later version.
''' 
''' This program is distributed in the hope that it will be useful,
''' but WITHOUT ANY WARRANTY; without even the implied warranty of
''' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
''' GNU Lesser General Public License for more details.
''' 
''' You should have received a copy of the GNU Lesser General Public License
''' along with this program.  If not, see </>.

Namespace IUPAC.InChI
    Public Class InchiStereo

        ''' <summary>
        ''' Use when a stereocentre's configuration references an implicit hydrogen 
        ''' </summary>
        Public Shared ReadOnly STEREO_IMPLICIT_H As InchiAtom = New InchiAtom("H")

        Private ReadOnly atomsField As InchiAtom()
        Private ReadOnly centralAtomField As InchiAtom
        Private ReadOnly typeField As InchiStereoType
        Private ReadOnly parityField As InchiStereoParity


        Friend Sub New(atoms As InchiAtom(), centralAtom As InchiAtom, type As InchiStereoType, parity As InchiStereoParity)
            If atoms Is Nothing Then
                Throw New ArgumentException("atoms was null")
            End If
            If type Is Nothing Then
                Throw New ArgumentException("type was null")
            End If
            If parity Is Nothing Then
                Throw New ArgumentException("parity was null")
            End If
            If atoms.Length <> 4 Then
                Throw New ArgumentException("Atoms array should be length 4")
            End If
            For i = 0 To atoms.Length - 1
                If atoms(i) Is Nothing Then
                    Throw New ArgumentException("Atom at index " & i.ToString() & " was null, use STEREO_IMPLICIT_H for implicit hydrogen, and the atom with a lone pair for lone pairs")
                End If
            Next
            If type IsNot InchiStereoType.DoubleBond AndAlso centralAtom Is Nothing Then
                Throw New ArgumentException("centralAtom was null")
            End If
            atomsField = atoms
            centralAtomField = centralAtom
            typeField = type
            parityField = parity
        End Sub

        ''' <summary>
        ''' Defines the stereo configuration around the give centralAtom. The four vertexes of the tetrahedral centre should be given along with the parity.
        ''' If one of the vertexes is an implicit hydrogen use <seealsocref="STEREO_IMPLICIT_H"/>. If one is a lone pair, use the centralAtom for this vertex </summary>
        ''' <param name="centralAtom"> </param>
        ''' <param name="atom1"> </param>
        ''' <param name="atom2"> </param>
        ''' <param name="atom3"> </param>
        ''' <param name="atom4"> </param>
        ''' <param name="parity">
        ''' @return </param>
        Public Shared Function createTetrahedralStereo(centralAtom As InchiAtom, atom1 As InchiAtom, atom2 As InchiAtom, atom3 As InchiAtom, atom4 As InchiAtom, parity As InchiStereoParity) As InchiStereo
            Return New InchiStereo(New InchiAtom() {atom1, atom2, atom3, atom4}, centralAtom, InchiStereoType.Tetrahedral, parity)
        End Function

        ''' <summary>
        ''' <pre>
        ''' Given
        ''' A       E
        '''  \     /
        '''   C = D
        '''  /     \
        ''' B       F
        ''' 
        ''' atom1 is A (or B)
        ''' atom2 is C
        ''' atom3 is D
        ''' atom4 is E (or F)
        ''' and the parity is whether atom1 and atom2 are on the same side; <seealsocref="InchiStereoParity.ODD"/> if on the same side
        ''' Atom1/2 should be chosen such that neither are implicit hydrogen
        ''' 
        ''' For a cumulene (NOTE stereochemistry on cumulenes with more than 3 double bonds are unsupported by InChI)
        ''' Given
        ''' A               G
        '''  \             /
        '''   C = D = E = F
        '''  /             \
        ''' B               H
        ''' 
        ''' atom1 is A (or B)
        ''' atom2 is D
        ''' atom3 is E
        ''' atom4 is G (or H)
        ''' 
        ''' Atom1/2 should be chosen such that neither are implicit hydrogen
        ''' 
        ''' For the 2 adjacent double-bond case use <seealsocref="InchiStereo.createAllenalStereo(InchiAtom,InchiAtom,InchiAtom,InchiAtom,InchiAtom,InchiStereoParity)"/>
        ''' </pre> </summary>
        ''' <param name="atom1"> </param>
        ''' <param name="atom2"> </param>
        ''' <param name="atom3"> </param>
        ''' <param name="atom4"> </param>
        ''' <param name="parity">
        ''' @return </param>
        Public Shared Function createDoubleBondStereo(atom1 As InchiAtom, atom2 As InchiAtom, atom3 As InchiAtom, atom4 As InchiAtom, parity As InchiStereoParity) As InchiStereo
            If STEREO_IMPLICIT_H Is atom1 OrElse STEREO_IMPLICIT_H Is atom2 OrElse STEREO_IMPLICIT_H Is atom3 OrElse STEREO_IMPLICIT_H Is atom4 Then
                Throw New ArgumentException("Double bond stereo should use non-implicit hydrogn atoms")
            End If
            Return New InchiStereo(New InchiAtom() {atom1, atom2, atom3, atom4}, Nothing, InchiStereoType.DoubleBond, parity)
        End Function

        ''' <summary>
        ''' <pre>
        ''' Defines the stereo configuration of an allenal stereocentre, these behave like an extended tetrahedron.
        ''' The four vertexes of the tetrahedron should be given along with the parity.
        ''' If one of the vertexes is an implicit hydrogen use <seealsocref="STEREO_IMPLICIT_H"/>.
        ''' 
        ''' Given
        ''' A           F
        '''  \         /
        '''   C = D = E
        '''  /         \
        ''' B           G
        ''' 
        ''' centralAtom is D
        ''' atom1 is A 
        ''' atom2 is B
        ''' atom3 is F
        ''' atom4 is G
        ''' 
        ''' (NOTE allenal centers with more than 2 double bonds are unsupported by InChI)
        ''' </pre> </summary>
        ''' <param name="centralAtom"> </param>
        ''' <param name="atom1"> </param>
        ''' <param name="atom2"> </param>
        ''' <param name="atom3"> </param>
        ''' <param name="atom4"> </param>
        ''' <param name="parity">
        ''' @return </param>
        Public Shared Function createAllenalStereo(centralAtom As InchiAtom, atom1 As InchiAtom, atom2 As InchiAtom, atom3 As InchiAtom, atom4 As InchiAtom, parity As InchiStereoParity) As InchiStereo
            Return New InchiStereo(New InchiAtom() {atom1, atom2, atom3, atom4}, centralAtom, InchiStereoType.Allene, parity)
        End Function

        Public Overridable ReadOnly Property Atoms As InchiAtom()
            Get
                Return atomsField
            End Get
        End Property

        ''' <summary>
        ''' Null for <seealsocref="InchiStereoType.DoubleBond"/>
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property CentralAtom As InchiAtom
            Get
                Return centralAtomField
            End Get
        End Property

        Public Overridable ReadOnly Property Type As InchiStereoType
            Get
                Return typeField
            End Get
        End Property

        Public Overridable ReadOnly Property Parity As InchiStereoParity
            Get
                Return parityField
            End Get
        End Property

    End Class

End Namespace
