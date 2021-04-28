Namespace DataObjects

    ''' <summary>
    ''' Type for Tune Method Settings
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure TuneMethodSettingType
        ''' <summary>
        ''' Tune category
        ''' </summary>
        Public Category As String

        ''' <summary>
        ''' Tune name
        ''' </summary>
        Public Name As String

        ''' <summary>
        ''' Tune value
        ''' </summary>
        Public Value As String

        ''' <summary>
        ''' Display the category, name, and value of this setting
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Format("{0,-20}  {1,-40} = {2}", If(Category, "Undefined") & ":", If(Name, String.Empty), If(Value, String.Empty))
        End Function
    End Structure
End Namespace