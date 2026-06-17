# 药代动力学 NCA 分析工具包 / PK NCA Analysis Toolkit

## 文件清单 / Files

| 文件 | 说明 |
|------|------|
| `PKAnalysis.vb` | VB.NET NCA 分析核心模块（含数据类、分析器、CSV 导出） |
| `PKDemo.vb` | 演示控制台程序（加载 JSON → NCA 分析 → 导出 CSV） |
| `plot_pk.R` | R 绘图脚本（生成 PDF + PNG 论文插图） |
| `test_data.json` | 模拟测试数据（18 时间点 × 9 生物学重复） |
| `generate_test_data.py` | 测试数据生成脚本（Python，可修改参数重新生成） |

## 数据结构 / Data Structure

```vb
Public Class DrugQuantify
    Public Property Time As Double       ' 采样时间点 (h)
    Public Property Quantify As Double() ' 该时间点多个生物学重复的浓度值
End Class
```

JSON 格式：
```json
[
  {"Time": 0.0, "Quantify": [0.01, 0.02, ...]},
  {"Time": 0.25, "Quantify": [5.59, 5.69, ...]},
  ...
]
```

## 使用流程 / Workflow

### 1. VB.NET 分析 (Visual Studio)

1. 创建 .NET Framework 4.8 控制台项目
2. 添加引用：`System.Web.Extensions`（用于 JSON 反序列化）
3. 将 `PKAnalysis.vb` 和 `PKDemo.vb` 加入项目
4. 编译运行：
   ```
   PKDemo.exe test_data.json 5000 SC
   ```
5. 输出文件：
   - `pk_summary.csv` — 时间序列汇总（均值/SD/SEM/各重复值）
   - `pk_parameters.csv` — NCA 全参数表

### 2. R 绘图

```bash
Rscript plot_pk.R pk_summary.csv pk_parameters.csv .
```

输出文件（PDF + PNG 各一份）：
| 文件 | 内容 |
|------|------|
| `pk_curve_linear.pdf/png` | 线性尺度药时曲线（Mean ± SD） |
| `pk_curve_log.pdf/png` | 半对数药时曲线 + 末端相拟合线 |
| `pk_replicates.pdf/png` | 个体重复曲线 + 均值叠加 |
| `pk_terminal_fit.pdf/png` | 末端消除相拟合细节 |
| `pk_combined.pdf/png` | 2×2 组合图 |
| `pk_parameter_table.pdf/png` | NCA 参数汇总表 |

## 计算的 NCA 参数 / NCA Parameters

| 参数 | 说明 | 计算方法 |
|------|------|----------|
| Cmax, Tmax | 峰浓度及达峰时间 | 实测均值最大值 |
| Cmin, Tmin | 谷浓度及达谷时间 | 实测均值最小值 |
| Clast, Tlast | 末点可测浓度及时间 | 最后高于0的浓度 |
| C0 | t=0 外推浓度 | IV给药对数外推 |
| λz | 末端消除速率常数 | 末端相 ln(C) vs t 线性回归 |
| t½ | 末端半衰期 | ln2 / λz |
| R², AdjR² | 末端拟合优度 | 线性回归统计量 |
| AUC(0-t) | 0→Tlast 药时曲线下面积 | 线性梯形法 |
| AUC(0-∞) | 0→∞ 药时曲线下面积 | AUC(0-t) + Clast/λz |
| AUC_Extrap_% | 外推面积百分比 | 外推/AUC(0-∞)×100% |
| AUMC(0-t), AUMC(0-∞) | 一阶矩曲线下面积 | 梯形法 |
| MRT | 平均驻留时间 | AUMC(0-∞)/AUC(0-∞) |
| CL | 清除率 | Dose/AUC(0-∞) |
| Vd | 表观分布容积 | CL/λz |
| Vss | 稳态分布容积 | MRT × CL |
| ka | 吸收速率常数 | 残差法（仅血管外给药） |
| t½(abs) | 吸收半衰期 | ln2/ka |

## 测试数据说明 / Test Data

模拟参数（单室模型 + 一级吸收，SC 给药）：
- Dose = 5000 µg/kg (5 mg/kg)
- F = 0.85, V = 300 mL/kg
- ka = 2.0 h⁻¹, ke = 0.025 h⁻¹ (t½ ≈ 27.7h)

时间点设计（18 点 × 9 重复）：
- 给药前：0h（空白对照）
- 吸收相：0.25, 0.5, 1, 2h
- 峰前后：3, 4, 6, 8h
- 消除相：12, 24, 36, 48, 72, 96, 120, 144, 168h
