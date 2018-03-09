window.addEventListener('load', function () {

    if (typeof web3 !== 'undefined') {
        startApp(web3);
    } else {
        alert("Install MetaMask");
    }
});

const etherValue = web3.toWei(0.2, 'ether');
var address = '0xf04057D9E0A5922AB31109152fAB3942593A8B96'
function startApp(web3) {
    const eth = new Eth(web3.currentProvider)
    const token = eth.contract(abi).at(contract_address);
    console.log(token);
    listenForClicks(token, web3)
}

function listenForClicks(miniToken, web3) {
    web3.eth.getAccounts(function (err, accounts) { console.log(accounts); address = accounts.toString(); })

    $("#advert-form").submit(function (event) {
        let transactionHash = $("#TransactionHash").val();
        if (transactionHash == "" || transactionHash == null) {
            event.preventDefault();

            miniToken.transfer({ from: address, gas: 3000000, value: 20000000000000000 })
                .then(function (txHash) {
                    console.log('Transaction sent')
                    console.dir(txHash)
                    $("#TransactionHash").val(txHash);
                    waitForTxToBeMined(txHash)
                })
                .catch(console.error)
        }
    });
}

async function waitForTxToBeMined(txHash) {
        try {
            await web3.eth.getTransactionReceipt(txHash, indicateSuccess)
        } catch (err) {
            return indicateFailure(err)
        }
}

function indicateFailure(error) {
    console.log(error);
}

function indicateSuccess() {
    $("#advert-form").submit();
}