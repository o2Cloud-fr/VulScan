using System;
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

        // Fonction pour enregistrer les logiciels dans un fichier CSV
        private void SaveToCSV(List<Software> softwareList, string filePath)
        {
            try
            {
                // Utiliser StreamWriter pour écrire dans un fichier CSV
                using (var writer = new StreamWriter(filePath))
                {
                    // Écrire l'en-tête du CSV
                    writer.WriteLine("Nom,Version,Date d'installation");

                    // Parcourir la liste des logiciels et écrire les informations dans chaque ligne
                    foreach (var software in softwareList)
                    {
                        // Assurez-vous que si une valeur est null, elle est remplacée par une chaîne vide
                        string name = software.Name ?? "A";
                        string version = software.Version ?? "B";
                        string installDate = software.InstallDate ?? "C";

                        // Écrire les données dans les colonnes A, B, et C
                        writer.WriteLine($"{name},{version},{installDate}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer les erreurs liées à l'écriture du fichier
                MessageBox.Show($"Erreur lors de la sauvegarde du fichier CSV : {ex.Message}");
            }
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
