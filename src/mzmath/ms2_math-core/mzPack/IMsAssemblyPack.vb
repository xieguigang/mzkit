Public Interface IMsAssemblyPack

    ''' <summary>
    ''' the source data file reference name
    ''' </summary>
    ''' <returns></returns>
    Property source As String

    ''' <summary>
    ''' get ion scan scatter data from the rawdata pack file
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="rt"></param>
    ''' <param name="mass_da"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    Function PickIonScatter(mz As Double, rt As Double,
                            Optional mass_da As Double = 0.25,
                            Optional dt As Double = 7.5) As IEnumerable(Of ms1_scan)

End Interface
