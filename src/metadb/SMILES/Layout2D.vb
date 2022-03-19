Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports stdNum = System.Math

''' <summary>
''' methods for evaluate 2D layout of the molecule atoms
''' </summary>
Module Layout2D

    <Extension>
    Public Function AutoLayout(chemical As ChemicalFormula, Optional radius As Double = 10) As ChemicalFormula
        Dim atom As ChemicalElement = chemical.AllElements.First

        atom.coordinate = New Double() {0, 0}

        Do While True
            chemical.LayoutTarget(atom, radius, 0)
            atom = chemical _
                .AllElements _
                .Where(Function(a) a.coordinate.IsNullOrEmpty) _
                .FirstOrDefault

            If atom Is Nothing Then
                Exit Do
            End If
        Loop

        Return chemical
    End Function

    ReadOnly atomMaxCharges As Dictionary(Of String, Atom) = Atom _
        .DefaultElements _
        .ToDictionary(Function(a)
                          Return a.label
                      End Function)

    Private Function EvaluateAngleDelta(atom As ChemicalElement, bonds As ChemicalKey()) As Double
        Dim maxN As Integer = atomMaxCharges(atom.elementName).maxKeys
        Dim n = Aggregate b In bonds Into Sum(b.bond)

        If bonds.Length > maxN OrElse (n > maxN) Then
            Throw New InvalidConstraintException
        End If

        ' fix for the missing H element
        ' bonds in SMILES
        n = maxN - n
        n += bonds.Length

        Return 2 * stdNum.PI / n
    End Function

    <Extension>
    Public Sub LayoutTarget(chemical As ChemicalFormula, atom As ChemicalElement, radius As Double, alpha As Double)
        ' get number of bounds (n) of
        ' the current atom links,
        ' then we can evaluate the angle
        ' of each bond, which is 360/n.
        Dim bonds As ChemicalKey() = chemical.AllBonds _
            .Where(Function(b)
                       Return b.U Is atom OrElse b.V Is atom
                   End Function) _
            .ToArray
        Dim n As Integer = bonds.Length
        Dim angleDelta As Double = EvaluateAngleDelta(atom, bonds)

        If alpha = 0 Then
            alpha = angleDelta
        End If

        Dim center As New PointF(atom.coordinate(0), atom.coordinate(1))

        For Each bond As ChemicalKey In From b In bonds Where b.U Is atom
            Dim [next] As ChemicalElement = bond.V
            Dim layout As New PointF With {
                .X = center.X + (radius * stdNum.Cos(alpha)),
                .Y = center.Y + (radius * stdNum.Sin(alpha))
            }

            [next].coordinate = {layout.X, layout.Y}
            alpha += angleDelta

            Call chemical.LayoutTarget([next], radius, alpha)
        Next
    End Sub
End Module
