using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Stomatolog
{
    public partial class FormNovaUsluga : Form
    {
        public FormNovaUsluga()
        {
            InitializeComponent();
        }

        private void pacijentBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.pacijentBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.stomatologDataSet);

        }

        private void FormNovaUsluga_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'stomatologDataSet.usluga' table. You can move, or remove it, as needed.
            this.uslugaTableAdapter.Fill(this.stomatologDataSet.usluga);
            // TODO: This line of code loads data into the 'stomatologDataSet.termin' table. You can move, or remove it, as needed.
            //this.terminTableAdapter.Fill(this.stomatologDataSet.termin);
            // TODO: This line of code loads data into the 'stomatologDataSet.pacijent' table. You can move, or remove it, as needed.
            //this.pacijentTableAdapter.Fill(this.stomatologDataSet.pacijent);

        }

        private void btnPretraga_Click(object sender, EventArgs e)
        {
            txtDijagnoza.Clear();
            //lblZakazani_termin.ResetText();
            lblProvjera_termina.Text = "Prazno";
            this.pacijentTableAdapter.FillBy(this.stomatologDataSet.pacijent, txtIme.Text, txtPrezime.Text);
            if (this.stomatologDataSet.pacijent.Rows.Count > 0)
                pPregled.Visible = true;
            else
            {
                pPregled.Visible = false;
                lblInfo.Text = "Nema rezultata za tražene podatke!";
            }
            this.pruzenaTableAdapter1.Fill(this.stomatologDataSet.pruzena);
            this.terminTableAdapter.Fill(this.stomatologDataSet.termin);
            for (int i = 0; i < this.stomatologDataSet.termin.Count; i++)
                for (int j = 0; j < this.stomatologDataSet.pruzena.Count; j++)
                    if (this.stomatologDataSet.pruzena[j].datVrijeme == this.stomatologDataSet.termin[i].datVrijeme)
                        this.stomatologDataSet.termin.Rows.Remove(this.stomatologDataSet.termin[i]);

            this.uslugaTableAdapter.Fill(this.stomatologDataSet.usluga);

        }

        private void pacijentDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            lblZakazani_termin.Text = DateTime.Now.ToShortDateString();
        }


        private void terminDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblZakazani_termin.Text = terminDataGridView[0, e.RowIndex].Value.ToString();
                lblZakazani_termin.Text = terminDataGridView[0, e.RowIndex].Value.ToString();
                txtDijagnoza.Text = terminDataGridView[1, e.RowIndex].Value.ToString();
                lblProvjera_termina.Text = "Nije prazno";
            }
        
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            pRacun.Visible = false;
            lblProvjera_termina.Text = "Prazno";
            lblInfo.Text = "Unesite podatke za pretraživanje.";
            this.uslugaTableAdapter.Fill(this.stomatologDataSet.usluga);
            txtDijagnoza.Clear();
            lblProvjera_termina.Text = "Prazno";
            this.pregled.Rows.Clear();
            lblZakazani_termin.Text = DateTime.Now.ToShortDateString();

            lblSifra_usluge_rez.Text = "";
            lblNaziv_usluge_rez.Text = "";
            lblGrupa_usluga_rez.Text = "";
            lblCijena_rez.Text = "";
            lblDatum_rez.Text = "";
            
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            pregled.Rows.Add();
            int red = pregled.RowCount - 1;
            
            if (lblProvjera_termina.Text == "Prazno")
            {
                lblZakazani_termin.Text = DateTime.Now.ToString();
                this.pregled["Novi_termin", red].Value = "Da";
            }
            else
                this.pregled["Novi_termin", pregled.RowCount - 1].Value = "Ne";

            this.pregled["Zakazani_termin", red].Value = lblZakazani_termin.Text;
            this.pregled["Sifra_pacijenta", red].Value = lblSif_pac.Text;
            this.pregled["Dijagnoza", red].Value = txtDijagnoza.Text;
            this.pregled["Usluga", red].Value = cmbUsluga.SelectedValue;
            txtDijagnoza.Clear();

            lblZakazani_termin.Text = DateTime.Now.ToShortDateString();
            lblProvjera_termina.Text = "Prazno";
                          
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnBrisi_Click(object sender, EventArgs e)
        {
            pregled.Rows.Remove(pregled.CurrentRow);
        }

        private void btnPotvrdi_Click_1(object sender, EventArgs e)
        {
            int total = 0;
            for (int i = 0; i < pregled.Rows.Count; i++)
            {
                if (pregled["Novi_termin", i].Value.ToString() == "Da") 
                {
                    this.terminTableAdapter.Insert(Convert.ToDateTime(this.pregled["Zakazani_termin",i].Value.ToString()),
                        Convert.ToInt32(this.pregled["Sifra_pacijenta", i].Value.ToString()),
                        this.pregled["Dijagnoza", i].Value.ToString());
                }
                this.pruzenaTableAdapter1.Insert(Convert.ToDateTime(this.pregled["Zakazani_termin", i].Value.ToString()),
                    Convert.ToInt32(this.pregled["Usluga", i].Value.ToString()),
                    DateTime.Now);
  
                this.uslugaTableAdapter.FillBy(this.stomatologDataSet.usluga, Convert.ToInt32(this.pregled["Usluga", i].Value.ToString()));
                int grupa = Convert.ToInt32(this.stomatologDataSet.usluga[0].sifGrupUsluga);
                this.grupaTableAdapter1.FillBy(this.stomatologDataSet.grupa, grupa);

                lblSifra_usluge_rez.Text += '\n' + this.stomatologDataSet.usluga[0].sifUsluga.ToString();
                lblNaziv_usluge_rez.Text += '\n' + this.stomatologDataSet.usluga[0].nazUsluga.ToString();
                lblGrupa_usluga_rez.Text += '\n' + this.stomatologDataSet.grupa[0].nazGrupUsluga.ToString();
                lblCijena_rez.Text += '\n' + this.stomatologDataSet.usluga[0].cijena.ToString() + " KM";
                total += this.stomatologDataSet.usluga[0].cijena;
                
            }

            lblDatum_rez.Text = DateTime.Now.ToString();
            
            lblCijena_rez.Text += '\n' + "-------------";
            lblGrupa_usluga_rez.Text +=  "\n\n                                  Ukupno:";
            lblCijena_rez.Text += '\n' + total.ToString() + " KM";

            pPregled.Visible = false;
            pRacun.Visible = true;

        }

      
    }
}