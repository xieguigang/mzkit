#!/usr/bin/env python3
"""
generate_test_data.py — 生成药代动力学模拟测试数据
═══════════════════════════════════════════════════════
模型: 单室模型 + 一级吸收 (血管外给药 SC)
  C(t) = (F × Dose × ka) / (V × (ka - ke)) × (e^(-ke·t) - e^(-ka·t))

参数:
  Dose = 5000 µg/kg (5 mg/kg)
  F    = 0.85  (SC 生物利用度)
  V    = 300 mL/kg (表观分布容积)
  ka   = 2.0 h⁻¹ (吸收速率常数)
  ke   = 0.025 h⁻¹ (消除速率常数, t½ ≈ 27.7h)

时间点: 19个, 每个时间点 9 个生物学重复
  给药前: 0h (空白对照)
  吸收相: 0.25, 0.5, 1, 2h
  峰前后: 3, 4, 6, 8h
  消除相: 12, 24, 36, 48, 72, 96, 120, 144, 168h
"""

import json
import math
import random

# ── 固定随机种子确保可复现 ──
random.seed(42)

# ── PK 模型参数 ──
F = 0.85
Dose = 5000.0  # µg/kg
ka = 2.0       # h⁻¹
ke = 0.025     # h⁻¹
V = 300.0      # mL/kg

coeff = (F * Dose * ka) / (V * (ka - ke))

# ── 时间点 ──
time_points = [0, 0.25, 0.5, 1, 2, 3, 4, 6, 8, 12,
               24, 36, 48, 72, 96, 120, 144, 168]

n_reps = 9
cv = 0.15  # 变异系数 15% (对数正态噪声)

# ── 生成数据 ──
data = []
for t in time_points:
    if t == 0:
        # 给药前空白对照: 接近 0 的小噪声
        mean_conc = 0.0
    else:
        mean_conc = coeff * (math.exp(-ke * t) - math.exp(-ka * t))

    reps = []
    for i in range(n_reps):
        if t == 0:
            # 空白对照: 0 附近的小随机值 (模拟检测噪声)
            val = abs(random.gauss(0, 0.05))
            val = round(val, 4)
        else:
            # 对数正态噪声 (CV = 15%)
            sigma = math.sqrt(math.log(1 + cv**2))
            mu = math.log(max(mean_conc, 1e-6)) - sigma**2 / 2
            val = math.exp(random.gauss(mu, sigma))
            val = round(val, 4)
        reps.append(val)

    data.append({
        "Time": t,
        "Quantify": reps
    })

# ── 输出 JSON ──
output_path = "/home/z/my-project/PKAnalysis/test_data.json"
with open(output_path, "w", encoding="utf-8") as f:
    json.dump(data, f, indent=2, ensure_ascii=False)

# ── 打印摘要 ──
print(f"✓ 测试数据已生成: {output_path}")
print(f"  时间点数: {len(data)}")
print(f"  每点重复数: {n_reps}")
print(f"  时间范围: 0 – {max(time_points)} h")
print()
print("  理论浓度 (均值):")
for d in data:
    t = d["Time"]
    reps = d["Quantify"]
    mean_val = sum(reps) / len(reps)
    print(f"    t={t:6.2f}h  mean={mean_val:8.3f}  reps=[{', '.join(f'{r:.3f}' for r in reps)}]")
