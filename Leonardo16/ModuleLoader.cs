using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Splat;

namespace Leonardo16
{
    public static class ModuleLoader
    {
        private static bool _isLoginLoaded = false;
        private static bool _isConnectionLoaded = false;
        private static bool _isMenuLoaded = false;
        private static bool _isSociLoaded = false;

        public static void EnsureLoginModuleLoaded()
        {
            if (_isLoginLoaded) return;

            try
            {
                // 1. Determina il percorso della DLL nella cartella dell'applicazione
                string assemblyPath = Path.Combine(AppContext.BaseDirectory, "Login.dll");

                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"Impossibile trovare il modulo di login: {assemblyPath}");
                }

                // 2. Carica l'Assembly in memoria dinamicamente
                Assembly loginAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

                // 3. Cerca la classe di configurazione del modulo (es. LoginModuleInitializer)
                Type initializerType = loginAssembly.GetType("Login.LoginModuleInitializer");
                if (initializerType != null)
                {
                    // Invocazione del metodo di registrazione delle dipendenze (Splat)
                    MethodInfo initMethod = initializerType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
                    initMethod?.Invoke(null, null);
                }

                _isLoginLoaded = true;
                System.Diagnostics.Debug.WriteLine("***** [MODULE] Modulo Login caricato in memoria con successo *****");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> [ERROR] Errore critico nel caricamento del modulo Login: {ex.Message}");
                throw;
            }
        }
        public static void EnsureConnectionModuleLoaded()
        {
            if (_isConnectionLoaded) return;

            try
            {
                // 1. Determina il percorso della DLL nella cartella dell'applicazione
                string assemblyPath = Path.Combine(AppContext.BaseDirectory, "Connection.dll");

                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"Impossibile trovare il modulo di connessione: {assemblyPath}");
                }

                // 2. Carica l'Assembly in memoria dinamicamente
                Assembly connectionAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                // 3. Cerca la classe di configurazione del modulo (es. ConnectionModuleInitializer)
                Type initializerType = connectionAssembly.GetType("Connection.ConnectionModuleInitializer");
                if (initializerType != null)
                {
                    // Invocazione del metodo di registrazione delle dipendenze (Splat)
                    MethodInfo initMethod = initializerType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
                    initMethod?.Invoke(null, null);
                }

                _isConnectionLoaded = true;
                System.Diagnostics.Debug.WriteLine("***** [MODULE] Modulo Connection caricato in memoria con successo *****");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> [ERROR] Errore critico nel caricamento del modulo Connection: {ex.Message}");
                throw;
            }
        }

        public static void EnsureMenuModuleLoaded()
        {
            if (_isMenuLoaded) return;

            try
            {
                // 1. Determina il percorso della DLL nella cartella dell'applicazione
                string assemblyPath = Path.Combine(AppContext.BaseDirectory, "Menu.dll");

                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"Impossibile trovare il modulo di menu: {assemblyPath}");
                }

                // 2. Carica l'Assembly in memoria dinamicamente
                Assembly menuAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                // 3. Cerca la classe di configurazione del modulo (es. MenuModuleInitializer)
                Type initializerType = menuAssembly.GetType("Menu.MenuModuleInitializer");
                if (initializerType != null)
                {
                    // Invocazione del metodo di registrazione delle dipendenze (Splat)
                    MethodInfo initMethod = initializerType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
                    initMethod?.Invoke(null, null);
                }

                _isMenuLoaded = true;
                System.Diagnostics.Debug.WriteLine("***** [MODULE] Modulo Menu caricato in memoria con successo *****");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> [ERROR] Errore critico nel caricamento del modulo Menu: {ex.Message}");
                throw;
            }
        }

        public static void EnsureSociModuleLoaded()
        {
            if (_isSociLoaded) return;

            try
            {
                // 1. Determina il percorso della DLL nella cartella dell'applicazione
                string assemblyPath = Path.Combine(AppContext.BaseDirectory, "Soci.dll");

                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"Impossibile trovare il modulo di menu: {assemblyPath}");
                }

                // 2. Carica l'Assembly in memoria dinamicamente
                Assembly menuAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                // 3. Cerca la classe di configurazione del modulo (es. MenuModuleInitializer)
                Type initializerType = menuAssembly.GetType("Soci.SociModuleInitializer");
                if (initializerType != null)
                {
                    // Invocazione del metodo di registrazione delle dipendenze (Splat)
                    MethodInfo initMethod = initializerType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
                    initMethod?.Invoke(null, null);
                }

                _isSociLoaded = true;
                System.Diagnostics.Debug.WriteLine("***** [MODULE] Modulo Soci caricato in memoria con successo *****");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> [ERROR] Errore critico nel caricamento del modulo Soci: {ex.Message}");
                throw;
            }
        }
    }
}