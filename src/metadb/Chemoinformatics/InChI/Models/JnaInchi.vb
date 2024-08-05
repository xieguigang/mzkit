Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text

Imports NativeLong = com.sun.jna.NativeLong
Imports Platform = com.sun.jna.Platform

Imports InchiLibrary = io.github.dan2097.jnainchi.inchi.InchiLibrary
Imports IxaFunctions = io.github.dan2097.jnainchi.inchi.IxaFunctions
Imports IXA_ATOMID = io.github.dan2097.jnainchi.inchi.IxaFunctions.IXA_ATOMID
Imports IXA_MOL_HANDLE = io.github.dan2097.jnainchi.inchi.IxaFunctions.IXA_MOL_HANDLE
Imports IXA_STATUS_HANDLE = io.github.dan2097.jnainchi.inchi.IxaFunctions.IXA_STATUS_HANDLE
Imports IXA_STEREOID = io.github.dan2097.jnainchi.inchi.IxaFunctions.IXA_STEREOID
Imports tagINCHIStereo0D = io.github.dan2097.jnainchi.inchi.tagINCHIStereo0D
Imports tagINCHI_Input = io.github.dan2097.jnainchi.inchi.tagINCHI_Input
Imports tagINCHI_InputINCHI = io.github.dan2097.jnainchi.inchi.tagINCHI_InputINCHI
Imports tagINCHI_Output = io.github.dan2097.jnainchi.inchi.tagINCHI_Output
Imports tagINCHI_OutputStruct = io.github.dan2097.jnainchi.inchi.tagINCHI_OutputStruct
Imports tagInchiAtom = io.github.dan2097.jnainchi.inchi.tagInchiAtom
Imports tagInchiInpData = io.github.dan2097.jnainchi.inchi.tagInchiInpData

''' <summary>
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
''' </summary>
Namespace io.github.dan2097.jnainchi

    Public Class JnaInchi

        Private Shared ReadOnly platform As String
        Private Shared ReadOnly libraryLoadingError As Exception
        Private Shared ReadOnly ISOTOPIC_SHIFT_RANGE_MIN As Integer = InchiLibrary.ISOTOPIC_SHIFT_FLAG - InchiLibrary.ISOTOPIC_SHIFT_MAX
        Private Shared ReadOnly ISOTOPIC_SHIFT_RANGE_MAX As Integer = InchiLibrary.ISOTOPIC_SHIFT_FLAG + InchiLibrary.ISOTOPIC_SHIFT_MAX
        Private Shared ReadOnly inchiBaseAtomicMasses As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

        Shared Sub New()
            Dim t As Exception = Nothing
            Dim p As String = Nothing
            Try
                p = com.sun.jna.Platform.RESOURCE_PREFIX
                InchiLibrary.JNA_NATIVE_LIB.Name
            Catch e As Exception
                t = e
            End Try
            platform = p
            libraryLoadingError = t

            'avg mw from util.c
            inchiBaseAtomicMasses("H") = 1
            inchiBaseAtomicMasses("D") = 2
            inchiBaseAtomicMasses("T") = 3
            inchiBaseAtomicMasses("He") = 4
            inchiBaseAtomicMasses("Li") = 7
            inchiBaseAtomicMasses("Be") = 9
            inchiBaseAtomicMasses("B") = 11
            inchiBaseAtomicMasses("C") = 12
            inchiBaseAtomicMasses("N") = 14
            inchiBaseAtomicMasses("O") = 16
            inchiBaseAtomicMasses("F") = 19
            inchiBaseAtomicMasses("Ne") = 20
            inchiBaseAtomicMasses("Na") = 23
            inchiBaseAtomicMasses("Mg") = 24
            inchiBaseAtomicMasses("Al") = 27
            inchiBaseAtomicMasses("Si") = 28
            inchiBaseAtomicMasses("P") = 31
            inchiBaseAtomicMasses("S") = 32
            inchiBaseAtomicMasses("Cl") = 35
            inchiBaseAtomicMasses("Ar") = 40
            inchiBaseAtomicMasses("K") = 39
            inchiBaseAtomicMasses("Ca") = 40
            inchiBaseAtomicMasses("Sc") = 45
            inchiBaseAtomicMasses("Ti") = 48
            inchiBaseAtomicMasses("V") = 51
            inchiBaseAtomicMasses("Cr") = 52
            inchiBaseAtomicMasses("Mn") = 55
            inchiBaseAtomicMasses("Fe") = 56
            inchiBaseAtomicMasses("Co") = 59
            inchiBaseAtomicMasses("Ni") = 59
            inchiBaseAtomicMasses("Cu") = 64
            inchiBaseAtomicMasses("Zn") = 65
            inchiBaseAtomicMasses("Ga") = 70
            inchiBaseAtomicMasses("Ge") = 73
            inchiBaseAtomicMasses("As") = 75
            inchiBaseAtomicMasses("Se") = 79
            inchiBaseAtomicMasses("Br") = 80
            inchiBaseAtomicMasses("Kr") = 84
            inchiBaseAtomicMasses("Rb") = 85
            inchiBaseAtomicMasses("Sr") = 88
            inchiBaseAtomicMasses("Y") = 89
            inchiBaseAtomicMasses("Zr") = 91
            inchiBaseAtomicMasses("Nb") = 93
            inchiBaseAtomicMasses("Mo") = 96
            inchiBaseAtomicMasses("Tc") = 98
            inchiBaseAtomicMasses("Ru") = 101
            inchiBaseAtomicMasses("Rh") = 103
            inchiBaseAtomicMasses("Pd") = 106
            inchiBaseAtomicMasses("Ag") = 108
            inchiBaseAtomicMasses("Cd") = 112
            inchiBaseAtomicMasses("In") = 115
            inchiBaseAtomicMasses("Sn") = 119
            inchiBaseAtomicMasses("Sb") = 122
            inchiBaseAtomicMasses("Te") = 128
            inchiBaseAtomicMasses("I") = 127
            inchiBaseAtomicMasses("Xe") = 131
            inchiBaseAtomicMasses("Cs") = 133
            inchiBaseAtomicMasses("Ba") = 137
            inchiBaseAtomicMasses("La") = 139
            inchiBaseAtomicMasses("Ce") = 140
            inchiBaseAtomicMasses("Pr") = 141
            inchiBaseAtomicMasses("Nd") = 144
            inchiBaseAtomicMasses("Pm") = 145
            inchiBaseAtomicMasses("Sm") = 150
            inchiBaseAtomicMasses("Eu") = 152
            inchiBaseAtomicMasses("Gd") = 157
            inchiBaseAtomicMasses("Tb") = 159
            inchiBaseAtomicMasses("Dy") = 163
            inchiBaseAtomicMasses("Ho") = 165
            inchiBaseAtomicMasses("Er") = 167
            inchiBaseAtomicMasses("Tm") = 169
            inchiBaseAtomicMasses("Yb") = 173
            inchiBaseAtomicMasses("Lu") = 175
            inchiBaseAtomicMasses("Hf") = 178
            inchiBaseAtomicMasses("Ta") = 181
            inchiBaseAtomicMasses("W") = 184
            inchiBaseAtomicMasses("Re") = 186
            inchiBaseAtomicMasses("Os") = 190
            inchiBaseAtomicMasses("Ir") = 192
            inchiBaseAtomicMasses("Pt") = 195
            inchiBaseAtomicMasses("Au") = 197
            inchiBaseAtomicMasses("Hg") = 201
            inchiBaseAtomicMasses("Tl") = 204
            inchiBaseAtomicMasses("Pb") = 207
            inchiBaseAtomicMasses("Bi") = 209
            inchiBaseAtomicMasses("Po") = 209
            inchiBaseAtomicMasses("At") = 210
            inchiBaseAtomicMasses("Rn") = 222
            inchiBaseAtomicMasses("Fr") = 223
            inchiBaseAtomicMasses("Ra") = 226
            inchiBaseAtomicMasses("Ac") = 227
            inchiBaseAtomicMasses("Th") = 232
            inchiBaseAtomicMasses("Pa") = 231
            inchiBaseAtomicMasses("U") = 238
            inchiBaseAtomicMasses("Np") = 237
            inchiBaseAtomicMasses("Pu") = 244
            inchiBaseAtomicMasses("Am") = 243
            inchiBaseAtomicMasses("Cm") = 247
            inchiBaseAtomicMasses("Bk") = 247
            inchiBaseAtomicMasses("Cf") = 251
            inchiBaseAtomicMasses("Es") = 252
            inchiBaseAtomicMasses("Fm") = 257
            inchiBaseAtomicMasses("Md") = 258
            inchiBaseAtomicMasses("No") = 259
            inchiBaseAtomicMasses("Lr") = 260
            inchiBaseAtomicMasses("Rf") = 261
            inchiBaseAtomicMasses("Db") = 270
            inchiBaseAtomicMasses("Sg") = 269
            inchiBaseAtomicMasses("Bh") = 270
            inchiBaseAtomicMasses("Hs") = 270
            inchiBaseAtomicMasses("Mt") = 278
            inchiBaseAtomicMasses("Ds") = 281
            inchiBaseAtomicMasses("Rg") = 281
            inchiBaseAtomicMasses("Cn") = 285
            inchiBaseAtomicMasses("Nh") = 278
            inchiBaseAtomicMasses("Fl") = 289
            inchiBaseAtomicMasses("Mc") = 289
            inchiBaseAtomicMasses("Lv") = 293
            inchiBaseAtomicMasses("Ts") = 297
            inchiBaseAtomicMasses("Og") = 294
        End Sub

        Public Shared Function toInchi(inchiInput As InchiInput) As InchiOutput
            Return toInchi(inchiInput, InchiOptions.DEFAULT_OPTIONS)
        End Function

        Public Shared Function toInchi(inchiInput As InchiInput, options As InchiOptions) As InchiOutput
            Call checkLibrary()
            Dim atoms = inchiInput.Atoms
            Dim atomCount = atoms.Count
            If atomCount > Short.MaxValue Then
                Throw New InvalidOperationException("InChI is limited to 32767 atoms, input contained " & atomCount.ToString() & " atoms")
            End If
            Dim bonds = inchiInput.Bonds
            Dim stereos = inchiInput.Stereos
            If stereos.Count > Short.MaxValue Then
                Throw New InvalidOperationException("Too many stereochemistry elements in input")
            End If

            Dim logger As IXA_STATUS_HANDLE = IxaFunctions.IXA_STATUS_Create()
            Dim nativeMol = IxaFunctions.IXA_MOL_Create(logger)
            IxaFunctions.IXA_MOL_ReserveSpace(logger, nativeMol, atomCount, bonds.Count, stereos.Count)
            Try
                Dim atomToNativeAtom = addAtoms(nativeMol, logger, atoms)
                addBonds(nativeMol, logger, bonds, atomToNativeAtom)
                addStereos(nativeMol, logger, stereos, atomToNativeAtom)
                Return buildInchi(logger, nativeMol, options)
            Finally
                IxaFunctions.IXA_MOL_Destroy(logger, nativeMol)
                IxaFunctions.IXA_STATUS_Destroy(logger)
            End Try
        End Function

        Private Shared Function addAtoms(mol As IXA_MOL_HANDLE, logger As IXA_STATUS_HANDLE, atoms As IList(Of InchiAtom)) As IDictionary(Of InchiAtom, IXA_ATOMID)
            Dim atomToNativeAtom As IDictionary(Of InchiAtom, IXA_ATOMID) = New Dictionary(Of InchiAtom, IXA_ATOMID)()
            For Each atom In atoms
                'For performance only call IxaFunctions when values differ from the defaults
                Dim nativeAtom = IxaFunctions.IXA_MOL_CreateAtom(logger, mol)
                atomToNativeAtom(atom) = nativeAtom

                If atom.X <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomX(logger, mol, nativeAtom, atom.X)
                End If
                If atom.Y <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomY(logger, mol, nativeAtom, atom.Y)
                End If
                If atom.Z <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomZ(logger, mol, nativeAtom, atom.Z)
                End If
                Dim elName = atom.ElName
                If Not elName.Equals("C") Then
                    If elName.Length > 5 Then
                        Throw New ArgumentException("Element name was too long: " & elName)
                    End If
                    IxaFunctions.IXA_MOL_SetAtomElement(logger, mol, nativeAtom, elName)
                End If
                If atom.IsotopicMass <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomMass(logger, mol, nativeAtom, atom.IsotopicMass)
                End If
                If atom.Charge <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomCharge(logger, mol, nativeAtom, atom.Charge)
                End If
                If atom.Radical IsNot InchiRadical.NONE Then
                    IxaFunctions.IXA_MOL_SetAtomRadical(logger, mol, nativeAtom, atom.Radical.Code)
                End If
                If atom.ImplicitHydrogen <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomHydrogens(logger, mol, nativeAtom, 0, atom.ImplicitHydrogen)
                End If
                If atom.ImplicitProtium <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomHydrogens(logger, mol, nativeAtom, 1, atom.ImplicitProtium)
                End If
                If atom.ImplicitDeuterium <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomHydrogens(logger, mol, nativeAtom, 2, atom.ImplicitDeuterium)
                End If
                If atom.ImplicitTritium <> 0 Then
                    IxaFunctions.IXA_MOL_SetAtomHydrogens(logger, mol, nativeAtom, 3, atom.ImplicitTritium)
                End If
            Next
            Return atomToNativeAtom
        End Function

        Private Shared Sub addBonds(mol As IXA_MOL_HANDLE, logger As IXA_STATUS_HANDLE, bonds As IList(Of InchiBond), atomToNativeAtom As IDictionary(Of InchiAtom, IXA_ATOMID))
            For Each bond In bonds
                Dim nativeAtom1 = atomToNativeAtom(bond.Start)
                Dim nativeAtom2 = atomToNativeAtom(bond.End)
                If nativeAtom1 Is Nothing OrElse nativeAtom2 Is Nothing Then
                    Throw New InvalidOperationException("Bond referenced an atom that was not part of the InchiInput")
                End If
                Dim nativeBond = IxaFunctions.IXA_MOL_CreateBond(logger, mol, nativeAtom1, nativeAtom2)
                Dim bondType = bond.Type
                If bondType IsNot InchiBondType.SINGLE Then
                    IxaFunctions.IXA_MOL_SetBondType(logger, mol, nativeBond, bondType.Code)
                End If
                Select Case bond.Stereo
                    Case DOUBLE_EITHER
                        'Default is to perceive configuration from 2D coordinates
                        IxaFunctions.IXA_MOL_SetDblBondConfig(logger, mol, nativeBond, InchiLibrary.IXA_DBLBOND_CONFIG_Fields.IXA_DBLBOND_CONFIG_EITHER)
                    Case SINGLE_1DOWN
                        IxaFunctions.IXA_MOL_SetBondWedge(logger, mol, nativeBond, nativeAtom1, InchiLibrary.IXA_BOND_WEDGE_Fields.IXA_BOND_WEDGE_DOWN)
                    Case SINGLE_1EITHER
                        IxaFunctions.IXA_MOL_SetBondWedge(logger, mol, nativeBond, nativeAtom1, InchiLibrary.IXA_BOND_WEDGE_Fields.IXA_BOND_WEDGE_EITHER)
                    Case SINGLE_1UP
                        IxaFunctions.IXA_MOL_SetBondWedge(logger, mol, nativeBond, nativeAtom1, InchiLibrary.IXA_BOND_WEDGE_Fields.IXA_BOND_WEDGE_UP)
                    Case SINGLE_2DOWN
                        IxaFunctions.IXA_MOL_SetBondWedge(logger, mol, nativeBond, nativeAtom2, InchiLibrary.IXA_BOND_WEDGE_Fields.IXA_BOND_WEDGE_DOWN)
                    Case SINGLE_2EITHER
                        IxaFunctions.IXA_MOL_SetBondWedge(logger, mol, nativeBond, nativeAtom2, InchiLibrary.IXA_BOND_WEDGE_Fields.IXA_BOND_WEDGE_EITHER)
                    Case SINGLE_2UP
                        IxaFunctions.IXA_MOL_SetBondWedge(logger, mol, nativeBond, nativeAtom2, InchiLibrary.IXA_BOND_WEDGE_Fields.IXA_BOND_WEDGE_UP)
                    Case NONE
                End Select
            Next
        End Sub
        Private Shared Sub addStereos(nativeMol As IXA_MOL_HANDLE, logger As IXA_STATUS_HANDLE, stereos As IList(Of InchiStereo), atomToNativeAtom As IDictionary(Of InchiAtom, IXA_ATOMID))
            For Each stereo In stereos
                Dim type = stereo.Type
                If type Is InchiStereoType.None Then
                    Continue For
                End If
                Dim atomsInCenter = stereo.Atoms
                Dim vertex1 = getStereoVertex(atomToNativeAtom, atomsInCenter(0))
                Dim vertex2 = getStereoVertex(atomToNativeAtom, atomsInCenter(1))
                Dim vertex3 = getStereoVertex(atomToNativeAtom, atomsInCenter(2))
                Dim vertex4 = getStereoVertex(atomToNativeAtom, atomsInCenter(3))

                Dim center As IXA_STEREOID
                Select Case type.innerEnumValue
                    Case InchiStereoType.InnerEnum.Tetrahedral
                        Dim centralAtom = atomToNativeAtom(stereo.CentralAtom)
                        If centralAtom Is Nothing Then
                            Throw New InvalidOperationException("Stereo configuration central atom referenced an atom that does not exist")
                        End If
                        center = IxaFunctions.IXA_MOL_CreateStereoTetrahedron(logger, nativeMol, centralAtom, vertex1, vertex2, vertex3, vertex4)
                        Exit Select
                    Case InchiStereoType.InnerEnum.Allene
                        Dim centralAtom = atomToNativeAtom(stereo.CentralAtom)
                        If centralAtom Is Nothing Then
                            Throw New InvalidOperationException("Stereo configuration central atom referenced an atom that does not exist")
                        End If
                        center = IxaFunctions.IXA_MOL_CreateStereoAntiRectangle(logger, nativeMol, centralAtom, vertex1, vertex2, vertex3, vertex4)
                        Exit Select
                    Case InchiStereoType.InnerEnum.DoubleBond
                        Dim centralBond = IxaFunctions.IXA_MOL_GetCommonBond(logger, nativeMol, vertex2, vertex3)
                        If centralBond Is Nothing Then
                            Throw New InvalidOperationException("Could not find olefin/cumulene central bond")
                        End If
                        'We intentionally pass dummy values for vertex2/vertex3, as the IXA API doesn't actually need these as long as vertex1 and vertex4 aren't implicit hydrogen
                        center = IxaFunctions.IXA_MOL_CreateStereoRectangle(logger, nativeMol, centralBond, vertex1, IxaFunctions.IXA_ATOMID_IMPLICIT_H, IxaFunctions.IXA_ATOMID_IMPLICIT_H, vertex4)
                        Exit Select
                    Case Else
                        Throw New InvalidOperationException("Unexpected InChI stereo type:" & type.ToString())
                End Select
                Dim parity = stereo.Parity.Code
                IxaFunctions.IXA_MOL_SetStereoParity(logger, nativeMol, center, parity)
            Next
        End Sub

        Private Shared Function getStereoVertex(atomToNativeAtom As IDictionary(Of InchiAtom, IXA_ATOMID), inchiAtom As InchiAtom) As IXA_ATOMID
            If InchiStereo.STEREO_IMPLICIT_H Is inchiAtom Then
                Return IxaFunctions.IXA_ATOMID_IMPLICIT_H
            End If
            Dim vertex = atomToNativeAtom(inchiAtom)
            If vertex Is Nothing Then
                Throw New InvalidOperationException("Stereo configuration referenced an atom that does not exist")
            End If
            Return vertex
        End Function

        Private Shared Function buildInchi(logger As IXA_STATUS_HANDLE, nativeMol As IXA_MOL_HANDLE, options As InchiOptions) As InchiOutput
            Dim builder = IxaFunctions.IXA_INCHIBUILDER_Create(logger)
            Try
                IxaFunctions.IXA_INCHIBUILDER_SetMolecule(logger, builder, nativeMol)

                Dim timeoutMilliSecs = options.TimeoutMilliSeconds
                If timeoutMilliSecs <> 0 Then
                    IxaFunctions.IXA_INCHIBUILDER_SetOption_Timeout_MilliSeconds(logger, builder, timeoutMilliSecs)
                End If
                For Each flag In options.Flags
                    Select Case flag.innerEnumValue
                        Case InchiFlag.InnerEnum.AuxNone
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_AuxNone, True)
                        Case InchiFlag.InnerEnum.ChiralFlagOFF
                            IxaFunctions.IXA_MOL_SetChiral(logger, nativeMol, False)
                        Case InchiFlag.InnerEnum.ChiralFlagON
                            IxaFunctions.IXA_MOL_SetChiral(logger, nativeMol, True)
                        Case InchiFlag.InnerEnum.DoNotAddH
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_DoNotAddH, True)
                        Case InchiFlag.InnerEnum.FixedH
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_FixedH, True)
                        Case InchiFlag.InnerEnum.KET
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_KET, True)
                        Case InchiFlag.InnerEnum.LargeMolecules
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_LargeMolecules, True)
                        Case InchiFlag.InnerEnum.NEWPSOFF
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_NewPsOff, True)
                        Case InchiFlag.InnerEnum.OneFiveT
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_15T, True)
                        Case InchiFlag.InnerEnum.RecMet
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_RecMet, True)
                        Case InchiFlag.InnerEnum.SLUUD
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_SLUUD, True)
                        Case InchiFlag.InnerEnum.SNon
                            IxaFunctions.IXA_INCHIBUILDER_SetOption_Stereo(logger, builder, InchiLibrary.IXA_INCHIBUILDER_STEREOOPTION_Fields.IXA_INCHIBUILDER_STEREOOPTION_SNon)
                        Case InchiFlag.InnerEnum.SRac
                            IxaFunctions.IXA_INCHIBUILDER_SetOption_Stereo(logger, builder, InchiLibrary.IXA_INCHIBUILDER_STEREOOPTION_Fields.IXA_INCHIBUILDER_STEREOOPTION_SRac)
                        Case InchiFlag.InnerEnum.SRel
                            IxaFunctions.IXA_INCHIBUILDER_SetOption_Stereo(logger, builder, InchiLibrary.IXA_INCHIBUILDER_STEREOOPTION_Fields.IXA_INCHIBUILDER_STEREOOPTION_SRel)
                        Case InchiFlag.InnerEnum.SUCF
                            IxaFunctions.IXA_INCHIBUILDER_SetOption_Stereo(logger, builder, InchiLibrary.IXA_INCHIBUILDER_STEREOOPTION_Fields.IXA_INCHIBUILDER_STEREOOPTION_SUCF)
                        Case InchiFlag.InnerEnum.SAbs
                            IxaFunctions.IXA_INCHIBUILDER_SetOption_Stereo(logger, builder, InchiLibrary.IXA_INCHIBUILDER_STEREOOPTION_Fields.IXA_INCHIBUILDER_STEREOOPTION_SAbs)
                        Case InchiFlag.InnerEnum.SUU
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_SUU, True)
                        Case InchiFlag.InnerEnum.SaveOpt
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_SaveOpt, True)
                        Case InchiFlag.InnerEnum.WarnOnEmptyStructure
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_WarnOnEmptyStructure, True)
                        Case InchiFlag.InnerEnum.NoWarnings
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_NoWarnings, True)
                        Case InchiFlag.InnerEnum.LooseTSACheck
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_LooseTSACheck, True)
                        Case InchiFlag.InnerEnum.Polymers
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_Polymers, True)
                        Case InchiFlag.InnerEnum.Polymers105
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_Polymers105, True)
                        Case InchiFlag.InnerEnum.FoldCRU
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_FoldCRU, True)
                        Case InchiFlag.InnerEnum.NoFrameShift
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_NoFrameShift, True)
                        Case InchiFlag.InnerEnum.NoEdits
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_NoEdits, True)
                        Case InchiFlag.InnerEnum.NPZz
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_NPZZ, True)
                        Case InchiFlag.InnerEnum.SAtZz
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_SATZZ, True)
                        Case InchiFlag.InnerEnum.OutErrInChI
                            IxaFunctions.IXA_INCHIBUILDER_SetOption(logger, builder, InchiLibrary.IXA_INCHIBUILDER_OPTION_Fields.IXA_INCHIBUILDER_OPTION_OutErrInChI, True)
                        Case Else
                            Throw New InvalidOperationException("Unexpected InChI option flag: " & flag.ToString())
                    End Select
                Next

                Dim inchi = IxaFunctions.IXA_INCHIBUILDER_GetInChI(logger, builder)
                Dim auxInfo = IxaFunctions.IXA_INCHIBUILDER_GetAuxInfo(logger, builder)
                Dim log = IxaFunctions.IXA_INCHIBUILDER_GetLog(logger, builder)

                Dim status = InchiStatus.SUCCESS
                If IxaFunctions.IXA_STATUS_HasError(logger) Then
                    status = InchiStatus.ERROR
                ElseIf IxaFunctions.IXA_STATUS_HasWarning(logger) Then
                    status = InchiStatus.WARNING
                End If

                Dim sb As StringBuilder = New StringBuilder()
                Dim messageCount = IxaFunctions.IXA_STATUS_GetCount(logger)
                For i = 0 To messageCount - 1
                    If i > 0 Then
                        sb.Append("; ")
                    End If
                    sb.Append(IxaFunctions.IXA_STATUS_GetMessage(logger, i))
                Next
                Return New InchiOutput(inchi, auxInfo, sb.ToString(), log, status)
            Finally
                IxaFunctions.IXA_INCHIBUILDER_Destroy(logger, builder)
            End Try
        End Function

        Public Shared Function molToInchi(molText As String) As InchiOutput
            Return molToInchi(molText, InchiOptions.DEFAULT_OPTIONS)
        End Function

        Public Shared Function molToInchi(molText As String, options As InchiOptions) As InchiOutput
            Call checkLibrary()
            Dim nativeOutput As tagINCHI_Output = New tagINCHI_Output()
            Try
                Dim ret As Integer = InchiLibrary.MakeINCHIFromMolfileText(molText, options.ToString(), nativeOutput)
                Dim status As InchiStatus
                Select Case ret
                    Case InchiLibrary.tagRetValMOL2INCHI_Fields.mol2inchi_Ret_OKAY
                        status = InchiStatus.SUCCESS
                    Case InchiLibrary.tagRetValMOL2INCHI_Fields.mol2inchi_Ret_WARNING
                        status = InchiStatus.WARNING
                    Case InchiLibrary.tagRetValMOL2INCHI_Fields.mol2inchi_Ret_EOF, InchiLibrary.tagRetValMOL2INCHI_Fields.mol2inchi_Ret_ERROR, InchiLibrary.tagRetValMOL2INCHI_Fields.mol2inchi_Ret_ERROR_get, InchiLibrary.tagRetValMOL2INCHI_Fields.mol2inchi_Ret_ERROR_comp
                        status = InchiStatus.ERROR
                    Case Else
                        status = InchiStatus.ERROR
                End Select
                ' The way nativeOutput.szLog is truncated can be a bit odd, but this seems pseudo-intentional, see copy_corrected_log_tail in inchi_dll.c 
                Return New InchiOutput(nativeOutput.szInChI, nativeOutput.szAuxInfo, nativeOutput.szMessage, nativeOutput.szLog, status)
            Finally
                InchiLibrary.FreeINCHI(nativeOutput)
            End Try
        End Function

        ''' <summary>
        ''' Converts InChI into InChI for validation purposes.
        ''' It may also be used to filter out specific layers.
        ''' For instance, SNon would remove the stereochemical layer.
        ''' Omitting FixedH and/or RecMet would remove Fixed-H or Reconnected layers. </summary>
        ''' <paramname="inchi"> </param>
        ''' <paramname="options">
        ''' @return </param>
        Public Shared Function inchiToInchi(inchi As String, options As InchiOptions) As InchiOutput
            Call checkLibrary()
            Dim logger As IXA_STATUS_HANDLE = IxaFunctions.IXA_STATUS_Create()
            Dim nativeMol = IxaFunctions.IXA_MOL_Create(logger)
            Try
                IxaFunctions.IXA_MOL_ReadInChI(logger, nativeMol, inchi)
                Return buildInchi(logger, nativeMol, options)
            Finally
                IxaFunctions.IXA_MOL_Destroy(logger, nativeMol)
                IxaFunctions.IXA_STATUS_Destroy(logger)
            End Try
        End Function

        Public Shared Function inchiToInchiKey(inchi As String) As InchiKeyOutput
            Call checkLibrary()
            Dim inchiKeyBytes = New SByte(27) {}
            Dim szXtra1Bytes = New SByte(64) {}
            Dim szXtra2Bytes = New SByte(64) {}
            Dim ret = InchiKeyStatus.of(InchiLibrary.GetINCHIKeyFromINCHI(inchi, 1, 1, inchiKeyBytes, szXtra1Bytes, szXtra2Bytes))
            Dim inchiKeyStr As String = StringHelper.NewString(inchiKeyBytes, StandardCharsets.UTF_8).Trim()
            Dim szXtra1 As String = StringHelper.NewString(szXtra1Bytes, StandardCharsets.UTF_8).Trim()
            Dim szXtra2 As String = StringHelper.NewString(szXtra2Bytes, StandardCharsets.UTF_8).Trim()
            Return New InchiKeyOutput(inchiKeyStr, ret, szXtra1, szXtra2)
        End Function

        ''' <summary>
        ''' Check if the string represents a valid InChI/StdInChI
        ''' If strict is true, try to perform InChI2InChI conversion; returns success if a resulting InChI string exactly matches source.
        ''' Be cautious: the result may be too strict, i.e. a 'false alarm', due to imperfection of conversion. </summary>
        ''' <paramname="inchi"> </param>
        ''' <paramname="strict"> if false, just briefly check for proper layout (prefix, version, etc.) </param>
        ''' <returns> InchiCheckStatus </returns>
        Public Shared Function checkInchi(inchi As String, strict As Boolean) As InchiCheckStatus
            Call checkLibrary()
            Return InchiCheckStatus.of(InchiLibrary.CheckINCHI(inchi, strict))
        End Function

        ''' <summary>
        ''' Check if the string represents valid InChIKey </summary>
        ''' <paramname="inchiKey"> </param>
        ''' <returns> InchiKeyCheckStatus </returns>
        Public Shared Function checkInchiKey(inchiKey As String) As InchiKeyCheckStatus
            Call checkLibrary()
            Return InchiKeyCheckStatus.of(InchiLibrary.CheckINCHIKey(inchiKey))
        End Function

        ''' <summary>
        ''' Creates the input data structure for InChI generation out of the auxiliary information (AuxInfo) 
        ''' string produced by previous InChI generator calls </summary>
        ''' <paramname="auxInfo"> contains ASCIIZ string of InChI output for a single structure or only the AuxInfo line </param>
        ''' <paramname="doNotAddH"> if true then InChI will not be allowed to add implicit H </param>
        ''' <paramname="diffUnkUndfStereo"> if true, use different labels for unknown and undefined stereo
        ''' @return </param>
        Public Shared Function getInchiInputFromAuxInfo(auxInfo As String, doNotAddH As Boolean, diffUnkUndfStereo As Boolean) As InchiInputFromAuxinfoOutput
            Call checkLibrary()
            Dim pInp As tagINCHI_Input = New tagINCHI_Input()
            Dim input As tagInchiInpData = New tagInchiInpData(pInp)
            Try
                Dim status = getInchiStatus(InchiLibrary.Get_inchi_Input_FromAuxInfo(auxInfo, doNotAddH, diffUnkUndfStereo, input))

                Dim inchiInput As InchiInput = New InchiInput()

                Dim populatedInput = input.pInp
                If populatedInput.num_atoms > 0 Then
                    Dim nativeAtoms = New tagInchiAtom(populatedInput.num_atoms - 1) {}
                    populatedInput.atom.toArray(nativeAtoms)
                    nativeToJavaAtoms(inchiInput, nativeAtoms)
                    nativeToJavaBonds(inchiInput, nativeAtoms)
                End If
                If populatedInput.num_stereo0D > 0 Then
                    Dim nativeStereos = New tagINCHIStereo0D(populatedInput.num_stereo0D - 1) {}
                    populatedInput.stereo0D.toArray(nativeStereos)
                    nativeToJavaStereos(inchiInput, nativeStereos)
                End If
                Dim message = toString(input.szErrMsg)
                Dim chiralFlag As Boolean? = Nothing
                If input.bChiral = 1 Then
                    chiralFlag = True
                ElseIf input.bChiral = 2 Then
                    chiralFlag = False
                End If
                Return New InchiInputFromAuxinfoOutput(inchiInput, chiralFlag, message, status)
            Finally
                InchiLibrary.Free_inchi_Input(pInp)
                input.clear()
            End Try
        End Function

        Public Shared Function getInchiInputFromInchi(inchi As String) As InchiInputFromInchiOutput
            Return getInchiInputFromInchi(inchi, InchiOptions.DEFAULT_OPTIONS)
        End Function

        Public Shared Function getInchiInputFromInchi(inchi As String, options As InchiOptions) As InchiInputFromInchiOutput
            Call checkLibrary()
            Dim input As tagINCHI_InputINCHI = New tagINCHI_InputINCHI(inchi, options.ToString())
            Dim output As tagINCHI_OutputStruct = New tagINCHI_OutputStruct()
            Try
                Dim status = getInchiStatus(InchiLibrary.GetStructFromINCHI(input, output))
                Dim inchiInput As InchiInput = New InchiInput()

                If output.num_atoms > 0 Then
                    Dim nativeAtoms = New tagInchiAtom(output.num_atoms - 1) {}
                    output.atom.toArray(nativeAtoms)
                    nativeToJavaAtoms(inchiInput, nativeAtoms)
                    nativeToJavaBonds(inchiInput, nativeAtoms)
                End If
                If output.num_stereo0D > 0 Then
                    Dim nativeStereos = New tagINCHIStereo0D(output.num_stereo0D - 1) {}
                    output.stereo0D.toArray(nativeStereos)
                    nativeToJavaStereos(inchiInput, nativeStereos)
                End If
                Dim message = output.szMessage
                Dim log = output.szLog
                Dim nativeFlags = output.WarningFlags 'This is a flattened multi-dimensional array, unflatten as we convert
                Dim warningFlags = {New Long(1) {}, New Long(1) {}}
                For i = 0 To nativeFlags.Length - 1
                    Dim val As Long = nativeFlags(i).longValue()
                    Select Case i
                        Case 0
                            warningFlags(0)(0) = val
                        Case 1
                            warningFlags(0)(1) = val
                        Case 2
                            warningFlags(1)(0) = val
                        Case 3
                            warningFlags(1)(1) = val
                        Case Else
                    End Select
                Next
                Return New InchiInputFromInchiOutput(inchiInput, message, log, status, warningFlags)
            Finally
                InchiLibrary.FreeStructFromINCHI(output)
                input.clear()
            End Try
        End Function

        Private Shared Sub nativeToJavaAtoms(inchiInput As InchiInput, nativeAtoms As tagInchiAtom())
            Dim i = 0, numAtoms = nativeAtoms.Length

            While i < numAtoms
                Dim nativeAtom = nativeAtoms(i)
                Dim elSymbol = toString(nativeAtom.elname)
                Dim atom As InchiAtom = New InchiAtom(elSymbol)
                atom.X = nativeAtom.x
                atom.Y = nativeAtom.y
                atom.Z = nativeAtom.z
                atom.ImplicitHydrogen = nativeAtom.num_iso_H(0)
                atom.ImplicitProtium = nativeAtom.num_iso_H(1)
                atom.ImplicitDeuterium = nativeAtom.num_iso_H(2)
                atom.ImplicitTritium = nativeAtom.num_iso_H(3)
                Dim isotopicMass As Integer = nativeAtom.isotopic_mass
                If isotopicMass >= ISOTOPIC_SHIFT_RANGE_MIN AndAlso isotopicMass <= ISOTOPIC_SHIFT_RANGE_MAX Then
                    'isotopic mass contains a delta from a hardcoded base mass
                    Dim baseMass As Integer = inchiBaseAtomicMasses.getOrDefault(elSymbol, 0)
                    Dim delta = isotopicMass - InchiLibrary.ISOTOPIC_SHIFT_FLAG
                    isotopicMass = baseMass + delta
                End If
                atom.IsotopicMass = isotopicMass
                atom.Radical = InchiRadical.of(nativeAtom.radical)
                atom.Charge = nativeAtom.charge
                inchiInput.addAtom(atom)
                i += 1
            End While
        End Sub

        Private Shared Sub nativeToJavaBonds(inchiInput As InchiInput, nativeAtoms As tagInchiAtom())
            Dim numAtoms = nativeAtoms.Length
            Dim seenAtoms = New Boolean(numAtoms - 1) {}
            For i = 0 To numAtoms - 1
                Dim nativeAtom = nativeAtoms(i)
                Dim numBonds As Integer = nativeAtom.num_bonds
                If numBonds > 0 Then
                    Dim atom = inchiInput.getAtom(i)
                    For j = 0 To numBonds - 1
                        Dim neighborIdx As Integer = nativeAtom.neighbor(j)
                        If seenAtoms(neighborIdx) Then
                            'Only add each bond once
                            Continue For
                        End If
                        Dim neighbor = inchiInput.getAtom(neighborIdx)
                        Dim bondType = InchiBondType.of(nativeAtom.bond_type(j))
                        Dim bondStereo = InchiBondStereo.of(nativeAtom.bond_stereo(j))
                        inchiInput.addBond(New InchiBond(atom, neighbor, bondType, bondStereo))
                    Next
                End If
                seenAtoms(i) = True
            Next
        End Sub

        Private Shared Sub nativeToJavaStereos(inchiInput As InchiInput, nativeStereos As tagINCHIStereo0D())
            For Each nativeStereo In nativeStereos
                Dim atoms = New InchiAtom(3) {}
                'idxToAtom will give null for -1 input (implicit hydrogen)
                For i = 0 To 3
                    Dim idx As Integer = nativeStereo.neighbor(i)
                    atoms(i) = If(idx >= 0, inchiInput.getAtom(idx), Nothing)
                Next

                Dim centralAtom = If(nativeStereo.central_atom >= 0, inchiInput.getAtom(nativeStereo.central_atom), Nothing)
                Dim stereoType = InchiStereoType.of(nativeStereo.type)
                Dim parity = InchiStereoParity.of(nativeStereo.parity)

                inchiInput.addStereo(New InchiStereo(atoms, centralAtom, stereoType, parity))
            Next
        End Sub

        Private Shared Function getInchiStatus(ret As Integer) As InchiStatus
            Select Case ret
                Case InchiLibrary.tagRetValGetINCHI_Fields.inchi_Ret_OKAY ' Success; no errors or warnings
                    Return InchiStatus.SUCCESS
                Case InchiLibrary.tagRetValGetINCHI_Fields.inchi_Ret_EOF, InchiLibrary.tagRetValGetINCHI_Fields.inchi_Ret_WARNING ' no structural data has been provided
                    ' Success; warning(s) issued
                    Return InchiStatus.WARNING
                Case InchiLibrary.tagRetValGetINCHI_Fields.inchi_Ret_ERROR, InchiLibrary.tagRetValGetINCHI_Fields.inchi_Ret_FATAL, InchiLibrary.tagRetValGetINCHI_Fields.inchi_Ret_UNKNOWN, InchiLibrary.tagRetValGetINCHI_Fields.inchi_Ret_BUSY ' Error: no InChI has been created
                    ' Severe error: no InChI has been created (typically, memory allocation failure)
                    ' Unknown program error
                    ' Previous call to InChI has not returned yet
                    Return InchiStatus.ERROR
                Case Else
                    Return InchiStatus.ERROR
            End Select
        End Function

        Private Shared Function toString([cstr] As SByte()) As String
            Dim sb As StringBuilder = New StringBuilder([cstr].Length)
            For i = 0 To [cstr].Length - 1
                Dim ch = Microsoft.VisualBasic.ChrW([cstr](i))
                If ch = ChrW(0) Then
                    Exit For
                End If
                sb.Append(ch)
            Next
            Return sb.ToString()
        End Function


        ''' <summary>
        ''' Returns the version of the wrapped InChI C library </summary>
        ''' <returns> Version number String </returns>
        Public Shared ReadOnly Property InchiLibraryVersion As String
            Get
                Try
                    Using [is] As Stream = GetType(JnaInchi).getResourceAsStream("jnainchi_build.props")
                        Dim props As Properties = New Properties()
                        props.load([is])
                        Return props.getProperty("inchi_version")
                    End Using
                Catch __unusedException1__ As Exception
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Returns the version of the JNA-InChI Java library </summary>
        ''' <returns> Version number String </returns>
        Public Shared ReadOnly Property JnaInchiVersion As String
            Get
                Try
                    Using [is] As Stream = GetType(JnaInchi).getResourceAsStream("jnainchi_build.props")
                        Dim props As Properties = New Properties()
                        props.load([is])
                        Return props.getProperty("jnainchi_version")
                    End Using
                Catch __unusedException1__ As Exception
                    Return Nothing
                End Try
            End Get
        End Property

        Private Shared Sub checkLibrary()
            If libraryLoadingError IsNot Nothing Then
                Throw New Exception("Error loading InChI native code. Please check that the binaries for your platform (" & platform & ") have been included on the classpath.", libraryLoadingError)
            End If
        End Sub

    End Class

End Namespace
