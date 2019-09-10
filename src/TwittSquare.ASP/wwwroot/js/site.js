function slideUp(div, button) {
    $(button).on("click", function () {
        $(div).slideUp();
    });
}

function setActiveTimes(userId, hourSvg, daySvg,hourOfDayDiv, hourOfDaySvg, hourOfDayP,hourOfDayButton) {
    $.ajax({
        type: 'GET',
        url: '/api/user/'+userId+'/active_time',
        dataType: 'json',
        fail: function (response) {
        },
        success: function (json) {
            var data1 = [
                {
                    "key": "ActiveAt",
                    "bar": true,
                    "values": json.hour_group
                }
            ];
            var data2 = [
                {
                    "key": "ActiveAt",
                    "bar": true,
                    "values": json.day_group
                }
            ];
            createActiveTimeHourGraph(userId, hourSvg, data1,function(e){});
            
            var createData = function (date) {
                return [
                    {
                        "key": "ActiveAt",
                        "bar": true,
                        "values": json.hour_of_day_group[date].hour
                    }
                ]
            };
            var showHourOfDarGraph = function (date) {
                $(hourOfDayDiv).slideDown();
                var data = createData(date);
                createActiveTimeHourGraph(userId, hourOfDaySvg, data,function(e){});
                $(hourOfDayP).text(date + "の行動");
            };
            //var nowDate = json.day_group[json.day_group.length - 1].time;
            //showHourOfDarGraph(nowDate);

            createActiveTimeDayGraph(userId, daySvg, data2,
                function (e) {
                    showHourOfDarGraph(e.time);
                }
                );
            slideUp(hourOfDayDiv, hourOfDayButton);
        }
    });
}

function createActiveTimeHourGraph(userId, svg, data,onClick) {
    nv.addGraph(function () {
        var chart = nv.models.discreteBarChart()
          .margin({ top: 30, right: 60, bottom: 50, left: 70 })
          .x(function (d) { return d.time + "時" })
          .y(function (d) { return d.count })
          .color(d3.scale.category10().range());

        d3.select(svg)
          .datum(data)
          .transition().duration(500)
          .call(chart);

        nv.utils.windowResize(chart.update);
        d3.selectAll(svg + " .nv-bar").on("click", onClick);
        return chart;
    });
}

function createActiveTimeDayGraph(userId, svg, data,onClick) {
    nv.addGraph(function () {
        var chart = nv.models.discreteBarChart()
          .margin({ top: 30, right: 60, bottom: 50, left: 70 })
          .x(function (d) { return d.time })
          .y(function (d) { return d.count })
          .color(d3.scale.category10().range());

        d3.select(svg)
          .datum(data)
          .transition().duration(500)
          .call(chart);

        nv.utils.windowResize(chart.update);
        d3.selectAll(svg + " .nv-bar").on("click", onClick);
        return chart;
    });
}
