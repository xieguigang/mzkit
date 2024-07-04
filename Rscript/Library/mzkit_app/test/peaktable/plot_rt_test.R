require(mzkit);

setwd("\\192.168.1.254\backup3\项目以外内容\全靶测试\Serum_Inhouse\20240702_CS_RI\demo_test\pos\peaks1");

svg(file = "rt_shifts.svg") {
    plot(read.rt_shifts("rt_shifts.csv"));
}