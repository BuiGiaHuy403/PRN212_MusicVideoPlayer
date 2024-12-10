using MusicPlayApp.DAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayApp.DAL.Repository
{
    public class FavoriteRepo
    {
        private const string FileName = "favoriteLists.json";
        public async Task AddFavoriteAsync(int songId, string listName)
        {
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);

            var existingFavorite = favoriteLists.FirstOrDefault(f =>  f.SongId == songId);
            if (existingFavorite != null)
            {
                throw new InvalidOperationException("This song is already in your favorite list.");
            }

            var favorite = new FavoriteList
            {
                SongId = songId,
                ListName = listName
            };

            favoriteLists.Add(favorite);
            await JsonDatabase.WriteAsync(FileName, favoriteLists);
        }

        // Kiểm tra xem bài hát đã có trong danh sách yêu thích của người dùng chưa
        public async Task<FavoriteList> GetFavoriteBySongIdAsync(int songId)
        {
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);
            return favoriteLists.FirstOrDefault(f => f.SongId == songId);
        }


        public async Task<List<int?>> GetAllFavoritesAsync()
        {
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);
            return favoriteLists
                .Select(f => f.SongId)
                .ToList();
        }

        public async Task RemoveFavoriteAsync(int songId)
        {

            // Tìm mục yêu thích dựa trên UserId và SongId
            var favoriteLists = await JsonDatabase.ReadAsync<FavoriteList>(FileName);
            var favorite = favoriteLists.FirstOrDefault(f => f.SongId == songId);

            if (favorite != null)
            {
                // Xóa mục khỏi FavoriteLists
                favoriteLists.Remove(favorite);
                await JsonDatabase.WriteAsync(FileName, favoriteLists);
                // Lưu thay đổi vào cơ sở dữ liệu
            }
        }
    }
}
