﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Reflection;
using System.Diagnostics;

namespace FinalQA_Project
{
    public partial class AddBirdForm : MaterialForm
    {
        public AddBirdForm()
        {
            InitializeComponent();
        }

        //
        // Species Combo Box mapping
        //
        // Define a dictionary to store the mapping between species and subspecies
        Dictionary<string, List<string>> subspeciesMapping = new Dictionary<string, List<string>>() {
            { "American Gouldian", new List<string> { "North America", "Central America", "South America" } },
            { "European Gouldian", new List<string> { "Eastern Europe", "Western Europe" } },
            { "Australian Gouldian", new List<string> { "Central Australia", "Coastal cities" } }
        };
        Dictionary<string, List<string>> BodyColorMapping = new Dictionary<string, List<string>>() {
            {"Female",new List<string> { "Green","Yellow","Silver","Blue"} },
            {"Male",new List<string>{"Green","Silver","Yellow","Blue","Pastel", "Diluted" } }
        };
        

        // Event handler for the "Species" ComboBox selection
        private void SpeciesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear the items in the "Subspecies" ComboBox
            SubspeciesComboBox.Items.Clear();

            // Get the selected species from the "Species" ComboBox
            string selectedSpecies = SpeciesComboBox.SelectedItem.ToString();

            // Check if the selected species exists in the mapping dictionary
            if (subspeciesMapping.ContainsKey(selectedSpecies))
            {
                // Populate the "Subspecies" ComboBox with the corresponding subspecies
                List<string> subspecies = subspeciesMapping[selectedSpecies];
                SubspeciesComboBox.Items.AddRange(subspecies.ToArray());
            }

        }
        private void BodyColorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BodyColorComboBox.Items.Clear();

            // Get the selected species from the "Species" ComboBox
            string selectedGender = GenderComboBox.SelectedItem.ToString();

            // Check if the selected species exists in the mapping dictionary
            if (BodyColorMapping.ContainsKey(selectedGender))
            {
                // Populate the "Subspecies" ComboBox with the corresponding subspecies
                List<string> subspecies = BodyColorMapping[selectedGender];
                BodyColorComboBox.Items.AddRange(subspecies.ToArray());
            }
        }

        public void AddBirdButton_Click(object sender, EventArgs e)
        {
            // Capture the data entered by the user
            string serialNumber = SerialNumberTextBox.Text;
            string species = SpeciesComboBox.Text;
            string subspecies = SubspeciesComboBox.Text;
            DateTime hatchingDate = HatchingDateTimePicker.Value;
            string gender = GenderComboBox.Text;
            string cageSerialNumber = CageSerialNumberTextBox.Text;
            string fatherSerialNumber = FatherSerialNumberTextBox.Text;
            string motherSerialNumber = MotherSerialNumberTextBox.Text;
            string headColor = HeadColorComboBox.Text;
            string breastColor = BreastColorComboBox.Text;
            string bodyColor = BodyColorComboBox.Text;


            // Validate the input data
            bool inputValid = ValidateInput(serialNumber, species, subspecies, gender, cageSerialNumber, fatherSerialNumber, motherSerialNumber, headColor, breastColor, bodyColor);
            if (!inputValid) { return; }

            try
            {
                // Create a new Excel Application object
                Application excelApp = new Application();

                // Open the Excel workbook containing the habitat information
                Workbook workbook = excelApp.Workbooks.Open(@"C:\Users\vladi\source\repos\FinalQA Project\FinalQA Project\Birds habitat.xlsx");

                // Get the Worksheet object for the sheet containing the habitat information
                Worksheet worksheetBird = (Worksheet)workbook.Worksheets["Birds"];
                Worksheet worksheetCage = (Worksheet)workbook.Worksheets["Cages"];

                // Check if the serial number already exists in the worksheet
                Range serialNumberRange = worksheetBird.Range["A:A"].Find(serialNumber, Missing.Value, XlFindLookIn.xlValues, XlLookAt.xlWhole, XlSearchOrder.xlByRows, XlSearchDirection.xlNext, false, Missing.Value, Missing.Value);
                Range cageSerialNumberRange = worksheetCage.Range["A:A"].Find(cageSerialNumber, Missing.Value, XlFindLookIn.xlValues, XlLookAt.xlWhole, XlSearchOrder.xlByRows, XlSearchDirection.xlNext, false, Missing.Value, Missing.Value);

                if (serialNumberRange != null)
                {
                    MessageBox.Show("Error!\nSerial number already exists.", "Duplicate serial number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClosingAll(excelApp, workbook, worksheetBird, worksheetCage);
                    return;
                }

                // Check if the cage serial number exists in the worksheet
                if (cageSerialNumberRange == null)
                {
                    MessageBox.Show("Error!\nCage serial number not exists.", "Invalid cage serial number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClosingAll(excelApp, workbook, worksheetBird, worksheetCage);
                    return;
                }

                // Add the new bird to the worksheet
                int lastRow = worksheetBird.UsedRange.Rows.Count + 1;

                worksheetBird.Cells[lastRow, 1] = serialNumber;
                worksheetBird.Cells[lastRow, 2] = species;
                worksheetBird.Cells[lastRow, 3] = subspecies;
                worksheetBird.Cells[lastRow, 4] = hatchingDate;
                worksheetBird.Cells[lastRow, 5] = gender;
                worksheetBird.Cells[lastRow, 6] = cageSerialNumber;
                worksheetBird.Cells[lastRow, 7] = fatherSerialNumber;
                worksheetBird.Cells[lastRow, 8] = motherSerialNumber;
                worksheetBird.Cells[lastRow, 9] = headColor;
                worksheetBird.Cells[lastRow, 10] = breastColor;
                worksheetBird.Cells[lastRow, 11] = bodyColor;



                // Close the workbook and the Excel application
                
                MessageBox.Show("New bird added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                ClosingAll(excelApp, workbook, worksheetBird, worksheetCage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding new bird: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Close the workbook and the Excel application
        private void ClosingAll(Application excelApp, Workbook workbook, Worksheet worksheetBird, Worksheet worksheetCage) {
            workbook.Save();
            workbook.Close();
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheetBird);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheetCage);

        }
        private bool ValidateInput(string serialNumber, string species, string subspecies, string gender, string cageSerialNumber, string fatherSerialNumber, string motherSerialNumber, string headColor, string breastColor, string bodyColor)
        {

            /////////////////// serialNumber ///////////////////
            if (string.IsNullOrEmpty(serialNumber))
            {
                MessageBox.Show("Please enter serial number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check if the serial number is a valid integer
            if (!IsNumeric(serialNumber))
            {
                MessageBox.Show("Please enter valid serial number (numbers only).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            /////////////////// species ///////////////////
            if (string.IsNullOrEmpty(species))
            {
                MessageBox.Show("Please enter species.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            /////////////////// subspecies ///////////////////
            if (string.IsNullOrEmpty(subspecies))
            {
                MessageBox.Show("Please enter subspecies.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            




            /////////////////// hatchingDate ///////////////////





            /////////////////// gender ///////////////////
            if (string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Please enter gender.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            /////////////////// cageSerialNumber ///////////////////
            if (string.IsNullOrEmpty(cageSerialNumber))
            {
                MessageBox.Show("Please enter cage serial number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // Check if the cageSerialNumber is a valid string (letters and numbers)
            if (!IsAlphabeticAndNumeric(cageSerialNumber))
            {
                MessageBox.Show("Please enter valid cage serial number (letters and numbers only).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }




            /////////////////// fatherSerialNumber ///////////////////
            if (string.IsNullOrEmpty(fatherSerialNumber))
            {
                MessageBox.Show("Please enter father's serial number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check if the serial number is a valid integer
            if (!IsNumeric(fatherSerialNumber))
            {
                MessageBox.Show("Please enter valid father's serial number (numbers only).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }





            /////////////////// motherSerialNumber ///////////////////
            if (string.IsNullOrEmpty(motherSerialNumber))
            {
                MessageBox.Show("Please enter mother's serial number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check if the serial number is a valid integer
            if (!IsNumeric(motherSerialNumber))
            {
                MessageBox.Show("Please enter valid mother's serial number (numbers only).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            // Check if the mothers or fathers serial number different from the bird itself
            if (motherSerialNumber == serialNumber || serialNumber  == fatherSerialNumber)
            {
                MessageBox.Show("Parent's serial numbers cannot be the same the as the bird you want to add.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check if the mothers and the fathers serial number not the same
            if (motherSerialNumber == fatherSerialNumber)
            {
                MessageBox.Show("Parents' serial numbers cannot be the same.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            /////////////////// headColor ///////////////////
            if (string.IsNullOrEmpty(headColor))
            {
                MessageBox.Show("Please enter head color.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }





            /////////////////// breastColor ///////////////////
            if (string.IsNullOrEmpty(breastColor))
            {
                MessageBox.Show("Please enter breast color.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }





            /////////////////// bodyColor ///////////////////
            if (string.IsNullOrEmpty(bodyColor))
            {
                MessageBox.Show("Please enter body color.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }




            return true;
        }


        // Returns true if the input string contains only alphabetical characters
        public bool IsAlphabetic(string s)
        {
            if (s == null || s.Length == 0) return false;
            foreach (char c in s)
            {
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')))
                {
                    return false;
                }
            }
            return true;
        }

        // Returns true if the input string contains only numeric characters
        public bool IsNumeric(string s)
        {
            if (s == null || s.Length == 0) return false;
            foreach (char c in s)
            {
                if (!(c >= '0' && c <= '9'))
                {
                    return false;
                }
            }
            return true;
        }


        // Returns true if the input string contains only alphabetical characters and numeric characters
        public static bool IsAlphabeticAndNumeric(string s)
        {
            if (s == null || s.Length < 1) return false;

            bool hasLetter = false;
            bool hasNumber = false;

            foreach (char c in s)
            {
                if (Char.IsLetter(c))
                {
                    hasLetter = true;
                }
                else if (Char.IsDigit(c))
                {
                    hasNumber = true;
                }
                else
                {
                    // If a character is not a letter or a number, the input is invalid
                    return false;
                }
            }

            return hasLetter && hasNumber;  
        }
        private void YourForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseExcelProcesses();
        }

        private void CloseExcelProcesses()
        {
            // Get all running Excel processes
            Process[] processes = Process.GetProcessesByName("EXCEL");

            // Close each Excel process
            foreach (Process process in processes)
            {
                process.CloseMainWindow();
                process.Close();
            }
        }


    }

}
