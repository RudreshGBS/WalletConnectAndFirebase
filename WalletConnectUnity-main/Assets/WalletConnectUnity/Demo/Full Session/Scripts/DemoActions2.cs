using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;
using Nethereum.StandardTokenEIP20.ContractDefinition;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.NEthereum;
using WalletConnectSharp.Unity;
using WalletConnectUnity.Demo.Scripts;

public class DemoActions2 : WalletConnectActions
{
    public Text resultText;
    public Text accountText;

    private int count;
    private string accountBalance;

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public string Owner { get; set; }
    }

    [FunctionOutput]
    public class BalanceOfFunctionOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)]
        public BigInteger Balance { get; set; }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        WalletConnect.ActiveSession.OnSessionDisconnect += ActiveSessionOnDisconnect;
    }

   

    private void ActiveSessionOnDisconnect(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
        foreach (var platformToggle in transform.parent.GetComponentsInChildren<PlatformToggle>(true))
        {
            platformToggle.MakeActive();
        }
    }

    public void GetBalance() 
	{

        StartCoroutine(getAccountBalance(WalletConnect.ActiveSession.Accounts[0], (balance) =>
        {
            // When the callback is called, we are just going print the balance of the account
            resultText.text = $"your account balance is  :{balance}";
            resultText.gameObject.SetActive(true);
            Debug.Log(balance);
        }));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (WalletConnect.ActiveSession.Accounts == null)
            return;
        
        accountText.text = "\nConnected to Chain " + WalletConnect.ActiveSession.ChainId + ":\n" + WalletConnect.ActiveSession.Accounts[0];
    }
    
    public async void OnClickPersonalSign()
    {
        var results = await PersonalSign("This is a test!");

        resultText.text = results;
        resultText.gameObject.SetActive(true);
    }
    
    public async void OnClickSendTransaction()
    {
        var address = WalletConnect.ActiveSession.Accounts[0];
        var transaction = new TransactionData()
        {
            data = "0x",
            from = address,
            to = address,
            gas = "21000",
            value = "0",
            chainId = 2,
        };

        var results = await SendTransaction(transaction);

        resultText.text = results;
        resultText.gameObject.SetActive(true);
    }
    
    public async void OnClickSignTransaction()
    {
        var address = WalletConnect.ActiveSession.Accounts[0];
        var transaction = new TransactionData()
        {
            data = "0x",
            from = address,
            to = address,
            gas = "21000",
            value = "0",
            chainId = 2,
            nonce = "0",
            gasPrice = "50000000000"
        };

        var results = await SignTransaction(transaction);

        resultText.text = results;
        resultText.gameObject.SetActive(true);
    }
    
    public async void OnClickSignTypedData()
    {
        var address = WalletConnect.ActiveSession.Accounts[0];

        var results = await SignTypedData(DemoSignTypedData.ExampleData, DemoSignTypedData.Eip712Domain);

        resultText.text = results;
        resultText.gameObject.SetActive(true);
    }

    public void OnClickDisconnectAndConnect()
    {
        bool shouldConnect = !WalletConnect.Instance.createNewSessionOnSessionDisconnect;
        CloseSession(shouldConnect);
    }

    public static IEnumerator getAccountBalance(string address, System.Action<decimal> callback)
    {



        //var wcProtocol = WalletConnect.Instance.Session;
        //var web3 = new Web3(wcProtocol.CreateProvider(new Uri("https://bsc-dataseed.binance.org/")));
        var balanceOfMessage = new BalanceOfFunction();
        balanceOfMessage.Owner = address;

        var queryRequest = new QueryUnityRequest<BalanceOfFunction, BalanceOfFunctionOutput>("https://bsc-dataseed.binance.org/", address);
        yield return queryRequest.Query(balanceOfMessage, "0x2ca25319e2e63719f87221d8bf3646f8f5de5ded");

        //Getting the dto response already decoded
        var dtoResult = queryRequest.Result;
        callback(Nethereum.Util.UnitConversion.Convert.FromWei(dtoResult.Balance, 9));
        //var queryHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
        //Debug.Log($"queryHandler : {queryHandler}");

        //var getBalanceRequest = queryHandler.QueryAsync<BigInteger>("0x2ca25319e2e63719f87221d8bf3646f8f5de5ded", balanceOfMessage, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        //Debug.Log("Sending Request");
        //var getBalanceRequest = web3.Eth.GetBalance.SendRequestAsync(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        //yield return getBalanceRequest;
        // Now we define a new EthGetBalanceUnityRequest and send it the testnet url where we are going to
        // check the address, in this case "https://kovan.infura.io".
        // (we get EthGetBalanceUnityRequest from the Netherum lib imported at the start)
        //var getBalanceRequest = new EthGetBalanceUnityRequest("https://mainnet.infura.io/v3/7a9a2068d92540118a37fe61533e2af5", null);
        // Then we call the method SendRequest() from the getBalanceRequest we created
        // with the address and the newest created block.
        //yield return getBalanceRequest.SendRequest(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        //Debug.Log(getBalanceRequest.IsFaulted);
        // Now we check if the request has an exception
        //if (getBalanceRequest.Exception == null)
        //{
        //    Debug.Log("No Error");
        //    // We define balance and assign the value that the getBalanceRequest gave us.
        //    var balance = getBalanceRequest.Result;
        //    Debug.Log($"result : {balance}");
        //    // Finally we execute the callback and we use the Netherum.Util.UnitConversion
        //    // to convert the balance from WEI to ETHER (that has 18 decimal places)
        //    callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 9));
        //}
        //else
        //{
        //    Debug.Log("Error");

        //    // If there was an error we just throw an exception.
        //    throw new System.InvalidOperationException("Get balance request failed");
        //}

    }
}
