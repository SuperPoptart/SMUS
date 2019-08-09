using SFML.Tools;
using SFML.Window;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SMUS.Module
{
    class AddSource : Module
    {
        private readonly Button button;

        public AddSource()
        {
            button = new Button(new BatchedSprite(new Vector2f(0, 0), Program.AtlasData["add"], 4));
            button.sprite.Position = new Vector2f(Program.Window.Size.X - Program.Window.Size.X / 4 + 4, Program.Window.Size.Y - (button.sprite.AtlasPosition.Height + button.sprite.AtlasPosition.Height / 6));
            button.sprite.Colour = Config.Colors["buttonsfaded"];
            button.shadowSprite.Position = button.sprite.Position += new Vector2f(0, 1);

            button.OnPress += () =>
            {
                string selectedPath = "hold";
                var t = new Thread((ThreadStart)(() =>
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
                    fbd.ShowNewFolderButton = true;
                    if (fbd.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                    selectedPath = fbd.SelectedPath;
                }));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
                if (!selectedPath.Equals("hold"))
                {
                    string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Resources\\Config\\config.xml";
                    XDocument document = XDocument.Load(path);
                    XElement root = document.Element("config");
                    IEnumerable<XElement> rows = root.Descendants("musicdir").Descendants("dir");
                    XElement firstRow = rows.First();
                    firstRow.AddBeforeSelf(
                        new XElement("dir", selectedPath));
                    document.Save(path);
                    Program.Window.Close();
                }
            };
        }

        public override void Update()
        {
            button.sprite.Colour = Config.Colors["buttonsfaded"];
            Program.SpriteBatch.CalculateVertices();

        }
    }
}
