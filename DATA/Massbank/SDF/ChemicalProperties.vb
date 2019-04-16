Namespace File

    Public Class ChemicalProperties

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

            ExactMass = Val(read("PUBCHEM_EXACT_MASS"))
            XLogP3 = Val(read("PUBCHEM_XLOGP3"))
        End Sub

        Private Shared Function getOne(data As Dictionary(Of String, String())) As Func(Of String, String)
            Return Function(key)
                       Return data.TryGetValue(key, [default]:={}).FirstOrDefault
                   End Function
        End Function
    End Class
End Namespace