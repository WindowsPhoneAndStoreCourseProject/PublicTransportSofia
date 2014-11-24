using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using Windows.Storage;
using SofiaPublicTransport.DataModel;

namespace SofiaPublicTransport.Utils
{
    public class SQLiteRequester
    {
        private const string dbName = "SofiaStations.db";
        private static SQLiteRequester instance;

        private SQLiteRequester()
        { 
        }

        public static SQLiteRequester Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteRequester();
                }

                return instance;
            }
        }

        //  protected async override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    // Create Db if not exist
        //    bool dbExists = await CheckDbAsync(dbName);
        //    if (!dbExists)
        //    {
        //        await CreateDatabaseAsync();
        //        await AddArticlesAsync();
        //    }
            
        //    // Get Articles
        //    SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
        //    var query = conn.Table<Article>();
        //    articles = await query.ToListAsync();

        //    // Show users
        //    ArticleList.ItemsSource = articles;
        //}

        #region SQLite utils
        private async Task<bool> CheckDbAsync(string dbName)
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(dbName);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        private async Task CreateDatabaseAsync()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            await conn.CreateTableAsync<StationDataModel>();
        }

        public async Task AddFavouriteStationAsync(StationDataModel favStation)
        {
                // Add rows to the Article Table
                bool dbExists = await CheckDbAsync(dbName);
                if (!dbExists)
                {
                    await CreateDatabaseAsync();
                }

                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
                await conn.InsertAsync(favStation);
        }



        public async Task<bool> IsStationInFavouritesAsync(string code)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);

            AsyncTableQuery<StationDataModel> query = conn.Table<StationDataModel>().Where(x => x.Code == code);
            List<StationDataModel> result = await query.ToListAsync();
            if(result == null || result.Count == 0)
            {
                return false;
            }

            return true;
        }

        //    var otherArticles = await conn.QueryAsync<Article>(
        //        "SELECT Content FROM Articles WHERE Title = ?", new object[] { "Hackers, Creed" });
        //    foreach (var article in otherArticles)
        //    {
        //        // ...
        //    }
        //}

        //private async Task UpdateArticleTitleAsync(string oldTitle, string newTitle)
        //{
        //    SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);

        //    // Retrieve Article
        //    var article = await conn.Table<Article>()
        //        .Where(x => x.Title == oldTitle).FirstOrDefaultAsync();
        //    if (article != null)
        //    {
        //        // Modify Article
        //        article.Title = newTitle;

        //        // Update record
        //        await conn.UpdateAsync(article);
        //    }
        //}

        public async Task<List<StationDataModel>> GetAllStationsFromFavourites()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            var stations = new List<StationDataModel>();
            var dbExists = await this.CheckDbAsync(dbName);
            if (dbExists)
            {
                stations = await conn.Table<StationDataModel>().ToListAsync();
            }

            return stations;
        }

        public async Task DeleteFavouriteStationAsync(string code)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);

            var station = await conn.Table<StationDataModel>().Where(x => x.Code == code).FirstOrDefaultAsync();
            if (station != null)
            {
                await conn.DeleteAsync(station);
            }
        }

        public async Task DropFavouriteStationsTableAsync()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            await conn.DropTableAsync<StationDataModel>();
        }

        #endregion SQLite utils
    }
}
