var names = Object.keys(msms);  // 获取得到了所有的谱图的名称

$(document).ready(function () {
	var name = names[0];
	show(name);
});

function show(name) {
	var data = msms[name];
	var count = 0;	
    var mid = "";				// Pass
    var mole = name;	// Pass molecule name here!
    var chart = new Highcharts.Chart({
        chart: {
            renderTo: 'container',
            defaultSeriesType: 'column',
            zoomType: 'xy',
            margin: [50, 50, 200, 80]
        },
        title: {
            text: '' + mole
        },
        subtitle: {
            text: ""// "MID: 81528&nbsp;&nbsp;&nbsp;&nbsp;<font color='blue'><b>Insilico predicted spectra<\/b><\/font>"
        },
        credits: {
            enabled: false
        },
        xAxis: {
            min: 0,
            //	max: 200,
            title: {
                enabled: true,
                text: 'Mass (m/z)'
            },
            maxZoom: 0.1,
            tickPixelInterval: 100
        },
        yAxis: {
            min: 0,
            max: 100,
            title: {
                text: 'Intensity (%)'
            }
        },
        legend: {
            enabled: true,
            showFragments: true,
            showNeutrals: false,
            showPeaks: false,
            exclusiveSelect: true,    // Turns on exclusive radio style buttons
            dblClick: false,
            startNumber: 1, // The default legend item when page loads
            borderWidth: 1,
            layout: 'vertical',
            backgroundColor: '#FFFFFF',
            style: {
                left: '50px',
                top: '300px',
                bottom: 'auto'
            }
        },
        // Tooltip HTML
        tooltip: {
            second: true,
            neutral: false,
            borderRadius: 0,
            formatter: function () {
                var namestr;
                if (this.series.name.match(/\+/g) && !this.series.name.match("Cl"))
                    namestr = "Mode: <b><font size=\"4\">(+)</font></b> &nbsp;&nbsp;&nbsp;&nbsp; Collision Energy: ";
                else if (this.series.name.match('-'))
                    namestr = "Mode: <b><font size=\"4\">(-)</font></b> &nbsp;&nbsp;&nbsp;&nbsp; Collision Energy: ";
                if (!(this.series.name.match("10 V") || this.series.name.match("20 V") || this.series.name.match("40 V")))
                    namestr += "<b><font size=\"3\">0 V</font></b>";
                else if (this.series.name.match("10 V"))
                    namestr += "<b><font size=\"3\">10 V</font></b>";
                else if (this.series.name.match("20 V"))
                    namestr += "<b><font size=\"3\">20 V</font></b>";
                else if (this.series.name.match("40 V"))
                    namestr += "<b><font size=\"3\">40 V</font></b>";
                return '<center><br/> &nbsp;&nbsp;&nbsp;&nbsp; ' + namestr + '<br/>' + '&nbsp;&nbsp;&nbsp; m/z: <b><font size="3">' + this.x.toFixed(4) + '</font></b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Intensity: <b><font size="3">' + parseInt(Math.abs(this.y)) + ' % &nbsp;&nbsp;</font></b></center><br/>';
            },
            formatter2: function () {
                var namestr;
                if (this.series.name.match(/\+/g))
                    namestr = "Mode: (+), &nbsp;&nbsp; Collision Energy: ";
                else if (this.series.name.match('-'))
                    namestr = "Mode: (-), &nbsp;&nbsp; Collision Energy: ";

                if (!(this.series.name.match("10 V") || this.series.name.match("20 V") || this.series.name.match("40 V")))
                    namestr += "0 V, &nbsp;&nbsp; Adduct: ";
                else if (this.series.name.match("10 V"))
                    namestr += "10 V, &nbsp;&nbsp; Adduct: ";
                else if (this.series.name.match("20 V"))
                    namestr += "20 V, &nbsp;&nbsp; Adduct: ";
                else if (this.series.name.match("40 V"))
                    namestr += "40 V, &nbsp;&nbsp; Adduct: ";

                return false;
            }
        },
        plotOptions: {
            column: {
                pointPadding: 0.53,
                // pointPadding: 0.99,
                borderWidth: 0,
                shadow: false
                // borderColor: '#000000'
            }
        },
        series: data
    });
}