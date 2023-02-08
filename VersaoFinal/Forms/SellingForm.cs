﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DGVPrinterHelper;
using System.IO;
using Bunifu.UI.WinForms;
using VersaoFinal.Classes;
using VersaoFinal.Forms;
using VersaoFinal;

namespace VersaoFinal.Forms
{

    public partial class SellingForm : Form
    {
        private string receiptName;
        int grandTotal = 0, n = 0, todayAmount = 0;
        DBConnect dBCon = new DBConnect();
        DGVPrinter printer = new DGVPrinter();
        Boolean totalAmountAdd = false;
        public SellingForm()
        {
            InitializeComponent();
            bunifuFormDock1.SubscribeControlToDragEvents(panel1);
        }

        private void Print(string name, DataGridView grid)
        {

            printer.Title = name;
            printer.SubTitle = String.Format("Date : {0}", DateTime.Now);
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Near;
            printer.Footer = receiptName;
            printer.FooterSpacing = 15;
            printer.printDocument.DefaultPageSettings.Landscape = true;
            printer.PrintDataGridView(grid);


        }
        private void getCategory()
        {
            string selectQuery = "SELECT * FROM Category";
            SqlCommand cmd = new SqlCommand(selectQuery, dBCon.GetCon());
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            CategoryDropdown.DataSource = table;
            CategoryDropdown.ValueMember = "Nome";

        }

        private void getTable()
        {
            string selectQuery = "SELECT Nome,Preço,Quantidade FROM Product";
            SqlCommand cmd = new SqlCommand(selectQuery, dBCon.GetCon());
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            DataGridViewProduct.DataSource = table;
        }

        private void getSellTable(string name)
        {
            string selectQuery = "SELECT * FROM Bill WHERE Nome ='" + name + "'";
            SqlCommand cmd = new SqlCommand(selectQuery, dBCon.GetCon());
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            DataGridViewSellList.DataSource = table;
        }



        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (labelShowSellerName.Text == "Admin")
            {
                AdminForm adminForm = new AdminForm();
                adminForm.Show();
                this.Hide();
            }
            else
            {
                Login loginForm = new Login();
                loginForm.Show();
                this.Hide();
            }
        }

        private void SellingForm_Load(object sender, EventArgs e)
        {
            Login lg = new Login();
            labelShowSellerName.Text = lg.sellerName;
            labelDate.Text = DateTime.Today.ToShortDateString();
            getTable();
            getCategory();
            getSellTable(labelShowSellerName.Text);
            receiptName = File.ReadAllText(@"ReceiptName.vf");
        }

        private void DataGridViewProduct_Click(object sender, EventArgs e)
        {
            TextBoxQuantity.Clear();
            TextBoxName.Text = DataGridViewProduct.SelectedRows[0].Cells[0].Value.ToString();
            TextBoxPrice.Text = DataGridViewProduct.SelectedRows[0].Cells[1].Value.ToString();
        }

        private void TextBoxQuantity_Click(object sender, EventArgs e)
        {
            LabelMessage.Text = "";
        }

        private void TextBoxName_Click(object sender, EventArgs e)
        {
            LabelMessage.Text = "";
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {

                string insertQuery = "INSERT INTO Bill VALUES(" + TextBoxBillID.Text + ",'" + labelShowSellerName.Text + "','" + labelDate.Text + "'," + grandTotal.ToString() + ")";
                SqlCommand cmd = new SqlCommand(insertQuery, dBCon.GetCon());
                dBCon.OpenCon();
                cmd.ExecuteNonQuery();
                LabelMessageBill.ForeColor = Color.Green;
                LabelMessageBill.Text = ("Pedido adicionado com sucesso!");
                dBCon.CloseCon();
                getSellTable(labelShowSellerName.Text);


            }
            catch
            {
                LabelMessageBill.ForeColor = Color.Red;
                LabelMessageBill.Text = "Falha em adicionar o pedido";
            }
        }

        private void TextBoxBill_Click(object sender, EventArgs e)
        {
            LabelMessageBill.Text = "";
        }

        private void ButtonPrint_Click(object sender, EventArgs e)
        {
            if (DropdownPrint.SelectedIndex < 0)
            {
                LabelMessageBill.ForeColor = Color.Red;
                LabelMessageBill.Text = "Por favor seleciona o que imprimir";
            }
            else
            {
                LabelMessageBill.Text = "";
                if (DropdownPrint.SelectedItem.ToString() == "Pedido")
                {
                    if (totalAmountAdd == false)
                    {
                        LabelMessageBill.ForeColor = Color.Red;
                        LabelMessageBill.Text = "Total não adicionado";
                    }
                    else
                    {
                        Print("Receipt", DataGridViewOrder);
                    }
                }
                else
                {
                    Print("Seller Bills", DataGridViewSellList);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DataGridViewRow addRow = new DataGridViewRow();
            addRow.CreateCells(DataGridViewOrder);
            addRow.Cells[0].Value = "";
            addRow.Cells[1].Value = "";
            addRow.Cells[2].Value = "";
            addRow.Cells[3].Value = "";
            addRow.Cells[4].Value = labelAmount.Text;
            DataGridViewOrder.Rows.Add(addRow);
            totalAmountAdd = true;
            todayAmount = todayAmount + grandTotal;
            LabelTodayAmount.Text = todayAmount.ToString() + "$";
        }

        private void CategoryDropdown_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectQuery = "SELECT Nome,Preço,Quantidade FROM Product WHERE Categoria ='" + CategoryDropdown.SelectedValue.ToString() + "'";
            SqlCommand cmd = new SqlCommand(selectQuery, dBCon.GetCon());
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            DataGridViewProduct.DataSource = table;
        }

        private void ButtonAddSellers_Click(object sender, EventArgs e)
        {
            getTable();
        }

        private void ButtonClearOrder_Click(object sender, EventArgs e)
        {
            DataGridViewOrder.Rows.Clear();
            grandTotal = 0;
            labelAmount.Text = "0$";
            totalAmountAdd = false;
        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            getSellTable(labelShowSellerName.Text);
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ButtonAddOrder_Click(object sender, EventArgs e)
        {
            int quantity = 0;
            dBCon.OpenCon();

            string selectQuery = "SELECT Quantidade FROM Product WHERE Nome='" + TextBoxName.Text + "'";
            SqlCommand cmd = new SqlCommand(selectQuery, dBCon.GetCon());
            quantity = (int)cmd.ExecuteScalar();




            if (TextBoxName.Text == "" || TextBoxQuantity.Text == "")
            {
                LabelMessage.Text = "Sem informação";
            }
            else
            {
                if (int.Parse(TextBoxQuantity.Text) > quantity)
                {
                    LabelMessage.Text = "Fora de Estoque " + quantity + " disponível";
                }
                else
                {
                    int Qty = int.Parse(TextBoxQuantity.Text);

                    int Total = Convert.ToInt32(TextBoxPrice.Text) * Convert.ToInt32(TextBoxQuantity.Text);
                    DataGridViewRow addRow = new DataGridViewRow();
                    addRow.CreateCells(DataGridViewOrder);
                    addRow.Cells[0].Value = n++;
                    addRow.Cells[1].Value = TextBoxName.Text;
                    addRow.Cells[2].Value = TextBoxPrice.Text;
                    addRow.Cells[3].Value = TextBoxQuantity.Text;
                    addRow.Cells[4].Value = Total;
                    DataGridViewOrder.Rows.Add(addRow);
                    grandTotal += Total;
                    labelAmount.Text = grandTotal + " $";

                    selectQuery = "UPDATE Product SET Quantidade = Quantidade-" + TextBoxQuantity.Text + " WHERE Nome ='" + TextBoxName.Text + "'";
                    cmd = new SqlCommand(selectQuery, dBCon.GetCon());
                    cmd.ExecuteNonQuery();
                    getTable();
                }


            }

        }
    }
}