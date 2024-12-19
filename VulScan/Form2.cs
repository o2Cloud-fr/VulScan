using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace VulScan
{
    public partial class Form2 : Form
    {
        private List<Software> softwareList = new List<Software>(); // Liste des logiciels
        private string csvFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "installed_software.csv");

        // Membres de la classe pour TextBox et ListBox
        private TextBox txtSearch;
        private ListBox lbResults;

        public Form2()
        {
            InitializeComponent();
            LoadSoftwareList();
            InitializeUIComponents(); // Initialisation des contrôles personnalisés

            // Empêcher le redimensionnement de la fenêtre
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true; // Optionnel : Permet de garder le bouton "Minimiser"
        }

        private void InitializeUIComponents()
        {
            // Création du TextBox pour la recherche
            txtSearch = new TextBox
            {
                Width = 200
            };

            // Calculer la position pour centrer le TextBox horizontalement sur la Form
            int xPos = (this.ClientSize.Width - txtSearch.Width) / 2;
            int yPos = 25; // Vous pouvez ajuster la position verticale si nécessaire

            txtSearch.Location = new Point(xPos, yPos);

            txtSearch.TextChanged += OnSearchTextChanged;

            // Création de la ListBox pour afficher les résultats de la recherche
            lbResults = new ListBox
            {
                Location = new Point(10, 50),
                Width = 500,
                Height = 300
            };

            this.Controls.Add(txtSearch);
            this.Controls.Add(lbResults);
        }

        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();

            // Si le champ de recherche est vide, on efface les résultats
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                lbResults.Items.Clear();
                return;
            }

            // Rechercher et afficher les résultats
            List<Software> filteredSoftware = SearchSoftware(searchTerm);
            DisplayResults(filteredSoftware);
        }

        private List<Software> SearchSoftware(string searchTerm)
        {
            return softwareList.Where(s => s.Name.ToLower().Contains(searchTerm)).ToList();
        }

        private void DisplayResults(List<Software> software)
        {
            lbResults.Items.Clear(); // Effacer les anciens résultats

            foreach (var s in software)
            {
                lbResults.Items.Add($"{s.Name} (Version: {s.Version})");
            }
        }

        private void LoadSoftwareList()
        {
            if (File.Exists(csvFilePath))
            {
                var lines = File.ReadAllLines(csvFilePath);

                foreach (var line in lines.Skip(1)) // Ignorer l'en-tête
                {
                    var values = line.Split(',');

                    if (values.Length == 3)
                    {
                        var software = new Software
                        {
                            Name = values[0].Trim(),
                            Version = values[1].Trim(),
                            InstallDate = values[2].Trim()
                        };

                        softwareList.Add(software);
                    }
                }
            }
            else
            {
                MessageBox.Show("Le fichier CSV n'existe pas.");
            }
        }
    }

    // Classe représentant un logiciel
    public class Software2
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string InstallDate { get; set; }
    }
}
