# ════════════════════════════════════════════════════════════════════
# plot_pk.R — Pharmacokinetic NCA Visualization Script
# ════════════════════════════════════════════════════════════════════
# Generates publication-quality PDF and PNG figures from NCA analysis
# output CSV files (pk_summary.csv and pk_parameters.csv).
#
# Usage:
#   Rscript plot_pk.R [summary_csv] [params_csv] [output_dir]
#
# Outputs:
#   pk_curve_linear.pdf/png   — Mean ± SD (linear scale)
#   pk_curve_log.pdf/png      — Mean ± SD (semi-log scale)
#   pk_replicates.pdf/png     — Individual replicates + mean
#   pk_terminal_fit.pdf/png   — Terminal phase λz fit
#   pk_combined.pdf/png       — All panels combined
# ════════════════════════════════════════════════════════════════════

workdir = "G:\\mzkit\\src\\mzmath\\TargetedMetabolomics\\test\\PKanalysis";

# ── Parse arguments ──
args <- commandArgs(trailingOnly = TRUE)
summary_csv <- file.path(workdir, "pk_summary.csv")
params_csv  <- file.path(workdir, "pk_parameters.csv") 
out_dir     <- file.path( workdir,"figs")

dir.create(out_dir);

# ── Load packages (install if missing) ──
required_pkgs <- c("ggplot2", "dplyr", "tidyr", "scales", "gridExtra")
for (pkg in required_pkgs) {
  if (!requireNamespace(pkg, quietly = TRUE)) {
    install.packages(pkg, repos = "https://cloud.r-project.org", quiet = TRUE)
  }
  suppressPackageStartupMessages(library(pkg, character.only = TRUE))
}

# ── Load data ──
cat("Loading data...\n")
pk_data <- read.csv(summary_csv, stringsAsFactors = FALSE)
pk_params <- read.csv(params_csv, stringsAsFactors = FALSE)

# Helper: get parameter value
get_param <- function(name) {
  row <- pk_params[pk_params$Parameter == name, ]
  if (nrow(row) == 0) return(NA)
  val <- row$Value
  # Try numeric conversion; if fails, return as string
  num_val <- suppressWarnings(as.numeric(val))
  if (is.na(num_val)) return(val)
  return(num_val)
}

# ── Prepare data ──
rep_cols <- grep("^Rep", names(pk_data), value = TRUE)
n_rep <- length(rep_cols)
cmax_val  <- get_param("Cmax")
tmax_val  <- get_param("Tmax")
half_life <- get_param("HalfLife")
lambda_z  <- get_param("LambdaZ")
term_start <- get_param("TerminalPhaseStart")
clast_val <- get_param("Clast")
tlast_val <- get_param("Tlast")

# Long format for individual replicates
pk_long <- pk_data %>%
  pivot_longer(
    cols = all_of(rep_cols),
    names_to = "Replicate",
    values_to = "Concentration",
    values_drop_na = TRUE
  ) %>%
  mutate(Replicate = gsub("Rep", "", Replicate))

# ── Color palette (publication-friendly) ──
color_main  <- "#2C5F8D"   # Deep blue
color_fill  <- "#4A90D9"   # Lighter blue for ribbon
color_log   <- "#D35400"   # Deep orange
color_fill2 <- "#E67E22"   # Lighter orange
color_rep   <- "#95A5A6"   # Gray for replicates
color_mean  <- "#C0392B"   # Red for mean line

# ════════════════════════════════════════════════════════════════════
# Plot 1: Mean ± SD (Linear Scale)
# ════════════════════════════════════════════════════════════════════
cat("Plotting linear-scale curve...\n")

p1 <- ggplot(pk_data, aes(x = Time_h, y = Mean)) +
  geom_ribbon(
    aes(ymin = pmax(Mean - SD, 0), ymax = Mean + SD),
    fill = color_fill, alpha = 0.2
  ) +
  geom_errorbar(
    aes(ymin = pmax(Mean - SD, 0), ymax = Mean + SD),
    width = max(pk_data$Time_h) * 0.015,
    color = color_main, linewidth = 0.4
  ) +
  geom_line(color = color_main, linewidth = 0.9) +
  geom_point(color = color_main, size = 2.8, shape = 16) +
  # Annotate Cmax/Tmax
  annotate("point", x = tmax_val, y = cmax_val,
           color = color_mean, size = 3, shape = 17) +
  annotate("text", x = tmax_val + max(pk_data$Time_h) * 0.03,
           y = cmax_val, label = sprintf("Cmax=%.2f\nTmax=%.1fh", cmax_val, tmax_val),
           hjust = 0, vjust = 0.5, size = 3, color = color_mean) +
  scale_x_continuous(
    breaks = sort(unique(c(0, 1, 2, 4, 8, 12, 24, 48, 72, 96, 120, 144, 168,
                           max(pk_data$Time_h)))),
    expand = c(0.01, 0.01)
  ) +
  labs(
    x = "Time (h)",
    y = "Plasma Concentration (µg/mL)",
    title = "Drug Plasma Concentration–Time Profile (Linear Scale)",
    subtitle = sprintf("Mean ± SD (n = %d per time point)", n_rep)
  ) +
  theme_bw(base_size = 12) +
  theme(
    plot.title = element_text(hjust = 0.5, face = "bold", size = 13),
    plot.subtitle = element_text(hjust = 0.5, size = 10, color = "gray30"),
    panel.grid.minor = element_blank(),
    axis.text = element_text(color = "black"),
    panel.border = element_rect(linewidth = 0.8)
  )

# ════════════════════════════════════════════════════════════════════
# Plot 2: Mean ± SD (Semi-log Scale)
# ════════════════════════════════════════════════════════════════════
cat("Plotting semi-log curve...\n")

p2 <- ggplot(pk_data, aes(x = Time_h, y = Mean)) +
  geom_ribbon(
    aes(ymin = pmax(Mean - SD, 0.001), ymax = Mean + SD),
    fill = color_fill2, alpha = 0.2
  ) +
  geom_errorbar(
    aes(ymin = pmax(Mean - SD, 0.001), ymax = Mean + SD),
    width = max(pk_data$Time_h) * 0.015,
    color = color_log, linewidth = 0.4
  ) +
  geom_line(color = color_log, linewidth = 0.9) +
  geom_point(color = color_log, size = 2.8, shape = 16) +
  # Terminal phase fit line
  {
    if (!is.na(lambda_z) && lambda_z > 0 && !is.na(term_start)) {
      # Get terminal fit intercept from last point
      fit_data <- pk_data[pk_data$Time_h >= term_start & pk_data$Mean > 0, ]
      if (nrow(fit_data) >= 2) {
        fit_lm <- lm(log(Mean) ~ Time_h, data = fit_data)
        intercept <- exp(coef(fit_lm)[1])
        t_seq <- seq(term_start, max(pk_data$Time_h) * 1.1, length.out = 100)
        fit_line <- data.frame(
          Time_h = t_seq,
          C_fit = intercept * exp(-lambda_z * t_seq)
        )
        list(
          geom_line(data = fit_line, aes(y = C_fit),
                    color = "gray40", linetype = "dashed", linewidth = 0.7),
          annotate("text", x = max(pk_data$Time_h) * 0.6,
                   y = intercept * exp(-lambda_z * max(pk_data$Time_h) * 0.6),
                   label = sprintf("λz = %.5f h⁻¹\nt½ = %.2f h\nR² = %.4f",
                                   lambda_z, half_life,
                                   get_param("RSquared")),
                   hjust = 0, vjust = 0, size = 3, color = "gray30",
                   fontface = "italic")
        )
      }
    }
  } +
  scale_y_log10(
    labels = label_number(drop0trailing = TRUE),
    expand = c(0, 0)
  ) +
  scale_x_continuous(
    breaks = sort(unique(c(0, 1, 2, 4, 8, 12, 24, 48, 72, 96, 120, 144, 168,
                           max(pk_data$Time_h)))),
    expand = c(0.01, 0.01)
  ) +
  labs(
    x = "Time (h)",
    y = "Plasma Concentration (µg/mL, log scale)",
    title = "Drug Plasma Concentration–Time Profile (Semi-log Scale)",
    subtitle = sprintf("Dashed line: terminal phase fit (λz = %.5f h⁻¹)", lambda_z)
  ) +
  theme_bw(base_size = 12) +
  theme(
    plot.title = element_text(hjust = 0.5, face = "bold", size = 13),
    plot.subtitle = element_text(hjust = 0.5, size = 10, color = "gray30"),
    panel.grid.minor = element_blank(),
    axis.text = element_text(color = "black"),
    panel.border = element_rect(linewidth = 0.8)
  )

# ════════════════════════════════════════════════════════════════════
# Plot 3: Individual Replicates + Mean Overlay
# ════════════════════════════════════════════════════════════════════
cat("Plotting individual replicates...\n")

p3 <- ggplot() +
  geom_line(
    data = pk_long,
    aes(x = Time_h, y = Concentration, group = Replicate),
    alpha = 0.25, color = color_rep, linewidth = 0.4
  ) +
  geom_point(
    data = pk_long,
    aes(x = Time_h, y = Concentration, group = Replicate),
    alpha = 0.2, color = color_rep, size = 0.8
  ) +
  geom_line(
    data = pk_data,
    aes(x = Time_h, y = Mean),
    color = color_mean, linewidth = 1.2
  ) +
  geom_point(
    data = pk_data,
    aes(x = Time_h, y = Mean),
    color = color_mean, size = 2.5, shape = 16
  ) +
  scale_x_continuous(
    breaks = sort(unique(c(0, 1, 2, 4, 8, 12, 24, 48, 72, 96, 120, 144, 168,
                           max(pk_data$Time_h)))),
    expand = c(0.01, 0.01)
  ) +
  labs(
    x = "Time (h)",
    y = "Plasma Concentration (µg/mL)",
    title = "Individual Replicate Profiles with Mean Overlay",
    subtitle = sprintf("Gray: %d individual replicates; Red: mean", n_rep)
  ) +
  theme_bw(base_size = 12) +
  theme(
    plot.title = element_text(hjust = 0.5, face = "bold", size = 13),
    plot.subtitle = element_text(hjust = 0.5, size = 10, color = "gray30"),
    panel.grid.minor = element_blank(),
    axis.text = element_text(color = "black"),
    panel.border = element_rect(linewidth = 0.8),
    legend.position = "none"
  )

# ════════════════════════════════════════════════════════════════════
# Plot 4: Terminal Phase Fit Detail
# ════════════════════════════════════════════════════════════════════
cat("Plotting terminal phase fit...\n")

if (!is.na(lambda_z) && lambda_z > 0 && !is.na(term_start)) {
  fit_data <- pk_data[pk_data$Time_h >= term_start & pk_data$Mean > 0, ]
  if (nrow(fit_data) >= 2) {
    fit_lm <- lm(log(Mean) ~ Time_h, data = fit_data)
    intercept <- exp(coef(fit_lm)[1])
    t_seq <- seq(min(fit_data$Time_h), max(pk_data$Time_h) * 1.05, length.out = 200)
    fit_line <- data.frame(
      Time_h = t_seq,
      C_fit = intercept * exp(-lambda_z * t_seq)
    )

    p4 <- ggplot() +
      geom_line(data = fit_line, aes(x = Time_h, y = C_fit),
                color = "#27AE60", linetype = "dashed", linewidth = 0.9) +
      geom_point(data = pk_data[pk_data$Mean > 0, ],
                 aes(x = Time_h, y = Mean),
                 color = color_main, size = 2.5, shape = 16) +
      geom_point(data = fit_data, aes(x = Time_h, y = Mean),
                 color = "#27AE60", size = 3.5, shape = 17) +
      scale_y_log10(labels = label_number(drop0trailing = TRUE)) +
      scale_x_continuous(
        breaks = sort(unique(c(0, 12, 24, 48, 72, 96, 120, 144, 168,
                               max(pk_data$Time_h))))
      ) +
      annotate("text", x = max(pk_data$Time_h) * 0.45,
               y = max(pk_data$Mean) * 0.5,
               label = sprintf("Terminal Phase Fit\nλz = %.5f h⁻¹\nt½ = %.2f h\nR² = %.4f\nn = %d points",
                               lambda_z, half_life,
                               get_param("RSquared"),
                               get_param("NumTerminalPoints")),
               hjust = 0, size = 3.5, color = "#27AE60",
               fontface = "bold") +
      labs(
        x = "Time (h)",
        y = "Plasma Concentration (µg/mL, log scale)",
        title = "Terminal Phase Elimination Fit",
        subtitle = "Green triangles: points used for λz estimation; dashed line: regression fit"
      ) +
      theme_bw(base_size = 12) +
      theme(
        plot.title = element_text(hjust = 0.5, face = "bold", size = 13),
        plot.subtitle = element_text(hjust = 0.5, size = 10, color = "gray30"),
        panel.grid.minor = element_blank(),
        axis.text = element_text(color = "black"),
        panel.border = element_rect(linewidth = 0.8)
      )
  }
} else {
  p4 <- ggplot() + theme_void() +
    labs(title = "Terminal phase fit not available")
}

# ════════════════════════════════════════════════════════════════════
# Save individual figures
# ════════════════════════════════════════════════════════════════════
save_fig <- function(plot, name, width, height) {
  pdf_path <- file.path(out_dir, paste0(name, ".pdf"))
  png_path <- file.path(out_dir, paste0(name, ".png"))
  ggsave(pdf_path, plot, width = width, height = height, device = cairo_pdf)
  ggsave(png_path, plot, width = width, height = height, dpi = 300, bg = "white")
  cat(sprintf("  ✓ %s.pdf / %s.png\n", name, name))
}

cat("\nSaving figures:\n")
save_fig(p1, "pk_curve_linear", 8, 6)
save_fig(p2, "pk_curve_log", 8, 6)
save_fig(p3, "pk_replicates", 8, 6)
save_fig(p4, "pk_terminal_fit", 8, 6)

# ════════════════════════════════════════════════════════════════════
# Combined figure (2×2)
# ════════════════════════════════════════════════════════════════════
cat("Creating combined figure...\n")
combined <- grid.arrange(p1, p2, p3, p4, ncol = 2, nrow = 2)

pdf_path <- file.path(out_dir, "pk_combined.pdf")
png_path <- file.path(out_dir, "pk_combined.png")
ggsave(pdf_path, combined, width = 16, height = 12, device = cairo_pdf)
ggsave(png_path, combined, width = 16, height = 12, dpi = 300, bg = "white")
cat("  ✓ pk_combined.pdf / pk_combined.png\n")

# ════════════════════════════════════════════════════════════════════
# NCA Parameter Summary Table (as figure)
# ════════════════════════════════════════════════════════════════════
cat("Creating parameter table figure...\n")

key_params_display <- pk_params %>%
  filter(Parameter %in% c("Cmax", "Tmax", "HalfLife", "AUC0_inf",
                           "AUC0_t", "AUC_Extrap_Pct", "CL", "Vd",
                           "Vss", "MRT", "LambdaZ", "RSquared",
                           "Clast", "Tlast", "Ka", "AbsorptionHalfLife")) %>%
  mutate(
    Value_num = suppressWarnings(as.numeric(Value)),
    Value_fmt = ifelse(is.na(Value_num), Value,
                       sprintf("%.4g", Value_num)),
    Display = sprintf("%s = %s %s", Parameter, Value_fmt, Unit)
  )

table_df <- pk_params %>%
  filter(Parameter %in% c("Cmax", "Tmax", "Clast", "Tlast",
                           "LambdaZ", "HalfLife", "RSquared",
                           "AUC0_t", "AUC0_inf", "AUC_Extrap_Pct",
                           "AUMC0_inf", "MRT", "CL", "Vd", "Vss",
                           "Ka", "AbsorptionHalfLife")) %>%
  mutate(
    Value_num = suppressWarnings(as.numeric(Value)),
    Value_fmt = ifelse(is.na(Value_num), Value,
                       sprintf("%.4g", Value_num))
  ) %>%
  select(Parameter, Value = Value_fmt, Unit, Description)

# Create table plot
tt <- ttheme_minimal(
  core = list(
    fg_params = list(cex = 0.9, hjust = 0, x = 0.05),
    bg_params = list(fill = c("white", "#F8F9FA"))
  ),
  colhead = list(
    fg_params = list(cex = 0.9, fontface = "bold", hjust = 0, x = 0.05),
    bg_params = list(fill = "#E8E8E8")
  )
)

table_grob <- tableGrob(table_df, rows = NULL, theme = tt)

p_table <- ggplot() +
  annotation_custom(table_grob) +
  theme_void() +
  labs(title = "NCA Parameter Summary") +
  theme(plot.title = element_text(hjust = 0.5, face = "bold", size = 14))

save_fig(p_table, "pk_parameter_table", 10, 8)

cat("\n══════════════════════════════════════════════════\n")
cat("  ✓ All PK figures generated successfully!\n")
cat("══════════════════════════════════════════════════\n")
cat("\nGenerated files:\n")
cat("  1. pk_curve_linear.pdf/png    — Linear scale PK curve\n")
cat("  2. pk_curve_log.pdf/png       — Semi-log scale PK curve\n")
cat("  3. pk_replicates.pdf/png      — Individual replicates\n")
cat("  4. pk_terminal_fit.pdf/png    — Terminal phase fit\n")
cat("  5. pk_combined.pdf/png        — Combined 2×2 figure\n")
cat("  6. pk_parameter_table.pdf/png — NCA parameter table\n")
