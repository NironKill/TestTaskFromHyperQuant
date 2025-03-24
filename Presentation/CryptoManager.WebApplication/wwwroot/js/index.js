$(document).ready(async function () {
    let currencyPairs = [];
    let tradeHubConnection = null;
    let currentSubscription = null;
    let currentSubscriptionData = null;

    async function initializeSignalR() {
        tradeHubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/tradeHub")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        tradeHubConnection.on("ReceiveTrade", async (id, pair, side, price, amount, time) => {
            await updateTradeTable(id, pair, side, price, amount, time);
        });

        tradeHubConnection.on("ReceiveCandle", async (pair, open, close, high, low, volume, totalPrice, openTime) => {
            await updateCandleTable(pair, open, close, high, low, volume, totalPrice, openTime);
        });

        try {
            await tradeHubConnection.start();
            console.log("SignalR connection established");
        } catch (err) {
            console.error("SignalR connection error: ", err);
            setTimeout(initializeSignalR, 5000);
        }
    }

    function clearExistingTable() {
        $("#mainTableContainer").empty();
    }

    async function updateTradeTable(id, pair, side, price, amount, time) {
        return new Promise((resolve) => {
            requestAnimationFrame(async () => {
                const date = new Date(time).toLocaleString();
                const tableId = "tradesTable";

                if ($("#candlesTable").length) {
                    clearExistingTable();
                }

                let table = $(`#${tableId}`);
                let tableContainer = $("#mainTableContainer");

                if (table.length === 0) {
                    tableContainer.html(`
                        <div class="table-container">
                            <h3 class="mt-3">Trades</h3>
                            <table class="table table-striped table-bordered" id="${tableId}">
                                <thead class="table-dark">
                                    <tr>
                                        <th>ID</th>
                                        <th>Pair</th>
                                        <th>Side</th>
                                        <th>Amount</th>
                                        <th>Price</th>
                                        <th>Time</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr class="${side === 'buy' ? 'table-success' : 'table-danger'}">
                                        <td>${id}</td>
                                        <td>${pair}</td>
                                        <td>${side.toUpperCase()}</td>
                                        <td>${amount}</td>
                                        <td>${price}</td>
                                        <td>${date}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    `);
                } else {
                    table.find('tbody').prepend(`
                        <tr class="${side === 'buy' ? 'table-success' : 'table-danger'}">
                            <td>${id}</td>
                            <td>${pair}</td>
                            <td>${side.toUpperCase()}</td>
                            <td>${amount}</td>
                            <td>${price}</td>
                            <td>${date}</td>
                        </tr>
                    `);
                }
                resolve();
            });
        });
    }

    async function updateCandleTable(pair, open, close, high, low, volume, totalPrice, openTime) {
        return new Promise((resolve) => {
            requestAnimationFrame(async () => {
                const tableId = "candlesTable";

                if ($("#tradesTable").length) {
                    clearExistingTable();
                }

                let table = $(`#${tableId}`);
                let tableContainer = $("#mainTableContainer");
                const date = new Date(openTime).toLocaleString();

                if (table.length === 0) {
                    tableContainer.html(`
                        <div class="table-container">
                            <h3 class="mt-3">Candles</h3>
                            <table class="table table-striped table-bordered" id="${tableId}">
                                <thead class="table-dark">
                                    <tr>
                                        <th>Pair</th>
                                        <th>Open Price</th>
                                        <th>Close Price</th>
                                        <th>High Price</th>
                                        <th>Low Price</th>
                                        <th>Total Volume</th>
                                        <th>Total Price</th>
                                        <th>Open Time</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>${pair}</td>
                                        <td>${open}</td>
                                        <td>${close}</td>
                                        <td>${high}</td>
                                        <td>${low}</td>
                                        <td>${volume}</td>
                                        <td>${totalPrice}</td>
                                        <td>${date}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    `);
                } else {
                    table.find('tbody').prepend(`
                        <tr>
                            <td>${pair}</td>
                            <td>${open}</td>
                            <td>${close}</td>
                            <td>${high}</td>
                            <td>${low}</td>
                            <td>${volume}</td>
                            <td>${totalPrice}</td>
                            <td>${date}</td>
                        </tr>
                    `);
                }
                resolve();
            });
        });
    }

    async function loadCurrencyPairs() {
        try {
            const data = await $.ajax({
                url: "/Home/GetCurrencyPairs",
                type: "GET"
            });
            currencyPairs = data.pairs || [];
            await updateFields();
        } catch (error) {
            alert("Failed to load currency pairs.");
        }
    }

    async function generateCurrencySelector(id) {
        const options = currencyPairs.map(pair => `<option value="${pair}">${pair}</option>`).join("");
        return `
            <div class="form-group">
                <label for="${id}">Currency Pair</label>
                <select id="${id}" class="form-select">
                    ${options}
                </select>
            </div>
        `;
    }

    async function updateFields() {
        const operation = $("#operationType").val();
        let fieldsHtml = "";

        if (operation === "trades") {
            fieldsHtml = await generateCurrencySelector("tradePair");
        } else if (operation === "candles") {
            const selector = await generateCurrencySelector("candlePair");
            fieldsHtml = `
                ${selector}
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
            `;
        }
        $("#dynamicFields").html(fieldsHtml);
    }

    async function handleSubscription(action) {
        try {
            const operation = $("#operationType").val();
            const requestData = {};

            if (action === 'subscribe' && currentSubscription && currentSubscription !== operation) {
                await performUnsubscribe(currentSubscription, currentSubscriptionData);
            }

            if (operation === "trades") {
                requestData.Pair = $("#tradePair").val();
            } else if (operation === "candles") {
                requestData.Pair = $("#candlePair").val();
                requestData.Period = $("#period").val();
            }

            const endpoint = action === 'subscribe'
                ? `Subscribe${operation.charAt(0).toUpperCase() + operation.slice(1)}`
                : `Unsubscribe${operation.charAt(0).toUpperCase() + operation.slice(1)}`;

            const response = await $.ajax({
                url: `/Websocket/${endpoint}`,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(requestData)
            });

            if (response.success) {
                if (action === 'subscribe') {
                    currentSubscription = operation;
                    currentSubscriptionData = requestData;
                    clearExistingTable();
                } else {
                    clearExistingTable();
                    currentSubscription = null;
                    currentSubscriptionData = null;
                }
            } else {
                alert("Error: " + response.message);
            }
        } catch (error) {
            alert("Error: " + (error.responseText || error.message));
        }
    }

    async function performUnsubscribe(type, data) {
        try {
            const endpoint = `Unsubscribe${type.charAt(0).toUpperCase() + type.slice(1)}`;
            await $.ajax({
                url: `/Websocket/${endpoint}`,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(data)
            });
            clearExistingTable();
            currentSubscription = null;
            currentSubscriptionData = null;
        } catch (error) {
            console.error("Error unsubscribing:", error);
        }
    }

    await initializeSignalR();
    await loadCurrencyPairs();

    $("#operationType").change(async function () {
        if (currentSubscription) {
            await performUnsubscribe(currentSubscription, currentSubscriptionData);
        }
        await updateFields();
    });

    $("#subscribe").click(async function (e) {
        e.preventDefault();
        await handleSubscription('subscribe');
    });

    $("#unsubscribe").click(async function (e) {
        e.preventDefault();
        await handleSubscription('unsubscribe');
    });
});