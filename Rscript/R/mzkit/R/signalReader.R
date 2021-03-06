#Region "Microsoft.ROpen::36db142dfe978ae9a98aec366571e0ce, R\signalReader.R"

    # Summaries:


#End Region

#' Read of general signal pack
#'
#' @description a helper module for read sciBASIC.NET
#'    general signal netcdf4 data file.
#'
#' @param cdf the file path of the netCDF data file.
#'
#' @details then general signal data format can be found at source file module
#'    https://github.com/xieguigang/sciBASIC/blob/master/Data_science/Mathematica/SignalProcessing/Signal.IO/cdfSignalsWriter.vb
#'
readAllSignals = function(cdf, verbose = TRUE) {
  library(ncdf4);
  library(rjson);

  Imports(System.Text.RegularExpressions);

  nc <- nc_open(cdf);

  if (verbose) {
    print(paste("The file [", cdf ,"]has",nc$nvars,"variables,",nc$ndims,"dimensions and",nc$natts,"NetCDF attributes"));
  }

  signals   = ncatt_get(nc, 0)$signals;
  metaNames = fromJSON(ncatt_get(nc, 0)$metadata);
  axis      = attributes(nc$var)$names;

  if (verbose) {
    print(sprintf("loading %s signals data...", signals));
  }

  meta = lapply(sprintf("meta:%s", metaNames), function(name) {
    data = ncvar_get(nc, name);
    info = ncatt_get(nc, name);

    if (info$type == "json") {
      fromJSON(data);
    } else {
      data;
    }
  });
  names(meta) = metaNames;

  x            = ncvar_get(nc, "measure_buffer");
  y            = ncvar_get(nc, "signal_buffer");
  chunk_size   = ncvar_get(nc, "chunk_size");
  signal_guid  = fromJSON(ncvar_get(nc, "signal_guid"));
  measure_unit = fromJSON(ncvar_get(nc, "measure_unit"));

  index       = list();
  i           = 1;
  buffer_size = 1;

  for(size in chunk_size) {
    index[[i]]  = c(buffer_size, buffer_size + size - 1);
    buffer_size = buffer_size + size;
    i           = i + 1;
  }

  signals = lapply(1:length(index), function(i) {
    range    = index[[i]];
    index    = range[1]:range[2];
    measure  = x[index];
    signals  = y[index];
    metadata = lapply(meta, function(data) data[i]);

    list(
      id     = signal_guid[i],
      unit   = measure_unit[i],
      meta   = metadata,
      signal = data.frame(measure = measure, signals = signals)
    );
  });

  nc_close(nc);

  names(signals) = sapply(signals, function(i) i$id);
  signals;
}
