Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace HMDB

    Public Class BriefTable : Implements ICloneable

        Public Property Sample As String
        Public Property HMDB As String
        Public Property KEGG As String
        Public Property chebi As String
        Public Property CAS As String
        Public Property MW As String
        Public Property name As String
        Public Property formula As String
        Public Property disease As String()
        Public Property water_solubility As String

        <Column("concentration (children-normal)")>
        Public Property ChildrenConcentrationNormal As String()
        <Column("concentration (children-abnormal)")>
        Public Property ChildrenConcentrationAbnormal As String()
        <Column("concentration (adult-normal)")>
        Public Property AdultConcentrationNormal As String()
        <Column("concentration (adult-abnormal)")>
        Public Property AdultConcentrationAbnormal As String()
        <Column("concentration (newborn-normal)")>
        Public Property NewbornConcentrationNormal As String()
        <Column("concentration (newborn-abnormal)")>
        Public Property NewbornConcentrationAbnormal As String()

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function
    End Class
End Namespace