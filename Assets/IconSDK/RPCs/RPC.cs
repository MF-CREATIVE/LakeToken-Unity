using System;
using System.Net.Http;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;


namespace IconSDK.RPCs
{
    using Extensions;
    using Types;

    public class RPC<TRPCRequestMessage, TRPCResponseMessage> : MonoBehaviour
        where TRPCRequestMessage : RPCRequestMessage
        where TRPCResponseMessage : RPCResponseMessage
        
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new JsonConverter[]
            {
                new BigIntegerConverter(),
                new DictionaryConverter(),
                new BoolConverter(),
                new BoolNullableConverter(),
                new BytesConverter<Bytes>(),
                new BytesConverter<Hash32>(),
                new BytesConverter<ExternalAddress>(),
                new BytesConverter<ContractAddress>(),
                new BytesConverter<Signature>(),
            }
        };

        public static Func<TRPCRequestMessage, UniTask<TRPCResponseMessage>> Create(string url)
        {
            return new RPC<TRPCRequestMessage, TRPCResponseMessage>(url).Invoke;
        }

        public readonly string URL;

        public RPC(string url)
        {
            URL = url;
        }

        public async UniTask<TRPCResponseMessage> Invoke(TRPCRequestMessage requestMessage)
        {
            
            
                string message = JsonConvert.SerializeObject(requestMessage, _settings);
                var abc = new StringContent(
                        message,
                        Encoding.UTF8,
                        "application/json"
                    );
                string myContent = await abc.ReadAsStringAsync();

                Debug.Log("in the middle");

            /**  using (var result = await httpClient.PostAsync(
                  URL,
                  new StringContent(
                      message,
                      Encoding.UTF8,
                      "application/json"
                  )**/
            Debug.Log(message);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
          
           
            UnityWebRequest request = new UnityWebRequest(URL);
            request.SetRequestHeader("Content-Type", "application/json");
            request.method = "POST";
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();


            request.SendWebRequest();
            while (!request.isDone)
                {
                Debug.Log("request not done");
                    await UniTask.Yield();
                }
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    throw RPCException.Create(401, request.error);
                }
                else
                {
                Debug.Log(request.downloadHandler.text);
                    var responseMessage = JsonConvert.DeserializeObject<TRPCResponseMessage>(request.downloadHandler.text, _settings);
                    return responseMessage;
                }

           /*     StartCoroutine(postRequest(URL, message, (result) =>
                  {

                      var responseMessage = JsonConvert.DeserializeObject<TRPCResponseMessage>(result, _settings);
                      if (!responseMessage.IsSuccess)
                          throw RPCException.Create(responseMessage.Error.Code, responseMessage.Error.Message);
                      //return responseMessage;
                  }
            ));*/
                    Debug.Log("in the middle");

            
        }

        IEnumerator postRequest(string url, string message, Action<string> result)
        { 

            UnityWebRequest request = UnityWebRequest.Post(URL, message);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.ConnectionError){
                Debug.Log("Error occured");
                result(request.error);
            }
            else{
                result(request.downloadHandler.text);
             }

        }
    }
}