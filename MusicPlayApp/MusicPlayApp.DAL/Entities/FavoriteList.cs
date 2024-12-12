using System;
using System.Collections.Generic;
namespace MusicPlayApp.DAL.Entities;

public partial class FavoriteList
{
    public int FavoriteListId { get; set; }

    public int? SongId { get; set; }

    public string ListName { get; set;}

    public virtual Song Song { get; set; }

}
