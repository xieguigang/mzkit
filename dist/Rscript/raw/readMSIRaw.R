library("mzkit/ThermoRaw");

const raw = open.raw("U:\项目以外内容\2021\空间代谢组\MTBLS805\20161012_Bathy_puteoserpentis_cryo_DHB_pos_FullMS_loop1_MSMS_20eV_scan_scenario_01_350-1400_5um_300x240_A33.RAW");

for(i in 1:100) {
  const scan =  raw :> read.rawscan(i);
  
  print(scan :> events);
  print(scan :> logs);
  
  pause();
}