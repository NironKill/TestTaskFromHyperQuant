$(document).ready(function () {
    let currencyPairs = [];

    function loadCurrencyPairs() {
        $.ajax({
            url: "/Home/GetCurrencyPairs",
            type: "GET",
            success: function (data) {
                currencyPairs = data.pairs || [];
                updateFields();
            },
            error: function () {
                alert("Failed to load currency pairs.");
            }
        });
    }

    function generateCurrencySelector(id) {
        let options = currencyPairs.map(pair => `<option value="${pair}">${pair}</option>`).join("");
        return `
            <div class="form-group">
                <label for="${id}">Currency Pair</label>
                <select id="${id}" class="form-select">
                    ${options}
                </select>
            </div>
        `;
    }

    function updateFields() {
        let operation = $("#operationType").val();
        let fieldsHtml = "";

        if (operation === "trades") {
            fieldsHtml = `
                ${generateCurrencySelector("tradePair")}
                <div class="form-group">
                    <label for="maxCount">Limit</label>
                    <input type="number" id="maxCount" class="form-control" value="100">
                </div>
            `;
        } else if (operation === "candles") {
            fieldsHtml = `
                ${generateCurrencySelector("candlePair")}
                <div class="form-group">
                    <label for="period">Period</label>
                    <select id="period" class="form-select">
                        <option value="1m">1 Minute</option>
                        <option value="5m">5 Minutes</option>
                        <option value="15m">15 Minutes</option>
                        <option value="30m">30 Minutes</option>
                        <option value="1h">1 Hour</option>
                        <option value="3h">3 Hours</option>
                        <option value="6h">6 Hours</option>
                        <option value="12h">12 Hours</option>
                        <option value="1D">1 Day</option>
                        <option value="1W">1 Week</option>
                        <option value="14D">14 Days</option>
                        <option value="1M">1 Month</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="fromDate">Start</label>
                    <input type="datetime-local" id="fromDate" class="form-control">
                    <div class="text-danger small" id="fromDateError"></div>
                </div>
                <div class="form-group">
                    <label for="toDate">End</label>
                    <input type="datetime-local" id="toDate" class="form-control">
                    <div class="text-danger small" id="toDateError"></div>
                </div>
                <div class="form-group">
                    <label for="count">Limit</label>
                    <input type="number" id="count" class="form-control" value="100">
                </div>
            `;
        } else if (operation === "ticker") {
            fieldsHtml = generateCurrencySelector("tickerPair");
        }

        $("#dynamicFields").html(fieldsHtml);
        attachValidationEvents();
    }

    function attachValidationEvents() {
        $("#fromDate, #toDate").on("change", function () {
            validateDateFields();
        });
    }

    function validateDateFields() {
        var now = new Date();
        var fromDate = $("#fromDate").val() ? new Date($("#fromDate").val()) : null;
        var toDate = $("#toDate").val() ? new Date($("#toDate").val()) : null;
        var isValid = true;

        if (fromDate && fromDate > now) {
            $("#fromDateError").text("The date cannot be in the future.");
            isValid = false;
        } else {
            $("#fromDateError").text("");
        }

        if (toDate && toDate > now) {
            $("#toDateError").text("The date cannot be in the future.");
            isValid = false;
        } else {
            $("#toDateError").text("");
        }

        if (fromDate && toDate && toDate < fromDate) {
            $("#toDateError").text("'End' date cannot be before 'Start' date.");
            isValid = false;
        }

        return isValid;
    }

    function generateTable(data, operation) {
        let tableHtml = "<table class='table table-bordered'>";
        let headers = [];
        let rows = [];

        if (operation === "ticker") {
            headers = ["Pair", "Ask Price", "Ask Size", "Bid Price", "Bid Size", "Daily Change", "Last Price", "Daily Change Relative", "High", "Low", "Volume"];
            rows.push([
                data.pair,
                data.askPrice,
                data.askSize,
                data.bidPrice,
                data.bidSize,
                data.dailyChange,
                data.lastPrice,
                data.dailyChangeRelative,
                data.high,
                data.low,
                data.volume
            ]);
        } else if (operation === "candles") {
            headers = ["Pair", "Open Time", "Open Price", "High Price", "Low Price", "Close Price", "Total Volume", "Total Price"];
            data.forEach(candle => {
                rows.push([
                    candle.pair,
                    new Date(candle.openTime).toLocaleString(),
                    candle.openPrice,
                    candle.highPrice,
                    candle.lowPrice,
                    candle.closePrice,
                    candle.totalVolume,
                    candle.totalPrice
                ]);
            });
        } else if (operation === "trades") {
            headers = ["ID", "Pair", "Price", "Amount", "Side", "Time"];
            data.forEach(trade => {
                rows.push([
                    trade.id,
                    trade.pair,
                    trade.price,
                    trade.amount,
                    trade.side,
                    new Date(trade.time).toLocaleString()
                ]);
            });
        }

        tableHtml += "<thead><tr>";
        headers.forEach(header => {
            tableHtml += `<th>${header}</th>`;
        });
        tableHtml += "</tr></thead>";

        tableHtml += "<tbody>";
        rows.forEach(row => {
            tableHtml += "<tr>";
            row.forEach(cell => {
                tableHtml += `<td>${cell}</td>`;
            });
            tableHtml += "</tr>";
        });
        tableHtml += "</tbody></table>";

        return tableHtml;
    }

    $("#operationType").change(updateFields);

    $("#tableForm").submit(function (event) {
        event.preventDefault();
        if (!validateDateFields()) return;

        let operation = $("#operationType").val();
        let requestData = {};

        if (operation === "trades") {
            requestData = {
                pair: $("#tradePair").val(),
                maxCount: parseInt($("#maxCount").val(), 10)
            };
        } else if (operation === "candles") {
            requestData = {
                pair: $("#candlePair").val(),
                period: $("#period").val(),
                count: parseInt($("#count").val(), 10)
            };
            let fromDate = $("#fromDate").val();
            let toDate = $("#toDate").val();
            if (fromDate) requestData.from = new Date(fromDate).toISOString();
            if (toDate) requestData.to = new Date(toDate).toISOString();
        } else if (operation === "ticker") {
            requestData = {
                pair: $("#tickerPair").val()
            };
        }

        $.ajax({
            url: `/Rest/${operation.charAt(0).toUpperCase() + operation.slice(1)}TableGeneration`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(requestData),
            success: function (response) {
                let tableHtml = generateTable(response.data, operation);
                $("#mainTableContainer").html(tableHtml);
                $("#tableModal").modal("hide");
            },
            error: function (xhr, status, error) {
                alert("Error fetching data: " + xhr.responseText);
            }
        });
    });

    loadCurrencyPairs();
});