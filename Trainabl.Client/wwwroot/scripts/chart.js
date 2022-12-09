window.setup = (id, config) => {
    console.log("App: setup called with config" + config);
    var ctx = document.getElementById(id).getContext('2d');
    let chartStatus = Chart.getChart(ctx)
    if (chartStatus != undefined) {
        chartStatus.destroy();
    }
    Chart.defaults.color = " #ffffffb2";
    new Chart(ctx, config);
}