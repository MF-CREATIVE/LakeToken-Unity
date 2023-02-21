using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Numerics;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace IconSDK.RPCs
{
    using Types;
    using Extensions;
    using Blockchain;

    public class GetLastBlockRequestMessage : RPCRequestMessage
    {
        public GetLastBlockRequestMessage()
        : base("icx_getLastBlock")
        {

        }
    }

    public class GetLastBlockResponseMessage : RPCResponseMessage<Dictionary<string, object>>
    {

    }

    public class GetLastBlock : RPC<GetLastBlockRequestMessage, GetLastBlockResponseMessage>
    {
        public GetLastBlock(string url) : base(url)
        {

        }

        public async UniTask<Block> Invoke()
        {
            Debug.Log("i am called");
            var request = new GetLastBlockRequestMessage();
            Debug.Log("bich");
            var response = await Invoke(request);
            Debug.Log("i am called");
            var bs = new BlockSerializer();
            Debug.Log("over");
            return  bs.Deserialize(response.Result);
        }

        public static new Func<UniTask<Block>> Create(string url)
        {
            return new GetLastBlock(url).Invoke;
        }
    }
}
