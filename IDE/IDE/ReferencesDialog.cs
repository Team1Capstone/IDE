using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDE
{
    public partial class ReferencesDialog : Form
    {
        public ReferencesDialog()
        {
            InitializeComponent();
        }

        private void ReferencesDialog_Load(object sender, EventArgs e)
        {
//            listBox1.DataSource;
            // Determine the path of the GAC
            var fileInfo = new FileInfo(typeof(Object).Assembly.Location);
            var dirInfo = fileInfo.Directory;

            // Enumerate files in GAC
            foreach (var file in dirInfo.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var asm = Assembly.ReflectionOnlyLoadFrom(file.FullName);

                    /*                    listBox1.Items.Add(new
                                        {
                                            Name = asm.GetName().Name,
                                            Version = asm.GetName().Version.ToString()
                                        });*/

                    listBox1.Items.Add(asm.GetName().Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
//                listBox1.Items.Add(file.Name);
            }            
        }
    }
}
