using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace Gabi.Base
{
    /// <summary>
    ///     Classe de configuration de la base de données.
    /// </summary>
    public class DatabaseConfig : INotifyPropertyChanged
    {
        private string _connectionString;
        private string _database;
        private string _login;
        private string _password;
        private int? _port;
        private string _server;

        /// <summary>
        ///     Obtient ou définit le nom de la base de données.
        /// </summary>
        public string Database
        {
            get => _database;
            set => SetField(ref _database, value);
        }

        /// <summary>
        ///     Obtient ou définit le nom d'utilisateur pour la connexion à la base de données.
        /// </summary>
        public string Login
        {
            get => _login;
            set => SetField(ref _login, value);
        }

        /// <summary>
        ///     Obtient ou définit le mot de passe pour la connexion à la base de données.
        /// </summary>
        public string Password
        {
            set => SetField(ref _password, value);
        }

        /// <summary>
        ///     Obtient ou définit le nom du serveur de base de données.
        /// </summary>
        public string Server
        {
            get => _server;
            set => SetField(ref _server, value);
        }

        /// <summary>
        ///     Obtient ou définit le port pour la connexion à la base de données (optionnel).
        /// </summary>
        public int? Port
        {
            get => _port;
            set => SetField(ref _port, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Obtient la chaîne de connexion SQL basée sur les propriétés de la classe.
        /// </summary>
        /// <returns>La chaîne de connexion SQL.</returns>
        private string GetConnectionString()
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = Port.HasValue ? $"{Server},{Port}" : Server,
                InitialCatalog = Database,
                UserID = Login,
                Password = _password
            }.ToString();
        }

        /// <summary>
        ///     Crée une nouvelle connexion SQL basée sur la chaîne de connexion.
        /// </summary>
        /// <returns>Un objet SqlConnection.</returns>
        public SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                _connectionString = GetConnectionString();
            return new SqlConnection(_connectionString);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            _connectionString = GetConnectionString();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}