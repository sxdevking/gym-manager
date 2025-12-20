namespace GymManager.Domain.Data;

/// <summary>
/// Datos geograficos de Mexico (Estados y Ciudades)
/// Datos de referencia del dominio para validacion de direcciones
/// </summary>
public static class MexicoGeographyData
{
    /// <summary>
    /// Lista de estados mexicanos (32 entidades federativas)
    /// </summary>
    public static readonly string[] States = new[]
    {
        "Aguascalientes",
        "Baja California",
        "Baja California Sur",
        "Campeche",
        "Chiapas",
        "Chihuahua",
        "Ciudad de Mexico",
        "Coahuila",
        "Colima",
        "Durango",
        "Estado de Mexico",
        "Guanajuato",
        "Guerrero",
        "Hidalgo",
        "Jalisco",
        "Michoacan",
        "Morelos",
        "Nayarit",
        "Nuevo Leon",
        "Oaxaca",
        "Puebla",
        "Queretaro",
        "Quintana Roo",
        "San Luis Potosi",
        "Sinaloa",
        "Sonora",
        "Tabasco",
        "Tamaulipas",
        "Tlaxcala",
        "Veracruz",
        "Yucatan",
        "Zacatecas"
    };

    /// <summary>
    /// Diccionario de ciudades principales por estado
    /// </summary>
    public static readonly Dictionary<string, string[]> CitiesByState = new()
    {
        ["Aguascalientes"] = new[]
        {
            "Aguascalientes", "Calvillo", "Jesus Maria", "Pabellon de Arteaga",
            "Rincon de Romos", "San Francisco de los Romo", "Asientos", "Tepezala"
        },

        ["Baja California"] = new[]
        {
            "Tijuana", "Mexicali", "Ensenada", "Rosarito", "Tecate",
            "San Quintin", "San Felipe", "Valle de Guadalupe"
        },

        ["Baja California Sur"] = new[]
        {
            "La Paz", "Los Cabos", "San Jose del Cabo", "Cabo San Lucas",
            "Comondu", "Loreto", "Mulege", "Santa Rosalia"
        },

        ["Campeche"] = new[]
        {
            "Campeche", "Ciudad del Carmen", "Champoton", "Escarcega",
            "Calkini", "Hecelchakan", "Hopelchen", "Candelaria"
        },

        ["Chiapas"] = new[]
        {
            "Tuxtla Gutierrez", "San Cristobal de las Casas", "Tapachula",
            "Comitan", "Palenque", "Chiapa de Corzo", "Tonala", "Ocosingo",
            "Villaflores", "Cintalapa"
        },

        ["Chihuahua"] = new[]
        {
            "Chihuahua", "Ciudad Juarez", "Cuauhtemoc", "Delicias", "Parral",
            "Nuevo Casas Grandes", "Camargo", "Jimenez", "Ojinaga", "Madera"
        },

        ["Ciudad de Mexico"] = new[]
        {
            "Alvaro Obregon", "Azcapotzalco", "Benito Juarez", "Coyoacan",
            "Cuajimalpa", "Cuauhtemoc", "Gustavo A. Madero", "Iztacalco",
            "Iztapalapa", "Magdalena Contreras", "Miguel Hidalgo", "Milpa Alta",
            "Tlahuac", "Tlalpan", "Venustiano Carranza", "Xochimilco"
        },

        ["Coahuila"] = new[]
        {
            "Saltillo", "Torreon", "Monclova", "Piedras Negras", "Acuna",
            "Sabinas", "San Pedro", "Frontera", "Matamoros", "Parras"
        },

        ["Colima"] = new[]
        {
            "Colima", "Manzanillo", "Tecoman", "Villa de Alvarez", "Armeria",
            "Comala", "Coquimatlan", "Cuauhtemoc", "Ixtlahuacan", "Minatitlan"
        },

        ["Durango"] = new[]
        {
            "Durango", "Gomez Palacio", "Lerdo", "Santiago Papasquiaro",
            "Canatlan", "Nuevo Ideal", "El Salto", "Vicente Guerrero", "Guadalupe Victoria"
        },

        ["Estado de Mexico"] = new[]
        {
            "Toluca", "Ecatepec", "Naucalpan", "Tlalnepantla", "Nezahualcoyotl",
            "Cuautitlan Izcalli", "Atizapan", "Metepec", "Huixquilucan", "Coacalco",
            "Texcoco", "Chalco", "Ixtapaluca", "Tultitlan", "Chimalhuacan", "Valle de Bravo"
        },

        ["Guanajuato"] = new[]
        {
            "Leon", "Guanajuato", "Irapuato", "Celaya", "Salamanca", "Silao",
            "San Miguel de Allende", "Dolores Hidalgo", "San Francisco del Rincon",
            "Penjamo", "Acambaro", "Moroleon"
        },

        ["Guerrero"] = new[]
        {
            "Acapulco", "Chilpancingo", "Iguala", "Taxco", "Zihuatanejo",
            "Chilapa", "Tlapa", "Coyuca de Benitez", "Arcelia", "Petatlan", "Ixtapa"
        },

        ["Hidalgo"] = new[]
        {
            "Pachuca", "Tulancingo", "Tula", "Huejutla", "Tepeji del Rio",
            "Actopan", "Ixmiquilpan", "Tizayuca", "Apan", "Mineral de la Reforma"
        },

        ["Jalisco"] = new[]
        {
            "Guadalajara", "Zapopan", "Tlaquepaque", "Tonala", "Puerto Vallarta",
            "Tlajomulco", "Lagos de Moreno", "Tepatitlan", "Ocotlan", "Chapala",
            "Tequila", "Ameca", "Autlan", "Ciudad Guzman"
        },

        ["Michoacan"] = new[]
        {
            "Morelia", "Uruapan", "Zamora", "Lazaro Cardenas", "Apatzingan",
            "Zitacuaro", "Patzcuaro", "Sahuayo", "Hidalgo", "La Piedad", "Jacona"
        },

        ["Morelos"] = new[]
        {
            "Cuernavaca", "Jiutepec", "Cuautla", "Temixco", "Yautepec",
            "Emiliano Zapata", "Xochitepec", "Puente de Ixtla", "Jojutla", "Zacatepec"
        },

        ["Nayarit"] = new[]
        {
            "Tepic", "Bahia de Banderas", "Santiago Ixcuintla", "Compostela",
            "Tuxpan", "Tecuala", "Acaponeta", "Ixtlan del Rio", "San Blas", "Nuevo Vallarta"
        },

        ["Nuevo Leon"] = new[]
        {
            "Monterrey", "Guadalupe", "San Nicolas", "Apodaca", "General Escobedo",
            "Santa Catarina", "San Pedro Garza Garcia", "Juarez", "Garcia",
            "Cadereyta", "Linares", "Santiago"
        },

        ["Oaxaca"] = new[]
        {
            "Oaxaca", "Salina Cruz", "Juchitan", "Tuxtepec", "Huatulco",
            "Puerto Escondido", "Tehuantepec", "Miahuatlan", "Tlaxiaco", "Huajuapan", "Ixtepec"
        },

        ["Puebla"] = new[]
        {
            "Puebla", "Tehuacan", "San Martin Texmelucan", "Atlixco", "San Pedro Cholula",
            "Huauchinango", "Teziutlan", "San Andres Cholula", "Izucar de Matamoros", "Zacatlan"
        },

        ["Queretaro"] = new[]
        {
            "Queretaro", "San Juan del Rio", "Corregidora", "El Marques",
            "Tequisquiapan", "Cadereyta", "Ezequiel Montes", "Pedro Escobedo", "Colon", "Jalpan"
        },

        ["Quintana Roo"] = new[]
        {
            "Cancun", "Playa del Carmen", "Chetumal", "Cozumel", "Tulum",
            "Felipe Carrillo Puerto", "Isla Mujeres", "Puerto Morelos", "Bacalar",
            "Holbox", "Puerto Aventuras", "Akumal"
        },

        ["San Luis Potosi"] = new[]
        {
            "San Luis Potosi", "Soledad de Graciano Sanchez", "Ciudad Valles",
            "Matehuala", "Rioverde", "Tamazunchale", "Ciudad Fernandez", "Tamasopo", "Ebano"
        },

        ["Sinaloa"] = new[]
        {
            "Culiacan", "Mazatlan", "Los Mochis", "Guasave", "Guamuchil",
            "Navolato", "El Rosario", "Escuinapa", "Concordia", "Cosala"
        },

        ["Sonora"] = new[]
        {
            "Hermosillo", "Ciudad Obregon", "Nogales", "San Luis Rio Colorado",
            "Guaymas", "Navojoa", "Agua Prieta", "Caborca", "Empalme", "Puerto Penasco"
        },

        ["Tabasco"] = new[]
        {
            "Villahermosa", "Cardenas", "Comalcalco", "Huimanguillo", "Macuspana",
            "Paraiso", "Tenosique", "Cunduacan", "Centla", "Balancan"
        },

        ["Tamaulipas"] = new[]
        {
            "Reynosa", "Matamoros", "Nuevo Laredo", "Tampico", "Ciudad Victoria",
            "Ciudad Madero", "Altamira", "Rio Bravo", "Valle Hermoso", "Mante"
        },

        ["Tlaxcala"] = new[]
        {
            "Tlaxcala", "Apizaco", "Huamantla", "San Pablo del Monte", "Chiautempan",
            "Calpulalpan", "Zacatelco", "Tlaxco", "Papalotla", "Contla"
        },

        ["Veracruz"] = new[]
        {
            "Veracruz", "Xalapa", "Coatzacoalcos", "Cordoba", "Poza Rica",
            "Orizaba", "Minatitlan", "Boca del Rio", "Tuxpan", "Papantla",
            "Tierra Blanca", "San Andres Tuxtla"
        },

        ["Yucatan"] = new[]
        {
            "Merida", "Valladolid", "Tizimin", "Progreso", "Uman", "Kanasin",
            "Motul", "Ticul", "Izamal", "Tekax", "Hunucma"
        },

        ["Zacatecas"] = new[]
        {
            "Zacatecas", "Fresnillo", "Guadalupe", "Rio Grande", "Jerez",
            "Sombrerete", "Loreto", "Calera", "Jalpa", "Nochistlan"
        }
    };

    /// <summary>
    /// Obtiene las ciudades de un estado
    /// </summary>
    /// <param name="state">Nombre del estado</param>
    /// <returns>Array de ciudades o array vacio si no existe el estado</returns>
    public static string[] GetCities(string? state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return Array.Empty<string>();

        return CitiesByState.TryGetValue(state, out var cities)
            ? cities
            : Array.Empty<string>();
    }

    /// <summary>
    /// Verifica si un estado es valido
    /// </summary>
    public static bool IsValidState(string? state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return false;

        return States.Contains(state, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifica si una ciudad pertenece a un estado
    /// </summary>
    public static bool IsValidCity(string? state, string? city)
    {
        if (string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(city))
            return false;

        var cities = GetCities(state);
        return cities.Contains(city, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Busca el estado al que pertenece una ciudad
    /// </summary>
    public static string? FindStateByCity(string? city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return null;

        foreach (var kvp in CitiesByState)
        {
            if (kvp.Value.Contains(city, StringComparer.OrdinalIgnoreCase))
                return kvp.Key;
        }

        return null;
    }
}