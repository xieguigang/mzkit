Imports BioNovoGene.Analytical.MassSpectrometry.Math.PKAnalysis
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("pk_analysis")>
Module PKAnalysis

    Sub Main()

    End Sub

    <ExportAPI("analysis")>
    Public Function analysis(<RRawVectorArgument> x As Object, dose As Double, route As String, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of DrugQuantify)(x, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim quantifyData As DrugQuantify() = pull.populates(Of DrugQuantify)(env) _
            .OrderBy(Function(a) a.Time) _
            .ToArray

        Console.WriteLine("═"c, 60)
        Console.WriteLine("  药代动力学 NCA 分析 (Non-Compartmental Analysis)")
        Console.WriteLine("═"c, 60)
        Console.WriteLine($"  数据检测 : {quantifyData.Select(Function(d) d.Time & "h").JoinBy(", ")}")
        Console.WriteLine($"  剂量     : {dose} µg/kg")
        Console.WriteLine($"  给药途径 : {route}")
        Console.WriteLine("═"c, 60)
        Console.WriteLine()

        Console.WriteLine($"✓ 已加载 {quantifyData.Length} 个时间点数据")
        Console.WriteLine($"  时间范围: {quantifyData.Min(Function(d) d.Time)} – {quantifyData.Max(Function(d) d.Time)} h")
        Console.WriteLine($"  每点重复数: {quantifyData(0).Quantify.Length}")
        Console.WriteLine()

        ' ── 2. 执行 NCA 分析 ──
        Console.WriteLine("正在执行 NCA 分析...")
        Dim pk As PKParameters = NCAAnalyzer.Analyze(quantifyData, dose, route)
        Console.WriteLine("✓ 分析完成")
        Console.WriteLine()

        ' ── 3. 导出 CSV ──
        Dim summary As TimePointSummary() = NCAAnalyzer.SummarizeTimePoints(quantifyData).ToArray
        Dim pars As NCAParameters() = NCAAnalyzer.NCAParametersTable(pk).ToArray

        ' ── 4. 打印关键参数 ──
        Console.WriteLine("─"c, 60)
        Console.WriteLine("  NCA 关键参数")
        Console.WriteLine("─"c, 60)
        Console.WriteLine($"  Cmax          = {pk.Cmax,10:F3}  (Tmax = {pk.Tmax:F1} h)")
        Console.WriteLine($"  Cmin          = {pk.Cmin,10:F3}  (Tmin = {pk.Tmin:F1} h)")
        Console.WriteLine($"  Clast         = {pk.Clast,10:F3}  (Tlast = {pk.Tlast:F1} h)")
        Console.WriteLine($"  C0 (外推)     = {pk.C0,10:F3}")
        Console.WriteLine()
        Console.WriteLine($"  λz            = {pk.LambdaZ,10:F5}  1/h")
        Console.WriteLine($"  t½            = {pk.HalfLife,10:F3}  h")
        Console.WriteLine($"  R² (末端拟合)  = {pk.RSquared,10:F4}  (adj R² = {pk.AdjRSquared:F4})")
        Console.WriteLine($"  末端点数      = {pk.NumTerminalPoints}  (起始 {pk.TerminalPhaseStart:F1} h)")
        Console.WriteLine()
        Console.WriteLine($"  AUC(0-t)      = {pk.AUC0_t,10:F2}  conc·h")
        Console.WriteLine($"  AUC(0-∞)      = {pk.AUC0_inf,10:F2}  conc·h")
        Console.WriteLine($"  外推%         = {pk.AUC_Extrap_Pct,10:F2}  %")
        Console.WriteLine($"  AUMC(0-∞)     = {pk.AUMC0_inf,10:F2}  conc·h²")
        Console.WriteLine()
        Console.WriteLine($"  MRT           = {pk.MRT,10:F3}  h")
        Console.WriteLine($"  CL            = {pk.CL,10:F4}  vol/h")
        Console.WriteLine($"  Vd            = {pk.Vd,10:F4}  vol")
        Console.WriteLine($"  Vss           = {pk.Vss,10:F4}  vol")
        Console.WriteLine()

        If pk.Ka > 0 Then
            Console.WriteLine($"  ka (残差法)   = {pk.Ka,10:F5}  1/h")
            Console.WriteLine($"  t½(吸收)      = {pk.AbsorptionHalfLife,10:F3}  h")
            Console.WriteLine()
        End If

        ' ── 5. 质量检查 ──
        Console.WriteLine("─"c, 60)
        Console.WriteLine("  质量检查")
        Console.WriteLine("─"c, 60)
        If pk.RSquared >= 0.8 Then
            Console.WriteLine($"  ✓ 末端拟合 R² = {pk.RSquared:F4} ≥ 0.8")
        Else
            Console.WriteLine($"  ⚠ 末端拟合 R² = {pk.RSquared:F4} < 0.8, 参数可靠性存疑")
        End If

        If pk.AUC_Extrap_Pct < 20 Then
            Console.WriteLine($"  ✓ 外推面积 %  = {pk.AUC_Extrap_Pct:F2}% < 20%")
        Else
            Console.WriteLine($"  ⚠ 外推面积 %  = {pk.AUC_Extrap_Pct:F2}% ≥ 20%, 需延长采样时间")
        End If

        If pk.NumTerminalPoints >= 3 Then
            Console.WriteLine($"  ✓ 末端相点数  = {pk.NumTerminalPoints} ≥ 3")
        Else
            Console.WriteLine($"  ⚠ 末端相点数  = {pk.NumTerminalPoints} < 3")
        End If

        Return New list(slot("summary") = summary, slot("nca_pars") = pars)
    End Function

End Module
