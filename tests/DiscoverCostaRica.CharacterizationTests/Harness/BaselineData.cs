namespace DiscoverCostaRica.CharacterizationTests.Harness;

/// <summary>
/// Reference data written with plain SQL, so it does not depend on the code under refactor.
/// Canton 1 and district 1 exist in several provinces on purpose: canton and district codes
/// are only unique inside their parent (see GEO-D2 / GEO-D3).
/// </summary>
public static class BaselineData
{
    public const string Sql = """
        INSERT INTO [Geo.Province] (Id, Name) VALUES
            (1, N'San José'), (2, N'Alajuela'), (5, N'Guanacaste');

        INSERT INTO [Geo.Canton] (Id, ProvinceId, Name) VALUES
            (1, 1, N'San José'), (2, 1, N'Escazú'),
            (1, 2, N'Alajuela'), (8, 2, N'Poás'), (10, 2, N'San Carlos'),
            (1, 5, N'Liberia');

        INSERT INTO [Geo.District] (Id, CantonId, CantonProvinceId, Name) VALUES
            (1, 1, 1, N'Carmen'), (2, 1, 1, N'Merced'),
            (1, 1, 2, N'Alajuela'), (7, 10, 2, N'La Fortuna'),
            (1, 1, 5, N'Liberia');

        INSERT INTO [Volcano.Volcano] (Id, Name, Description, ProvinceId, CantonId, DistrictId) VALUES
            (1, N'Arenal', N'Young stratovolcano near La Fortuna.', 2, 10, 7),
            (2, N'Poás', N'Active volcano with an acidic crater lake.', 2, 8, NULL),
            (3, N'Rincón de la Vieja', N'Complex volcano in Guanacaste.', 5, 1, NULL);

        INSERT INTO [Beach.Beach] (Name, Description) VALUES
            (N'Manuel Antonio', N'White sand inside a national park.'),
            (N'Tamarindo', N'Surf town on the Nicoya peninsula.');

        INSERT INTO [Culture.Tradition] (Name, Description, ImageUrl) VALUES
            (N'Día del Boyero', N'Parade of painted ox carts.', N'https://example.org/boyero.jpg'),
            (N'Fiestas de Palmares', N'January festival with tope and music.', NULL);

        INSERT INTO [Culture.Dish] (Name, Description, Ingredients, Preparation, ImageUrl) VALUES
            (N'Gallo pinto', N'Rice and beans breakfast.', N'Rice, black beans, onion, cilantro', N'Fry the rice with the beans.', N'https://example.org/gallo-pinto.jpg');
        """;
}
