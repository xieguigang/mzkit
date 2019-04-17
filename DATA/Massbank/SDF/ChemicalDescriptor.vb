Namespace File

    ''' <summary>
    ''' Chemical descriptor
    ''' </summary>
    Public Class ChemicalDescriptor

        ''' <summary>
        ''' Computed Octanol/Water Partition Coefficient
        ''' </summary>
        ''' <returns></returns>
        Public Property XLogP3 As Double

        ''' <summary>
        ''' Hydrogen Bond Donor Count
        ''' </summary>
        ''' <returns></returns>
        Public Property HydrogenDonors As Integer
        ''' <summary>
        ''' Hydrogen Bond Acceptor count
        ''' </summary>
        ''' <returns></returns>
        Public Property HydrogenAcceptor As Integer
        ''' <summary>
        ''' Rotatable Bond Count
        ''' </summary>
        ''' <returns></returns>
        Public Property RotatableBonds As Integer
        Public Property ExactMass As Double
        Public Property TopologicalPolarSurfaceArea As Double
        Public Property HeavyAtoms As Integer
        Public Property FormalCharge As Integer
        Public Property Complexity As Integer

        Sub New(data As Dictionary(Of String, String()))
            Dim read = getOne(data)

            On Error Resume Next

            ExactMass = Double.Parse(read("PUBCHEM_EXACT_MASS"))
            XLogP3 = Double.Parse(read("PUBCHEM_XLOGP3"))
            FormalCharge = Integer.Parse(read("PUBCHEM_TOTAL_CHARGE"))
            TopologicalPolarSurfaceArea = Double.Parse(read("PUBCHEM_CACTVS_TPSA"))
            HydrogenAcceptor = Integer.Parse(read("PUBCHEM_CACTVS_HBOND_ACCEPTOR"))
            HydrogenDonors = Integer.Parse(read("PUBCHEM_CACTVS_HBOND_DONOR"))
            RotatableBonds = Integer.Parse(read("PUBCHEM_CACTVS_ROTATABLE_BOND"))
            HeavyAtoms = Integer.Parse(read("PUBCHEM_HEAVY_ATOM_COUNT"))
            Complexity = Integer.Parse(read("PUBCHEM_CACTVS_COMPLEXITY"))
        End Sub

        Sub New()
        End Sub

        Private Shared Function getOne(data As Dictionary(Of String, String())) As Func(Of String, String)
            Return Function(key)
                       Return data.TryGetValue(key, [default]:={CStr(1 / 0)}).FirstOrDefault
                   End Function
        End Function
    End Class
End Namespace