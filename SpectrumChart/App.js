$(document).ready(function () {

    var count = 0;

    var mid = "81528";				// Pass
    var mole = "PA(18:2(9Z,12Z)\/22:2(13Z,16Z))";	// Pass molecule name here!
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
            text: "MID: 81528&nbsp;&nbsp;&nbsp;&nbsp;<font color='blue'><b>Insilico predicted spectra<\/b><\/font>"
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
        series: [{ name: '&nbsp;&nbsp;&nbsp;&nbsp; (+) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 10 V &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [M+H]+ &nbsp;&nbsp;', data: [{ x: 753.543, y: 100, fragment: false }, { x: 735.532, y: 7.6923076923077, fragment: false }, { x: 735.532, y: 7.6923076923077, fragment: false }, { x: 735.532, y: 0, fragment: false }, { x: 667.433, y: 0, fragment: false }, { x: 655.566, y: 15.384615384615, fragment: false }, { x: 629.418, y: 0, fragment: false }, { x: 627.402, y: 0, fragment: false }, { x: 491.313, y: 7.6923076923077, fragment: false }, { x: 473.303, y: 30.769230769231, fragment: false }, { x: 471.287, y: 7.6923076923077, fragment: false }, { x: 435.251, y: 38.461538461538, fragment: false }, { x: 419.256, y: 7.6923076923077, fragment: false }, { x: 417.24, y: 92.307692307692, fragment: false }, { x: 415.224, y: 15.384615384615, fragment: false }, { x: 413.209, y: 7.6923076923077, fragment: false }, { x: 337.31, y: 53.846153846154, fragment: false }, { x: 321.279, y: 7.6923076923077, fragment: false }, { x: 319.3, y: 53.846153846154, fragment: false }, { x: 291.305, y: 7.6923076923077, fragment: false }, { x: 281.247, y: 7.6923076923077, fragment: false }, { x: 277.289, y: 7.6923076923077, fragment: false }, { x: 263.237, y: 84.615384615385, fragment: false }, { x: 261.221, y: 7.6923076923077, fragment: false }, { x: 245.226, y: 0, fragment: false }, { x: 235.242, y: 7.6923076923077, fragment: false }, { x: 221.226, y: 7.6923076923077, fragment: false }, { x: 155.01, y: 0, fragment: false }, { x: 139.015, y: 7.6923076923077, fragment: false }, { x: 98.9842, y: 23.076923076923, fragment: false }, { x: 96.9685, y: 7.6923076923077, fragment: false }] }, { name: '&nbsp;&nbsp;&nbsp;&nbsp; (+) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 20 V &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [M+H]+ &nbsp;&nbsp;', data: [{ x: 735.532, y: 10, fragment: false }, { x: 655.566, y: 10, fragment: false }, { x: 639.571, y: 10, fragment: false }, { x: 491.313, y: 10, fragment: false }, { x: 473.303, y: 10, fragment: false }, { x: 471.287, y: 10, fragment: false }, { x: 435.251, y: 20, fragment: false }, { x: 419.256, y: 10, fragment: false }, { x: 417.24, y: 30, fragment: false }, { x: 415.224, y: 10, fragment: false }, { x: 413.209, y: 10, fragment: false }, { x: 337.31, y: 40, fragment: false }, { x: 321.279, y: 20, fragment: false }, { x: 319.3, y: 100, fragment: false }, { x: 319.263, y: 20, fragment: false }, { x: 301.289, y: 10, fragment: false }, { x: 291.305, y: 60, fragment: false }, { x: 289.289, y: 10, fragment: false }, { x: 281.247, y: 10, fragment: false }, { x: 277.289, y: 40, fragment: false }, { x: 263.237, y: 80, fragment: false }, { x: 261.221, y: 20, fragment: false }, { x: 245.226, y: 10, fragment: false }, { x: 235.242, y: 50, fragment: false }, { x: 221.226, y: 50, fragment: false }, { x: 219.211, y: 10, fragment: false }, { x: 155.01, y: 20, fragment: false }, { x: 139.015, y: 20, fragment: false }, { x: 98.9842, y: 60, fragment: false }, { x: 96.9685, y: 30, fragment: false }, { x: 80.9736, y: 10, fragment: false }] }, { name: '&nbsp;&nbsp;&nbsp;&nbsp; (+) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 40 V &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [M+H]+ &nbsp;&nbsp;', data: [{ x: 667.433, y: 7.1428571428571, fragment: false }, { x: 639.571, y: 21.428571428571, fragment: false }, { x: 627.402, y: 7.1428571428571, fragment: false }, { x: 471.287, y: 7.1428571428571, fragment: false }, { x: 419.256, y: 7.1428571428571, fragment: false }, { x: 413.209, y: 7.1428571428571, fragment: false }, { x: 337.274, y: 7.1428571428571, fragment: false }, { x: 321.279, y: 14.285714285714, fragment: false }, { x: 319.3, y: 7.1428571428571, fragment: false }, { x: 319.263, y: 14.285714285714, fragment: false }, { x: 317.284, y: 7.1428571428571, fragment: false }, { x: 301.289, y: 14.285714285714, fragment: false }, { x: 293.32, y: 7.1428571428571, fragment: false }, { x: 291.305, y: 100, fragment: false }, { x: 289.289, y: 28.571428571429, fragment: false }, { x: 277.289, y: 50, fragment: false }, { x: 275.273, y: 7.1428571428571, fragment: false }, { x: 261.221, y: 14.285714285714, fragment: false }, { x: 245.226, y: 14.285714285714, fragment: false }, { x: 235.242, y: 64.285714285714, fragment: false }, { x: 233.226, y: 14.285714285714, fragment: false }, { x: 221.226, y: 42.857142857143, fragment: false }, { x: 219.211, y: 14.285714285714, fragment: false }, { x: 207.211, y: 7.1428571428571, fragment: false }, { x: 193.195, y: 7.1428571428571, fragment: false }, { x: 179.179, y: 7.1428571428571, fragment: false }, { x: 155.01, y: 21.428571428571, fragment: false }, { x: 139.015, y: 21.428571428571, fragment: false }, { x: 98.9842, y: 35.714285714286, fragment: false }, { x: 96.9685, y: 21.428571428571, fragment: false }, { x: 80.9736, y: 14.285714285714, fragment: false }] }]
    });

});