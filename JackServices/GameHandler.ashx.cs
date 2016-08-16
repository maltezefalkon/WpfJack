using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using Jack;

namespace JackServices
{

    public class GameHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Game game = GetGame(context);
            context.Response.ContentType = "text/plain";
            context.Response.Write(SerializeGameToText(context, game));
        }

        protected virtual Game GetGame(HttpContext context)
        {
            Game ret = null;
            string gameIDFromClient = context.Request["GameID"];
            string accessKeyId = Environment.GetEnvironmentVariable("AWS_ID");
            string secretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET");
            AmazonDynamoDBClient client = new Amazon.DynamoDBv2.AmazonDynamoDBClient(accessKeyId, secretAccessKey, Amazon.RegionEndpoint.USEast1);
            GetItemRequest req = new GetItemRequest()
            {
                TableName = "Jack",
                Key = new Dictionary<string, AttributeValue>()
            {
                { "GameID", new AttributeValue() { S = gameIDFromClient } }
            },
                ConsistentRead = true
            };
            GetItemResponse response = client.GetItem(req);
            if (response.Item.Count == 0)
            {
                ret = new Game(gameIDFromClient);
                ret.Init();
                PutItemRequest put = new PutItemRequest()
                {
                    TableName = "Jack",
                    Item = GetDictionaryForGame(ret)
                };
                client.PutItem(put);
            }
            else
            {
                IEnumerable<IEnumerable<string>> castleStacks = null;
                if (response.Item.ContainsKey("CastleStacks"))
                {
                    castleStacks = ReadStacks(response.Item["CastleStacks"].M);
                }
                IEnumerable<IEnumerable<string>> beanstalkStacks = null;
                if (response.Item.ContainsKey("BeanstalkStacks"))
                {
                    beanstalkStacks = ReadStacks(response.Item["BeanstalkStacks"].M);
                }
                IEnumerable<string> discardPile = null;
                if (response.Item.ContainsKey("DiscardPile"))
                {
                    discardPile = response.Item["DiscardPile"].L.Select(x => x.S);
                }
                ret = new Game(gameIDFromClient, castleStacks, beanstalkStacks, discardPile);
            }
            return ret;
        }

        protected virtual IEnumerable<IEnumerable<string>> ReadStacks(Dictionary<string, AttributeValue> dictionary)
        {
            List<List<string>> list = new List<List<string>>();
            if (null != dictionary)
            {
                foreach (KeyValuePair<string, AttributeValue> pair in dictionary)
                {
                    int index = Int32.Parse(pair.Key);
                    if (index >= list.Count)
                    {
                        int stop = index - list.Count + 1;
                        for (int i = 0; i <= stop; i++)
                        {
                            list.Add(new List<string>());
                        }
                    }
                    list[index].AddRange(pair.Value.L.Select(x => x.S));
                }
            }
            return list;
        }

        protected virtual string SerializeGameToText(HttpContext context, Jack.Game game)
        {
            return game.GetJson();
        }

        protected Dictionary<string, AttributeValue> GetDictionaryForGame(Game game)
        {
            Dictionary<string, AttributeValue> ret = new Dictionary<string, AttributeValue>();
            ret.Add("GameID", new AttributeValue() { S = game.ID });
            if (game.CastleStacks.SelectMany(x => x).Any())
            {
                ret.Add("CastleStacks", new AttributeValue
                {
                    M = game.CastleStacks.ToDictionary(s => s.Index.ToString(), x => new AttributeValue() { L = x.Select(y => new AttributeValue(y.Code)).ToList() })
                });
            }
            if (game.BeanstalkStacks.SelectMany(x => x).Any())
            {
                ret.Add("BeanstalkStacks", new AttributeValue
                {
                    M = game.BeanstalkStacks.ToDictionary(s => s.Name, x => new AttributeValue() { L = x.Select(y => new AttributeValue(y.Code)).ToList() })
                });
            }
            if (game.DiscardPile.Any())
            {
                ret.Add("DiscardPile", new AttributeValue
                {
                    L = new List<AttributeValue>(game.DiscardPile.Select(x => new AttributeValue(x.Code)))
                });
            }
            return ret;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}