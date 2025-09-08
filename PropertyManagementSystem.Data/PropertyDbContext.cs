using Dapper;
using Microsoft.Data.SqlClient;
using PropertyManagementSystem.Core.Models;
using System.Data;
using System.Text;

namespace PropertyManagementSystem.Data
{
    public class PropertyDbContext
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PropertyDbContext(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            // 設定 Dapper 支援中文字元
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = false;
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            const string sql = @"
                SELECT p.*, pi.Id as ImageId, pi.PropertyId, pi.ImageUrl, pi.AltText, pi.SortOrder, pi.CreatedAt as ImageCreatedAt
                FROM Properties p
                LEFT JOIN PropertyImages pi ON p.Id = pi.PropertyId
                WHERE p.IsDeleted = 0
                ORDER BY p.CreatedAt DESC";

            using var connection = _connectionFactory.CreateConnection();
            var propertyDict = new Dictionary<int, Property>();

            await connection.QueryAsync<Property, PropertyImage, Property>(
                sql,
                (property, image) =>
                {
                    if (!propertyDict.TryGetValue(property.Id, out var propertyEntry))
                    {
                        propertyEntry = property;
                        propertyEntry.Images = new List<PropertyImage>();
                        propertyDict.Add(propertyEntry.Id, propertyEntry);
                    }

                    if (image != null)
                    {
                        propertyEntry.Images.Add(image);
                    }

                    return propertyEntry;
                },
                splitOn: "ImageId");

            return propertyDict.Values;
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            const string sql = @"
                SELECT p.*, pi.Id as ImageId, pi.PropertyId, pi.ImageUrl, pi.AltText, pi.SortOrder, pi.CreatedAt as ImageCreatedAt
                FROM Properties p
                LEFT JOIN PropertyImages pi ON p.Id = pi.PropertyId
                WHERE p.Id = @Id AND p.IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var propertyDict = new Dictionary<int, Property>();

            await connection.QueryAsync<Property, PropertyImage, Property>(
                sql,
                (property, image) =>
                {
                    if (!propertyDict.TryGetValue(property.Id, out var propertyEntry))
                    {
                        propertyEntry = property;
                        propertyEntry.Images = new List<PropertyImage>();
                        propertyDict.Add(propertyEntry.Id, propertyEntry);
                    }

                    if (image != null)
                    {
                        propertyEntry.Images.Add(image);
                    }

                    return propertyEntry;
                },
                new { Id = id },
                splitOn: "ImageId");

            return propertyDict.Values.FirstOrDefault();
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string? city = null, string? district = null,
            int? typeId = null, int? statusId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var conditions = new List<string> { "p.IsDeleted = 0" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(city))
            {
                conditions.Add("p.City LIKE @City");
                parameters.Add("City", $"%{city}%", DbType.String);
            }

            if (!string.IsNullOrWhiteSpace(district))
            {
                conditions.Add("p.District LIKE @District");
                parameters.Add("District", $"%{district}%", DbType.String);
            }

            if (typeId.HasValue)
            {
                conditions.Add("p.TypeId = @TypeId");
                parameters.Add("TypeId", typeId.Value, DbType.Int32);
            }

            if (statusId.HasValue)
            {
                conditions.Add("p.StatusId = @StatusId");
                parameters.Add("StatusId", statusId.Value, DbType.Int32);
            }

            if (minPrice.HasValue)
            {
                conditions.Add("p.Price >= @MinPrice");
                parameters.Add("MinPrice", minPrice.Value, DbType.Decimal);
            }

            if (maxPrice.HasValue)
            {
                conditions.Add("p.Price <= @MaxPrice");
                parameters.Add("MaxPrice", maxPrice.Value, DbType.Decimal);
            }

            var whereClause = string.Join(" AND ", conditions);
            var sql = $@"
                SELECT p.*, pi.Id as ImageId, pi.PropertyId, pi.ImageUrl, pi.AltText, pi.SortOrder, pi.CreatedAt as ImageCreatedAt
                FROM Properties p
                LEFT JOIN PropertyImages pi ON p.Id = pi.PropertyId
                WHERE {whereClause}
                ORDER BY p.CreatedAt DESC";

            using var connection = _connectionFactory.CreateConnection();
            var propertyDict = new Dictionary<int, Property>();

            await connection.QueryAsync<Property, PropertyImage, Property>(
                sql,
                (property, image) =>
                {
                    if (!propertyDict.TryGetValue(property.Id, out var propertyEntry))
                    {
                        propertyEntry = property;
                        propertyEntry.Images = new List<PropertyImage>();
                        propertyDict.Add(propertyEntry.Id, propertyEntry);
                    }

                    if (image != null)
                    {
                        propertyEntry.Images.Add(image);
                    }

                    return propertyEntry;
                },
                parameters,
                splitOn: "ImageId");

            return propertyDict.Values;
        }

        public async Task<int> CreatePropertyAsync(Property property)
        {
            const string propertyInsertSql = @"
                INSERT INTO Properties (Title, Description, Address, City, District, Price, Bedrooms, Bathrooms, Area, TypeId, StatusId, CreatedAt, UpdatedAt, IsDeleted)
                VALUES (@Title, @Description, @Address, @City, @District, @Price, @Bedrooms, @Bathrooms, @Area, @TypeId, @StatusId, @CreatedAt, @UpdatedAt, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            const string imageInsertSql = @"
                INSERT INTO PropertyImages (PropertyId, ImageUrl, AltText, SortOrder, CreatedAt)
                VALUES (@PropertyId, @ImageUrl, @AltText, @SortOrder, @CreatedAt);";

            using var connection = _connectionFactory.CreateConnection();
            var sqlConnection = (SqlConnection)connection;
            await sqlConnection.OpenAsync();
            using var transaction = await sqlConnection.BeginTransactionAsync();

            try
            {
                property.CreatedAt = DateTime.UtcNow;
                property.UpdatedAt = DateTime.UtcNow;
                property.IsDeleted = false;

                // 明確指定中文字串參數類型
                var parameters = new DynamicParameters();
                parameters.Add("Title", property.Title, DbType.String);
                parameters.Add("Description", property.Description, DbType.String);
                parameters.Add("Address", property.Address, DbType.String);
                parameters.Add("City", property.City, DbType.String);
                parameters.Add("District", property.District, DbType.String);
                parameters.Add("Price", property.Price, DbType.Decimal);
                parameters.Add("Bedrooms", property.Bedrooms, DbType.Int32);
                parameters.Add("Bathrooms", property.Bathrooms, DbType.Int32);
                parameters.Add("Area", property.Area, DbType.Decimal);
                parameters.Add("TypeId", property.TypeId, DbType.Int32);
                parameters.Add("StatusId", property.StatusId, DbType.Int32);
                parameters.Add("CreatedAt", property.CreatedAt, DbType.DateTime2);
                parameters.Add("UpdatedAt", property.UpdatedAt, DbType.DateTime2);
                parameters.Add("IsDeleted", property.IsDeleted, DbType.Boolean);

                var propertyId = await sqlConnection.QuerySingleAsync<int>(propertyInsertSql, parameters, transaction);
                property.Id = propertyId;

                // 插入圖片資料
                if (property.Images?.Any() == true)
                {
                    foreach (var image in property.Images)
                    {
                        var imageParameters = new DynamicParameters();
                        imageParameters.Add("PropertyId", propertyId, DbType.Int32);
                        imageParameters.Add("ImageUrl", image.ImageUrl, DbType.String);
                        imageParameters.Add("AltText", image.AltText, DbType.String);
                        imageParameters.Add("SortOrder", image.SortOrder, DbType.Int32);
                        imageParameters.Add("CreatedAt", DateTime.UtcNow, DbType.DateTime2);

                        await sqlConnection.ExecuteAsync(imageInsertSql, imageParameters, transaction);
                    }
                }

                await transaction.CommitAsync();
                return propertyId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdatePropertyAsync(Property property)
        {
            const string propertyUpdateSql = @"
                UPDATE Properties
                SET Title = @Title, Description = @Description, Address = @Address, 
                    City = @City, District = @District, Price = @Price, 
                    Bedrooms = @Bedrooms, Bathrooms = @Bathrooms, Area = @Area, 
                    TypeId = @TypeId, StatusId = @StatusId, UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND IsDeleted = 0";

            const string deleteImagesSql = @"DELETE FROM PropertyImages WHERE PropertyId = @PropertyId";
            
            const string imageInsertSql = @"
                INSERT INTO PropertyImages (PropertyId, ImageUrl, AltText, SortOrder, CreatedAt)
                VALUES (@PropertyId, @ImageUrl, @AltText, @SortOrder, @CreatedAt);";

            using var connection = _connectionFactory.CreateConnection();
            var sqlConnection = (SqlConnection)connection;
            await sqlConnection.OpenAsync();
            using var transaction = await sqlConnection.BeginTransactionAsync();

            try
            {
                property.UpdatedAt = DateTime.UtcNow;

                // 更新物件基本資料
                var parameters = new DynamicParameters();
                parameters.Add("Id", property.Id, DbType.Int32);
                parameters.Add("Title", property.Title, DbType.String);
                parameters.Add("Description", property.Description, DbType.String);
                parameters.Add("Address", property.Address, DbType.String);
                parameters.Add("City", property.City, DbType.String);
                parameters.Add("District", property.District, DbType.String);
                parameters.Add("Price", property.Price, DbType.Decimal);
                parameters.Add("Bedrooms", property.Bedrooms, DbType.Int32);
                parameters.Add("Bathrooms", property.Bathrooms, DbType.Int32);
                parameters.Add("Area", property.Area, DbType.Decimal);
                parameters.Add("TypeId", property.TypeId, DbType.Int32);
                parameters.Add("StatusId", property.StatusId, DbType.Int32);
                parameters.Add("UpdatedAt", property.UpdatedAt, DbType.DateTime2);

                var affectedRows = await sqlConnection.ExecuteAsync(propertyUpdateSql, parameters, transaction);
                
                if (affectedRows > 0)
                {
                    // 刪除現有圖片
                    await sqlConnection.ExecuteAsync(deleteImagesSql, new { PropertyId = property.Id }, transaction);

                    // 插入新圖片
                    if (property.Images?.Any() == true)
                    {
                        foreach (var image in property.Images)
                        {
                            var imageParameters = new DynamicParameters();
                            imageParameters.Add("PropertyId", property.Id, DbType.Int32);
                            imageParameters.Add("ImageUrl", image.ImageUrl, DbType.String);
                            imageParameters.Add("AltText", image.AltText, DbType.String);
                            imageParameters.Add("SortOrder", image.SortOrder, DbType.Int32);
                            imageParameters.Add("CreatedAt", DateTime.UtcNow, DbType.DateTime2);

                            await sqlConnection.ExecuteAsync(imageInsertSql, imageParameters, transaction);
                        }
                    }
                }

                await transaction.CommitAsync();
                return affectedRows > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            const string sql = @"
                UPDATE Properties
                SET IsDeleted = 1, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
            return affectedRows > 0;
        }

        public async Task<bool> PropertyExistsAsync(int id)
        {
            const string sql = "SELECT COUNT(1) FROM Properties WHERE Id = @Id AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CreatePropertyImageAsync(PropertyImage image)
        {
            const string sql = @"
                INSERT INTO PropertyImages (PropertyId, ImageUrl, AltText, SortOrder, CreatedAt)
                VALUES (@PropertyId, @ImageUrl, @AltText, @SortOrder, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _connectionFactory.CreateConnection();
            image.CreatedAt = DateTime.UtcNow;

            // 明確指定中文字串參數類型
            var parameters = new DynamicParameters();
            parameters.Add("PropertyId", image.PropertyId, DbType.Int32);
            parameters.Add("ImageUrl", image.ImageUrl, DbType.String);
            parameters.Add("AltText", image.AltText, DbType.String);
            parameters.Add("SortOrder", image.SortOrder, DbType.Int32);
            parameters.Add("CreatedAt", image.CreatedAt, DbType.DateTime2);

            var id = await connection.QuerySingleAsync<int>(sql, parameters);
            image.Id = id;
            return id;
        }

        public async Task<bool> DeletePropertyImageAsync(int id)
        {
            const string sql = "DELETE FROM PropertyImages WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
    }
}
