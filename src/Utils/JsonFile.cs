using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gabi.Base.Utils
{
    public static class JsonFile
    {
        /// <summary>
        ///     Charge un fichier JSON
        /// </summary>
        /// <param name="fileName">Chemin du fichier de configuration</param>
        /// <exception cref="BaseException">Erreur dans l'extraction des données de configuration</exception>
        public static T Load<T>(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    var json = File.ReadAllText(fileName);
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (SystemException e)
            {
                throw new BaseException($"Le fichier JSON {fileName} et de type {typeof(T)} n'a pas été trouvé !", e);
            }

            return default;
        }

        /// <summary>
        ///     Sauvegarde un objet dans un fichier JSON
        /// </summary>
        /// <typeparam name="T">Type de l'objet à sauvegarder</typeparam>
        /// <param name="obj">Objet à sauvegarder</param>
        /// <param name="fileName">Chemin du fichier de sauvegarde</param>
        /// <exception cref="BaseException">Erreur lors de la sauvegarde dans le fichier JSON</exception>
        public static void Save<T>(T obj, string fileName)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                throw new BaseException(
                    $"Erreur lors de la sauvegarde de l'objet dans le fichier JSON {fileName} : {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Charge un fichier JSON sous forme de JObject
        /// </summary>
        /// <param name="filePath">Chemin du fichier JSON</param>
        /// <returns>Objet JObject représentant le contenu JSON</returns>
        /// <exception cref="BaseException">Le fichier JSON n'a pas été trouvé</exception>
        public static JObject LoadJson(string filePath)
        {
            try
            {
                var jsonContent = File.ReadAllText(filePath);
                return JObject.Parse(jsonContent);
            }
            catch (Exception ex)
            {
                throw new BaseException($"Erreur lors du chargement du fichier JSON {filePath} : {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Sauvegarde un JObject dans un fichier JSON avec une indentation personnalisée
        /// </summary>
        /// <param name="obj">Objet JObject à sauvegarder</param>
        /// <param name="filePath">Chemin du fichier de sauvegarde</param>
        /// <param name="indentation">Le nombre d'indentations pour chaque niveau de l'objet JSON (par défaut 1).</param>
        /// <param name="indentChar">Le caractère utilisé pour l'indentation (par défaut une tabulation).</param>
        /// <exception cref="BaseException">Erreur dans la sauvegarde du JSON</exception>
        public static void SaveJson(JObject obj, string filePath, int indentation = 1, char indentChar = '\t')
        {
            try
            {
                // Créer un StringWriter pour écrire le JSON
                using var stringWriter = new StringWriter();
                var jsonWriter = new JsonTextWriter(stringWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = indentation,
                    IndentChar = indentChar
                };
                obj.WriteTo(jsonWriter);
                var jsonString = stringWriter.ToString();
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new BaseException($"Erreur dans la sauvegarde du JSON : {ex.Message}", ex);
            }
        }
    }
}