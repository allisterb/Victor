namespace Victor.CUI.PM.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class BoardsQueryResult
    {
        [JsonProperty("data")]
        public BoardsQueryResultData Data { get; set; }

        [JsonProperty("account_id")]
        public long AccountId { get; set; }
    }

    public partial class BoardsQueryResultData
    {
        [JsonProperty("boards")]
        public List<Board> Boards { get; set; }
    }

    public partial class Board
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("columns")]
        public List<Column> Columns { get; set; }

        [JsonProperty("groups")]
        public List<Group> Groups { get; set; }
    }

    public partial class Column
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Group
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
