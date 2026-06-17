' ════════════════════════════════════════════════════════════════════
' PKDemo.vb — 药代动力学 NCA 分析演示程序
' ════════════════════════════════════════════════════════════════════
' 功能:
'   1. 从 JSON 文件加载 DrugQuantify 时间序列数据
'   2. 执行 NCA 分析
'   3. 导出时间序列汇总 CSV 和参数 CSV
'   4. 控制台打印关键参数
'
' 依赖: .NET Framework 4.8
'   - 引用 System.Web.Extensions (JavaScriptSerializer 用于 JSON 解析)
' ════════════════════════════════════════════════════════════════════

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.PKAnalysis
Imports Microsoft.VisualBasic.Serialization.JSON

Module PKDemo

    Sub Main(args As String())
        ' ── 默认参数 ──
        Dim jsonPath As String = "G:\mzkit\src\mzmath\TargetedMetabolomics\PKanalysis\test_data.json"
        Dim dose As Double = 5000.0      ' µg/kg (5 mg/kg)
        Dim route As String = "SC"       ' 皮下给药

        ' 支持命令行参数: PKDemo.exe <json> <dose> <route>
        If args.Length >= 1 Then jsonPath = args(0)
        If args.Length >= 2 Then Double.TryParse(args(1), dose)
        If args.Length >= 3 Then route = args(2)

        Console.WriteLine("═"c, 60)
        Console.WriteLine("  药代动力学 NCA 分析 (Non-Compartmental Analysis)")
        Console.WriteLine("═"c, 60)
        Console.WriteLine($"  数据文件 : {jsonPath}")
        Console.WriteLine($"  剂量     : {dose} µg/kg")
        Console.WriteLine($"  给药途径 : {route}")
        Console.WriteLine("═"c, 60)
        Console.WriteLine()

        ' ── 1. 加载 JSON 数据 ──
        If Not File.Exists(jsonPath) Then
            Console.WriteLine($"✗ 错误: 找不到文件 {jsonPath}")
            Return
        End If

        Dim json As String = File.ReadAllText(jsonPath)
        Dim dataList As DrugQuantify() = json.LoadJSON(Of DrugQuantify())
        Dim data As DrugQuantify() = dataList.ToArray()

        Console.WriteLine($"✓ 已加载 {data.Length} 个时间点数据")
        Console.WriteLine($"  时间范围: {data.Min(Function(d) d.Time)} – {data.Max(Function(d) d.Time)} h")
        Console.WriteLine($"  每点重复数: {data(0).Quantify.Length}")
        Console.WriteLine()

        ' ── 2. 执行 NCA 分析 ──
        Console.WriteLine("正在执行 NCA 分析...")
        Dim pk As PKParameters = NCAAnalyzer.Analyze(data, dose, route)
        Console.WriteLine("✓ 分析完成")
        Console.WriteLine()

        ' ── 3. 导出 CSV ──
        Dim summaryPath As String = "pk_summary.csv"
        Dim paramsPath As String = "pk_parameters.csv"
        NCAAnalyzer.ExportSummaryCSV(data, summaryPath)
        NCAAnalyzer.ExportParametersCSV(pk, paramsPath)
        Console.WriteLine($"✓ 时间序列汇总已导出: {summaryPath}")
        Console.WriteLine($"✓ NCA 参数已导出: {paramsPath}")
        Console.WriteLine()

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

        Console.WriteLine()
        Console.WriteLine("分析完成。请使用 R 脚本 plot_pk.R 生成图表。")
    End Sub

End Module
