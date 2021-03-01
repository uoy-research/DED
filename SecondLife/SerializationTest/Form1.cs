using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;

namespace SerializationTest
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtAdd;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.ListBox lstItems;
		private System.Windows.Forms.Button btnXmlSerialize;
		private System.Windows.Forms.Button btnXmlDeSerialize;
		private System.Windows.Forms.Button btnBinaryDeSerialize;
		private System.Windows.Forms.Button btnBinarySerialize;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnClear;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lstItems = new System.Windows.Forms.ListBox();
			this.txtAdd = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnXmlSerialize = new System.Windows.Forms.Button();
			this.btnXmlDeSerialize = new System.Windows.Forms.Button();
			this.btnBinaryDeSerialize = new System.Windows.Forms.Button();
			this.btnBinarySerialize = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.btnClear = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// lstItems
			// 
			this.lstItems.Items.AddRange(new object[] {
														  "Red",
														  "Blue",
														  "Green"});
			this.lstItems.Location = new System.Drawing.Point(56, 24);
			this.lstItems.Name = "lstItems";
			this.lstItems.Size = new System.Drawing.Size(128, 108);
			this.lstItems.TabIndex = 0;
			// 
			// txtAdd
			// 
			this.txtAdd.Location = new System.Drawing.Point(56, 136);
			this.txtAdd.Name = "txtAdd";
			this.txtAdd.Size = new System.Drawing.Size(128, 20);
			this.txtAdd.TabIndex = 1;
			this.txtAdd.Text = "Add new item";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(200, 136);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Location = new System.Drawing.Point(200, 24);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 3;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnXmlSerialize
			// 
			this.btnXmlSerialize.Location = new System.Drawing.Point(128, 16);
			this.btnXmlSerialize.Name = "btnXmlSerialize";
			this.btnXmlSerialize.Size = new System.Drawing.Size(80, 23);
			this.btnXmlSerialize.TabIndex = 4;
			this.btnXmlSerialize.Text = "Serialize";
			this.btnXmlSerialize.Click += new System.EventHandler(this.btnXmlSerialize_Click);
			// 
			// btnXmlDeSerialize
			// 
			this.btnXmlDeSerialize.Location = new System.Drawing.Point(216, 16);
			this.btnXmlDeSerialize.Name = "btnXmlDeSerialize";
			this.btnXmlDeSerialize.Size = new System.Drawing.Size(80, 23);
			this.btnXmlDeSerialize.TabIndex = 5;
			this.btnXmlDeSerialize.Text = "DeSerialize";
			this.btnXmlDeSerialize.Click += new System.EventHandler(this.btnXmlDeSerialize_Click);
			// 
			// btnBinaryDeSerialize
			// 
			this.btnBinaryDeSerialize.Location = new System.Drawing.Point(216, 16);
			this.btnBinaryDeSerialize.Name = "btnBinaryDeSerialize";
			this.btnBinaryDeSerialize.Size = new System.Drawing.Size(80, 23);
			this.btnBinaryDeSerialize.TabIndex = 7;
			this.btnBinaryDeSerialize.Text = "DeSerialize";
			this.btnBinaryDeSerialize.Click += new System.EventHandler(this.btnBinaryDeSerialize_Click);
			// 
			// btnBinarySerialize
			// 
			this.btnBinarySerialize.Location = new System.Drawing.Point(128, 16);
			this.btnBinarySerialize.Name = "btnBinarySerialize";
			this.btnBinarySerialize.Size = new System.Drawing.Size(80, 23);
			this.btnBinarySerialize.TabIndex = 6;
			this.btnBinarySerialize.Text = "Serialize";
			this.btnBinarySerialize.Click += new System.EventHandler(this.btnBinarySerialize_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.btnXmlDeSerialize);
			this.panel1.Controls.Add(this.btnXmlSerialize);
			this.panel1.Location = new System.Drawing.Point(8, 176);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(304, 56);
			this.panel1.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "XML Serialization";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.btnBinarySerialize);
			this.panel2.Controls.Add(this.btnBinaryDeSerialize);
			this.panel2.Location = new System.Drawing.Point(8, 240);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(304, 48);
			this.panel2.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "Binary Serialization";
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(200, 56);
			this.btnClear.Name = "btnClear";
			this.btnClear.TabIndex = 10;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(320, 302);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.txtAdd);
			this.Controls.Add(this.lstItems);
			this.Name = "Form1";
			this.Text = "Form1";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private ArrayList GetItemsFromListBox()
		{
			ArrayList items = new ArrayList();
			foreach ( string item in lstItems.Items )
			{
				items.Add ( item );
			}

			return items;
		}

		private void LoadItems( ArrayList items )
		{
			lstItems.Items.Clear();

			// Iterate through all items in the arraylist and populate the listbox.
			foreach ( string item in items )
			{
				lstItems.Items.Add ( item );
			}
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			// Add the item from textbox to the list.
			lstItems.Items.Add( txtAdd.Text );
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			// Remove the selected item from the listbox.
			if ( lstItems.SelectedIndex == -1 )
			{
				MessageBox.Show( "Please select an item. ", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information );
				return;
			}

			lstItems.Items.RemoveAt ( lstItems.SelectedIndex );
		}

		private void btnXmlSerialize_Click(object sender, System.EventArgs e)
		{
			// Get the list of items from the listbox and add them into an array list.
			// We will save (serialize) this array list into an XML file.
			ArrayList itemsToSerialize = GetItemsFromListBox();
			
			// Create a serializer object, which can serialize 'ArrayList' (specified as parameter)
			XmlSerializer serializer = new XmlSerializer( typeof(ArrayList) );
			
			// Serialize the ArrayList into an xml file.
			TextWriter writer = new StreamWriter( @"MyApplicationData.xml" );
			serializer.Serialize( writer, itemsToSerialize );
			writer.Close();

			MessageBox.Show ( "Data in the list box serialized into xml file 'MyApplicationData.xml'.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		private void btnBinarySerialize_Click(object sender, System.EventArgs e)
		{
			ArrayList itemsToSerialize = GetItemsFromListBox();

			Stream stream = new FileStream( @"MyApplicationData.dat", System.IO.FileMode.Create );
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize( stream, itemsToSerialize );

			stream.Close();

			MessageBox.Show ( "Data in the list box serialized into binary file 'MyApplicationData.dat'.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		private void btnXmlDeSerialize_Click(object sender, System.EventArgs e)
		{
			ArrayList itemsDeserialized;

			XmlSerializer serializer = new XmlSerializer( typeof(ArrayList) );
			TextReader reader;
			
			try
			{
				reader = new StreamReader( @"MyApplicationData.xml" );
			}
			catch ( System.IO.FileNotFoundException )
			{
				MessageBox.Show ( "Serialized data file not found.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information );
				return;
			}

			itemsDeserialized = (ArrayList) serializer.Deserialize ( reader );
			reader.Close();
			
			LoadItems( itemsDeserialized );
			MessageBox.Show ( "Data deserialized and loaded into listbox.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		private void btnBinaryDeSerialize_Click(object sender, System.EventArgs e)
		{
			ArrayList itemsDeserialized;
			Stream stream;

			try
			{
				stream = new FileStream( @"MyApplicationData.dat", System.IO.FileMode.Open );
			}
			catch ( System.IO.FileNotFoundException )
			{
				MessageBox.Show ( "Serialized data file not found.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information );
				return;
			}

			IFormatter formatter = new BinaryFormatter();
			itemsDeserialized = (ArrayList)formatter.Deserialize( stream );

			stream.Close();

			LoadItems( itemsDeserialized );
			MessageBox.Show ( "Data deserialized and loaded into listbox.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			lstItems.Items.Clear();
		}
	}
}
