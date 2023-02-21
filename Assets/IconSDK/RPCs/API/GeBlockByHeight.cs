using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;


namespace IconSDK.RPCs
{
    using Types;
    using Extensions;
    using Blockchain;

    public class GetBlockByHeightRequestMessage : RPCRequestMessage<GetBlockByHeightRequestMessage.Parameter>
    {
        public class Parameter
        {
            [JsonProperty]
            public readonly string Height;

            public Parameter(BigInteger height)
            {
                Height = height.ToHex0x();
            }
        }
        public GetBlockByHeightRequestMessage(BigInteger height)
        : base("icx_getBlockByHeight", new Parameter(height))
        {

        }
    }

    public class GetBlockByHeightResponseMessage : RPCResponseMessage<Dictionary<string, object>>
    {

    }

    public class GetBlockByHeight : RPC<GetBlockByHeightRequestMessage, GetBlockByHeightResponseMessage>
    {
        public GetBlockByHeight(string url) : base(url)
        {

        }

        public async UniTask<Block> Invoke(BigInteger height)
        {
            var request = new GetBlockByHeightRequestMessage(height);
            var response = await Invoke(request);
            var bs = new BlockSerializer();
            return  bs.Deserialize(response.Result);
        }

        public static new Func<BigInteger, UniTask<Block>> Create(string url)
        {
            return new GetBlockByHeight(url).Invoke;
        }
    }
}
