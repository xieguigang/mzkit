require(ggplot);

let df = read.csv(relative_work("umap.csv"), row.names  = 1);

bitmap(file = relative_work("umap.png"), width = 1600, height = 1200) {
  # 假设您的数据框名为 df
  ggplot(df, aes(x = "dimension_1", y = "dimension_2"), padding = "padding: 10% 20% 10% 15%;") +
    # 散点图层：颜色按class分组，大小按size列映射
    geom_point(aes(color = "Cluster", size = "rsd"), alpha = 0.7, color = "paper") +  
    # 自定义颜色（示例调色板，可替换）
    # scale_color_brewer(palette = "Set1", name = "Class") +  
    # 调整点大小范围（根据实际数据调整）
    scale_size_continuous(range = c(9, 28), name = "Size") +  
    # 坐标轴与标题
    labs(
      title = "MSn fragment cluster",
      x = "dimension 1",
      y = "dimension 2"
    ) + theme(
      panel.grid = element_blank()
    );
}

