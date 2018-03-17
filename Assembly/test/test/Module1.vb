Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MSP
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML

Module Module1

    Sub populateMS2()
        Dim mzML = "D:\XT-ZWA-1.mzML"

        For Each x In mzML.PopulateMS2



            Pause()
        Next

    End Sub

    Sub Main()

        Call populateMS2()


        Dim ions = "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\ion_pair.csv".LoadCsv(Of IonPair)
        Dim ionData = LoadChromatogramList("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\test\Data20180111-L1.mzML") _
            .MRMSelector(ions) _
            .Where(Function(ion) Not ion.chromatogram Is Nothing) _
            .Select(Function(ion)
                        Return New NamedValue(Of PointF()) With {
                            .Name = ion.ion.name,
                            .Description = ion.ion.ToString,
                            .Value = ion.chromatogram.Ticks.Select(Function(tick) CType(tick, PointF)).ToArray
                        }
                    End Function) _
            .ToArray

        Pause()


        Dim testMsp = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\HMDB00008.msp").ToArray


        Dim lysoPC = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\custom-lysoPC+Hpos.msp")

        Dim meta = lysoPC.First.Comments.LipidBlastParser

        Call meta.DictionaryTable(Function(x)
                                      If x Is Nothing Then
                                          Return False
                                      ElseIf {GetType(Integer), GetType(Double), GetType(Long)}.IndexOf(x.GetType) > -1 Then
                                          If Val(x) = 0R Then
                                              Return False
                                          Else
                                              Return True
                                          End If
                                      Else
                                          Return True
                                      End If
                                  End Function).GetJson.__DEBUG_ECHO


        Pause()
    End Sub

End Module
