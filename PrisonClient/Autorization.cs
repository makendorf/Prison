using MetroFramework.Forms;
using Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrisonClient
{
    public partial class Autorization : MetroForm
    {
        public UserList[] Username;
        ENetClient Client;
        public bool Auth = false;
        public Autorization(ENetClient client)
        {
            InitializeComponent();
            Client = client;
            DialogResult = DialogResult.None;
        }

        private void Autorization_Load(object sender, EventArgs e)
        {
            NetworkPayload payload = new NetworkPayload(PacketType.RequestUserList);
            Client.Send(payload);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            
            if(metroTextBox2.Text == Username[UserComboBox.SelectedIndex].Password)
            {
                DialogResult = DialogResult.OK;
                Auth = true;
            }
            else
            {
                MessageBox.Show("неверный пароль");
            }
        }

        private void UserComboBox_DropDown(object sender, EventArgs e)
        {
            UserComboBox.DisplayMember = "DisplayName";
            UserComboBox.ValueMember = "ID";
            UserComboBox.DataSource = Username;
        }
    }
}
