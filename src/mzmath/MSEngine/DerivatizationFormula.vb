Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class DerivatizationFormula : Inherits Formula

    Public ReadOnly Property TMS As Integer
    Public ReadOnly Property MEOX As Integer
    Public ReadOnly Property C13 As Integer

    ''' <summary>
    ''' H[2] isotopic element
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property D As Integer

    Dim m_mass As Double?

    Public Overrides ReadOnly Property ExactMass As Double
        Get
            If m_mass Is Nothing Then
                Return MyBase.ExactMass
            Else
                Return CDbl(m_mass)
            End If
        End Get
    End Property

    Sub New(C%, H%, N%, O%, P%, S%, F%, Cl%, Br%, I%, Si%, C13%, D%, Optional TMS% = 0, Optional MEOX% = 0)
        Call Me.New(C, H, N, O, P, S, F, Cl, Br, I, Si, TMS, MEOX)

        Me.C13 = C13
        Me.D = D

        m_mass = GetExactMass(C, H, N, O, P, S, F, Cl, Br, I, Si, C13, D)
        m_formula = GetFormulaString(C, H, N, O, P, S, F, Cl, Br, I, Si, C13, D, TMS, MEOX)
    End Sub

    Sub New(C%, H%, N%, O%, P%, S%, F%, Cl%, Br%, I%, Si%, Optional TMS% = 0, Optional MEOX% = 0)
        Call MyBase.New(New Dictionary(Of String, Integer) From {
            {NameOf(C), C},
            {NameOf(H), H},
            {NameOf(N), N},
            {NameOf(O), O},
            {NameOf(P), P},
            {NameOf(S), S},
            {NameOf(F), F},
            {NameOf(Cl), Cl},
            {NameOf(Br), Br},
            {NameOf(I), I},
            {NameOf(Si), Si}
        })

        Me.TMS = TMS
        Me.MEOX = MEOX

        m_mass = FormulaCalculateUtility.GetExactMass(C, H, N, O, P, S, F, Cl, Br, I, Si)
        m_formula = FormulaCalculateUtility.GetFormulaString(C, H, N, O, P, S, F, Cl, Br, I, Si, TMS, MEOX)
    End Sub

    Sub New(C%, H%, N%, O%, P%, S%, F%, Cl%, Br%, I%, Si%,
            cLabelMass#, hLabelMass#, nLabelMass#, oLabelMass#, pLabelMass#, sLabelMass#, fLabelMass#, clLabelMass#, brLabelMass#, iLabelMass#, siLabelMass#)

        Call Me.New(C%, H%, N%, O%, P%, S%, F%, Cl%, Br%, I%, Si%)

        m_mass = GetExactMass(C, H, N, O, P, S, F, Cl, Br, I, Si, cLabelMass, hLabelMass, nLabelMass, oLabelMass, pLabelMass, sLabelMass, fLabelMass, clLabelMass, brLabelMass, iLabelMass, siLabelMass)
        m_formula = GetFormulaString(C, H, N, O, P, S, F, Cl, Br, I, Si, 0, 0)
    End Sub

    Public Sub New(elem2count As Dictionary(Of String, Integer))
        Call MyBase.New(elem2count)

        m_mass = GetExactMass(elem2count)
        m_formula = GetFormulaString(elem2count)
    End Sub

End Class
