﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace VulScan
{
    public partial class Form1 : Form
    {
        private List<Software> softwareList = new List<Software>(); // Liste des logiciels
        private int currentIndex = 0; // Index du logiciel actuel à afficher
        private Timer timer = new Timer(); // Timer pour afficher les logiciels un par un

        public Form1()
        {
            InitializeComponent();
            StartSoftwareScan();

            // Empêcher le redimensionnement de la fenêtre
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true; // Optionnel : Permet de garder le bouton "Minimiser"
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartSoftwareScan(); // Appeler la méthode qui lance l'analyse automatiquement
        }

        private void StartSoftwareScan()
        {
            progressBar.Value = 0;
            lblMessage.Text = "Recherche des logiciels...";

            // Effacer la liste précédente dans le label
            lblSoftwareList.Text = string.Empty;

            // Démarrer l'analyse dans un thread séparé
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    // Lister les logiciels installés
                    softwareList = GetInstalledSoftware();

                    // Mettre à jour la progression et l'interface graphique
                    Invoke(new Action(() =>
                    {
                        progressBar.Maximum = softwareList.Count;

                        // Initialiser le Timer
                        timer.Interval = 500; // Définir l'intervalle pour chaque logiciel (500 ms)
                        timer.Tick += Timer_Tick;
                        timer.Start();
                    }));

                    // Enregistrer les logiciels dans un fichier CSV
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string csvFile = Path.Combine(desktopPath, "installed_software.csv");

                    SaveToCSV(softwareList, csvFile);

                    // Mettre à jour le message de fin
                    Invoke(new Action(() =>
                    {
                        lblMessage.Text = $"Fichier CSV généré : {csvFile}";
                    }));
                }
                catch (Exception ex)
                {
                    // Gérer toute exception et afficher un message d'erreur
                    Invoke(new Action(() =>
                    {
                        lblMessage.Text = $"Erreur : {ex.Message}";
                    }));
                }
            });
        }

        // Fonction pour obtenir les logiciels installés
        private List<Software> GetInstalledSoftware()
        {
            var softwareList = new List<Software>();

            // Rechercher dans les clés de registre pour les logiciels installés
            string[] registryPaths = new string[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" // Pour 32 bits sur 64 bits
            };

            foreach (var registryPath in registryPaths)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    if (key != null)
                    {
                        foreach (var subKeyName in key.GetSubKeyNames())
                        {
                            using (var subKey = key.OpenSubKey(subKeyName))
                            {
                                var software = new Software
                                {
                                    Name = subKey.GetValue("DisplayName")?.ToString(),
                                    Version = subKey.GetValue("DisplayVersion")?.ToString(),
                                    InstallDate = subKey.GetValue("InstallDate")?.ToString(),
                                };
                                if (!string.IsNullOrEmpty(software.Name))
                                {
                                    softwareList.Add(software);
                                }
                            }
                        }
                    }
                }
            }
            return softwareList;
        }
        // Fonction pour enregistrer les logiciels dans deux fichiers CSV
        private void SaveToCSV(List<Software> softwareList, string filePath)
        {
            try
            {
                // Définir les chemins des fichiers CSV
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string microsoftAndWindowsCsvFile = Path.Combine(desktopPath, "microsoft_and_windows_software.csv");
                string otherCsvFile = Path.Combine(desktopPath, "installed_software.csv");

                // Utiliser StreamWriter pour écrire dans les fichiers CSV
                using (var writerMicrosoftAndWindows = new StreamWriter(microsoftAndWindowsCsvFile, false, System.Text.Encoding.UTF8))
                using (var writerOther = new StreamWriter(otherCsvFile, false, System.Text.Encoding.UTF8))
                {
                    // Écrire l'en-tête du CSV pour les logiciels Microsoft et Windows
                    writerMicrosoftAndWindows.WriteLine("Software,Version");

                    // Écrire l'en-tête du CSV pour les autres logiciels
                    writerOther.WriteLine("Software,Version");

                    // Parcourir la liste des logiciels et les enregistrer dans les fichiers appropriés
                    foreach (var software in softwareList)
                    {
                        // Assurez-vous que si une valeur est null, elle est remplacée par une chaîne vide
                        string name = software.Name ?? "Unknown";
                        string version = software.Version ?? "Unknown";

                        // Nettoyer et formater le nom et la version
                        name = CleanSoftwareName(name);
                        version = CleanSoftwareVersion(version);

                        // Vérifier si le nom du logiciel commence par "Microsoft" ou "Windows"
                        if (name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Windows", StringComparison.OrdinalIgnoreCase))
                        {
                            // Écrire dans le fichier CSV pour les logiciels Microsoft et Windows
                            writerMicrosoftAndWindows.WriteLine($"{name},{version}");
                        }
                        else
                        {
                            // Écrire dans le fichier CSV pour les autres logiciels
                            writerOther.WriteLine($"{name},{version}");
                        }
                    }
                }

                // Message de confirmation pour les fichiers CSV générés
                //MessageBox.Show("Les fichiers CSV ont été générés : 'microsoft_and_windows_software.csv' et 'installed_software'.", "Scan terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs liées à l'écriture du fichier
                MessageBox.Show($"Erreur lors de la sauvegarde des fichiers CSV : {ex.Message}");
            }
        }



        // Méthode pour nettoyer le nom du logiciel
        private string CleanSoftwareName(string name)
        {
            // Supprimer le texte entre parenthèses (ex : "(x64)")
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\s*\(.*?\)", string.Empty);

            // Remplacer les espaces par des tirets pour respecter le format du CSV
            name = name.Replace(" ", "-");

            return name;
        }

        // Méthode pour nettoyer la version du logiciel
        private string CleanSoftwareVersion(string version)
        {
            // Enlever les espaces dans la version et garder uniquement la partie principale de la version
            if (!string.IsNullOrEmpty(version))
            {
                version = version.Trim();

                // Par exemple, pour la version "27.0.44.217", on pourrait vouloir garder seulement "27.0.44"
                version = System.Text.RegularExpressions.Regex.Replace(version, @"\.\d+$", "");
            }
            return version;
        }

        // Méthode pour formater le nom du logiciel et la version en format CPE
        private string FormatCPEName(string name, string version)
        {
            // Remplacer les espaces par des tirets (CPE ne supporte pas les espaces)
            name = name.Replace(" ", "-");

            // Formater la version pour qu'elle corresponde au format souhaité
            // Si la version n'est pas au bon format, nous devons peut-être la modifier ici
            version = version.Replace(" ", "-"); // Remplacer les espaces dans la version également

            // Retourner le CPE dans le format attendu
            return $"{name},{version}";
        }

        // Méthode pour supprimer le texte entre parenthèses (ex : "(x64)")
        private string RemoveTextInParentheses(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Utiliser une expression régulière pour supprimer tout texte entre parenthèses
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s*\(.*?\)", string.Empty);
        }

        // Méthode pour supprimer les numéros de version dans le nom du logiciel
        private string RemoveVersionFromName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Utiliser une expression régulière pour enlever les versions typiques (comme "24.09")
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s*\d+\.\d+(\.\d+)*", string.Empty).Trim();
        }



        // Méthode pour gérer le Timer_Tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentIndex < softwareList.Count)
            {
                // Effacer le label pour afficher uniquement le logiciel en cours d'analyse
                lblSoftwareList.Text = string.Empty;

                // Ajouter un seul logiciel à la liste dans le label
                var software = softwareList[currentIndex];
                lblSoftwareList.Text = $"{software.Name} (Version: {software.Version})";

                // Mettre à jour la barre de progression
                progressBar.Value = currentIndex + 1;

                currentIndex++;
            }
            else
            {
                // Arrêter le timer lorsque tous les logiciels sont affichés
                timer.Stop();

                // Afficher le message box après la fin du scan
                MessageBox.Show("Le scan est terminé. L'analyse des logiciels est terminée.", "Scan terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);  // Force la fermeture de l'application
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // Ouvrir Form2 pour effectuer la recherche
            Form2 searchForm = new Form2();
            searchForm.Show();
        }
    }

    // Classe représentant un logiciel installé
    public class Software
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string InstallDate { get; set; }
    }
}
